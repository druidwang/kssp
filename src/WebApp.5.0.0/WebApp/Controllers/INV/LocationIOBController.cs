using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models;
using com.Sconit.Entity.INV;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Web.Models.INV;

namespace com.Sconit.Web.Controllers.INV
{
    public class LocationIOBController : WebAppBaseController
    {

        #region Properties
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        private static string selectCountStatement = "select count(distinct Item) from LocationTransaction as l";

        private static string selectStatement = "select distinct l.Item from  LocationTransaction as l";


        #region public

        // [SconitAuthorize(Permissions = "Url_LocationIOB_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        // [SconitAuthorize(Permissions = "Url_LocationIOB_View")]
        public ActionResult List(GridCommand command, LocationTransactionSearchModel searchModel)
        {
            TempData["LocationTransactionSearchModel"] = searchModel;
            if (string.IsNullOrEmpty(searchModel.Location))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_LocationCanNotBeEmpty);
            }
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        //[SconitAuthorize(Permissions = "Url_LocationIOB_View")]
        public ActionResult _AjaxList(GridCommand command, LocationTransactionSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.Location))
            {
                return PartialView(new GridModel(new List<LocationTransactionView>()));
            }

            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<string> itemModel = GetAjaxPageData<string>(searchStatementModel, command);

            #region 拼接库存收发存对象
            IList<string> itemLocList = (IList<string>)itemModel.Data;
            string hql = "from LocationTransaction as l where l.Item in (?";
            IList<object> param = new List<object>();
            param.Add(itemLocList[0]);
            for (int i = 1; i < itemLocList.Count; i++)
            {
                hql += ",?";
                param.Add(itemLocList[i]);
            }
            hql += ")";

            hql += " and (l.LocationFrom = ? and l.LocationTo = ?)";
            param.Add(searchModel.Location.Trim());
            param.Add(searchModel.Location.Trim());

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                hql += " and l.Item = ?";
                param.Add(searchModel.Item.Trim());
            }
            IList<LocationTransaction> loctransList = genericMgr.FindAll<LocationTransaction>(hql, param.ToArray());
            IList<LocationTransactionView> groupLoctransList = (from l in loctransList
                                                                group l by new
                                                                {
                                                                    Item = l.Item,
                                                                    Location = l.Location
                                                                }
                                                                    into result
                                                                    select new LocationTransactionView
                                                                    {
                                                                        Item = result.Key.Item,
                                                                        GroupLocation = result.Key.Location,
                                                                        SumProcurementInQty = result.Sum(t => t.ProcurementInQty),
                                                                        SumProductionInQty = result.Sum(t => t.ProductionInQty),
                                                                        SumProductionOutQty = result.Sum(t => t.ProductionOutQty),
                                                                        SumDistributionOutQty = result.Sum(t => t.DistributionOutQty),
                                                                        SumTransferInQty = result.Sum(t => t.TransferInQty),
                                                                        SumTransferOutQty = result.Sum(t => t.TransferOutQty),
                                                                    }).ToList();
            #endregion

            GridModel<LocationTransactionView> groupModel = new GridModel<LocationTransactionView>();
            groupModel.Total = itemModel.Total;
            groupModel.Data = groupLoctransList;
            ViewBag.Total = groupModel.Total;


            return PartialView(groupModel);
        }
        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, LocationTransactionSearchModel searchModel)
        {
            string whereStatement = " where (l.LocationFrom = '" + searchModel.Location + "' or l.LocationTo = '" + searchModel.Location + "')";

            IList<object> param = new List<object>();


            //  HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "l", ref whereStatement, param);

            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("EffectiveDate", searchModel.StartDate, searchModel.EndDate, "l", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("EffectiveDate", searchModel.StartDate, "l", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("EffectiveDate", searchModel.EndDate, "l", ref whereStatement, param);
            }

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
