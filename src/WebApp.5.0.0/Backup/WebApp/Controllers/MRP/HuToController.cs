using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Web.Models;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Service;
using Castle.Services.Transaction;
using System.Web.Routing;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.MD;


namespace com.Sconit.Web.Controllers.MRP
{
    public class HuToController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        private static string selectCountStatement = "select count(*) from HuTo as h";

        private static string selectStatement = "select h from HuTo as h";

        #region HuTo Method

        #region View

        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_CUST_HuTo")]
        public ActionResult List(GridCommand command, HuToSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CUST_HuTo")]
        public ActionResult _AjaxList(GridCommand command, HuToSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<HuTo>(searchStatementModel, command));
        }
        #endregion

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CUST_HuTo")]
        public ActionResult Edit(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return HttpNotFound();
            }
            else
            {
                HuTo huto = this.genericMgr.FindAll<HuTo>("select h from HuTo as h where h.Code=?", new object[] { Code })[0];
                return View(huto);
            }
        }


        [SconitAuthorize(Permissions = "Url_CUST_HuTo")]
        public ActionResult Edit(HuTo huto)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(huto);
                SaveSuccessMessage(Resources.MRP.HuTo.HuTo_Updated);
            }

            return View(huto);
        }


        [SconitAuthorize(Permissions = "Url_CUST_HuTo")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CUST_HuTo")]
        public ActionResult New(HuTo huto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this.genericMgr.CreateWithTrim(huto);
                    SaveSuccessMessage(Resources.MRP.HuTo.HuTo_Added);
                    string Code = huto.Code;

                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "HuTo" }, { "Code", Code } });
                }
            }
            catch (Exception e)
            {
                if (e is CommitResourceException)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheDirectionAlreadyExits);
                }

            }

            return View(huto);
        }

        [SconitAuthorize(Permissions = "Url_CUST_HuTo")]
        public ActionResult Delete(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return HttpNotFound();
            }
            else
            {
                HuTo huto = this.genericMgr.FindAll<HuTo>("select h from HuTo as h where h.Code=? ", new object[] { Code })[0];
                this.genericMgr.Delete(huto);
                SaveSuccessMessage(Resources.MRP.HuTo.HuTo_Deleted);
                return RedirectToAction("List");
                // return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "ProdLineEx" }, { "id", itemEx.Code } });
            }
        }

        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, HuToSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "h", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by h.CreateDate,Code desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion


        #region HuToMapping Method

        #region View

        public ActionResult HuToMappingIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_CUST_HuToMapping")]
        public ActionResult HuToMappingList(GridCommand command, HuToMappingSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CUST_HuToMapping")]
        public ActionResult _AjaxHuToMappingList(GridCommand command, HuToMappingSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            var gridModel = GetAjaxPageData<HuToMapping>(searchStatementModel, command);
            foreach (var huToMapping in gridModel.Data)
            {

                huToMapping.HuToDescription = this.genericMgr.FindById<HuTo>(huToMapping.HuTo).Description;
                if (!string.IsNullOrWhiteSpace(huToMapping.Flow))
                {
                    huToMapping.FlowDescription = this.genericMgr.FindById<FlowMaster>(huToMapping.Flow).Description;
                }
                if (!string.IsNullOrWhiteSpace(huToMapping.Item))
                {
                    huToMapping.ItemDescription = this.genericMgr.FindById<Item>(huToMapping.Item).Description;
                }
                if (!string.IsNullOrWhiteSpace(huToMapping.Fg))
                {
                    huToMapping.FgDescription = this.genericMgr.FindById<Item>(huToMapping.Fg).Description;
                }
                if (!string.IsNullOrWhiteSpace(huToMapping.Party))
                {

                    var party = this.genericMgr.FindById<Party>(huToMapping.Party);
                    huToMapping.PartyName = this.genericMgr.FindById<Party>(huToMapping.Party).Name;
                    if (party is Customer)
                    {
                        huToMapping.PartyType = Resources.EXT.ControllerLan.Con_Customer;
                    }
                    else if (party is Region)
                    {
                        huToMapping.PartyType = Resources.EXT.ControllerLan.Con_Area;
                    }
                    else
                    {
                        huToMapping.PartyType = Resources.EXT.ControllerLan.Con_Supplier;
                    }
                }
            }
            return PartialView(gridModel);
        }
        #endregion

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CUST_HuToMapping")]
        public ActionResult HuToMappingEdit(int id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            else
            {
                HuToMapping huto = this.genericMgr.FindAll<HuToMapping>("select h from HuToMapping as h where h.Id=?", new object[] { id })[0];
                return View(huto);
            }
        }


        [SconitAuthorize(Permissions = "Url_CUST_HuToMapping")]
        public ActionResult HuToMappingEdit(HuToMapping huToMapping)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(huToMapping);
                SaveSuccessMessage(Resources.MRP.HuToMapping.HuToMapping_Updated);
            }

            return View(huToMapping);
        }


        [SconitAuthorize(Permissions = "Url_CUST_HuToMapping")]
        public ActionResult HuToMappingNew()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CUST_HuToMapping")]
        public ActionResult HuToMappingNew(HuToMapping huToMapping)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this.genericMgr.CreateWithTrim(huToMapping);
                    SaveSuccessMessage(Resources.MRP.HuToMapping.HuToMapping_Added);
                    int id = huToMapping.Id;

                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "HuToMappingEdit" }, { "controller", "HuTo" }, { "id", id } });
                }
            }
            catch (Exception e)
            {
                if (e is CommitResourceException)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheHuToMappingAlreadyExits);
                }
                else
                {
                    SaveErrorMessage(e);
                }
            }

            return View(huToMapping);
        }

        [SconitAuthorize(Permissions = "Url_CUST_HuToMapping")]
        public ActionResult HuToMappingDelete(int id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            else
            {
                HuToMapping huto = this.genericMgr.FindAll<HuToMapping>("select h from HuToMapping as h where h.Id=? ", new object[] { id })[0];
                this.genericMgr.Delete(huto);
                SaveSuccessMessage(Resources.MRP.HuToMapping.HuToMapping_Deleted);
                return RedirectToAction("List");
                // return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "ProdLineEx" }, { "id", itemEx.Code } });
            }
        }

        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, HuToMappingSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            if (!string.IsNullOrWhiteSpace(searchModel.Flow))
                HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "h", ref whereStatement, param);
            if (!string.IsNullOrWhiteSpace(searchModel.HuTo))
                HqlStatementHelper.AddEqStatement("HuTo", searchModel.HuTo, "h", ref whereStatement, param);
            if (!string.IsNullOrWhiteSpace(searchModel.Item))
                HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "h", ref whereStatement, param);
            if (!string.IsNullOrWhiteSpace(searchModel.Party))
                HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "h", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by h.Id desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from HuToMapping as h";
            searchStatementModel.SelectStatement = "select h from HuToMapping as h";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion
    }
}
