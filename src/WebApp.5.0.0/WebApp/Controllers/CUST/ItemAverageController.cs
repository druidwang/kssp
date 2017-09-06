using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.CUST;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.CUST;

namespace com.Sconit.Web.Controllers.CUST
{
    public class ItemAverageController : WebAppBaseController
    {
        //
        // GET: /ItemAverage/

        private static string selectCountStatement = "select count(*) from ItemAverage as i";

        private static string selectStatement = "from ItemAverage as i";

        //public IGenericMgr genericMgr { get; set; }
        //
        // GET: /ItemAverage/
        #region  public
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ItemAverage_View")]
        [GridAction]
        public ActionResult List(GridCommand command, ItemAverageSearchModel searchModel)
        {
            TempData["ItemAverage"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

         [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ItemAverage_View")]
        public ActionResult _AjaxList(GridCommand command, ItemAverageSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ItemAverage>(searchStatementModel, command));
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_ItemAverage_View")]
        public ActionResult _Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<ItemAverage>(Id);
               // SaveSuccessMessage(Resources.INV.StockTakeLocation.StockTakeLocation_Deleted);
            }
            IList<ItemAverage> ItemAverageList = genericMgr.FindAll<ItemAverage>();
            return PartialView(new GridModel(ItemAverageList));
        }


        [SconitAuthorize(Permissions = "Url_ItemAverage_View")]
        public ActionResult New()
        {

            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ItemAverage_View")]
        public ActionResult New(ItemAverage itemAverage)
        {
            if (ModelState.IsValid)
            {
                IList<ItemAverage> itemAverageList = genericMgr.FindAll<ItemAverage>("from ItemAverage as i where i.Item=?", itemAverage.Item);
                if (itemAverageList.Count > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ItemAlreadyExists);

                }
                else{
                this.genericMgr.CreateWithTrim(itemAverage);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_AddedSuccessfully);
                return RedirectToAction("List");
                }
            }
            return View(itemAverage);
        }


        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ItemAverageSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Item", searchModel.Item, HqlStatementHelper.LikeMatchMode.End, "i", ref whereStatement, param);

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

    }
}
