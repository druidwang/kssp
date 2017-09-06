/// <summary>
/// Summary description for FacilityMasterController
/// </summary>
namespace com.Sconit.Web.Controllers.FMS
{
    #region reference
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Web.Models.SearchModels.FMS;
    using com.Sconit.Entity.FMS;
    using com.Sconit.Entity.ACC;
    #endregion

    /// <summary>
    /// This controller response to control the FacilityMaster.
    /// </summary>
    public class FacilityMasterController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the facilityMaster security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        public IFacilityMgr facilityMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the facilityMaster
        /// </summary>
        private static string selectCountStatement = "select count(*) from FacilityMaster as f";

        /// <summary>
        /// hql to get all of the facilityMaster
        /// </summary>
        private static string selectStatement = "select f from FacilityMaster as f";

        /// <summary>
        /// hql
        /// </summary>
        private static string selectFacilityMaintainPlanCountStatement = "select count(*) from FacilityMaintainPlan as f";

        /// <summary>
        /// hql
        /// </summary>
        private static string selectFacilityMaintainPlanStatement = "select f from FacilityMaintainPlan as f";

      
        private static string duiplicateVerifyStatement = @"select count(*) from FacilityMaster as f where f.Code = ?";

      
        private static string facilityMaintainPlanDuiplicateVerifyStatement = @"select count(*) from FacilityMaintainPlan as f where f.FCID = ? and f.MaintainPlan.Code = ?";


        private static string selectFacilityTransStatement = "select f from FacilityTrans as f";

        private static string selectFacilityTransCountStatement = "select count(*) from FacilityTrans as f";


