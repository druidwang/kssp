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
using com.Sconit.Entity.Exception;

namespace com.Sconit.Web.Controllers.MRP
{
    public class IslandController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        private static string selectCountStatement = "select count(*) from Island as i";

        private static string selectStatement = "select i from Island as i";
        //
        // GET: /ProdLineEx/

        #region Public Method

        #region View

        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_MRP_Island_View")]
        public ActionResult List(GridCommand command, IslandSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

       
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MRP_Island_View")]
        public ActionResult _AjaxList(GridCommand command, IslandSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Island>(searchStatementModel, command));
        }
        #endregion

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MRP_Island_View")]
        public ActionResult Edit(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return HttpNotFound();
            }
            else
            {
                Island island = this.genericMgr.FindAll<Island>("select i from Island as i where i.Code=? ", new object[] { Code })[0];
                return View(island);
            }
        }


        [SconitAuthorize(Permissions = "Url_MRP_Island_View")]
        public ActionResult Edit(Island island)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(island);
                SaveSuccessMessage(Resources.MRP.Island.Island_Updated);
            }

            return View(island);
        }


        [SconitAuthorize(Permissions = "Url_MRP_Island_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MRP_Island_View")]
        public ActionResult New(Island island)
        {
            try
            {
                //ModelState.Remove("Qty");
                if (ModelState.IsValid)
                {
                    this.genericMgr.CreateWithTrim(island);
                    SaveSuccessMessage(Resources.MRP.Island.Island_Added);
                    string Code = island.Code;
                  
                    //return RedirectToAction("Edit", new object[] { ProductLine, Item });
                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "Island" }, { "Code", Code } });
                }
            }
            catch (Exception e)
            {
                if (e is CommitResourceException)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheIslandAlreadyExits);
                }
                
            }

            return View(island);
        }

        [SconitAuthorize(Permissions = "Url_MRP_Island_View")]
        public ActionResult Delete(string Code)
        {
            try
            {
                if (string.IsNullOrEmpty(Code))
                {
                    return HttpNotFound();
                }
                else
                {
                    Island island = this.genericMgr.FindAll<Island>("select i from Island as i where i.Code=? ", new object[] { Code })[0];

                    this.genericMgr.Delete(island);
                    SaveSuccessMessage(Resources.MRP.Island.Island_Deleted);
                    return RedirectToAction("List");

                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_IslandAlreadyBeenOccupiedCanNotDeleted);
                return RedirectToAction("Edit", new { Code = Code });
            }

        }

        #endregion
        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, IslandSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "i", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by i.CreateDate desc";
            }

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
