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
using System;
using com.Sconit.Entity.SYS;
using System.Text;

namespace com.Sconit.Web.Controllers.INV
{
    public class LibraryAgeStatementsController : ReportBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public ILocationDetailMgr IocationMgr { get; set; }
        //public IReportGen reportGen { get; set; }

        public ActionResult Index()
        {
            InventoryAgeSearchModel serch = new InventoryAgeSearchModel();
            serch.TypeLocation = "0";
            TempData["Display"] = "0";
            TempData["InventoryAgeSearchModel"] = serch;
            
            //初始化数据
            ViewBag.Range1 = 7;
            ViewBag.Range2 = 14;
            ViewBag.Range3 = 30;
            ViewBag.Range4 = 60;
            ViewBag.Range5 = 90;
            ViewBag.Range6 = 180;
            ViewBag.Range7 = 360;
            ViewBag.Range8 = 720;
            ViewBag.Range9 = 1080;
            ViewBag.Range10 = 1440;
            ViewBag.Range11 = 1800;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Menu.LibraryAge.Statements")]
        public ActionResult List(GridCommand command, InventoryAgeSearchModel searchModel)
        {
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            ViewBag.Range1 = searchModel.Range1;
            ViewBag.Range2 = searchModel.Range2;
            ViewBag.Range3 = searchModel.Range3;
            ViewBag.Range4 = searchModel.Range4;
            ViewBag.Range5 = searchModel.Range5;
            ViewBag.Range6 = searchModel.Range6;
            ViewBag.Range7 = searchModel.Range7;
            ViewBag.Range8 = searchModel.Range8;
            ViewBag.Range9 = searchModel.Range9;
            ViewBag.Range10 = searchModel.Range10;
            ViewBag.Range11 = searchModel.Range11;
            TempData["Display"] = searchModel.Level;
            TempData["InventoryAgeSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation,searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (string.IsNullOrEmpty(ViewBag.Location))
                ViewBag.Location = Resources.View.LocationDetailView.LocationDetailView_Location;
            else
                ViewBag.Location = ViewBag.Location;

            if (locationList.Count > 200)
            {
                if (string.IsNullOrEmpty(searchModel.itemFrom) || string.IsNullOrEmpty(searchModel.itemTo))
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
                    && string.IsNullOrEmpty(searchModel.regionFrom) && string.IsNullOrEmpty(searchModel.regionTo))
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
        #region export libraryAgeStatements
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Menu.LibraryAge.Statements")]
        public ActionResult Export(InventoryAgeSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            IList<Location> locationList = GetReportLocations(null, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (locationList.Count > 200)
            {
                if (string.IsNullOrEmpty(searchModel.itemFrom) || string.IsNullOrEmpty(searchModel.itemTo))
                {
                    command.PageSize = 0;
                }
            }
            if (itemList.Count > 200)
            {
                command.PageSize = 0;
            }

            if (searchModel.TypeLocation == "0")
            {
                if (string.IsNullOrEmpty(searchModel.Level))
                {
                    command.PageSize = 0;
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo) && string.IsNullOrEmpty(searchModel.locationFrom) && string.IsNullOrEmpty(searchModel.locationTo)
                     && string.IsNullOrEmpty(searchModel.regionFrom) && string.IsNullOrEmpty(searchModel.regionTo))
                {
                    command.PageSize = 0;
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.itemTo))
                    {
                        command.PageSize = 0;
                    }
                }
                if (string.IsNullOrEmpty(searchModel.locationFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.locationTo))
                    {
                        command.PageSize = 0;
                    }
                }

                if (string.IsNullOrEmpty(searchModel.regionFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.regionTo))
                    {
                        command.PageSize = 0;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(searchModel.SAPLocation))
                {
                    command.PageSize = 0;
                }
            }
            var list = new GridModel<InventoryAge>();
            if (command.PageSize != 0)
            {
                ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);
                list = GetInventoryAgeAjaxPageData<InventoryAge>(reportSearchStatementModel);
                var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
                foreach (var listdata in list.Data)
                {
                    listdata.ItemFromDesc = itemMgr.GetCacheItem(listdata.Item).Description;
                    listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                    listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup,  itemCategoryList).Description;

                }
            }

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            #region Head
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_ItemCode+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_ItemDescription+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_MaterialGroupDescription+"</th>");
            str.Append("<th>" + Resources.EXT.ControllerLan.Con_Location + "</th>");
            str.Append("<th>0-" + searchModel.Range1 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range1 + "-" + searchModel.Range2 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range2 + "-" + searchModel.Range3 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range3 + "-" + searchModel.Range4 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range4 + "-" + searchModel.Range5 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range5 + "-" + searchModel.Range6 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range6 + "-" + searchModel.Range7 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range7 + "-" + searchModel.Range8 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range8 + "-" + searchModel.Range9 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range9 + "-" + searchModel.Range10 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range10 + "-" + searchModel.Range11 + Resources.EXT.ControllerLan.Con_DayWord+"</th>");
            str.Append("<th>" + searchModel.Range11 + "-" + Resources.EXT.ControllerLan.Con_LaterDate+"</th>");
            str.Append("</tr></thead><tbody>");
            #endregion
            foreach (var las in list.Data)
            {
                str.Append("<tr>");
                str.Append("<td>");
                str.Append(las.Item);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.ItemFromDesc);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.MaterialsGroupDesc);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Location);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range0);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range1);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range2);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range3);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range4);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range5);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range6);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range7);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range8);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range9);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range10);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(las.Range11);
                str.Append("</td>");

                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            //ExportToXLS<InventoryAge>("libraryAgeStatements.xls", list.Data.ToList());
            return new DownloadFileActionResult(str.ToString(), "LibraryAgeStatement.xls");
        }

        #endregion
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Menu.LibraryAge.Statements")]
        public ActionResult _AjaxList(GridCommand command, InventoryAgeSearchModel searchModel)
        {
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            IList<Location> locationList = GetReportLocations(null, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (locationList.Count > 200)
            {
                if (string.IsNullOrEmpty(searchModel.itemFrom) || string.IsNullOrEmpty(searchModel.itemTo))
                {
                    return PartialView(new GridModel(new List<InventoryAge>()));
                }
            }
            if (itemList.Count > 200)
            {
                return PartialView(new GridModel(new List<InventoryAge>()));
            }

            if (searchModel.TypeLocation == "0")
            {
                if (string.IsNullOrEmpty(searchModel.Level))
                {
                    return PartialView(new GridModel(new List<InventoryAge>()));
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo) && string.IsNullOrEmpty(searchModel.locationFrom) && string.IsNullOrEmpty(searchModel.locationTo)
                     && string.IsNullOrEmpty(searchModel.regionFrom) && string.IsNullOrEmpty(searchModel.regionTo))
                {
                    return PartialView(new GridModel(new List<InventoryAge>()));
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.itemTo))
                    {
                        return PartialView(new GridModel(new List<InventoryAge>()));
                    }
                }
                if (string.IsNullOrEmpty(searchModel.locationFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.locationTo))
                    {
                        return PartialView(new GridModel(new List<InventoryAge>()));
                    }
                }

                if (string.IsNullOrEmpty(searchModel.regionFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.regionTo))
                    {
                        return PartialView(new GridModel(new List<InventoryAge>()));
                    }
                }


            }
            else
            {
                if (string.IsNullOrEmpty(searchModel.SAPLocation))
                {
                    return PartialView(new GridModel(new List<InventoryAge>()));
                }
            }

          


            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);
            var list = GetInventoryAgeAjaxPageData<InventoryAge>(reportSearchStatementModel);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();

            foreach (var listdata in list.Data)
            {
                listdata.ItemFromDesc = itemMgr.GetCacheItem(listdata.Item).FullDescription;    
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup,  itemCategoryList).Description;
            }
            return PartialView(list);
        }

        private ReportSearchStatementModel PrepareSearchStatement(GridCommand command, InventoryAgeSearchModel searchModel)
        {
            ReportSearchStatementModel reportSearchStatementModel = new ReportSearchStatementModel();
            reportSearchStatementModel.ProcedureName = "USP_Report_InventoryAge";

            IList<Location> locationList = GetReportLocations(null
                ,searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
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


            SqlParameter[] parameters = new SqlParameter[19];

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

            parameters[5] = new SqlParameter("@SummaryLevel", SqlDbType.VarChar, 50);
            parameters[5].Value = searchModel.Level;

            parameters[6] = new SqlParameter("@Range1", SqlDbType.Int);
            parameters[6].Value = searchModel.Range1;

            parameters[7] = new SqlParameter("@Range2", SqlDbType.Int);
            parameters[7].Value = searchModel.Range2;

            parameters[8] = new SqlParameter("@Range3", SqlDbType.Int);
            parameters[8].Value = searchModel.Range3;

            parameters[9] = new SqlParameter("@Range4", SqlDbType.Int);
            parameters[9].Value = searchModel.Range4;


            parameters[10] = new SqlParameter("@Range5", SqlDbType.Int);
            parameters[10].Value = searchModel.Range5;


            parameters[11] = new SqlParameter("@Range6", SqlDbType.Int);
            parameters[11].Value = searchModel.Range6;

            parameters[12] = new SqlParameter("@Range7", SqlDbType.Int);
            parameters[12].Value = searchModel.Range7;

            parameters[13] = new SqlParameter("@Range8", SqlDbType.Int);
            parameters[13].Value = searchModel.Range8;

            parameters[14] = new SqlParameter("@Range9", SqlDbType.Int);
            parameters[14].Value = searchModel.Range9;


            parameters[15] = new SqlParameter("@Range10", SqlDbType.Int);
            parameters[15].Value = searchModel.Range10;

            parameters[16] = new SqlParameter("@Range11", SqlDbType.Int);
            parameters[16].Value = searchModel.Range11;

            parameters[17] = new SqlParameter("@IsSummaryBySAPLoc", SqlDbType.Bit);
            parameters[17].Value = searchModel.TypeLocation == "1" ? true : false; ;

         
            reportSearchStatementModel.Parameters = parameters;

            return reportSearchStatementModel;
        }
    }
}
