using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Service;
using com.Sconit.Utility;
using com.Sconit.Entity.Exception;
using com.Sconit.Service.MRP;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.MD;
using System.Text;
using com.Sconit.Entity.MRP.MD;
using System.Data.SqlClient;
using com.Sconit.Web.Models.SearchModels.ORD;
using System.Data;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MrpPlanFiController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        //public IMrpMgr mrpMgr { get; set; }
        public IPlanMgr planMgr { get; set; }
        public IMrpOrderMgr mrpOrderMgr { get; set; }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_Shift")]
        public ActionResult ShiftView()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_Shift")]
        public ActionResult Shift()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_FiPlanExecutionMonitor_View")]
        public ActionResult _MrpFiExecutionMonitorReport()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_Shift")]
        public ActionResult _MrpShiftPlanList(string flow, DateTime planVersion, DateTime planDate, string shift)
        {
            ViewBag.Flow = flow;
            ViewBag.PlanDate = planDate;
            ViewBag.Shift = shift;
            //DateTime planVersion=DateTime.Now;
            //var dateIndex = Utility.DateTimeHelper.GetWeekOfYear(planDate);
            //var maxPlanversionOfWeeks = genericMgr.FindAll<MrpPlanMaster>
            //    ("from MrpPlanMaster where DateIndex =? And IsRelease = 1 And ResourceGroup = ? order by PlanVersion Desc  ",
            //    new object[] { dateIndex, CodeMaster.ResourceGroup.FI }, 0, 1);
            //var maxPlanversionOfWeek = (maxPlanversionOfWeeks ?? new List<MrpPlanMaster>()).FirstOrDefault();
            //if (maxPlanversionOfWeek != null)
            //{
            //    planVersion = maxPlanversionOfWeek.PlanVersion;
            //}
            //else
            //{
            //    SaveWarningMessage(string.Format("此日期没有对应的已释放班产计划。"));
            //    return PartialView();
            //}
            ViewBag.PlanVersion = planVersion;
            MrpPlanMaster mrpPlanMaster = this.genericMgr.FindById<MrpPlanMaster>(planVersion);
            ViewBag.IsRelease = mrpPlanMaster.IsRelease;
            string sql = @"select OrderNo from ORD_OrderMstr_4 as a where a.Type = ? and a.Flow = ? and a.Shift = ?
                           and a.EffDate=? and a.Status in ( ?,?,? ) ";
            IList<object> orderNos = this.genericMgr.FindAllWithNativeSql<object>(sql,
                new Object[] { CodeMaster.OrderType.Production, flow, shift, planDate.Date,
                                CodeMaster.OrderStatus.Create, CodeMaster.OrderStatus.Submit, CodeMaster.OrderStatus.InProcess });
            if (orderNos != null && orderNos.Count() > 0)
            {
                SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_PleaseCancelCorrespondingProductionOrderOfThisFlowShiftExits, flow, shift, orderNos[0]));
            }
            return PartialView();
        }

        [HttpGet]
        public string _planVersion(DateTime planDate)
        {
            DateTime planVersion = DateTime.Now;
            var dateIndex = Utility.DateTimeHelper.GetWeekOfYear(planDate);
            var maxPlanversionOfWeeks = genericMgr.FindAll<MrpPlanMaster>
                ("from MrpPlanMaster where DateIndex =? And IsRelease = 1 And ResourceGroup = ? order by PlanVersion Desc  ",
                new object[] { dateIndex, CodeMaster.ResourceGroup.FI }, 0, 1);
            var maxPlanversionOfWeek = (maxPlanversionOfWeeks ?? new List<MrpPlanMaster>()).FirstOrDefault();
            if (maxPlanversionOfWeek != null)
            {
                planVersion = maxPlanversionOfWeek.PlanVersion;
            }
            else
            {
                SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_TheDateLackCorrespondingAlreadyReleasedShiftPlan));
            }
            return planVersion.ToString("yyyy-MM-dd HH:mm:ss");
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _SelectBatchEditing(DateTime? planVersion, string flow, DateTime? planDate, string shift)
        {
            IList<MrpFiShiftPlan> planList = new List<MrpFiShiftPlan>();

            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(shift) && planVersion.HasValue && planDate.HasValue)
            {
                string hql = "from MrpFiShiftPlan where PlanVersion =? and ProductLine = ? and PlanDate =? and Shift = ? order by Sequence";
                planList = genericMgr.FindAll<MrpFiShiftPlan>(hql,
                    new object[] { planVersion.Value, flow, planDate.Value, shift });
                foreach (var plan in planList)
                {
                    var item = this.itemMgr.GetCacheItem(plan.Item);
                    plan.ItemDescription = item.Description;
                    plan.ReferenceItemCode = item.ReferenceCode;
                    plan.Uom = item.Uom;
                }
                planList = planList.OrderBy(p => p.Island).ThenBy(p => p.Machine).ToList();
            }
            return PartialView(new GridModel(planList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_Shift")]
        public JsonResult _SaveBatchEditing([Bind(Prefix = "updated")]IEnumerable<MrpFiShiftPlan> updatedPlans)
        {
            try
            {
                if (updatedPlans != null)
                {
                    foreach (var plan in updatedPlans)
                    {
                        this.genericMgr.Update(plan);
                    }
                }
                object obj = new { };
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedPlanSuccessfully);
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_Shift")]
        public ActionResult _CreateOrder(string flow, DateTime planVersion, DateTime planDate, string shift)
        {
            try
            {
                var orderMaster = mrpOrderMgr.CreateFiOrder(flow, planVersion, planDate, shift);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CreateProductionOrderSuccessfully);
                return Json(orderMaster.OrderNo);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(null);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_ShiftView")]
        public string _GetShiftView(DateTime planVersion, string flow)
        {
            return planMgr.GetFiShiftPlanView(planVersion, flow).ToString();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_ShiftView")]
        public ActionResult _MrpPlanOverallView(DateTime planVersion, string flow)
        {
            var planList = new List<MrpFiMachinePlan>();
            string hql = "from MrpFiMachinePlan where PlanVersion =? and ProductLine =? order by Machine";



            planList = genericMgr.FindAll<MrpFiMachinePlan>(hql, new object[] { planVersion, flow })
               .Where(p => Math.Round(p.ShiftQty, 1) >= 0).ToList();
            return PartialView(planList);
        }

        #region Export ShiftView
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_ShiftView")]
        public ActionResult Export(DateTime planVersion, string flow, int Type)
        {
            var table = "";
            if (Type == 1)
            {
                table = _GetShiftView(planVersion, flow);
            }
            else
            {
                table = _MrpPlanOverallViewHtml(planVersion, flow);
            }
            string excelName = Type == 1 ? "ShiftView.xls" : "MrpPlanOverallView.xls";
            return new DownloadFileActionResult(table, excelName);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_ShiftView")]
        public string _MrpPlanOverallViewHtml(DateTime planVersion, string flow)
        {
            int tableColumnCount;
            int settledColumnCount;
            string ProcedureName = "USP_Busi_MRP_FiMachinePlan";
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@Planversion", planVersion);
            sqlParams[1] = new SqlParameter("@Flow", flow);
            sqlParams[2] = new SqlParameter("@Type", "GetWebDisPlay");
            sqlParams[3] = new SqlParameter("@ParaAll", "");
            DataSet ds = genericMgr.GetDatasetByStoredProcedure(ProcedureName, sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            StringBuilder strHead = new StringBuilder("<tr>");
            #region Head
            //拼接固定列
            for (int i = 0; i < 4; i++)
            {
                str.Append("<th>" + (string)ds.Tables[1].Columns[i].ColumnName.ToString() + "</th>");
            }
            for (int i = 4; i < tableColumnCount; i++)
            {
                //SP return each column's length
                str.Append("<th style=\"text-align:center\"  style=\"text-align:center\" >");
                str.Append((string)ds.Tables[1].Columns[i].ColumnName.ToString().Substring(5, 12));
                str.Append("</th>");
            }
            str.Append("</tr>" + strHead.ToString() + "</tr></thead><tbody>");
            #endregion
            int l = 0;
            string formmerIsland = "";
            string laterIsland = "";
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                l++;
                laterIsland = (string)ds.Tables[1].Rows[i][1];
                if (formmerIsland != laterIsland && formmerIsland !="")
                {
                    if (trcss == "")
                    {
                        trcss = "t-alt";
                    }
                    else
                    {
                        trcss = "";
                    }
                }
                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");
                for (int j = 0; j < tableColumnCount; j++)
                {
                    str.Append("<td>");
                    str.Append(ds.Tables[1].Rows[i][j]);
                    str.Append("</td>");
                }
                formmerIsland = laterIsland;
                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            return str.ToString();

            //table returned from SP is a temporary table ,so colculate columns in SP.
           //return GetTableHtmlByStoredProcedure(ProcedureName, sqlParams);
        }
        #endregion
        //[SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_ShiftView")]
        //public string _MrpPlanOverallViewHtml(DateTime planVersion, string flow)
        //{
        //    var planList = new List<MrpFiMachinePlan>();
        //    string hql = "from MrpFiMachinePlan where PlanVersion =? and ProductLine =? order by Machine";

        //    planList = genericMgr.FindAll<MrpFiMachinePlan>(hql, new object[] { planVersion, flow })
        //       .Where(p => Math.Round(p.ShiftQty, 1) >= 0).ToList();

        //    if (planList.Count() == 0 || planList == null)
        //    {
        //        return "没有记录";
        //    }
        //    var startDate = planList.Select(p => p.PlanDate).Min();
        //    DateTime currentDate = startDate;
        //    var endDate = planList.Select(p => p.PlanDate).Max();
        //    var planBodyList = planList.GroupBy(p => new { p.ProductLine, p.Island, p.Machine })
        //        .OrderBy(p => p.Key.ProductLine).ThenBy(p => p.First().IslandDescription).ThenBy(p => p.Key.Machine).ToList();

        //    string island = planBodyList.First().Key.Island;
        //    int l = 0;
        //    StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr style=\"text-align: center\">");
        //    str.Append("<th style=\"min-width: 40px\" rowspan=\"2\">生产线</th>");
        //    str.Append("<th style=\"min-width: 40px\" rowspan=\"2\">岛区</th>");
        //    str.Append("<th style=\"min-width: 40px\" rowspan=\"2\">模具</th>");
        //    str.Append("<th rowspan=\"2\">模具描述</th>");
        //    while (currentDate <= endDate)
        //    {
        //        str.Append("<th>");
        //        str.Append(currentDate.ToString("dddd", new System.Globalization.CultureInfo("zh-CN")));
        //        str.Append("</th>");
        //        currentDate = currentDate.AddDays(1);
        //    }
        //    currentDate = startDate;
        //    str.Append("</tr><tr>");

        //    while (currentDate <= endDate)
        //    {
        //        str.Append("<th>");
        //        str.Append(currentDate.ToString("MM-dd"));
        //        str.Append("</th>");
        //        currentDate = currentDate.AddDays(1);
        //    }
        //    str.Append("</tr></thead><tbody>");


        //    foreach (var planBody in planBodyList)
        //    {
        //        if (island != planBody.Key.Island)
        //        {
        //            island = planBody.Key.Island;
        //            l++;
        //        }
        //        var dic = planBody.ToDictionary(d => d.PlanDate, d => d.ShiftSplit);
        //        currentDate = startDate;
        //        var style = "";
        //        if (l % 2 == 1)
        //        {
        //            style = "t-alt";
        //        }
        //        str.Append("<tr class=\"" + style + "\">");
        //        str.Append("<td>" + planBody.Key.ProductLine + "</td>");
        //        str.Append("<td>" + planBody.Key.Island + "</td>");
        //        str.Append("<td>" + planBody.Key.Machine + "</td>");
        //        str.Append("<td>" + planBody.First().MachineDescription + "</td>");
        //        while (currentDate <= endDate)
        //        {
        //            str.Append("<td>");
        //            str.Append(dic.ValueOrDefault(currentDate) ?? "-");
        //            str.Append("</td>");
        //            currentDate = currentDate.AddDays(1);
        //        }
        //        str.Append("</tr>");
        //    }
        //    str.Append("</tbody></table>");
        //    return str.ToString();
        //}
        //#endregion

        [SconitAuthorize(Permissions = "Url_Mrp_FiPlanExecutionMonitor_View")]
        public string _GetExportFiPlanMonitor(string prodLine, DateTime planVersion, DateTime startDate, DateTime endDate)
        {
            string ProcedureName = "USP_Search_FIPlanExecutionControl";
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@ProdLine", prodLine);
            sqlParams[1] = new SqlParameter("@PlanVersion", planVersion);
            sqlParams[2] = new SqlParameter("@StartDate", startDate);
            sqlParams[3] = new SqlParameter("@EndDate", endDate);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            return GetTableHtmlByStoredProcedure(ProcedureName, sqlParams);
        }
        #region Export FiPlanMonitor Report
        [SconitAuthorize(Permissions = "Url_Mrp_FiPlanExecutionMonitor_View")]
        public ActionResult ExportFiPlanMonitor(string prodLine, DateTime planVersion, DateTime startDate, DateTime endDate)
        {
            var table = _GetExportFiPlanMonitor(prodLine, planVersion, startDate, endDate);
            return new DownloadFileActionResult(table, "FiPlanExecutionReport.xls");
        }
        #endregion


        #region Demand
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_Demand")]
        public ActionResult Demand()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_Demand")]
        public string _GetDemand(string dateIndex, string flow, string type)
        {
            var planDate = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@PlanDate", planDate);
            var ds = this.genericMgr.GetDatasetByStoredProcedure("USP_Busi_GetMachinePlan", sqlParams);

            var machinePlanList = Utility.IListHelper.DataTableToList<MachinePlan>(ds.Tables[0]);
            if (!string.IsNullOrWhiteSpace(flow))
            {
                machinePlanList = machinePlanList.Where(p => p.Flow == flow).ToList();
            }

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            if (type == "0")
            {
                var machineList = machinePlanList.GroupBy(p => new { p.Machine, p.WeekIndex })
                    .Select(p =>
                    {
                        var m = new MachinePlan();
                        var plan = p.First();
                        m.Flow = plan.Flow;
                        m.IslandDescription = plan.IslandDescription;
                        m.Machine = p.Key.Machine;
                        m.MachineDescription = plan.MachineDescription;
                        if (plan.MachineType == (int)CodeMaster.MachineType.Kit)
                        {
                            m.MachineQty = Math.Round(p.Max(q => q.Qty) / plan.ShiftQuota, 1);
                            m.NetMachineQty = Math.Round(p.Max(q => q.NetQty) / plan.ShiftQuota, 1);
                        }
                        else
                        {
                            m.MachineQty = Math.Round(p.Sum(q => q.Qty) / plan.ShiftQuota, 1);
                            m.NetMachineQty = Math.Round(p.Sum(q => q.NetQty) / plan.ShiftQuota, 1);
                        }
                        m.MachineType = plan.MachineType;
                        m.ShiftQuota = plan.ShiftQuota;
                        m.WeekIndex = p.Key.WeekIndex;
                        return m;
                    });
                var weekIndexList = machineList.Select(p => p.WeekIndex).Distinct().OrderBy(p => p);

                str.Append("<th>"+Resources.EXT.ControllerLan.Con_ProductionLine+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_Machine+"</th>");
                str.Append("<th>" + Resources.EXT.ControllerLan.Con_MachineDescription + "</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_Type+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_Quota+"</th>");
                for (int i = 0; i < weekIndexList.Count(); i++)
                {
                    var weekIndex = weekIndexList.ElementAt(i);
                    if (i == 0)
                    {
                        str.Append(string.Format("<th>"+Resources.EXT.ControllerLan.Con_Gross+"</th>", weekIndex));
                        str.Append(string.Format("<th>"+Resources.EXT.ControllerLan.Con_Net + "</th>", weekIndex));
                    }
                    else
                    {
                        str.Append(string.Format("<th>{0}</th>", weekIndex));
                    }
                }
                str.Append("</tr></thead><tbody>");

                var machineGroupList = machineList.GroupBy(p => p.Machine);
                int l = 0;
                string islandDescription = machineGroupList.First().First().IslandDescription;
                foreach (var machineGroup in machineGroupList)
                {
                    var plan = machineGroup.First();
                    if (islandDescription != plan.IslandDescription)
                    {
                        islandDescription = plan.IslandDescription;
                        l++;
                    }
                    if (l % 2 == 1)
                    {
                        str.Append("<tr class=\"t-alt\">");
                    }
                    else
                    {
                        str.Append("<tr>");
                    }

                    str.Append(string.Format("<td>{0}</td>", plan.Flow));
                    str.Append(string.Format("<td>{0}</td>", machineGroup.Key));
                    str.Append(string.Format("<td>{0}</td>", plan.MachineDescription));
                    str.Append(string.Format("<td>{0}</td>", plan.MachineType == 1 ? "成套" : "单件"));
                    str.Append(string.Format("<td>{0}</td>", plan.ShiftQuota));

                    var planDic = machineGroup.GroupBy(p => p.WeekIndex).ToDictionary(d => d.Key, d => d.First());
                    for (int i = 0; i < weekIndexList.Count(); i++)
                    {
                        var weekIndex = weekIndexList.ElementAt(i);
                        var _plan = planDic.ValueOrDefault(weekIndex) ?? new MachinePlan();
                        str.Append(string.Format("<td>{0}</td>", _plan.MachineQty > 0 ? _plan.MachineQty : 0));
                        if (i == 0)
                        {
                            str.Append(string.Format("<td>{0}</td>", _plan.NetMachineQty > 0 ? _plan.NetMachineQty : 0));
                        }
                    }
                    str.Append("</tr>");
                }
                str.Append("</tbody></table>");
            }
            else
            {
                //Flow	WeekIndex	Item	IslandDescription	MachineDescription	Machine	MachineType	ShiftQuota	Qty	MachineQty	
                //StartQty	SafeStock	OutQty	InQty	EndQty	NetQty
                machinePlanList = machinePlanList.Where(p => p.WeekIndex == dateIndex).ToList();
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_ProductionLine+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_Machine+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_MachineDescription+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_Type+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_Quota+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_Item+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_ItemDescription+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_CurrentInventory+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_SafeInventory+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_WaitingForReceiving+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_WaitingForShipping+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_EndInventory+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_GrossRequirement+"</th>");
                str.Append("<th>"+Resources.EXT.ControllerLan.Con_NetRequirement+"</th>");
                str.Append("</tr></thead><tbody>");
                string machine = machinePlanList.First().Machine;
                int l = 0;
                foreach (var machinePlan in machinePlanList)
                {
                    if (machine != machinePlan.Machine)
                    {
                        machine = machinePlan.Machine;
                        l++;
                    }
                    if (l % 2 == 1)
                    {
                        str.Append("<tr class=\"t-alt\">");
                    }
                    else
                    {
                        str.Append("<tr>");
                    }
                    str.Append(string.Format("<td>{0}</td>", machinePlan.Flow));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.Machine));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.MachineDescription));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.MachineType == (int)CodeMaster.MachineType.Kit ? "成套" : "单件"));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.ShiftQuota));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.Item));
                    str.Append(string.Format("<td>{0}</td>", itemMgr.GetCacheItem(machinePlan.Item).FullDescription));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.StartQty.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.SafeStock.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.InQty.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.OutQty.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.EndQty.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", machinePlan.Qty.ToString("0.##")));
                    str.Append(string.Format((machinePlan.NetQty < 0 ? "<td style='background-color:YellowGreen'>{0}</td>" : "<td>{0}</td>"), machinePlan.NetQty.ToString("0.##")));
                    str.Append("</tr>");
                }
                str.Append("</tbody></table>");
            }
            return str.ToString();
        }
        #endregion
        #region Export Fi Demand
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanFi_Demand")]
        public ActionResult ExportFiDemand(string dateIndex, string flow, string type)
        {
            var table = _GetDemand(dateIndex, flow, type);
            return new DownloadFileActionResult(table, "FiDemand.xls");
        }
        #endregion
        public string _GetDateRange(string planversion)
        {
            var dateRange = genericMgr.FindAllWithNativeSql<object[]>(@"select MIN(PlanDate) As DateFrom, MAX(PlanDate) As DateTo 
                            from MRP_MrpFiShiftPlan where PlanVersion=? ", Convert.ToDateTime(planversion));
            dateRange = dateRange ?? new List<object[]>();
            return string.Format(Resources.EXT.ControllerLan.Con_VersionStartTimeEndTime, dateRange.FirstOrDefault()[0].ToString(), dateRange.FirstOrDefault()[1].ToString());
        }
    }
    class MachinePlan
    {
        public string Flow { get; set; }
        public string IslandDescription { get; set; }
        public string MachineDescription { get; set; }
        public string WeekIndex { get; set; }
        public string Item { get; set; }
        public string Machine { get; set; }
        public int MachineType { get; set; }
        public decimal ShiftQuota { get; set; }
        public decimal Qty { get; set; }
        public decimal StartQty { get; set; }
        public decimal SafeStock { get; set; }
        public decimal OutQty { get; set; }
        public decimal InQty { get; set; }
        public decimal EndQty { get; set; }
        public decimal NetQty { get; set; }
        public decimal MachineQty { get; set; }
        public decimal NetMachineQty { get; set; }
    }
}
