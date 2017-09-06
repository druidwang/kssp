using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using com.Sconit.Entity.VIEW;
using System.Text;
using com.Sconit.Utility;
using System.Data;
using System.Data.SqlClient;

namespace com.Sconit.Web.Controllers.Report
{
    public class InventoryController : WebAppBaseController
    {
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryMonitor")]
        public ActionResult ExportInventoryMonitor(string location, string itemCategory, string materialsGroup,string item)
        {
            var table = _GetInventoryMonitor(location, itemCategory, materialsGroup,item);
            return new DownloadFileActionResult(table, "InventoryMonitor.xls");
        }
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryMonitor")]
        public ActionResult InventoryMonitor()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryLotNo")]
        public ActionResult InventoryLotNo()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Rep_SmartReport")]
        public ActionResult SmartReport()
        {
            return View();
        }
        //ItemCategory = 0,//物料类型
        //MaterialsGroup = 5,//物料组
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryMonitor")]
        public string _GetInventoryMonitor(string location, string itemCategory, string materialsGroup,string item)
        {
            SqlParameter[] sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter("@Location", location);
            sqlParams[1] = new SqlParameter("@Item", item);
            sqlParams[2] = new SqlParameter("@ItemCategory", itemCategory);
            sqlParams[3] = new SqlParameter("@MaterialsGroup", materialsGroup);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Report_GetInventoryMonitor", sqlParams);

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            //库位	物料	描述	单位	

            #region Head
            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_RowCount);
            str.Append("</th>");
            if (!string.IsNullOrWhiteSpace(location))
            {
                str.Append("<th>");
                str.Append(Resources.EXT.ControllerLan.Con_Location);
                str.Append("</th>");
            }
            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Item);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Description);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_MaterialGroup);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_MaterialGroup);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Uom);
            str.Append("</th>");
            //总库存	安全库存 无效库存,合格数,不合格数,冻结数 寄售库存 安全库存	最大库存	在途	安全差额	最大差额	百分比
            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_TotalInventory);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_InvalidInventory);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.EXT.ControllerLan.Con_QualifiedCount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_InqualifiedCount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_FreezeCount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_ConsignmentInventory);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_SafeInventory);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_MaxInventory);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_InProcess);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_SafeGapRate);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_MaxGapRate);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_PercentRate);
            str.Append("</th>");

            str.Append("</tr></thead><tbody>");
            #endregion

            int l = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //(string)dr.ItemArray[0], (decimal)dr.ItemArray[1]
                l++;

                string trcss = string.Empty;
                //总库存超过最大库存，背景显红色
                string maxStokWarmingColor = string.Empty;
                if ((decimal)dr.ItemArray[6] > (decimal)dr.ItemArray[13])
                {
                    maxStokWarmingColor = "bgcolor=\"#FF9797\" color=\"black\"";
                }
                if ((decimal)dr.ItemArray[15] < 0)
                {
                    trcss = "mrp-warning";
                }
                else if ((decimal)dr.ItemArray[16] < 0)
                {
                    trcss = "WarningColor_Yellow";
                }
                else
                {
                    if (l % 2 == 0)
                    {
                        trcss = "t-alt";
                    }
                }
                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");

                str.Append("<td>");
                str.Append(l);
                str.Append("</td>");
                if (!string.IsNullOrWhiteSpace(location))
                {
                    str.Append("<td>");
                    str.Append(dr.ItemArray[0]);
                    str.Append("</td>");
                }
                str.Append("<td>");
                str.Append(dr.ItemArray[1]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[2]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[3]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[4]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[5]);
                str.Append("</td>");

                str.Append("<td " + maxStokWarmingColor + ">");
                str.Append(dr.ItemArray[6]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[7]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[8]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[9]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[10]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[11]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[12]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[13]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[14]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[15]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[16]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[17]);
                str.Append("</td>");

                str.Append("</tr>");
            }
            str.Append("</tbody></table>");
            return str.ToString();

        }
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryIp")]
        public ActionResult InventoryIp()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryIp")]
        public ActionResult Export(string locationFrom, string locationTo, string itemFrom, string itemTo, string flow, int ipReportType)
        {
            var table = _GetIpReport(locationFrom, locationTo, itemFrom, itemTo, flow, ipReportType);
            return new DownloadFileActionResult(table, "InventoryIpReport.xls");
        }
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryIp")]
        public string _GetIpReport(string locationFrom, string locationTo, string itemFrom, string itemTo, string flow, int ipReportType)
        {
            if (string.IsNullOrWhiteSpace(locationTo))
            {
                locationTo = locationFrom;
            }
            if (string.IsNullOrWhiteSpace(itemTo))
            {
                itemTo = itemFrom;
            }

            SqlParameter[] sqlParams = new SqlParameter[6];
            sqlParams[0] = new SqlParameter("@LocationFrom", locationFrom);
            sqlParams[1] = new SqlParameter("@LocationTo", locationTo);
            sqlParams[2] = new SqlParameter("@ItemFrom", itemFrom);
            sqlParams[3] = new SqlParameter("@ItemTo", itemTo);
            sqlParams[4] = new SqlParameter("@Flow", flow);
            sqlParams[5] = new SqlParameter("@IpReportType", ipReportType);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Report_IPInv", sqlParams);

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            //库位	物料	描述	单位	

            #region Head
            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_RowCount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_ItemCode);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Description);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_MaterialGroup);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_MaterialGroup);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Uom);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Flow);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_LocationFrom);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_LocationTo);
            str.Append("</th>");

            //str.Append("<th>");
            //str.Append("待验");
            //str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Inqualified);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Qualified);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_TotalCount);
            str.Append("</th>");
            str.Append("</tr></thead><tbody>");
            #endregion

            int l = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //(string)dr.ItemArray[0], (decimal)dr.ItemArray[1]
                l++;

                string trcss = string.Empty;
                if (l % 2 == 0)
                {
                    trcss = "t-alt";
                }

                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");

                str.Append("<td>");
                str.Append(l);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[0]);
                str.Append("</td>");
                
                str.Append("<td>");
                str.Append(dr.ItemArray[1]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[2]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[3]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[4]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[5]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[6]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[7]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[8]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[9]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[10]);
                str.Append("</td>");

                str.Append("</tr>");
            }
            str.Append("</tbody></table>");
            return str.ToString();

        }
        #region LotNoInvReport
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryLotNo")]
        public string _GetInventoryLotNoReportView(string location, string itemFrom, string itemTo, int sortType, string manufactureParty)
        {
            string ProcedureName = "USP_Report_GetLotNoInventory";
            SqlParameter[] sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter("@Location", location);
            sqlParams[1] = new SqlParameter("@ItemFrom", itemFrom);
            sqlParams[2] = new SqlParameter("@ItemTo", itemTo);
            sqlParams[3] = new SqlParameter("@SortType", sortType);
            sqlParams[4] = new SqlParameter("@ManufactureParty", manufactureParty);
            return GetTableHtmlByStoredProcedure(ProcedureName, sqlParams);
        }
        #endregion
        #region Export LotNoInvReport
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryLotNo")]
        public ActionResult ExportInventoryLotNoReport(string location, string itemFrom, string itemTo, int sortType, string manufactureParty)
        {
            var table = _GetInventoryLotNoReportView(location, itemFrom, itemTo, sortType, manufactureParty);
            return new DownloadFileActionResult(table, "InventoryLotNoReport.xls");
        }
        #endregion
        #region test report
        //public string _GetTestString()
        //{
        //    
        //}
        #endregion
    }
}
