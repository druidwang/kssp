using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Services.Transaction;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.CUST;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.CUST;

namespace com.Sconit.Web.Controllers.CUST
{
    public class ItemPropertyController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        private static string selectCountStatement = "select count(*) from ItemProperty as i";

        private static string selectStatement = "select i from ItemProperty as i";
        //
        // GET: /ProdLineEx/

        #region Public Method

        #region View

        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "CUST_ItemProperty_View")]
        public ActionResult List(GridCommand command, ItemPropertySearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

       
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "CUST_ItemProperty_View")]
        public ActionResult _AjaxList(GridCommand command, ItemPropertySearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ItemProperty>(searchStatementModel, command));
        }
        #endregion

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "CUST_ItemProperty_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ItemProperty itemProperty = this.genericMgr.FindAll<ItemProperty>("select i from ItemProperty as i where i.Id=? ", new object[] { id })[0];
                return View(itemProperty);
            }
        }


        [SconitAuthorize(Permissions = "CUST_ItemProperty_View")]
        public ActionResult Edit(ItemProperty itemProperty)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>("select count(*)  from ItemProperty as f where f.RmItem=? and f.Viscosity=? and  f.SfgItem=?  and f.Flow=? and Id!=? ",
                    new object[] { itemProperty.RmItem, itemProperty.Viscosity, itemProperty.SfgItem, itemProperty.Flow, itemProperty.Id })[0] > 0)
                {
                    base.SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_SameGlueMaterialCodeAlreadyExists, itemProperty.RmItem, itemProperty.Viscosity, itemProperty.SfgItem, itemProperty.Flow));
                    return View(itemProperty);
                }
                this.genericMgr.UpdateWithTrim(itemProperty);
                SaveSuccessMessage(Resources.CUST.ItemProperty.ItemProperty_Updated);
            }

            return View(itemProperty);
        }


        [SconitAuthorize(Permissions = "CUST_ItemProperty_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "CUST_ItemProperty_View")]
        public ActionResult New(ItemProperty itemProperty)
        {
            try
            {
                //ModelState.Remove("Qty");
                if (ModelState.IsValid)
                {
                    if (this.genericMgr.FindAll<long>("select count(*)  from ItemProperty as f where f.RmItem=? and f.Viscosity=? and  f.SfgItem=?  and f.Flow=? ",
                      new object[] { itemProperty.RmItem, itemProperty.Viscosity, itemProperty.SfgItem, itemProperty.Flow })[0] > 0)
                    {
                        base.SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_SameGlueMaterialCodeAlreadyExists, itemProperty.RmItem, itemProperty.Viscosity, itemProperty.SfgItem, itemProperty.Flow));
                        return View(itemProperty);
                    }
                    this.genericMgr.CreateWithTrim(itemProperty);
                    SaveSuccessMessage(Resources.CUST.ItemProperty.ItemProperty_Added);
                    int id = itemProperty.Id;
                  
                    //return RedirectToAction("Edit", new object[] { ProductLine, Item });
                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "ItemProperty" }, { "id", id } });
                }
            }
            catch (Exception e)
            {
                if (e is CommitResourceException)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_SameGlueMaterialAttributeAlreadyExists);
                }
                
            }

            return View(itemProperty);
        }

        [SconitAuthorize(Permissions = "CUST_ItemProperty_View")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ItemProperty itemProperty = this.genericMgr.FindAll<ItemProperty>("select i from ItemProperty as i where i.Id=? ", new object[] { id })[0];
                this.genericMgr.Delete(itemProperty);
                SaveSuccessMessage(Resources.CUST.ItemProperty.ItemProperty_Deleted);
                return RedirectToAction("List");
               
            }
        }

        #endregion
        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, ItemPropertySearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("RmItem", searchModel.RmItem, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "i", ref whereStatement, param);
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
