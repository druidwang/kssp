using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using com.Sconit.Entity.SI.EDI_Ford;
using log4net;
using System.Reflection;
using NPOI.HSSF.UserModel;
using System.Collections;
using NPOI.SS.UserModel;
using com.Sconit.Entity.SI;
using com.Sconit.Utility;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.SCM;
using Castle.Services.Transaction;
using com.Sconit.Entity.Exception;
using System.Net.Mail;
using com.Sconit.Service.MRP;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Service.SI.Impl
{
    [Transactional]
    public class EDI_ScheduleMgrImpl : BaseMgr, IEDI_ScheduleMgr
    {
        //public IGenericMgr siGenericMgr { get; set; }
        #region
        private static ILog log = LogManager.GetLogger("EDI.Schedule");
        #endregion

        public void LoadEDI()
        {
            string sourceFilePath = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.FordEdiFileFolder);
            string bakFilePath = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.FordEdiBakFolder);
            string errorFilePath = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.FordEdiErrorFolder);
            ExcelHelper.ProcessPath(ref sourceFilePath);
            log.Debug("FordEdiFileFolder:" + sourceFilePath);
            log.Debug("FordEdiBakFolder:" + bakFilePath);
            log.Debug("FordEdiErrorFolder:" + errorFilePath);

            try
            {
                string[] files = null;
                if(Directory.Exists(sourceFilePath))
                {
                    files = Directory.GetFiles(sourceFilePath);
                }
                else
                {
                    throw new Exception("文件地址不存在");
                }

                if(files != null && files.Length > 0)
                {
                    #region 导入/备份
                    List<string> errorlogs = new List<string>();
                    var fileNames = files.OrderBy(f => f);
                    foreach(string filePath in fileNames)
                    {
                        string fileName = Path.GetFileName(filePath);
                        try
                        {
                            ExcelHelper.ProcessPath(ref bakFilePath);
                            if(!Directory.Exists(bakFilePath))
                            {
                                Directory.CreateDirectory(bakFilePath);
                            }
                            string bakFile = DateTime.Now.ToString("yyyyMMddTHHmm") + "." + fileName;

                            Stream stream = File.OpenRead(filePath);
                            this.ImportSchedule(stream, bakFile);

                            File.Move(filePath, Path.Combine(bakFilePath, bakFile));
                        }
                        catch(Exception ex)
                        {
                            ExcelHelper.ProcessPath(ref errorFilePath);
                            string errFile = DateTime.Now.ToString("yyyyMMddTHHmm") + "." + fileName;
                            File.Move(filePath, Path.Combine(errorFilePath, errFile));
                            log.Error("导入读取EDI文件时出错:" + errFile, ex);
                            SendErrorEmail("导入读取EDI文件时出错", ex);
                        }
                    }

                    #endregion
                }
                else
                {
                    //SendAlert(importMethod);
                    log.Info("No files found to process.");
                }
            }
            catch(Exception ex)
            {
                log.Error("EDI导入失败", ex);
                SendErrorEmail("EDI导入失败", ex);
            }
        }

        public void ImportSchedule(Stream inputStream, string fileName)
        {
            try
            {
                log.Info("ReadCustomerOrder Start");
                if(inputStream == null || inputStream.Length == 0)
                {
                    return;
                }

                FlatFileReaderHelper reader = new FlatFileReaderHelper(inputStream);

                string[] firstLine = reader.ReadLine();

                if(firstLine[0] == "830")
                {
                    PlanningScheduleMaster planningScheduleMaster = new PlanningScheduleMaster();
                    planningScheduleMaster.FileName = fileName;
                    planningScheduleMaster.FileType = firstLine[0];
                    planningScheduleMaster.ReceiverID = firstLine[5];
                    planningScheduleMaster.SenderID = firstLine[3];
                    this.genericMgr.Create(planningScheduleMaster);

                    List<PlanningScheduleDetail> planningScheduleDetailList = new List<PlanningScheduleDetail>();
                    #region 定义
                    int ControlNum = 1;
                    int ReleaseNum = 2;
                    int Purpose = 3;
                    int StartDate = 4;
                    int EndDate = 5;
                    int ForecastType = 6;
                    int ScheduleQty = 7;
                    int ReleaseDate = 8;
                    int ShipTo = 9;
                    int ShipFrom = 10;
                    int Item = 11;
                    int Po = 12;
                    int Uom = 13;
                    int ContactName = 14;
                    int ContactTelephone = 15;
                    int DeliverPattern = 16;
                    int DeliveryPatternTime = 17;
                    int FI = 18;
                    int FIEndDate = 19;
                    int FICumQty = 20;
                    int FIStartDate = 21;
                    int MT = 22;
                    int MTEndDate = 23;
                    int MTCumQty = 24;
                    int MTStartDate = 25;
                    int LastIpQty = 26;
                    int LastIpDate = 27;
                    int IpCumQty = 28;
                    int IpEndDate = 29;
                    int LastIpNo = 30;
                    int ScheduleTiming = 31;
                    int ScheduleWhen = 32;
                    int Qty = 33;
                    int CumQty = 34;

                    //byte endColIndex = 34;
                    int startRow = 3;

                    #endregion
                    log.Info("Start txt customerorder inbound ");

                    int i = 2;
                    for(string[] lineData = reader.ReadLine(); lineData != null; lineData = reader.ReadLine())
                    {
                        if(i >= startRow)
                        {
                            if(lineData.Length > 0)
                            {
                                PlanningScheduleDetail scheduleDetail = new PlanningScheduleDetail();
                                #region 读取数据

                                scheduleDetail.ControlNum = ExcelHelper.GetLineDataValue(lineData, ControlNum);
                                scheduleDetail.ReleaseNum = ExcelHelper.GetLineDataValue(lineData, ReleaseNum);
                                scheduleDetail.Purpose = ExcelHelper.GetLineDataValue(lineData, Purpose);
                                scheduleDetail.StartDate = ExcelHelper.GetLineDataValue(lineData, StartDate);
                                scheduleDetail.EndDate = ExcelHelper.GetLineDataValue(lineData, EndDate);
                                scheduleDetail.ForecastType = ExcelHelper.GetLineDataValue(lineData, ForecastType);
                                scheduleDetail.ScheduleQty = ExcelHelper.GetLineDataValue(lineData, ScheduleQty);
                                scheduleDetail.ReleaseDate = ExcelHelper.GetLineDataValue(lineData, ReleaseDate);
                                scheduleDetail.ShipTo = ExcelHelper.GetLineDataValue(lineData, ShipTo);
                                scheduleDetail.ShipFrom = ExcelHelper.GetLineDataValue(lineData, ShipFrom);
                                scheduleDetail.Item = ExcelHelper.GetLineDataValue(lineData, Item);
                                scheduleDetail.Po = ExcelHelper.GetLineDataValue(lineData, Po);
                                scheduleDetail.Uom = ExcelHelper.GetLineDataValue(lineData, Uom);
                                scheduleDetail.ContactName = ExcelHelper.GetLineDataValue(lineData, ContactName);
                                scheduleDetail.ContactTelephone = ExcelHelper.GetLineDataValue(lineData, ContactTelephone);
                                scheduleDetail.DeliverPattern = ExcelHelper.GetLineDataValue(lineData, DeliverPattern);
                                scheduleDetail.DeliveryPatternTime = ExcelHelper.GetLineDataValue(lineData, DeliveryPatternTime);
                                scheduleDetail.FI = ExcelHelper.GetLineDataValue(lineData, FI);
                                scheduleDetail.FIEndDate = ExcelHelper.GetLineDataValue(lineData, FIEndDate);
                                scheduleDetail.FICumQty = ExcelHelper.GetLineDataValue(lineData, FICumQty);
                                scheduleDetail.FIStartDate = ExcelHelper.GetLineDataValue(lineData, FIStartDate);
                                scheduleDetail.MT = ExcelHelper.GetLineDataValue(lineData, MT);
                                scheduleDetail.MTEndDate = ExcelHelper.GetLineDataValue(lineData, MTEndDate);
                                scheduleDetail.MTCumQty = ExcelHelper.GetLineDataValue(lineData, MTCumQty);
                                scheduleDetail.MTStartDate = ExcelHelper.GetLineDataValue(lineData, MTStartDate);
                                scheduleDetail.LastIpQty = ExcelHelper.GetLineDataValue(lineData, LastIpQty);
                                scheduleDetail.LastIpDate = ExcelHelper.GetLineDataValue(lineData, LastIpDate);
                                scheduleDetail.IpCumQty = ExcelHelper.GetLineDataValue(lineData, IpCumQty);
                                scheduleDetail.IpEndDate = ExcelHelper.GetLineDataValue(lineData, IpEndDate);
                                scheduleDetail.LastIpNo = ExcelHelper.GetLineDataValue(lineData, LastIpNo);
                                scheduleDetail.ScheduleTiming = ExcelHelper.GetLineDataValue(lineData, ScheduleTiming);
                                scheduleDetail.ScheduleWhen = ExcelHelper.GetLineDataValue(lineData, ScheduleWhen);
                                scheduleDetail.Qty = ExcelHelper.GetLineDataValue(lineData, Qty);
                                scheduleDetail.CumQty = ExcelHelper.GetLineDataValue(lineData, CumQty);

                                scheduleDetail.PlanningId = planningScheduleMaster.Id;
                                planningScheduleDetailList.Add(scheduleDetail);
                                #endregion
                            }
                        }
                        i++;
                    }

                    this.genericMgr.BulkInsert<PlanningScheduleDetail>(planningScheduleDetailList);
                }
                else if(firstLine[0] == "862")
                {
                    ShippingScheduleMaster shippingScheduleMaster = new ShippingScheduleMaster();
                    shippingScheduleMaster.FileName = fileName;
                    shippingScheduleMaster.FileType = firstLine[0];
                    shippingScheduleMaster.ReceiverID = firstLine[5];
                    shippingScheduleMaster.SenderID = firstLine[3];
                    this.genericMgr.Create(shippingScheduleMaster);

                    List<ShippingScheduleDetail> shippingScheduleDetailList = new List<ShippingScheduleDetail>();
                    #region 定义
                    int ControlNum = 1;
                    int ReleaseNum = 2;
                    int ReleaseDate = 3;
                    int Purpose = 4;
                    int ForecastType = 5;
                    int StartDate = 6;
                    int EndDate = 7;
                    int ReferenceNum = 8;
                    int ShipTo = 9;
                    int ShipFrom = 10;
                    int DeliverPattern = 11;
                    int Item = 12;
                    int Po = 13;
                    int Dock = 14;
                    int LineFeed = 15;
                    int ReserveLineFeed = 16;
                    int ContactName = 17;
                    int ContactTelephone = 18;
                    int LastIpNo = 19;
                    int LastIpQty = 20;
                    int LastIpDate = 21;
                    int CumIpQty = 22;
                    int CumStartDate = 23;
                    int CumEndDate = 24;
                    int CumQty = 25;
                    int Qty = 26;
                    int UOM = 27;
                    int ForecastStatus = 28;
                    int ForecastDate = 29;
                    int ForecastTime = 30;


                    //byte endColIndex = 34;
                    int startRow = 3;

                    #endregion
                    log.Info("Start txt customerorder inbound ");

                    int i = 2;
                    for(string[] lineData = reader.ReadLine(); lineData != null; lineData = reader.ReadLine())
                    {
                        if(i >= startRow)
                        {
                            if(lineData.Length > 0)
                            {
                                ShippingScheduleDetail scheduleDetail = new ShippingScheduleDetail();
                                #region 读取数据
                                scheduleDetail.ControlNum = ExcelHelper.GetLineDataValue(lineData, ControlNum);
                                scheduleDetail.ReleaseNum = ExcelHelper.GetLineDataValue(lineData, ReleaseNum);
                                scheduleDetail.ReleaseDate = ExcelHelper.GetLineDataValue(lineData, ReleaseDate);
                                scheduleDetail.Purpose = ExcelHelper.GetLineDataValue(lineData, Purpose);
                                scheduleDetail.ForecastType = ExcelHelper.GetLineDataValue(lineData, ForecastType);
                                scheduleDetail.StartDate = ExcelHelper.GetLineDataValue(lineData, StartDate);
                                scheduleDetail.EndDate = ExcelHelper.GetLineDataValue(lineData, EndDate);
                                scheduleDetail.ReferenceNum = ExcelHelper.GetLineDataValue(lineData, ReferenceNum);
                                scheduleDetail.ShipTo = ExcelHelper.GetLineDataValue(lineData, ShipTo);
                                scheduleDetail.ShipFrom = ExcelHelper.GetLineDataValue(lineData, ShipFrom);
                                scheduleDetail.DeliverPattern = ExcelHelper.GetLineDataValue(lineData, DeliverPattern);
                                scheduleDetail.Item = ExcelHelper.GetLineDataValue(lineData, Item);
                                scheduleDetail.Po = ExcelHelper.GetLineDataValue(lineData, Po);
                                scheduleDetail.Dock = ExcelHelper.GetLineDataValue(lineData, Dock);
                                scheduleDetail.LineFeed = ExcelHelper.GetLineDataValue(lineData, LineFeed);
                                scheduleDetail.ReserveLineFeed = ExcelHelper.GetLineDataValue(lineData, ReserveLineFeed);
                                scheduleDetail.ContactName = ExcelHelper.GetLineDataValue(lineData, ContactName);
                                scheduleDetail.ContactTelephone = ExcelHelper.GetLineDataValue(lineData, ContactTelephone);
                                scheduleDetail.LastIpNo = ExcelHelper.GetLineDataValue(lineData, LastIpNo);
                                scheduleDetail.LastIpQty = ExcelHelper.GetLineDataValue(lineData, LastIpQty);
                                scheduleDetail.LastIpDate = ExcelHelper.GetLineDataValue(lineData, LastIpDate);
                                scheduleDetail.CumIpQty = ExcelHelper.GetLineDataValue(lineData, CumIpQty);
                                scheduleDetail.CumStartDate = ExcelHelper.GetLineDataValue(lineData, CumStartDate);
                                scheduleDetail.CumEndDate = ExcelHelper.GetLineDataValue(lineData, CumEndDate);
                                scheduleDetail.CumQty = ExcelHelper.GetLineDataValue(lineData, CumQty);
                                scheduleDetail.Qty = ExcelHelper.GetLineDataValue(lineData, Qty);
                                scheduleDetail.Uom = ExcelHelper.GetLineDataValue(lineData, UOM);
                                scheduleDetail.ForecastStatus = ExcelHelper.GetLineDataValue(lineData, ForecastStatus);
                                scheduleDetail.ForecastDate = ExcelHelper.GetLineDataValue(lineData, ForecastDate);
                                scheduleDetail.ForecastTime = ExcelHelper.GetLineDataValue(lineData, ForecastTime);

                                scheduleDetail.ShippingId = shippingScheduleMaster.Id;
                                shippingScheduleDetailList.Add(scheduleDetail);
                                #endregion
                            }
                        }
                        i++;
                    }

                    this.genericMgr.BulkInsert<ShippingScheduleDetail>(shippingScheduleDetailList);
                }
                else
                {
                    throw new Exception("不支持此文件格式:" + firstLine[0]);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Processcustomerorder inbound file Error.", ex);
            }
            finally
            {
                inputStream.Dispose();
                log.Info("Process txt customerorder inbound file successful.");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void EDI2Plan()
        {
            try
            {
                string flowCode = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.FordFlow);
                var flowMaster = this.genericMgr.FindById<FlowMaster>(flowCode);

                var shippingScheduleMasters = this.genericMgr.FindAll<ShippingScheduleMaster>
                    (" from ShippingScheduleMaster order by Id desc", 0, 1);
                var errorItems = new List<string>();

                if(shippingScheduleMasters != null && shippingScheduleMasters.Count > 0 && !shippingScheduleMasters.First().IsImported)
                {
                    var shippingScheduleMaster = shippingScheduleMasters.First();
                    shippingScheduleMaster.IsImported = true;
                    this.genericMgr.Update(shippingScheduleMaster);

                    var shippingScheduleDetails = this.genericMgr.FindAll<ShippingScheduleDetail>
                        (" from ShippingScheduleDetail where ShippingId =? ", new object[] { shippingScheduleMaster.Id });

                    var itemDic = this.itemMgr.GetRefItemCode(flowCode, shippingScheduleDetails.Select(p => p.Item).Distinct().ToList());

                    var mrpPlanLogList = new List<MrpPlanLog>();
                    foreach(var p in shippingScheduleDetails)
                    {
                        if(!itemDic.ContainsKey(p.Item))
                        {
                            errorItems.Add(p.Item);
                            continue;
                        }
                        var mrpPlanLog = new MrpPlanLog();
                        mrpPlanLog.Flow = flowCode;
                        var item = itemDic[p.Item];
                        mrpPlanLog.Item = item.Code;
                        mrpPlanLog.UnitQty = 1;
                        mrpPlanLog.ItemDescription = item.Deriction;
                        mrpPlanLog.ItemReference = item.ReferenceCode;
                        mrpPlanLog.Location = flowMaster.LocationFrom;
                        mrpPlanLog.OrderType = flowMaster.Type;
                        mrpPlanLog.Party = flowMaster.PartyTo;
                        mrpPlanLog.PlanDate = DateTime.Parse(p.ForecastDate.Substring(0, 4) + "-" + p.ForecastDate.Substring(4, 2) + "-" + p.ForecastDate.Substring(6, 2));
                        mrpPlanLog.Qty = double.Parse(p.Qty);
                        mrpPlanLog.Uom = item.Uom;
                        mrpPlanLogList.Add(mrpPlanLog);
                    }

                    planMgr.CreateMrpPlan(flowCode, mrpPlanLogList);
                }

                var planningScheduleMasters = this.genericMgr.FindAll<PlanningScheduleMaster>
                    (" from PlanningScheduleMaster order by Id desc", 0, 1);

                if(planningScheduleMasters != null && planningScheduleMasters.Count > 0 && !planningScheduleMasters.Last().IsImported)
                {
                    var planningScheduleMaster = planningScheduleMasters.First();
                    planningScheduleMaster.IsImported = true;
                    this.genericMgr.Update(planningScheduleMaster);

                    var planningScheduleDetails = this.genericMgr.FindAll<PlanningScheduleDetail>
                        (" from PlanningScheduleDetail where PlanningId =? ", new object[] { planningScheduleMaster.Id });

                    var itemDic = this.itemMgr.GetRefItemCode(flowCode, planningScheduleDetails.Select(p => p.Item).Distinct().ToList());

                    var mrpPlanLogList = new List<MrpPlanLog>();
                    var rccpPlanLogList = new List<RccpPlanLog>();
                    var currentItem = string.Empty;
                    var forcastDateFrom = DateTime.Now;

                    foreach(var plan in planningScheduleDetails)
                    {
                        if(!itemDic.ContainsKey(plan.Item))
                        {
                            errorItems.Add(plan.Item);
                            continue;
                        }
                        var item = itemDic[plan.Item];
                        double qty = double.Parse(plan.Qty);
                        if(plan.ScheduleTiming == "W")
                        {
                            var days = 6;
                            DateTime planDate = DateTime.Parse(plan.ScheduleWhen);

                            #region MrpPlanLog 拆分周到天
                            if(planDate > DateTime.Now.AddDays(14) && planDate < DateTime.Now.AddDays(30))
                            {
                                double dayQty = Math.Round(qty / days);
                                DateTime currentDate = planDate;
                                for(int j = 0; j < days; j++)
                                {
                                    MrpPlanLog mrpPlanLog = new MrpPlanLog();

                                    mrpPlanLog.UnitQty = 1;
                                    mrpPlanLog.ItemDescription = item.Description;
                                    mrpPlanLog.ItemReference = item.ReferenceCode;
                                    mrpPlanLog.PlanDate = currentDate;
                                    mrpPlanLog.Item = item.Code;
                                    mrpPlanLog.Party = flowMaster.PartyTo;
                                    mrpPlanLog.OrderType = flowMaster.Type;
                                    mrpPlanLog.Flow = flowMaster.Code;
                                    mrpPlanLog.Location = flowMaster.LocationFrom;
                                    mrpPlanLog.Uom = item.Uom;

                                    if(j == days - 1)
                                    {
                                        mrpPlanLog.Qty = qty - dayQty * (days - 1);
                                    }
                                    else
                                    {
                                        mrpPlanLog.Qty = dayQty;
                                    }
                                    currentDate = currentDate.AddDays(1);
                                    mrpPlanLogList.Add(mrpPlanLog);
                                }
                            }
                            #endregion

                            #region RccpPlanLog 周转粗能力计划单
                            RccpPlanLog rccpPlanLog = new RccpPlanLog();
                            rccpPlanLog.Flow = flowCode;
                            rccpPlanLog.DateIndexTo = DateTimeHelper.GetWeekOfYear(planDate);
                            rccpPlanLog.Item = item.Code;
                            rccpPlanLog.Uom = item.Uom;
                            rccpPlanLog.DateType = CodeMaster.TimeUnit.Week;
                            rccpPlanLog.Qty = qty;
                            rccpPlanLog.UnitQty = 1;
                            rccpPlanLog.ItemDescription = item.Description;
                            rccpPlanLog.ItemReference = item.ReferenceCode;
                            rccpPlanLogList.Add(rccpPlanLog);
                            #endregion
                        }
                        else if(plan.ScheduleTiming == "F")
                        {
                            #region RccpPlanLog  把月度的拆分到周 首月不拆分
                            DateTime forcastDateTo = DateTime.Parse(plan.ScheduleWhen.Split('~')[1]).AddDays(1);
                            if(currentItem == plan.Item)
                            {
                                var weeks = Math.Round((forcastDateTo - forcastDateFrom).TotalDays / 7);
                                double weekQty = Math.Round(qty / weeks);
                                for(int i = 0; i < weeks; i++)
                                {
                                    RccpPlanLog rccpPlanLog = new RccpPlanLog();
                                    rccpPlanLog.Flow = flowCode;
                                    rccpPlanLog.DateIndexTo = DateTimeHelper.GetWeekOfYear(forcastDateFrom);
                                    rccpPlanLog.Item = item.Code;
                                    rccpPlanLog.Uom = item.Uom;
                                    rccpPlanLog.DateType = CodeMaster.TimeUnit.Week;
                                    if(i == weeks - 1)
                                    {
                                        rccpPlanLog.Qty = qty - weekQty * (weeks - 1);
                                    }
                                    else
                                    {
                                        rccpPlanLog.Qty = weekQty;
                                    }
                                    rccpPlanLog.UnitQty = 1;
                                    rccpPlanLog.ItemDescription = item.Description;
                                    rccpPlanLog.ItemReference = item.ReferenceCode;
                                    forcastDateFrom = forcastDateFrom.AddDays(7);
                                    rccpPlanLogList.Add(rccpPlanLog);
                                }
                            }
                            currentItem = plan.Item;
                            forcastDateFrom = forcastDateTo;
                            #endregion
                        }
                    }
                    planMgr.CreateMrpPlan(flowCode, mrpPlanLogList);
                    //planMgr.CreateRccpPlan(CodeMaster.TimeUnit.Week, rccpPlanLogList);
                }

                if(errorItems.Count > 0)
                {
                    string errorMessage = string.Format("参考物料号{0}不存在", string.Join(",", errorItems.Distinct()));
                    var ex = new Exception(errorMessage);

                    log.Error(ex);
                    SendErrorEmail("Edi转Plan失败", ex);

                }

            }
            catch(Exception ex)
            {
                log.Error(ex);
                SendErrorEmail("数据由EDI转Plan失败", ex);
            }
        }

        private void SendErrorEmail(string title, Exception ex)
        {
            try
            {
                var logToUser = genericMgr.FindById<LogToUser>
                    ((int)NVelocityTemplateRepository.TemplateEnum.EDI_ScheduleMgrImpl_EDI2Plan);
                var emailReceiveUsers = logToUser.Emails;

                if(!string.IsNullOrWhiteSpace(emailReceiveUsers))
                {
                    IDictionary<string, object> data = new Dictionary<string, object>();
                    ex.HelpLink = ExceptionHelper.GetExceptionMessage(ex);
                    data.Add("Title", title);
                    data.Add("Message", ex.HelpLink);
                    data.Add("StackTrace", ex.StackTrace);
                    string content = vmReporsitory.RenderTemplate(logToUser.Template, data);
                    emailMgr.AsyncSendEmail(title, content, emailReceiveUsers, MailPriority.High);
                }
            }
            catch(Exception exception)
            {
                log.Fatal(exception);
            }
        }
    }
}
