using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models.SearchModels.ORD;
using com.Sconit.Web.Models;
using com.Sconit.Entity.ORD;
using System.Text;
using com.Sconit.Service;
using com.Sconit.Web.Models.SearchModels.BIL;
using com.Sconit.Entity.BIL;


namespace com.Sconit.Web.Controllers.SP
{
    public class SupplierConsignmentController : WebAppBaseController
    {
        private static string selectCountStatement = "select count(*) from PlanBill as p";

        private static string selectStatement = "select p from PlanBill as p";

        //public ISystemMgr systemMgr { get; set; }
        //public IGenericMgr genericMgr { get; set; }

        #region public
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Consignment")]
        public ActionResult List(GridCommand command, PlanBillSearchModel searchModel)
        {
            TempData["PlanBillSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);

            return View();
        }

        [GridAction(EnableCustomBinding = false)]
        [SconitAuthorize(Permissions = "Url_Supplier_Consignment")]
        public ActionResult _AjaxList(GridCommand command, PlanBillSearchModel searchModel)
        {
            //int pageSize = command.PageSize;
            //int page = command.Page;
            command.PageSize = int.MaxValue;
            command.Page = 1;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            var gridData = GetAjaxPageData<PlanBill>(searchStatementModel, command);
            //command.PageSize = pageSize;
            //command.Page = page;
            gridData.Data = gridData.Data.GroupBy(p => new
                {
                    p.Party,
                    p.Item,
                    p.Uom,
                }, (k, g) => new PlanBill
                {
                    Party = k.Party,
                    Item = k.Item,
                    ItemDescription = g.First().ItemDescription,
                    Uom = k.Uom,
                    CurrentActingQty = g.Sum(q => q.PlanQty) - g.Sum(q => q.ActingQty),
                });
            return PartialView(gridData);
        }
        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PlanBillSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            SecurityHelper.AddBillPermissionStatement(ref whereStatement, "p", "Party", com.Sconit.CodeMaster.BillType.Procurement);

            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ReceiptNo", searchModel.ReceiptNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", com.Sconit.CodeMaster.BillType.Procurement, "p", ref whereStatement, param);

            if (searchModel.CreateDate_start != null & searchModel.CreateDate_End != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.CreateDate_start, searchModel.CreateDate_End, "p", ref whereStatement, param);
            }
            else if (searchModel.CreateDate_start != null & searchModel.CreateDate_End == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.CreateDate_start, "p", ref whereStatement, param);
            }
            else if (searchModel.CreateDate_start == null & searchModel.CreateDate_End != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.CreateDate_End, "p", ref whereStatement, param);
            }
            if (whereStatement == string.Empty)
            {
                whereStatement += " where p.PlanQty>p.ActingQty";
            }
            else
            {
                whereStatement += " and p.PlanQty>p.ActingQty";
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
