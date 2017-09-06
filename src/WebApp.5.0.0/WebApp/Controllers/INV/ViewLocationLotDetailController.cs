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
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.VIEW;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Entity.SYS;


namespace com.Sconit.Web.Controllers.INV
{
    public class ViewLocationLotDetailController : ReportBaseController
    {
        #region public
        public ActionResult Index()
        {
            LocationLotDetailSearchModel serch = new LocationLotDetailSearchModel();
            serch.TypeLocation = "0";
            TempData["Display"] = "0";
            TempData["LocationLotDetailSearchModel"] = serch;
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Menu.Inventory.ViewInventory")]
        public ActionResult List(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            TempData["Display"] = searchModel.Level;
            TempData["LocationLotDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (searchModel.hideSupper)
            {
                ViewBag.HideParty = false;
            }
            else
            {
                ViewBag.HideParty = true;
            }
            if (searchModel.hideLotNo)
            {
                ViewBag.HideLotNo = false;
            }
            else
            {
                ViewBag.HideLotNo = true;
            }
            ViewBag.IsOnlyShowQtyInv = searchModel.IsOnlyShowQtyInv;
            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (string.IsNullOrEmpty(ViewBag.Location))
                ViewBag.Location = Resources.View.LocationDetailView.LocationDetailView_Location;
            else
                ViewBag.Location = ViewBag.Location;


            if (locationList.Count > 200)
            {
                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo))
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ItemCodeCanNotBeEmpty);
                    return View();
                }
            }
            if (itemList.Count > 2000)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PartExceedTwoThousand);
                return View();
            }

            if (searchModel.TypeLocation == "0")
            {
                if (string.IsNullOrEmpty(searchModel.Level))
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_SummaryToCanNotBeEmpty);
                    return View();
                }
                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo) && string.IsNullOrEmpty(searchModel.locationFrom) && string.IsNullOrEmpty(searchModel.locationTo)
                    && string.IsNullOrEmpty(searchModel.regionFrom) && string.IsNullOrEmpty(searchModel.regionTo)
                    )
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_ConditionSearchNeeded);
                    return View();
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.itemTo))
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_CanNotInputSecondItemWhenErrorFirstItemIsEmpty);
                        return View();
                    }
                }
                if (string.IsNullOrEmpty(searchModel.locationFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.locationTo))
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_CanNotInputSecondLocationWhenErrorFirstLocationIsEmpty);
                        return View();
                    }
                }

                if (string.IsNullOrEmpty(searchModel.regionFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.regionTo))
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_CanNotInputSecondAreaWhenErrorFirstAreaIsEmpty);
                        return View();
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(searchModel.SAPLocation))
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_SAPLocationCanNotBeEmpty_1);
                    return View();
                }
            }
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Menu.Inventory.ViewInventory")]
        public ActionResult _AjaxList(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            if (this.ReturnZeroRow(command, searchModel))
            {
                return PartialView(new GridModel(new List<LocationDetailView>()));
            }

            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<LocationDetailView> gridModel = GetReportAjaxPageData<LocationDetailView>(reportSearchStatementModel);
            var itemlist = itemMgr.GetCacheAllItem();
            foreach(var gridModelData in gridModel.Data)
            {
                gridModelData.ItemDescription = itemlist.ValueOrDefault(gridModelData.Item).FullDescription;
            }
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            var locationDic = this.genericMgr.FindAll<Location>().GroupBy(p => p.Code).ToDictionary(d => d.Key, d => d.First());
            foreach (LocationDetailView locationDetail in gridModel.Data)
            {
                Item item = this.itemMgr.GetCacheItem(locationDetail.Item);
                locationDetail.ItemDescription = item.Description;
                locationDetail.Uom = item.Uom;
                locationDetail.LocationName = locationDic[locationDetail.Location].Name;
                locationDetail.MaterialsGroup = item.MaterialsGroup;
              //  locationDetail.MaterialsGroupDesc = GetItemCategory(locationDetail.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            }

            return PartialView(gridModel);
        }

        #endregion
        private bool ReturnZeroRow(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            if (searchModel.TypeLocation == "0")
            {
                if (string.IsNullOrEmpty(searchModel.Level))
                {
                    return true;
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo) && string.IsNullOrEmpty(searchModel.locationFrom) && string.IsNullOrEmpty(searchModel.locationTo)
                    && string.IsNullOrEmpty(searchModel.regionFrom) && string.IsNullOrEmpty(searchModel.regionTo))
                {
                    return true;
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.itemTo))
                    {
                        return true;
                    }
                }
                if (string.IsNullOrEmpty(searchModel.locationFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.locationTo))
                    {
                        return true;
                    }
                }

                if (string.IsNullOrEmpty(searchModel.regionFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.regionTo))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(searchModel.SAPLocation))
                {
                    return true;
                }
            }
            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (locationList.Count > 200)
            {
                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo))
                {
                    return true;
                }
            }
            if (itemList.Count > 2000)
            {
                return true;
            }
            return false;
        }

        #region 导出实时库存报表
        [SconitAuthorize(Permissions = "Menu.Inventory.ViewInventory")]
        public void ExportXLS(LocationLotDetailSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = ReturnZeroRow(command, searchModel) ? 0 : value;

            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<LocationDetailView> gridModel = GetReportAjaxPageData<LocationDetailView>(reportSearchStatementModel);
            //List<LocationDetailView> LocationDetailViewLists = new List<LocationDetailView>();
            //var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            //var locationDic = this.genericMgr.FindAll<Location>().GroupBy(p => p.Code).ToDictionary(d => d.Key, d => d.First());
            //foreach (LocationDetailView locationDetail in gridModel.Data)
            //{
            //    Item item = this.itemMgr.GetCacheItem(locationDetail.Item);
            //    locationDetail.ItemDescription = item.Description;
            //    locationDetail.Uom = item.Uom;
            //    locationDetail.LocationName = locationDic[locationDetail.Location].Name;
            //    locationDetail.MaterialsGroup = item.MaterialsGroup;
            //    locationDetail.MaterialsGroupDesc = GetItemCategory(locationDetail.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            //    LocationDetailViewLists.Add(locationDetail);
            //}
            var fileName = string.Format("RealtimeInventoryView.{0}-{1}.xls", searchModel.locationFrom, searchModel.locationTo);
            ExportToXLS<LocationDetailView>(fileName, gridModel.Data.ToList());
        }

        #endregion

        #region private

        private ReportSearchStatementModel PrepareSearchStatement(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            ReportSearchStatementModel reportSearchStatementModel = new ReportSearchStatementModel();
            reportSearchStatementModel.ProcedureName = "USP_Report_RealTimeLocationDet";

            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            string location = string.Empty;
            foreach (var lcoList in locationList)
            {
                if (location == string.Empty)
                {
                    location = lcoList.Code;
                }
                else
                {
                    location += "," + lcoList.Code;
                }
            }

            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            string item = string.Empty;
            foreach (var ite in itemList)
            {
                if (item == string.Empty)
                {
                    item = ite.Code;
                }
                else
                {
                    item += "," + ite.Code;
                }
            }

            SqlParameter[] parameters = new SqlParameter[11];

            parameters[0] = new SqlParameter("@Locations", SqlDbType.VarChar, 8000);
            parameters[0].Value = location;

            parameters[1] = new SqlParameter("@Items", SqlDbType.VarChar, 8000);
            parameters[1].Value = item;

            parameters[2] = new SqlParameter("@SortDesc", SqlDbType.VarChar, 50);
            parameters[2].Value = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            parameters[3] = new SqlParameter("@PageSize", SqlDbType.Int);
            parameters[3].Value = command.PageSize;

            parameters[4] = new SqlParameter("@Page", SqlDbType.Int);
            parameters[4].Value = command.Page;

            parameters[5] = new SqlParameter("@IsSummaryBySAPLoc", SqlDbType.Bit);
            parameters[5].Value = searchModel.TypeLocation == "1" ? true : false;

            parameters[6] = new SqlParameter("@SummaryLevel", SqlDbType.VarChar, 50);
            parameters[6].Value = searchModel.Level;

            parameters[7] = new SqlParameter("@IsGroupByManufactureParty", SqlDbType.Bit);
            parameters[7].Value = searchModel.hideSupper;

            parameters[8] = new SqlParameter("@IsGroupByLotNo", SqlDbType.Bit);
            parameters[8].Value = searchModel.hideLotNo;


            parameters[9] = new SqlParameter("@IsOnlyShowQtyInv", SqlDbType.Bit);
            parameters[9].Value = searchModel.IsOnlyShowQtyInv;

            parameters[10] = new SqlParameter("@ManufactureParty", SqlDbType.VarChar, 50);
            parameters[10].Value = searchModel.ManufactureParty;
            reportSearchStatementModel.Parameters = parameters;

            return reportSearchStatementModel;
        }
        #endregion

        #region ConsignmentInventoryIndex
        [SconitAuthorize(Permissions = "Url_Supplier_Consignment_Inventory")]
        public ActionResult SupplierConsignmentIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Consignment_Inventory")]
        public ActionResult SupplierConsignmentList(GridCommand command, PlanBillSearchModel searchModel)
        {
            TempData["PlanBillSearchModel"] = searchModel;
            //if (this.CheckSearchModelIsNull(searchModel))
            //{
            //    TempData["_AjaxMessage"] = "";
            //}
            //else
            //{
            //    SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            //}
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [SconitAuthorize(Permissions = "Url_Customer_Consignment_Inventory")]
        public ActionResult CustomerConsignmentIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Customer_Consignment_Inventory")]
        public ActionResult CustomerConsignmentList(GridCommand command, PlanBillSearchModel searchModel)
        {
            TempData["PlanBillSearchModel"] = searchModel;
            //if (this.CheckSearchModelIsNull(searchModel))
            //{
            //    TempData["_AjaxMessage"] = "";
            //}
            //else
            //{
            //    SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            //}
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Consignment_Inventory,Url_Customer_Consignment_Inventory")]
        public ActionResult _ConsignmentAjaxList(GridCommand command, PlanBillSearchModel searchModel)
        {
            command.PageSize = int.MaxValue;
            command.Page = 1;

            //if (!this.CheckSearchModelIsNull(searchModel))
            //{
            //    return PartialView(new GridModel(new List<PlanBill>()));
            //}
            SearchStatementModel searchStatementModel = PrepareSearchConsignmentInventoryStatement(command, searchModel);
            var gridData = GetAjaxPageData<PlanBill>(searchStatementModel, command);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var listdata in gridData.Data)
            {
                listdata.ItemDescription = itemMgr.GetCacheItem(listdata.Item).FullDescription;
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup,  itemCategoryList).Description;
            }
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
                MaterialsGroup = g.First().MaterialsGroup,
                MaterialsGroupDesc = g.First().MaterialsGroupDesc,
                Uom = k.Uom,
                CurrentActingQty = g.Sum(q => q.PlanQty) - g.Sum(q => q.ActingQty),
            });
            return PartialView(gridData);
        }

        private SearchStatementModel PrepareSearchConsignmentInventoryStatement(GridCommand command, PlanBillSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            SecurityHelper.AddBillPermissionStatement(ref whereStatement, "p", "Party", com.Sconit.CodeMaster.BillType.Procurement);

            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ReceiptNo", searchModel.ReceiptNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "p", ref whereStatement, param);

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
                whereStatement += " where p.PlanQty>p.ActingQty";
            else
                whereStatement += " and p.PlanQty>p.ActingQty";
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            string selectCountInventoryStatement = "select count(*) from PlanBill as p";
            string selectInventoryStatement = "select p from PlanBill as p";

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountInventoryStatement;
            searchStatementModel.SelectStatement = selectInventoryStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        #endregion
    }
}
