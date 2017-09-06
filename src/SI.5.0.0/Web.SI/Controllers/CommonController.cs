
namespace com.Sconit.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.PRD;
    using com.Sconit.Entity.SCM;
    using com.Sconit.Entity.SYS;
    //using com.Sconit.Entity.SCM;
    using com.Sconit.Web.Models;
    using Telerik.Web.Mvc.UI;
    using System;
    using com.Sconit.Entity.BIL;
    using com.Sconit.Service;
    using NHibernate.Criterion;
    using com.Sconit.Entity.ORD;
    using NHibernate.Type;
    using NHibernate;
    using com.Sconit.Entity.ISS;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity.INP;
    using com.Sconit.Entity;
    public class CommonController : WebAppBaseController
    {

           private static int firstRow = 0;
        private static int maxRow = 20;
        private static string selectEqItemStatement = "from Item as i where i.Code = ? and IsActive = ?";

        private static string selectLikeItemStatement = "from Item as i where i.Code like ? and IsActive = ?";

        //private static string selectEqUserStatement = "from User as u where u.Code = ? and u.IsActive = ?";

        //private static string selectLikeUserStatement = "from User as u where u.Code like ? and u.IsActive = ?";

        //private static string selectEqItemPackageStatement = "from ItemPackage as i where  convert(VARCHAR,i.UnitCount) = ?";

        //private static string selectLikeItemPackageStatement = "from ItemPackage as i where i.Item=? and convert(VARCHAR,i.UnitCount) like ? ";

        private static string selectEqSupplierStatement = "from Supplier as s where s.Code = ? and IsActive = ?";

        private static string selectLikeSupplierStatement = "from Supplier as s where s.Code like ? and IsActive = ?";

        //private static string selectEqCustomerStatement = "from Customer as c where c.Code = ? and IsActive = ?";

        //private static string selectLikeCustomerStatement = "from Customer as c where c.Code like ? and IsActive = ?";

        //private static string selectEqRegionStatement = "from Region as r where r.Code = ? and IsActive = ?";

        //private static string selectLikeRegionStatement = "from Region as r where r.Code like ? and IsActive = ?";

        //private static string selectLikeRoutingStatement = "from RoutingMaster as r where r.Code like ? and IsActive = ?";

        //private static string selectEqBomStatement = "from BomMaster as b where b.Code = ? and IsActive = ?";

        //private static string selectLikeBomStatement = "from BomMaster as b where b.Code like ? and IsActive = ?";

        //private static string selectEqIssueAddressStatement = "from IssueAddress as ia where  ia.Code = ? or ia.Description = ? order by Sequence";

        //private static string selectLikeIssueAddressStatement = "from IssueAddress as ia where  ia.Code like ? or ia.Description like ?  order by Sequence";

        //private static string selectRegionStatement = "from Region as r where r.IsActive = ?";

        //private static string selectEqFlowStatement = "from FlowMaster as f where f.Code = ?";

        //private static string selectEqOrderStatement = "from OrderMaster as o where o.Code = ?";

        //private static string selectEqTraceCodeStatement = "from OrderDetailTrace as t where t.Code = ?";

        //private static string supplierAddress = @"select a from PartyAddress p join p.Address as a where  p.Party=?  and a.Type=?";

        //private static string shipToAddress = @"select a from PartyAddress p join p.Address as a where  p.Party=? and a.Type=?";

        //private static string selectProductLineFacilityStatement = "from ProductLineFacility as p where p.ProductLine = ?";

        //private static string selectLikeLocationStatement = "from Location as l where l.Code like ? and l.IsActive = ?";

        //public IGenericMgr genericMgr { get; set; }

        //#region public methods
        //public ActionResult _SiteMapPath(string menuContent)
        //{
        //    IList<com.Sconit.Entity.SYS.Menu> allMenu = systemMgr.GetAllMenu();
        //    IList<MenuModel> allMenuModel = Mapper.Map<IList<com.Sconit.Entity.SYS.Menu>, IList<MenuModel>>(allMenu);

        //    List<MenuModel> menuList = allMenuModel.Where(m => m.Code == menuContent).ToList();
        //    this.NestGetParentMenu(menuList, menuList, allMenuModel);

        //    return PartialView(menuList);
        //}

        //#region CodeMaster
        //public ActionResult _CodeMasterDropDownList(com.Sconit.CodeMaster.CodeMaster code, string controlName, string controlId, string selectedValue, string ajaxActionName,
        //    //string[] parentCascadeControlNames, string[] cascadingControlNames,
        //                                            bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable, bool? isConsignment, bool? isShowNA)
        //{

        //    IList<CodeDetail> codeDetailList = systemMgr.GetCodeDetails(code, includeBlankOption, blankOptionDescription, blankOptionValue);
        //    //采购路线中的结算方式 不显示寄售结算  
        //    if (isConsignment != null)
        //    {
        //        if (code == com.Sconit.CodeMaster.CodeMaster.BillTerm)
        //        {
        //            if ((bool)isConsignment)
        //            {
        //                codeDetailList = this.queryMgr.FindAll<CodeDetail>("from CodeDetail c where c.Description in ('" + "CodeDetail_BillTerm_BAC" + "','" + "CodeDetail_BillTerm_NA" + "','" + "CodeDetail_BillTerm_BAR" + "') order by c.Sequence");
        //            }
        //            else
        //            {
        //                IList<CodeDetail> codeDetail = this.queryMgr.FindAll<CodeDetail>("from CodeDetail c where c.Description = ?", "CodeDetail_BillTerm_BAC");
        //                if (codeDetail.Count > 0)
        //                {
        //                    codeDetailList.Remove(codeDetail[0]);
        //                }
        //            }
        //            if (isShowNA != null)
        //            {
        //                if (!(bool)isShowNA)
        //                {
        //                    IList<CodeDetail> codeDetail = this.queryMgr.FindAll<CodeDetail>("from CodeDetail c where c.Description = ?", "CodeDetail_BillTerm_NA");
        //                    if (codeDetail.Count > 0)
        //                    {
        //                        codeDetailList.Remove(codeDetail[0]);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    //收货和发货的OrderType 不显示销售和生产
        //    if (code == com.Sconit.CodeMaster.CodeMaster.OrderType)
        //    {
        //        if (controlName == "GoodsReceiptOrderType" || controlName == "IpOrderType")
        //        {

        //            IList<CodeDetail> codeDetail = this.queryMgr.FindAll<CodeDetail>("from CodeDetail c where c.Description = ? or c.Description=?",
        //                new object[] { "CodeDetail_OrderType_Distribution", "CodeDetail_OrderType_Production" });
        //            if (codeDetail.Count > 0)
        //            {
        //                for (int i = 0; i < codeDetail.Count; i++)
        //                {
        //                    codeDetailList.Remove(codeDetail[i]);
        //                }
        //            }

        //        }
        //    }
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    ViewBag.AjaxActionName = ajaxActionName;
        //    ViewBag.Enable = enable;
        //   // codeDetailList.Add(new CodeDetail());
        //    //ViewBag.CascadingControlNames = cascadingControlNames;
        //    //ViewBag.ParentCascadeControlNames = parentCascadeControlNames;
        //    return PartialView(base.Transfer2DropDownList(code, codeDetailList, selectedValue));
        //}

        ////public ActionResult _CodeMasterAjaxDropDownList(com.Sconit.CodeMaster.CodeMaster code, string controlName, string controlId, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        ////{
        ////    ViewBag.Code = code;
        ////    ViewBag.ControlName = controlName;
        ////    ViewBag.IncludeBlankOption = includeBlankOption;
        ////    ViewBag.BlankOptionDescription = blankOptionDescription;
        ////    ViewBag.BlankOptionValue = blankOptionValue;
        ////    return PartialView();
        ////}

        //public ActionResult _AjaxLoadingCodeMaster(com.Sconit.CodeMaster.CodeMaster code, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        //{
        //    ViewBag.Enable = enable;
        //    IList<CodeDetail> codeDetailList = systemMgr.GetCodeDetails(code, includeBlankOption, blankOptionDescription, blankOptionValue);
        //    return new JsonResult { Data = base.Transfer2DropDownList(code, codeDetailList) };
        //}
        //#endregion

        //#region Region
        //public ActionResult _RegionDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? coupled)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Coupled = coupled;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<Region> regionList = queryMgr.FindAll<Region>(selectRegionStatement, true);
        //    if (regionList == null)
        //    {
        //        regionList = new List<Region>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Region blankRegion = new Region();
        //        blankRegion.Code = blankOptionValue;
        //        blankRegion.Name = blankOptionDescription;

        //        regionList.Insert(0, blankRegion);
        //    }
        //    return PartialView(new SelectList(regionList, "Code", "Name", selectedValue));
        //}


        //#endregion

        //#region User
        //public ActionResult _UserComboBox(string controlName, string controlId, string selectedValue, bool? enable)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Enable = enable;

        //    IList<User> userList = new List<User>();
        //    if (selectedValue != null && selectedValue.Trim() != string.Empty)
        //    {
        //        userList = queryMgr.FindAll<User>(selectEqUserStatement, new object[] { selectedValue, true });
        //    }

        //    //ViewBag.UserFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.UserFilterMode);
        //    //ViewBag.UserFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.UserFilterMinimumChars));
        //    return PartialView(new SelectList(userList, "Code", "CodeDescription", selectedValue));
        //}

        //public ActionResult _UserAjaxLoading(string text)
        //{
        //    //AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.UserFilterMode), true);
        //    IList<User> userList = new List<User>();

        //    //if (fileterMode == AutoCompleteFilterMode.Contains)
        //    //{
        //    //    userList = queryMgr.FindAll<User>(selectLikeUserStatement, new object[] { "%" + text + "%", true });
        //    //}
        //    //else
        //    //{
        //        userList = queryMgr.FindAll<User>(selectLikeUserStatement, new object[] { text + "%", true });
        //    //}

        //    return new JsonResult { Data = new SelectList(userList, "Code", "CodeDescription") };
        //}
        //#endregion

        //#region Item
        public ActionResult _ItemComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? coupled)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.Coupled = coupled;
            IList<Item> itemList = new List<Item>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                itemList = queryMgr.FindAll<Item>(selectEqItemStatement, new object[] { selectedValue, true });
            }

            ViewBag.ItemFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode);
            ViewBag.ItemFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMinimumChars));
            return PartialView(new SelectList(itemList, "Code", "CodeDescription", selectedValue));
        }






        public ActionResult _ItemAjaxLoading(string text)
        {
            AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
            IList<Item> itemList = new List<Item>();

            if (fileterMode == AutoCompleteFilterMode.Contains)
            {
                itemList = queryMgr.FindAll<Item>(selectLikeItemStatement, new object[] { "%" + text + "%", true }, firstRow, maxRow);
            }
            else
            {
                itemList = queryMgr.FindAll<Item>(selectLikeItemStatement, new object[] { text + "%", true }, firstRow, maxRow);
            }

            return new JsonResult { Data = new SelectList(itemList, "Code", "CodeDescription") };
        }
        //#endregion

        //#region Bom
        //public ActionResult _BomComboBox(string controlName, string controlId, string selectedValue)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<BomMaster> bomList = new List<BomMaster>();
        //    if (selectedValue != null && selectedValue.Trim() != string.Empty)
        //    {
        //        bomList = queryMgr.FindAll<BomMaster>(selectEqBomStatement, new object[] { selectedValue, true });
        //    }

        //    ViewBag.ItemFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode);
        //    ViewBag.ItemFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMinimumChars));

        //    return PartialView(new SelectList(bomList, "Code", "CodeDescription", selectedValue));
        //}

        //public ActionResult _BomAjaxLoading(string text)
        //{
        //    AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);

        //    IList<BomMaster> bomList = new List<BomMaster>();
        //    if (fileterMode == AutoCompleteFilterMode.Contains)
        //    {
        //        bomList = queryMgr.FindAll<BomMaster>(selectLikeBomStatement, new object[] { "%" + text + "%", true });
        //    }
        //    else
        //    {
        //        bomList = queryMgr.FindAll<BomMaster>(selectLikeBomStatement, new object[] { text + "%", true });
        //    }

        //    return new JsonResult { Data = new SelectList(bomList, "Code", "CodeDescription") };
        //}
        //#endregion

        //#region ItemCategory
        //public ActionResult _ItemCategoryDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    ViewBag.Enable = enable;
        //    IList<ItemCategory> itemCategoryList = queryMgr.FindAll<ItemCategory>("from ItemCategory as i");
        //    if (itemCategoryList == null)
        //    {
        //        itemCategoryList = new List<ItemCategory>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        ItemCategory blankitemCategory = new ItemCategory();
        //        blankitemCategory.Code = blankOptionValue;
        //        blankitemCategory.Description = blankOptionDescription;

        //        itemCategoryList.Insert(0, blankitemCategory);
        //    }
        //    return PartialView(new SelectList(itemCategoryList, "Code", "Description", selectedValue));
        //}
        //#endregion

        //#region Location
        //public ActionResult _LocationDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.Enable = enable;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<Location> locationList = queryMgr.FindAll<Location>("from Location as l where l.IsActive=?", true);
        //    if (locationList == null)
        //    {
        //        locationList = new List<Location>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Location blankLocation = new Location();
        //        blankLocation.Code = blankOptionValue;
        //        blankLocation.Name = blankOptionDescription;

        //        locationList.Insert(0, blankLocation);
        //    }
        //    return PartialView(new SelectList(locationList, "Code", "Name", selectedValue));
        //}

        //public ActionResult _LocationComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.Enable = enable;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.SelectedValue = selectedValue;
        //    IList<Location> locationList = queryMgr.FindAll<Location>("from Location as l where l.IsActive=?", true);
        //    if (locationList == null)
        //    {
        //        locationList = new List<Location>();
        //    }

        //    return PartialView(new SelectList(locationList, "Code", "CodeName", selectedValue));
        //}

        //public ActionResult _LocationAjaxLoading(string text)
        //{
        //    AutoCompleteFilterMode fileterMode = AutoCompleteFilterMode.StartsWith;
        //    IList<Location> locationList = new List<Location>();

        //    if (fileterMode == AutoCompleteFilterMode.Contains)
        //    {
        //        locationList = queryMgr.FindAll<Location>(selectLikeLocationStatement, new object[] { "%" + text + "%", true });
        //    }
        //    else
        //    {
        //        locationList = queryMgr.FindAll<Location>(selectLikeLocationStatement, new object[] { text + "%", true });
        //    }

        //    return new JsonResult { Data = new SelectList(locationList, "Code", "Name") };
        //}
        //#endregion

        //#region Party
        //public ActionResult _PartyDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<Party> PartyList = queryMgr.FindAll<Party>("from Party as p");
        //    if (PartyList == null)
        //    {
        //        PartyList = new List<Party>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Party blankParty = new Party();
        //        blankParty.Code = blankOptionValue;
        //        blankParty.Name = blankOptionDescription;

        //        PartyList.Insert(0, blankParty);
        //    }
        //    return PartialView(new SelectList(PartyList, "Code", "Name", selectedValue));
        //}
        //#endregion

        //#region Tax
        //public ActionResult _TaxDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<Tax> taxList = queryMgr.FindAll<Tax>("from Tax as t");
        //    if (taxList == null)
        //    {
        //        taxList = new List<Tax>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Tax blankTax = new Tax();
        //        blankTax.Code = blankOptionValue;
        //        blankTax.Name = blankOptionDescription;

        //        taxList.Insert(0, blankTax);
        //    }
        //    return PartialView(new SelectList(taxList, "Code", "Name", selectedValue));
        //}
        //#endregion

        //#region Supplier lqy
        //public ActionResult _SupplierDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? coupled)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Coupled = coupled;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<Supplier> supplierList = queryMgr.FindAll<Supplier>("from Supplier as s where s.IsActive = ?", true);
        //    if (supplierList == null)
        //    {
        //        supplierList = new List<Supplier>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Supplier blankSupplier = new Supplier();

        //        supplierList.Insert(0, blankSupplier);
        //    }
        //    return PartialView(new SelectList(supplierList, "Code", "Name", selectedValue));
        //}
        //#endregion

        //#region Customer lqy
        //public ActionResult _CustomerDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? coupled)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Coupled = coupled;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<Customer> customerList = queryMgr.FindAll<Customer>("from Customer as c where c.IsActive = ?", true);
        //    if (customerList == null)
        //    {
        //        customerList = new List<Customer>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Customer blankCustomer = new Customer();

        //        customerList.Insert(0, blankCustomer);
        //    }
        //    return PartialView(new SelectList(customerList, "Code", "Name", selectedValue));
        //}
        //#endregion

        //#region ShipAddress lqy
        //public ActionResult _ShipAddressDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.Enable = enable;
        //    IList<Address> addressList = queryMgr.FindAll<Address>(" from Address as a where a.Type = ? ", 0);
        //    if (addressList == null)
        //    {
        //        addressList = new List<Address>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Address blankAddress = new Address();

        //        addressList.Insert(0, blankAddress);
        //    }
        //    return PartialView(new SelectList(addressList, "Code", "AddressContent", selectedValue));
        //}


        //public ActionResult _ShipAddressComboBox(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.Enable = enable;
        //    IList<Address> addressList = queryMgr.FindAll<Address>(" from Address as a where a.Type = ? ", 0);
        //    if (addressList == null)
        //    {
        //        addressList = new List<Address>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Address blankAddress = new Address();

        //        addressList.Insert(0, blankAddress);
        //    }
        //    return PartialView(new SelectList(addressList, "Code", "AddressContent", selectedValue));
        //}
        //#endregion

        //#region BillAddress lqy
        //public ActionResult _BillAddressDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.Enable = enable;
        //    IList<Address> addressList = queryMgr.FindAll<Address>(" from Address as a where a.Type = ? ", 1);
        //    if (addressList == null)
        //    {
        //        addressList = new List<Address>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Address blankAddress = new Address();

        //        addressList.Insert(0, blankAddress);
        //    }
        //    return PartialView(new SelectList(addressList, "Code", "AddressContent", selectedValue));
        //}

        //public ActionResult _BillAddressComboBox(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.Enable = enable;
        //    IList<Address> addressList = queryMgr.FindAll<Address>(" from Address as a where a.Type = ? ", 1);
        //    if (addressList == null)
        //    {
        //        addressList = new List<Address>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Address blankAddress = new Address();

        //        addressList.Insert(0, blankAddress);
        //    }
        //    return PartialView(new SelectList(addressList, "Code", "AddressContent", selectedValue));
        //}
        //#endregion

        //#region Routing lqy
        //public ActionResult _RoutingDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    IList<RoutingMaster> routingList = queryMgr.FindAll<RoutingMaster>(" from RoutingMaster as r where r.IsActive = ? ", true);
        //    if (routingList == null)
        //    {
        //        routingList = new List<RoutingMaster>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        RoutingMaster blankRoutingMaster = new RoutingMaster();
        //        routingList.Insert(0, blankRoutingMaster);
        //    }
        //    return PartialView(new SelectList(routingList, "Code", "Name", selectedValue));
        //}

        //public ActionResult _RoutingComboBox(string controlName, string selectedValue, bool? enable)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.Enable = enable;
        //    IList<RoutingMaster> routingList = queryMgr.FindAll<RoutingMaster>(" from RoutingMaster as r where r.IsActive = ? ", true);
        //    if (routingList == null)
        //    {
        //        routingList = new List<RoutingMaster>();
        //    }

        //    return PartialView(new SelectList(routingList, "Code", "Name", selectedValue));
        //}

        //public ActionResult _RoutingAjaxLoading(string text)
        //{
        //    AutoCompleteFilterMode fileterMode = AutoCompleteFilterMode.StartsWith;
        //    IList<RoutingMaster> routingList = new List<RoutingMaster>();

        //    if (fileterMode == AutoCompleteFilterMode.Contains)
        //    {
        //        routingList = queryMgr.FindAll<RoutingMaster>(selectLikeRoutingStatement, new object[] { "%" + text + "%", true });
        //    }
        //    else
        //    {
        //        routingList = queryMgr.FindAll<RoutingMaster>(selectLikeRoutingStatement, new object[] { text + "%", true });
        //    }

        //    return new JsonResult { Data = new SelectList(routingList, "Code", "Name") };
        //}
        //#endregion

        //#region PriceList lqy
        //public ActionResult _PriceListDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.Enable = enable;
        //    IList<PriceListMaster> priceList = queryMgr.FindAll<PriceListMaster>(" from PriceListMaster as p where p.IsActive = ? ", true);
        //    if (priceList == null)
        //    {
        //        priceList = new List<PriceListMaster>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        PriceListMaster blankPriceListMaster = new PriceListMaster();
        //        priceList.Insert(0, blankPriceListMaster);
        //    }
        //    return PartialView(new SelectList(priceList, "Code", "Code", selectedValue));
        //}
        //#endregion

        //#region Uom
        //public ActionResult _UomDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    ViewBag.Enable = enable;
        //    IList<Uom> uomList = queryMgr.FindAll<Uom>("from Uom as u");
        //    if (uomList == null)
        //    {
        //        uomList = new List<Uom>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Uom blankUom = new Uom();
        //        blankUom.Code = blankOptionValue;
        //        blankUom.Description = blankOptionDescription;

        //        uomList.Insert(0, blankUom);
        //    }
        //    return PartialView(new SelectList(uomList, "Code", "Code", selectedValue));
        //}
        //#endregion

        //#region IssueType
        //public ActionResult _IssueTypeDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable, bool? coupled)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    ViewBag.Enable = enable;
        //    ViewBag.Coupled = coupled;
        //    IList<IssueType> issueTypeList = queryMgr.FindAll<IssueType>("from IssueType where IsActive = ?", true);
        //    if (issueTypeList == null)
        //    {
        //        issueTypeList = new List<IssueType>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        IssueType blankIssueType = new IssueType();
        //        blankIssueType.Code = blankOptionValue;
        //        blankIssueType.Description = blankOptionDescription;

        //        issueTypeList.Insert(0, blankIssueType);
        //    }
        //    return PartialView(new SelectList(issueTypeList, "Code", "Description", selectedValue));
        //}
        //#endregion

        //#region IssueNo
        //public ActionResult _IssueNoDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    ViewBag.Enable = enable;
        //    ViewBag.IsChange = isChange;
        //    IList<IssueNo> issueNoList = queryMgr.FindAll<IssueNo>("from IssueNo as ino");
        //    if (issueNoList == null)
        //    {
        //        issueNoList = new List<IssueNo>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        IssueNo blankIssueNo = new IssueNo();
        //        blankIssueNo.Code = blankOptionValue;
        //        blankIssueNo.Description = blankOptionDescription;

        //        issueNoList.Insert(0, blankIssueNo);
        //    }
        //    return PartialView(new SelectList(issueNoList, "Code", "Description", selectedValue));
        //}
        //#endregion

        //#region IssueLevel
        //public ActionResult _IssueLevelDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    ViewBag.Enable = enable;
        //    IList<IssueLevel> issueLevelList = queryMgr.FindAll<IssueLevel>("from IssueLevel as il");
        //    if (issueLevelList == null)
        //    {
        //        issueLevelList = new List<IssueLevel>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        IssueLevel blankIssueLevel = new IssueLevel();
        //        blankIssueLevel.Code = blankOptionValue;
        //        blankIssueLevel.Description = blankOptionDescription;

        //        issueLevelList.Insert(0, blankIssueLevel);
        //    }
        //    return PartialView(new SelectList(issueLevelList, "Code", "Description", selectedValue));
        //}
        //#endregion

        //#region IssueAddress
        //public ActionResult _IssueAddressDropDownList(string code, string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    ViewBag.Enable = enable;
        //    string hql = "from IssueAddress as ia ";
        //    if (!string.IsNullOrEmpty(code))
        //    {
        //        hql += "where ia.Code !='" + code + "'";
        //    }

        //    IList<IssueAddress> issueAddressList = queryMgr.FindAll<IssueAddress>(hql);

        //    if (issueAddressList == null)
        //    {
        //        issueAddressList = new List<IssueAddress>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        IssueAddress blankIssueAddress = new IssueAddress();
        //        issueAddressList.Insert(0, blankIssueAddress);
        //    }
        //    return PartialView(new SelectList(issueAddressList, "Code", "CodeDescription", selectedValue));
        //}


        //public ActionResult _IssueAddressComboBox(string controlName, string controlId, string selectedValue)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.SelectedValue = selectedValue;
        //    IList<IssueAddress> issueAddressList = new List<IssueAddress>();

        //    //ViewBag.IssueAddressFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.IssueAddressFilterMode);
        //    //ViewBag.IssueAddressFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.IssueAddressFilterMinimumChars));

        //    if (selectedValue != null && selectedValue.Trim() != string.Empty)
        //    {
        //        issueAddressList = queryMgr.FindAll<IssueAddress>(selectEqIssueAddressStatement, new object[] { selectedValue, selectedValue });
        //        if (issueAddressList.Count == 0)
        //        {
        //            IssueAddress ia = new IssueAddress();
        //            ia.Code = selectedValue;
        //            issueAddressList.Add(ia);
        //            return PartialView(new SelectList(issueAddressList, "Code", "CodeDescription", selectedValue));

        //        }
        //    }

        //    return PartialView(new SelectList(issueAddressList, "Code", "CodeDescription", selectedValue));

        //}

        //public ActionResult _IssueAddressAjaxLoading(string text)
        //{
        //    //AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.IssueAddressFilterMode), true);

        //    IList<IssueAddress> issueAddressList = new List<IssueAddress>();
        //    //if (fileterMode == AutoCompleteFilterMode.Contains)
        //    //{
        //    //    issueAddressList = queryMgr.FindAll<IssueAddress>(selectLikeIssueAddressStatement, new object[] { "%" + text + "%", "%" + text + "%" });
        //    //}
        //    //else
        //    //{
        //        issueAddressList = queryMgr.FindAll<IssueAddress>(selectLikeIssueAddressStatement, new object[] { text + "%", text + "%" });
        //    //}

        //    return new JsonResult { Data = new SelectList(issueAddressList, "Code", "CodeDescription") };
        //}

        //#endregion

        //#region LocationArea
        //public ActionResult _LocationAreaDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, string LocationCode)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<LocationArea> locationAreaList = queryMgr.FindAll<LocationArea>("from LocationArea as i where i.IsActive = ? and i.Location=?",
        //                                                           new object[] { true, LocationCode });
        //    if (locationAreaList == null)
        //    {
        //        locationAreaList = new List<LocationArea>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        LocationArea blankItem = new LocationArea();
        //        blankItem.Code = blankOptionValue;
        //        blankItem.Name = blankOptionDescription;

        //        locationAreaList.Insert(0, blankItem);
        //    }
        //    return PartialView(new SelectList(locationAreaList, "Code", "Name", selectedValue));
        //}
        //#endregion

        //#region LocationBin
        //public ActionResult _LocationBinDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<LocationBin> locationBinList = queryMgr.FindAll<LocationBin>("from LocationBin as l where l.IsActive = ?", true);
        //    if (locationBinList == null)
        //    {
        //        locationBinList = new List<LocationBin>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        LocationBin blankItem = new LocationBin();
        //        blankItem.Code = blankOptionValue;
        //        blankItem.Name = blankOptionDescription;

        //        locationBinList.Insert(0, blankItem);
        //    }
        //    return PartialView(new SelectList(locationBinList, "Code", "Name", selectedValue));
        //}

        //public ActionResult _LocationBinComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.Enable = enable;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.SelectedValue = selectedValue;
        //    IList<LocationBin> locationBinList = queryMgr.FindAll<LocationBin>("from LocationBin as l where l.IsActive=?", true);
        //    if (locationBinList == null)
        //    {
        //        locationBinList = new List<LocationBin>();
        //    }

        //    return PartialView(new SelectList(locationBinList, "Code", "CodeName", selectedValue));
        //}

        //public ActionResult _LocationBinAjaxLoading(string text)
        //{
        //    AutoCompleteFilterMode fileterMode = AutoCompleteFilterMode.StartsWith;
        //    IList<LocationBin> locationBinList = new List<LocationBin>();

        //    if (fileterMode == AutoCompleteFilterMode.Contains)
        //    {
        //        locationBinList = queryMgr.FindAll<LocationBin>("from LocationBin as l where l.Code like ? and l.IsActive = ?", new object[] { "%" + text + "%", true });
        //    }
        //    else
        //    {
        //        locationBinList = queryMgr.FindAll<LocationBin>("from LocationBin as l where l.Code like ? and l.IsActive = ?", new object[] { text + "%", true });
        //    }

        //    return new JsonResult { Data = new SelectList(locationBinList, "Code", "Name") };
        //}
        //#endregion

        //#region ItemPackage
        //public ActionResult _ItemPackageDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, string itemCode)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<ItemPackage> itemPackageList = queryMgr.FindAll<ItemPackage>("from ItemPackage as i where  i.Item=?", itemCode);
        //    if (itemPackageList == null)
        //    {
        //        itemPackageList = new List<ItemPackage>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        ItemPackage blankItemPackage = new ItemPackage();
        //        blankItemPackage.UnitCount = Convert.ToDecimal(blankOptionValue);
        //        blankItemPackage.UnitCount = Convert.ToDecimal(blankOptionDescription);

        //        itemPackageList.Insert(0, blankItemPackage);
        //    }
        //    return PartialView(new SelectList(itemPackageList, "UnitCount", "UnitCount", selectedValue));
        //}
        //#endregion

        //#region Container
        //public ActionResult _ContainerDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    //ViewBag.SelectedValue = selectedValue;
        //    IList<Container> containerList = queryMgr.FindAll<Container>("from Container as c where  c.IsActive=?", true);
        //    if (containerList == null)
        //    {
        //        containerList = new List<Container>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Container blankContainer = new Container();
        //        blankContainer.Code = blankOptionValue;
        //        blankContainer.Description = blankOptionDescription;

        //        containerList.Insert(0, blankContainer);
        //    }
        //    return PartialView(new SelectList(containerList, "Code", "Description", selectedValue));
        //}
        //#endregion

        //#region ItemPackage ComboBox
        //public ActionResult _ItemPackageComboBox(string controlName, string controlId, string selectedValue, string item)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    IList<ItemPackage> itemPackageList = new List<ItemPackage>();
        //    if (!string.IsNullOrEmpty(item))
        //    {
        //        itemPackageList = queryMgr.FindAll<ItemPackage>("select i from ItemPackage as i where i.Item=?", item);
        //    }
        //    else if (selectedValue != null && selectedValue.Trim() != string.Empty)
        //    {
        //        itemPackageList = queryMgr.FindAll<ItemPackage>(selectEqItemPackageStatement, selectedValue);
        //    }

        //    //ViewBag.UnitCountFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.UnitCountFilterMode);
        //    //ViewBag.UnitCountFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.UnitCountMinimumChars));
        //    if (!string.IsNullOrEmpty(selectedValue))
        //    {
        //        if (itemPackageList.Count < 1)
        //        {
        //            ItemPackage itemPackage = new ItemPackage();
        //            itemPackage.Item = item;
        //            itemPackage.IsDefault = false;
        //            itemPackage.UnitCount = decimal.Parse(selectedValue);
        //            itemPackage.Description = selectedValue;
        //            itemPackageList.Add(itemPackage);
        //        }
        //    }

        //    return PartialView(new SelectList(itemPackageList, "UnitCount", "UnitCount", selectedValue));
        //}

        //public ActionResult _ItemPackageAjaxLoading(string text, string item)
        //{
        //    //AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.UnitCountFilterMode), true);
        //    IList<ItemPackage> itemPackageList = new List<ItemPackage>();

        //    if (fileterMode == AutoCompleteFilterMode.Contains)
        //    {
        //        if (item == null)
        //        {
        //            itemPackageList = queryMgr.FindAll<ItemPackage>(selectLikeItemPackageStatement, new object[] { "", "%" + text + "%" });
        //        }
        //        else
        //        {
        //            itemPackageList = queryMgr.FindAll<ItemPackage>(selectLikeItemPackageStatement, new object[] { item, "%" + text + "%" });
        //        }
        //    }
        //    else
        //    {
        //        if (item == null)
        //        {
        //            itemPackageList = queryMgr.FindAll<ItemPackage>(selectLikeItemPackageStatement, new object[] { "", text + "%" });
        //        }
        //        else
        //        {
        //            itemPackageList = queryMgr.FindAll<ItemPackage>(selectLikeItemPackageStatement, new object[] { item, text + "%" });
        //        }
        //    }

        //    return new JsonResult { Data = new SelectList(itemPackageList, "UnitCount", "UnitCount") };
        //}
        //#endregion

        //#region Flow lqy
        ///// <summary>
        ///// 获取Flow
        ///// </summary>
        ///// <param name="controlName"></param>
        ///// <param name="selectedValue"></param>
        ///// <param name="includeBlankOption"></param>
        ///// <param name="flowTypes">多个flowtype用'|'分割</param>
        ///// <returns></returns>
        //public ActionResult _FlowDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string flowTypes)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    IList<FlowMaster> flowList = new List<FlowMaster>();
        //    if (string.IsNullOrWhiteSpace(flowTypes))
        //    {
        //        flowList = queryMgr.FindAll<FlowMaster>
        //            (" from FlowMaster as f where f.IsActive = ?  ", true);
        //    }
        //    else
        //    {
        //        DetachedCriteria criteria = DetachedCriteria.For<FlowMaster>();

        //        criteria.Add(Expression.Eq("IsActive", true));
        //        criteria.Add(Expression.In("Type", flowTypes.Split('|')));

        //        flowList = queryMgr.FindAll<FlowMaster>(criteria);
        //    }
        //    if (flowList == null)
        //    {
        //        flowList = new List<FlowMaster>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        FlowMaster flowMaster = new FlowMaster();
        //        flowList.Insert(0, flowMaster);
        //    }
        //    return PartialView(new SelectList(flowList, "Code", "Description", selectedValue));
        //}

        //#region Flow
        //public ActionResult _FlowComboBox(string controlName, string controlId, string selectedValue, int? type, string onChangeFunc, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Type = type;
        //    ViewBag.OnChangeFunc = onChangeFunc;
        //    ViewBag.IsChange = isChange;

        //    IList<FlowMaster> flowList = new List<FlowMaster>();
        //    if (selectedValue != null && selectedValue.Trim() != string.Empty)
        //    {
        //        flowList = queryMgr.FindAll<FlowMaster>(selectEqFlowStatement, new object[] { selectedValue.Trim() });
        //    }

        //    ViewBag.FlowFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.FlowFilterMode);
        //    ViewBag.FlowFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.FlowFilterMinimumChars));

        //    return PartialView(new SelectList(flowList, "Code", "Description"));
        //}


        //public ActionResult _FlowAjaxLoading(string text, int? type)
        //{
        //    AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.FlowFilterMode), true);
        //    IList<FlowMaster> flowList = new List<FlowMaster>();

        //    string selectLikeFlowStatement = null;
        //    object[] paramContainList = new object[] { };
        //    object[] paramList = new object[] { };
        //    if (type == null)
        //    {
        //        selectLikeFlowStatement = "from FlowMaster as f where f.Code like ? ";
        //        paramContainList = new object[] { "%" + text + "%" };
        //        paramList = new object[] { text + "%" };

        //    }
        //    else if ((int)type == (int)com.Sconit.CodeMaster.OrderType.Procurement)
        //    {
        //        selectLikeFlowStatement = "from FlowMaster as f where f.Code like ? and f.Type in (?,?,?,?,?) ";
        //        paramContainList = new object[] { "%" + text + "%", (int)com.Sconit.CodeMaster.OrderType.CustomerGoods, (int)com.Sconit.CodeMaster.OrderType.Procurement, (int)com.Sconit.CodeMaster.OrderType.SubContract, (int)com.Sconit.CodeMaster.OrderType.Transfer, (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer };
        //        paramList = new object[] { text + "%", (int)com.Sconit.CodeMaster.OrderType.CustomerGoods, (int)com.Sconit.CodeMaster.OrderType.Procurement, (int)com.Sconit.CodeMaster.OrderType.SubContract, (int)com.Sconit.CodeMaster.OrderType.Transfer, (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer };
        //    }
        //    else if ((int)type == (int)com.Sconit.CodeMaster.OrderType.Distribution)
        //    {
        //        selectLikeFlowStatement = "from FlowMaster as f where f.Code like ? and f.Type in (?,?,?) ";
        //        paramContainList = new object[] { "%" + text + "%", (int)com.Sconit.CodeMaster.OrderType.Distribution, (int)com.Sconit.CodeMaster.OrderType.Transfer, (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer };
        //        paramList = new object[] { text + "%", (int)com.Sconit.CodeMaster.OrderType.Distribution, (int)com.Sconit.CodeMaster.OrderType.Transfer, (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer };
        //    }
        //    else if ((int)type == (int)com.Sconit.CodeMaster.OrderType.Production)
        //    {
        //        selectLikeFlowStatement = "from FlowMaster as f where f.Code like ? and f.Type in (?) ";
        //        paramContainList = new object[] { "%" + text + "%", (int)com.Sconit.CodeMaster.OrderType.Production };
        //        paramList = new object[] { text + "%", (int)com.Sconit.CodeMaster.OrderType.Production };
        //    }

        //    if (fileterMode == AutoCompleteFilterMode.Contains)
        //    {
        //        flowList = queryMgr.FindAll<FlowMaster>(selectLikeFlowStatement, paramContainList);
        //    }
        //    else
        //    {
        //        flowList = queryMgr.FindAll<FlowMaster>(selectLikeFlowStatement, paramList);
        //    }

        //    return new JsonResult { Data = new SelectList(flowList, "Code", "CodeDescription") };
        //}
        //#endregion
        //#endregion

        //#region Supplier Combox
        public ActionResult _SupplierComboBox(string controlName, string controlId, string selectedValue, bool? coupled, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Coupled = coupled;
            ViewBag.Enable = enable;
            IList<Supplier> supplierList = new List<Supplier>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                supplierList = queryMgr.FindAll<Supplier>(selectEqSupplierStatement, new object[] { selectedValue, true });
            }

            ViewBag.ItemFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode);
            ViewBag.ItemFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMinimumChars));
            return PartialView(new SelectList(supplierList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _SupplierAjaxLoading(string text)
        {
            AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
            IList<Supplier> supplierList = new List<Supplier>();

            if (fileterMode == AutoCompleteFilterMode.Contains)
            {
                supplierList = queryMgr.FindAll<Supplier>(selectLikeSupplierStatement, new object[] { "%" + text + "%", true });
            }
            else
            {
                supplierList = queryMgr.FindAll<Supplier>(selectLikeSupplierStatement, new object[] { text + "%", true });
            }

            return new JsonResult { Data = new SelectList(supplierList, "Code", "CodeDescription") };
        }
        //#endregion

        //#region Customer Combox
        //public ActionResult _CustomerComboBox(string controlName, string controlId, string selectedValue, bool? enable)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Enable = enable;
        //    IList<Customer> customerList = new List<Customer>();
        //    if (selectedValue != null && selectedValue.Trim() != string.Empty)
        //    {
        //        customerList = queryMgr.FindAll<Customer>(selectEqCustomerStatement, new object[] { selectedValue, true });
        //    }

        //    ViewBag.ItemFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode);
        //    ViewBag.ItemFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMinimumChars));
        //    return PartialView(new SelectList(customerList, "Code", "CodeDescription", selectedValue));
        //}

        //public ActionResult _CustomerAjaxLoading(string text)
        //{
        //    AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
        //    IList<Customer> customerList = new List<Customer>();

        //    if (fileterMode == AutoCompleteFilterMode.Contains)
        //    {
        //        customerList = queryMgr.FindAll<Customer>(selectLikeCustomerStatement, new object[] { "%" + text + "%", true });
        //    }
        //    else
        //    {
        //        customerList = queryMgr.FindAll<Customer>(selectLikeCustomerStatement, new object[] { text + "%", true });
        //    }

        //    return new JsonResult { Data = new SelectList(customerList, "Code", "CodeDescription") };
        //}
        //#endregion

        //#region Region Combox
        //public ActionResult _RegionComboBox(string controlName, string controlId, string selectedValue, bool? coupled)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Coupled = coupled;
        //    IList<Region> regionList = new List<Region>();
        //    if (selectedValue != null && selectedValue.Trim() != string.Empty)
        //    {
        //        regionList = queryMgr.FindAll<Region>(selectEqRegionStatement, new object[] { selectedValue, true });
        //    }

        //    //ViewBag.ItemFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode);
        //    //ViewBag.ItemFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMinimumChars));
        //    //if (itemList.Count > 0)
        //    //{
        //    return PartialView(new SelectList(regionList, "Code", "CodeDescription", selectedValue));
        //    //}
        //    //else
        //    //{
        //    //    Item item = new Item();
        //    //    item.Code = selectedValue;
        //    //    item.Description = selectedValue;
        //    //    itemList.Add(item);
        //    //    return PartialView(new SelectList(itemList, "Code", "Description"));
        //    //}
        //}

        //public ActionResult _RegionAjaxLoading(string text)
        //{
        //    AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
        //    IList<Region> regionList = new List<Region>();

        //    if (fileterMode == AutoCompleteFilterMode.Contains)
        //    {
        //        regionList = queryMgr.FindAll<Region>(selectLikeRegionStatement, new object[] { "%" + text + "%", true });
        //    }
        //    else
        //    {
        //        regionList = queryMgr.FindAll<Region>(selectLikeRegionStatement, new object[] { text + "%", true });
        //    }

        //    return new JsonResult { Data = new SelectList(regionList, "Code", "CodeDescription") };
        //}
        //#endregion

        //#region OrderMaster PartyFrom
        //public ActionResult _OrderMasterPartyFromDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable, int? orderType)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Enable = enable;
        //    IList<Supplier> supplierList = queryMgr.FindAll<Supplier>("from Supplier as s where s.IsActive = ?", true);
        //    IList<Customer> customerList = queryMgr.FindAll<Customer>("from Customer as c where c.IsActive = ?", true);
        //    IList<Region> regionList = queryMgr.FindAll<Region>("from Region as r where r.IsActive = ?", true);
        //    List<Party> partyList = new List<Party>();
        //    if (orderType == (int)com.Sconit.CodeMaster.OrderType.Procurement)
        //    {
        //        partyList.AddRange(supplierList);
        //        partyList.AddRange(customerList);
        //        partyList.AddRange(regionList);
        //    }
        //    if (orderType == (int)com.Sconit.CodeMaster.OrderType.Distribution)
        //    {
        //        partyList.AddRange(regionList);
        //    }
        //    if (orderType == (int)com.Sconit.CodeMaster.OrderType.Production)
        //    {
        //        partyList.AddRange(regionList);
        //    }
        //    else
        //    {
        //        partyList.AddRange(supplierList);
        //        partyList.AddRange(customerList);
        //        partyList.AddRange(regionList);
        //    }
        //    if (partyList == null)
        //    {
        //        partyList = new List<Party>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Party blankParty = new Party();
        //        partyList.Insert(0, blankParty);
        //    }
        //    return PartialView(new SelectList(partyList, "Code", "Name", selectedValue));
        //}
        //#endregion

        //#region OrderMaster PartyFrom
        //public ActionResult _OrderMasterPartyToDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable, int? orderType)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Enable = enable;
        //    IList<Supplier> supplierList = queryMgr.FindAll<Supplier>("from Supplier as s where s.IsActive = ?", true);
        //    IList<Customer> customerList = queryMgr.FindAll<Customer>("from Customer as c where c.IsActive = ?", true);
        //    IList<Region> regionList = queryMgr.FindAll<Region>("from Region as r where r.IsActive = ?", true);
        //    List<Party> partyList = new List<Party>();
        //    if (orderType == (int)com.Sconit.CodeMaster.OrderType.Procurement)
        //    {
        //        partyList.AddRange(regionList);
        //    }
        //    if (orderType == (int)com.Sconit.CodeMaster.OrderType.Distribution)
        //    {
        //        partyList.AddRange(regionList);
        //        partyList.AddRange(customerList);
        //    }
        //    if (orderType == (int)com.Sconit.CodeMaster.OrderType.Production)
        //    {
        //        partyList.AddRange(regionList);
        //    }
        //    else
        //    {
        //        partyList.AddRange(supplierList);
        //        partyList.AddRange(customerList);
        //        partyList.AddRange(regionList);
        //    }
        //    if (partyList == null)
        //    {
        //        partyList = new List<Party>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Party blankParty = new Party();

        //        partyList.Insert(0, blankParty);
        //    }
        //    return PartialView(new SelectList(partyList, "Code", "Name", selectedValue));
        //}
        //#endregion

        //#region Currency
        //public ActionResult _CurrencyDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.Enable = enable;
        //    IList<Currency> currencyList = queryMgr.FindAll<Currency>("from Currency as c");
        //    if (currencyList == null)
        //    {
        //        currencyList = new List<Currency>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Currency blankCurrency = new Currency();
        //        blankCurrency.Code = blankOptionValue;
        //        blankCurrency.Name = blankOptionDescription;

        //        currencyList.Insert(0, blankCurrency);
        //    }
        //    return PartialView(new SelectList(currencyList, "Code", "Name", selectedValue));
        //}
        //#endregion

        //#region Address
        //public ActionResult _AddressDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, string addressType)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    IList<Address> addressList = queryMgr.FindAll<Address>("from Address as a where a.Type=?", addressType);
        //    if (addressList == null)
        //    {
        //        addressList = new List<Address>();
        //    }

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        Address blankAddress = new Address();
        //        blankAddress.Code = blankOptionValue;
        //        blankAddress.AddressContent = blankOptionDescription;

        //        addressList.Insert(0, blankAddress);
        //    }
        //    return PartialView(new SelectList(addressList, "Code", "AddressContent", selectedValue));
        //}
        //#endregion

        //#region Address
        //public ActionResult _ProductLineFacilityDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? isChange, string dataBindFunc)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.ControlId = controlId;
        //    ViewBag.IsChange = isChange;
        //    ViewBag.DataBindFunc = dataBindFunc;
        //    IList<ProductLineFacility> productLineFacilityList = new List<ProductLineFacility>();

        //    if (includeBlankOption.HasValue && includeBlankOption.Value)
        //    {
        //        ProductLineFacility productLineFacility = new ProductLineFacility();
        //        productLineFacility.Code = blankOptionValue;

        //        productLineFacilityList.Insert(0, productLineFacility);
        //    }
        //    return PartialView(new SelectList(productLineFacilityList, "Code", "Code", selectedValue));
        //}

        //#endregion

        //#region Order
        //public ActionResult _OrderComboBox(string controlName, string selectedValue, int? orderType, bool? canFeed)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.OrderType = orderType;
        //    ViewBag.CanFeed = canFeed;
        //    IList<OrderMaster> orderList = new List<OrderMaster>();
        //    IList<object> para = new List<object>();
        //    string hql =  "from OrderMaster as o where o.OrderNo = ?";;
        //    if (!string.IsNullOrEmpty(selectedValue))
        //    {
        //        para.Add(selectedValue);
        //        orderList = queryMgr.FindAll<OrderMaster>(hql, para.ToArray());
        //    }
        //    return PartialView(new SelectList(orderList, "OrderNo", "OrderNo"));
        //}

        //public ActionResult _OrderAjaxLoading(string text, int? orderType, bool? canFeed)
        //{
        //    IList<OrderMaster> orderList = new List<OrderMaster>();
        //    string hql = "from OrderMaster as o where 1 = 1  ";
        //    IList<object> para = new List<object>();
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        hql += " and o.OrderNo like ?";
        //        para.Add(text + "%");
        //    }

        //    if (orderType != null)
        //    {
        //        hql += " and o.Type = ?";
        //        para.Add(orderType.Value);
        //    }
        //    if (canFeed != null)
        //    {
        //        if (canFeed.Value)
        //        {
        //            hql += " and o.Status in (?,?)";
        //            para.Add((int)com.Sconit.CodeMaster.OrderStatus.InProcess);
        //            para.Add((int)com.Sconit.CodeMaster.OrderStatus.Complete);
        //        }
        //        else
        //        {
        //            hql += " and o.Status not in (?,?)";
        //            para.Add((int)com.Sconit.CodeMaster.OrderStatus.InProcess);
        //            para.Add((int)com.Sconit.CodeMaster.OrderStatus.Complete);
        //        }
        //    }
        //    orderList = queryMgr.FindAll<OrderMaster>(hql, para.ToArray());
        //    return new JsonResult { Data = new SelectList(orderList, "OrderNo", "OrderNo") };
        //}
        //#endregion

        //#region OrderDetailTrace
        //public ActionResult _TraceCodeComboBox(string controlName, string selectedValue, bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.IsChange = isChange;
        //    IList<OrderDetailTrace> orderDetailTraceList = new List<OrderDetailTrace>();
        //    if (selectedValue != null && selectedValue.Trim() != string.Empty)
        //    {
        //        orderDetailTraceList = queryMgr.FindAll<OrderDetailTrace>(selectEqTraceCodeStatement, new object[] { selectedValue.Trim() });
        //    }
        //    return PartialView(new SelectList(orderDetailTraceList, "Id", "TraceCode"));
        //}

        //public ActionResult _TraceCodeAjaxLoading(string text)
        //{
        //    IList<OrderDetailTrace> orderDetailTraceList = new List<OrderDetailTrace>();
        //    string selectLikeTraceCodeStatement = null;
        //    object[] paramList = new object[] { };
        //    selectLikeTraceCodeStatement = "from OrderDetailTrace as o where o.TraceCode like ? ";
        //    paramList = new object[] { text + "%" };

        //    //追溯码只能like +%
        //    orderDetailTraceList = queryMgr.FindAll<OrderDetailTrace>(selectLikeTraceCodeStatement, paramList);
        //    return new JsonResult { Data = new SelectList(orderDetailTraceList, "Id", "TraceCode") };
        //}
        //#endregion


        //#region RejectOrder
        //public ActionResult _RejectComboBox(string controlName, string selectedValue, int? status,bool? isChange)
        //{
        //    ViewBag.ControlName = controlName;
        //    ViewBag.Status = status;
        //    ViewBag.IsChange = isChange;

        //    IList<RejectMaster> rejectList = new List<RejectMaster>();

        //    IList<object> para = new List<object>();
        //    string hql = "from RejectMaster as r where r.RejectNo = ?"; ;
        //    if (!string.IsNullOrEmpty(selectedValue))
        //    {
        //        para.Add(selectedValue);
        //        rejectList = queryMgr.FindAll<RejectMaster>(hql, para.ToArray());
        //    }
        //    return PartialView(new SelectList(rejectList, "RejectNo", "RejectNo"));
        //}

        //public ActionResult _RejectAjaxLoading(string text, int? status)
        //{
        //    IList<RejectMaster> rejectList = new List<RejectMaster>();

        //    string hql = "from RejectMaster as r where 1 = 1  ";
        //    IList<object> para = new List<object>();

        //    if (!string.IsNullOrEmpty(text))
        //    {
        //        hql += " and r.RejectNo like ?";
        //        para.Add(text + "%");
        //    }

        //    if (status != null)
        //    {
        //        hql += " and r.Status = ?";
        //        para.Add(status.Value);
        //    }

        //    rejectList = queryMgr.FindAll<RejectMaster>(hql, para.ToArray());
        //    return new JsonResult { Data = new SelectList(rejectList, "RejectNo", "RejectNo") };
        //}
        //#endregion

        //#region 联动
        //public ActionResult _AjaxLoadingIssueNo(string issueType)
        //{
        //    IList<IssueNo> issueNoList = new List<IssueNo>();
        //    if (issueType == null)
        //    {
        //        issueNoList = this.genericMgr.FindAll<IssueNo>("from IssueNo i ");
        //    }
        //    else
        //    {
        //        issueNoList = this.genericMgr.FindAll<IssueNo>("select i from IssueNo i join i.IssueType it where it.Code =? and it.IsActive=?", new object[] { issueType, true });
        //    }
        //    IssueNo blankIssueNo = new IssueNo();
        //    blankIssueNo.Code = string.Empty;
        //    blankIssueNo.Description = string.Empty;
        //    issueNoList.Insert(0, blankIssueNo);

        //    return new JsonResult
        //    {
        //        Data = new SelectList(issueNoList, "Code", "Description", "")
        //    };
        //}

        //public ActionResult _AjaxLoadingLocation(string party, string RegionValue)
        //{

        //    IList<Location> locationList = new List<Location>();
        //    if (party == null)
        //    {
        //        locationList = this.genericMgr.FindAll<Location>("from Location l where l.Region is null and l.IsActive=?", true);
        //    }
        //    else
        //    {
        //        locationList = this.genericMgr.FindAll<Location>("from Location l where l.Region=? and l.IsActive=?", new object[] { party, true });
        //    }
        //    Location blankLocation = new Location();
        //    blankLocation.Code = string.Empty;
        //    blankLocation.Name = string.Empty;
        //    locationList.Insert(0, blankLocation);
        //    if (string.IsNullOrEmpty(RegionValue))
        //    {
        //        return new JsonResult
        //        {
        //            Data = new SelectList(locationList, "Code", "CodeName", "")
        //        };
        //    }
        //    else
        //    {
        //        return new JsonResult
        //        {
        //            Data = new SelectList(locationList, "Code", "Name", RegionValue)
        //        };
        //    }
        //}

        //public ActionResult _AjaxLoadingLocationFrom(string partyFrom, string regionValue)
        //{
        //    return _AjaxLoadingLocation(partyFrom, regionValue);
        //}

        //public ActionResult _AjaxLoadingLocationTo(string partyTo)
        //{
        //    return _AjaxLoadingLocation(partyTo, "");
        //}


        //public ActionResult _AjaxLoadingLocationBin(string party, string RegionValue)
        //{

        //    IList<LocationBin> locationBinList = new List<LocationBin>();
        //    if (party == null)
        //    {
        //        locationBinList = this.genericMgr.FindAll<LocationBin>("from locationBin l where l.Region is null and l.IsActive=?", true);
        //    }
        //    else
        //    {
        //        locationBinList = this.genericMgr.FindAll<LocationBin>("from locationBin l where l.Region=? and l.IsActive=?", new object[] { party, true });
        //    }
        //    LocationBin blankLocation = new LocationBin();
        //    blankLocation.Code = string.Empty;
        //    blankLocation.Name = string.Empty;
        //    locationBinList.Insert(0, blankLocation);
        //    if (string.IsNullOrEmpty(RegionValue))
        //    {
        //        return new JsonResult
        //        {
        //            Data = new SelectList(locationBinList, "Code", "CodeName", "")
        //        };
        //    }
        //    else
        //    {
        //        return new JsonResult
        //        {
        //            Data = new SelectList(locationBinList, "Code", "Name", RegionValue)
        //        };
        //    }
        //}

        //public ActionResult _AjaxLoadingLocationBinFrom(string partyFrom, string regionValue)
        //{
        //    return _AjaxLoadingLocationBin(partyFrom, regionValue);
        //}

        //public ActionResult _AjaxLoadingLocationBinTo(string partyTo)
        //{
        //    return _AjaxLoadingLocationBin(partyTo, "");
        //}

        //public ActionResult _AjaxLoadingBillAddress(string party)
        //{
        //    IList<Address> addressList = new List<Address>();
        //    addressList = this.genericMgr.FindAll<Address>(supplierAddress, new object[] { party, 1 });
        //    Address blankAddress = new Address();
        //    blankAddress.Code = string.Empty;
        //    blankAddress.AddressContent = string.Empty;
        //    addressList.Insert(0, blankAddress);

        //    return new JsonResult
        //    {
        //        Data = new SelectList(addressList, "Code", "AddressContent", "")
        //    };
        //}

        //public ActionResult _AjaxLoadingShipFrom(string partyFrom)
        //{
        //    return _AjaxLoadingShipAdress(partyFrom);
        //}

        //public ActionResult _AjaxLoadingShipTo(string partyTo)
        //{
        //    return _AjaxLoadingShipAdress(partyTo);
        //}

        //public ActionResult _AjaxLoadingShipAdress(string party)
        //{
        //    IList<Address> addressList = new List<Address>();
        //    addressList = this.genericMgr.FindAll<Address>(shipToAddress, new object[] { party, 0 });
        //    Address blankAddress = new Address();
        //    blankAddress.Code = string.Empty;
        //    blankAddress.AddressContent = string.Empty;
        //    addressList.Insert(0, blankAddress);

        //    return new JsonResult
        //    {
        //        Data = new SelectList(addressList, "Code", "AddressContent", "")
        //    };
        //}

        //public ActionResult _AjaxLoadingProductLineFacility(string productLine)
        //{
        //    IList<ProductLineFacility> facilityList = this.genericMgr.FindAll<ProductLineFacility>(selectProductLineFacilityStatement, productLine);
        //    ProductLineFacility facility = new ProductLineFacility();
        //    facility.Code = string.Empty;
        //    facilityList.Insert(0, facility);

        //    return new JsonResult
        //    {
        //        Data = new SelectList(facilityList, "Code", "Code", "")
        //    };
        //}

        //public ActionResult _AjaxLoadingPriceList(string party)
        //{
        //    IList<PriceListMaster> priceListMasterList = new List<PriceListMaster>();
        //    priceListMasterList = this.genericMgr.FindAll<PriceListMaster>("from PriceListMaster p where p.Party=? and p.IsActive=?", new object[] { party, true });
        //    PriceListMaster blankPriceListMaster = new PriceListMaster();
        //    blankPriceListMaster.Code = string.Empty;
        //    blankPriceListMaster.Code = string.Empty;
        //    priceListMasterList.Insert(0, blankPriceListMaster);

        //    return new JsonResult
        //    {
        //        Data = new SelectList(priceListMasterList, "Code", "Code", "")
        //    };
        //}

        public ActionResult _AjaxLoadingSupplier(string text, bool checkPermission)
        {
            string hql = "from Supplier as s where s.Code like ? and s.IsActive = ?";
            IList<object> paraList = new List<object>();

            paraList.Add(text + "%");
            paraList.Add(true);

            User user = SecurityContextHolder.Get();
            if (user.Code.Trim().ToLower() != "su" && checkPermission)
            {
                hql += "  and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType =" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Supplier + " and u.PermissionCode = s.Code)";
            }
            IList<Supplier> supplierList = queryMgr.FindAll<Supplier>(hql, paraList.ToArray(), firstRow, maxRow);
            return new JsonResult { Data = new SelectList(supplierList, "Code", "CodeDescription", text) };
        }

        //public ActionResult _AjaxLoadingCustomer()
        //{
        //    IList<Customer> customerList = queryMgr.FindAll<Customer>("from Customer as c where c.IsActive = ?", true);
        //    return new JsonResult
        //    {
        //        Data = new SelectList(customerList, "Code", "Name")
        //    };
        //}
        //public ActionResult _AjaxLoadingRegion()
        //{
        //    IList<Region> regionList = queryMgr.FindAll<Region>("from Region as r where r.IsActive = ?", true);
        //    return new JsonResult
        //    {
        //        Data = new SelectList(regionList, "Code", "Name")
        //    };
        //}
        //public ActionResult _AjaxLoadingIssueType()
        //{
        //    IList<IssueType> issueTypeList = queryMgr.FindAll<IssueType>("from IssueType as it where it.IsActive = ?", true);
        //    return new JsonResult
        //    {
        //        Data = new SelectList(issueTypeList, "Code", "Description")
        //    };
        //}

        //#endregion

        //#endregion
        //#region private methods

        //#endregion


        public ActionResult _StatusDropDownList(int? selectedValue)
        {
            List<Status> statusList = new List<Status>();
            Status status0 = new Status();
            status0.Code = 0;
            status0.Name = "创建";
            statusList.Add(status0);
            Status status1 = new Status();
            status1.Code = 1;
            status1.Name = "成功";
            statusList.Add(status1);
            Status status2 = new Status();
            status2.Code = 2;
            status2.Name = "失败";
            statusList.Add(status2);
            selectedValue = selectedValue.HasValue ? selectedValue.Value : 2;

            return PartialView(new SelectList(statusList, "Code", "Name", selectedValue));
        }


    }

    class Status
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }
}
