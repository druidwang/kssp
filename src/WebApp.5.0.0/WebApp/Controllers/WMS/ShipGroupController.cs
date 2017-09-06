
namespace com.Sconit.Web.Controllers.WMS
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using Telerik.Web.Mvc;
    using System.Web.Routing;
    using com.Sconit.Web.Models;
    using com.Sconit.Utility;
    using com.Sconit.Web.Models.SearchModels.WMS;
    using com.Sconit.Entity.WMS;
    using com.Sconit.CodeMaster;
    using com.Sconit.Entity.ACC;

    public class ShipGroupController : WebAppBaseController
    {
        #region 发货组

        private static string selectCountStatement = "select count(*) from PickGroup as p";

        private static string selectStatement = "select p from PickGroup as p";

        private static string selectCountPickGroupStatement = "select count(*) from PickGroup as s where s.PickGroupCode = ?";


        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult List(GridCommand command, PickGroupSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult _AjaxList(GridCommand command, PickGroupSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<PickGroup>(searchStatementModel, command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult New(PickGroup pickGroup)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                if (base.genericMgr.FindAll<long>(selectCountPickGroupStatement, new object[] { pickGroup.PickGroupCode })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.WMS.ShipGroup.ShipGroup_Errors_Existing_ShipGroupCode, pickGroup.PickGroupCode);
                }
                pickGroup.Type = PickGroupType.Ship;
                genericMgr.Create(pickGroup);
                SaveSuccessMessage(Resources.WMS.ShipGroup.ShipGroup_Added);
                return RedirectToAction("Edit/" + pickGroup.PickGroupCode);
            }
            return View(pickGroup);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult Edit(string Id)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }

            return View("Edit", "", Id);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _Edit(string Id)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.PickGroupCode = Id;
                PickGroup pickGroup = genericMgr.FindById<PickGroup>(Id);
                return PartialView(pickGroup);
            }

        }


        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _Edit(PickGroup pickGroup)
        {

            if (ModelState.IsValid)
            {
                pickGroup.Type = PickGroupType.Ship;
                genericMgr.Update(pickGroup);
                SaveSuccessMessage(Resources.WMS.ShipGroup.ShipGroup_Updated);
            }

            TempData["TabIndex"] = 0;
            return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_Edit" }, 
                                                       { "controller", "PickGroup" } ,
                                                       { "Id", pickGroup.PickGroupCode }
                                                   });
        }


        [SconitAuthorize(Permissions = "Url_PickGroup_Delete")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<PickGroup>(id);
                SaveSuccessMessage(Resources.WMS.ShipGroup.ShipGroup_Deleted);
                return RedirectToAction("List");
            }
        }
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PickGroupSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("PickGroupCode", searchModel.PickGroupCode, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", (int)PickGroupType.Ship, "p", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        #endregion


        #region 发货规则
        private static string selectPickRuleCountStatement = "select count(*) from PickRule as  pr  ";
        private static string selectPickRuleStatement = "select  pr  from PickRule as  pr ";

        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult _PickRule(string Id)
        {
            ViewBag.PickGroupCode = Id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult _PickRuleList(GridCommand command, PickRuleSearchModel searchModel, string PickGroupCode)
        {
            ViewBag.PickGroupCode = PickGroupCode;
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult _AjaxPickRuleList(GridCommand command, PickRuleSearchModel searchModel, string PickGroupCode)
        {
            SearchStatementModel searchStatementModel = PrepareSearchPickRuleStatement(command, searchModel, PickGroupCode);
            return PartialView(GetAjaxPageData<PickRule>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _PickRuleNew(String PickGroupCode)
        {
            PickRule pickRule = new PickRule();
            pickRule.PickGroupCode = PickGroupCode;
            return PartialView(pickRule);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _PickRuleNew(PickRule pickRule, string PickGroupCode, string Location)
        {
            if (ModelState.IsValid)
            {
                pickRule.PickGroupCode = PickGroupCode;
                pickRule.Location = Location;
                genericMgr.Create(pickRule);

                SaveSuccessMessage(Resources.WMS.ShipRule.ShipRule_Added);
                return RedirectToAction("_PickRuleEdit/" + pickRule.Id);
            }
            return PartialView(pickRule);
        }

        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult DeletePickRule(int? id, string PickGroupCode)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<PickRule>(id);
                SaveSuccessMessage(Resources.WMS.ShipRule.ShipRule_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary { 
                                                        { "action", "_PickRuleList" }, 
                                                        { "controller", "PickGroup" }, 
                                                        { "PickGroupCode", PickGroupCode } });
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _PickRuleEdit(int? Id)
        {
            if (!Id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                PickRule pickRule = genericMgr.FindById<PickRule>(Id);
                return PartialView(pickRule);
            }

        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _PickRuleEdit(PickRule pickRule, string PickGroupCode)
        {
            if (ModelState.IsValid)
            {
                ViewBag.PickGroupCode = PickGroupCode;
                pickRule.PickGroupCode = PickGroupCode;
                genericMgr.Update(pickRule);

                SaveSuccessMessage(Resources.WMS.ShipRule.ShipRule_Updated);
            }

            TempData["TabIndex"] = 1;
            return PartialView(pickRule);
        }

        private SearchStatementModel PrepareSearchPickRuleStatement(GridCommand command, PickRuleSearchModel searchModel, string pickGroupCode)
        {
            string whereStatement = "  where pr.PickGroupCode='" + pickGroupCode + "' ";
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Location", searchModel.Location, HqlStatementHelper.LikeMatchMode.Start, "pr", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectPickRuleCountStatement;
            searchStatementModel.SelectStatement = selectPickRuleStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        #endregion


        #region 发货用户
        private static string selectPickUserCountStatement = "select count(*) from PickUser as  pu  ";
        private static string selectPickUserStatement = "select  pu  from PickUser as  pu ";

        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult _PickUser(string Id)
        {
            ViewBag.PickGroupCode = Id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult _PickUserList(GridCommand command, PickUserSearchModel searchModel, string PickGroupCode)
        {
            ViewBag.PickGroupCode = PickGroupCode;
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipGroup_View")]
        public ActionResult _AjaxPickUserList(GridCommand command, PickUserSearchModel searchModel, string PickGroupCode)
        {
            SearchStatementModel searchStatementModel = PrepareSearchPickUserStatement(command, searchModel, PickGroupCode);
            return PartialView(GetAjaxPageData<PickUser>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _PickUserNew(String PickGroupCode)
        {
            PickUser pickRule = new PickUser();
            pickRule.PickGroupCode = PickGroupCode;
            return PartialView(pickRule);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _PickUserNew(PickUser pickUser)
        {


            User user = genericMgr.FindById<User>(Convert.ToInt32(pickUser.PickUserId));
            pickUser.PickUserId = user.Id;
            pickUser.PickUserName = user.FullName;
            genericMgr.Create(pickUser);

            SaveSuccessMessage(Resources.WMS.ShipUser.ShipUser_Added);
            return RedirectToAction("_PickUserEdit/" + pickUser.Id);

        }

        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult DeletePickUser(int? id, string PickGroupCode)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<PickUser>(id);
                SaveSuccessMessage(Resources.WMS.ShipUser.ShipUser_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary { 
                                                        { "action", "_PickUserList" }, 
                                                        { "controller", "PickGroup" }, 
                                                        { "PickGroupCode", PickGroupCode } });
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _PickUserEdit(int? Id)
        {
            if (!Id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                PickUser pickRule = genericMgr.FindById<PickUser>(Id);
                return PartialView(pickRule);
            }

        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ShipGroup_Edit")]
        public ActionResult _PickUserEdit(PickUser pickUser)
        {
            if (ModelState.IsValid)
            {
                ViewBag.PickGroupCode = pickUser.PickGroupCode;
                User user = genericMgr.FindById<User>(Convert.ToInt32(pickUser.PickUserId));
                pickUser.PickUserId = user.Id;
                pickUser.PickUserName = user.FullName;
                genericMgr.Update(pickUser);

                SaveSuccessMessage(Resources.WMS.ShipUser.ShipUser_Updated);
            }

            TempData["TabIndex"] = 2;
            return PartialView(pickUser);
        }

        private SearchStatementModel PrepareSearchPickUserStatement(GridCommand command, PickUserSearchModel searchModel, string pickGroupCode)
        {
            string whereStatement = "  where pu.PickGroupCode='" + pickGroupCode + "' ";
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("PickUserName", searchModel.PickUserName, HqlStatementHelper.LikeMatchMode.Start, "pu", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectPickUserCountStatement;
            searchStatementModel.SelectStatement = selectPickUserStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        #endregion

    }
}
