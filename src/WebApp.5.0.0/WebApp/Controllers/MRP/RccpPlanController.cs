using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.PRD;
using com.Sconit.Service;
using com.Sconit.Service.MRP;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace com.Sconit.Web.Controllers.MRP
{
    public class RccpPlanController : WebAppBaseController
    {
        //
        // GET: /CustomerPlan/
        public IPlanMgr planMgr { get; set; }
        public IRccpMgr rccpMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        //public IItemMgr itemMgr { get; set; }

        //public IGenericMgr genericMgr { get; set; }
        private static string selectCountStatement = "select count(*) from RccpPlan as r";

        private static string selectStatement = "select r from RccpPlan as r";

        private static string selectRccpPlanMasterCountStatement = "select count(*) from RccpPlanMaster as r";

        private static string selectRccpPlanMasterStatement = "select r from RccpPlanMaster as r";

        #region Public Method
        [SconitAuthorize(Permissions = "Url_RccpPlan_Edit")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public ActionResult Run()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public JsonResult _RunRccpPlan(string snapTime, com.Sconit.CodeMaster.TimeUnit dateType, string dateIndex)
        {
            try
            {
                DateTime planVersion = DateTime.Now;
                AsyncRun(planVersion, DateTime.Parse(snapTime), dateType, dateIndex, this.CurrentUser);

                SaveSuccessMessage(string.Format(Resources.EXT.ControllerLan.Con_RunningRccPlan, planVersion));
                return Json(new { });
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(null);
        }

        private delegate void Async(DateTime planVersion, DateTime snapTime, com.Sconit.CodeMaster.TimeUnit dateType, string dateIndex, Sconit.Entity.ACC.User user);
        private void AsyncRun(DateTime planVersion, DateTime snapTime, com.Sconit.CodeMaster.TimeUnit dateType, string dateIndex, Sconit.Entity.ACC.User user)
        {
            Async async = new Async(rccpMgr.RunRccp);
            async.BeginInvoke(planVersion, snapTime, dateType, dateIndex, user, null, null);
        }

        [GridAction]
        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public ActionResult RccpPlanMasterList(GridCommand command, RccpPlanMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }

            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public ActionResult _AjaxRccpPlanMasterList(GridCommand command, RccpPlanMasterSearchModel searchModel)
        {
            string replaceFrom = "_AjaxRccpPlanMasterList";
            string replaceTo = "_AjaxRccpPlanMasterList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<RccpPlanMaster>(searchStatementModel, command));
        }
        private SearchStatementModel PrepareSearchStatement(GridCommand command, RccpPlanMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            if (searchModel.DateType != null)
            {
                whereStatement += " where r.DateType=? ";
                param.Add(searchModel.DateType);
            }

            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "r", ref whereStatement, param);

            HqlStatementHelper.AddEqStatement("SnapTime", searchModel.SnapTime, "r", ref whereStatement, param);


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by PlanVersion desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectRccpPlanMasterCountStatement;
            searchStatementModel.SelectStatement = selectRccpPlanMasterStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public ActionResult Edit(string planVersion)
        {
            DateTime time = Convert.ToDateTime(planVersion);
            RccpPlanMaster rccpPlanMaster = this.genericMgr.FindAll<RccpPlanMaster>("select r from RccpPlanMaster as r where r.PlanVersion=? ", new object[] { time })[0];
            return View(rccpPlanMaster);

        }

        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public ActionResult Save(string planVersion, bool isRelease)
        {
            DateTime time = Convert.ToDateTime(planVersion);
            RccpPlanMaster rccpPlanMaster = this.genericMgr.FindAll<RccpPlanMaster>
                ("select r from RccpPlanMaster as r where r.PlanVersion=? ", new object[] { time })[0];
            rccpPlanMaster.IsRelease = isRelease;
            rccpPlanMaster.LastModifyUserId = this.CurrentUser.Id;
            rccpPlanMaster.LastModifyUserName = this.CurrentUser.FullName;
            rccpPlanMaster.LastModifyDate = DateTime.Now;

            genericMgr.Update(rccpPlanMaster);
            SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedSuccessfully);
            return RedirectToAction("Edit", new { planVersion = planVersion });
        }

        [SconitAuthorize(Permissions = "Url_RccpPlan_New")]
        public ActionResult New()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "MRP_MrpPlan_New")]
        public ActionResult ImportRccpPlanDay(IEnumerable<HttpPostedFileBase> ImportDayRccpPlan,
            DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!startDate.HasValue)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_StartToTimeCanNotBeEmpty);
                }
                if (!startDate.HasValue)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_EndTimeCanNotBeEmpty);
                }
                if (!(ImportDayRccpPlan.Count() > 0))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ImportTemplateDetailBeEmpty);
                }

                foreach (var file in ImportDayRccpPlan)
                {
                    planMgr.ReadRccpPlanFromXls(file.InputStream, startDate.Value, endDate.Value, false, CodeMaster.TimeUnit.Day);
                }

                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ModelDailyPlanImportSuccessfully);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return Content("");
        }

        [SconitAuthorize(Permissions = "Url_RccpPlan_New")]
        public ActionResult ImportRccpPlanWeek(IEnumerable<HttpPostedFileBase> ImportWeekRccpPlan,
            string weekStart, string weekEnd, com.Sconit.CodeMaster.TimeUnit periodType)
        {
            try
            {
                if (string.IsNullOrEmpty(weekStart))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_StartToWeekCanNotBeEmpty);
                }
                if (string.IsNullOrEmpty(weekEnd))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_EndWeekCanNotBeEmpty);
                }
                if (!(ImportWeekRccpPlan.Count() > 0))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ImportTemplateDetailBeEmpty);
                }

                foreach (var file in ImportWeekRccpPlan)
                {
                    planMgr.ReadRccpPlanFromXls(file.InputStream, weekStart, weekEnd, false, periodType);
                }

                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ModelWeekPlanImportSuccessfully);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex);
            }
            return Content("");

        }

        [SconitAuthorize(Permissions = "Url_RccpPlan_New")]
        public ActionResult ImporRccpPlanMonth(IEnumerable<HttpPostedFileBase> ImportMonthRccpPlan,
            string startMonth, string endMonth, com.Sconit.CodeMaster.TimeUnit periodType)
        {
            try
            {
                if (string.IsNullOrEmpty(startMonth))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_StartToMothCanNotBeEmpty);
                }
                if (string.IsNullOrEmpty(endMonth))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_EndMothCanNotBeEmpty);
                }
                if (!(ImportMonthRccpPlan.Count() > 0))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ImportTemplateDetailBeEmpty);
                }

                foreach (var file in ImportMonthRccpPlan)
                {
                    planMgr.ReadRccpPlanFromXls(file.InputStream, startMonth, endMonth, false, periodType);
                }
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ModelMothPlanImportSuccessfully);

            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Content(string.Empty);
        }

        #endregion

        [SconitAuthorize(Permissions = "Url_RccpPlan_Edit")]
        public ActionResult _GetRccpPlanList(RccpPlanSearchModel searchModel)
        {
            ViewBag.PageSize = 20;
            ViewBag.ImportType = searchModel.ImportType;
            ViewBag.Item = searchModel.Item;
            ViewBag.StartMonth = searchModel.StartMonth;
            ViewBag.EndMonth = searchModel.EndMonth;
            ViewBag.StartWeek = searchModel.StartWeek;
            ViewBag.EndWeek = searchModel.EndWeek;
            ViewBag.StartDate = searchModel.StartDate;
            ViewBag.EndDate = searchModel.EndDate;

            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_RccpPlan_Edit")]
        public ActionResult _AjaxList(GridCommand command, RccpPlanSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<RccpPlan>(searchStatementModel, command));
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, RccpPlanSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.ImportType == "4")
            {
                if (searchModel.StartWeek != null & searchModel.EndWeek != null)
                {
                    HqlStatementHelper.AddEqStatement("DateType", com.Sconit.CodeMaster.TimeUnit.Day, "r", ref whereStatement, param);
                    HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.StartDate, searchModel.EndDate, "r", ref whereStatement, param);
                }
            }
            else if (searchModel.ImportType == "5")
            {
                if (searchModel.StartWeek != null & searchModel.EndWeek != null)
                {
                    HqlStatementHelper.AddEqStatement("DateType", com.Sconit.CodeMaster.TimeUnit.Week, "r", ref whereStatement, param);
                    HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.StartWeek, searchModel.EndWeek, "r", ref whereStatement, param);
                }
            }
            else if (searchModel.ImportType == "6")
            {
                if (!string.IsNullOrEmpty(searchModel.StartMonth) && !string.IsNullOrEmpty(searchModel.EndMonth))
                {
                    HqlStatementHelper.AddEqStatement("DateType", com.Sconit.CodeMaster.TimeUnit.Month, "r", ref whereStatement, param);
                    HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.StartMonth, searchModel.EndMonth, "r", ref whereStatement, param);
                }
            }
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "r", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_RccpPlan_Edit")]
        public ActionResult _Update(GridCommand command, RccpPlan rccpPlan, string importTypeTo, string startMonthTo
            , string endMonthTo, string startWeekTo, string endWeekTo, string itemTo, DateTime? startDate, DateTime? endDate)
        {
            RccpPlanSearchModel searchModel = new RccpPlanSearchModel();
            searchModel.ImportType = importTypeTo;

            searchModel.Item = itemTo;
            searchModel.StartMonth = startMonthTo;
            searchModel.EndMonth = endMonthTo;
            searchModel.StartWeek = startWeekTo;
            searchModel.EndWeek = endWeekTo;
            searchModel.StartDate = startDate;
            searchModel.EndDate = endDate;

            RccpPlan newRccpPlan = genericMgr.FindById<RccpPlan>(rccpPlan.Id);
            if (rccpPlan.Qty != newRccpPlan.Qty)
            {
                newRccpPlan.Qty = rccpPlan.Qty;
                genericMgr.Update(newRccpPlan);
                var rccpPlanLog = genericMgr.FindAll<RccpPlanLog>
                    (" from RccpPlanLog where PlanId =? and PlanVersion = ?", new object[] { rccpPlan.Id, rccpPlan.PlanVersion }).First();
                rccpPlanLog.Qty = rccpPlan.Qty;
                rccpPlanLog.UnitQty = 1;
                this.genericMgr.Update(rccpPlanLog);
            }

            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<RccpPlan>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlan_View")]
        public ActionResult RccpPlanView()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            IList<string> rccpVersions = this.queryMgr.FindAllWithNativeSql<string>
                ("select cast(MAX(PlanVersion) as varchar) from MRP_RccpPlan where DateType =5 ");
            ViewBag.CurrentPlanVersion = rccpVersions[0] ?? "0";
            return View();
        }
        public string _GetCurrentPlanVersion(int dateType)
        {
            IList<string> rccpVersions = this.queryMgr.FindAllWithNativeSql<string>
            ("select cast(MAX(PlanVersion) as varchar) from MRP_RccpPlan where DateType = " + dateType.ToString());
            return rccpVersions[0] ?? "0";
        }
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlan_View")]
        public string _GetRccpPlanView(com.Sconit.CodeMaster.TimeUnit dateType, string dateIndex, string item, int planVersion, string timeType)
        {
            IList<object> param = new List<object>();
            string dateIndexTo = string.Empty;
            if (dateType == CodeMaster.TimeUnit.Day)
            {
                dateIndexTo = DateTime.Parse(dateIndex).AddDays(14).ToString("yyyy-MM-dd");
            }
            else if (dateType == CodeMaster.TimeUnit.Week)
            {
                dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex).AddDays(16 * 7));
            }
            else if (dateType == CodeMaster.TimeUnit.Month)
            {
                dateIndexTo = DateTime.Parse(dateIndex + "-1").AddMonths(12).ToString("yyyy-MM");
            }

            string hql = "  from RccpPlan as r where r.DateType=? and r.DateIndexTo between ? and ?";
            string orderByDec = " order by DateIndex,Flow,Item ";
            if (timeType == "StartTime")
            {
                hql = "  from RccpPlan as r where r.DateType=? and r.DateIndex between ? and ?";
            }
            param.Add(dateType);
            param.Add(dateIndex);
            param.Add(dateIndexTo);
            if (!string.IsNullOrEmpty(item))
            {
                hql += " and r.Item=?";
                param.Add(item);
            }
            hql = hql + orderByDec;
            IList<RccpPlan> rccpPlanList = genericMgr.FindAll<RccpPlan>(hql, param.ToArray());
            return planMgr.GetStringRccpPlanView(rccpPlanList, planVersion, timeType);
        }
        #region Export RccpPlanView
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlan_View")]
        public ActionResult Export(com.Sconit.CodeMaster.TimeUnit dateType, string dateIndex, string DateIndexDate, string item, int planVersion, string timeType)
        {
            if (dateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                dateIndex = DateIndexDate;
            }
            var table = _GetRccpPlanView(dateType, dateIndex, item, planVersion, timeType);
            return new DownloadFileActionResult(table, "RccpPlanView.xls");
        }
        #endregion
        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public ActionResult _RccpPlanMasterError(string planVersion)
        {
            IList<RccpLog> rccpLogList = genericMgr.FindAll<RccpLog>(" from RccpLog as r where r.PlanVersion=?", Convert.ToDateTime(planVersion));
            return PartialView(rccpLogList);
        }

        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public ActionResult _RccpTransList(string planVersion)
        {
            IList<RccpTrans> rccpTransList = genericMgr.FindAll<RccpTrans>(" from RccpTrans as r where r.PlanVersion=?", Convert.ToDateTime(planVersion));
            return PartialView(rccpTransList);
        }

        [SconitAuthorize(Permissions = "MRP_RccpPlan_Run")]
        public string _GetRunLog()
        {
            try
            {
                FileStream fs = new FileStream(@"D:\logs\Sconit5_Shenya\RunRccp.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                string log = sr.ReadToEnd();
                fs.Close();
                string[] lines = Regex.Split(log, "\r\n", RegexOptions.IgnoreCase);
                string testLog = string.Empty;
                var startIndex = lines.Select((n, i) => new { n = n, i = i + 1 }).Where(n => n.n.Contains("---**---")).Last().i;
                foreach (var line in lines.Skip(startIndex - 1).Reverse())
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

        public ActionResult RccpTrans()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "MRP_RccpPlan_RccpTrans")]
        public ActionResult _AjaxLoadingTree(TreeViewItem node)
        {
            try
            {
                string parentId = !String.IsNullOrEmpty(node.Value) ? (node.Value) : null;
                IList<TreeViewItemModel> nodes = new List<TreeViewItemModel>();

                if (parentId != null)
                {
                    string[] s = parentId.Split(',');
                    if (s != null && s.Length == 5)
                    {
                        string hql = string.Empty;
                        bool isDown = bool.Parse(s[0]);
                        DateTime planVersion = DateTime.Parse(s[1]);
                        string fgCode = s[2];
                        string dateIndex = s[3];
                        CodeMaster.TimeUnit dateType = (CodeMaster.TimeUnit)(int.Parse(s[4]));
                        DateTime effdate = DateTime.Now;
                        if (dateType == CodeMaster.TimeUnit.Month)
                        {
                            effdate = DateTime.Parse(string.Format("{0}-01", dateIndex));
                        }
                        else
                        {
                            effdate = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                        }

                        IList<BomDetail> bomDetails = new List<BomDetail>();
                        if (isDown)
                        {
                            bomDetails = bomMgr.GetOnlyNextLevelBomDetail(fgCode, effdate);
                        }
                        else
                        {
                            bomDetails = this.genericMgr.FindAll<BomDetail>
                                (@"select bd from BomDetail as bd where bd.Item = ? and bd.StartDate <= ? 
                                   and (bd.EndDate is null or bd.EndDate >= ?)",
                                 new object[] { fgCode, effdate, effdate });
                        }

                        foreach (var bomDetail in bomDetails)
                        {
                            var bomMaster = this.bomMgr.GetCacheBomMaster(bomDetail.Bom);
                            Item fg = itemMgr.GetCacheItem(bomDetail.Bom);
                            var item = this.itemMgr.GetCacheItem(bomDetail.Item);
                            string currentItemCode = string.Empty;
                            decimal calculatedQty = 1;
                            //1.将bomMaster的单位转成基本单位 
                            var fgQty = itemMgr.ConvertItemUomQty(bomDetail.Bom, bomMaster.Uom, 1, fg.Uom);
                            //2.将BomDetail的单位转成基本单位
                            var itemQty = itemMgr.ConvertItemUomQty(item.Code, bomDetail.Uom, bomDetail.RateQty * (1 + bomDetail.ScrapPercentage) / bomMaster.Qty, item.Uom);
                            //3.单位成品基本用量
                            calculatedQty = itemQty / fgQty;

                            if (isDown)
                            {
                                currentItemCode = bomDetail.Item;
                            }
                            else
                            {
                                currentItemCode = bomDetail.Bom;
                            }

                            var rccpTransGroups = this.genericMgr.FindAll<RccpTransGroup>
                                ("from RccpTransGroup where DateIndex = ? and DateType =? and Item =? and PlanVersion =? ",
                                new object[] { dateIndex, dateType, currentItemCode, planVersion });
                            foreach (var rccpTransGroup in rccpTransGroups)
                            {
                                Item currentItem = itemMgr.GetCacheItem(currentItemCode);

                                TreeViewItemModel tvim = new TreeViewItemModel();
                                tvim.Text = string.Format(Resources.EXT.ControllerLan.Con_ItemRequirementQuantityBomUomScraptRate,
                                    currentItem.Code, currentItem.Description, rccpTransGroup.Qty.ToString("0.####"),
                                    calculatedQty.ToString("0.####"), bomDetail.ScrapPercentage.ToString("0.####"), currentItem.Uom, bomDetail.Uom);

                                tvim.Value = isDown + "," + planVersion + "," + currentItemCode + "," + dateIndex + "," + (int)dateType;
                                tvim.LoadOnDemand = true;
                                nodes.Add(tvim);
                            }
                        }
                    }
                }
                return new JsonResult { Data = nodes };
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return new JsonResult { };
            }
        }

        [SconitAuthorize(Permissions = "MRP_RccpPlan_RccpTrans")]
        public ActionResult TreeViewList(RccpPlanMasterSearchModel searchModel)
        {
            var rccpTransGroups = new List<RccpTransGroup>();
            TempData["RccpPlanMasterSearchModel"] = searchModel;
            ViewBag.SearchModel = searchModel;
            if (string.IsNullOrWhiteSpace(searchModel.Item) || !searchModel.PlanVersion.HasValue || string.IsNullOrWhiteSpace(searchModel.DateIndex))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PlanTypeVersionTimeItemCodeTimeIndexCanNotBeEmpty);
            }
            else
            {
                string hql = "from Item  where IsActive = 1 and Code = ? ";
                IList<Item> items = this.queryMgr.FindAll<Item>(hql, searchModel.Item, 0, 20);

                foreach (var item in items)
                {
                    var rccpTransGroup = this.genericMgr.FindAll<RccpTransGroup>
                    ("from RccpTransGroup where DateIndex = ? and DateType =? and Item =? and PlanVersion =? ",
                    new object[] { searchModel.DateIndex, searchModel.DateType, searchModel.Item, searchModel.PlanVersion }).FirstOrDefault();
                    if (rccpTransGroup != null)
                    {
                        rccpTransGroup.ItemDescription = item.Description;
                        rccpTransGroup.IsDown = searchModel.IsDown.HasValue ? searchModel.IsDown.Value : false;
                        rccpTransGroups.Add(rccpTransGroup);
                    }
                }
            }
            return View(rccpTransGroups);
        }
    }
}
