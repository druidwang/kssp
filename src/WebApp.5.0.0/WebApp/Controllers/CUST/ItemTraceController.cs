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
    public class ItemTraceController : WebAppBaseController
    {
        //
        // GET: /ItemTrace/

        private static string selectCountStatement = "select count(*) from ItemTrace as i";

        private static string selectStatement = "from ItemTrace as i";

        //public IGenericMgr genericMgr { get; set; }
        //
        // GET: /FailCode/
        #region  public
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ItemTrace_View")]
        [GridAction]
        public ActionResult List(GridCommand command, ItemTraceSearchModel searchModel)
        {
            TempData["ItemTrace"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ItemTrace_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, ItemTraceSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ItemTrace>(searchStatementModel, command));
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_ItemTrace_View")]
        public ActionResult _Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<ItemTrace>(Id);
               // SaveSuccessMessage(Resources.INV.StockTakeLocation.StockTakeLocation_Deleted);
            }
            IList<ItemTrace> ItemTraceList = genericMgr.FindAll<ItemTrace>();
            return PartialView(new GridModel(ItemTraceList));
        }


        [SconitAuthorize(Permissions = "Url_ItemTrace_View")]
        public ActionResult New()
        {

            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ItemTrace_View")]
        public ActionResult New(ItemTrace itemTrace)
        {
            if (ModelState.IsValid)
            {
                IList<ItemTrace> itemTraceList = genericMgr.FindAll<ItemTrace>("from ItemTrace as i where i.Item=?", itemTrace.Item);
                if (itemTraceList.Count > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_KeyArticleAlreadyExists);

                }
                else{
                this.genericMgr.CreateWithTrim(itemTrace);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_AddedSuccessfully);
                return RedirectToAction("List");
                }
            }
            return View(itemTrace);
        }


        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ItemTraceSearchModel searchModel)
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
