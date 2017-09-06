namespace com.Sconit.Web.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using com.Sconit.Service;
    using Models;
    using Telerik.Web.Mvc;
    using AutoMapper;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Utility;
    using System.Text;
    using System.Data.SqlClient;
    using com.Sconit.Web.Models.ReportModels;
    using System.Reflection;
    using Resources.Report;
    using com.Sconit.Entity;
    using com.Sconit.Entity.Report;

    public class CustReportController : WebAppBaseController
    {
        public ICustomizationMgr custmgr { get; set; }
        #region SearCh/Edit CustReport Main Menu
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the CustReportMaster security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the CustReportMaster 
        /// </summary>
        private static string selectCountStatement = "select count(*) from CustReportMaster as i";
        private static string selectDetCountStatement = "select count(*) from CustReportDetail as i";

        /// <summary>
        /// hql to get all of the CustReportMaster
        /// </summary>
        private static string selectStatement = "select i from CustReportMaster as i Order by i.Seq";
        private static string selectDetStatement = "select i from CustReportDetail as i";

        /// <summary>
        /// hql to get count of the CustReport by CustReportMaster's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from CustReportMaster as i where i.Code = ?";
        private static string duiplicateDetVerifyStatement = @"select count(*) from CustReportDetail as i where i.Code = ? and i.ParamType=?";
        private static string existsCodeVerifyStatement = @"select count(*) from CustReportMaster as i where i.Code = ? ";

        #region public actions
        /// Index action for CustReport controller
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult Index()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult DetailIndex()
        {
            return View();
        }
        /// List action
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult List(GridCommand command, CustReportSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }
        /// List action
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult DetailList(GridCommand command, CustReportSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }
        /// AjaxList action
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _AjaxList(GridCommand command, CustReportSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<CustReportMaster>(searchStatementModel, command));
        }
        /// AjaxList action
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _AjaxDetList(GridCommand command, CustReportSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareDetSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<CustReportDetail>(searchStatementModel, command));
        }
        /// New action
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult New()
        {
            return View();
        }
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                return View("Edit", string.Empty, id);
            }
        }
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _Detail(GridCommand command, CustReportDetail custReportDetail,string code)
        {
            ViewBag.Code = code;
            ViewBag.Name = TempData["Name"];
            if (ViewBag.Name == null)
            {
                ViewBag.Name = genericMgr.FindById<CustReportMaster>(code).Name;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _DetailNew(string id)
        {
            CustReportDetail custReportDetail = new CustReportDetail();
            custReportDetail.Code = id;
            return PartialView(custReportDetail);
        }
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _DetailSearch(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            ViewBag.Code = id;
            ViewBag.Name = genericMgr.FindById<CustReportMaster>(id).Name;
            return PartialView();
        }
        /// New action
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult New(CustReportMaster CustReport)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { CustReport.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, CustReport.Code);
                }
                else
                {
                    CustReport.IsActive = true;
                    this.genericMgr.CreateWithTrim(CustReport);
                    this.custmgr.AddNewCustReport(CustReport.Code);
                    systemMgr.ResetCache();
                    SaveSuccessMessage(Resources.Report.CustReport.CustReport_Added);
                    return RedirectToAction("Edit/" + CustReport.Code);
                }
            }

            return View(CustReport);
        }
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _DetailNew(CustReportDetail CustReport)
        {
            if (ModelState.IsValid)
            {

                if (this.genericMgr.FindAll<long>(existsCodeVerifyStatement, new object[] { CustReport.Code})[0] == 0)
                {
                    SaveErrorMessage("Report menu code not exist.", CustReport.Code);
                }
                else if (this.genericMgr.FindAll<long>(duiplicateDetVerifyStatement, new object[] { CustReport.Code, CustReport.ParamType })[0] > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ParamTypeDuplicate, CustReport.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(CustReport);
                    SaveSuccessMessage(Resources.Report.CustReport.CustReport_Added);
                    TempData["Name"] = genericMgr.FindById<CustReportMaster>(CustReport.Code).Name;
                    return RedirectToAction("_Detail/" + CustReport.Code);
                }
            }

            return PartialView(CustReport);
        }
        /// Edit action
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                CustReportMaster CustReport = this.genericMgr.FindById<CustReportMaster>(id);
                return PartialView(CustReport);
            }
        }
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _DetailEdit(int? id)
        {
            if (!id.HasValue || id.Value == 0)
            {
                return HttpNotFound();
            }
            else
            {
                CustReportDetail CustReportDet = genericMgr.FindById<CustReportDetail>(id);
                return PartialView(CustReportDet);
            }
        }
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _DetailEdit(CustReportDetail CustReportDet, int? id)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(CustReportDet);
                SaveSuccessMessage(Resources.Report.CustReport.CustReport_Updated);
            }
            TempData["TabIndex"] = 1;
            return PartialView(CustReportDet);
        }
        /// Edit action
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult _Edit(CustReportMaster CustReport)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(CustReport);
                SaveSuccessMessage(Resources.Report.CustReport.CustReport_Updated);
            }
            this.custmgr.UpdateCustReport(CustReport);
            systemMgr.ResetCache();
            return PartialView(CustReport);
        }

        /// Delete action
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<CustReportMaster>(id);
                this.custmgr.DeleteCustReport(id);
                systemMgr.ResetCache();
                SaveSuccessMessage(Resources.Report.CustReport.CustReport_Deleted);
                return RedirectToAction("List");
            }
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult btnDel(string id,string code)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<CustReportDetail>(int.Parse(id));
                SaveSuccessMessage(Resources.Report.CustReport.CustReport_Deleted);
                TempData["Name"] = genericMgr.FindById<CustReportMaster>(code).Name;
                return RedirectToAction("_Detail/"+code);
            }
        }
        #endregion

        /// Search Statement
        private SearchStatementModel PrepareSearchStatement(GridCommand command, CustReportSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = " order by i.Code ";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        /// Det Search Statement
        private SearchStatementModel PrepareDetSearchStatement(GridCommand command, CustReportSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = " order by i.Code ";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectDetCountStatement;
            searchStatementModel.SelectStatement = selectDetStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        #endregion
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult ListIndex(string code)
        {
            var reportDetialList = genericMgr.FindAll<CustReportDetail>("from CustReportDetail As c where c.Code=? ", code);
            ViewBag.Code = code;
            CustReportMaster CustReportMst = genericMgr.FindById<CustReportMaster>(code);
            ViewBag.Name = CustReportMst.Name;
            ViewBag._AddressComboBox = reportDetialList.Where(p => p.ParamType == "_AddressComboBox").Count() > 0 ? true : false;
            if (ViewBag._AddressComboBox) ViewBag._AddressComboBoxText = reportDetialList.Where(p => p.ParamType == "_AddressComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._BomComboBox = reportDetialList.Where(p => p.ParamType == "_BomComboBox").Count() > 0 ? true : false;
            if (ViewBag._BomComboBox) ViewBag._BomComboBoxText = reportDetialList.Where(p => p.ParamType == "_BomComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._CodeMasterComboBox = reportDetialList.Where(p => p.ParamType == "_CodeMasterComboBox").Count() > 0 ? true : false;
            if (ViewBag._CodeMasterComboBox) ViewBag._CodeMasterComboBoxText = reportDetialList.Where(p => p.ParamType == "_CodeMasterComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._CodeMasterDropDownList = reportDetialList.Where(p => p.ParamType == "_CodeMasterDropDownList").Count() > 0 ? true : false;
            if (ViewBag._CodeMasterDropDownList) ViewBag._CodeMasterDropDownListText = reportDetialList.Where(p => p.ParamType == "_CodeMasterDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._CommonDropDownList = reportDetialList.Where(p => p.ParamType == "_CommonDropDownList").Count() > 0 ? true : false;
            if (ViewBag._CommonDropDownList) ViewBag._CommonDropDownListText = reportDetialList.Where(p => p.ParamType == "_CommonDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ContainerDropDownList = reportDetialList.Where(p => p.ParamType == "_ContainerDropDownList").Count() > 0 ? true : false;
            if (ViewBag._ContainerDropDownList) ViewBag._ContainerDropDownListText = reportDetialList.Where(p => p.ParamType == "_ContainerDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._CostCenterComboBox = reportDetialList.Where(p => p.ParamType == "_CostCenterComboBox").Count() > 0 ? true : false;
            if (ViewBag._CostCenterComboBox) ViewBag._CostCenterComboBoxText = reportDetialList.Where(p => p.ParamType == "_CostCenterComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._CurrencyDropDownList = reportDetialList.Where(p => p.ParamType == "_CurrencyDropDownList").Count() > 0 ? true : false;
            if (ViewBag._CurrencyDropDownList) ViewBag._CurrencyDropDownListText = reportDetialList.Where(p => p.ParamType == "_CurrencyDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._CustomerComboBox = reportDetialList.Where(p => p.ParamType == "_CustomerComboBox").Count() > 0 ? true : false;
            if (ViewBag._CustomerComboBox) ViewBag._CustomerComboBoxText = reportDetialList.Where(p => p.ParamType == "_CustomerComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._DateIndexComboBox = reportDetialList.Where(p => p.ParamType == "_DateIndexComboBox").Count() > 0 ? true : false;
            if (ViewBag._DateIndexComboBox) ViewBag._DateIndexComboBoxText = reportDetialList.Where(p => p.ParamType == "_DateIndexComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._DefectCodeComboBox = reportDetialList.Where(p => p.ParamType == "_DefectCodeComboBox").Count() > 0 ? true : false;
            if (ViewBag._DefectCodeComboBox) ViewBag._DefectCodeComboBoxText = reportDetialList.Where(p => p.ParamType == "_DefectCodeComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._DefectCodeDropDownList = reportDetialList.Where(p => p.ParamType == "_DefectCodeDropDownList").Count() > 0 ? true : false;
            if (ViewBag._DefectCodeDropDownList) ViewBag._DefectCodeDropDownListText = reportDetialList.Where(p => p.ParamType == "_DefectCodeDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._FailCodeComboBox = reportDetialList.Where(p => p.ParamType == "_FailCodeComboBox").Count() > 0 ? true : false;
            if (ViewBag._FailCodeComboBox) ViewBag._FailCodeComboBoxText = reportDetialList.Where(p => p.ParamType == "_FailCodeComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._FiShiftComboBox = reportDetialList.Where(p => p.ParamType == "_FiShiftComboBox").Count() > 0 ? true : false;
            if (ViewBag._FiShiftComboBox) ViewBag._FiShiftComboBoxText = reportDetialList.Where(p => p.ParamType == "_FiShiftComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._FlowComboBox = reportDetialList.Where(p => p.ParamType == "_FlowComboBox").Count() > 0 ? true : false;
            if (ViewBag._FlowComboBox) ViewBag._FlowComboBoxText = reportDetialList.Where(p => p.ParamType == "_FlowComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._FlowItemComboBox = reportDetialList.Where(p => p.ParamType == "_FlowItemComboBox").Count() > 0 ? true : false;
            if (ViewBag._FlowItemComboBox) ViewBag._FlowItemComboBoxText = reportDetialList.Where(p => p.ParamType == "_FlowItemComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._HuToComboBox = reportDetialList.Where(p => p.ParamType == "_HuToComboBox").Count() > 0 ? true : false;
            if (ViewBag._HuToComboBox) ViewBag._HuToComboBoxText = reportDetialList.Where(p => p.ParamType == "_HuToComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._InspectComboBox = reportDetialList.Where(p => p.ParamType == "_InspectComboBox").Count() > 0 ? true : false;
            if (ViewBag._InspectComboBox) ViewBag._InspectComboBoxText = reportDetialList.Where(p => p.ParamType == "_InspectComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._IslandComboBox = reportDetialList.Where(p => p.ParamType == "_IslandComboBox").Count() > 0 ? true : false;
            if (ViewBag._IslandComboBox) ViewBag._IslandComboBoxText = reportDetialList.Where(p => p.ParamType == "_IslandComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ItemCategoryDropDownList = reportDetialList.Where(p => p.ParamType == "_ItemCategoryDropDownList").Count() > 0 ? true : false;
            if (ViewBag._ItemCategoryDropDownList) ViewBag._ItemCategoryDropDownListText = reportDetialList.Where(p => p.ParamType == "_ItemCategoryDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ItemComboBox = reportDetialList.Where(p => p.ParamType == "_ItemComboBox").Count() > 0 ? true : false;
            if (ViewBag._ItemComboBox) ViewBag._ItemComboBoxText = reportDetialList.Where(p => p.ParamType == "_ItemComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ItemPackageComboBox = reportDetialList.Where(p => p.ParamType == "_ItemPackageComboBox").Count() > 0 ? true : false;
            if (ViewBag._ItemPackageComboBox) ViewBag._ItemPackageComboBoxText = reportDetialList.Where(p => p.ParamType == "_ItemPackageComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ItemPackageDropDownList = reportDetialList.Where(p => p.ParamType == "_ItemPackageDropDownList").Count() > 0 ? true : false;
            if (ViewBag._ItemPackageDropDownList) ViewBag._ItemPackageDropDownListText = reportDetialList.Where(p => p.ParamType == "_ItemPackageDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._LocationBinComboBox = reportDetialList.Where(p => p.ParamType == "_LocationBinComboBox").Count() > 0 ? true : false;
            if (ViewBag._LocationBinComboBox) ViewBag._LocationBinComboBoxText = reportDetialList.Where(p => p.ParamType == "_LocationBinComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._LocationBinDropDownList = reportDetialList.Where(p => p.ParamType == "_LocationBinDropDownList").Count() > 0 ? true : false;
            if (ViewBag._LocationBinDropDownList) ViewBag._LocationBinDropDownListText = reportDetialList.Where(p => p.ParamType == "_LocationBinDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._LocationComboBox = reportDetialList.Where(p => p.ParamType == "_LocationComboBox").Count() > 0 ? true : false;
            if (ViewBag._LocationComboBox) ViewBag._LocationComboBoxText = reportDetialList.Where(p => p.ParamType == "_LocationComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._MachineComboBox = reportDetialList.Where(p => p.ParamType == "_MachineComboBox").Count() > 0 ? true : false;
            if (ViewBag._MachineComboBox) ViewBag._MachineComboBoxText = reportDetialList.Where(p => p.ParamType == "_MachineComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ManufacturePartyComboBox = reportDetialList.Where(p => p.ParamType == "_ManufacturePartyComboBox").Count() > 0 ? true : false;
            if (ViewBag._ManufacturePartyComboBox) ViewBag._ManufacturePartyComboBoxText = reportDetialList.Where(p => p.ParamType == "_ManufacturePartyComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ManufacturePartyDropDownList = reportDetialList.Where(p => p.ParamType == "_ManufacturePartyDropDownList").Count() > 0 ? true : false;
            if (ViewBag._ManufacturePartyDropDownList) ViewBag._ManufacturePartyDropDownListText = reportDetialList.Where(p => p.ParamType == "_ManufacturePartyDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._MoveTypeDropDownList = reportDetialList.Where(p => p.ParamType == "_MoveTypeDropDownList").Count() > 0 ? true : false;
            if (ViewBag._MoveTypeDropDownList) ViewBag._MoveTypeDropDownListText = reportDetialList.Where(p => p.ParamType == "_MoveTypeDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._MrpPlanMasterComboBox = reportDetialList.Where(p => p.ParamType == "_MrpPlanMasterComboBox").Count() > 0 ? true : false;
            if (ViewBag._MrpPlanMasterComboBox) ViewBag._MrpPlanMasterComboBoxText = reportDetialList.Where(p => p.ParamType == "_MrpPlanMasterComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._MrpSnapMasterComboBox = reportDetialList.Where(p => p.ParamType == "_MrpSnapMasterComboBox").Count() > 0 ? true : false;
            if (ViewBag._MrpSnapMasterComboBox) ViewBag._MrpSnapMasterComboBoxText = reportDetialList.Where(p => p.ParamType == "_MrpSnapMasterComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._OrderComboBox = reportDetialList.Where(p => p.ParamType == "_OrderComboBox").Count() > 0 ? true : false;
            if (ViewBag._OrderComboBox) ViewBag._OrderComboBoxText = reportDetialList.Where(p => p.ParamType == "_OrderComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._OrderMasterPartyFromComboBox = reportDetialList.Where(p => p.ParamType == "_OrderMasterPartyFromComboBox").Count() > 0 ? true : false;
            if (ViewBag._OrderMasterPartyFromComboBox) ViewBag._OrderMasterPartyFromComboBoxText = reportDetialList.Where(p => p.ParamType == "_OrderMasterPartyFromComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._OrderMasterPartyToComboBox = reportDetialList.Where(p => p.ParamType == "_OrderMasterPartyToComboBox").Count() > 0 ? true : false;
            if (ViewBag._OrderMasterPartyToComboBox) ViewBag._OrderMasterPartyToComboBoxText = reportDetialList.Where(p => p.ParamType == "_OrderMasterPartyToComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._PartyDropDownList = reportDetialList.Where(p => p.ParamType == "_PartyDropDownList").Count() > 0 ? true : false;
            if (ViewBag._PartyDropDownList) ViewBag._PartyDropDownListText = reportDetialList.Where(p => p.ParamType == "_PartyDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._PickStrategyComboBox = reportDetialList.Where(p => p.ParamType == "_PickStrategyComboBox").Count() > 0 ? true : false;
            if (ViewBag._PickStrategyComboBox) ViewBag._PickStrategyComboBoxText = reportDetialList.Where(p => p.ParamType == "_PickStrategyComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._PlanNoComboBox = reportDetialList.Where(p => p.ParamType == "_PlanNoComboBox").Count() > 0 ? true : false;
            if (ViewBag._PlanNoComboBox) ViewBag._PlanNoComboBoxText = reportDetialList.Where(p => p.ParamType == "_PlanNoComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._PriceListComboBox = reportDetialList.Where(p => p.ParamType == "_PriceListComboBox").Count() > 0 ? true : false;
            if (ViewBag._PriceListComboBox) ViewBag._PriceListComboBoxText = reportDetialList.Where(p => p.ParamType == "_PriceListComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ProductCodeDropDownList = reportDetialList.Where(p => p.ParamType == "_ProductCodeDropDownList").Count() > 0 ? true : false;
            if (ViewBag._ProductCodeDropDownList) ViewBag._ProductCodeDropDownListText = reportDetialList.Where(p => p.ParamType == "_ProductCodeDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ProductLineFacilityComboBox = reportDetialList.Where(p => p.ParamType == "_ProductLineFacilityComboBox").Count() > 0 ? true : false;
            if (ViewBag._ProductLineFacilityComboBox) ViewBag._ProductLineFacilityComboBoxText = reportDetialList.Where(p => p.ParamType == "_ProductLineFacilityComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ProductTypeDropDownList = reportDetialList.Where(p => p.ParamType == "_ProductTypeDropDownList").Count() > 0 ? true : false;
            if (ViewBag._ProductTypeDropDownList) ViewBag._ProductTypeDropDownListText = reportDetialList.Where(p => p.ParamType == "_ProductTypeDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._PurchasePlanMasterComboBox = reportDetialList.Where(p => p.ParamType == "_PurchasePlanMasterComboBox").Count() > 0 ? true : false;
            if (ViewBag._PurchasePlanMasterComboBox) ViewBag._PurchasePlanMasterComboBoxText = reportDetialList.Where(p => p.ParamType == "_PurchasePlanMasterComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._RccpPlanMasterComboBox = reportDetialList.Where(p => p.ParamType == "_RccpPlanMasterComboBox").Count() > 0 ? true : false;
            if (ViewBag._RccpPlanMasterComboBox) ViewBag._RccpPlanMasterComboBoxText = reportDetialList.Where(p => p.ParamType == "_RccpPlanMasterComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._RegionCombobox = reportDetialList.Where(p => p.ParamType == "_RegionCombobox").Count() > 0 ? true : false;
            if (ViewBag._RegionCombobox) ViewBag._RegionComboboxText = reportDetialList.Where(p => p.ParamType == "_RegionCombobox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._RegionWorkShopComboBox = reportDetialList.Where(p => p.ParamType == "_RegionWorkShopComboBox").Count() > 0 ? true : false;
            if (ViewBag._RegionWorkShopComboBox) ViewBag._RegionWorkShopComboBoxText = reportDetialList.Where(p => p.ParamType == "_RegionWorkShopComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._RejectComboBox = reportDetialList.Where(p => p.ParamType == "_RejectComboBox").Count() > 0 ? true : false;
            if (ViewBag._RejectComboBox) ViewBag._RejectComboBoxText = reportDetialList.Where(p => p.ParamType == "_RejectComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._RoutingComboBox = reportDetialList.Where(p => p.ParamType == "_RoutingComboBox").Count() > 0 ? true : false;
            if (ViewBag._RoutingComboBox) ViewBag._RoutingComboBoxText = reportDetialList.Where(p => p.ParamType == "_RoutingComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._RoutingDropDownList = reportDetialList.Where(p => p.ParamType == "_RoutingDropDownList").Count() > 0 ? true : false;
            if (ViewBag._RoutingDropDownList) ViewBag._RoutingDropDownListText = reportDetialList.Where(p => p.ParamType == "_RoutingDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._SAPLocationComboBox = reportDetialList.Where(p => p.ParamType == "_SAPLocationComboBox").Count() > 0 ? true : false;
            if (ViewBag._SAPLocationComboBox) ViewBag._SAPLocationComboBoxText = reportDetialList.Where(p => p.ParamType == "_SAPLocationComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._SectionComboBox = reportDetialList.Where(p => p.ParamType == "_SectionComboBox").Count() > 0 ? true : false;
            if (ViewBag._SectionComboBox) ViewBag._SectionComboBoxText = reportDetialList.Where(p => p.ParamType == "_SectionComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._ShiftComboBox = reportDetialList.Where(p => p.ParamType == "_ShiftComboBox").Count() > 0 ? true : false;
            if (ViewBag._ShiftComboBox) ViewBag._ShiftComboBoxText = reportDetialList.Where(p => p.ParamType == "_ShiftComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._SupplierComboBox = reportDetialList.Where(p => p.ParamType == "_SupplierComboBox").Count() > 0 ? true : false;
            if (ViewBag._SupplierComboBox) ViewBag._SupplierComboBoxText = reportDetialList.Where(p => p.ParamType == "_SupplierComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._TransferPlanMasterComboBox = reportDetialList.Where(p => p.ParamType == "_TransferPlanMasterComboBox").Count() > 0 ? true : false;
            if (ViewBag._TransferPlanMasterComboBox) ViewBag._TransferPlanMasterComboBoxText = reportDetialList.Where(p => p.ParamType == "_TransferPlanMasterComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._UomDropDownList = reportDetialList.Where(p => p.ParamType == "_UomDropDownList").Count() > 0 ? true : false;
            if (ViewBag._UomDropDownList) ViewBag._UomDropDownListText = reportDetialList.Where(p => p.ParamType == "_UomDropDownList").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._UserComboBox = reportDetialList.Where(p => p.ParamType == "_UserComboBox").Count() > 0 ? true : false;
            if (ViewBag._UserComboBox) ViewBag._UserComboBoxText = reportDetialList.Where(p => p.ParamType == "_UserComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag._WorkingCalendarFlowComboBox = reportDetialList.Where(p => p.ParamType == "_WorkingCalendarFlowComboBox").Count() > 0 ? true : false;
            if (ViewBag._WorkingCalendarFlowComboBox) ViewBag._WorkingCalendarFlowComboBoxText = reportDetialList.Where(p => p.ParamType == "_WorkingCalendarFlowComboBox").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.Item = reportDetialList.Where(p => p.ParamType == "Item").Count() > 0 ? true : false;
            if (ViewBag.Item) ViewBag.ItemText = reportDetialList.Where(p => p.ParamType == "Item").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.OrderNo = reportDetialList.Where(p => p.ParamType == "OrderNo").Count() > 0 ? true : false;
            if (ViewBag.OrderNo) ViewBag.OrderNoText = reportDetialList.Where(p => p.ParamType == "OrderNo").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.HuId = reportDetialList.Where(p => p.ParamType == "HuId").Count() > 0 ? true : false;
            if (ViewBag.HuId) ViewBag.HuIdText = reportDetialList.Where(p => p.ParamType == "HuId").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.Text1 = reportDetialList.Where(p => p.ParamType == "Text1").Count() > 0 ? true : false;
            if (ViewBag.Text1) ViewBag.Text1Text = reportDetialList.Where(p => p.ParamType == "Text1").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.Text2 = reportDetialList.Where(p => p.ParamType == "Text2").Count() > 0 ? true : false;
            if (ViewBag.Text2) ViewBag.Text2Text = reportDetialList.Where(p => p.ParamType == "Text2").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.Text3 = reportDetialList.Where(p => p.ParamType == "Text3").Count() > 0 ? true : false;
            if (ViewBag.Text3) ViewBag.Text3Text = reportDetialList.Where(p => p.ParamType == "Text3").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.Text4 = reportDetialList.Where(p => p.ParamType == "Text4").Count() > 0 ? true : false;
            if (ViewBag.Text4) ViewBag.Text4Text = reportDetialList.Where(p => p.ParamType == "Text4").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.Text5 = reportDetialList.Where(p => p.ParamType == "Text5").Count() > 0 ? true : false;
            if (ViewBag.Text5) ViewBag.Text5Text = reportDetialList.Where(p => p.ParamType == "Text5").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.EndDate = reportDetialList.Where(p => p.ParamType == "EndDate").Count() > 0 ? true : false;
            if (ViewBag.EndDate) ViewBag.EndDateText = reportDetialList.Where(p => p.ParamType == "EndDate").Select(p => p.ParamText).FirstOrDefault();
            ViewBag.StartDate = reportDetialList.Where(p => p.ParamType == "StartDate").Count() > 0 ? true : false;
            if (ViewBag.StartDate) ViewBag.StartDateText = reportDetialList.Where(p => p.ParamType == "StartDate").Select(p => p.ParamText).FirstOrDefault(); return View();
        }

        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public string _GetHtmlStr(CustReportModel custReportModel)
        {
            try
            {
                var reportDetialList = genericMgr.FindAll<CustReportDetail>("from CustReportDetail As c where c.Code=? ", custReportModel.Code);
                CustReportMaster CustReportMst = genericMgr.FindById<CustReportMaster>(custReportModel.Code);

                PropertyInfo[] myPropertyInfo = typeof(CustReportModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var dic = new Dictionary<string, string>();
                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    System.Reflection.PropertyInfo pi = myPropertyInfo[i];
                    dic.Add(pi.Name, pi.GetValue(custReportModel, null) == null ? "" : pi.GetValue(custReportModel, null).ToString().Trim());
                }
                string sql = CustReportMst.Sql;
                SqlParameter[] sqlParams = new SqlParameter[reportDetialList.Count];
                for (int i = 0; i < reportDetialList.Count; i++)
                {
                    sqlParams[i] = new SqlParameter("@" + reportDetialList[i].ParamKey, dic.ValueOrDefault(reportDetialList[i].ParamType));
                }

                return GetTableHtmlBySql(sql, sqlParams);
            }
            catch (Exception e)
            {
                return "Error:"+e.Message;
            }
        }
        #region Export
        [SconitAuthorize(Permissions = "Url_CustReport_ReportMenu")]
        public ActionResult Export(CustReportModel custReportModel)
        {
            var table = _GetHtmlStr(custReportModel);
            return new DownloadFileActionResult(table, "ExportedRecords.xls");
        }
        #endregion
    }
}
