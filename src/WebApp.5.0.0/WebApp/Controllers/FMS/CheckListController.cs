/// <summary>
/// Summary description for CheckListController
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
    using System.Xml;
    using System;
    using System.Xml.Linq;
    using System.Text;
    #endregion

    /// <summary>
    /// This controller response to control the CheckList.
    /// </summary>
    public class CheckListController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the checkList security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the checkList
        /// </summary>
        private static string selectCountStatement = "select count(*) from CheckListMaster as c";

        /// <summary>
        /// hql to get all of the checkList
        /// </summary>
        private static string selectStatement = "select c from CheckListMaster as c";

        /// <summary>
        /// hql to get count of the checkList by checkList's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from CheckListMaster as c where c.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for CheckList controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_CheckList_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">CheckList Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CheckList_View")]
        public ActionResult List(GridCommand command, CheckListSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">CheckList Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CheckList_View")]
        public ActionResult _AjaxList(GridCommand command, CheckListSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<CheckListMaster>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_CheckList_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="checkList">CheckList Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CheckList_Edit")]
        public ActionResult New(CheckListMaster checkList)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { checkList.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, checkList.Code);
                }
                else
                {
                  
                    this.genericMgr.CreateWithTrim(checkList);
                    SaveSuccessMessage(Resources.FMS.CheckList.CheckList_Added);
                    return RedirectToAction("Edit/" + checkList.Code);
                }
            }

            return View(checkList);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">checkList id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CheckList_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                CheckListMaster checkList = this.genericMgr.FindById<CheckListMaster>(id);
                return View(checkList);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="checkList">CheckList Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_CheckList_Edit")]
        public ActionResult Edit(CheckListMaster checkList)
        {
            if (ModelState.IsValid)
            {
              
                this.genericMgr.UpdateWithTrim(checkList);
                SaveSuccessMessage(Resources.FMS.CheckList.CheckList_Added);
            }

            return View(checkList);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">checkList id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_CheckList_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<CheckListMaster>(id);
                SaveSuccessMessage(Resources.FMS.CheckList.CheckList_Deleted);
                return RedirectToAction("List");
            }
        }


        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public ActionResult _CheckListDetail(string CheckListCode)
        {

          
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_CheckList_Edit")]
        public ActionResult _SelectBatchEditing(string checkListCode)
        {

            IList<CheckListDetail> checkListDetailList = genericMgr.FindAll<CheckListDetail>("from CheckListDetail where CheckListCode=?", checkListCode);
            return View(new GridModel(checkListDetailList));
        }

        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">CheckList Search Model</param>
        /// <returns>return checkList search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, CheckListSearchModel searchModel)
        {            
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Description, HqlStatementHelper.LikeMatchMode.Anywhere, "c", ref whereStatement, param);
           
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }




    }
}
