using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Services.Transaction;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.SCM;
using System.Text;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Service.MRP;
using System.IO;
using System.Text.RegularExpressions;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MrpScheduleController : WebAppBaseController
    {
        public IMrpMgr mrpMgr { get; set; }
        //public IGenericMgr genericMgr { get; set; }

        private static string selectCountStatement = "select count(*) from MrpPlanMaster as m";

        private static string selectStatement = "select m from MrpPlanMaster as m";

        #region Fi
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Fi")]
        public ActionResult Fi()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Ex")]
        public ActionResult Ex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Mi")]
        public ActionResult Mi()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Fi")]
        public ActionResult FiList(GridCommand command, MrpPlanMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Mi")]
        public ActionResult MiList(GridCommand command, MrpPlanMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Ex")]
        public ActionResult ExList(GridCommand command, MrpPlanMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Fi")]
        public ActionResult _AjaxList(GridCommand command, MrpPlanMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("SourcePlanVersion", searchModel.PlanVersion, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("DateIndex", searchModel.DateIndex, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsRelease", searchModel.IsRelease, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ResourceGroup", searchModel.ResourceGroup, "m", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by PlanVersion desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return PartialView(GetAjaxPageData<MrpPlanMaster>(searchStatementModel, command));
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Fi,Url_Mrp_MrpSchedule_Mi,Url_Mrp_MrpSchedule_Ex")]
        public JsonResult _RunMrpPlanMaster(DateTime planVersion, com.Sconit.CodeMaster.ResourceGroup resourceGroup, string dateIndex)
        {
            try
            {
                string returnStr = string.Empty;
                DateTime newPlanVersion = DateTime.Now;
                AsyncRun(newPlanVersion, planVersion, resourceGroup, dateIndex, this.CurrentUser);
                if (resourceGroup == com.Sconit.CodeMaster.ResourceGroup.FI)
                {
                    returnStr = string.Format(Resources.EXT.ControllerLan.Con_RunningFIShiftVersion, newPlanVersion);
                }
                else if (resourceGroup == com.Sconit.CodeMaster.ResourceGroup.MI)
                {
                    returnStr = string.Format(Resources.EXT.ControllerLan.Con_RunningMIShiftVersion, newPlanVersion);
                }
                else
                {
                    returnStr = string.Format(Resources.EXT.ControllerLan.Con_RunningEXShiftVersion, newPlanVersion);
                }
                SaveSuccessMessage(returnStr);
                return Json(new { });
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(null);
        }

        private delegate void Async(DateTime newPlanVersion, DateTime planVersion, CodeMaster.ResourceGroup resourceGroup, string dateIndex, User user);
        private void AsyncRun(DateTime newPlanVersion, DateTime planVersion, CodeMaster.ResourceGroup resourceGroup, string dateIndex, User user)
        {
            Async async = new Async(mrpMgr.RunMrp);
            async.BeginInvoke(newPlanVersion, planVersion, resourceGroup, dateIndex, user, null, null);
        }

        #endregion

        public string _GetRunLog()
        {
            try
            {
                FileStream fs = new FileStream(@"D:\logs\Sconit5_Shenya\RunMrp.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                string log = sr.ReadToEnd();
                fs.Close();
                string[] lines = Regex.Split(log, "\r\n", RegexOptions.IgnoreCase);
                string testLog = string.Empty;
                var startIndex = lines.Select((n, i) => new { n = n, i = i + 1 }).Where(n => n.n.Contains("---**---")).Last().i;
                foreach (var line in lines.Skip(startIndex - 1).Reverse().Take(200))
                {
                    testLog += line + "</br>";
                }
                return testLog;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Fi,Url_Mrp_MrpSchedule_Mi")]
        public ActionResult Save(string planVersion, bool isRelease)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(planVersion);
                MrpPlanMaster mrpPlanMaster = this.genericMgr.FindById<MrpPlanMaster>(dt);
                mrpPlanMaster.IsRelease = isRelease;
                mrpPlanMaster.LastModifyUserId = this.CurrentUser.Id;
                mrpPlanMaster.LastModifyUserName = this.CurrentUser.FullName;
                mrpPlanMaster.LastModifyDate = DateTime.Now;

                genericMgr.Update(mrpPlanMaster);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedSuccessfully);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("Edit", new { planVersion = planVersion });
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Ex")]
        public ActionResult ReleaseExPlan(string planVersion, bool isRelease)
        {
            try
            {
                Save(planVersion, isRelease);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("Edit", new { planVersion = planVersion });
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Fi,Url_Mrp_MrpSchedule_Mi,Url_Mrp_MrpSchedule_Ex")]
        public ActionResult Edit(string planVersion)
        {
            DateTime dt = Convert.ToDateTime(planVersion);
            MrpPlanMaster mrpPlanMaster = this.genericMgr.FindById<MrpPlanMaster>(dt);
            switch ((int)mrpPlanMaster.ResourceGroup)
            {
                case 10:
                mrpPlanMaster.ShortWord="Mi";break ;
                case 20:
                mrpPlanMaster.ShortWord = "Ex";break ;
                case 30:
                mrpPlanMaster.ShortWord = "Fi";break ;
                default:
                    mrpPlanMaster.ShortWord = "Fi";break ;
            }
            return View(mrpPlanMaster);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Fi,Url_Mrp_MrpSchedule_Mi,Url_Mrp_MrpSchedule_Ex")]
        public ActionResult _MrpPlanMasterError(string planVersion)
        {
            IList<MrpLog> mrpLogList = genericMgr.FindAll<MrpLog>(" from MrpLog as m where m.PlanVersion=? order by ErrorLevel ",
                Convert.ToDateTime(planVersion));
            if (mrpLogList.Count > 100)
            {
                mrpLogList = mrpLogList.Where(p => p.ErrorLevel != "Info").ToList();
            }
            return PartialView(mrpLogList);
        }

        #region

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Purchase")]
        public ActionResult Purchase()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Purchase")]
        public ActionResult _AjaxPurchaseList(GridCommand command, PurchasePlanMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("SnapTime", searchModel.SnapTime, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("DateType", searchModel.DateType, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PlanVersion", searchModel.PlanVersion, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsRelease", searchModel.IsRelease, "m", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by PlanVersion desc";
            }

            searchStatementModel.SelectCountStatement = "select count(*) from PurchasePlanMaster as m";
            searchStatementModel.SelectStatement = "select m from PurchasePlanMaster as m";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return PartialView(GetAjaxPageData<PurchasePlanMaster>(searchStatementModel, command));
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Purchase")]
        public JsonResult _RunPurchase(DateTime planVersion)
        {
            try
            {
                AsyncRun(planVersion);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_RunningPurchaseDailPlan);
                return Json(new { });
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(null);
        }

        private void AsyncRun(DateTime planVersion)
        {
            Async async = new Async(mrpMgr.RunMrp);
            DateTime newPlanVersion = DateTime.Now;
            User user = this.CurrentUser;
            async.BeginInvoke(newPlanVersion, planVersion, CodeMaster.ResourceGroup.Other, null, user, null, null);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Purchase")]
        public ActionResult PurchaseEdit(int id)
        {
            PurchasePlanMaster mrpPlanMaster = this.genericMgr.FindById<PurchasePlanMaster>(id);
            return View(mrpPlanMaster);
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Purchase")]
        public ActionResult PurchaseList(GridCommand command, PurchasePlanMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSchedule_Purchase")]
        public ActionResult BatchPurchaseEdit(int[] ids, bool isRelease)
        {
            foreach (var id in ids)
            {
                PurchasePlanMaster purchasePlanMaster = this.genericMgr.FindById<PurchasePlanMaster>(id);
                purchasePlanMaster.IsRelease = !isRelease;
                genericMgr.Update(purchasePlanMaster);
            }
            if (isRelease)
            {
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CancelledReleasedSuccessfully);
            }
            else
            {
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ReleasedSuccessfully);
            }
            return RedirectToAction("PurchaseList");
        }

        #endregion
    }
}
