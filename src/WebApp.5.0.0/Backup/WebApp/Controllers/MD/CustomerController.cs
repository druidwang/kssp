
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


    public class CustomerController : WebAppBaseController
    {
        #region 客户
        /// <summary>
        /// select  the  count of  Party
        /// </summary>
        private static string selectCountStatement = "select count(*) from Customer as c";

        /// <summary>
        /// select  whole Party
        /// </summary>
        private static string selectStatement = "select c from Customer as c";

        /// <summary>
        /// 
        /// </summary>
        private static string CodeDuiplicateVerifyStatement = @"select count(*) from Customer as c where c.Code = ?";

        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        public IPartyMgr partyMgr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_Customer_View")]
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
        [SconitAuthorize(Permissions = "Url_Customer_View")]
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
        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult _AjaxList(GridCommand command, PartySearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Customer>(searchStatementModel, command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
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
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
        public ActionResult New(Customer Customer)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                if (this.genericMgr.FindAll<long>(CodeDuiplicateVerifyStatement, new object[] { Customer.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.MD.Party.Party_Customer_Errors_Existing_Code, Customer.Code);
                }
                else
                {
                    partyMgr.Create(Customer);
                    SaveSuccessMessage(Resources.MD.Party.Party_Customer_Added);
                    return RedirectToAction("Edit/"+Customer.Code);
                }
            }
            return View(Customer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Customer_View")]
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
        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult _Edit(string Id)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.PartyCode = Id;
                Customer Customer = genericMgr.FindById<Customer>(Id);
                return PartialView(Customer);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
        public ActionResult _Edit(Customer Customer)
        {

            if (ModelState.IsValid)
            {
                genericMgr.Update(Customer);
                SaveSuccessMessage(Resources.MD.Party.Party_Customer_Updated);
            }

            TempData["TabIndex"] = 0;
            return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_Edit" }, 
                                                       { "controller", "Customer" } ,
                                                       { "Id", Customer.Code }
                                                   });
        }

        [SconitAuthorize(Permissions = "Url_Customer_Delete")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<Customer>(id);
                SaveSuccessMessage(Resources.MD.Party.Party_Customer_Deleted);
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

        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult _BillAddress(string Id)
        {
            ViewBag.PartyCode = Id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult _BillAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            ViewBag.PartyCode = PartyCode;
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult _AjaxBillAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            SearchStatementModel searchStatementModel = PrepareSearchAddressStatement(command, searchModel, PartyCode);
            return PartialView(GetAjaxPageData<PartyAddress>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
        public ActionResult _BillAddressNew()
        {

            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
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

        [SconitAuthorize(Permissions = "Url_Customer_Delete")]
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
                                                        { "controller", "Customer" }, 
                                                        { "PartyCode", PartyCode } });
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Customer_View")]
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
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
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

        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult _ShipAddress(string Id)
        {
            ViewBag.PartyCode = Id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult _ShipAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            ViewBag.PartyCode = PartyCode;

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult _AjaxShipAddressList(GridCommand command, PartyAddressSearchModel searchModel, string PartyCode)
        {
            SearchStatementModel searchStatementModel = PrepareSearchShipAddressStatement(command, searchModel, PartyCode);
            return PartialView(GetAjaxPageData<PartyAddress>(searchStatementModel, command));
        }



        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
        public ActionResult _ShipAddressNew()
        {
            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
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
                partyMgr.AddPartyAddress(partyAddress);

                SaveSuccessMessage(Resources.MD.Address.Address_Added);
                return RedirectToAction("_ShipAddressEdit/"+partyAddress.Id);
            }
            return PartialView(address);
        }

        [SconitAuthorize(Permissions = "Url_Customer_Delete")]
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
                                                        { "controller", "Customer" }, 
                                                        { "PartyCode", PartyCode } });
            }
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Customer_View")]
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
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
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
            string whereStatement = "  where pa.Party='" + partyCode + "' and  pa.Type='0' ";
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
                address = genericMgr.FindById<Address>(addressCode);
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
                address = genericMgr.FindById<Address>(addressCode);
            }
            return PartialView("_ShipAddressNew", address);
        }
        #endregion
        #region Export Customer by key fileds
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Customer_View")]
        public ActionResult Export(PartySearchModel searchModel)
        {
            try
            {
                string sql = @" Select a.Code As 客户代码,c.Name As 客户名称,b.Address As 客户地址,PostCode As 邮编,TelPhone As 固定电话,MobilePhone
                            As 手机号码,Fax As 传真,Email As 邮箱,ContactPsnNm 联系人,Case when IsActive=1 then '有效' else '无效' End
                            As 是否有效  from MD_Customer a,MD_Address b,MD_Party c 
                            where a.Code =b.Code and b.Code =c.Code ";
                if (!string.IsNullOrWhiteSpace(searchModel.Code))
                {
                    sql = sql + "and a.Code like '" + searchModel.Code + "%'";
                }
                if (!string.IsNullOrWhiteSpace(searchModel.Name))
                {
                    sql = sql + "and c.Name like '" + searchModel.Name +"%'";
                }
                var table = GetTableHtmlBySql(sql, null);
                return new DownloadFileActionResult(table, "Customer.xls");
            }
            catch (System.Exception e)
            {
                return Json(null);
            }
        }
        #endregion
    }
}
