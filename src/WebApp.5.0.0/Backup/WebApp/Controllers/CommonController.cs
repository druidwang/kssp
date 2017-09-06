namespace com.Sconit.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;
    using AutoMapper;
    using com.Sconit.Entity;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity.BIL;
    using com.Sconit.Entity.CUST;
    using com.Sconit.Entity.INP;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.ISS;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.MRP.MD;
    using com.Sconit.Entity.MRP.ORD;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity.PRD;
    using com.Sconit.Entity.SCM;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Type;
    using Telerik.Web.Mvc.UI;
    using com.Sconit.Entity.MRP.TRANS;
    using System.Text;
    using com.Sconit.CodeMaster;
    using com.Sconit.Utility;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections;
    using com.Sconit.Entity.SI.SAP;
    public class CommonController : WebAppBaseController
    {
        #region
        //下拉框默认只选出20条
        private static int firstRow = 0;
        private static int maxRow = 100;

        private static string selectEqMachineStatement = "from Machine as m where m.Code = ? ";

        private static string selectLikeMachineStatement = "from Machine as m where m.Code like ? ";

        private static string selectEqIslandStatement = "from Island as i where i.Code = ? ";

        private static string selectLikeIslandStatement = "from Island as i where i.Code like ? ";

        private static string selectEqItemStatement = "from Item as i where i.Code = ? and IsActive = ?";

        private static string selectLikeItemStatement = "from Item as i where i.Code like ? and IsActive = ?";

        private static string selectEqUserStatement = "from User as u where u.Code = ? and u.IsActive = ?";

        private static string selectLikeUserStatement = "from User as u where u.Code like ? and u.IsActive = ?";

        private static string selectEqItemPackageStatement = "from ItemPackage as i where  convert(VARCHAR,i.UnitCount) = ?";

        private static string selectLikeItemPackageStatement = "from ItemPackage as i where i.Item=? and convert(VARCHAR,i.UnitCount) like ? ";

        private static string selectEqSupplierStatement = "from Supplier as s where s.Code = ?";

        private static string selectEqCustomerStatement = "from Customer as c where c.Code = ?";

        private static string selectEqRegionStatement = "from Region as r where r.Code = ?";

        private static string selectLikeRoutingStatement = "from RoutingMaster as r where r.Code like ? and IsActive = ?";

        private static string selectEqBomStatement = "from BomMaster as b where b.Code = ? and IsActive = ?";

        private static string selectSAPLocationStatement = "  select distinct top " + maxRow + "  SAPLocation from md_location  where SAPLocation = '{0}'";

        private static string selectLikeSAPLocationStatement = " select distinct top " + maxRow + "  SAPLocation from md_location  where SAPLocation like '%{0}%' ";

        private static string selectEqIssueAddressStatement = "from IssueAddress as ia where  ia.Code = ? or ia.Description = ? order by Sequence";

        private static string selectLikeIssueAddressStatement = "from IssueAddress as ia where  ia.Code like ? or ia.Description like ?  order by Sequence";

        private static string selectEqFlowStatement = "from FlowMaster as f where f.Code = ? and f.IsActive = ? ";

        private static string selectEqLocationStatement = "from Location as l where l.Code = ?";

        private static string selectEqAddressStatement = "from Address as l where l.Code = ?";

        private static string selectEqPickStrategyStatement = " from PickStrategy as w where w.Code= ? ";

        #endregion

        //public IGenericMgr genericMgr { get; set; }
        //public IItemMgr itemMgr { get; set; }

        #region public methods
        public ActionResult _SiteMapPath(string menuContent)
        {
            //IList<com.Sconit.Entity.SYS.Menu> allMenu = systemMgr.GetAllMenu();
            //IList<MenuModel> allMenuModel = Mapper.Map<IList<com.Sconit.Entity.SYS.Menu>, IList<MenuModel>>(allMenu);

            //List<MenuModel> menuList = allMenuModel.Where(m => m.Code == menuContent).ToList();
            //this.NestGetParentMenu(menuList, menuList, allMenuModel);

            //foreach (var menu in menuList)
            //{
            //    string name = Resources.SYS.Menu.ResourceManager.GetString(menu.Name);
            //    if (name != null)
            //    {
            //        menu.Name = name;
            //    }
            //}
            //return PartialView(menuList);
            return PartialView();
        }

        #region MoveType

        public ActionResult _MoveTypeDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? coupled, string IOType, bool? isChange, string SubType, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Coupled = coupled;
            ViewBag.SelectedValue = selectedValue;
            ViewBag.IsChange = isChange;
            ViewBag.Enable = enable;
            List<Object> objs = new List<Object>();
            string hql = "from MiscOrderMoveType as m where 1=1  ";
            if (!string.IsNullOrWhiteSpace(SubType))
            {
                hql += " and m.SubType =? ";
                objs.Add(SubType);
            }
            if (!string.IsNullOrWhiteSpace(IOType))
            {
                hql += " and m.IOType=? ";
                objs.Add(IOType);
            }

            IList<MiscOrderMoveType> MoveTypeList = queryMgr.FindAll<MiscOrderMoveType>(hql, objs.ToArray());
            if (MoveTypeList == null)
            {
                MoveTypeList = new List<MiscOrderMoveType>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                MiscOrderMoveType blankMoveType = new MiscOrderMoveType();

                blankMoveType.Description = blankOptionDescription;
                MoveTypeList.Insert(0, blankMoveType);
            }

            return PartialView(new SelectList(MoveTypeList, "MoveType", "Description", selectedValue));

        }
        #endregion

        #region CodeMaster
        public ActionResult _CodeMasterDropDownList(com.Sconit.CodeMaster.CodeMaster code, string controlName, string controlId, string selectedValue, string ajaxActionName, bool? isSupplier,
             bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable, bool? isConsignment, bool? isShowNA, int? orderType, bool? isWidth)
        {
            IList<CodeDetail> codeDetailList = systemMgr.GetCodeDetails(code, includeBlankOption, blankOptionDescription, blankOptionValue);
            if (controlName == "ExcelTemplate")
            {
                string defaultpara = code.ToString();
                //DataSet ds = genericMgr.GetDatasetBySql(@"select distinct code from sys_codedet where value like '%xls'", null);
                //List<string> MultiCode = new List<string>();
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{
                //    MultiCode.Add(dr.ItemArray[0].ToString());
                //}

                var multiCodes = this.genericMgr.FindAllWithNativeSql<string>("select distinct code from sys_codedet where value like '%xls'");

                IList<CodeDetail> MulticodeDetailList = systemMgr.GetMultiCodeDetails(multiCodes, includeBlankOption, blankOptionDescription, blankOptionValue);
                codeDetailList = MulticodeDetailList;
            }
            if (isSupplier != null && isSupplier.Value)
            {
                codeDetailList = codeDetailList.Where(q => q.Value != ((int)com.Sconit.CodeMaster.OrderStatus.Create).ToString()).ToList();
            }

            //采购路线中的结算方式 不显示寄售结算  
            if (isConsignment != null)
            {
                if (code == com.Sconit.CodeMaster.CodeMaster.OrderBillTerm)
                {
                    if ((bool)isConsignment)
                    {
                        codeDetailList = codeDetailList.Where(p => p.Value == ((int)com.Sconit.CodeMaster.OrderBillTerm.ConsignmentBilling).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderBillTerm.NA).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderBillTerm.ReceivingSettlement).ToString()).ToList();
                    }
                    else
                    {
                        codeDetailList = codeDetailList.Where(p => p.Value != ((int)com.Sconit.CodeMaster.OrderBillTerm.ConsignmentBilling).ToString()).ToList();
                    }
                    if (isShowNA != null)
                    {
                        if (!(bool)isShowNA)
                        {
                            codeDetailList = codeDetailList.Where(p => p.Value != ((int)com.Sconit.CodeMaster.OrderBillTerm.NA).ToString()).ToList();
                        }
                    }
                }
            }

            //收货和发货的OrderType 不显示销售和生产
            if (code == com.Sconit.CodeMaster.CodeMaster.OrderType)
            {
                if (orderType != null)
                {
                    //codeDetailList = systemMgr.GetCodeDetails(code);
                    if (orderType.Value == (int)com.Sconit.CodeMaster.OrderType.Production)
                    {
                        codeDetailList = codeDetailList.Where(p => p.Value == ((int)com.Sconit.CodeMaster.OrderType.Production).ToString()).ToList();
                    }
                    else if (orderType.Value == (int)com.Sconit.CodeMaster.OrderType.Procurement)
                    {
                        if (isSupplier != null && isSupplier.Value)
                        {
                            codeDetailList = codeDetailList.Where(p => p.Value == ((int)com.Sconit.CodeMaster.OrderType.Procurement).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.CustomerGoods).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.SubContract).ToString()).ToList();
                        }
                        else
                        {
                            codeDetailList = codeDetailList.Where(p => p.Value == ((int)com.Sconit.CodeMaster.OrderType.Procurement).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.CustomerGoods).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.Transfer).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.SubContractTransfer).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.SubContract).ToString()).ToList();
                        }
                    }
                    else if (orderType.Value == (int)com.Sconit.CodeMaster.OrderType.Distribution)
                    {
                        if (controlName == "IpReportType")
                        {
                            codeDetailList = codeDetailList.Where(p => p.Value == ((int)com.Sconit.CodeMaster.OrderType.Distribution).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.Transfer).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.Procurement).ToString()).ToList();

                        }
                        else
                        {
                            codeDetailList = codeDetailList.Where(p => p.Value == ((int)com.Sconit.CodeMaster.OrderType.Distribution).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.Transfer).ToString() || p.Value == ((int)com.Sconit.CodeMaster.OrderType.SubContractTransfer).ToString()).ToList();
                        }
                    }

                    #region empty codedetail
                    if (includeBlankOption.HasValue && includeBlankOption.Value)
                    {
                        CodeDetail emptyCodeDetail = new CodeDetail();
                        emptyCodeDetail.Value = blankOptionValue;
                        emptyCodeDetail.Description = blankOptionDescription;
                        codeDetailList.Insert(0, emptyCodeDetail);
                    }
                    #endregion
                }
            }
            else if (code == com.Sconit.CodeMaster.CodeMaster.OrderSubType)
            {
                if(orderType.HasValue && orderType.Value == (int)com.Sconit.CodeMaster.OrderType.Production)
                {
                    codeDetailList = codeDetailList.Where(p =>
                        p.Value != ((int)com.Sconit.CodeMaster.OrderSubType.Return).ToString())
                        .ToList();
                }
                else
                {
                    codeDetailList = codeDetailList.Where(p =>
                          p.Value != ((int)com.Sconit.CodeMaster.OrderSubType.Other).ToString())
                          .ToList();
                }
            }
            else if (code == com.Sconit.CodeMaster.CodeMaster.HandleResult)
            {
                codeDetailList = codeDetailList.Where(p => p.Value != ((int)com.Sconit.CodeMaster.HandleResult.Qualify).ToString()).ToList();
            }
            else if (code == Sconit.CodeMaster.CodeMaster.QualityType)
            {
                codeDetailList = codeDetailList.Where(p => p.Value != ((int)com.Sconit.CodeMaster.QualityType.Inspect).ToString()).ToList();
            }
            else if(code == Sconit.CodeMaster.CodeMaster.ScheduleType)
            {
                codeDetailList = codeDetailList.Where(p => p.Sequence > 100).ToList();
            }
            else if (code == Sconit.CodeMaster.CodeMaster.OrderBillTerm)
            {
                codeDetailList = codeDetailList.Where(p => p.Value != ((int)com.Sconit.CodeMaster.OrderBillTerm.ConsignmentBilling).ToString()).ToList();
            }
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.AjaxActionName = ajaxActionName;
            ViewBag.isWidth = isWidth;
            ViewBag.Enable = enable;
            // codeDetailList.Add(new CodeDetail());
            //ViewBag.CascadingControlNames = cascadingControlNames;
            //ViewBag.ParentCascadeControlNames = parentCascadeControlNames;
            return PartialView(base.Transfer2DropDownList(code, codeDetailList, selectedValue));
        }
        public ActionResult _SAPLocationComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;

            ViewBag.isChange = isChange;
            IList<object> locationobjectList = new List<object>();
            IList<Location> locationList = new List<Location>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                locationobjectList = queryMgr.FindAllWithNativeSql<object>(string.Format(selectSAPLocationStatement, selectedValue));
                if (locationobjectList.Count != 0)
                {
                    foreach (object obj in locationobjectList)
                    {
                        Location cation = new Location();
                        cation.SAPLocation = obj.ToString();
                        locationList.Add(cation);
                    }
                }
            }

            return PartialView(new SelectList(locationList, "SAPLocation", "SAPLocation", selectedValue));


        }
        public ActionResult _SAPLocationAjaxLoading(string text)
        {

            IList<object> locationobjectList = null;
            IList<Location> locationList = new List<Location>();
            if (text == "")
                locationobjectList = queryMgr.FindAllWithNativeSql<object>("select distinct top " + maxRow + "  SAPLocation from md_location ");
            else
                locationobjectList = queryMgr.FindAllWithNativeSql<object>(string.Format(selectLikeSAPLocationStatement, text));


            if (locationobjectList.Count != 0)
            {
                foreach (object obj in locationobjectList)
                {
                    Location cation = new Location();
                    cation.SAPLocation = obj.ToString();
                    locationList.Add(cation);
                }
            }

            return new JsonResult { Data = new SelectList(locationList, "SAPLocation", "SAPLocation") };
        }

        public ActionResult _CodeMasterComboBox(com.Sconit.CodeMaster.CodeMaster code, string controlName, string controlId, string selectedValue, bool? enable, bool? isChange
           , bool? isMrp, bool? isRccpPlan)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.Code = code;
            ViewBag.isChange = isChange;
            ViewBag.isMrp = isMrp;
            ViewBag.isRccpPlan = isRccpPlan;
            //IList<CodeDetail> codeDetailList = new List<CodeDetail>();
            //if (selectedValue != null && selectedValue.Trim() != string.Empty)
            //{
            //    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ? and c.Value = ?", new object[] { code, selectedValue });
            //}
            IList<CodeDetail> codeDetailList = null;
            if (isRccpPlan != null)
            {
                if (selectedValue == null || selectedValue.Trim() == string.Empty)
                    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ?  and c.Value in(?,?)", new object[] { code,
                    (int)com.Sconit.CodeMaster.TimeUnit.Month,(int)com.Sconit.CodeMaster.TimeUnit.Week });
                else
                    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ? and c.Value in(?,?)", new object[] { code,
                        (int)com.Sconit.CodeMaster.TimeUnit.Month,(int)com.Sconit.CodeMaster.TimeUnit.Week }).Where(p => p.Value == selectedValue).ToList();
            }
            else if (isMrp != null)
            {
                if (selectedValue == null || selectedValue.Trim() == string.Empty)
                    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ?  and c.Value in(?,?,?)", new object[] { code,
                    (int)com.Sconit.CodeMaster.TimeUnit.Day,(int)com.Sconit.CodeMaster.TimeUnit.Month,(int)com.Sconit.CodeMaster.TimeUnit.Week });
                else
                    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ? and c.Value in(?,?,?)", new object[] { code,
                        (int)com.Sconit.CodeMaster.TimeUnit.Day,(int)com.Sconit.CodeMaster.TimeUnit.Month,(int)com.Sconit.CodeMaster.TimeUnit.Week }).Where(p => p.Value == selectedValue).ToList();
            }
            else
            {

                if (selectedValue == null || selectedValue.Trim() == string.Empty)
                    codeDetailList = systemMgr.GetCodeDetails(code);
                else
                    codeDetailList = systemMgr.GetCodeDetails(code).Where(p => p.Value == selectedValue).ToList();
            }
            if (controlName == "FiScrapType")
            {
                codeDetailList = systemMgr.GetCodeDetails(code).Where(p => p.Value == "27" || p.Value == "28" || p.Value == "29").ToList();
            }
            else if (controlName == "ScrapType")
            {
                codeDetailList = systemMgr.GetCodeDetails(code).Where(p => p.Value != "27" && p.Value != "28" && p.Value != "29").ToList();
            }
            return PartialView(base.Transfer2DropDownList(code, codeDetailList, selectedValue));
        }

        public ActionResult _CodeMasterAjaxLoading(string text, com.Sconit.CodeMaster.CodeMaster code, bool isMrp, bool isRccpPlan)
        {
            //IList<CodeDetail> codeDetailList = new List<CodeDetail>();

            //string hql = "from CodeDetail as c where c.Code = ?";
            //IList<object> paraList = new List<object>();
            //paraList.Add(code);
            //if (!string.IsNullOrEmpty(text))
            //{
            //    hql += " and c.Value like ?";
            //    paraList.Add(text + "%");
            //}
            IList<CodeDetail> codeDetailList = null;
            if (isRccpPlan)
            {
                if (text == "")
                {
                    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ? and c.Value in(?,?)", new object[] { code,
                        (int)com.Sconit.CodeMaster.TimeUnit.Month,(int)com.Sconit.CodeMaster.TimeUnit.Week });
                }
                else
                {
                    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ? and c.Value in(?,?)", new object[] { code,
                        (int)com.Sconit.CodeMaster.TimeUnit.Month,(int)com.Sconit.CodeMaster.TimeUnit.Week }).Where(p => p.Value == text).ToList();
                }
            }
            else if (isMrp)
            {
                if (text == "")
                {
                    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ? and c.Value in(?,?,?)", new object[] { code,
                        (int)com.Sconit.CodeMaster.TimeUnit.Day,(int)com.Sconit.CodeMaster.TimeUnit.Month,(int)com.Sconit.CodeMaster.TimeUnit.Week });
                }
                else
                {
                    codeDetailList = queryMgr.FindAll<CodeDetail>("from CodeDetail as c where c.Code = ? and c.Value in(?,?,?)", new object[] { code,
                        (int)com.Sconit.CodeMaster.TimeUnit.Day,(int)com.Sconit.CodeMaster.TimeUnit.Month,(int)com.Sconit.CodeMaster.TimeUnit.Week }).Where(p => p.Value == text).ToList();
                }
            }
            else
            {

                if (text == "")
                    codeDetailList = systemMgr.GetCodeDetails(code);
                else
                    codeDetailList = systemMgr.GetCodeDetails(code).Where(p => p.Value == text).ToList();
            }
            //queryMgr.FindAll<CodeDetail>(hql, paraList.ToArray(), firstRow, maxRow);

            if (code == com.Sconit.CodeMaster.CodeMaster.ScheduleType)
            {
                if (isMrp)
                {
                    codeDetailList = systemMgr.GetCodeDetails(code).Where(p => p.Value == "27" || p.Value == "28" || p.Value == "29").ToList();
                }
                else
                {
                    codeDetailList = systemMgr.GetCodeDetails(code).Where(p => p.Value != "27" && p.Value != "28" && p.Value != "29").ToList();
                }
            }
            IList<SelectListItem> itemList = Mapper.Map<IList<CodeDetail>, IList<SelectListItem>>(codeDetailList);
            foreach (var item in itemList)
            {
                item.Text = systemMgr.TranslateCodeDetailDescription(item.Text);
            }
            return new JsonResult { Data = new SelectList(itemList, "Value", "Text") };
        }
        #endregion

        #region User
        public ActionResult _UserComboBox(string controlName, string controlId, string selectedValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;

            IList<User> userList = new List<User>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                userList = queryMgr.FindAll<User>(selectEqUserStatement, new object[] { selectedValue, true });
            }
            return PartialView(new SelectList(userList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _UserAjaxLoading(string text)
        {
            IList<User> userList = new List<User>();
            userList = queryMgr.FindAll<User>(selectLikeUserStatement, new object[] { text + "%", true }, firstRow, maxRow);
            return new JsonResult { Data = new SelectList(userList, "Code", "CodeDescription") };
        }
        #endregion

        #region Item
        public ActionResult _ItemComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? coupled,
            string itemCategory, string materialsGroup, bool? includeBlankOption)
        {
            ViewBag.ItemCategory = itemCategory;
            ViewBag.MaterialsGroup = materialsGroup;
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.Coupled = coupled;
            ViewBag.IncludeBlankOption = includeBlankOption;
            IList<Item> itemList = new List<Item>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                itemList = queryMgr.FindAll<Item>(selectEqItemStatement, new object[] { selectedValue, true });
            }

            ViewBag.ItemFilterMode = base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode);
            ViewBag.ItemFilterMinimumChars = int.Parse(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMinimumChars));
            return PartialView(new SelectList(itemList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _ItemAjaxLoading(string text, string itemCategory, string materialsGroup, string flow, bool? includeBlankOption)
        {
            var isManualCreateDetail = false;
            if (!string.IsNullOrWhiteSpace(flow))
            {
                var flowMaster = this.genericMgr.FindById<FlowMaster>(flow);
                isManualCreateDetail = flowMaster.IsManualCreateDetail;
            }

            string sql = "select top " + maxRow +
                    @" i.* from MD_Item as i ";
            if (!string.IsNullOrWhiteSpace(flow) && !isManualCreateDetail)
            {
                sql += " left join SCM_FlowDet f on f.Item = i.Code ";
            }
            sql += " where i.Code like ? and i.IsActive = ? ";

            AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
            List<object> param = new List<object>();
            if (fileterMode == AutoCompleteFilterMode.Contains)
            {
                param.Add("%" + text + "%");
            }
            else
            {
                param.Add(text + "%");
            }
            param.Add(true);
            if (!string.IsNullOrWhiteSpace(itemCategory))
            {
                sql += " and i.ItemCategory in(? ";
                var itemCategorys = itemCategory.Split(',');
                for (int i = 0; i < itemCategorys.Length; i++)
                {
                    if (i > 0)
                    {
                        sql += " ,? ";
                    }
                }
                sql += ")";
                param.AddRange(itemCategorys);
            }
            if (!string.IsNullOrWhiteSpace(materialsGroup))
            {
                sql += " and i.MaterialsGroup=? ";
                param.Add(materialsGroup);
            }

            if (!string.IsNullOrWhiteSpace(flow) && (!isManualCreateDetail || (isManualCreateDetail && string.IsNullOrWhiteSpace(text))))
            {
                sql += " and f.Flow=? ";
                param.Add(flow);
            }
            var itemList = queryMgr.FindEntityWithNativeSql<Item>(sql, param.ToArray());
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                itemList.Insert(0, new Item());
            }
            return new JsonResult { Data = new SelectList(itemList, "Code", "CodeDescription") };
        }

        public ActionResult _ItemAjaxLoadingFilterByFlow(string text, string flow, string location, bool? isBomDetial)
        {

            var isManualCreateDetail = false;
            FlowMaster flowMaster = null;
            if (!string.IsNullOrWhiteSpace(flow))
            {
                flowMaster = this.genericMgr.FindById<FlowMaster>(flow);
                isManualCreateDetail = flowMaster.IsManualCreateDetail;
            }

            AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
            List<object> param = new List<object>();

            string sql = " select top " + maxRow + @" i.* from MD_Item as i ";

            if (!string.IsNullOrWhiteSpace(flow) && !isManualCreateDetail)
            {
                var referenceFlow = string.IsNullOrWhiteSpace(flowMaster.ReferenceFlow) ? flow : flowMaster.ReferenceFlow;
                if (isBomDetial.HasValue && isBomDetial.Value)
                {
                    sql += @"where i.code in 
                            (
	                            select distinct(Item) from PRD_BomDet where Item not like '29%' and Bom in 
	                            (
		                            select  distinct(case when LEN(isnull(d.Bom,''))=0 then d.Item else d.Bom end) from SCM_FlowDet d where d.Flow in(?,?)
	                            )
	                            union
	                            select distinct(Item) from PRD_BomDet where Bom in
	                            (
		                            select distinct(Item) from PRD_BomDet where Item like '29%' and Bom in 
		                            (
		                            select distinct(case when LEN(isnull(d.Bom,''))=0 then d.Item else d.Bom end) from SCM_FlowDet d where d.Flow in(?,?)
		                            )
	                            )
                            )
                          and i.Code like ? and i.IsActive = ? ";
                    param.Add(flow);
                    param.Add(referenceFlow);
                }
                else
                {
                    sql += @" where i.code in (select distinct(item) from SCM_FlowDet where Flow in(?,?) ) and i.Code like ? and i.IsActive = ? ";
                }
                param.Add(flow);
                param.Add(referenceFlow);
            }
            else if (!string.IsNullOrWhiteSpace(location))
            {
                if (isBomDetial.HasValue && isBomDetial.Value)
                {
                    sql += @"where i.code in 
                            (
	                            select distinct(Item) from PRD_BomDet where Item not like '29%' and Bom in 
	                            (
		                            select  distinct(case when LEN(isnull(d.Bom,''))=0 then d.Item else d.Bom end) from SCM_FlowDet d 
                                    join SCM_FlowMstr m on d.Flow = m.Code where m.LocFrom = ? 
	                            )
	                            union
	                            select distinct(Item) from PRD_BomDet where Bom in
	                            (
		                            select distinct(Item) from PRD_BomDet where Item like '29%' and Bom in 
		                            (
		                                select distinct(case when LEN(isnull(d.Bom,''))=0 then d.Item else d.Bom end) from SCM_FlowDet d 
                                        join SCM_FlowMstr m on d.Flow = m.Code where m.LocFrom = ? 
		                            )
	                            )
                            )
                          and i.Code like ? and i.IsActive = ? ";
                    param.Add(location);
                }
                else
                {
                    sql += @" where i.code in (select distinct(d.item) from SCM_FlowDet d join SCM_FlowMstr m on d.Flow = m.Code
                            where m.LocFrom = ? ) 
                            and i.Code like ? and i.IsActive = ? ";
                }
                param.Add(location);
            }
            else
            {
                sql += " where i.Code like ? and i.IsActive = ? ";
            }

            if (fileterMode == AutoCompleteFilterMode.Contains)
            {
                param.Add("%" + text + "%");
            }
            else
            {
                param.Add(text + "%");
            }
            param.Add(true);

            var itemList = queryMgr.FindEntityWithNativeSql<Item>(sql, param.ToArray());


            return new JsonResult { Data = new SelectList(itemList, "Code", "CodeDescription") };
        }

        public ActionResult _ItemAjaxLoadingFilterByLocation(string text, string location)
        {
            AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
            List<object> param = new List<object>();

            string sql = " select top " + maxRow + @" i.* from MD_Item as i ";

            if (!string.IsNullOrWhiteSpace(location))
            {
                sql += @"where i.Code in 
                        (
	                       select distinct(Item) from VIEW_LocationDet where Location =?
                        )
                        and i.Code like ? and i.IsActive = ? ";
                param.Add(location);
            }
            else
            {
                sql += " where i.Code like ? and i.IsActive = ? ";
            }

            if (fileterMode == AutoCompleteFilterMode.Contains)
            {
                param.Add("%" + text + "%");
            }
            else
            {
                param.Add(text + "%");
            }
            param.Add(true);

            var itemList = queryMgr.FindEntityWithNativeSql<Item>(sql, param.ToArray());
            return new JsonResult { Data = new SelectList(itemList, "Code", "CodeDescription") };
        }

        public ActionResult _ItemAjaxLoadingFilterBySection(string text, string section, string flow)
        {
            AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
            List<object> param = new List<object>();

            string sql = " select top " + maxRow + @" i.* from MD_Item as i ";


            if (!string.IsNullOrWhiteSpace(section))
            {
                if (!string.IsNullOrWhiteSpace(flow))
                {
                    sql += @" where i.code in (select distinct(d.Item) from SCM_FlowDet as d where d.Flow =? and d.Item in
                        (select distinct(Bom) from PRD_BomDet where Item =? ))
                        and i.Code like ? and i.IsActive = ? ";
                    param.Add(flow);
                }
                else
                {
                    sql += @" where i.code in (select distinct(Bom) from PRD_BomDet where Item =? )
                        and i.Code like ? and i.IsActive = ? ";
                }
                param.Add(section);
            }
            else
            {
                sql += " where i.Code like ? and i.IsActive = ? ";
            }

            if (fileterMode == AutoCompleteFilterMode.Contains)
            {
                param.Add("%" + text + "%");
            }
            else
            {
                param.Add(text + "%");
            }
            param.Add(true);

            var itemList = queryMgr.FindEntityWithNativeSql<Item>(sql, param.ToArray());
            return new JsonResult { Data = new SelectList(itemList, "Code", "CodeDescription") };
        }

        #endregion


        #region Machine
        public ActionResult _MachineComboBox(string controlName, string controlId, string selectedValue, bool? enable,
            bool? isChange, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.isChange = isChange;
            ViewBag.IncludeBlankOption = includeBlankOption;
            IList<Machine> MachineList = new List<Machine>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                MachineList = queryMgr.FindAll<Machine>(selectEqMachineStatement, new object[] { selectedValue });
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                MachineList.Insert(0, new Machine());
            }
            return PartialView(new SelectList(MachineList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _MachineAjaxLoading(string text, bool? includeBlankOption)
        {
            IList<Machine> MachineList = new List<Machine>();
            MachineList = queryMgr.FindAll<Machine>(selectLikeMachineStatement, new object[] { "%" + text + "%" }, firstRow, maxRow);
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                MachineList.Insert(0, new Machine());
            }
            return new JsonResult { Data = new SelectList(MachineList, "Code", "CodeDescription").Distinct() };
        }
        #endregion

        #region MrpSnapMaster
        public ActionResult _MrpSnapMasterComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange, bool? isRelease, bool? includeBlankOption, com.Sconit.CodeMaster.SnapType? snapType)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.isChange = isChange;
            ViewBag.IsRelease = isRelease;
            ViewBag.IncludeBlankOption = includeBlankOption;
            ViewBag.SnapType = snapType;
            IList<MrpSnapMaster> mrpSnapMasterList = new List<MrpSnapMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                mrpSnapMasterList = queryMgr.FindAll<MrpSnapMaster>(" from MrpSnapMaster m where m.SnapTime=? ", new object[] { Convert.ToDateTime(selectedValue) });
            }
            return PartialView(new SelectList(mrpSnapMasterList, "SnapTimeShow", "SnapTimeShow", selectedValue != "" ? Convert.ToDateTime(selectedValue).ToString("yyyy-MM-dd HH:mm:ss") : selectedValue));
        }

        public ActionResult _MrpSnapMasterAjaxLoading(string text, bool isRelease, bool? includeBlankOption, com.Sconit.CodeMaster.SnapType? snapType)
        {
            IList<MrpSnapMaster> mrpSnapMasterList = new List<MrpSnapMaster>();
            if (isRelease)
            {
                mrpSnapMasterList = queryMgr.FindAll<MrpSnapMaster>(" from MrpSnapMaster m where m.IsRelease =? and m.Type=? order by m.SnapTime desc  ", new object[] { isRelease, snapType }, firstRow, maxRow);
            }
            else
            {
                mrpSnapMasterList = queryMgr.FindAll<MrpSnapMaster>(" from MrpSnapMaster m where m.Type =? order by m.SnapTime desc  ", new object[] { snapType }, firstRow, maxRow);
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                mrpSnapMasterList.Insert(0, new MrpSnapMaster());
            }
            return new JsonResult { Data = new SelectList(mrpSnapMasterList, "SnapTimeShow", "SnapTimeShow") };
        }
        #endregion

        #region Island
        public ActionResult _IslandComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.isChange = isChange;
            ViewBag.IncludeBlankOption = includeBlankOption;
            IList<Island> islandList = new List<Island>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                islandList = queryMgr.FindAll<Island>(selectEqIslandStatement, new object[] { selectedValue });
            }
            return PartialView(new SelectList(islandList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _IslandAjaxLoading(string text, bool? includeBlankOption)
        {
            IList<Island> islandList = new List<Island>();
            islandList = queryMgr.FindAll<Island>(selectLikeIslandStatement, new object[] { "%" + text + "%" }, firstRow, maxRow);
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                islandList.Insert(0, new Island());
            }

            return new JsonResult { Data = new SelectList(islandList, "Code", "CodeDescription") };
        }
        #endregion

        #region _DateIndexComboBox
        public ActionResult _DateIndexComboBox(string controlName, string controlId, string selectedValue, bool? enable, string dateType, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IncludeBlankOption = includeBlankOption;

            IList<DateIndex> dateIndexList = new List<DateIndex>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                DateIndex dateIndex = new DateIndex();
                dateIndex.Code = selectedValue;
                if (dateType == ((int)com.Sconit.CodeMaster.TimeUnit.Week).ToString())
                {
                    var dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(selectedValue);
                    //weekOfYear.Description = 
                    var str = new StringBuilder(selectedValue);
                    str.Append("(");
                    str.Append(dateFrom.ToString("MM-dd"));
                    str.Append(" -> ");
                    dateFrom = dateFrom.AddDays(6);
                    str.Append(dateFrom.ToString("MM-dd"));
                    str.Append(")");
                    dateIndex.Description = str.ToString();
                }
                else
                {
                    dateIndex.Description = selectedValue;
                }
                dateIndexList.Add(dateIndex);
            }

            return PartialView(new SelectList(dateIndexList, "Code", "Description", selectedValue));
        }

        public ActionResult _AjaxLoadingDateIndex(string text, string dateType, bool? includeBlankOption)
        {
            IList<DateIndex> dateIndexList = new List<DateIndex>();
            if (string.IsNullOrEmpty(dateType))
            {
                return new JsonResult
                {
                    Data = new SelectList(dateIndexList, "Code", "Description")
                };
            }
            DateTime datetimeNow = DateTime.Now;
            if (text.Length > 4)
            {
                string[] dateArray = text.Split('-');
                string yyyy = dateArray[0];
                string MM = dateArray.Length > 1 ? dateArray[1] : "01";
                int mm = 1;
                int.TryParse(MM, out mm);
                MM = mm < 1 ? "01" : mm.ToString("D2");
                if (dateType == ((int)com.Sconit.CodeMaster.TimeUnit.Week).ToString())
                {
                    MM = mm > 52 ? "52" : mm.ToString("D2");
                    try
                    {
                        datetimeNow = com.Sconit.Utility.DateTimeHelper.GetWeekIndexDateFrom(yyyy + "-" + MM);
                    }
                    catch (Exception)
                    {
                        datetimeNow = DateTime.Now;
                    }
                }
                else
                {
                    MM = mm > 12 ? "12" : mm.ToString("D2");
                    string dd = "01";
                    DateTime.TryParse(yyyy + "-" + MM + "-" + dd, out datetimeNow);
                }
            }
            if (dateType == ((int)com.Sconit.CodeMaster.TimeUnit.Week).ToString())
            {
                string currentWeekOfYear = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(datetimeNow);

                string[] wky = currentWeekOfYear.Split('-');
                int yearIndex = int.Parse(wky[0]);
                int weekIndex = int.Parse(wky[1]);
                var dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(currentWeekOfYear);
                dateFrom = dateFrom.AddDays(-1);
                for (int i = 0; i < 30; i++)
                {
                    DateIndex dateIndex = new DateIndex();

                    if (weekIndex <= 0)
                    {
                        dateIndex.Code = (yearIndex - 1).ToString();
                        dateIndex.Code += "-" + (52 + weekIndex).ToString("D2");
                    }
                    else if (weekIndex > 52)
                    {
                        dateIndex.Code = (yearIndex + 1).ToString();
                        dateIndex.Code += "-" + (weekIndex - 52).ToString("D2");
                    }
                    else
                    {
                        dateIndex.Code = yearIndex.ToString();
                        dateIndex.Code += "-" + weekIndex.ToString("D2");
                    }

                    var str = new StringBuilder(dateIndex.Code);
                    str.Append("(");
                    dateFrom = dateFrom.AddDays(1);
                    str.Append(dateFrom.ToString("MM-dd"));
                    str.Append(" -> ");
                    dateFrom = dateFrom.AddDays(6);
                    str.Append(dateFrom.ToString("MM-dd"));
                    str.Append(")");
                    dateIndex.Description = str.ToString();

                    dateIndexList.Add(dateIndex);

                    weekIndex++;
                }
            }
            else
            {
                string currentMonthOfYear = datetimeNow.ToString("yyyy-MM-dd");
                string[] wky = currentMonthOfYear.Split('-');
                int yearIndex = int.Parse(wky[0]);
                int monthIndex = int.Parse(wky[1]);

                if (monthIndex == 12)
                {
                    monthIndex = 1;
                    ++yearIndex;
                }

                for (int i = 0; i < 30; i++)
                {
                    DateIndex newMonthOfYear = new DateIndex();
                    newMonthOfYear.Code = yearIndex + "-" + monthIndex.ToString("D2");

                    newMonthOfYear.Description = newMonthOfYear.Code;

                    dateIndexList.Add(newMonthOfYear);
                    if (monthIndex == 12)
                    {
                        monthIndex = 0;
                        ++yearIndex;
                    }
                    monthIndex++;
                }
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                dateIndexList.Insert(0, new DateIndex());
            }
            return new JsonResult
            {
                Data = new SelectList(dateIndexList, "Code", "Description")
            };
        }

        #region _PlanVersionComboBox
        public ActionResult _RccpPlanMasterComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isRelease, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IsRelease = isRelease;
            ViewBag.IncludeBlankOption = includeBlankOption;
            IList<RccpPlanMaster> rccpPlanMasterList = new List<RccpPlanMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                rccpPlanMasterList = queryMgr.FindAll<RccpPlanMaster>(" from RccpPlanMaster r where r.PlanVersion=? ", new object[] { Convert.ToDateTime(selectedValue) });
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                rccpPlanMasterList.Insert(0, new RccpPlanMaster());
            }
            return PartialView(new SelectList(rccpPlanMasterList, "PlanVersionShow", "PlanVersionShow", selectedValue != "" ? Convert.ToDateTime(selectedValue).ToString("yyyy-MM-dd HH:mm:ss") : selectedValue));
        }

        public ActionResult _RccpPlanMasterAjaxLoading(string text, int? timeUnit, bool? isRelease, bool? includeBlankOption)
        {
            IList<RccpPlanMaster> rccpPlanMasterList = new List<RccpPlanMaster>();
            if (timeUnit != null)
            {
                if (!isRelease.HasValue || !isRelease.Value)
                {
                    rccpPlanMasterList = queryMgr.FindAll<RccpPlanMaster>(" from RccpPlanMaster r where r.DateType=? order by r.PlanVersion desc ", new object[] { timeUnit }, firstRow, maxRow);
                }
                else
                {
                    rccpPlanMasterList = queryMgr.FindAll<RccpPlanMaster>(" from RccpPlanMaster r where r.DateType=? and IsRelease=? order by r.PlanVersion desc ", new object[] { timeUnit, isRelease.Value }, firstRow, maxRow);
                }
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                rccpPlanMasterList.Insert(0, new RccpPlanMaster());
            }
            return new JsonResult
            {
                Data = new SelectList(rccpPlanMasterList, "PlanVersionShow", "PlanVersionShow")
            };
        }
        #endregion
        #endregion

        #region _MrpPlanMasterComboBox
        public ActionResult _MrpPlanMasterComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isRelease, string resourceGroup, bool? includeBlankOption, bool? coupled, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IsRelease = isRelease;
            ViewBag.ResourceGroup = resourceGroup;
            ViewBag.IncludeBlankOption = includeBlankOption;
            ViewBag.Coupled = coupled;
            ViewBag.IsChange = isChange;
            IList<MrpPlanMaster> mrpPlanMasterList = new List<MrpPlanMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                mrpPlanMasterList = queryMgr.FindAll<MrpPlanMaster>(" from MrpPlanMaster m where m.PlanVersion=? ", new object[] { Convert.ToDateTime(selectedValue) });
            }
            return PartialView(new SelectList(mrpPlanMasterList, "PlanVersionShow", "PlanVersionShow", selectedValue != "" ? Convert.ToDateTime(selectedValue).ToString("yyyy-MM-dd HH:mm:ss") : selectedValue));
        }

        public ActionResult _MrpPlanMasterAjaxLoading(string text, bool? isRelease, int? resourceGroup, bool? includeBlankOption)
        {
            IList<MrpPlanMaster> mrpPlanMasterList = new List<MrpPlanMaster>();

            if (!isRelease.HasValue || !isRelease.Value)
            {
                if (resourceGroup == null)
                    mrpPlanMasterList = queryMgr.FindAll<MrpPlanMaster>(" from MrpPlanMaster m   order by m.PlanVersion desc ", firstRow, maxRow);
                else
                    mrpPlanMasterList = queryMgr.FindAll<MrpPlanMaster>(" from MrpPlanMaster m where ResourceGroup=?  order by m.PlanVersion desc ", new object[] { resourceGroup.Value }, firstRow, maxRow);
            }
            else
            {
                if (resourceGroup == null)
                    mrpPlanMasterList = queryMgr.FindAll<MrpPlanMaster>(" from MrpPlanMaster m where  IsRelease=? order by m.PlanVersion desc ", new object[] { isRelease.Value }, firstRow, maxRow);
                else
                    mrpPlanMasterList = queryMgr.FindAll<MrpPlanMaster>(" from MrpPlanMaster m where  IsRelease=? and ResourceGroup=? order by m.PlanVersion desc ", new object[] { isRelease.Value, resourceGroup.Value }, firstRow, maxRow);
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                mrpPlanMasterList.Insert(0, new MrpPlanMaster());
            }

            return new JsonResult
            {
                Data = new SelectList(mrpPlanMasterList, "PlanVersionShow", "PlanVersionShow")
            };
        }
        //
        #endregion

        #region _PurchasePlanMasterComboBox
        public ActionResult _PurchasePlanMasterComboBox(string controlName, string controlId, string selectedValue, bool? enable, int? dateType, string flow, bool? includeBlankOption, bool? isRelease, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.DateType = dateType;
            ViewBag.Flow = flow;
            ViewBag.IncludeBlankOption = includeBlankOption;
            ViewBag.IsRelease = isRelease;
            ViewBag.isChange = isChange;
            IList<PurchasePlanMaster> planMasterList = new List<PurchasePlanMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                var planMaster = new PurchasePlanMaster();
                planMaster.PlanVersion = Convert.ToDateTime(selectedValue);
                planMasterList.Add(planMaster);
            }
            return PartialView(new SelectList(planMasterList, "PlanVersionShow", "PlanVersionShow", selectedValue != "" ? Convert.ToDateTime(selectedValue).ToString("yyyy-MM-dd HH:mm:ss") : selectedValue));
        }

        public ActionResult _PurchasePlanMasterAjaxLoading(string text, int? dateType, string flow, bool? includeBlankOption, bool? isRelease)
        {
            var paramList = new List<object>();
            var sql = " select top " + maxRow + " t.a from(select distinct(PlanVersion) as a from MRP_PurchasePlanMaster where 1=1 ";
            //手工运行计划版本MRP_RccpPlanMaster必须是已经释放的
            sql += " and PlanVersion in (select PlanVersion from MRP_RccpPlanMaster rccp where status =1 and rccp.IsRelease=1)";
            if (dateType.HasValue)
            {
                sql += " and DateType=? ";
                paramList.Add(dateType.Value);
            }
            if (!string.IsNullOrWhiteSpace(flow))
            {
                sql += " and Flow=? ";
                paramList.Add(flow);
            }

            if (isRelease.HasValue)
            {
                sql += " and IsRelease=? ";
                paramList.Add(isRelease.Value);
            }

            sql += ") t order by t.a desc";

            var planMasterList = queryMgr.FindAllWithNativeSql<object>(sql, paramList.ToArray())
                .Select(p => new PurchasePlanMaster { PlanVersion = (DateTime)p }).ToList();

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                planMasterList.Insert(0, new PurchasePlanMaster());
            }
            return new JsonResult
            {
                Data = new SelectList(planMasterList, "PlanVersionShow", "PlanVersionShow")
            };
        }
        //
        #endregion

        #region _TransferPlanMasterComboBox
        public ActionResult _TransferPlanMasterComboBox(string controlName, string controlId, string selectedValue, bool? enable, string flow, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.Flow = flow;
            ViewBag.IncludeBlankOption = includeBlankOption;
            IList<TransferPlanMaster> planMasterList = new List<TransferPlanMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                var planMaster = new TransferPlanMaster();
                planMaster.PlanVersion = Convert.ToDateTime(selectedValue);
                planMasterList.Add(planMaster);
            }
            return PartialView(new SelectList(planMasterList, "PlanVersionShow", "PlanVersionShow", selectedValue != "" ? Convert.ToDateTime(selectedValue).ToString("yyyy-MM-dd HH:mm:ss") : selectedValue));
        }

        public ActionResult _TransferPlanMasterAjaxLoading(string text, string flow, bool? includeBlankOption)
        {
            var planMasterList = queryMgr.FindAll<TransferPlanMaster>
                (" from TransferPlanMaster m where Flow=? order by m.PlanVersion desc ",
                new object[] { flow }, firstRow, maxRow);

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                planMasterList.Insert(0, new TransferPlanMaster());
            }
            return new JsonResult
            {
                Data = new SelectList(planMasterList, "PlanVersionShow", "PlanVersionShow")
            };
        }
        //
        #endregion


        //
        #region _FlowItemComboBox
        public ActionResult _FlowItemComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IncludeBlankOption = includeBlankOption;

            IList<Item> itemList = new List<Item>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                itemList = queryMgr.FindAll<Item>(" from Item i where i.Code=? ", new object[] { selectedValue });
            }
            return PartialView(new SelectList(itemList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _FlowItemAjaxLoading(string text, string flow, bool? includeBlankOption)
        {
            AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
            string likeParam = fileterMode == AutoCompleteFilterMode.Contains ? ("%" + text + "%") : (text + "%");
            IList<Item> itemList = new List<Item>();
            if (!string.IsNullOrEmpty(flow))
            {
                IList<FlowDetail> flowDetailList = genericMgr.FindAll<FlowDetail>(" from FlowDetail where Flow=?", flow);
                IList<object> paraArrayList = new List<object>();
                string hql = string.Empty;
                if (flowDetailList != null && flowDetailList.Count > 0)
                {
                    foreach (var flowDetail in flowDetailList)
                    {
                        if (hql == string.Empty)
                        {
                            hql = "from Item f where f.Code like ? and f.Code in (?";
                            paraArrayList.Add(likeParam);
                            paraArrayList.Add(flowDetail.Item);
                        }
                        else
                        {
                            hql += ",?";
                            paraArrayList.Add(flowDetail.Item);
                        }
                    }
                    hql += " )";
                    itemList = queryMgr.FindAll<Item>(hql, paraArrayList.ToArray(), firstRow, maxRow);
                }
            }
            else
            {
                itemList = queryMgr.FindAll<Item>(selectLikeItemStatement, new object[] { likeParam, true }, firstRow, maxRow);
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                itemList.Insert(0, new Item());
            }

            return new JsonResult
            {
                Data = new SelectList(itemList, "Code", "CodeDescription")
            };
        }
        #endregion

        #region Bom
        public ActionResult _BomComboBox(string controlName, string controlId, string selectedValue, bool? isIncludeInActinve, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.IsChange = isChange;
            ViewBag.IsIncludeInActinve = isIncludeInActinve;
            IList<BomMaster> bomList = new List<BomMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                if (isIncludeInActinve.HasValue && isIncludeInActinve.Value)
                {
                    bomList = queryMgr.FindAll<BomMaster>("from BomMaster as b where b.Code = ?", selectedValue);
                }
                else
                {
                    bomList = queryMgr.FindAll<BomMaster>(selectEqBomStatement, new object[] { selectedValue, true });
                }
            }

            return PartialView(new SelectList(bomList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _BomAjaxLoading(string text, bool? isIncludeInActinve)
        {
            IList<BomMaster> bomList = new List<BomMaster>();

            List<object> paraList = new List<object>();
            paraList.Add(text + "%");
            string selectLikeBomStatement = string.Empty;
            if (isIncludeInActinve.HasValue && isIncludeInActinve.Value)
            {
                selectLikeBomStatement = "from BomMaster as b where b.Code like ? ";
            }
            else
            {
                selectLikeBomStatement = "from BomMaster as b where b.Code like ? and IsActive = ?";
                paraList.Add(true);
            }

            bomList = queryMgr.FindAll<BomMaster>(selectLikeBomStatement, paraList.ToArray(), firstRow, maxRow);
            return new JsonResult { Data = new SelectList(bomList, "Code", "CodeDescription") };
        }
        #endregion
        #region Cust report paraType
        public ActionResult _CustreportParaTypeDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            var ds = this.genericMgr.GetDatasetBySql("Select ParamType As Code,ParamType As CodeDescription from CustReport_ParamType Order by case when ParamType not like '/_%' escape '/'  then '__'+ParamType else ParamType End", new SqlParameter[1]);
            IList<CustReportParaType> paraTypeList = Utility.IListHelper.DataTableToList<CustReportParaType>(ds.Tables[0]);
            return PartialView(new SelectList(paraTypeList, "Code", "CodeDescription", selectedValue));
        }
        #endregion
        #region SIMESSnapShotTime
        public ActionResult _SIMESSnapShotTimeDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            string hql = "select distinct(r.MesInvSnaptime)  from SAPSnapShotInv as r ";
            IList<object> ObjectList = queryMgr.FindAll<object>(hql);
            IList<SAPSnapShotInv> sAPSnapShotInvList = new List<SAPSnapShotInv>();
            if (ObjectList != null)
            {
                foreach (object obj in ObjectList)
                {
                    SAPSnapShotInv sAPSnapShotInv = new SAPSnapShotInv();
                    sAPSnapShotInv.MesInvSnaptime = DateTime.Parse(obj.ToString());
                    sAPSnapShotInvList.Add(sAPSnapShotInv);
                }
            }
            return PartialView(new SelectList(sAPSnapShotInvList.OrderByDescending(p=>p.MesInvSnaptime), "MesInvSnaptime", "MesInvSnaptime", selectedValue));
        }
        #endregion
        #region ProDuctType
        public ActionResult _ProductTypeDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            IList<ProductType> productTypeCategoryList = null;
            productTypeCategoryList = queryMgr.FindAll<ProductType>("from ProductType as i");

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                ProductType blankproductTypeCategory = new ProductType();
                blankproductTypeCategory.Code = blankOptionValue;
                blankproductTypeCategory.Description = blankOptionDescription;

                productTypeCategoryList.Insert(0, blankproductTypeCategory);
            }
            return PartialView(new SelectList(productTypeCategoryList.OrderBy(p => p.Code), "Code", "CodeDescription", selectedValue));
        }
        #endregion
        #region ItemCategory
        public ActionResult _ItemCategoryDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable, string SubCategory)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            IList<ItemCategory> itemCategoryList = null;
            if (!string.IsNullOrWhiteSpace(SubCategory))
            {
                itemCategoryList = queryMgr.FindAll<ItemCategory>("from ItemCategory as i where i.SubCategory=?", SubCategory);
            }
            else
            {
                itemCategoryList = queryMgr.FindAll<ItemCategory>("from ItemCategory as i where i.SubCategory=0");
            }
            if (itemCategoryList == null)
            {
                itemCategoryList = new List<ItemCategory>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                ItemCategory blankitemCategory = new ItemCategory();
                blankitemCategory.Code = blankOptionValue;
                blankitemCategory.Description = blankOptionDescription;

                itemCategoryList.Insert(0, blankitemCategory);
            }
            return PartialView(new SelectList(itemCategoryList.OrderBy(p => p.Code), "Code", "CodeDescription", selectedValue));
        }
        #endregion

        #region Location
        public ActionResult _LocationComboBox(string controlName, string controlId, string selectedValue, bool? enable,
            bool? isChange, bool? checkRegion, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.Enable = enable;
            ViewBag.IsChange = isChange;
            ViewBag.ControlId = controlId;
            ViewBag.CheckRegion = checkRegion;
            ViewBag.SelectedValue = selectedValue;
            ViewBag.IncludeBlankOption = includeBlankOption;

            IList<Location> locationList = new List<Location>();

            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                locationList = queryMgr.FindAll<Location>(selectEqLocationStatement, new object[] { selectedValue });
            }
            return PartialView(new SelectList(locationList, "Code", "CodeName", selectedValue));
        }

        public ActionResult _AjaxLoadingLocation(string region, string text, bool checkRegion, bool? includeBlankOption)
        {
            string hql = "from Location l where  l.Code like ? and l.IsActive = ?";
            IList<object> paramList = new List<object>();

            paramList.Add(text + "%");
            paramList.Add(true);

            if (!string.IsNullOrEmpty(region))
            {
                hql += " and l.Region = ?";
                paramList.Add(region);
            }
            hql += " order by LEN(Code),Code ";
            var locationList = queryMgr.FindAll<Location>(hql, paramList.ToArray());
            if (checkRegion)
            {
                User user = SecurityContextHolder.Get();
                locationList = locationList.Where(p => user.RegionPermissions.Contains(p.Region)).Take(maxRow).ToList();
            }
            else
            {
                locationList = locationList.Take(maxRow).ToList();
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                locationList.Insert(0, new Location());
            }
            return new JsonResult
            {
                Data = new SelectList(locationList, "Code", "CodeName", text)
            };
        }

        public ActionResult _ProductionAdjustLocationComboBox(string controlName, string controlId, string selectedValue)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.SelectedValue = selectedValue;

            IList<Location> locationList = new List<Location>();

            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                locationList = queryMgr.FindAll<Location>(selectEqLocationStatement, new object[] { selectedValue });
            }
            return PartialView(new SelectList(locationList, "Code", "CodeName", selectedValue));
        }

        public ActionResult _AjaxLoadingProductionAdjustLocation(string text)
        {
            User user = SecurityContextHolder.Get();

            string sql = @"select * from MD_Location as l where Code in(select distinct(LocFrom) from SCM_FlowMstr where type=4) 
                         and l.Code like ? and l.IsActive = ?";
            IList<object> paramList = new List<object>();

            paramList.Add(text + "%");
            paramList.Add(true);

            sql += " order by LEN(Code),Code ";
            var locationList = queryMgr.FindEntityWithNativeSql<Location>(sql, paramList.ToArray())
                .Where(p => user.RegionPermissions.Contains(p.Region)).Take(maxRow);

            return new JsonResult
            {
                Data = new SelectList(locationList, "Code", "CodeName", text)
            };
        }
        #endregion

        #region LocationTransaction


        #endregion

        #region Party
        public ActionResult _PartyDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            IList<Party> PartyList = queryMgr.FindAll<Party>("from Party as p");
            if (PartyList == null)
            {
                PartyList = new List<Party>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                Party blankParty = new Party();
                blankParty.Code = blankOptionValue;
                blankParty.Name = blankOptionDescription;

                PartyList.Insert(0, blankParty);
            }
            return PartialView(new SelectList(PartyList.OrderBy(p => p.Code), "Code", "CodeDescription", selectedValue));
        }
        #endregion

        #region Tax
        public ActionResult _TaxDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            IList<Tax> taxList = queryMgr.FindAll<Tax>("from Tax as t");
            if (taxList == null)
            {
                taxList = new List<Tax>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                Tax blankTax = new Tax();
                blankTax.Code = blankOptionValue;
                blankTax.Name = blankOptionDescription;

                taxList.Insert(0, blankTax);
            }
            return PartialView(new SelectList(taxList, "Code", "Name", selectedValue));
        }
        #endregion

        #region Address
        public ActionResult _AddressComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange, bool? checkParty, int type)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.IsChange = isChange;
            ViewBag.Enable = enable;
            ViewBag.CheckParty = checkParty;
            ViewBag.Type = type;
            IList<Address> addressList = new List<Address>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                addressList = queryMgr.FindAll<Address>(selectEqAddressStatement, new object[] { selectedValue }, firstRow, maxRow);
            }

            return PartialView(new SelectList(addressList, "Code", "CodeAddressContent", selectedValue));
        }

        public ActionResult _AjaxLoadingAddress(string text, string party, int type, bool checkParty)
        {
            IList<Address> addressList = new List<Address>();

            string hql = string.Empty;
            IList<object> paramList = new List<object>();
            if (checkParty || !string.IsNullOrEmpty(party))
            {
                hql += "select a from PartyAddress p join p.Address as a where p.Party = ? and p.Type =? ";
                paramList.Add(party);
                paramList.Add(type);
            }
            else
            {
                hql += " from Address a where 1=1 ";
            }

            if (!string.IsNullOrEmpty(text))
            {
                hql += " and a.Code like ? ";
                paramList.Add(text + "%");
            }

            addressList = queryMgr.FindAll<Address>(hql, paramList.ToArray(), firstRow, maxRow).Distinct().ToList();

            return new JsonResult
            {
                Data = new SelectList(addressList, "Code", "CodeAddressContent", "")
            };
        }
        #endregion

        #region Routing lqy
        public ActionResult _RoutingDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            IList<RoutingMaster> routingList = queryMgr.FindAll<RoutingMaster>(" from RoutingMaster as r where r.IsActive = ? ", true);
            if (routingList == null)
            {
                routingList = new List<RoutingMaster>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                RoutingMaster blankRoutingMaster = new RoutingMaster();
                routingList.Insert(0, blankRoutingMaster);
            }
            return PartialView(new SelectList(routingList, "Code", "Name", selectedValue));
        }

        public ActionResult _RoutingComboBox(string controlName, string controlId, string selectedValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.controlId = controlId;
            ViewBag.Enable = enable;
            IList<RoutingMaster> routingList = queryMgr.FindAll<RoutingMaster>(" from RoutingMaster as r where r.IsActive = ? ", true);
            if (routingList == null)
            {
                routingList = new List<RoutingMaster>();
            }


            return PartialView(new SelectList(routingList, "Code", "CodeName", selectedValue));
        }

        public ActionResult _RoutingAjaxLoading(string text)
        {
            //AutoCompleteFilterMode fileterMode = AutoCompleteFilterMode.StartsWith;
            IList<RoutingMaster> routingList = new List<RoutingMaster>();

            //if (fileterMode == AutoCompleteFilterMode.Contains)
            //{
            //    routingList = queryMgr.FindAll<RoutingMaster>(selectLikeRoutingStatement, new object[] { "%" + text + "%", true }, firstRow, maxRow);
            //}
            //else
            //{
            routingList = queryMgr.FindAll<RoutingMaster>(selectLikeRoutingStatement, new object[] { text + "%", true }, firstRow, maxRow);
            //}

            return new JsonResult { Data = new SelectList(routingList, "Code", "CodeName") };
        }
        #endregion

        #region PriceList
        public ActionResult _PriceListComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange, bool? checkParty,string interfacePriceType)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.IsChange = isChange;
            ViewBag.Enable = enable;
            ViewBag.CheckParty = checkParty;
            ViewBag.InterfacePriceType = interfacePriceType;
            IList<PriceListMaster> priceList = new List<PriceListMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                priceList = queryMgr.FindAll<PriceListMaster>(" from PriceListMaster as p where p.Code = ? ", selectedValue);
            }

            return PartialView(new SelectList(priceList, "Code", "Code", selectedValue));
        }

        public ActionResult _AjaxLoadingPriceList(string party, string text, bool checkParty,string interfacePriceType)
        {
            IList<PriceListMaster> priceListMasterList = new List<PriceListMaster>();

            string hql = "from PriceListMaster p where  p.IsActive = ? ";
            IList<object> paramList = new List<object>();
            paramList.Add(true);

            if (!string.IsNullOrEmpty(text))
            {
                hql += " and p.Code like ?";
                paramList.Add(text + "%");
            }
            if (checkParty)
            {
                hql += " and p.Party = ?";
                paramList.Add(party);
            }

            if (!string.IsNullOrWhiteSpace(interfacePriceType))
            {
                hql += " and p.InterfacePriceType = ?";
                paramList.Add(interfacePriceType);
            }
            priceListMasterList = queryMgr.FindAll<PriceListMaster>(hql, paramList.ToArray(), firstRow, maxRow);

            return new JsonResult
            {
                Data = new SelectList(priceListMasterList, "Code", "Code", text)
            };
        }
        #endregion

        #region Uom
        public ActionResult _UomDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            IList<Uom> uomList = queryMgr.FindAll<Uom>("from Uom as u order by u.Sequence");
            if (uomList == null)
            {
                uomList = new List<Uom>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                Uom blankUom = new Uom();
                blankUom.Code = blankOptionValue;
                blankUom.Description = blankOptionDescription;

                uomList.Insert(0, blankUom);
            }
            return PartialView(new SelectList(uomList, "Code", "Code", selectedValue));
        }

        public ActionResult _AjaxLoadingUom(string text, string item)
        {
            #region 基本单位
            IList<string> uomList = new List<string>();
            if (string.IsNullOrEmpty(item))
            {
                Item baseItem = genericMgr.FindById<Item>(item);
                uomList.Add(baseItem.Uom);
            }
            #endregion

            #region 单位转换
            IList<UomConversion> uomConversionList = genericMgr.FindAll<UomConversion>
                ("from UomConversion as u where u.Item.Code = ? or u.Item is null ", item);
            if (uomConversionList != null && uomConversionList.Count > 0)
            {
                foreach (UomConversion uc in uomConversionList)
                {
                    if (!uomList.Contains(uc.BaseUom))
                    {
                        uomList.Add(uc.BaseUom);
                    }
                    if (!uomList.Contains(uc.AlterUom))
                    {
                        uomList.Add(uc.AlterUom);
                    }
                }
            }
            #endregion

            return new JsonResult { Data = new SelectList(uomList) };
        }

        #region newUom
        public ActionResult _NewUomDropDownList(string item, string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            ViewBag.Item = item;
            IList<Uom> uomList = itemMgr.GetItemUoms(item);

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                Uom blankUom = new Uom();
                blankUom.Code = blankOptionValue;
                blankUom.Description = blankOptionDescription;

                uomList.Insert(0, blankUom);
            }
            return PartialView(new SelectList(uomList, "Code", "Code", selectedValue));
        }

        #endregion



        #endregion

        #region IssueType
        public ActionResult _IssueTypeDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable, bool? coupled)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            ViewBag.Coupled = coupled;
            IList<com.Sconit.Entity.ISS.IssueType> issueTypeList = queryMgr.FindAll<com.Sconit.Entity.ISS.IssueType>("from IssueType where IsActive = ?", true);
            if (issueTypeList == null)
            {
                issueTypeList = new List<com.Sconit.Entity.ISS.IssueType>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                com.Sconit.Entity.ISS.IssueType blankIssueType = new com.Sconit.Entity.ISS.IssueType();
                blankIssueType.Code = blankOptionValue;
                blankIssueType.Description = blankOptionDescription;

                issueTypeList.Insert(0, blankIssueType);
            }
            return PartialView(new SelectList(issueTypeList, "Code", "Description", selectedValue));
        }
        #endregion

        #region IssueNo
        public ActionResult _IssueNoDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            ViewBag.IsChange = isChange;
            IList<IssueNo> issueNoList = queryMgr.FindAll<IssueNo>("from IssueNo as ino");
            if (issueNoList == null)
            {
                issueNoList = new List<IssueNo>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                IssueNo blankIssueNo = new IssueNo();
                blankIssueNo.Code = blankOptionValue;
                blankIssueNo.Description = blankOptionDescription;

                issueNoList.Insert(0, blankIssueNo);
            }
            return PartialView(new SelectList(issueNoList, "Code", "Description", selectedValue));
        }
        #endregion

        #region IssueLevel
        public ActionResult _IssueLevelDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            IList<IssueLevel> issueLevelList = queryMgr.FindAll<IssueLevel>("from IssueLevel as il");
            if (issueLevelList == null)
            {
                issueLevelList = new List<IssueLevel>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                IssueLevel blankIssueLevel = new IssueLevel();
                blankIssueLevel.Code = blankOptionValue;
                blankIssueLevel.Description = blankOptionDescription;

                issueLevelList.Insert(0, blankIssueLevel);
            }
            return PartialView(new SelectList(issueLevelList, "Code", "Description", selectedValue));
        }
        #endregion

        #region IssueAddress
        public ActionResult _IssueAddressDropDownList(string code, string controlName, string controlId, string selectedValue, bool? includeBlankOption, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;
            string hql = " from IssueAddress as ia ";
            if (!string.IsNullOrEmpty(code))
            {
                hql += "where ia.Code !='" + code + "'";
            }

            IList<IssueAddress> issueAddressList = queryMgr.FindAll<IssueAddress>(hql);

            if (issueAddressList == null)
            {
                issueAddressList = new List<IssueAddress>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                IssueAddress blankIssueAddress = new IssueAddress();
                issueAddressList.Insert(0, blankIssueAddress);
            }
            return PartialView(new SelectList(issueAddressList, "Code", "CodeDescription", selectedValue));
        }


        public ActionResult _IssueAddressComboBox(string controlName, string controlId, string selectedValue)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.SelectedValue = selectedValue;
            IList<IssueAddress> issueAddressList = new List<IssueAddress>();

            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                issueAddressList = queryMgr.FindAll<IssueAddress>(selectEqIssueAddressStatement, new object[] { selectedValue, selectedValue });
                if (issueAddressList.Count == 0)
                {
                    IssueAddress ia = new IssueAddress();
                    ia.Code = selectedValue;
                    issueAddressList.Add(ia);
                    return PartialView(new SelectList(issueAddressList, "Code", "CodeDescription", selectedValue));

                }
            }

            return PartialView(new SelectList(issueAddressList, "Code", "CodeDescription", selectedValue));

        }

        public ActionResult _IssueAddressAjaxLoading(string text)
        {
            IList<IssueAddress> issueAddressList = new List<IssueAddress>();

            issueAddressList = queryMgr.FindAll<IssueAddress>(selectLikeIssueAddressStatement, new object[] { text + "%", text + "%" });

            return new JsonResult { Data = new SelectList(issueAddressList, "Code", "CodeDescription") };
        }

        #endregion

        #region LocationArea
        public ActionResult _LocationAreaDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, string LocationCode)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            IList<LocationArea> locationAreaList = queryMgr.FindAll<LocationArea>("from LocationArea as i where i.IsActive = ? and i.Location=?",
                                                                   new object[] { true, LocationCode });
            if (locationAreaList == null)
            {
                locationAreaList = new List<LocationArea>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                LocationArea blankItem = new LocationArea();
                blankItem.Code = blankOptionValue;
                blankItem.Name = blankOptionDescription;

                locationAreaList.Insert(0, blankItem);
            }
            return PartialView(new SelectList(locationAreaList, "Code", "Name", selectedValue));
        }
        #endregion

        #region LocationBin
        public ActionResult _LocationBinDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            IList<LocationBin> locationBinList = queryMgr.FindAll<LocationBin>("from LocationBin as l where l.IsActive = ?", true);
            if (locationBinList == null)
            {
                locationBinList = new List<LocationBin>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                LocationBin blankItem = new LocationBin();
                blankItem.Code = blankOptionValue;
                blankItem.Name = blankOptionDescription;

                locationBinList.Insert(0, blankItem);
            }
            return PartialView(new SelectList(locationBinList, "Code", "Name", selectedValue));
        }

        public ActionResult _LocationBinComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange, string location)
        {
            ViewBag.ControlName = controlName;
            ViewBag.Enable = enable;
            ViewBag.IsChange = isChange;
            ViewBag.ControlId = controlId;
            ViewBag.SelectedValue = selectedValue;
            ViewBag.Location = location;
            IList<LocationBin> locationBinList = queryMgr.FindAll<LocationBin>("from LocationBin as l where l.IsActive=?", true);
            if (locationBinList == null)
            {
                locationBinList = new List<LocationBin>();
            }

            return PartialView(new SelectList(locationBinList, "Code", "CodeName", selectedValue));
        }

        public ActionResult _LocationBinAjaxLoading(string location, string text)
        {
            List<object> paramList = new List<object> { text + "%", true };
            var hql = "from LocationBin as l where l.Code like ? and l.IsActive = ?";
            if (!string.IsNullOrWhiteSpace(location))
            {
                hql += " and Location =? ";
                paramList.Add(location);
            }
            var locationBinList = queryMgr.FindAll<LocationBin>(hql, paramList.ToArray(), firstRow, maxRow);
            return new JsonResult { Data = new SelectList(locationBinList, "Code", "CodeName") };
        }
        #endregion

        #region ItemPackage
        public ActionResult _ItemPackageDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, string itemCode)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            IList<ItemPackage> itemPackageList = queryMgr.FindAll<ItemPackage>("from ItemPackage as i where  i.Item=?", itemCode);
            if (itemPackageList == null)
            {
                itemPackageList = new List<ItemPackage>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                ItemPackage blankItemPackage = new ItemPackage();
                blankItemPackage.UnitCount = Convert.ToDecimal(blankOptionValue);
                blankItemPackage.UnitCount = Convert.ToDecimal(blankOptionDescription);

                itemPackageList.Insert(0, blankItemPackage);
            }
            return PartialView(new SelectList(itemPackageList, "UnitCount", "UnitCount", selectedValue));
        }
        #endregion

        #region Container
        public ActionResult _ContainerDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            IList<Container> containerList = queryMgr.FindAll<Container>("from Container as c where  c.IsActive=?", true);
            if (containerList == null)
            {
                containerList = new List<Container>();
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                Container blankContainer = new Container();
                blankContainer.Code = blankOptionValue;
                blankContainer.Description = blankOptionDescription;

                containerList.Insert(0, blankContainer);
            }
            return PartialView(new SelectList(containerList, "Code", "Description", selectedValue));
        }
        #endregion

        #region ItemPackage ComboBox
        public ActionResult _ItemPackageComboBox(string controlName, string controlId, string selectedValue, string item)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            IList<ItemPackage> itemPackageList = new List<ItemPackage>();
            if (!string.IsNullOrEmpty(item))
            {
                itemPackageList = queryMgr.FindAll<ItemPackage>("select i from ItemPackage as i where i.Item=?", item, firstRow, maxRow);
            }
            else if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                itemPackageList = queryMgr.FindAll<ItemPackage>(selectEqItemPackageStatement, selectedValue, firstRow, maxRow);
            }

            if (!string.IsNullOrEmpty(selectedValue))
            {
                if (itemPackageList.Count < 1)
                {
                    ItemPackage itemPackage = new ItemPackage();
                    itemPackage.Item = item;
                    itemPackage.IsDefault = false;
                    itemPackage.UnitCount = decimal.Parse(selectedValue);
                    itemPackage.Description = selectedValue;
                    itemPackageList.Add(itemPackage);
                }
            }

            return PartialView(new SelectList(itemPackageList, "UnitCount", "UnitCount", selectedValue));
        }

        public ActionResult _ItemPackageAjaxLoading(string text, string item)
        {
            IList<ItemPackage> itemPackageList = new List<ItemPackage>();

            if (item == null)
            {
                itemPackageList = queryMgr.FindAll<ItemPackage>(selectLikeItemPackageStatement, new object[] { "", text + "%" });
            }
            else
            {
                itemPackageList = queryMgr.FindAll<ItemPackage>(selectLikeItemPackageStatement, new object[] { item, text + "%" });
            }

            return new JsonResult { Data = new SelectList(itemPackageList, "UnitCount", "UnitCount") };
        }
        #endregion

        #region Flow
        public ActionResult _FlowComboBox(string controlName, string controlId, string selectedValue,
            int? type, bool? isChange, bool? isSupplier, bool? isCreateHu, bool? isCreateOrder, bool? isMrp, bool? isReturn,
            bool? isVanOrder, bool? enable, int? resourceGroup, bool? coupled, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.Type = type;
            ViewBag.IsChange = isChange;
            ViewBag.IsSupplier = isSupplier;
            ViewBag.IsCreateHu = isCreateHu;
            ViewBag.IsCreateOrder = isCreateOrder;
            ViewBag.IsVanOrder = isVanOrder;
            ViewBag.Coupled = coupled;
            ViewBag.IsMrp = isMrp;
            ViewBag.IsReturn = isReturn;
            ViewBag.ResourceGroup = resourceGroup;
            ViewBag.IncludeBlankOption = includeBlankOption;

            List<FlowMaster> flowList = new List<FlowMaster>();
            if (!string.IsNullOrWhiteSpace(selectedValue))
            {
                flowList.AddRange(this.genericMgr.FindAll<FlowMaster>("from FlowMaster where Code like ? ", selectedValue + "%"));
            }
            return PartialView(new SelectList(flowList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _FlowAjaxLoading(string text, int? type, bool isSupplier, bool isCreateHu, bool isMrp,
            bool isCreateOrder, bool isVanOrder, bool isReturn, int? resourceGroup, bool? includeBlankOption)
        {
            ViewBag.ResourceGroup = resourceGroup;
            IList<FlowMaster> flowList = GetFlowMasterList(text, type, isSupplier, isCreateHu, isCreateOrder, isVanOrder, isMrp, isReturn, resourceGroup);
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                flowList.Insert(0, new FlowMaster());
            }
            return new JsonResult { Data = new SelectList(flowList, "Code", "CodeDescription") };
        }

        private IList<FlowMaster> GetFlowMasterList(string text, int? type, bool isSupplier, bool isCreateHu, bool isCreateOrder, bool isVanOrder, bool isMrp, bool isReturn, int? resourceGroup)
        {
            IList<FlowMaster> flowList = new List<FlowMaster>();

            User user = SecurityContextHolder.Get();
            string selectLikeFlowStatement = null;
            object[] paramList = new object[] { };

            if (type == null)
            {
                selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and  f.Code like ? ";
                paramList = new object[] { true, text + "%" };

                if (isCreateHu)
                {
                    selectLikeFlowStatement += " and f.Type in (?,?,?,?,?) ";
                    paramList = new object[] { true, text + "%", 
                        (int)com.Sconit.CodeMaster.OrderType.CustomerGoods,
                        (int)com.Sconit.CodeMaster.OrderType.Procurement, 
                        (int)com.Sconit.CodeMaster.OrderType.SubContract, 
                        (int)com.Sconit.CodeMaster.OrderType.Production, 
                        (int)com.Sconit.CodeMaster.OrderType.ScheduleLine };
                }
            }
            else if ((int)type == (int)com.Sconit.CodeMaster.OrderType.Procurement)
            {
                if (isSupplier)
                {
                    selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and f.Code like ? and f.Type in (?,?,?,?) ";
                    paramList = new object[] { true, text + "%", 
                        (int)com.Sconit.CodeMaster.OrderType.CustomerGoods, 
                        (int)com.Sconit.CodeMaster.OrderType.Procurement, 
                        (int)com.Sconit.CodeMaster.OrderType.SubContract, 
                        (int)com.Sconit.CodeMaster.OrderType.ScheduleLine };
                }
                else
                {
                    bool allowManualCreateProcurementOrder = bool.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.AllowManualCreateProcurementOrder));
                    if (isCreateOrder && !allowManualCreateProcurementOrder)
                    {
                        selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and f.Code like ? and f.Type in (?,?) ";
                        paramList = new object[] { true, text + "%", 
                            (int)com.Sconit.CodeMaster.OrderType.Transfer, 
                            (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer };
                    }
                    else
                    {
                        if (isReturn == false)
                        {
                            selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and f.Code like ? and f.Type in(?,?,?,?,?,?)";
                            paramList = new object[] { true, text + "%", 
                                (int)com.Sconit.CodeMaster.OrderType.Transfer, 
                                (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                                (int)com.Sconit.CodeMaster.OrderType.Procurement, 
                                (int)com.Sconit.CodeMaster.OrderType.CustomerGoods, 
                                (int)com.Sconit.CodeMaster.OrderType.ScheduleLine,
                                (int)com.Sconit.CodeMaster.OrderType.SubContract
                            };
                        }
                        else
                        {
                            selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and f.Code like ? and f.Type in(?,?,?,?,?)";
                            paramList = new object[] { true, text + "%", 
                                (int)com.Sconit.CodeMaster.OrderType.Transfer, 
                                (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                                (int)com.Sconit.CodeMaster.OrderType.Procurement, 
                                (int)com.Sconit.CodeMaster.OrderType.CustomerGoods, 
                                (int)com.Sconit.CodeMaster.OrderType.ScheduleLine
                            };
                        }
                    }
                }
            }
            else if ((int)type == (int)com.Sconit.CodeMaster.OrderType.Distribution)
            {
                if (isSupplier)
                {
                    selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and  f.Code like ? and f.Type =? ";
                    paramList = new object[] { true, text + "%" ,
                    (int)com.Sconit.CodeMaster.OrderType.Distribution,};
                }
                else
                {
                    selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and  f.Code like ? and f.Type in (?,?,?)";
                    paramList = new object[] { true, text + "%" ,
                    (int)com.Sconit.CodeMaster.OrderType.Transfer,
                    (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                    (int)com.Sconit.CodeMaster.OrderType.Distribution};
                }
            }
            else if ((int)type == (int)com.Sconit.CodeMaster.OrderType.Transfer)
            {
                selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and f.Code like ? and f.Type in (?,?) ";
                if (resourceGroup != null)
                {
                    selectLikeFlowStatement += @" and exists (select 1 from FlowMaster as p where p.LocationFrom = f.LocationTo
                       and p.ResourceGroup =? and p.IsActive = ? ) ";
                    paramList = new object[] { true, text + "%",
                    (int)com.Sconit.CodeMaster.OrderType.Transfer,
                    (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                    (int)resourceGroup,true};
                }
                else
                {
                    paramList = new object[] { true, text + "%",
                    (int)com.Sconit.CodeMaster.OrderType.Transfer,
                    (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer };
                }
            }
            else if ((int)type == (int)com.Sconit.CodeMaster.OrderType.Production)
            {
                selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and f.Code like ? and f.Type in (?) ";
                paramList = new object[] { true, text + "%", (int)com.Sconit.CodeMaster.OrderType.Production };
                if (resourceGroup != null)
                {
                    selectLikeFlowStatement += "and resourceGroup =? ";
                    paramList = new object[] { true, text + "%", (int)com.Sconit.CodeMaster.OrderType.Production, resourceGroup };
                }
                if (isVanOrder)
                {
                    selectLikeFlowStatement += @" and exists (select 1 from ProductLineMap as p where (p.ProductLine = f.Code 
                      or p.CabFlow = f.Code or p.ChassisFlow = f.Code) and (p.CabFlow != null and p.ChassisFlow != null))";
                }
            }

            var _flowList = queryMgr.FindAll<FlowMaster>(selectLikeFlowStatement, paramList);

            int i = 0;
            foreach (var flow in _flowList)
            {
                if (Utility.SecurityHelper.HasPermission(flow, isSupplier, isCreateOrder) && i < maxRow && ((isMrp && flow.IsMRP) || !isMrp))
                {
                    flowList.Add(flow);
                    i++;
                }
            }
            return flowList;
        }
        #endregion

        #region WorkingCalendar Flow
        public ActionResult _WorkingCalendarFlowComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IsChange = isChange;

            IList<FlowMaster> flowList = new List<FlowMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                flowList = queryMgr.FindAll<FlowMaster>("from FlowMaster as f where f.Code = ? and f.IsActive = ? and f.Type=?", new object[] { selectedValue.Trim(), true, (int)com.Sconit.CodeMaster.OrderType.Production });
            }

            return PartialView(new SelectList(flowList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _WorkingCalendarFlowAjaxLoading(string text, string region)
        {
            IList<FlowMaster> flowList = new List<FlowMaster>();
            if (!string.IsNullOrWhiteSpace(region))
            {
                User user = SecurityContextHolder.Get();
                string selectLikeFlowStatement = null;
                object[] paramContainList = new object[] { };
                object[] paramList = new object[] { };

                selectLikeFlowStatement = "from FlowMaster as f where f.IsActive = ? and  f.Code like ? and f.PartyFrom=? and f.Type=? ";
                paramList = new object[] { true, text + "%", region, (int)com.Sconit.CodeMaster.OrderType.Production };


                if (user.Code.Trim().ToLower() != "su")
                {
                    selectLikeFlowStatement += " and (f.IsCheckPartyFromAuthority = 0  or ( exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id + "and  p.PermissionCategoryType =" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + " and p.PermissionCode = f.PartyFrom)))";
                }

                flowList = queryMgr.FindAll<FlowMaster>(selectLikeFlowStatement, paramList, firstRow, maxRow);
            }
            return new JsonResult { Data = new SelectList(flowList, "Code", "CodeDescription") };
        }

        #endregion

        #region　pickstrategy

        public ActionResult _PickStrategyComboBox(string controlName, string controlId, string selectedValue, int? type, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Type = type;
            ViewBag.IsChange = isChange;

            //using com.Sconit.CodeMaster;
            IList<PickStrategy> flowList = new List<PickStrategy>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {

                flowList = queryMgr.FindAll<PickStrategy>(selectEqPickStrategyStatement, new object[] { selectedValue.Trim() });
            }

            return PartialView(new SelectList(flowList, "Code", "Code", selectedValue));
        }


        public ActionResult _FlowStrategyAjaxLoading(string text, int? type)
        {
            IList<PickStrategy> flowList = new List<PickStrategy>();

            string selectLikeFlowStatement = null;

            object[] paramList = new object[] { };
            if (type == null)
            {
                selectLikeFlowStatement = "from PickStrategy as w where w.Code like ? ";
                paramList = new object[] { text + "%" };
            }
            flowList = queryMgr.FindAll<PickStrategy>(selectLikeFlowStatement, paramList, firstRow, maxRow);

            return new JsonResult { Data = new SelectList(flowList, "Code", "Code") };
        }
        #endregion

        #region Supplier Combox
        public ActionResult _SupplierComboBox(string controlName, string controlId, string selectedValue, bool? isChange, bool? enable, bool? checkPermission)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.IsChange = isChange;
            ViewBag.Enable = enable;
            ViewBag.CheckPermission = checkPermission;

            IList<Supplier> supplierList = new List<Supplier>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                supplierList = queryMgr.FindAll<Supplier>(selectEqSupplierStatement, selectedValue);
            }

            return PartialView(new SelectList(supplierList, "Code", "CodeDescription", selectedValue));
        }


        public ActionResult _AjaxLoadingSupplier(string text, bool checkPermission)
        {
            string hql = "from Supplier as s where s.Code like ? and s.IsActive = ?";
            IList<object> paraList = new List<object>();
            paraList.Add(text + "%");
            paraList.Add(true);
            User user = SecurityContextHolder.Get();
            IList<Supplier> supplierList = queryMgr.FindAll<Supplier>(hql, paraList.ToArray());
            if (checkPermission)
            {
                supplierList = supplierList.Where(p => user.SupplierPersmissions.Contains(p.Code)).Take(maxRow).ToList();
            }
            return new JsonResult { Data = new SelectList(supplierList, "Code", "CodeDescription", text) };
        }
        #endregion

        #region Item ManufactureParty
        public ActionResult _ManufacturePartyComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IsChange = isChange;

            string hql = "select p from Region p where p.Code in(select distinct m.PartyFrom from FlowMaster as m where m.Type=" + (int)com.Sconit.CodeMaster.OrderType.Production + ")";

            IList<Region> regionList = queryMgr.FindAll<Region>(hql);
            if (regionList == null)
            {
                regionList = new List<Region>();
            }

            IList<Supplier> supplierList = queryMgr.FindAll<Supplier>("from Supplier as s");

            if (supplierList.Count > 0)
            {
                foreach (Supplier item in supplierList)
                {
                    Region region = new Region();
                    region.Code = item.Code;
                    region.Name = item.Name;
                    regionList.Add(region);
                }
            }

            return PartialView(new SelectList(regionList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _AjaxLoadingItemManufactureParty(string text, string item)
        {

            string partyhql = "select p from Supplier p where p.Code in (select distinct  m.PartyFrom from FlowMaster as m where exists (select 1 from FlowDetail as d where d.Flow=m.Code and d.Item=?) and m.Type=" + (int)com.Sconit.CodeMaster.OrderType.Procurement + ") and p.Code like ?";
            IList<object> partyParaList = new List<object>();

            partyParaList.Add(item);
            partyParaList.Add(text + "%");
            IList<Supplier> supplierList = genericMgr.FindAll<Supplier>(partyhql, partyParaList.ToArray(), firstRow, maxRow);
            if (supplierList.Count > 0)
            {
                return new JsonResult { Data = new SelectList(supplierList, "Code", "CodeDescription", text) };
            }
            else
            {
                string hql = "select p from Region p where p.Code in(select distinct m.PartyFrom from FlowMaster as m where m.Type=" + (int)com.Sconit.CodeMaster.OrderType.Production + ")";
                IList<Region> regionList = genericMgr.FindAll<Region>(hql);
                return new JsonResult { Data = new SelectList(regionList, "Code", "CodeDescription", text) };
            }
        }

        #endregion

        #region ManufactureParty
        public ActionResult _AjaxLoadingManufactureParty(string item)
        {
            string hql = "select p from Supplier p where p.Code in (select distinct  m.PartyFrom from FlowMaster as m where exists (select 1 from FlowDetail as d where d.Flow=m.Code and d.Item='" + item + "') and m.Type=" + (int)com.Sconit.CodeMaster.OrderType.Procurement + ")";
            IList<Supplier> supplierList = genericMgr.FindAll<Supplier>(hql);
            return new JsonResult { Data = new SelectList(supplierList, "Code", "CodeDescription") };
        }

        #endregion

        #region Customer Combox
        public ActionResult _CustomerComboBox(string controlName, string controlId, string selectedValue, bool? isChange, bool? enable, bool? checkPermission)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.IsChange = isChange;
            ViewBag.Enable = enable;
            ViewBag.CheckPermission = checkPermission;

            IList<Customer> customerList = new List<Customer>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                customerList = queryMgr.FindAll<Customer>(selectEqCustomerStatement, selectedValue);
            }
            return PartialView(new SelectList(customerList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _AjaxLoadingCustomer(string text, bool checkPermission)
        {
            string hql = "from Customer as c where c.Code like ? and c.IsActive = ?";
            IList<object> paraList = new List<object>();
            paraList.Add(text + "%");
            paraList.Add(true);
            User user = SecurityContextHolder.Get();
            IList<Customer> customerList = queryMgr.FindAll<Customer>(hql, paraList.ToArray());
            if (checkPermission)
            {
                customerList = customerList.Where(p => user.CustomerPermissions.Contains(p.Code)).Take(maxRow).ToList();
            }
            return new JsonResult { Data = new SelectList(customerList, "Code", "CodeDescription", text) };
        }
        #endregion

        #region Region Combox
        public ActionResult _RegionComboBox(string controlName, string controlId, string selectedValue, bool? isChange, bool? enable, bool? checkPermission)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.IsChange = isChange;
            ViewBag.Enable = enable;
            ViewBag.CheckPermission = checkPermission;
            IList<Region> regionList = new List<Region>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                regionList = queryMgr.FindAll<Region>(selectEqRegionStatement, selectedValue);
            }
            return PartialView(new SelectList(regionList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _RegionWorkShopComboBox(string controlName, string controlId, string selectedValue, bool? isChange, bool? enable, bool? checkPermission)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.IsChange = isChange;
            ViewBag.Enable = enable;
            ViewBag.CheckPermission = checkPermission;
            IList<Region> regionList = new List<Region>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                regionList = queryMgr.FindAll<Region>(" from Region as r where r.Workshop = ? ", selectedValue);
            }
            return PartialView(new SelectList(regionList, "Workshop", "Workshop", selectedValue));
        }

        public ActionResult _AjaxLoadingRegionWorkShop(string text)
        {
            string hql = "from Region as r where r.Code like ? ";
            IList<object> paraList = new List<object>();
            paraList.Add(text + "%");
            IList<Region> regionList = queryMgr.FindAll<Region>(hql, paraList.ToArray(), firstRow, maxRow);
            return new JsonResult { Data = new SelectList(regionList, "Workshop", "Workshop", text) };
        }

        public ActionResult _AjaxLoadingRegion(string text, bool checkPermission)
        {
            string hql = "from Region as r where r.Code like ? and r.IsActive = ?";
            IList<object> paraList = new List<object>();

            paraList.Add(text + "%");
            paraList.Add(true);

            User user = SecurityContextHolder.Get();
            if (user.Code.Trim().ToLower() != "su" && checkPermission)
            {
                hql += "  and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType =" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + " and u.PermissionCode = r.Code)";
            }
            IList<Region> regionList = queryMgr.FindAll<Region>(hql, paraList.ToArray(), firstRow, maxRow);
            return new JsonResult { Data = new SelectList(regionList, "Code", "CodeDescription", text) };
        }

        #endregion

        #region Operation
        public ActionResult _OperationDropDownList(string controlName, string controlId, string selectedValue, string Routing, int? CurrentOperation, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            //ViewBag.SelectedValue = selectedValue;
            ViewBag.Enable = enable;

            string hql = "";
            if (CurrentOperation == null)
            {
                hql = "select distinct(r.Operation)  from RoutingDetail as r where r.Routing='" + Routing + "'";
            }
            else
            {
                hql = "select distinct(r.Operation)  from RoutingDetail as r where r.Routing='" + Routing + "' and Operation > " + CurrentOperation + "";
            }
            IList<object> ObjectList = queryMgr.FindAll<object>(hql);
            IList<Operation> OperationList = new List<Operation>();
            if (ObjectList != null)
            {
                foreach (object obj in ObjectList)
                {
                    Operation operation = new Operation();
                    operation.OpCode = int.Parse(obj.ToString());
                    OperationList.Add(operation);
                }
            }
            return PartialView(new SelectList(OperationList, "OpCode", "OpCode", selectedValue));
        }

        #endregion

        #region OrderMaster Party
        public ActionResult _OrderMasterPartyFromComboBox(string controlName, string controlId, string selectedValue, bool? enable, int? orderType, bool? isSupplier, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.OrderType = orderType;
            ViewBag.isChange = isChange;
            ViewBag.IsSupplier = isSupplier;
            IList<Party> partyList = new List<Party>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                partyList = queryMgr.FindAll<Party>("from Party as p where p.Code = ?", selectedValue);
            }
            return PartialView(new SelectList(partyList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _AjaxLoadingOrderMasterPartyFrom(string text, int? orderType, bool isSupplier)
        {
            List<Party> partyList = new List<Party>();

            //su特殊处理
            User user = SecurityContextHolder.Get();
            string hql = "from Party as p where p.IsActive = ? and p.Code like ?";
            if (orderType == (int)com.Sconit.CodeMaster.OrderType.Procurement)
            {
                if (isSupplier)
                {
                    hql += " and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and u.PermissionCategoryType in (" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Supplier + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Customer + ") and u.PermissionCode = p.Code)";
                }
                else
                {
                    hql += " and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType in (" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Supplier + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Customer + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + ") and u.PermissionCode = p.Code)";
                }
            }
            else if (orderType == (int)com.Sconit.CodeMaster.OrderType.Distribution)
            {
                hql += " and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType =" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + " and u.PermissionCode = p.Code)";
            }
            else if (orderType == (int)com.Sconit.CodeMaster.OrderType.Production)
            {
                hql += " and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType =" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + " and u.PermissionCode = p.Code)";
            }
            else
            {
                hql += " and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType in (" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Supplier + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Customer + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + ") and u.PermissionCode = p.Code)";
            }
            partyList = queryMgr.FindAll<Party>(hql, new object[] { true, text + "%" }, firstRow, maxRow).ToList();
            return new JsonResult { Data = new SelectList(partyList, "Code", "CodeDescription", text) };
        }

        public ActionResult _OrderMasterPartyToComboBox(string controlName, string controlId, string selectedValue, bool? enable, int? orderType, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.isChange = isChange;
            ViewBag.OrderType = orderType;
            IList<Party> partyList = new List<Party>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                partyList = queryMgr.FindAll<Party>("from Party as p where p.Code = ?", selectedValue);
            }
            return PartialView(new SelectList(partyList, "Code", "CodeDescription", selectedValue));

        }

        public ActionResult _AjaxLoadingOrderMasterPartyTo(string text, int? orderType)
        {
            List<Party> partyList = new List<Party>();

            //su特殊处理
            User user = SecurityContextHolder.Get();

            string hql = "from Party as p where p.IsActive = ? and p.Code like ?";
            if (orderType == (int)com.Sconit.CodeMaster.OrderType.Procurement)
            {
                hql += " and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType =" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + " and u.PermissionCode = p.Code)";
            }
            else if (orderType == (int)com.Sconit.CodeMaster.OrderType.Distribution)
            {
                hql += " and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType in (" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Customer + ") and u.PermissionCode = p.Code)";
            }
            else if (orderType == (int)com.Sconit.CodeMaster.OrderType.Production)
            {
                hql += " and exists (select 1 from UserPermissionView as u where u.UserId =" + user.Id + "and  u.PermissionCategoryType =" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + " and u.PermissionCode = p.Code)";
            }
            else
            {
                hql += " and exists (select 1 from UserPermissionView as u where  u.UserId =" + user.Id + "and u.PermissionCategoryType in (" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Supplier + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Customer + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + ") and u.PermissionCode = p.Code)";
            }
            partyList = queryMgr.FindAll<Party>(hql, new object[] { true, text + "%" }, firstRow, maxRow).ToList();
            return new JsonResult { Data = new SelectList(partyList, "Code", "CodeDescription", text) };

        }
        #endregion

        #region Currency
        public ActionResult _CurrencyDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            IList<Currency> currencyList = queryMgr.FindAll<Currency>("from Currency as c");
            if (currencyList == null)
            {
                currencyList = new List<Currency>();
            }
            currencyList = currencyList.OrderByDescending(p => p.IsBase).ToList();
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                Currency blankCurrency = new Currency();
                blankCurrency.Code = blankOptionValue;
                blankCurrency.Name = blankOptionDescription;

                currencyList.Insert(0, blankCurrency);
            }
            return PartialView(new SelectList(currencyList, "Code", "Name", selectedValue));
        }
        #endregion

        #region Facility
        public ActionResult _ProductLineFacilityComboBox(string controlName, string controlId, string selectedValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            IList<ProductLineFacility> productLineFacilityList = new List<ProductLineFacility>();

            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                productLineFacilityList = queryMgr.FindAll<ProductLineFacility>("from ProductLineFacility as p where p.Code = ?", selectedValue);
            }
            return PartialView(new SelectList(productLineFacilityList, "Code", "Code", selectedValue));
        }

        public ActionResult _AjaxLoadingProductLineFacility(string productLine, string text)
        {
            IList<ProductLineFacility> facilityList = this.genericMgr.FindAll<ProductLineFacility>("from ProductLineFacility as p where p.Code like ? and p.ProductLine = ?", new object[] { text + "%", productLine }, firstRow, maxRow);

            return new JsonResult
            {
                Data = new SelectList(facilityList, "Code", "Code", "")
            };
        }
        #endregion

        #region Order
        public ActionResult _OrderComboBox(string controlName, string selectedValue, int? orderType, int? resourceGroup, bool? canFeed, bool? isChange, bool? isSupplier, bool? isCreateHu)
        {
            ViewBag.ControlName = controlName;
            ViewBag.OrderType = orderType;
            ViewBag.ResourceGroup = resourceGroup;
            ViewBag.CanFeed = canFeed;
            ViewBag.IsChange = isChange;
            ViewBag.IsSupplier = isSupplier;
            ViewBag.IsCreateHu = isCreateHu;

            IList<OrderMaster> orderList = new List<OrderMaster>();
            IList<object> para = new List<object>();
            string hql = "from OrderMaster as o where o.OrderNo = ?"; ;
            if (!string.IsNullOrEmpty(selectedValue))
            {
                para.Add(selectedValue);
                orderList = queryMgr.FindAll<OrderMaster>(hql, para.ToArray());
            }
            return PartialView(new SelectList(orderList, "OrderNo", "OrderNo", selectedValue));
        }

        public ActionResult _OrderAjaxLoading(string text, int? orderType, int? resourceGroup, bool? canFeed, bool isSupplier, bool isCreateHu)
        {
            IList<OrderMaster> orderList = new List<OrderMaster>();

            User user = SecurityContextHolder.Get();
            string hql = "from OrderMaster as o where o.OrderNo like ?";
            IList<object> para = new List<object>();
            para.Add(text + "%");

            if (isSupplier)
            {
                hql += "and o.Type in (?,?,?)";
                para.Add((int)com.Sconit.CodeMaster.OrderType.Procurement);
                para.Add((int)com.Sconit.CodeMaster.OrderType.CustomerGoods);
                para.Add((int)com.Sconit.CodeMaster.OrderType.SubContract);
            }
            else
            {
                if (orderType != null)
                {
                    hql += " and o.Type = ?";
                    para.Add(orderType.Value);
                }
                //resourceGroup
                if (resourceGroup != null)
                {
                    hql += " and o.ResourceGroup = ?";
                    para.Add(resourceGroup.Value);
                }
                if (isCreateHu)
                {
                    hql += "and o.Type in (?,?,?,?)";
                    para.Add((int)com.Sconit.CodeMaster.OrderType.Procurement);
                    para.Add((int)com.Sconit.CodeMaster.OrderType.CustomerGoods);
                    para.Add((int)com.Sconit.CodeMaster.OrderType.SubContract);
                    para.Add((int)com.Sconit.CodeMaster.OrderType.Production);
                }
            }
            if (canFeed != null)
            {
                if (canFeed.Value)
                {
                    hql += " and o.Status in (?,?)";
                    para.Add((int)com.Sconit.CodeMaster.OrderStatus.InProcess);
                    para.Add((int)com.Sconit.CodeMaster.OrderStatus.Complete);
                }
                else
                {
                    hql += " and o.Status not in (?,?)";
                    para.Add((int)com.Sconit.CodeMaster.OrderStatus.InProcess);
                    para.Add((int)com.Sconit.CodeMaster.OrderStatus.Complete);
                }
            }

            if (user.Code.Trim().ToLower() != "su")
            {
                if (isSupplier)
                {
                    hql += " and  exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id + " and  p.PermissionCategoryType in (" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Customer + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Supplier + ") and p.PermissionCode = o.PartyFrom)";
                }
                else
                {
                    hql += " and (o.IsCheckPartyFromAuthority = 0  or ( exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id + " and  p.PermissionCategoryType in (" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Customer + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Supplier + ") and p.PermissionCode = o.PartyFrom)))"
                           + " and (o.IsCheckPartyToAuthority = 0  or ( exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id + " and  p.PermissionCategoryType in (" + (int)com.Sconit.CodeMaster.PermissionCategoryType.Region + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Customer + "," + (int)com.Sconit.CodeMaster.PermissionCategoryType.Supplier + ") and p.PermissionCode = o.PartyTo)))";
                }
            }
            orderList = queryMgr.FindAll<OrderMaster>(hql, para.ToArray(), firstRow, maxRow);

            return new JsonResult { Data = new SelectList(orderList, "OrderNo", "OrderNo") };
        }
        #endregion

        #region InspectOrder
        public ActionResult _InspectComboBox(string controlName, string selectedValue, int? status, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.Status = status;
            ViewBag.IsChange = isChange;

            IList<InspectMaster> inspectList = new List<InspectMaster>();

            IList<object> para = new List<object>();
            string hql = "from InspectMaster as i where i.InspectNo = ?";
            if (!string.IsNullOrEmpty(selectedValue))
            {
                para.Add(selectedValue);
                inspectList = queryMgr.FindAll<InspectMaster>(hql, para.ToArray());
            }

            return PartialView(new SelectList(inspectList, "InspectNo", "InspectNo"));
        }

        public ActionResult _InspectAjaxLoading(string text, int? status)
        {
            IList<InspectMaster> inspectList = new List<InspectMaster>();

            string hql = "from InspectMaster as i where  i.InspectNo like ?";
            IList<object> para = new List<object>();
            para.Add(text + "%");

            if (status != null)
            {
                hql += " and i.Status = ?";
                para.Add(status.Value);
            }
            else
            {
                hql += " and i.Status in (?,?)";
                para.Add(com.Sconit.CodeMaster.InspectStatus.Submit);
                para.Add(com.Sconit.CodeMaster.InspectStatus.InProcess);
            }

            inspectList = queryMgr.FindAll<InspectMaster>(hql, para.ToArray(), firstRow, maxRow);
            return new JsonResult { Data = new SelectList(inspectList, "InspectNo", "InspectNo") };
        }
        #endregion

        #region RejectOrder
        public ActionResult _RejectComboBox(string controlName, string selectedValue, int? status, bool? isChange, int? handleResult)
        {
            ViewBag.ControlName = controlName;
            ViewBag.Status = status;
            ViewBag.IsChange = isChange;
            ViewBag.HandleResult = handleResult;

            IList<RejectMaster> rejectList = new List<RejectMaster>();

            IList<object> para = new List<object>();
            string hql = "from RejectMaster as r where r.RejectNo = ?";
            if (!string.IsNullOrEmpty(selectedValue))
            {
                para.Add(selectedValue);
                rejectList = queryMgr.FindAll<RejectMaster>(hql, para.ToArray());
            }

            return PartialView(new SelectList(rejectList, "RejectNo", "RejectNo"));
        }

        public ActionResult _RejectAjaxLoading(string text, int? status, int? handleResult)
        {
            IList<RejectMaster> rejectList = new List<RejectMaster>();

            string hql = "from RejectMaster as r where r.RejectNo like ?";
            IList<object> para = new List<object>();
            para.Add(text + "%");

            if (status != null)
            {
                hql += " and r.Status = ?";
                para.Add(status.Value);
            }
            if (handleResult != null)
            {
                hql += " and r.HandleResult = ?";
                para.Add(handleResult.Value);
            }

            rejectList = queryMgr.FindAll<RejectMaster>(hql, para.ToArray(), firstRow, maxRow);
            return new JsonResult { Data = new SelectList(rejectList, "RejectNo", "RejectNo") };
        }
        #endregion

        #region FailCode && DefectCode

        public ActionResult _FailCodeComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange, bool? isIncludeQualify)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IsChange = isChange;
            ViewBag.IsIncludeQualify = isIncludeQualify;
            IList<FailCode> FailCodeList = new List<FailCode>();

            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                FailCodeList = queryMgr.FindAll<FailCode>("from FailCode as f where f.Code = ?", selectedValue);
            }
            return PartialView(new SelectList(FailCodeList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _AjaxLoadingFailCode(string text, bool? isIncludeQualify)
        {
            IList<FailCode> failCodeList = queryMgr.FindAll<FailCode>("from FailCode as f where f.Code like ?", text + "%", firstRow, maxRow);
            if (isIncludeQualify == null || (isIncludeQualify.HasValue && !isIncludeQualify.Value))
            {
                failCodeList = failCodeList.Where(p => p.HandleResult != HandleResult.Qualify).ToList();
            }
            return new JsonResult { Data = new SelectList(failCodeList, "Code", "CodeDescription") };
        }

        public ActionResult _DefectCodeComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IsChange = isChange;
            IList<DefectCode> DefectCodeList = new List<DefectCode>();

            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                DefectCodeList = queryMgr.FindAll<DefectCode>("from DefectCode as f where f.Code = ?", selectedValue);
            }
            return PartialView(new SelectList(DefectCodeList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _AjaxLoadingDefect(string text)
        {
            IList<DefectCode> defectCodeList = queryMgr.FindAll<DefectCode>("from DefectCode as d where d.Code like ?", text + "%", firstRow, maxRow);
            return new JsonResult { Data = new SelectList(defectCodeList, "Code", "CodeDescription") };
        }
        #endregion

        #region ProductCode  Assemblies  FailCode DropDownList

        public ActionResult _ProductCodeDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            IList<object[]> ObjectList = queryMgr.FindAll<object[]>("select d.ProductCode,d.ProductCode as desc1 from DefectCode as d group by d.ProductCode");
            IList<ProductCode> ProductCodeList = new List<ProductCode>();
            if (ObjectList != null)
            {
                foreach (object obj in ObjectList)
                {
                    ProductCode pro = new ProductCode();
                    pro.Code = (((object[])(obj))[0]).ToString();
                    ProductCodeList.Add(pro);
                }
            }

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                ProductCode pro = new ProductCode();
                pro.Code = "";

                ProductCodeList.Insert(0, pro);
            }
            return PartialView(new SelectList(ProductCodeList, "Code", "Code", selectedValue));
        }

        public ActionResult _AssembliesDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            IList<Assemblies> AssembliesList = new List<Assemblies>();
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                Assemblies assemblies = new Assemblies();
                assemblies.Code = "";

                AssembliesList.Insert(0, assemblies);
            }
            return PartialView(new SelectList(AssembliesList, "Code", "Code", selectedValue));
        }

        public ActionResult _AssembliesAjaxLoading(string productCode)
        {
            IList<Assemblies> AssembliesList = new List<Assemblies>();
            if (productCode == "")
            {
                return new JsonResult { Data = new SelectList(AssembliesList, "Code", "Code") };
            }
            string hql = "select d.Assemblies,d.Assemblies as desc1 from DefectCode as d where d.Assemblies like ?  group by d.Assemblies";
            IList<object[]> ObjectList = queryMgr.FindAll<object[]>(hql, productCode + "%", firstRow, maxRow);

            if (ObjectList != null)
            {
                foreach (object obj in ObjectList)
                {
                    Assemblies assemblies = new Assemblies();
                    assemblies.Code = (((object[])(obj))[0]).ToString();
                    AssembliesList.Add(assemblies);
                }
            }

            return new JsonResult { Data = new SelectList(AssembliesList, "Code", "Code") };
        }


        public ActionResult _DefectCodeDropDownList(string controlName, string controlId, string selectedValue, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue, bool? enable)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            IList<DefectCode> AssembliesList = new List<DefectCode>();
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                DefectCode defectCode = new DefectCode();
                defectCode.Code = blankOptionDescription;
                defectCode.Description = blankOptionValue;

                AssembliesList.Insert(0, defectCode);
            }
            return PartialView(new SelectList(AssembliesList, "Code", "ComponentDefectCode", selectedValue));
        }

        public ActionResult _DefectCodeAjaxLoading(string assemblies)
        {
            IList<DefectCode> DefectCodeList = new List<DefectCode>();
            if (assemblies == "")
            {
                return new JsonResult { Data = new SelectList(DefectCodeList, "Code", "Description") };
            }
            string hql = "from DefectCode as d where d.ComponentDefectCode like ? ";
            DefectCodeList = queryMgr.FindAll<DefectCode>(hql, assemblies + "%", firstRow, maxRow);
            return new JsonResult { Data = new SelectList(DefectCodeList, "Code", "CodeDescription") };
        }


        #endregion

        #region  返回默认年默认月或者周
        public string DateWeek()
        {
            string currentWeekOfYear = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            string currentWeekOfYearTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return currentWeekOfYear + ',' + currentWeekOfYearTo;
        }

        public string DateMonth()
        {
            return DateTime.Now.ToString("yyyy-MM") + ',' + DateTime.Now.AddMonths(12).ToString("yyyy-MM");
        }

        #endregion

        #region 联动
        public ActionResult _AjaxLoadingIssueNo(string issueType)
        {
            IList<IssueNo> issueNoList = new List<IssueNo>();
            if (issueType == null)
            {
                issueNoList = this.genericMgr.FindAll<IssueNo>("from IssueNo i ");
            }
            else
            {
                issueNoList = this.genericMgr.FindAll<IssueNo>("select i from IssueNo i join i.IssueType it where it.Code =? and it.IsActive=?", new object[] { issueType, true });
            }
            IssueNo blankIssueNo = new IssueNo();
            blankIssueNo.Code = string.Empty;
            blankIssueNo.Description = string.Empty;
            issueNoList.Insert(0, blankIssueNo);

            return new JsonResult
            {
                Data = new SelectList(issueNoList, "Code", "Description", "")
            };
        }

        public ActionResult _AjaxLoadingLocationBin(string party, string RegionValue)
        {

            IList<LocationBin> locationBinList = new List<LocationBin>();
            if (party == null)
            {
                locationBinList = this.genericMgr.FindAll<LocationBin>("from locationBin l where l.Region is null and l.IsActive=?", true, firstRow, maxRow);
            }
            else
            {
                locationBinList = this.genericMgr.FindAll<LocationBin>("from locationBin l where l.Region=? and l.IsActive=?", new object[] { party, true }, firstRow, maxRow);
            }
            LocationBin blankLocation = new LocationBin();
            blankLocation.Code = string.Empty;
            blankLocation.Name = string.Empty;
            locationBinList.Insert(0, blankLocation);
            if (string.IsNullOrEmpty(RegionValue))
            {
                return new JsonResult
                {
                    Data = new SelectList(locationBinList, "Code", "CodeName", "")
                };
            }
            else
            {
                return new JsonResult
                {
                    Data = new SelectList(locationBinList, "Code", "Name", RegionValue)
                };
            }
        }

        public ActionResult _AjaxLoadingLocationBinFrom(string partyFrom, string regionValue)
        {
            return _AjaxLoadingLocationBin(partyFrom, regionValue);
        }

        public ActionResult _AjaxLoadingLocationBinTo(string partyTo)
        {
            return _AjaxLoadingLocationBin(partyTo, "");
        }



        public ActionResult _AjaxLoadingIssueType()
        {
            IList<com.Sconit.Entity.ISS.IssueType> issueTypeList = queryMgr.FindAll<com.Sconit.Entity.ISS.IssueType>("from IssueType as it where it.IsActive = ?", true);
            return new JsonResult
            {
                Data = new SelectList(issueTypeList, "Code", "Description")
            };
        }

        #endregion

        #endregion

        #region CostCenter
        public ActionResult _CostCenterComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.isChange = isChange;
            IList<CostCenter> list = new List<CostCenter>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                list = queryMgr.FindAll<CostCenter>("from CostCenter as m where m.Code = ? ",
                    new object[] { selectedValue });
            }
            return PartialView(new SelectList(list, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _CostCenterAjaxLoading(string text)
        {
            IList<CostCenter> list = new List<CostCenter>();
            list = queryMgr.FindAll<CostCenter>("from CostCenter as m where m.Code like ? ",
                new object[] { "%" + text + "%" }, firstRow, maxRow);
            return new JsonResult { Data = new SelectList(list, "Code", "CodeDescription") };
        }
        #endregion

        #region GeneralLedger
        public ActionResult _GeneralLedgerComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.isChange = isChange;
            IList<GeneralLedger> list = new List<GeneralLedger>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                list = queryMgr.FindAll<GeneralLedger>("from GeneralLedger as m where m.Code = ? ",
                    new object[] { selectedValue });
            }
            return PartialView(new SelectList(list, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _GeneralLedgerAjaxLoading(string text)
        {
            IList<GeneralLedger> list = new List<GeneralLedger>();
            list = queryMgr.FindAll<GeneralLedger>("from GeneralLedger as m where m.Code like ? ",
                new object[] { "%" + text + "%" }, firstRow, maxRow);
            return new JsonResult { Data = new SelectList(list, "Code", "CodeDescription") };
        }
        #endregion

        #region Section
        public ActionResult _SectionComboBox(string controlName, string controlId, string selectedValue, bool? enable,
            bool? coupled, string flow, bool? includeBlankOption)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.Coupled = coupled;
            ViewBag.Flow = flow;
            ViewBag.IncludeBlankOption = includeBlankOption;
            IList<Item> itemList = new List<Item>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                itemList = queryMgr.FindAll<Item>("from Item where code =? and IsActive =? ", new object[] { selectedValue, true });
            }
            return PartialView(new SelectList(itemList, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _SectionAjaxLoadingFilterByFlow(string text, string flow, bool? includeBlankOption)
        {
            AutoCompleteFilterMode fileterMode = (AutoCompleteFilterMode)Enum.Parse(typeof(AutoCompleteFilterMode), base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ItemFilterMode), true);
            List<object> param = new List<object>();

            string sql = " select top " + maxRow + @" i.* from MD_Item as i where i.ItemCategory=? ";
            param.Add("ZHDM");
            if (!string.IsNullOrWhiteSpace(flow))
            {
                sql += " and Code in (select distinct(Item) from MRP_ProdLineEx where ProductLine =?) ";
                param.Add(flow);
            }
            sql += " and i.Code like ? and i.IsActive = ? ";
            if (fileterMode == AutoCompleteFilterMode.Contains)
            {
                param.Add("%" + text + "%");
            }
            else
            {
                param.Add(text + "%");
            }
            param.Add(true);
            sql += " or i.Code ='299999' ";
            var itemList = queryMgr.FindEntityWithNativeSql<Item>(sql, param.ToArray());
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                itemList.Insert(0, new Item());
            }

            return new JsonResult { Data = new SelectList(itemList, "Code", "CodeDescription") };
        }

        #endregion

        #region HuTo
        public ActionResult _HuToComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? isChange)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.isChange = isChange;
            IList<HuTo> list = new List<HuTo>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                list = queryMgr.FindAll<HuTo>("from HuTo as m where m.Code = ? ",
                    new object[] { selectedValue });
            }
            return PartialView(new SelectList(list, "Code", "CodeDescription", selectedValue));
        }

        public ActionResult _HuToAjaxLoading(string text)
        {
            IList<HuTo> list = new List<HuTo>();
            list = queryMgr.FindAll<HuTo>("from HuTo as m where m.Code like ? ",
                new object[] { text + "%" });
            return new JsonResult { Data = new SelectList(list, "Code", "CodeDescription") };
        }
        public ActionResult _ProductTypeAjaxLoading(string text)
        {
            IList<ProductType> list = new List<ProductType>();
            list = queryMgr.FindAll<ProductType>("from ProductType as m where m.Code like ? Order by Code ",
                new object[] { text + "%" });
            return new JsonResult { Data = new SelectList(list, "Code", "CodeDescription") };
        }
        #endregion

        #region _FiShiftComboBox
        public ActionResult _FiShiftComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? includeBlankOption, bool? isChange, bool? coupled)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IncludeBlankOption = includeBlankOption;
            ViewBag.IsChange = isChange;
            ViewBag.Coupled = coupled;
            IList<ShiftMaster> shiftMasterList = new List<ShiftMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                shiftMasterList = queryMgr.FindAll<ShiftMaster>(" from ShiftMaster r where r.Code=? ", selectedValue);
            }
            return PartialView(new SelectList(shiftMasterList, "Code", "Name", selectedValue));
        }

        public ActionResult _FiShiftAjaxLoading(string text, string flow, DateTime? planVersion, DateTime? planDate, bool? includeBlankOption)
        {
            IList<ShiftMaster> shiftMasterList = new List<ShiftMaster>();
            if (!string.IsNullOrWhiteSpace(flow) && planVersion.HasValue && planDate.HasValue)
            {
                string sql = @"select s.* from PRD_ShiftMstr as s where code in
                        (select distinct(Shift) from MRP_MrpFiShiftPlan where ProductLine =? 
                         and PlanVersion = ?  and PlanDate = ? )";
                shiftMasterList = queryMgr.FindEntityWithNativeSql<ShiftMaster>(sql,
                    new object[] 
                { 
                    flow, planVersion.Value,planDate.Value
                });
            }
            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                shiftMasterList.Insert(0, new ShiftMaster());
            }
            return new JsonResult
            {
                Data = new SelectList(shiftMasterList, "Code", "Name")
            };
        }
        #endregion

        #region _ShiftComboBox
        public ActionResult _ShiftComboBox(string controlName, string controlId, string selectedValue, bool? enable, bool? includeBlankOption, bool? isChange, bool? coupled)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.IncludeBlankOption = includeBlankOption;
            ViewBag.IsChange = isChange;
            ViewBag.Coupled = coupled;
            IList<ShiftMaster> shiftMasterList = new List<ShiftMaster>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                shiftMasterList = queryMgr.FindAll<ShiftMaster>(" from ShiftMaster r where r.Code=? ", selectedValue);
            }
            return PartialView(new SelectList(shiftMasterList, "Code", "Name", selectedValue));
        }

        public ActionResult _ShiftAjaxLoading(string text, string flow, int? resourceGroup, bool? includeBlankOption)
        {
            IList<ShiftMaster> shiftMasterList = new List<ShiftMaster>();
            FlowMaster flowMstr = new FlowMaster();
            if (!string.IsNullOrWhiteSpace(flow))
            {
                flowMstr = genericMgr.FindById<FlowMaster>(flow);
            }
            if (!resourceGroup.HasValue && flowMstr.Code != null)
            {
                resourceGroup = (int)flowMstr.ResourceGroup;
            }
            string sql = @" from ShiftMaster as s where s.Code like ? ";
            string codeLike = "%";
            if (resourceGroup.HasValue)
            {
                codeLike = ((com.Sconit.CodeMaster.ResourceGroup)(resourceGroup.Value)).ToString() + codeLike;
            }
            shiftMasterList = queryMgr.FindAll<ShiftMaster>(sql, codeLike);

            if (includeBlankOption.HasValue && includeBlankOption.Value)
            {
                shiftMasterList.Insert(0, new ShiftMaster());
            }
            return new JsonResult
            {
                Data = new SelectList(shiftMasterList, "Code", "Name")
            };
        }
        #endregion

        #region PlanNo
        public ActionResult _PlanNoComboBox(string controlName, string controlId, string selectedValue,
            bool? enable, bool? coupled, string flow, string dateIndex)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            ViewBag.Enable = enable;
            ViewBag.Coupled = coupled;
            ViewBag.Flow = flow;
            ViewBag.DateIndex = dateIndex;
            IList<MrpExOrder> mrpExOrderList = new List<MrpExOrder>();
            if (selectedValue != null && selectedValue.Trim() != string.Empty)
            {
                mrpExOrderList = queryMgr.FindAll<MrpExOrder>("from MrpExOrder where PlanNo =? order by Sequence ",
                    new object[] { selectedValue });
            }
            return PartialView(new SelectList(mrpExOrderList, "PlanNo", "PlanNo", selectedValue));
        }

        public ActionResult _PlanNoAjaxLoading(string text, string flow, string dataIndex)
        {
            List<object> param = new List<object>();
            string hql = " from MrpExOrder where DateIndex =? ";
            if (string.IsNullOrWhiteSpace(dataIndex))
            {
                dataIndex = Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            }
            param.Add(dataIndex);
            if (!string.IsNullOrWhiteSpace(flow))
            {
                hql += " and ProductLine =? ";
                param.Add(flow);
            }
            hql += "order by Sequence";
            var mrpExOrderList = queryMgr.FindAll<MrpExOrder>(hql, param);
            return new JsonResult { Data = new SelectList(mrpExOrderList, "PlanNo", "PlanNo") };
        }
        #endregion

        #region CodeMasterMulti 复选下拉框
        public ActionResult _CodeMasterMultiSelectBox(string controlName, string controlId, com.Sconit.CodeMaster.CodeMaster code, string checkedValues)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            IList<CodeDetail> codeDetailList = systemMgr.GetCodeDetails(code);
            codeDetailList = systemMgr.GetCodeDetailDescription(codeDetailList);
            ViewBag.CodeDetails = codeDetailList;
            if (!string.IsNullOrWhiteSpace(checkedValues))
            {
                ViewBag.CheckedValues = checkedValues.Split(',').ToList();
            }
            return PartialView();
        }
        #endregion

        #region 接口 复选下拉框
        public ActionResult _SAPInterfaceSelectBox(string controlName, string controlId, string interfaceCode, string checkedValues)
        {
            ViewBag.ControlName = controlName;
            ViewBag.ControlId = controlId;
            IList<CodeDetail> codeDetailList = new List<CodeDetail>();
            if (controlName == "MultiSelectOrderType")
            {
                codeDetailList.Add(new CodeDetail { Value = "MIOrder", Description = Resources.EXT.ControllerLan.Con_MIOrder });
                codeDetailList.Add(new CodeDetail { Value = "FilterOrder", Description = Resources.EXT.ControllerLan.Con_FilterOrder });
                codeDetailList.Add(new CodeDetail { Value = "EXOrder", Description = Resources.EXT.ControllerLan.Con_EXOrder });
                codeDetailList.Add(new CodeDetail { Value = "FIOrder", Description = Resources.EXT.ControllerLan.Con_FIOrder });
                codeDetailList.Add(new CodeDetail { Value = "RWKOrder", Description = Resources.EXT.ControllerLan.Con_RWKOrder });
                codeDetailList.Add(new CodeDetail { Value = "AdjustOrder", Description = Resources.EXT.ControllerLan.Con_AdjustOrder });
            }
            else
            {
                codeDetailList.Add(new CodeDetail { Value = "GetSapItem", Description = Resources.EXT.ControllerLan.Con_Item });
                codeDetailList.Add(new CodeDetail { Value = "GetSapBom", Description = "Bom" });
                codeDetailList.Add(new CodeDetail { Value = "GetSapUomComv", Description = Resources.EXT.ControllerLan.Con_UomComv });
                codeDetailList.Add(new CodeDetail { Value = "GetSapPriceList", Description = Resources.EXT.ControllerLan.Con_PriceList });
                codeDetailList.Add(new CodeDetail { Value = "GetSapSupplier", Description = Resources.EXT.ControllerLan.Con_Supplier });
                codeDetailList.Add(new CodeDetail { Value = "GetSapScutomer", Description = Resources.EXT.ControllerLan.Con_Customer });
                codeDetailList.Add(new CodeDetail { Value = "BusinessData", Description = Resources.EXT.ControllerLan.Con_BusinessData });
                codeDetailList.Add(new CodeDetail { Value = "BusinessDataAdjustOrder", Description = Resources.EXT.ControllerLan.Con_BusinessDataCostCenterAdjustOrder });
                codeDetailList.Add(new CodeDetail { Value = "BusinessDataAdjustTailOrder", Description = Resources.EXT.ControllerLan.Con_BusinessDataAdjustTailOrder });
                codeDetailList.Add(new CodeDetail { Value = "SDMES0001", Description = Resources.EXT.ControllerLan.Con_SalesDistribution });
                codeDetailList.Add(new CodeDetail { Value = "SDMES0002", Description = Resources.EXT.ControllerLan.Con_SalesDistributionDCancel });
                codeDetailList.Add(new CodeDetail { Value = "PPMES0001", Description = Resources.EXT.ControllerLan.Con_PPReceive });
                codeDetailList.Add(new CodeDetail { Value = "PPMES0002", Description = Resources.EXT.ControllerLan.Con_PPReceiveCancel });
                codeDetailList.Add(new CodeDetail { Value = "PPMES0003", Description = Resources.EXT.ControllerLan.Con_PPExScrapt });
                codeDetailList.Add(new CodeDetail { Value = "PPMES0004", Description = Resources.EXT.ControllerLan.Con_PPMIFilter });
                codeDetailList.Add(new CodeDetail { Value = "PPMES0005", Description = Resources.EXT.ControllerLan.Con_PPAdjust });
                codeDetailList.Add(new CodeDetail { Value = "PPMES0006", Description = Resources.EXT.ControllerLan.Con_PPTrail });
                codeDetailList.Add(new CodeDetail { Value = "MMMES0001", Description = Resources.EXT.ControllerLan.Con_MMPurchase });
                codeDetailList.Add(new CodeDetail { Value = "MMMES0002", Description = Resources.EXT.ControllerLan.Con_MMPurchaseCancel });
                codeDetailList.Add(new CodeDetail { Value = "STMES0001", Description = Resources.EXT.ControllerLan.Con_StockTransfer });
            }
            ViewBag.CodeDetails = codeDetailList;
            if (!string.IsNullOrWhiteSpace(checkedValues))
            {
                ViewBag.CheckedValues = checkedValues.Split(',').ToList();
            }
            return PartialView();
        }
        #endregion

        #region private methods

        #endregion

        class DateIndex
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
        class CustReportParaType
        {
            public string Code { get; set; }
            public string CodeDescription { get; set; }
        }
    }

}
