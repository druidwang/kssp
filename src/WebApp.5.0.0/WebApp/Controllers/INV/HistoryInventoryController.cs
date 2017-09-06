using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.PrintModel.INV;
using com.Sconit.Service;
using com.Sconit.Utility.Report;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Utility.Report.Operator;
using com.Sconit.Web.Models.ReportModels;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Service.Impl;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Web.Controllers.INV
{
    public class HistoryInventoryController : ReportBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public ILocationDetailMgr IocationMgr { get; set; }
        //public IReportGen reportGen { get; set; }

        public ActionResult Index()
        {
            HistoryInventorySearchModel serch = new HistoryInventorySearchModel();
            serch.TypeLocation = "0";
            TempData["Display"] = "0";
            TempData["HistoryInventorySearchModel"] = serch;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Menu.History.Inventory")]
        public ActionResult List(GridCommand command, HistoryInventorySearchModel searchModel)
        {
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            if (!string.IsNullOrEmpty(searchModel.SAPLocation))
                TempData["SAPLocation"] = searchModel.SAPLocation;

            TempData["Display"] = searchModel.Level;
            TempData["HistoryInventorySearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);

            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (string.IsNullOrEmpty(ViewBag.Location))
                ViewBag.Location = Resources.View.LocationDetailView.LocationDetailView_Location;
            else
                ViewBag.Location = ViewBag.Location;



            if (string.IsNullOrEmpty(searchModel.HistoryDate.ToString()))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_TimeCanNotBeEmpty);
                return View();
            }

            if (locationList.Count > 200)
            {
                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo))
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ItemCodeCanNotBeEmpty);
                    return View();
                }
            }
            if (itemList.Count > 200)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PartExceedTwoHundred);
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
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_SAPLocationCanNotBeEmpty);
                    return View();
                }
            }

         

            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Menu.History.Inventory")]
        public ActionResult _AjaxList(GridCommand command, HistoryInventorySearchModel searchModel)
        {
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            if (ReturnZeroRow(command, searchModel))
            {
                return PartialView(new GridModel(new List<HistoryInventory>()));
            }
            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);

            GridModel<HistoryInventory> gridModel = GetHistoryInvAjaxPageData<HistoryInventory>(reportSearchStatementModel);
           //IList<HistoryInventory> gridModel = IocationMgr.GetHistoryLocationDetails(locationCode, list, searchModel.HistoryDate, SortDescriptors, command.PageSize, command.Page);
          
            return PartialView(gridModel);
        }
        #region 导出历史库存报表
        private bool ReturnZeroRow(GridCommand command, HistoryInventorySearchModel searchModel)
        {
            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (locationList.Count > 200)
            {
                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo))
                {
                    return true;
                }
            }
            if (itemList.Count > 200)
            {
                return true;
            }
            if (string.IsNullOrEmpty(searchModel.HistoryDate.ToString()))
            {
                return true;
            }

            if (searchModel.TypeLocation == "0")
            {
                if (string.IsNullOrEmpty(searchModel.Level))
                {
                    return true;
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo) && string.IsNullOrEmpty(searchModel.locationFrom) && string.IsNullOrEmpty(searchModel.locationTo)
                     && string.IsNullOrEmpty(searchModel.regionFrom) && string.IsNullOrEmpty(searchModel.regionTo)
                   )
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
            return false;
        }
        [SconitAuthorize(Permissions = "Menu.History.Inventory")]
        public void ExportXLS(HistoryInventorySearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = ReturnZeroRow(command, searchModel) ? 0 : value;
            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);

            GridModel<HistoryInventory> gridModel = GetHistoryInvAjaxPageData<HistoryInventory>(reportSearchStatementModel);
            ExportToXLS<HistoryInventory>("HistoryInventory.xls", gridModel.Data.ToList());
        }

        #endregion
        private ReportSearchStatementModel PrepareSearchStatement(GridCommand command, HistoryInventorySearchModel searchModel)
        {
            
            ReportSearchStatementModel reportSearchStatementModel = new ReportSearchStatementModel();
            reportSearchStatementModel.ProcedureName = "USP_Report_GetHistoryInv";


            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            string locations = string.Empty;
            foreach (var lcoList in locationList)
            {
                if (locations == string.Empty)
                {
                    locations = lcoList.Code;
                }
                else
                {
                    locations += "," + lcoList.Code;
                }
            }


            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            string items = string.Empty;
            foreach (var ite in itemList)
            {
                if (items == string.Empty)
                {
                    items = ite.Code;
                }
                else
                {
                    items += "," + ite.Code;
                }
            }



            SqlParameter[] parm = new SqlParameter[8];

            parm[0] = new SqlParameter("@Locations", SqlDbType.VarChar, 50);
            parm[0].Value = locations;

          
            parm[1] = new SqlParameter("@Items", SqlDbType.VarChar, 4000);
            parm[1].Value = items;

            parm[2] = new SqlParameter("@HistoryData", SqlDbType.DateTime);
            parm[2].Value = searchModel.HistoryDate;

            parm[3] = new SqlParameter("@SortDesc", SqlDbType.VarChar, 100);
            parm[3].Value = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            parm[4] = new SqlParameter("@PageSize", SqlDbType.Int);
            parm[4].Value =command.PageSize;

            parm[5] = new SqlParameter("@Page", SqlDbType.Int);
            parm[5].Value = command.Page;

            parm[6] = new SqlParameter("@IsSummaryBySAPLoc", SqlDbType.Bit);
            parm[6].Value = searchModel.TypeLocation == "1" ? true : false; ;


            parm[7] = new SqlParameter("@SummaryLevel", SqlDbType.VarChar, 50);
            parm[7].Value = searchModel.Level;
            
            reportSearchStatementModel.Parameters = parm;

            return reportSearchStatementModel;
        }

    }
}
