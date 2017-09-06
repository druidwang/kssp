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
using com.Sconit.Entity.SCM;
using AutoMapper;
using System.Data.SqlClient;
using System.Data;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MrpPlanMiController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public IPlanMgr planMgr { get; set; }
        public IMrpOrderMgr mrpOrderMgr { get; set; }
        //public IItemMgr itemMgr { get; set; }
        public IMrpMgr mrpMgr { get; set; }

        #region
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_Shift")]
        public ActionResult Shift()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_PlanTrace")]
        public ActionResult TrackView()
        {
            return View();
        }
        
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_Shift")]
        public ActionResult _ShiftPlanList(string flow, DateTime planDate, string shift)
        {
            ViewBag.Flow = flow;
            //ViewBag.PlanVersion = planVersion;
            ViewBag.PlanDate = planDate;
            ViewBag.Shift = shift;

            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_Shift")]
        public ActionResult _SelectShiftList(string flow, DateTime? planDate, string shift)
        {
            IList<MrpMiShiftPlan> planList = new List<MrpMiShiftPlan>();

            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(shift) && planDate.HasValue)
            {
                //var orderNos = mrpOrderMgr.GetActiveOrder(flow, planDate.Value, shift);
                //if (orderNos != null && orderNos.Count() > 0)
                //{
                //    SaveWarningMessage(string.Format("此路线{0}班次{1}已经有生产单{2}。请先取消对应的生产单。", flow, shift, orderNos[0]));
                //}
                //else
                //{
                string hql = @"select a.* from MRP_MrpMiShiftPlan a join MRP_MrpMiDateIndex b 
                      on a.CreateDate = b.CreateDate and a.ProductLine = b.ProductLine and a.PlanDate = b.PlanDate
                      where b.ProductLine = ? and b.PlanDate =? and b.IsActive =? and a.Shift = ? ";
                planList = genericMgr.FindEntityWithNativeSql<MrpMiShiftPlan>(hql, new object[] { flow, planDate.Value, true, shift });
                foreach (var plan in planList)
                {
                    var item = this.genericMgr.FindById<Item>(plan.Item);
                    plan.ItemDescription = item.Description;
                    //plan.Uom = item.Uom;
                    plan.CurrentCheQty = Math.Round(plan.CheQty);
                    if (!string.IsNullOrWhiteSpace(plan.HuTo))
                    {
                        plan.HuToDescription = this.genericMgr.FindById<HuTo>(plan.HuTo).Description;
                    }
                }
                planList = planList.OrderBy(p => p.Sequence).ToList();
            }
            return PartialView(new GridModel(planList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_Shift")]
        public JsonResult _SaveShift(int[] Ids, int[] Sequences, string[] Items, string[] HuTos, double[] CurrentQtys)
        {
            try
            {
                var planList = GetMrpMiShiftPlanList(Ids, Sequences, Items, HuTos, CurrentQtys);

                var leftMinutes = mrpMgr.AdjustMrpMiShiftPlan(planList);

                if (leftMinutes >= 0)
                {
                    SaveSuccessMessage(string.Format(Resources.EXT.ControllerLan.Con_RemainedMinutesOfIsm, leftMinutes));
                }
                else
                {
                    SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_ExceedMinutesOfIsm, -leftMinutes));
                }

                object obj = new { };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        private IList<MrpMiShiftPlan> GetMrpMiShiftPlanList(int[] Ids, int[] Sequences, string[] Items, string[] HuTos, double[] CurrentQtys)
        {
            var planList = genericMgr.FindAllIn<MrpMiShiftPlan>
           ("from MrpMiShiftPlan where Id in(? ", Ids.Where(p => p > 0).Select(p => (object)p));

            for (int i = 0; i < Ids.Length; i++)
            {
                if (Ids[i] > 0)
                {
                    var plan = planList.Single(p => p.Id == Ids[i]);
                    plan.Sequence = Sequences[i];
                    plan.HuTo = HuTos[i];
                    plan.CurrentCheQty = CurrentQtys[i];
                }
                else
                {
                    var newPlan = new MrpMiShiftPlan();
                    newPlan.Item = Items[i];
                    var flowDetails = genericMgr.FindAll<FlowDetail>("from FlowDetail where Flow = ? and Item = ?",
                        new object[] { planList.First().ProductLine, newPlan.Item });
                    if (flowDetails == null || flowDetails.Count() == 0)
                    {
                        throw new BusinessException(string.Format(Resources.EXT.ControllerLan.Con_TheItemIsNotInFlowNow, newPlan.Item, planList.First().ProductLine));
                    }

                    //newPlan.AdjustQty = 0;
                    newPlan.Bom = string.IsNullOrWhiteSpace(flowDetails.First().Bom) ? newPlan.Item : flowDetails.First().Bom;
                    //newPlan.CheQty = 0;
                    newPlan.UnitCount = (double)flowDetails.First().MinUnitCount;
                    newPlan.LocationFrom = flowDetails.First().LocationFrom;
                    newPlan.LocationTo = flowDetails.First().LocationTo;
                    newPlan.WorkHour = this.itemMgr.GetCacheItem(newPlan.Item).WorkHour;

                    if (string.IsNullOrWhiteSpace(newPlan.LocationFrom)
                        || string.IsNullOrWhiteSpace(newPlan.LocationTo))
                    {
                        var flowMaster = this.genericMgr.FindById<FlowMaster>(flowDetails.First().Flow);
                        if (string.IsNullOrWhiteSpace(newPlan.LocationFrom))
                        {
                            newPlan.LocationFrom = flowMaster.LocationFrom;
                        }
                        if (string.IsNullOrWhiteSpace(newPlan.LocationTo))
                        {
                            newPlan.LocationTo = flowMaster.LocationTo;
                        }
                    }

                    //newPlan.ParentItem = null;
                    newPlan.PlanDate = planList.First().PlanDate;
                    newPlan.PlanVersion = planList.First().PlanVersion;
                    newPlan.ProductLine = planList.First().ProductLine;
                    //newPlan.Qty =0;
                    //newPlan.Sequence = flowDetails.First().Sequence;
                    newPlan.Uom = flowDetails.First().Uom;

                    newPlan.Sequence = Sequences[i];
                    newPlan.HuTo = HuTos[i];
                    newPlan.CurrentCheQty = CurrentQtys[i];
                    planList.Add(newPlan);
                }
            }
            return planList;
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_Shift")]
        public ActionResult _CreateOrder(int[] Ids, int[] Sequences, string[] Items, string[] HuTos, double[] CurrentQtys)
        {
            try
            {
                var planList = GetMrpMiShiftPlanList(Ids, Sequences, Items, HuTos, CurrentQtys);
                if (planList.Count > 0)
                {
                    var orderNos = mrpOrderMgr.GetActiveOrder(planList.First().ProductLine, planList.First().StartTime.Date, planList.First().Shift);
                    if (orderNos != null && orderNos.Count() > 0)
                    {
                        SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_PleaseCancelCorrespondingProductionOrderOfThisFlowShiftExits,
                           planList.First().ProductLine, planList.First().StartTime.Date, planList.First().Shift));
                    }
                    else
                    {
                        var leftMinutes = mrpMgr.AdjustMrpMiShiftPlan(planList);

                        if (leftMinutes >= 0)
                        {
                            SaveSuccessMessage(string.Format(Resources.EXT.ControllerLan.Con_RemainedMinutesOfIsm, leftMinutes));
                        }
                        else
                        {
                            SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_ExceedMinutesOfIsm, -leftMinutes));
                        }

                        var orderMaster = mrpOrderMgr.CreateMiOrder(planList);
                        SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CreateProductionOrderSuccessfully);
                        return Json(orderMaster.OrderNo);
                    }
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(null);
        }
        #endregion

        #region ShiftView

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_ShiftView")]
        public ActionResult ShiftView()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_ShiftPlanSearch")]
        public ActionResult ShiftPlanSearch()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_ShiftView")]
        public string _ShiftViewList(DateTime planVersion, string flow)
        {
            return planMgr.GetMiDailyPlanView(planVersion, flow).ToString();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_Shift")]
        public JsonResult _Check(int[] Ids, int[] Sequences, string[] Items, string[] HuTos, double[] CurrentQtys)
        {
            try
            {
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ContainerIsEnough);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_FilterCapacityIsEnough);
                return Json(new object());
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_ShiftView")]
        public string _GetShiftView(DateTime planVersion, string flow)
        {
            return planMgr.GetMiDailyPlanView(planVersion, flow).ToString();
        }
        #endregion

        #region Export MiPlanSearchView
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_ShiftView")]
        public ActionResult Export(DateTime planDate, string flow, int tabType)
        {
            string viewType = "Day";
            if (tabType == 1) viewType = "Shift";
            var table = _GetShiftPlanSearchView(planDate, flow, viewType);
            return new DownloadFileActionResult(table, "MiPlanSearchView.xls");
        }
        #endregion
        #region PlanTrackSearch
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_PlanTrace")]
        public string _GetTrackView(DateTime? formerVersion, DateTime? latterVersion)
        {
            if (formerVersion.Value >= latterVersion.Value)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_CurrentVersionNumberMustGreaterFormerVersionNumber);
                return "";
            }
            int tableColumnCount;
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@FormerVersion", formerVersion);
            sqlParams[1] = new SqlParameter("@LatterVersion", latterVersion);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Busi_MRP_MiShipDailyPlanTrack", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable2\" width=\"100%\"><thead><tr>");

            #region Head
            str.Append("<th>");
            str.Append(ds.Tables[1].Columns[0].ColumnName);
            str.Append("</th>");
            str.Append("<th>");
            str.Append(ds.Tables[1].Columns[1].ColumnName);
            str.Append("</th>");
            for (int i = 2; i < tableColumnCount; i++)
            {
                str.Append("<th>");
                str.Append(ds.Tables[1].Columns[i].ColumnName.Substring(5, 5));
                str.Append("</th>");
            }
    
            str.Append("</tr></thead><tbody>");
            #endregion
            int l = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
            {
                l++;
                trcss = "";
                if (l % 2 == 0)
                {
                    trcss = "t-alt";
                }
                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");
                for (int j = 0; j < tableColumnCount; j++)
                {
                    if (ds.Tables[1].Rows[i][j].ToString() != ds.Tables[2].Rows[i][j].ToString())
                    {
                        //Added YellowGreen  ，Deleted Orange ,Changed Yellow & Title
                        if (ds.Tables[1].Rows[i][j].ToString() == "0")
                        {
                            str.Append("<td style='background-color:YellowGreen'>");
                        }
                        else if (ds.Tables[2].Rows[i][j].ToString() == "0")
                        {
                            str.Append("<td style='background-color:Orange'>");
                        }
                        else
                        {
                            str.Append("<td style='background-color:Yellow' title='" + ds.Tables[1].Rows[i][j] + "'>");
                        }
                    }
                    else
                    {
                        str.Append("<td>");
                    }
                    str.Append(ds.Tables[2].Rows[i][j].ToString() == "0" ? (ds.Tables[1].Rows[i][j].ToString() != "0" ? ds.Tables[1].Rows[i][j] : "") : ds.Tables[2].Rows[i][j]);
                    str.Append("</td>");
                }

                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            return str.ToString();

        }
        #endregion
        #region ShiftPlanSearch
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_ShiftView")]
        public string _GetShiftPlanSearchView(DateTime planDate, string flow,string viewType )
        {
            int tableColumnCount;
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@PlandateStart", planDate);
            sqlParams[1] = new SqlParameter("@flow", flow);
            sqlParams[2] = new SqlParameter("@Type", viewType);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Report_MRP_MIShiftPlanSearch", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable2\" width=\"100%\"><thead><tr>");

            #region Head
            str.Append("<th rowspan=\"" + (viewType == "Shift" ? 2 : 1) + "\" style=\"text-align:center;\" >"); str.Append(Resources.EXT.ControllerLan.Con_Item); str.Append("</th>");
            str.Append("<th rowspan=\"" + (viewType == "Shift" ? 2 : 1) + "\" style=\"text-align:center;min-width:80px;\" >"); str.Append(Resources.EXT.ControllerLan.Con_ItemDescription); str.Append("</th>");
            for (int i = 2; i < tableColumnCount; i++)
            {
                str.Append("<th colspan=\"" + (viewType == "Shift" ? 3 : 1) + "\" style=\"text-align:center\" >");
                str.Append(ds.Tables[1].Columns[i].ColumnName.Substring(0, 5));
                str.Append("</th>");
                i += viewType == "Shift" ? 2 : 0;
            }
            if (viewType == "Shift")
            {
                str.Append("</tr>");
                str.Append("<tr>");
                for (int i = 2; i < tableColumnCount; i++)
                {
                    str.Append("<th>");
                    str.Append(ds.Tables[1].Columns[i].ColumnName.Substring(6, 4));
                    str.Append("</th>");
                }
            }
            str.Append("</tr></thead><tbody>");
            #endregion
            int l = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                l++;
                trcss = "";
                if (l % 2 == 0)
                {
                    trcss = "t-alt";
                }
                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");
                for (int j = 0; j < tableColumnCount; j++)
                {
                    str.Append("<td>");
                    str.Append(ds.Tables[1].Rows[i][j].ToString() == "0" ? "" : ds.Tables[1].Rows[i][j]);
                    str.Append("</td>");
                }

                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            return str.ToString();

        }
        #endregion
        #region DailyPlan
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_DailyPlan")]
        public ActionResult DailyPlan()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_DailyPlan")]
        public ActionResult _DailyPlanList(string flow, DateTime planVersion, DateTime planDate)
        {
            ViewBag.Flow = flow;
            ViewBag.PlanVersion = planVersion;
            ViewBag.PlanDate = planDate;
            ViewBag.Released = Resources.EXT.ControllerLan.Con_Released;

            var mrpMiDateIndexs = this.genericMgr.FindAll<MrpMiDateIndex>
                (" from MrpMiDateIndex where PlanDate=? and ProductLine=? and PlanVersion=? ",
                new object[] { planDate, flow, planVersion });
            if (mrpMiDateIndexs != null && mrpMiDateIndexs.Count > 0)
            {
                ViewBag.Released = Resources.EXT.ControllerLan.Con_ReleasedAgain;
            }
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_DailyPlan")]
        public JsonResult _SaveDailyPlan(int[] Ids, int[] Sequences, string[] Items, string[] HuTos, double[] CurrentQtys)
        {
            try
            {
                if (Ids != null && Ids.Length > 0)
                {
                    var planList = GetMrpMiPlanList(Ids, Sequences, Items, HuTos, CurrentQtys);
                    double leftMinutes = this.mrpMgr.AdjustMrpMiPlan(planList);
                    if (leftMinutes >= 0)
                    {
                        SaveSuccessMessage(string.Format(Resources.EXT.ControllerLan.Con_RemainedMinutesOfIsm, leftMinutes));
                    }
                    else
                    {
                        SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_ExceedMinutesOfIsm, -leftMinutes));
                    }
                }
                object obj = new { };
                //SaveSuccessMessage("保存计划成功。");
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        private IList<MrpMiPlan> GetMrpMiPlanList(int[] Ids, int[] Sequences, string[] Items, string[] HuTos, double[] CurrentQtys)
        {
            var planList = genericMgr.FindAllIn<MrpMiPlan>
                ("from MrpMiPlan where Id in(? ", Ids.Where(p => p > 0).Select(p => (object)p));

            for (int i = 0; i < Ids.Length; i++)
            {
                if (Ids[i] > 0)
                {
                    var plan = planList.Single(p => p.Id == Ids[i]);
                    plan.Sequence = Sequences[i];
                    plan.HuTo = HuTos[i];
                    plan.CurrentCheQty = CurrentQtys[i];
                }
                else
                {
                    var newPlan = new MrpMiPlan();
                    newPlan.Item = Items[i];
                    var flowDetails = genericMgr.FindAll<FlowDetail>("from FlowDetail where Flow = ? and Item = ?",
                        new object[] { planList.First().ProductLine, newPlan.Item });
                    if (flowDetails == null || flowDetails.Count() == 0)
                    {
                        throw new BusinessException(string.Format(Resources.EXT.ControllerLan.Con_TheItemIsNotInFlowNow, newPlan.Item, planList.First().ProductLine));
                    }

                    //newPlan.AdjustQty = 0;
                    newPlan.Bom = string.IsNullOrWhiteSpace(flowDetails.First().Bom) ? newPlan.Item : flowDetails.First().Bom;
                    //newPlan.CheQty = 0;
                    newPlan.UnitCount = (double)flowDetails.First().UnitCount;
                    newPlan.InvQty = 0;
                    newPlan.LocationFrom = flowDetails.First().LocationFrom;
                    newPlan.LocationTo = flowDetails.First().LocationTo;

                    if (string.IsNullOrWhiteSpace(newPlan.LocationFrom)
                        || string.IsNullOrWhiteSpace(newPlan.LocationTo))
                    {
                        var flowMaster = this.genericMgr.FindById<FlowMaster>(flowDetails.First().Flow);
                        if (string.IsNullOrWhiteSpace(newPlan.LocationFrom))
                        {
                            newPlan.LocationFrom = flowMaster.LocationFrom;
                        }
                        if (string.IsNullOrWhiteSpace(newPlan.LocationTo))
                        {
                            newPlan.LocationTo = flowMaster.LocationTo;
                        }
                    }

                    newPlan.MaxStock = (double)flowDetails.First().MaxStock;
                    newPlan.MrpPriority = flowDetails.First().MrpPriority;
                    //newPlan.ParentItem = null;
                    newPlan.PlanDate = planList.First().PlanDate;
                    newPlan.PlanVersion = planList.First().PlanVersion;
                    newPlan.ProductLine = planList.First().ProductLine;
                    //newPlan.Qty =0;
                    newPlan.SafeStock = (double)flowDetails.First().SafeStock;
                    //newPlan.Sequence = flowDetails.First().Sequence;
                    newPlan.Uom = flowDetails.First().Uom;
                    newPlan.WorkHour = itemMgr.GetCacheItem(newPlan.Item).WorkHour;

                    newPlan.Sequence = Sequences[i];
                    newPlan.HuTo = HuTos[i];
                    newPlan.CurrentCheQty = CurrentQtys[i];
                    planList.Add(newPlan);
                }
            }
            return planList;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_DailyPlan")]
        public JsonResult _ReleaseDailyPlan(int[] Ids, int[] Sequences, string[] Items, string[] HuTos, double[] CurrentQtys)
        {
            try
            {
                if (Ids != null && Ids.Length > 0)
                {
                    var planList = GetMrpMiPlanList(Ids, Sequences, Items, HuTos, CurrentQtys);
                    this.mrpMgr.ReleaseMiPlan(planList);
                }
                object obj = new { };
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_PlanReleasedSuccessfullyDailyPlanTransformToShiftPlanSuccessfully);
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_DailyPlan")]
        public ActionResult _SelectDailyPlanList(DateTime? planVersion, string flow, DateTime? planDate)
        {
            IList<MrpMiPlan> planList = new List<MrpMiPlan>();

            if (!string.IsNullOrEmpty(flow) && planVersion.HasValue && planDate.HasValue)
            {
                string hql = @"from MrpMiPlan where PlanVersion =? and ProductLine = ? and PlanDate =?  
                                order by Sequence ";
                planList = genericMgr.FindAll<MrpMiPlan>(hql, new object[] { planVersion.Value, flow, planDate.Value });

                foreach (var plan in planList)
                {
                    var item = this.itemMgr.GetCacheItem(plan.Item);
                    plan.ItemDescription = item.Description;
                    plan.Uom = item.Uom;
                    plan.CurrentCheQty = Math.Round(plan.CheQty, 2);
                    //plan.CurrentCheQty = Math.Round(plan.TotalQty / (plan.BatchSize * plan.UnitCount)) * plan.BatchSize;

                    if (!string.IsNullOrWhiteSpace(plan.HuTo))
                    {
                        plan.HuToDescription = this.genericMgr.FindById<HuTo>(plan.HuTo).Description;
                    }
                }
                ViewBag.TotalCheQty = planList.Sum(p => p.CurrentCheQty);
                ViewBag.TotalWorkHours = planList.Sum(p => p.WorkHour * p.CurrentCheQty);
            }
            return PartialView(new GridModel(planList));
        }

        [GridAction]
        public ActionResult _SelectMiPlanDetailList(int Id)
        {
            var planDetailList = this.genericMgr.FindAll<MrpMiPlanDetail>(" from MrpMiPlanDetail where PlanId = ?", Id);
            if (planDetailList != null)
            {
                foreach (var planDetail in planDetailList)
                {
                    if (!string.IsNullOrWhiteSpace(planDetail.SourceFlow))
                    {
                        planDetail.SourceFlowDescription = this.genericMgr.FindById<FlowMaster>(planDetail.SourceFlow).Description;
                    }
                    if (!string.IsNullOrWhiteSpace(planDetail.SourceParty))
                    {
                        planDetail.SourcePartyDescription = this.genericMgr.FindById<Party>(planDetail.SourceParty).Name;
                    }
                    if (!string.IsNullOrWhiteSpace(planDetail.ParentItem))
                    {
                        planDetail.ParentItemDescription = this.itemMgr.GetCacheItem(planDetail.ParentItem).Description;
                    }
                }
            }
            return View(new GridModel(planDetailList));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanMi_DailyPlan")]
        public JsonResult _WebMrpMiPlan(string flow, string itemCode)
        {
            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(itemCode))
            {
                var plan = new MrpMiPlan();
                var item = itemMgr.GetCacheItem(itemCode);
                plan.Item = item.Code;
                plan.ItemDescription = item.Description;
                plan.Uom = item.Uom;
                var flowDetails = genericMgr.FindAll<FlowDetail>("from FlowDetail where Flow = ? and Item = ?",
                    new object[] { flow, itemCode });
                if (flowDetails == null || flowDetails.Count() == 0)
                {
                    SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_TheItemIsNotInFlowNow, item, flow));
                    return null;
                }
                else
                {
                    plan.UnitCount = (float)flowDetails.First().UnitCount;
                }
                return this.Json(plan);
            }
            return null;
        }
        #endregion
    }
}
