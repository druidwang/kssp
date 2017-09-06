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

namespace com.Sconit.Web.Controllers.MRP
{
    public class RccpPlanExController : WebAppBaseController
    {

        public IPlanMgr planMgr { get; set; }
        public IRccpMgr rccpMgr { get; set; }

        //public IGenericMgr genericMgr { get; set; }

        #region  View

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineLoad")]
        public ActionResult ProdLineLoad()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineQty")]
        public ActionResult ProdLineQty()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineSpeed")]
        public ActionResult ProdLineSpeed()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineScrapPercentage")]
        public ActionResult ProdLineScrapPercentage()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemTime")]
        public ActionResult ItemTime()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemLoad")]
        public ActionResult ItemLoad()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemQty")]
        public ActionResult ItemQty()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifyQty")]
        public ActionResult ClassifyLoad()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifySpeed")]
        public ActionResult ClassifySpeed()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifyScrapPercentage")]
        public ActionResult ClassifyScrapPercentage()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifyQty")]
        public ActionResult ClassifyQty()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_AllotSection")]
        public ActionResult AllotSection()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }
        #endregion

        #region ProdLineLoad
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineLoad")]
        public string _GetRccpTransView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";

            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=? ";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=? ";
                param.Add(item);
            }

            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());

            //var rccpTransGroupAvg=from 
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByProdLineView> rccpExGroupByProdLineViewList = planMgr.GetExRccpViewGroupByProdLineLoad(rccpTransGroupList);

            return GetStringRccpExGroupByProdLineView(rccpExGroupByProdLineViewList, true, true);
        }
        #endregion
        #region Export prodlLineLoad
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineLoad")]
        public ActionResult ExportProdLineLoad(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetRccpTransView(dateIndexTo, dateIndex, planVersion, item, dateType );
            return new DownloadFileActionResult(table, "ProdLineLoad.xls");
        }
        #endregion
        #region ProdLineQty
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineQty")]
        public string _GetRccpGroupByProdLineQtyView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }


            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByProdLineView> rccpExGroupByProdLineViewList = planMgr.GetExRccpViewGroupByProdLineQty(rccpTransGroupList);

            return Resources.EXT.ControllerLan.Con_UomTenThousandM+"<br />" + GetStringRccpExGroupByProdLineView(rccpExGroupByProdLineViewList, false);
        }


        #endregion

        #region Export ProdLineQty
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineQty")]
        public ActionResult ExportProdLineQty(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetRccpGroupByProdLineQtyView(dateIndexTo, dateIndex, planVersion, item, dateType);
            return new DownloadFileActionResult(table, "ProdLineQty.xls");
        }
        #endregion
        #region ProdLineSpeed

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineSpeed")]
        public string _GetExRccpViewGroupByProdLineSpeed(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }

            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByProdLineView> rccpExGroupByProdLineViewList = planMgr.GetExRccpViewGroupByProdLineSpeed(rccpTransGroupList);

            return GetStringRccpExGroupByProdLineView(rccpExGroupByProdLineViewList, false);
        }


        #endregion
        #region Export ProdLineSpeed
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineSpeed")]
        public ActionResult ExportProdLineSpeed(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetExRccpViewGroupByProdLineSpeed(dateIndexTo, dateIndex, planVersion, item, dateType);
            return new DownloadFileActionResult(table, "ProdLineSpeed.xls");
        }
        #endregion
        #region ProdLineScrapPercentage

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineScrapPercentage")]
        public string _GetExRccpViewGroupByProdLineScrapPercentage(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }


            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByProdLineView> rccpExGroupByProdLineViewList = planMgr.GetExRccpViewGroupByProdLineScrapPercentage(rccpTransGroupList);

            return GetStringRccpExGroupByProdLineView(rccpExGroupByProdLineViewList, true);
        }
        #endregion
        #region Export ProdLineScrapPercentage
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ProdLineScrapPercentage")]
        public ActionResult ExportProdLineScrapPercentage(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetExRccpViewGroupByProdLineScrapPercentage(dateIndexTo, dateIndex, planVersion, item, dateType);
            return new DownloadFileActionResult(table, "ProdLineScrapPercentage.xls");
        }
        #endregion
        #region RccpExGroupByProdLineView
        private string GetStringRccpExGroupByProdLineView(IEnumerable<RccpExGroupByProdLineView> rccpExGroupByProdLineViewList, bool isPercentage, bool isCss = false)
        {
            if (rccpExGroupByProdLineViewList.Count() == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var rccpExGroupByDateIndex = (from r in rccpExGroupByProdLineViewList
                                          group r by
                                          new
                                          {
                                              DateIndex = r.DateIndex,
                                          } into g
                                          select new
                                          {
                                              DateIndex = g.Key.DateIndex,
                                              List = g
                                          }).OrderBy(r => r.DateIndex);


            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.MRP.RccpTrans.RccpTrans_ProductLine);
            str.Append("</th>");


            foreach (var rccpExDateIndex in rccpExGroupByDateIndex)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(rccpExDateIndex.DateIndex);
                str.Append("</th>");
            }

            str.Append("</tr>");

            var rccpExGroupByProductLine = from r in rccpExGroupByProdLineViewList
                                           group r by
                                           new
                                           {
                                               ProductLine = r.ProductLine,
                                           } into g
                                           select new
                                           {
                                               ProductLine = g.Key.ProductLine,
                                           };

            int l = 0;
            foreach (var rccpExProductLine in rccpExGroupByProductLine)
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
                str.Append(rccpExProductLine.ProductLine);
                str.Append("</td>");
                #region
                foreach (var rccpExDateIndex in rccpExGroupByDateIndex)
                {
                    var rccpExViewFirst = rccpExDateIndex.List.FirstOrDefault(m => m.ProductLine == rccpExProductLine.ProductLine);
                    if (rccpExViewFirst != null)
                    {
                        if (isPercentage)
                        {
                            if (isCss)
                            {
                                str.Append("<td class=\"" + rccpExViewFirst.Css + "\">");
                                str.Append(rccpExViewFirst.Qty.ToString("P"));
                                str.Append("</td>");
                            }
                            else
                            {
                                str.Append("<td>");
                                str.Append(rccpExViewFirst.Qty.ToString("P"));
                                str.Append("</td>");
                            }
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append(rccpExViewFirst.Qty.ToString("0.##"));
                            str.Append("</td>");
                        }
                    }
                    else
                    {
                        if (isPercentage)
                        {
                            str.Append("<td>");
                            str.Append("0.00%");
                            str.Append("</td>");
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append("0");
                            str.Append("</td>");

                        }
                    }
                }

                #endregion
                str.Append("</tr>");
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }
        #endregion

        #region GetExRccpViewGroupByItemTime

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemTime")]
        public string _GetItemTimeView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }

            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByItemView> rccpExGroupByItemViewList = planMgr.GetExRccpViewGroupByItemTime(rccpTransGroupList);

            if (!string.IsNullOrEmpty(productLine))
            {
                rccpExGroupByItemViewList = rccpExGroupByItemViewList.Where(r => r.ProductLine == productLine);
            }
            return Resources.EXT.ControllerLan.Con_UomHour+"<br />" + GetStringRccpExGroupByItemView(rccpExGroupByItemViewList, false);
        }


        #endregion
        #region Export ItemTime
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemTime")]
        public ActionResult ExportItemTime(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            var table = _GetItemTimeView(dateIndexTo, dateIndex, planVersion, item, dateType, productLine);
            return new DownloadFileActionResult(table, "ItemTime.xls");
        }
        #endregion
        #region GetExRccpViewGroupByItemLoad

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemLoad")]
        public string _GetItemLoadView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }


            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByItemView> rccpExGroupByItemViewList = planMgr.GetExRccpViewGroupByItemLoad(rccpTransGroupList);

            if (!string.IsNullOrEmpty(productLine))
            {
                rccpExGroupByItemViewList = rccpExGroupByItemViewList.Where(r => r.ProductLine == productLine);
            }
            return GetStringRccpExGroupByItemView(rccpExGroupByItemViewList, true);
        }


        #endregion
        #region Export ItemLoad
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemLoad")]
        public ActionResult ExportItemLoad(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            var table = _GetItemLoadView(dateIndexTo, dateIndex, planVersion, item, dateType, productLine);
            return new DownloadFileActionResult(table, "ItemLoad.xls");
        }
        #endregion
        #region GetExRccpViewGroupByItemQty

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemQty")]
        public string _GetItemQtyView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }


            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByItemView> rccpExGroupByItemViewList = planMgr.GetExRccpViewGroupByItemQty(rccpTransGroupList);

            if (!string.IsNullOrEmpty(productLine))
            {
                rccpExGroupByItemViewList = rccpExGroupByItemViewList.Where(r => r.ProductLine == productLine);
            }
            return Resources.EXT.ControllerLan.Con_UomTenThousandM+"<br />" + GetStringRccpExGroupByItemView(rccpExGroupByItemViewList, false);
        }


        #endregion
        #region Export ItemQty
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ItemQty")]
        public ActionResult ExportItemQty(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType, string productLine)
        {
            var table = _GetItemQtyView(dateIndexTo, dateIndex, planVersion, item, dateType, productLine);
            return new DownloadFileActionResult(table, "ItemQty.xls");
        }
        #endregion
        #region RccpExGroupByItemView
        private string GetStringRccpExGroupByItemView(IEnumerable<RccpExGroupByItemView> rccpExGroupByItemViewList, bool isPercentage)
        {
            if (rccpExGroupByItemViewList.Count() == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var items = this.genericMgr.FindAllIn<Item>("from Item where Code in(?", rccpExGroupByItemViewList.Select(p => p.Item).Distinct());

            var rccpExGroupByItemDateIndex = (from r in rccpExGroupByItemViewList
                                              group r by
                                              new
                                              {
                                                  DateIndex = r.DateIndex,
                                              } into g
                                              select new
                                              {
                                                  DateIndex = g.Key.DateIndex,
                                                  List = g
                                              }).OrderBy(r => r.DateIndex);


            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.MRP.RccpTrans.RccpTrans_ProductLine);
            str.Append("</th>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.MRP.RccpTrans.RccpTrans_Item);
            str.Append("</th>");


            foreach (var rccpExItemDateIndex in rccpExGroupByItemDateIndex)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(rccpExItemDateIndex.DateIndex);
                str.Append("</th>");
            }
            str.Append("</tr>");

            var rccpExGroupByProductLine = from r in rccpExGroupByItemViewList
                                           group r by
                                           new
                                           {
                                               ProductLine = r.ProductLine,
                                           } into g1
                                           select new
                                           {
                                               ProductLine = g1.Key.ProductLine,
                                               List = from s in g1
                                                      group s by
                                                      new
                                                      {
                                                          ProductLine = s.ProductLine,
                                                          Item = s.Item,
                                                      } into g2
                                                      select new
                                                      {
                                                          ProductLine = g2.Key.ProductLine,
                                                          Item = g2.Key.Item,
                                                          //ProductLineCount=g.Key.
                                                          List = g2
                                                      }
                                           };

            string productLine = string.Empty;
            int l = 0;
            foreach (var rccp in rccpExGroupByProductLine)
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

                str.Append("<td  rowspan=\"" + rccp.List.Count() + "\">");
                str.Append(rccp.ProductLine);
                str.Append("</td>");
                productLine = rccp.ProductLine;

                for (int i = 0; i < rccp.List.Count(); i++)
                {
                    var plan = rccp.List.ElementAt(i);
                    if (i > 0)
                    {
                        if (l % 2 == 0)
                        {
                            str.Append("<tr class=\"t-alt\">");
                        }
                        else
                        {
                            str.Append("<tr>");

                        }
                    }

                    //Item item = genericMgr.FindById<Item>(plan.Item);
                    Item item = items.First(p => p.Code == plan.Item);

                    str.Append("<td>");
                    str.Append(plan.Item + "<br>" + item.Description);
                    str.Append("</td>");

                    #region
                    foreach (var rccpExItemDateIndex in rccpExGroupByItemDateIndex)
                    {
                        var rccpExViewFirst = rccpExItemDateIndex.List.FirstOrDefault(m => m.ProductLine == rccp.ProductLine & m.Item == plan.Item);
                        if (rccpExViewFirst != null)
                        {
                            if (isPercentage)
                            {
                                str.Append("<td>");
                                str.Append(rccpExViewFirst.Qty.ToString("P"));
                                str.Append("</td>");
                            }
                            else
                            {
                                str.Append("<td>");
                                str.Append(rccpExViewFirst.Qty.ToString("0.#"));
                                str.Append("</td>");
                            }
                        }
                        else
                        {
                            if (isPercentage)
                            {
                                str.Append("<td>");
                                str.Append("0.00%");
                                str.Append("</td>");
                            }
                            else
                            {
                                str.Append("<td>");
                                str.Append("0");
                                str.Append("</td>");
                            }
                        }
                    }

                    str.Append("</tr>");
                    #endregion
                }

            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }
        #endregion

        #region ClassifyLoad

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifyLoad")]
        public string _GetClassifyLoadView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }


            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByClassifyView> rccpExGroupByClassifyViewList = planMgr.GetExRccpViewGroupByClassifyLoad(rccpTransGroupList);

            foreach (var rccpExGroupByClassifyView in rccpExGroupByClassifyViewList)
            {
                rccpExGroupByClassifyView.Classify = systemMgr.TranslaterccpExClassifyDescription("FlowClassify_" + rccpExGroupByClassifyView.Classify);
            }
            return GetStringRccpClassifyView(rccpExGroupByClassifyViewList, true, true);
        }


        #endregion
        #region Export ClassifyLoad
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifyLoad")]
        public ActionResult ExportClassifyLoad(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetClassifyLoadView(dateIndexTo, dateIndex, planVersion, item, dateType);
            return new DownloadFileActionResult(table, "ClassifyLoad.xls");
        }
        #endregion
        #region ClassifySpeed

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifySpeed")]
        public string _GetClassifySpeedView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }


            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByClassifyView> rccpExGroupByClassifyViewList = planMgr.GetExRccpViewGroupByClassifySpeed(rccpTransGroupList);

            foreach (var rccpExGroupByClassifyView in rccpExGroupByClassifyViewList)
            {
                rccpExGroupByClassifyView.Classify = systemMgr.TranslaterccpExClassifyDescription("FlowClassify_" + rccpExGroupByClassifyView.Classify);
            }
            return GetStringRccpClassifyView(rccpExGroupByClassifyViewList, false);
        }


        #endregion
        #region Export ClassifySpeed
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifySpeed")]
        public ActionResult ExportClassifySpeed(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetClassifySpeedView(dateIndexTo, dateIndex, planVersion, item, dateType);
            return new DownloadFileActionResult(table, "ClassifySpeed.xls");
        }
        #endregion
        #region ClassifyScrapPercentage

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifyScrapPercentage")]
        public string _GetClassifyScrapPercentageView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }


            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByClassifyView> rccpExGroupByClassifyViewList = planMgr.GetExRccpViewGroupByClassifyScrapPercentage(rccpTransGroupList);

            foreach (var rccpExGroupByClassifyView in rccpExGroupByClassifyViewList)
            {
                rccpExGroupByClassifyView.Classify = systemMgr.TranslaterccpExClassifyDescription("FlowClassify_" + rccpExGroupByClassifyView.Classify);
            }
            return GetStringRccpClassifyView(rccpExGroupByClassifyViewList, true);
        }


        #endregion
        #region Export ClassifyScrapPercentage
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifySpeed")]
        public ActionResult ExportClassifyScrapPercentage(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetClassifyScrapPercentageView(dateIndexTo, dateIndex, planVersion, item, dateType);
            return new DownloadFileActionResult(table, "ClassifyScrapPercentage.xls");
        }
        #endregion
        #region ClassifyQty

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifyQty")]
        public string _GetClassifyQtyView(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r
                            join MRP_ProdLineEx b on r.Item = b.Item
                            where b.Item is not null and  r.PlanVersion=? and r.DateType= ? ";
            param.Add(planVersion);
            param.Add(dateType);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (string.IsNullOrEmpty(dateIndexTo))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    DateTime dateTime = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateIndexTo = Utility.DateTimeHelper.GetWeekOfYear(dateTime.AddDays(7 * 16));
                }
                else
                {
                    DateTime dateTime = DateTime.Parse(dateIndex + "-1");
                    dateIndexTo = dateTime.AddMonths(12).ToString("yyyy-MM");
                }
            }
            param.Add(dateIndexTo);
            hql += "  and r.DateIndex<=? ";

            if (!string.IsNullOrEmpty(item))
            {
                hql += "  and r.Item=?";
                param.Add(item);
            }


            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IEnumerable<RccpExGroupByClassifyView> rccpExGroupByClassifyViewList = planMgr.GetExRccpViewGroupByClassifyQty(rccpTransGroupList);

            foreach (var rccpExGroupByClassifyView in rccpExGroupByClassifyViewList)
            {
                rccpExGroupByClassifyView.Classify = systemMgr.TranslaterccpExClassifyDescription("FlowClassify_" + rccpExGroupByClassifyView.Classify);
            }
            return Resources.EXT.ControllerLan.Con_UomTenThousandM + "<br />" + GetStringRccpClassifyView(rccpExGroupByClassifyViewList, false);
        }


        #endregion
        #region Export ClassifyQty
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_ClassifyQty")]
        public ActionResult ExportClassifyQty(string dateIndexTo, string dateIndex, DateTime planVersion, string item, CodeMaster.TimeUnit dateType)
        {
            var table = _GetClassifyQtyView(dateIndexTo, dateIndex, planVersion, item, dateType);
            return new DownloadFileActionResult(table, "ClassifyQty.xls");
        }
        #endregion
        #region RccpExGroupByClassifyView
        private string GetStringRccpClassifyView(IEnumerable<RccpExGroupByClassifyView> rccpExGroupByClassifyViewList, bool isPercentage, bool isCss = false)
        {
            if (rccpExGroupByClassifyViewList.Count() == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var rccpExGroupByDateIndex = (from r in rccpExGroupByClassifyViewList
                                          group r by
                                          new
                                          {
                                              DateIndex = r.DateIndex,
                                          } into g
                                          select new
                                          {
                                              DateIndex = g.Key.DateIndex,
                                              List = g
                                          }).OrderBy(r => r.DateIndex);


            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><tr>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.MRP.RccpTrans.RccpTrans_FlowClassify);
            str.Append("</th>");


            foreach (var rccpExDateIndex in rccpExGroupByDateIndex)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(rccpExDateIndex.DateIndex);
                str.Append("</th>");
            }
            str.Append("</tr>");


            var rccpExGroupByClassify = from r in rccpExGroupByClassifyViewList
                                        group r by
                                        new
                                        {
                                            Classify = r.Classify,

                                        } into g
                                        select new
                                        {
                                            Classify = g.Key.Classify,
                                            List = g
                                        };


            int l = 0;
            foreach (var rccpClassify in rccpExGroupByClassify)
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
                str.Append(rccpClassify.Classify);
                str.Append("</td>");
                #region
                foreach (var rccpExDateIndex in rccpExGroupByDateIndex)
                {
                    var rccpExViewFirst = rccpExDateIndex.List.FirstOrDefault(m => m.Classify == rccpClassify.Classify);
                    if (rccpExViewFirst != null)
                    {
                        if (isPercentage)
                        {
                            if (isCss)
                            {
                                str.Append("<td class=\"" + rccpExViewFirst.Css + "\">");
                                str.Append(rccpExViewFirst.Qty.ToString("P"));
                                str.Append("</td>");
                            }
                            else
                            {
                                str.Append("<td>");
                                str.Append(rccpExViewFirst.Qty.ToString("P"));
                                str.Append("</td>");
                            }
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append(rccpExViewFirst.Qty.ToString("0.##"));
                            str.Append("</td>");
                        }
                    }
                    else
                    {
                        if (isPercentage)
                        {
                            str.Append("<td>");
                            str.Append("0.00%");
                            str.Append("</td>");
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append("0");
                            str.Append("</td>");
                        }
                    }
                }

                #endregion
                str.Append("</tr>");
            }

            //表尾
            str.Append("</table>");
            return str.ToString();
        }
        #endregion
        #region AllotSection
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_AllotSection")]
        public string GetAllotSection(string dateIndex, DateTime planVersion, CodeMaster.TimeUnit dateType, bool onlyShowSub)
        {
            IList<object> param = new List<object>();
            string hql = @"  select r.* from MRP_RccpTransGroup r where r.PlanVersion=? and r.DateType= ? and r.DateIndex>=? ";
            param.Add(planVersion);
            param.Add(dateType);
            param.Add(dateIndex);

            IList<RccpTransGroup> rccpTransGroupList = genericMgr.FindEntityWithNativeSql<RccpTransGroup>(hql, param.ToArray());
            if (rccpTransGroupList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var rccpExViews = planMgr.GetExRccpView(rccpTransGroupList);
            var _subItems = rccpExViews.Select(p => p.Item).Distinct().ToList();
            if (onlyShowSub)
            {
                _subItems = rccpExViews.Where(p => p.ApsPriority == CodeMaster.ApsPriorityType.Backup).Select(p => p.Item).Distinct().ToList();
            }
            var _productLines = rccpExViews.Where(p => _subItems.Contains(p.Item))
                .GroupBy(p => p.ProductLine, (k, g) => new { k, g }).OrderBy(p => p.k).ToList();

            var _items = rccpExViews.Where(p => _subItems.Contains(p.Item))
                .GroupBy(p => p.Item, (k, g) => new { k, g }).OrderBy(p => p.k).ToList();

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" id=\"datatable\" width=\"100%\"><tr>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.EXT.ControllerLan.Con_Section);
            str.Append("</th>");
            foreach (var _productLine in _productLines)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(_productLine.k);
                str.Append("</th>");
            }
            str.Append("</tr>");

            foreach (var _item in _items)
            {
                if (_item.g.Sum(p => p.Qty) <= 0)
                {
                    continue;
                }
                str.Append("<tr>");
                Item item = itemMgr.GetCacheItem(_item.k);
                str.Append("<td>");
                str.Append(item.Code + "<br>" + item.Description);
                str.Append("</td>");

                foreach (var _productLine in _productLines)
                {
                    var rccp = _item.g.FirstOrDefault(p => p.ProductLine == _productLine.k);
                    if (rccp != null)
                    {
                        if (rccp.ApsPriority == CodeMaster.ApsPriorityType.Normal)
                        {
                            str.Append("<td class='WarningColor_Green'>");
                        }
                        else
                        {
                            str.Append("<td class='WarningColor_Yellow'>");
                        }
                        str.Append((rccp.Qty / 10000).ToString("0.##"));
                    }
                    else
                    {
                        str.Append("<td>");
                    }
                    str.Append("</td>");
                }
                str.Append("</tr>");
            }
            return Resources.EXT.ControllerLan.Con_UomTenThousandM+"<br />" + str.ToString();
        }
        #endregion
        #region Export AllotSection
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanEx_AllotSection")]
        public ActionResult ExportAllotSection(string dateIndex, DateTime planVersion, CodeMaster.TimeUnit dateType, bool onlyShowSub)
        {
            if (onlyShowSub) 
            {
                onlyShowSub = false;
            }
            else 
            {
                onlyShowSub = true;
            }
            var table = GetAllotSection(dateIndex, planVersion, dateType, onlyShowSub);
            return new DownloadFileActionResult(table, "AllotSection.xls");
        }
        #endregion
    }
}
