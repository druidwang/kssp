/// <summary>
/// Summary description 
/// </summary>
namespace com.Sconit.Web.Controllers.SP
{
    #region reference
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using System.Text;
    using System;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This controller response to control the Address.
    /// </summary>
    public class SupplierReceiptController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the ProcurementGoodsReceipt security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql 
        /// </summary>
        private static string selectCountStatement = "select count(*) from ReceiptMaster as r";

        /// <summary>
        /// hql 
        /// </summary>
        private static string selectStatement = "select r from ReceiptMaster as r";
        //public IGenericMgr genericMgr { get; set; }

        #region public actions
        /// <summary>
        /// Index action for ProcurementGoodsReceipt controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Query")]
        public ActionResult Index()
        {
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Query")]
        public ActionResult _ReceiptDetail(string receiptNo)
        {
            string hql = "select r from ReceiptDetail as r where r.ReceiptNo = ?";
            IList<ReceiptDetail> receiptDetailList = genericMgr.FindAll<ReceiptDetail>(hql, receiptNo);
            return PartialView(receiptDetailList);
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Query")]
        public ActionResult Edit(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.ReceiptNo = receiptNo;
                ReceiptMaster rm = this.genericMgr.FindById<ReceiptMaster>(receiptNo);
                return View(rm);
            }
        }

        [SconitAuthorize(Permissions = "Url_ProcurementReceipt_Cancel")]
        public ActionResult CancelIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Detail_Query")]
        public ActionResult DetailIndex()
        {
            return View();
        }


        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> ReceiptMaster Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Query")]
        public ActionResult List(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementReceipt_Cancel")]
        public ActionResult CancelList(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, (ReceiptMasterSearchModel)searchCacheModel.SearchObject);
            return View(GetPageData<ReceiptMaster>(searchStatementModel, command));
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> ReceiptMaster Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Query")]
        public ActionResult _AjaxList(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<ReceiptMaster>()));
            }
            string whereStatement = string.Empty;
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageDataProcedure<ReceiptMaster>(procedureSearchStatementModel, command));
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Detail_Query")]
        public ActionResult DetailList(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxRecDetList(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<ReceiptDetail>()));
            }
            string whereStatement = string.Empty;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageDataProcedure<ReceiptDetail>(procedureSearchStatementModel, command));
        }
        #region
        [GridAction(EnableCustomBinding = true)]
        public void ExportXLS(ReceiptMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            string whereStatement = null;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            var ReceiptDetailLists = GetAjaxPageDataProcedure<ReceiptDetail>(procedureSearchStatementModel, command);
            List<ReceiptDetail> ReceiptDetailList = ReceiptDetailLists.Data.ToList();
            ExportToXLS<ReceiptDetail>("DailsOfSupplierReceiptMaster.xls", ReceiptDetailList);
        }
        #endregion

        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ReceiptMaster Search Model</param>
        /// <returns>return ReceiptMaster search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            string whereStatement = " where r.OrderType in (" + (int)com.Sconit.CodeMaster.OrderType.Procurement + "," + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + ")"
            + " and r.OrderSubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("ReceiptNo", searchModel.ReceiptNo, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("IpNo", searchModel.IpNo, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("OrderType", searchModel.GoodsReceiptOrderType, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("WMSNo", searchModel.ExternalReceiptNo, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Dock", searchModel.Dock, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "r", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, true);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "r", "OrderType", "r", "PartyFrom", "r", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, true);

            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "r", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "r", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "r", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by CreateDate desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private ProcedureSearchStatementModel PrepareProcedureSearchStatement(GridCommand command, ReceiptMasterSearchModel searchModel, string whereStatement)
        {
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ReceiptNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.Procurement + "," + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + ","
                + (int)com.Sconit.CodeMaster.OrderType.SubContract,
                Type = NHibernate.NHibernateUtil.String
            });
            paraList.Add(new ProcedureParameter { Parameter = (int)com.Sconit.CodeMaster.OrderSubType.Normal, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpDetailType, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.StartDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.EndDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Dock, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Item, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalReceiptNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ManufactureParty, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "ReceiptMasterStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : null, Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? (command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc") : "asc", Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.PageSize, Type = NHibernate.NHibernateUtil.Int32 });
            pageParaList.Add(new ProcedureParameter { Parameter = command.Page, Type = NHibernate.NHibernateUtil.Int32 });
            var procedureSearchStatementModel = new ProcedureSearchStatementModel();
            procedureSearchStatementModel.Parameters = paraList;
            procedureSearchStatementModel.PageParameters = pageParaList;
            procedureSearchStatementModel.CountProcedure = "USP_Search_RecMstrCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_RecMstr";
            return procedureSearchStatementModel;
        }


        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, ReceiptMasterSearchModel searchModel, string whereStatement)
        {
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ReceiptNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract,
                Type = NHibernate.NHibernateUtil.String
            });
            paraList.Add(new ProcedureParameter { Parameter = (int)com.Sconit.CodeMaster.OrderSubType.Normal, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpDetailType, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.StartDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.EndDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Dock, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Item, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalReceiptNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ManufactureParty, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Int64 });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : null, Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? (command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc") : "asc", Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.PageSize, Type = NHibernate.NHibernateUtil.Int32 });
            pageParaList.Add(new ProcedureParameter { Parameter = command.Page, Type = NHibernate.NHibernateUtil.Int32 });

            var procedureSearchStatementModel = new ProcedureSearchStatementModel();
            procedureSearchStatementModel.Parameters = paraList;
            procedureSearchStatementModel.PageParameters = pageParaList;
            procedureSearchStatementModel.CountProcedure = "USP_Search_RecDetCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_RecDet";

            return procedureSearchStatementModel;
        }
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Query")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstr(ReceiptMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            string whereStatement = string.Empty;
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);
            ExportToXLS<ReceiptMaster>("SupplierReceiptOrderMaster.xls", GetAjaxPageDataProcedure<ReceiptMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
    }
}
