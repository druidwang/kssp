using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models;
using com.Sconit.Entity.MD;
using NHibernate.Type;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using com.Sconit.Web.Models.SearchModels.MD;
using com.Sconit.Entity.SCM;

namespace com.Sconit.Web.Controllers.MD
{
    public class SwitchTradingController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from SwitchTrading as f";

        private static string selectStatement = "from SwitchTrading as f";

        public ILocationDetailMgr locationMgr { get; set; }

        #region  public
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_SwitchTrading_View")]
        [GridAction]
        public ActionResult List(GridCommand command, SwitchTradingSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_SwitchTrading_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, SwitchTradingSearchModel searchModel)
        {
            TempData["GridCommand"] = command;
            TempData["searchModel"] = searchModel;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<SwitchTrading> gridModel = GetAjaxPageData<SwitchTrading>(searchStatementModel, command);
            return PartialView(gridModel);
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_SwitchTrading_View")]
        public ActionResult _Insert(SwitchTrading SwitchTrading)
        {
            if (CheckSwitchTrading(SwitchTrading))
            {
                this.genericMgr.Create(SwitchTrading);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_AddedSuccessfully);
            }
            GridCommand command = (GridCommand)TempData["GridCommand"];
            SwitchTradingSearchModel searchModel = (SwitchTradingSearchModel)TempData["searchModel"];
            TempData["GridCommand"] = command;
            TempData["searchModel"] = searchModel;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<SwitchTrading> gridModel = GetAjaxPageData<SwitchTrading>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SwitchTrading_View")]
        public ActionResult _Update(SwitchTrading switchTrading, string id)
        {
            if (CheckSwitchTrading(switchTrading))
            {
                SwitchTrading upSwitchTrading = base.genericMgr.FindById<SwitchTrading>(Convert.ToInt32(id));
                upSwitchTrading.Flow = switchTrading.Flow;
                upSwitchTrading.Supplier = switchTrading.Supplier;
                upSwitchTrading.Customer = switchTrading.Customer;
                upSwitchTrading.PurchaseGroup = switchTrading.PurchaseGroup;
                upSwitchTrading.SalesOrg = switchTrading.SalesOrg;
                upSwitchTrading.DistrChan = switchTrading.DistrChan;
                upSwitchTrading.PriceList = switchTrading.PriceList;
                upSwitchTrading.DIVISION = switchTrading.DIVISION;
                this.genericMgr.Update(upSwitchTrading);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ModificateSuccessfully);
            }
            GridCommand command = (GridCommand)TempData["GridCommand"];
            SwitchTradingSearchModel searchModel = (SwitchTradingSearchModel)TempData["searchModel"];
            TempData["GridCommand"] = command;
            TempData["searchModel"] = searchModel;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<SwitchTrading> gridModel = GetAjaxPageData<SwitchTrading>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SwitchTrading_View")]
        public ActionResult _Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<SwitchTrading>(Convert.ToInt32(Id));
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_DeletedSuccessfully);
            }
            GridCommand command = (GridCommand)TempData["GridCommand"];
            SwitchTradingSearchModel searchModel = (SwitchTradingSearchModel)TempData["searchModel"];
            TempData["GridCommand"] = command;
            TempData["searchModel"] = searchModel;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<SwitchTrading> gridModel = GetAjaxPageData<SwitchTrading>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, SwitchTradingSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Supplier", searchModel.Supplier, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Customer", searchModel.Customer, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PurchaseGroup", searchModel.PurchaseGroup, "f", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private bool CheckSwitchTrading(SwitchTrading switchTrading)
        {
            bool hasError = false;
            FlowMaster flowMaster =new FlowMaster();
            if (string.IsNullOrWhiteSpace(switchTrading.SalesOrg))
            {
                hasError = true;
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_SaleOrgCanNotBeEmpty);
            }
            if (string.IsNullOrWhiteSpace(switchTrading.DistrChan))
            {
                hasError = true;
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_DistrChanCanNotBeEmpty);
            }
            if (string.IsNullOrWhiteSpace(switchTrading.Flow))
            {
                hasError = true;
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_FlowCodeCanNotBeEmpty);
            }
            else
            {
                try
                {
                    flowMaster = this.genericMgr.FindById<FlowMaster>(switchTrading.Flow);
                }
                catch (Exception e)
                {
                    hasError = true;
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_FlowCodeIsWrong);
                }

            }
            if (string.IsNullOrWhiteSpace(switchTrading.Supplier))
            {
                //hasError = true;
                //SaveErrorMessage("供应商不能为空。");
            }
            else
            {
                try
                {
                    var supplier = this.genericMgr.FindById<Supplier>(switchTrading.Supplier);
                }
                catch (Exception e)
                {
                    hasError = true;
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_SupplierIsWrong);
                }

            }
            if (!string.IsNullOrWhiteSpace(switchTrading.Customer))
            {
                try
                {
                    var supplier = this.genericMgr.FindById<Customer>(switchTrading.Customer);
                }
                catch (Exception e)
                {
                    hasError = true;
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CustomerIsWrong);
                }
            }
            //if (string.IsNullOrWhiteSpace(switchTrading.PurchaseGroup))
            //{
            //    hasError = true;
            //    SaveErrorMessage("采购组不能为空。");
            //}
            if (this.genericMgr.FindAllWithNativeSql<int>(" select isnull(count(*),0) as counts from MD_SwitchTrading where Flow=?  and Id <>? ", new object[] { switchTrading.Flow, switchTrading.Id }, new IType[] { NHibernate.NHibernateUtil.String,NHibernate.NHibernateUtil.Int32 })[0] > 0)
            {
                hasError = true;
                SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_FlowAlreadyMaitained, switchTrading.Flow));
            }
            return !hasError;
        }
        #endregion

    }
}
