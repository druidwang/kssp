using System;
using System.Linq;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.MD;
using System.Collections.Generic;
using NHibernate.Criterion;
using System.Text;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.FMS;
using com.Sconit.Utility;
using System.Xml;
using com.Sconit.Entity.MES;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class FacilityMgrImpl : BaseMgr, IFacilityMgr
    {
        #region 变量
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public IOrderMgr orderMgr { get; set; }
        #endregion

        #region Public Methods

        [Transaction(TransactionMode.Requires)]
        public  void CreateFacilityMaster(FacilityMaster facilityMaster)
        {
            facilityMaster.FCID = numberControlMgr.GetFCID(facilityMaster);
            facilityMaster.Status = CodeMaster.FacilityStatus.Create;
            facilityMaster.CurrChargePersonName = genericMgr.FindById<User>(facilityMaster.CurrChargePersonId).FullName;
            genericMgr.Create(facilityMaster);

            #region 记事务
            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUserId = facilityMaster.CreateUserId;
            facilityTrans.CreateUserName = facilityMaster.CreateUserName;
            facilityTrans.EffDate = DateTime.Now.Date;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePersonId = facilityMaster.CurrChargePersonId;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.ToChargePersonId = facilityMaster.CurrChargePersonId;
            facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
            facilityTrans.TransType = CodeMaster.FacilityTransType.Create;

            facilityTrans.AssetNo = facilityMaster.AssetNo;
            facilityTrans.FacilityName = facilityMaster.Name;
            facilityTrans.FacilityCategory = facilityMaster.Category;
            genericMgr.Create(facilityTrans);
            #endregion


        }



        [Transaction(TransactionMode.Requires)]
        public void GetFacilityControlPoint(string facilityName, string traceCode)
        {
            string opStr = facilityName + "/Op";
            string opRefStr = facilityName + "/OpRef";

            XmlElement opPointXml = ObixHelper.Request_WebRequest(opStr);
            XmlNodeList opNodeList = opPointXml.ChildNodes;

            XmlElement opRefPointXml = ObixHelper.Request_WebRequest(opRefStr);
            XmlNodeList opRefNodeList = opRefPointXml.ChildNodes;

          
            
            MesScanControlPoint mscp = new MesScanControlPoint();
            mscp.ControlPoint = facilityName;
            mscp.CreateDate = DateTime.Now;
            mscp.Op = opNodeList[2].Attributes["val"].Value;
            mscp.OpReference = opRefNodeList[2].Attributes["val"].Value;     
            mscp.ProdItem = "";
            mscp.ScanDate = DateTime.Now.ToString("yyyyMMdd");
            mscp.ScanTime = DateTime.Now.ToString("HHmmss");
            mscp.Status = 0;
            mscp.Type = CodeMaster.FacilityParamaterType.Scan;
            //if (mscp.Op.Equals("1"))
            //{
            //    mscp.TraceCode = orderMgr.PrintTraceCode(orderNo);
            //}
            //else
            //{
            mscp.TraceCode = traceCode;
            //}
            genericMgr.Create(mscp);


        }


        [Transaction(TransactionMode.Requires)]
        public void GetFacilityParamater(string facilityName, string paramaterName, string name, string traceCode)
        {
            string opStr = facilityName + "/Op";
            string opRefStr = facilityName + "/OpRef";
            string paramaterStr = facilityName + "/" + paramaterName;

            XmlElement opPointXml = ObixHelper.Request_WebRequest(opStr);
            XmlNodeList opNodeList = opPointXml.ChildNodes;

            XmlElement opRefPointXml = ObixHelper.Request_WebRequest(opRefStr);
            XmlNodeList opRefNodeList = opRefPointXml.ChildNodes;

            XmlElement paramaterPointXml = ObixHelper.Request_WebRequest(paramaterStr);
            XmlNodeList paramaterNodeList = paramaterPointXml.ChildNodes;

            MesScanControlPoint mscp = new MesScanControlPoint();
            mscp.ControlPoint = facilityName;
            mscp.CreateDate = DateTime.Now;
            mscp.Op = opNodeList[2].Attributes["val"].Value;
            mscp.OpReference = opRefNodeList[2].Attributes["val"].Value;
            mscp.ProdItem = "";
            mscp.ScanDate = DateTime.Now.ToString("yyyyMMdd");
            mscp.ScanTime = DateTime.Now.ToString("HHmmss");
            mscp.Status = 0;
            mscp.Note = name;
            mscp.NoteValue = paramaterNodeList[2].Attributes["val"].Value;
            mscp.Type = CodeMaster.FacilityParamaterType.Paramater;
            mscp.TraceCode = traceCode;
            genericMgr.Create(mscp);
           

        }


        [Transaction(TransactionMode.Requires)]
        public bool CheckProductLine(string productline)
        {
            bool prodlineStatus = false;
            XmlElement controlPointXml = ObixHelper.Request_WebRequest(productline);
            XmlNodeList nodeList = controlPointXml.ChildNodes;

            return prodlineStatus;

        }

        [Transaction(TransactionMode.Requires)]
        public void CreateFacilityOrder(string facilityName)
        {
          
            string useCounterStr = facilityName + "/UseCounter";


            XmlElement useCounterXml = ObixHelper.Request_WebRequest(useCounterStr);
            XmlNodeList userCounterNodeList = useCounterXml.ChildNodes;


            decimal accQty = Decimal.Parse(userCounterNodeList[2].Attributes["val"].Value);
           
       
            IList<FacilityMaintainPlan> facilityMaintainPlanList = genericMgr.FindAll<FacilityMaintainPlan>("from FacilityMaintainPlan f where f.FCID = ? and f.NextMaintainQty > 0", new object[] { facilityName });
            if (facilityMaintainPlanList != null && facilityMaintainPlanList.Count > 0)
            {
                FacilityMaintainPlan facilityMaintainPlan = facilityMaintainPlanList[0];
              

                if (accQty >= facilityMaintainPlan.NextMaintainQty)
                {
                    MaintainPlan maintainPlan = genericMgr.FindById<MaintainPlan>(facilityMaintainPlan.MaintainPlan.Code);

                    FacilityOrderMaster facilityOrderMaster = new FacilityOrderMaster();
                    facilityOrderMaster.FacilityOrderNo = "FO" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    facilityOrderMaster.Status = CodeMaster.FacilityOrderStatus.Submit;
                    facilityOrderMaster.Type = CodeMaster.FacilityOrderType.Maintain;
                    facilityOrderMaster.ReferenceNo = maintainPlan.Description;
                    facilityOrderMaster.Region = string.Empty;
                    genericMgr.Create(facilityOrderMaster);


                    IList<MaintainPlanItem> maintainPlanItemList = genericMgr.FindAll<MaintainPlanItem>("from MaintainPlanItem m where m.MaintainPlanCode = ?", new object[] { facilityMaintainPlan.MaintainPlan.Code });
                    foreach (MaintainPlanItem maintainPlanItem in maintainPlanItemList)
                    {
                        FacilityOrderDetail facilityOrderDetail = new FacilityOrderDetail();
                        facilityOrderDetail.FacilityOrderNo = facilityOrderMaster.FacilityOrderNo;
                        facilityOrderDetail.BaseUom = maintainPlanItem.BaseUom;
                        facilityOrderDetail.FacilityId = facilityMaintainPlan.FCID;
                        facilityOrderDetail.Item = maintainPlanItem.Item;
                        facilityOrderDetail.ItemDescription = maintainPlanItem.ItemDescription;
                        facilityOrderDetail.PlanQty = maintainPlanItem.Qty;
                        facilityOrderDetail.Sequence = maintainPlanItem.Sequence;
                        facilityOrderDetail.Uom = maintainPlanItem.Uom;
                        genericMgr.Create(facilityOrderDetail);
                        facilityOrderMaster.AddFacilityOrderDetail(facilityOrderDetail);
                    }

                    #region 更新下次保养数量
                    facilityMaintainPlan.NextMaintainQty += maintainPlan.TypePeriod;
                    genericMgr.Update(facilityMaintainPlan);
                    #endregion
                }

               
            }
          

        }



        [Transaction(TransactionMode.Requires)]
        public void GenerateFacilityMaintainPlan()
        {

            #region 取所有到时间的预防计划的设施
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityMaintainPlan));
            criteria.Add (Expression.And(Expression.IsNotNull("NextWarnDate"), Expression.Le("NextWarnDate", DateTime.Now)));
            IList<FacilityMaintainPlan> facilityMaintainPlanList = genericMgr.FindAll<FacilityMaintainPlan>(criteria);
            #endregion

            #region 生成ISI任务
           
            if (facilityMaintainPlanList != null && facilityMaintainPlanList.Count > 0)
            {

                foreach (FacilityMaintainPlan facilityPlan in facilityMaintainPlanList)
                {
                    #region 生成ISI任务
                    MaintainPlan maintainPlan = genericMgr.FindById<MaintainPlan>(facilityPlan.MaintainPlan.Code);

                    FacilityOrderMaster facilityOrderMaster = new FacilityOrderMaster();
                    facilityOrderMaster.FacilityOrderNo = "FO" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    facilityOrderMaster.Status = CodeMaster.FacilityOrderStatus.Submit;
                    facilityOrderMaster.Type = CodeMaster.FacilityOrderType.Maintain;
                    facilityOrderMaster.ReferenceNo = maintainPlan.Description;
                    facilityOrderMaster.Region = string.Empty;
                    genericMgr.Create(facilityOrderMaster);


                    IList<MaintainPlanItem> maintainPlanItemList = genericMgr.FindAll<MaintainPlanItem>("from MaintainPlanItem m where m.MaintainPlanCode = ?", new object[] { facilityPlan.MaintainPlan.Code });
                    foreach (MaintainPlanItem maintainPlanItem in maintainPlanItemList)
                    {
                        FacilityOrderDetail facilityOrderDetail = new FacilityOrderDetail();
                        facilityOrderDetail.FacilityOrderNo = facilityOrderMaster.FacilityOrderNo;
                        facilityOrderDetail.BaseUom = maintainPlanItem.BaseUom;
                        facilityOrderDetail.FacilityId = facilityPlan.FCID;
                        facilityOrderDetail.Item = maintainPlanItem.Item;
                        facilityOrderDetail.ItemDescription = maintainPlanItem.ItemDescription;
                        facilityOrderDetail.PlanQty = maintainPlanItem.Qty;
                        facilityOrderDetail.Sequence = maintainPlanItem.Sequence;
                        facilityOrderDetail.Uom = maintainPlanItem.Uom;
                        genericMgr.Create(facilityOrderDetail);
                        facilityOrderMaster.AddFacilityOrderDetail(facilityOrderDetail);
                    }
                    #endregion

                    #region 更新下次时间、数量
                    if (facilityPlan.MaintainPlan.Type == CodeMaster.MaintainPlanType.Once)
                    {
                        facilityPlan.NextMaintainDate = null;
                        facilityPlan.NextWarnDate = null;
                    }
                    else
                    {
                        #region 现在周期都维护
                        if (facilityPlan.MaintainPlan.Type == CodeMaster.MaintainPlanType.Minute)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddMinutes(facilityPlan.MaintainPlan.TypePeriod);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddMinutes(facilityPlan.MaintainPlan.TypePeriod);
                        }
                        else if (facilityPlan.MaintainPlan.Type == CodeMaster.MaintainPlanType.Hour)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddHours(facilityPlan.MaintainPlan.TypePeriod);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddHours(facilityPlan.MaintainPlan.TypePeriod);
                        }
                        else if (facilityPlan.MaintainPlan.Type == CodeMaster.MaintainPlanType.Day)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddDays(facilityPlan.MaintainPlan.TypePeriod);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddDays(facilityPlan.MaintainPlan.TypePeriod);
                        }
                        else if (facilityPlan.MaintainPlan.Type == CodeMaster.MaintainPlanType.Week)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddDays(7 * facilityPlan.MaintainPlan.TypePeriod);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddDays(7 * facilityPlan.MaintainPlan.TypePeriod);
                        }
                        else if (facilityPlan.MaintainPlan.Type == CodeMaster.MaintainPlanType.Month)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddMonths(facilityPlan.MaintainPlan.TypePeriod);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddMonths(facilityPlan.MaintainPlan.TypePeriod);
                        }
                        else if (facilityPlan.MaintainPlan.Type == CodeMaster.MaintainPlanType.Year)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddYears(facilityPlan.MaintainPlan.TypePeriod);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddYears(facilityPlan.MaintainPlan.TypePeriod);
                        }
                        #endregion
                    }

                    genericMgr.Update(facilityPlan);
                    #endregion
                }
            }
            #endregion
        }




        [Transaction(TransactionMode.Requires)]
        public void CreateCheckListOrder(CheckListOrderMaster checkListOrderMaster)
        {
            CheckListMaster checkListMaster = genericMgr.FindById<CheckListMaster>(checkListOrderMaster.CheckListCode);

            IList<CheckListDetail> checkListDetailList = genericMgr.FindAll<CheckListDetail>("from CheckListDetail c where c.CheckListCode = ?", new object[] { checkListOrderMaster.CheckListCode });

       
            checkListOrderMaster.Code = "CL" + DateTime.Now.ToString("yyyyMMddHHmmss");
            checkListOrderMaster.CheckListCode = checkListMaster.Code;
            checkListOrderMaster.CheckListName = checkListMaster.Name;
            checkListOrderMaster.Status = CodeMaster.CheckListOrderStatus.Create;

            genericMgr.Create(checkListOrderMaster);

            foreach (CheckListDetail checkListDetail in checkListDetailList)
            {
                CheckListOrderDetail checkListOrderDetail = new CheckListOrderDetail();
                checkListOrderDetail.CheckListCode = checkListDetail.CheckListCode;
                checkListOrderDetail.CheckListDetailCode = checkListDetail.Code;
                checkListOrderDetail.MaxValue = checkListDetail.MaxValue;
                checkListOrderDetail.MinValue = checkListDetail.MinValue;
                checkListOrderDetail.OrderNo = checkListOrderMaster.Code;
                checkListOrderDetail.Description = checkListDetail.Description;
                checkListOrderDetail.IsNormal = true;
                genericMgr.Create(checkListOrderDetail);


            }

        }

           [Transaction(TransactionMode.Requires)]
        public  void ReleaseCheckListOrder(CheckListOrderMaster checkListOrderMaster){
            checkListOrderMaster.Status = CodeMaster.CheckListOrderStatus.Submit;
            genericMgr.Update(checkListOrderMaster);
            foreach (CheckListOrderDetail d in checkListOrderMaster.CheckListOrderDetailList)
            {
                genericMgr.Update(d);
            }
           }


           [Transaction(TransactionMode.Requires)]
           public void StartFacilityOrder(string facilityOrderNo)
           {
               FacilityOrderMaster facilityOrderMaster = genericMgr.FindById<FacilityOrderMaster>(facilityOrderNo);


               IList<FacilityOrderDetail> facilityOrderDetailList = genericMgr.FindAll<FacilityOrderDetail>("from FacilityOrderDetail where FacilityOrderNo=?", facilityOrderNo);

               List<string> facilityList = new List<string>();
               foreach (FacilityOrderDetail d in facilityOrderDetailList)
               {
                   genericMgr.Update(d);
                   if (!facilityList.Contains(d.FacilityId))
                   {
                       facilityList.Add(d.FacilityId);

                       #region 记设备事务
                       FacilityMaster facilityMaster = genericMgr.FindById<FacilityMaster>(d.FacilityId);
                       FacilityTrans facilityTrans = new FacilityTrans();
                       facilityTrans.CreateDate = DateTime.Now;
                       facilityTrans.CreateUserId = facilityMaster.CreateUserId;
                       facilityTrans.CreateUserName = facilityMaster.CreateUserName;
                       facilityTrans.EffDate = DateTime.Now.Date;
                       facilityTrans.FCID = facilityMaster.FCID;
                       facilityTrans.FromChargePersonId = facilityMaster.CurrChargePersonId;
                       facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
                       facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
                       facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
                       facilityTrans.ToChargePersonId = facilityMaster.CurrChargePersonId;
                       facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
                       facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
                       facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
                       facilityTrans.TransType = CodeMaster.FacilityTransType.StartMaintain;
                       facilityTrans.Remark = d.Note;
                       facilityTrans.AssetNo = facilityMaster.AssetNo;
                       facilityTrans.FacilityName = facilityMaster.Name;
                       facilityTrans.FacilityCategory = facilityMaster.Category;

                       genericMgr.Create(facilityTrans);
                       #endregion

                   }
               }

               facilityOrderMaster.Status = CodeMaster.FacilityOrderStatus.InProcess;
               genericMgr.Update(facilityOrderMaster);

           }

           [Transaction(TransactionMode.Requires)]
           public void FinishFacilityOrder(FacilityOrderMaster facilityOrderMaster)
           {
               List<string> facilityList = new List<string>();
               foreach (FacilityOrderDetail d in facilityOrderMaster.FacilityOrderDetails)
               {
                   genericMgr.Update(d);
                   if (!facilityList.Contains(d.FacilityId))
                   {
                       facilityList.Add(d.FacilityId);

                       #region 记设备事务
                       FacilityMaster facilityMaster = genericMgr.FindById<FacilityMaster>(d.FacilityId);
                       FacilityTrans facilityTrans = new FacilityTrans();
                       facilityTrans.CreateDate = DateTime.Now;
                       facilityTrans.CreateUserId = facilityMaster.CreateUserId;
                       facilityTrans.CreateUserName = facilityMaster.CreateUserName;
                       facilityTrans.EffDate = DateTime.Now.Date;
                       facilityTrans.FCID = facilityMaster.FCID;
                       facilityTrans.FromChargePersonId = facilityMaster.CurrChargePersonId;
                       facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
                       facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
                       facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
                       facilityTrans.ToChargePersonId = facilityMaster.CurrChargePersonId;
                       facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
                       facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
                       facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
                       facilityTrans.TransType = CodeMaster.FacilityTransType.FinishMaintain;
                       facilityTrans.Remark = d.Note;
                       facilityTrans.AssetNo = facilityMaster.AssetNo;
                       facilityTrans.FacilityName = facilityMaster.Name;
                       facilityTrans.FacilityCategory = facilityMaster.Category;
                      
                       genericMgr.Create(facilityTrans);
                       #endregion

                   }
               }

               facilityOrderMaster.Status = CodeMaster.FacilityOrderStatus.Close;
               genericMgr.Update(facilityOrderMaster);

           }
        #endregion

        #region Private Methods

      

        #endregion



    }
}