        #region public actions
        /// <summary>
        /// Index action for FacilityMaster controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_FacilityMaster_View")]
        public ActionResult Index()
        {
            // ObixHelper.Request_WebRequest("glue_settingval1/in10/");
           // facilityMgr.GetFacilityControlPoint("glue_settingval1");
            //  ObixHelper.Response_WebRequest("glue_settingval1/in9/display", "66.6 cm³ {ok}");
           // facilityMgr.CreateFacilityOrder("FC000000008");
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FacilityMaster Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityMaster_View")]
        public ActionResult List(GridCommand command, FacilityMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FacilityMaster Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityMaster_View")]
        public ActionResult _AjaxList(GridCommand command, FacilityMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<FacilityMaster>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_FacilityMaster_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="facilityMaster">FacilityMaster Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_FacilityMaster_Edit")]
        public ActionResult New(FacilityMaster facilityMaster)
        {
            if (ModelState.IsValid)
            {
                facilityMaster.Category = "DZ_SB4";
                facilityMgr.CreateFacilityMaster(facilityMaster);
                SaveSuccessMessage(Resources.FMS.FacilityMaster.FacilityMaster_Added);
                return RedirectToAction("Edit/" + facilityMaster.FCID);
            }

            return View(facilityMaster);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_FacilityMaster_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                return View("Edit", string.Empty, id);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">facilityMaster id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_FacilityMaster_View")]
        public ActionResult _Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                FacilityMaster facilityMaster = this.genericMgr.FindById<FacilityMaster>(id);
                return PartialView(facilityMaster);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="facilityMaster">FacilityMaster Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_FacilityMaster_Edit")]
        public ActionResult _Edit(FacilityMaster facilityMaster)
        {
            if (ModelState.IsValid)
            {
                facilityMaster.Category = "DZ_SB4";
                facilityMaster.CurrChargePersonName = genericMgr.FindById<User>(facilityMaster.CurrChargePersonId).FullName;
                this.genericMgr.UpdateWithTrim(facilityMaster);
                SaveSuccessMessage(Resources.FMS.FacilityMaster.FacilityMaster_Updated);
            }

          //  return View(facilityMaster);


            TempData["TabIndex"] = 0;
            return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_Edit" }, 
                                                       { "controller", "FacilityMaster" } ,
                                                       { "Id", facilityMaster.FCID }
                                                   });
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">facilityMaster id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_FacilityMaster_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<FacilityMaster>(id);
                SaveSuccessMessage(Resources.FMS.FacilityMaster.FacilityMaster_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion


        #region FacilityMaintainPlan
      
        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="itemRefsearchModel">facilityMaintainPlan Search model</param>
        /// <param name="id">facilityMaintainPlan id</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_FacilityMaintainPlan_View")]
        public ActionResult _FacilityMaintainPlan( string id)
        {
            ViewBag.FCID = id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_FacilityMaintainPlan_View")]
        public ActionResult _FacilityMaintainPlanList(GridCommand command, MaintainPlanSearchModel searchModel, string FCID)
        {
            ViewBag.FCID = FCID;

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="itemRefsearchModel">FacilityMaintainPlan Search Model</param>
        /// <param name="itemCode">FacilityMaintainPlan itemCode</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityMaintainPlan_View")]
        public ActionResult _AjaxFacilityMaintainPlanList(GridCommand command, MaintainPlanSearchModel maintainPlanSearchModel, string fcId)
        {
            SearchStatementModel searchStatementModel = this.FacilityMaintainPlanPrepareSearchStatement(command, maintainPlanSearchModel, fcId);
            return PartialView(GetAjaxPageData<FacilityMaintainPlan>(searchStatementModel, command));
        }

     

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="id">FacilityMaintainPlan id</param>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_FacilityMaster_Edit")]
        public ActionResult _FacilityMaintainPlanNew(string FCID)
        {
            FacilityMaintainPlan facilityMaintainPlan = new FacilityMaintainPlan();
            facilityMaintainPlan.FCID = FCID;
            return PartialView(facilityMaintainPlan);
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="facilityMaintainPlan">FacilityMaintainPlan model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_FacilityMaintainPlan_Edit")]
        public ActionResult _FacilityMaintainPlanNew(FacilityMaintainPlan facilityMaintainPlan,MaintainPlan maintainPlan)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(facilityMaintainPlanDuiplicateVerifyStatement, new object[] { facilityMaintainPlan.FCID, maintainPlan.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, facilityMaintainPlan.MaintainPlan.Code);
                }
               
                else
                {
                    facilityMaintainPlan.MaintainPlan = maintainPlan;
                    if (facilityMaintainPlan.StartQty != 0)
                    {
                        facilityMaintainPlan.NextMaintainQty = facilityMaintainPlan.StartQty;
                        facilityMaintainPlan.NextWarnQty = facilityMaintainPlan.StartQty;
                        facilityMaintainPlan.NextWarnDate = null;
                        facilityMaintainPlan.NextMaintainDate = null;
                        facilityMaintainPlan.StartDate = null;
                    }
                    if (facilityMaintainPlan.StartDate.HasValue)
                    {
                        facilityMaintainPlan.NextMaintainDate = facilityMaintainPlan.StartDate;
                        facilityMaintainPlan.NextWarnDate = facilityMaintainPlan.StartDate;
                    }
               
              
                    this.genericMgr.CreateWithTrim(facilityMaintainPlan);

                    SaveSuccessMessage(Resources.FMS.FacilityMaintainPlan.FacilityMaintainPlan_Added);
                    return RedirectToAction("_FacilityMaintainPlanEdit/" + facilityMaintainPlan.Id);
                }
            }

            return PartialView(facilityMaintainPlan);
        }

        /// <summary>
        /// FacilityMaintainPlanEdit action
        /// </summary>
        /// <param name="id">FacilityMaintainPlan id for Edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_FacilityMaintainPlan_Edit")]
        public ActionResult _FacilityMaintainPlanEdit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
               
                FacilityMaintainPlan facilityMaintainPlan = this.genericMgr.FindById<FacilityMaintainPlan>(id);
                return PartialView(facilityMaintainPlan);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_FacilityMaintainPlan_Edit")]
        public ActionResult _FacilityMaintainPlanEdit(FacilityMaintainPlan facilityMaintainPlan, MaintainPlan maintainPlan)
        {
            if (ModelState.IsValid)
            {
                facilityMaintainPlan.MaintainPlan = maintainPlan;
                if (facilityMaintainPlan.StartQty != 0)
                {
                    facilityMaintainPlan.NextMaintainQty = facilityMaintainPlan.StartQty;
                    facilityMaintainPlan.NextWarnQty = facilityMaintainPlan.StartQty;
                    facilityMaintainPlan.NextWarnDate = null;
                    facilityMaintainPlan.NextMaintainDate = null;
                    facilityMaintainPlan.StartDate = null;
                }
                if (facilityMaintainPlan.StartDate.HasValue)
                {
                    facilityMaintainPlan.NextMaintainDate = facilityMaintainPlan.StartDate;
                    facilityMaintainPlan.NextWarnDate = facilityMaintainPlan.StartDate;
                }
               
              
              
             
                this.genericMgr.UpdateWithTrim(facilityMaintainPlan);

                SaveSuccessMessage(Resources.FMS.FacilityMaintainPlan.FacilityMaintainPlan_Added);
              
            }
            TempData["TabIndex"] = 1;
            return PartialView(facilityMaintainPlan);
        }

        /// <summary>
        /// FacilityMaintainPlanDelete action
        /// </summary>
        /// <param name="id">facilityMaintainPlan id for delete</param>
        /// <param name="item">facilityMaintainPlan item</param>
        /// <returns>return to FacilityMaintainPlanDelete action</returns>
        [SconitAuthorize(Permissions = "Url_FacilityMaintainPlan_Edit")]
        public ActionResult FacilityMaintainPlanDelete(int? id,string FCID)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<FacilityMaintainPlan>(id);
                SaveSuccessMessage(Resources.FMS.FacilityMaintainPlan.FacilityMaintainPlan_Deleted);
                
                return new RedirectToRouteResult(new RouteValueDictionary { 
                                                        { "action", "_FacilityMaintainPlanList" }, 
                                                        { "controller", "FacilityMaster" }, 
                                                        { "FCID", FCID } });
            }
        }
        #endregion


        #region facilityTrans

        [GridAction]
        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult _FacilityTrans(GridCommand command, FacilityTransSearchModel facilityTransSearchModel, string id)
        {
            ViewBag.FCID = id;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult _AjaxFacilityTransList(GridCommand command, FacilityTransSearchModel facilityTransSearchModel, string fcId)
        {
            SearchStatementModel searchStatementModel = this.FacilityTransPrepareSearchStatement(command, facilityTransSearchModel, fcId);
            return PartialView(GetAjaxPageData<FacilityTrans>(searchStatementModel, command));
        }


        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult _FacilityTransNew(string id)
        {
            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.FCID = id;
            return PartialView(facilityTrans);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult _FacilityTransEdit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                FacilityTrans facilityTrans = this.genericMgr.FindById<FacilityTrans>(id);
                return PartialView(facilityTrans);
            }
        }


        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult FacilityTransDelete(int? id, string item)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<FacilityMaintainPlan>(id);
                SaveSuccessMessage(Resources.FMS.FacilityMaintainPlan.FacilityMaintainPlan_Deleted);
                return RedirectToAction("FacilityMaintainPlan/" + item);
            }
        }





        [SconitAuthorize(Permissions = "Url_FacilityMaster_View")]
        public ActionResult GenerateFacilityOrder()
        {
           facilityMgr.GenerateFacilityMaintainPlan();
           return View("Index");
            
        }


        [SconitAuthorize(Permissions = "Url_FacilityMaster_View")]
        public ActionResult GenerateOrderFromFacility()
        {
            string facilityName = "FC000000008";
            facilityMgr.CreateFacilityOrder(facilityName);
            return View("Index");

        }
        #endregion

      
        private SearchStatementModel PrepareSearchStatement(GridCommand command, FacilityMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("FCID", searchModel.FCID, HqlStatementHelper.LikeMatchMode.Start, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ChargePerson", searchModel.ChargePerson, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ChargeSite", searchModel.ChargeSite, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ChargeOrganization", searchModel.ChargeOrganization, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("RefenceCode", searchModel.RefenceCode, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MaintainGroup", searchModel.MaintainGroup, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("OwnerDescription", searchModel.OwnerDescription, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel FacilityMaintainPlanPrepareSearchStatement(GridCommand command, MaintainPlanSearchModel maintainPlanSearchModel, string fcId)
        {
            string whereStatement = " where f.FCID='" + fcId + "'";

            IList<object> param = new List<object>();

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectFacilityMaintainPlanCountStatement;
            searchStatementModel.SelectStatement = selectFacilityMaintainPlanStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel FacilityTransPrepareSearchStatement(GridCommand command, FacilityTransSearchModel facilityTransSearchModel, string fcId)
        {
            string whereStatement = " where f.FCID='" + fcId + "'";

            IList<object> param = new List<object>();

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectFacilityTransCountStatement;
            searchStatementModel.SelectStatement = selectFacilityTransStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


    }
}
