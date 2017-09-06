using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Web.Models;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Service.MRP;
using com.Sconit.Service;
using com.Sconit.Entity.MRP.VIEW;
using Telerik.Web.Mvc.UI;
using System.Text;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.MRP.MD;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

namespace com.Sconit.Web.Controllers.MRP
{
    public class RccpPlanMiController : WebAppBaseController
    {

        public IPlanMgr planMgr { get; set; }
        public IRccpMgr rccpMgr { get; set; }

        //public IGenericMgr genericMgr { get; set; }

        #region  View

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_Load")]
        public ActionResult Load()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_LoadDetail")]
        public ActionResult LoadDetail()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);

            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_FilterCapacity")]
        public ActionResult FilterCapacity()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);

            return View();
        }

        #endregion

        #region Load

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_Load")]
        public string _GetLoadView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = " from RccpMiPlan as r where r.PlanVersion=? and DateType=?";

            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=? ";
                param.Add(dateIndex);
            }
            if (!string.IsNullOrEmpty(dateIndexTo))
            {

                hql += "  and r.DateIndex<=? ";
                param.Add(dateIndexTo);
            }

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=? ";
                param.Add(item);
            }

            IList<RccpMiPlan> rccpMiPlanList = genericMgr.FindAll<RccpMiPlan>(hql, param.ToArray());

            if (rccpMiPlanList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }

            return GetStringRccpMiPlanView(rccpMiPlanList);
        }

        #endregion
        #region Export Load
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_Load")]
        public ActionResult ExportLoad(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetLoadView(dateIndexTo, dateIndex, planVersion, item, dateType);
            return new DownloadFileActionResult(table, "Load.xls");
        }
        #endregion
        #region RccpMiPlanLoad
        private string GetStringRccpMiPlanView(IList<RccpMiPlan> rccpMiPlanList)
        {
            if (rccpMiPlanList.Count() == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var rccpMiPlanGroup1 = (from r in rccpMiPlanList
                                    group r by
                                    new
                                    {
                                        DateIndex = r.DateIndex,
                                        ProductLine = r.ProductLine,
                                        DateType = r.DateType,
                                    } into g
                                    select new
                                    {
                                        DateIndex = g.Key.DateIndex,
                                        ProductLine = g.Key.ProductLine,
                                        HaltTime = g.First().HaltTime / 60,
                                        UpTime = g.First().UpTime / 60,
                                        TrialProduceTime = g.First().TrialProduceTime / 60,
                                        Holiday = g.First().Holiday / 60,
                                        RequireTime = g.Sum(r => r.RequireTime) / 60,
                                        Load = g.Sum(r => r.RequireTime) / g.First().UpTime,
                                        Qty = g.Sum(r => r.Qty) / 1000,
                                        SubQty = g.Sum(r => r.SubQty) / 1000,
                                    }).OrderBy(r => r.DateIndex);

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.MRP.RccpMiPlan.RccpMiPlan_ProductLine);
            str.Append("</th>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.EXT.ControllerLan.Con_Type);
            str.Append("</th>");

            var rccpGroupHead = from p in rccpMiPlanGroup1
                                group p by p.DateIndex into g
                                select new
                                {
                                    DateIndex = g.Key,
                                    List = g
                                };

            foreach (var head in rccpGroupHead)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(head.DateIndex.ToString());
                str.Append("</th>");
            }

            str.Append("</tr>");

            var rccpGroupBody = from p in rccpMiPlanGroup1
                                group p by p.ProductLine into g
                                select new
                                {
                                    ProductLine = g.Key,
                                    List = g
                                };

            int l = 0;
            foreach (var body in rccpGroupBody)
            {
                l++;
                for (int i = 0; i < 8; i++)
                {
                    if (l % 2 == 0)
                    {
                        str.Append("<tr class=\"t-alt\">");
                    }
                    else
                    {
                        str.Append("<tr>");
                    }

                    if (i == 0)
                    {
                        str.Append("<td  rowspan=\"8\">");
                        str.Append(body.ProductLine);
                        str.Append("</td>");
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpMiPlan.RccpMiPlan_UpTime);
                        str.Append("</td>");
                    }
                    else if (i == 1)
                    {
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpMiPlan.RccpMiPlan_RequireTime);
                        str.Append("</td>");
                    }
                    else if (i == 2)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_HaltHours);
                        str.Append("</td>");
                    }
                    else if (i == 3)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_TrailHours);
                        str.Append("</td>");
                    }
                    else if (i == 4)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_HolidayHours);
                        str.Append("</td>");
                    }
                    else if (i == 5)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpMiPlan.RccpMiPlan_Load);
                        str.Append("</td>");
                    }
                    else if (i == 6)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpMiPlan.RccpMiPlan_Qty);
                        str.Append("</td>");
                    }
                    else if (i == 7)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpMiPlan.RccpMiPlan_SubQty);
                        str.Append("</td>");
                    }

                    foreach (var head in rccpGroupHead)
                    {
                        var rccp = body.List.FirstOrDefault(p => p.DateIndex == head.DateIndex);
                        if (rccp != null)
                        {
                            if (i == 0)
                            {
                                str.Append("<td>");
                                str.Append(rccp.UpTime.ToString("0"));
                                str.Append("</td>");
                            }
                            else if (i == 1)
                            {
                                str.Append("<td>");
                                str.Append(rccp.RequireTime.ToString("0"));
                                str.Append("</td>");
                            }
                            else if (i == 2)
                            {
                                str.Append("<td>");
                                str.Append(rccp.HaltTime.ToString("0"));
                                str.Append("</td>");
                            }
                            else if (i == 3)
                            {
                                str.Append("<td>");
                                str.Append(rccp.TrialProduceTime.ToString("0"));
                                str.Append("</td>");
                            }
                            else if (i == 4)
                            {
                                str.Append("<td>");
                                str.Append(rccp.Holiday.ToString("0"));
                                str.Append("</td>");
                            }
                            else if (i == 5)
                            {
                                str.Append("<td>");
                                str.Append(rccp.Load.ToString("P"));
                                str.Append("</td>");
                            }
                            else if (i == 6)
                            {
                                str.Append("<td>");
                                str.Append(rccp.Qty.ToString("0"));
                                str.Append("</td>");
                            }
                            else if (i == 7)
                            {
                                str.Append("<td>");
                                str.Append(rccp.SubQty.ToString("0"));
                                str.Append("</td>");
                            }
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append("0");
                            str.Append("</td>");
                        }
                    }
                    str.Append("</tr>");
                }
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }
        #endregion
        #region FilterCapacity
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_FilterCapacity")]
        public string _GetFilterCapacityView(string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            IList<object> param = new List<object>();
            string startDateIndex = string.Empty;
            string endDateIndex = string.Empty;
            int tableColumnCount;
            if (dateType == com.Sconit.CodeMaster.TimeUnit.Week)
            {
                startDateIndex = dateIndex;
                endDateIndex = Utility.DateTimeHelper.GetWeekOfYear((Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex).AddDays(7 * 16)));
            }
            else if (dateType == com.Sconit.CodeMaster.TimeUnit.Month)
            {
                startDateIndex = dateIndex;
                endDateIndex = DateTime.Parse(dateIndex + "-01").AddMonths(12).ToString("yyyy-MM");
            }

            SqlParameter[] sqlParams = new SqlParameter[6];
            sqlParams[0] = new SqlParameter("@PlanVersion", planVersion);
            sqlParams[1] = new SqlParameter("@DateType", dateType);
            sqlParams[2] = new SqlParameter("@startDateIndex", startDateIndex);
            sqlParams[3] = new SqlParameter("@endDateIndex", endDateIndex);
            sqlParams[4] = new SqlParameter("@Item", item);
            sqlParams[5] = new SqlParameter("@ProductLine", productLine);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Report_GetFilterCapacity", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head
            for (int i = 0; i < tableColumnCount; i++)
            {
                str.Append("<th>");
                str.Append(ds.Tables[1].Columns[i].ColumnName);
                str.Append("</th>");
            }
            str.Append("</tr></thead><tbody>");
            #endregion
            int l = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                l++;
                trcss = "";
                if (l % 2 == 0)
                {
                    trcss = "t-alt";
                }
                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");
                for (int j = 0; j < tableColumnCount; j++)
                {
                    str.Append("<td>");
                    if (ds.Tables[1].Rows[i][0].ToString() == Resources.EXT.ControllerLan.Con_LoadRate && j > 1)
                    {
                        str.Append(ds.Tables[1].Rows[i][j] + "%");
                    }
                    else
                    {
                        str.Append(ds.Tables[1].Rows[i][j]);
                    }
                    str.Append("</td>");
                }

                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            return Resources.EXT.ControllerLan.Con_UomCars+" <br />"+str.ToString();

        }
        #endregion
        #region Export FilterCapacity
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_FilterCapacity")]
        public ActionResult ExportFilterCapacity(string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            var table = _GetFilterCapacityView(dateIndex, planVersion, item, dateType, productLine);
            return new DownloadFileActionResult(table, "FilterCapacity.xls");
        }
        #endregion

        #region LoadDetail
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_LoadDetail")]
        public string _GetLoadDetailView(string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            IList<object> param = new List<object>();
            string startDateIndex = string.Empty;
            string endDateIndex = string.Empty;
            if (dateType == com.Sconit.CodeMaster.TimeUnit.Week)
            {
                startDateIndex = dateIndex;
                endDateIndex = Utility.DateTimeHelper.GetWeekOfYear((Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex).AddDays(7 * 16)));
            }
            else if (dateType == com.Sconit.CodeMaster.TimeUnit.Month)
            {
                startDateIndex = dateIndex;
                endDateIndex = DateTime.Parse(dateIndex + "-01").AddMonths(12).ToString("yyyy-MM");
            }

            string hql = " from RccpMiPlan as r where r.PlanVersion=? and DateType=? and  DateIndex between ? and ? ";

            param.Add(planVersion);
            param.Add(dateType);
            param.Add(startDateIndex);
            param.Add(endDateIndex);

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=? ";
                param.Add(item);
            }
            if (!string.IsNullOrEmpty(productLine))
            {
                hql += "  and r.ProductLine=? ";
                param.Add(productLine);
            }
            IList<RccpMiPlan> rccpMiPlanList = genericMgr.FindAll<RccpMiPlan>(hql, param.ToArray());

            if (rccpMiPlanList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            return GetStringLoadDetailView(rccpMiPlanList);
        }

        private string GetStringLoadDetailView(IList<RccpMiPlan> rccpMiPlanList)
        {
            if (rccpMiPlanList.Count() == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var rccpMiPlanGroupByDateIndex = (from r in rccpMiPlanList
                                              group r by
                                              new
                                              {
                                                  DateIndex = r.DateIndex,
                                              } into g
                                              select new
                                              {
                                                  DateIndex = g.Key.DateIndex,
                                                  List = g,
                                              }).OrderBy(r => r.DateIndex);

            IList<Item> itemList = genericMgr.FindAllIn<Item>(" from Item where Code in(? ", rccpMiPlanList.Select(r => r.Item).Distinct());
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            str.Append("<th  style=\"text-align:center;min-width: 40px;\"  rowspan=\"2\"   >");
            str.Append(Resources.MRP.RccpMiPlan.RccpMiPlan_ProductLine);
            str.Append("</th>");

            str.Append("<th  style=\"text-align:centermin-width: 50px;\"  rowspan=\"2\"   >");
            str.Append(Resources.MRP.RccpMiPlan.RccpMiPlan_Item);
            str.Append("</th>");

            str.Append("<th  style=\"text-align:center;min-width: 70px;\"  rowspan=\"2\"   >");
            str.Append(Resources.MRP.RccpMiPlan.RccpMiPlan_ItemDescription);
            str.Append("</th>");

            foreach (var rccpMiPlanGroupBy in rccpMiPlanGroupByDateIndex)
            {
                str.Append("<th  style=\"text-align:center\"  colspan=\"3\" >");
                str.Append(rccpMiPlanGroupBy.DateIndex);
                str.Append("</th>");
            }
            str.Append("</tr>");
            str.Append("<tr>");

            foreach (var rccpMiPlanGroupBy in rccpMiPlanGroupByDateIndex)
            {
                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_ISM);
                str.Append("</th>");
                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_SelfMade);
                str.Append("</th>");
                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_Consignment);
                str.Append("</th>");
            }

            str.Append("</tr>");
            var rccpMiPlanGroupByItemProductLine = (from r in rccpMiPlanList
                                                    group r by
                                                    new
                                                    {
                                                        ProductLine = r.ProductLine,
                                                        Item = r.Item,
                                                    } into g
                                                    select new
                                                    {
                                                        ProductLine = g.Key.ProductLine,
                                                        Item = g.Key.Item,
                                                        List = g,
                                                    });
            int l = 0;
            foreach (var rccpMiPlanItemProductLine in rccpMiPlanGroupByItemProductLine)
            {
                l++;

                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");

                }
                str.Append("<td>");
                str.Append(rccpMiPlanItemProductLine.ProductLine);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(rccpMiPlanItemProductLine.Item);
                str.Append("</td>");

                var item = itemList.First(i => i.Code == rccpMiPlanItemProductLine.Item);
                str.Append("<td>");
                str.Append(item.Description);
                str.Append("</td>");

                #region
                foreach (var rccpMiPlanDateIndex in rccpMiPlanGroupByDateIndex)
                {
                    var firstrccpMiPlan = rccpMiPlanDateIndex.List.FirstOrDefault(r => r.ProductLine == rccpMiPlanItemProductLine.ProductLine & r.Item == rccpMiPlanItemProductLine.Item
                        & rccpMiPlanItemProductLine.List.FirstOrDefault(p => p.DateIndex == rccpMiPlanDateIndex.DateIndex) != null);
                    if (firstrccpMiPlan != null)
                    {
                        str.Append("<td>");
                        str.Append((firstrccpMiPlan.RequireTime / 60).ToString("0.##"));
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append((firstrccpMiPlan.Qty / 1000).ToString("0.##"));
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append((firstrccpMiPlan.SubQty / 1000).ToString("0.##"));
                        str.Append("</td>");
                    }
                    else
                    {
                        str.Append("<td>");
                        str.Append("0");
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append("0");
                        str.Append("</td>");


                        str.Append("<td>");
                        str.Append("0");
                        str.Append("</td>");
                    }
                }
                #endregion
                str.Append("</tr>");
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return Resources.EXT.ControllerLan.Con_UomISMHourSelfMadeTConsignmentT+" <br />" + str.ToString();
        }
        #endregion
        #region Export LoadDetail
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanMi_LoadDetail")]
        public ActionResult ExportLoadDetail(string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            var table = _GetLoadDetailView(dateIndex, planVersion, item, dateType, productLine);
            return new DownloadFileActionResult(table, "LoadDetail.xls");
        }
        #endregion
    }
}
