
namespace com.Sconit.Web.Controllers.MD
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
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using System.Web.Routing;
    using com.Sconit.Web.Models;


    public class RegionController : WebAppBaseController
    {
        #region 区域
        /// <summary>
        /// select  the  count of  Party
        /// </summary>
        private static string selectCountStatement = "select count(*) from Region as r";

        /// <summary>
        /// select  whole Party
        /// </summary>
        private static string selectStatement = "select r from Region as r";

        /// <summary>
        /// 
        /// </summary>
        private static string CodeDuiplicateVerifyStatement = @"select count(*) from Region as r where r.Code = ?";
        private static string CodeDuiplicateVerifyStatement1 = @"select count(*) from PartyAddress as i where i.Party = ? and i.Address.Code = ? and i.Type= ?";
        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        public IPartyMgr partyMgr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_Region_View")]
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
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult List(GridCommand command, PartySearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
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
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _AjaxList(GridCommand command, PartySearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Region>(searchStatementModel, command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_Region_Edit")]
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
        [SconitAuthorize(Permissions = "Url_Region_Edit")]
        public ActionResult New(Region region)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                if (this.genericMgr.FindAll<long>(CodeDuiplicateVerifyStatement, new object[] { region.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.MD.Region.Region_Errors_Existing_Code, region.Code);
                }
                else
                {
                    partyMgr.Create(region);
                    SaveSuccessMessage(Resources.MD.Region.Region_Added);
                    return RedirectToAction("Edit/" + region.Code);
                }
            }
            return View(region);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Region_View")]
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
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _Edit(string Id)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.PartyCode = Id;
                Party party = genericMgr.FindById<Party>(Id);
                return PartialView(party);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _Edit(Region region)
        {

            if (ModelState.IsValid)
            {
                genericMgr.Update(region);
                SaveSuccessMessage(Resources.MD.Region.Region_Updated);
            }
            ViewBag.PartyCode = region.Code;
            return PartialView(region);
        }

        [SconitAuthorize(Permissions = "Url_Region_Delete")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<Party>(id);
                SaveSuccessMessage(Resources.MD.Region.Region_Deleted);
                return RedirectToAction("List");
            }
        }
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PartySearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("Plant", searchModel.Plant, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Workshop", searchModel.Workshop, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
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

        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _BillAddress(string Id)
        {
            ViewBag.PartyCode = Id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _BillAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            ViewBag.PartyCode = PartyCode;
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchAddressStatement(command, (PartyAddressSearchModel)searchCacheModel.SearchObject, PartyCode);
            return PartialView(GetPageData<PartyAddress>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _AjaxBillAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            SearchStatementModel searchStatementModel = PrepareSearchAddressStatement(command, searchModel, PartyCode);
            return PartialView(GetAjaxPageData<PartyAddress>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Region_Edit")]
        public ActionResult _BillAddressNew()
        {

            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Region_Edit")]
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
                if (this.genericMgr.FindAll<long>(CodeDuiplicateVerifyStatement1, new object[] { partyAddress.Party, partyAddress.Address.Code, partyAddress.Type })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.MD.Region.Region_Errors_Existing_Code, partyAddress.Address.AddressContent);
                }
                else
                {
                    partyMgr.AddPartyAddress(partyAddress);

                    SaveSuccessMessage(Resources.MD.Address.Address_Added);
                    return RedirectToAction("_BillAddressEdit/" + partyAddress.Id);
                }
            }
            return PartialView(address);
        }

        [SconitAuthorize(Permissions = "Url_Region_Delete")]
        public ActionResult DeleteBillAddress(int? id, string PartyCode)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<PartyAddress>(id);
                SaveSuccessMessage(Resources.MD.Address.Address_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary { 
                                                        { "action", "_BillAddressList" }, 
                                                        { "controller", "Region" }, 
                                                        { "PartyCode", PartyCode } });
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _BillAddressEdit(int? Id)
        {
            if (!Id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                PartyAddress partyAddress = genericMgr.FindById<PartyAddress>(Id);
                return PartialView(partyAddress);
            }

        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Region_Edit")]
        public ActionResult _BillAddressEdit(PartyAddress partyAddress, string PartyCode)
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

        //[HttpPost]
        //[SconitAuthorize(Permissions = "Url_Region_Edit")]
        //public ActionResult _GetBillAddress(string addressCode)
        //{
        //    Address address = new Address();
        //    if (!string.IsNullOrEmpty(addressCode))
        //    {
        //        address = genericMgr.FindById<Address>(addressCode);
        //    }
        //    return PartialView("_BillAddressNew", address);
        //}

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Region_Edit")]
        public JsonResult _GetAddress(string addressCode)
        {
            Address address = new Address();
            if (!string.IsNullOrEmpty(addressCode))
            {
                address = genericMgr.FindById<Address>(addressCode);
            }
            // return PartialView("_BillAddressNew", address);
            return this.Json(address);
        }
        #endregion

        #region 货运地址
        private static string selectShipAddressCountStatement = "select count(*) from PartyAddress as  pa  ";
        private static string selectShipAddressStatement = "select  pa  from PartyAddress as  pa ";
        //private static string ShipAddressCodeDuiplicateVerifyStatement = @"select count(*) from Address as a where a.Code = ?";

        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _ShipAddress(string Id)
        {
            ViewBag.PartyCode = Id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _ShipAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            ViewBag.PartyCode = PartyCode;
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchShipAddressStatement(command, (PartyAddressSearchModel)searchCacheModel.SearchObject, PartyCode);
            return PartialView(GetPageData<PartyAddress>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _AjaxShipAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            SearchStatementModel searchStatementModel = PrepareSearchShipAddressStatement(command, searchModel, PartyCode);
            return PartialView(GetAjaxPageData<PartyAddress>(searchStatementModel, command));
        }



        [SconitAuthorize(Permissions = "Url_Region_Edit")]
        public ActionResult _ShipAddressNew()
        {
            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Region_Edit")]
        public ActionResult _ShipAddressNew(Address address, string PartyCode, string sequence, bool isPrimary)
        {
            if (ModelState.IsValid)
            {
                PartyAddress partyAddress = new PartyAddress();
                partyAddress.Party = PartyCode;
                partyAddress.Address = address;
                partyAddress.Sequence = int.Parse(sequence);
                partyAddress.IsPrimary = isPrimary;
                partyAddress.Type = Sconit.CodeMaster.AddressType.ShipAddress;
                if (this.genericMgr.FindAll<long>(CodeDuiplicateVerifyStatement1, new object[] { partyAddress.Party, partyAddress.Address.Code, partyAddress.Type })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.MD.Region.Region_Errors_Existing_Code, partyAddress.Address.AddressContent);
                }
                else
                {
                    partyMgr.AddPartyAddress(partyAddress);

                    TempData[""] = null;
                    SaveSuccessMessage(Resources.MD.Address.Address_Added);
                    return RedirectToAction("_ShipAddressEdit/" + partyAddress.Id);
                }
            }
            return PartialView(address);
        }

        [SconitAuthorize(Permissions = "Url_Region_Delete")]
        public ActionResult DeleteShipAddress(int? id, string PartyCode)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<PartyAddress>(id);
                SaveSuccessMessage(Resources.MD.Address.Address_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary { 
                                                        { "action", "_ShipAddressList" }, 
                                                        { "controller", "Region" }, 
                                                        { "PartyCode", PartyCode } });
            }
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult _ShipAddressEdit(int? Id)
        {
            if (!Id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                PartyAddress address = genericMgr.FindById<PartyAddress>(Id);
                return PartialView(address);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Region_Edit")]
        public ActionResult _ShipAddressEdit(PartyAddress partyAddress, string PartyCode)
        {
            if (ModelState.IsValid)
            {
                partyAddress.Party = PartyCode;
                partyMgr.UpdatePartyAddress(partyAddress);

                SaveSuccessMessage(Resources.MD.Address.Address_Updated);
            }
            return PartialView(partyAddress);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Region_Edit")]
        public ActionResult _GetShipAddress(string addressCode)
        {
            Address address = new Address();
            if (!string.IsNullOrEmpty(addressCode))
            {
                address = genericMgr.FindById<Address>(addressCode);
            }
            return PartialView("_ShipAddressNew", address);
        }

        private SearchStatementModel PrepareSearchShipAddressStatement(GridCommand command, PartyAddressSearchModel searchModel, string partyCode)
        {
            string whereStatement = "  where pa.Party='" + partyCode + "' and  pa.Type='0' ";
            IList<object> param = new List<object>();
            //HqlStatementHelper.AddLikeStatement("Party", searchModel.Party, HqlStatementHelper.LikeMatchMode.Start, "pa", ref whereStatement, param);
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
        #endregion
    }
}
