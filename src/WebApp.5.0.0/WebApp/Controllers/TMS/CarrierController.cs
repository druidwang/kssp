
namespace com.Sconit.Web.Controllers.TMS
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using Telerik.Web.Mvc;
    using System.Web.Routing;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Entity.TMS;
    using com.Sconit.Utility;

    public class CarrierController : WebAppBaseController
    {
        #region 承运商
        /// <summary>
        /// select  the  count of  Party
        /// </summary>
        private static string selectCountStatement = "select count(*) from Carrier as c";

        private static string selectCountRegionStatement = "select count(*) from Region as r where r.Code = ?";

        private static string selectCountSupplierStatement = "select count(*) from Supplier as s where s.Code = ?";

        private static string selectCountCustomerStatement = "select count(*) from Customer as c where c.Code = ?";

        /// <summary>
        /// select  whole Party
        /// </summary>
        private static string selectStatement = "select c from Carrier as c";

        private static string selectCountCarrierStatement = "select count(*) from Carrier as s where s.Code = ?";

        public IPartyMgr partyMgr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult List(GridCommand command, PartySearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult _AjaxList(GridCommand command, PartySearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Carrier>(searchStatementModel, command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult New(Carrier Carrier)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                if (base.genericMgr.FindAll<long>(selectCountCarrierStatement, new object[] { Carrier.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.MD.Party.Party_Supplier_Errors_Existing_Code, Carrier.Code);
                }
                else if (base.genericMgr.FindAll<long>(selectCountSupplierStatement, new object[] { Carrier.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.MD.Party.Party_Errors_Exists_Code_UserdBySupplier, Carrier.Code);
                }
                else  if (base.genericMgr.FindAll<long>(selectCountRegionStatement, new object[] { Carrier.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.MD.Party.Party_Errors_Exists_Code_UserdByRegion, Carrier.Code);
                }
                else if (base.genericMgr.FindAll<long>(selectCountCustomerStatement, new object[] { Carrier.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.MD.Party.Party_Errors_Exists_Code_UserdByCustomer, Carrier.Code);
                }
                else
                {
                    partyMgr.Create(Carrier);
                    SaveSuccessMessage(Resources.MD.Party.Party_Carrier_Added);
                    return RedirectToAction("Edit/"+Carrier.Code);
                }
            }
            return View(Carrier);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult Edit(string Id)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }

            return View("Edit", "", Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _Edit(string Id)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.PartyCode = Id;
                Carrier Carrier = base.genericMgr.FindById<Carrier>(Id);
                return PartialView(Carrier);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _Edit(Carrier Carrier)
        {

            if (ModelState.IsValid)
            {
                base.genericMgr.Update(Carrier);
                SaveSuccessMessage(Resources.MD.Party.Party_Carrier_Updated);
            }

            TempData["TabIndex"] = 0;
            return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_Edit" }, 
                                                       { "controller", "Carrier" } ,
                                                       { "Id", Carrier.Code }
                                                   });
        }

        [SconitAuthorize(Permissions = "Url_Carrier_Delete")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<Carrier>(id);
                SaveSuccessMessage(Resources.MD.Party.Party_Carrier_Deleted);
                return RedirectToAction("List");
            }
        }
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PartySearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);
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

        #region 开票地址
        private static string selectBillAddressCountStatement = "select count(*) from PartyAddress as  pa  ";
        private static string selectBillAddressStatement = "select  pa  from PartyAddress as  pa ";
        //private static string BillAddressCodeDuiplicateVerifyStatement = @"select count(*) from Address as a where a.Code = ?";

        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult _BillAddress(string Id)
        {
            ViewBag.PartyCode = Id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult _BillAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            ViewBag.PartyCode = PartyCode;
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult _AjaxBillAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            SearchStatementModel searchStatementModel = PrepareSearchAddressStatement(command, searchModel, PartyCode);
            return PartialView(GetAjaxPageData<PartyAddress>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _BillAddressNew()
        {

            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _BillAddressNew(Address address, string PartyCode, string sequence, bool isPrimary)
        {
            if (ModelState.IsValid)
            {
                PartyAddress partyAddress = new PartyAddress();
                partyAddress.Party = PartyCode;
                partyAddress.Address = address;
                partyAddress.Sequence = int.Parse(sequence);
                partyAddress.IsPrimary = isPrimary;
                partyAddress.Type = Sconit.CodeMaster.AddressType.BillAddress;
                partyMgr.AddPartyAddress(partyAddress);

                SaveSuccessMessage(Resources.MD.Address.Address_Added);
                return RedirectToAction("_BillAddressEdit/"+partyAddress.Id);
            }
            return PartialView(address);
        }

        [SconitAuthorize(Permissions = "Url_Carrier_Delete")]
        public ActionResult DeleteBillAddress(int? id, string PartyCode)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<PartyAddress>(id);
                SaveSuccessMessage(Resources.MD.Address.Address_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary { 
                                                        { "action", "_BillAddressList" }, 
                                                        { "controller", "Carrier" }, 
                                                        { "PartyCode", PartyCode } });
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _BillAddressEdit(int? Id)
        {
            if (!Id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                PartyAddress partyAddress = base.genericMgr.FindById<PartyAddress>(Id);
                return PartialView(partyAddress);
            }

        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _BillAddressEdit(PartyAddress partyAddress, string PartyCode)
        {
            if (ModelState.IsValid)
            {
                ViewBag.PartyCode = PartyCode;
                partyAddress.Party = PartyCode;
                partyMgr.UpdatePartyAddress(partyAddress);

                SaveSuccessMessage(Resources.MD.Address.Address_Updated);
            }

            TempData["TabIndex"] = 1;
            return PartialView(partyAddress);
        }

        private SearchStatementModel PrepareSearchAddressStatement(GridCommand command, PartyAddressSearchModel searchModel, string partyCode)
        {
            string whereStatement = "  where pa.Party='" + partyCode + "' and  pa.Type='1' ";
            IList<object> param = new List<object>();
            //HqlStatementHelper.AddLikeStatement("Party", searchModel.Party, HqlStatementHelper.LikeMatchMode.Anywhere, "pa", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Address.Code", searchModel.AddressCode, HqlStatementHelper.LikeMatchMode.Start, "pa", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Address.AddressContent", searchModel.AddressContent, HqlStatementHelper.LikeMatchMode.Start, "pa", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectBillAddressCountStatement;
            searchStatementModel.SelectStatement = selectBillAddressStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        #endregion

        #region 货运地址
        private static string selectShipAddressCountStatement = "select count(*) from PartyAddress as  pa  ";
        private static string selectShipAddressStatement = "select  pa  from PartyAddress as  pa ";
       // private static string ShipAddressCodeDuiplicateVerifyStatement = @"select count(*) from Address as a where a.Code = ?";

        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult _ShipAddress(string Id)
        {
            ViewBag.PartyCode = Id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult _ShipAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            ViewBag.PartyCode = PartyCode;

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Carrier_View")]
        public ActionResult _AjaxShipAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            SearchStatementModel searchStatementModel = PrepareSearchShipAddressStatement(command, searchModel, PartyCode);
            return PartialView(GetAjaxPageData<PartyAddress>(searchStatementModel, command));
        }



        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _ShipAddressNew()
        {
            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _ShipAddressNew(Address address, string PartyCode, string sequence, bool isPrimary)
        {
            if (ModelState.IsValid)
            {
                PartyAddress partyAddress = new PartyAddress();
                partyAddress.Party = PartyCode;
                partyAddress.Address = address;
                partyAddress.Sequence = int.Parse(sequence);
                partyAddress.IsPrimary = isPrimary;
                partyMgr.AddPartyAddress(partyAddress);

                SaveSuccessMessage(Resources.MD.Address.Address_Added);
                return RedirectToAction("_ShipAddressEdit/"+partyAddress.Id);
            }
            return PartialView(address);
        }

        [SconitAuthorize(Permissions = "Url_Carrier_Delete")]
        public ActionResult DeleteShipAddress(int? id, string PartyCode)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<PartyAddress>(id);
                SaveSuccessMessage(Resources.MD.Address.Address_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary { 
                                                        { "action", "_ShipAddressList" }, 
                                                        { "controller", "Carrier" }, 
                                                        { "PartyCode", PartyCode } });
            }
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _ShipAddressEdit(int? Id)
        {
            if (!Id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                PartyAddress address = base.genericMgr.FindById<PartyAddress>(Id);
                return PartialView(address);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Carrier_Edit")]
        public ActionResult _ShipAddressEdit(PartyAddress partyAddress, string PartyCode)
        {
            if (ModelState.IsValid)
            {
                partyAddress.Party = PartyCode;
                partyMgr.UpdatePartyAddress(partyAddress);

                SaveSuccessMessage(Resources.MD.Address.Address_Updated);
            }
            TempData["TabIndex"] = 1;
            return PartialView(partyAddress);
        }

        private SearchStatementModel PrepareSearchShipAddressStatement(GridCommand command, PartyAddressSearchModel searchModel, string partyCode)
        {
            string whereStatement = "  where pa.Party='" + partyCode + "' and  pa.Address.Type='0' ";
            IList<object> param = new List<object>();
            //HqlStatementHelper.AddLikeStatement("Party", searchModel.Party, HqlStatementHelper.LikeMatchMode.Anywhere, "pa", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Address.Code", searchModel.AddressCode, HqlStatementHelper.LikeMatchMode.Start, "pa", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Address.AddressContent", searchModel.AddressContent, HqlStatementHelper.LikeMatchMode.Start, "pa", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectShipAddressCountStatement;
            searchStatementModel.SelectStatement = selectShipAddressStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Supplier_Edit")]
        public JsonResult _GetAddress(string addressCode)
        {
            Address address = new Address();
            if (!string.IsNullOrEmpty(addressCode))
            {
                address = base.genericMgr.FindById<Address>(addressCode);
            }
            return this.Json(address);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Supplier_Edit")]
        public ActionResult _GetShipAddress(string addressCode)
        {
            Address address = new Address();
            if (!string.IsNullOrEmpty(addressCode))
            {
                address = base.genericMgr.FindById<Address>(addressCode);
            }
            return PartialView("_ShipAddressNew", address);
        }
        #endregion
    }
}
