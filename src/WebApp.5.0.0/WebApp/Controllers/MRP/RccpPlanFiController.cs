using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.MRP.ORD;
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

namespace com.Sconit.Web.Controllers.MRP
{
    public class RccpPlanFiController : WebAppBaseController
    {

        public IPlanMgr planMgr { get; set; }
        public IRccpMgr rccpMgr { get; set; }

        //public IGenericMgr genericMgr { get; set; }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanFi_IslandMonth")]
        public ActionResult IslandMonth()
        {
            ViewBag.DateIndex = DateTime.Now.ToString("yyyy-MM");
            ViewBag.DateIndexTo = DateTime.Now.AddMonths(12).ToString("yyyy-MM");
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanFi_IslandWeek")]
        public ActionResult IslandWeek()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanFi_MachineWeek")]
        public ActionResult MachineWeek()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            ViewBag.DateIndexTo = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now.AddDays(7 * 16));
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanFi_MachineMonth")]
        public ActionResult MachineMonth()
        {
            ViewBag.DateIndex = DateTime.Now.ToString("yyyy-MM");
            ViewBag.DateIndexTo = DateTime.Now.AddMonths(12).ToString("yyyy-MM");
            return View();
        }

        #region IslandMonth

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanFi_IslandMonth")]
        public string _GetRccpFiPlanView(string dateIndexTo, string dateIndex, DateTime planVersion, string island, string productLine)
        {
            IList<object> param = new List<object>();
            string hql = "  from RccpFiPlan as r where  r.PlanVersion=?  and r.DateType=?";
            //param.Add(Convert.ToDateTime("2013-01-25 15:51:07.000"));
            param.Add(planVersion);
            param.Add(com.Sconit.CodeMaster.TimeUnit.Month);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (!string.IsNullOrEmpty(dateIndexTo))
            {

                hql += "  and r.DateIndex<=?";
                param.Add(dateIndexTo);
            }

            if (!string.IsNullOrEmpty(island))
            {
                string str = string.Empty;
                IList<Machine> machineList = genericMgr.FindAll<Machine>(" from Machine m where m.Island=?", island);
                foreach (var machine in machineList)
                {
                    if (str == string.Empty)
                    {
                        str += " and r.Machine in (?";
                    }
                    else
                    {
                        str += ",?";
                    }
                    param.Add(machine.Code);
                }
                str += " )";

                hql += str;
            }
            if (!string.IsNullOrEmpty(productLine))
            {
                hql += " and r.ProductLine=?";
                param.Add(productLine);
            }

            IList<RccpFiPlan> rccpFiPlanList = genericMgr.FindAll<RccpFiPlan>(hql, param.ToArray());
            if (rccpFiPlanList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IList<RccpFiView> rccpFiViewList = planMgr.GetIslandRccpView(rccpFiPlanList);

            return GetStringIsland(rccpFiViewList);
        }

        private string GetStringIsland(IList<RccpFiView> rccpFiViewList)
        {
            if (rccpFiViewList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var dateIndexList = (from r in rccpFiViewList
                                 group r by
                                 new
                                 {
                                     DateIndex = r.DateIndex,
                                 } into g
                                 select new
                                 {
                                     DateIndex = g.Key.DateIndex,
                                     List = g
                                 }).OrderBy(r => r.DateIndex).ToList();

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            str.Append("<th  style=\"text-align:center;min-width:70px;width:70px \" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Island);
            str.Append("</th>");

            str.Append("<th  style=\"text-align:center;min-width:70px;width:70px \" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Machine);
            str.Append("</th>");

            str.Append("<th  style=\"text-align:center;min-width:60px;width:60px \" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Type);
            str.Append("</th>");

            foreach (var rccpFiView in dateIndexList)
            {
                str.Append("<th  style=\"text-align:center;\" >");
                str.Append(rccpFiView.DateIndex);
                str.Append("</th>");
            }

            str.Append("</tr></thead><tbody>");

            var rccpBodys = (from r in rccpFiViewList
                             group r by
                             new
                             {
                                 Island = r.Island,
                             } into g
                             select new
                            {
                                Island = g.Key.Island,
                                IslandDescription = g.First().IslandDescription,
                                List = g.ToList()
                            }).ToList();

            int l = 0;
            foreach (var rccpBody in rccpBodys)
            {
                l++;
                var machineList = rccpBody.List.SelectMany(p => p.RccpFiViewList)
                    .GroupBy(p => new { p.Machine, p.Description }, (k, g) => new { Machine = k.Machine, Description = k.Description })
                    .OrderBy(p => p.Machine).ToList();

                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                str.Append(string.Format("<td  rowspan=\"{0}\">", machineList.Count() * 2 + 3));
                str.Append(rccpBody.Island + "<br>[" + rccpBody.IslandDescription + "]");
                str.Append("</td>");

                foreach (var machine in machineList)
                {
                    if (machineList.IndexOf(machine) > 0)
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

                    str.Append("<td rowspan='2'>");
                    str.Append(machine.Machine + "<br>[" + machine.Description + "]");
                    str.Append("</td>");

                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            str.Append("<td >");
                            str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_Demand);
                            str.Append("</td>");
                        }
                        else if (i == 1)
                        {
                            if (l % 2 == 0)
                            {
                                str.Append("<tr class=\"t-alt\">");
                            }
                            else
                            {
                                str.Append("<tr>");
                            }
                            str.Append("<td >");
                            str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_RequiredFactQty);
                            str.Append("</td>");
                        }
                        foreach (var dateIndex in dateIndexList)
                        {
                            var rccpMachine = rccpBody.List.SelectMany(p => p.RccpFiViewList)
                                .Where(p => p.DateIndex == dateIndex.DateIndex && p.Machine == machine.Machine).FirstOrDefault() ?? new RccpFiView();
                            if (i == 0)
                            {
                                str.Append("<td >");
                                str.Append(rccpMachine.KitQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 1)
                            {

                                str.Append("<td >");
                                if (rccpMachine.DateType == CodeMaster.TimeUnit.Week)
                                {
                                    str.Append(rccpMachine.CurrentRequiredFactQty.ToString("0.##"));
                                }
                                else
                                {
                                    str.Append(rccpMachine.RequiredFactQty.ToString("0.##"));
                                }
                                str.Append("</td>");
                            }
                        }
                        str.Append("</tr>");
                    }
                }

                for (int i = 0; i < 3; i++)
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
                        str.Append("<td rowspan='3'>");
                        str.Append(Resources.EXT.ControllerLan.Con_Summary);
                        str.Append("</td>");

                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_Demand);
                        str.Append("</td>");
                    }
                    else if (i == 1)
                    {
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_IslandQty);
                        str.Append("</td>");
                    }
                    else
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_RequiredFactQty);
                        str.Append("</td>");
                    }

                    #region
                    foreach (var rccpFiView in dateIndexList)
                    {
                        var rccpFiViewFirst = rccpFiView.List.FirstOrDefault(m => m.Island == rccpBody.Island);
                        if (rccpFiViewFirst != null)
                        {
                            if (i == 0)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.KitQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 1)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.IslandQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else
                            {
                                if (rccpFiViewFirst.DateType == CodeMaster.TimeUnit.Week)
                                {
                                    if (rccpFiViewFirst.CurrentRequiredFactQty > rccpFiViewFirst.IslandQty)
                                    {
                                        str.Append("<td   class=\"mrp-warning\">");
                                        str.Append(rccpFiViewFirst.CurrentRequiredFactQty.ToString("0.##"));
                                        str.Append("</td>");
                                    }
                                    else
                                    {
                                        str.Append("<td>");
                                        str.Append(rccpFiViewFirst.CurrentRequiredFactQty.ToString("0.##"));
                                        str.Append("</td>");
                                    }
                                }
                                else
                                {
                                    if (rccpFiViewFirst.RequiredFactQty > rccpFiViewFirst.IslandQty)
                                    {
                                        str.Append("<td   class=\"mrp-warning\">");
                                        str.Append(rccpFiViewFirst.RequiredFactQty.ToString("0.##"));
                                        str.Append("</td>");
                                    }
                                    else
                                    {
                                        str.Append("<td>");
                                        str.Append(rccpFiViewFirst.RequiredFactQty.ToString("0.##"));
                                        str.Append("</td>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append("0");
                            str.Append("</td>");
                        }
                    }
                    #endregion
                    str.Append("</tr>");
                }
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }

        #endregion

        #region IslandWeek

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanFi_IslandWeek")]
        public string _GetRccpFiPlanWeekView(string dateIndexTo, string dateIndex, DateTime planVersion, string island, string productLine)
        {
            IList<object> param = new List<object>();
            string hql = "  from RccpFiPlan as r where  r.PlanVersion=?  and r.DateType=?";
            //param.Add(Convert.ToDateTime("2013-01-25 15:51:07.000"));
            param.Add(planVersion);
            param.Add(com.Sconit.CodeMaster.TimeUnit.Week);
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (!string.IsNullOrEmpty(dateIndexTo))
            {

                hql += "  and r.DateIndex<=?";
                param.Add(dateIndexTo);
            }

            if (!string.IsNullOrEmpty(island))
            {
                string str = string.Empty;
                IList<Machine> machineList = genericMgr.FindAll<Machine>(" from Machine m where m.Island=?", island);
                foreach (var machine in machineList)
                {
                    if (str == string.Empty)
                    {
                        str += " and r.Machine in (?";
                    }
                    else
                    {
                        str += ",?";
                    }
                    param.Add(machine.Code);
                }
                str += " )";

                hql += str;
            }
            if (!string.IsNullOrEmpty(productLine))
            {
                hql += " and r.ProductLine=?";
                param.Add(productLine);
            }

            IList<RccpFiPlan> rccpFiPlanList = genericMgr.FindAll<RccpFiPlan>(hql, param.ToArray());
            if (rccpFiPlanList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IList<RccpFiView> rccpFiViewList = planMgr.GetIslandRccpView(rccpFiPlanList);

            return GetStringIsland(rccpFiViewList);
        }

        private string GetStringRccpFiPlanWeekView(IList<RccpFiView> rccpFiViewList)
        {
            if (rccpFiViewList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var rccpFiViewListGruopby = (from r in rccpFiViewList
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

            str.Append("<th  style=\"text-align:center;width:100px\" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Island);
            str.Append("</th>");
            str.Append("<th  style=\"text-align:center;width:80px\" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Type);
            str.Append("</th>");

            foreach (var rccpFiView in rccpFiViewListGruopby)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(rccpFiView.DateIndex);
                str.Append("</th>");
            }

            str.Append("</tr></thead><tbody>");

            var rccpFiViewIslandWeekGruopby = from r in rccpFiViewList
                                              group r by
                                              new
                                              {
                                                  Island = r.Island,
                                              } into g
                                              select new RccpFiView
                                              {
                                                  Island = g.Key.Island,
                                                  IslandDescription = g.First().IslandDescription,
                                              };

            int l = 0;
            foreach (var rccpPurchasePlanIsland in rccpFiViewIslandWeekGruopby)
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
                        str.Append(rccpPurchasePlanIsland.Island + "<br>[" + rccpPurchasePlanIsland.IslandDescription + "]");
                        str.Append("</td>");
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_Demand);
                        str.Append("</td>");

                    }
                    else if (i == 1)
                    {
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_IslandQty);
                        str.Append("</td>");
                    }
                    else if (i == 2)
                    {
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_NormalQty);
                        str.Append("</td>");
                    }
                    else if (i == 3)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_WeekMaxQty);
                        str.Append("</td>");
                    }
                    else if (i == 4)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_MaxShiftQty);
                        str.Append("</td>");
                    }
                    else if (i == 5)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_NormalShiftQty);
                        str.Append("</td>");
                    }
                    else if (i == 6)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_RequiredShiftPerWeek);
                        str.Append("</td>");
                    }
                    else if (i == 7)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_AlarmLamp);
                        str.Append("</td>");

                    }

                    #region
                    foreach (var rccpFiView in rccpFiViewListGruopby)
                    {
                        var rccpFiViewFirst = rccpFiView.List.FirstOrDefault(m => m.Island == rccpPurchasePlanIsland.Island);
                        if (rccpFiViewFirst != null)
                        {
                            if (i == 0)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.KitQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 1)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.IslandQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 2)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.NormalQty / rccpFiViewFirst.ModelRate).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 3)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.MaxQty / rccpFiViewFirst.ModelRate).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 4)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.MaxShiftQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 5)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.NormalShiftQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 6)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.RequiredShiftQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 7)
                            {
                                if (rccpFiViewFirst.Qty > rccpFiViewFirst.MaxQty)
                                {
                                    str.Append("<td>");
                                    str.Append("<img src='/Content/Images/Icon/Error.png'/>");
                                    str.Append("</td>");
                                }
                                else if (rccpFiViewFirst.Qty < rccpFiViewFirst.MaxQty && rccpFiViewFirst.Qty > rccpFiViewFirst.NormalQty)
                                {
                                    str.Append("<td>");
                                    str.Append("<img src='/Content/Images/Icon/Warning.png'/>");
                                    str.Append("</td>");
                                }
                                else
                                {
                                    str.Append("<td>");
                                    str.Append("<img src='/Content/Images/Icon/Success.png'/>");
                                    str.Append("</td>");
                                }
                            }
                        }
                        else
                        {
                            if (i == 7)
                            {
                                str.Append("<td>");
                                str.Append("<img src='/Content/Images/Icon/Success.png'>");
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

        #region MachineMonth

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanFi_MachineMonth")]
        public string _GetMachineMonthView(string dateIndexTo, string dateIndex, DateTime planVersion, string machine, string productLine, string island)
        {
            IList<object> param = new List<object>();
            string hql = "  from RccpFiPlan as r where  r.PlanVersion=?   and r.DateType=?";
            //param.Add(Convert.ToDateTime("2013-01-25 15:51:07.000"));
            param.Add(planVersion);
            param.Add(com.Sconit.CodeMaster.TimeUnit.Month);
            if (!string.IsNullOrEmpty(machine))
            {
                hql += " and r.Machine=?";
                param.Add(machine);
            }
            if (!string.IsNullOrEmpty(productLine))
            {
                hql += " and r.ProductLine=?";
                param.Add(productLine);
            }
            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (!string.IsNullOrEmpty(dateIndexTo))
            {

                hql += "  and r.DateIndex<=?";
                param.Add(dateIndexTo);
            }

            if (!string.IsNullOrEmpty(island))
            {
                string hql1 = string.Empty;
                IList<Machine> machineList = genericMgr.FindAll<Machine>(" from Machine m where m.Island=?", island);
                foreach (var mdMachine in machineList)
                {
                    if (hql1 == string.Empty)
                    {
                        hql1 += " and r.Machine in (?";
                    }
                    else
                    {
                        hql1 += ",?";
                    }
                    param.Add(mdMachine.Code);
                }
                hql1 += " )";

                hql += hql1;
            }
            IList<RccpFiPlan> rccpFiPlanList = genericMgr.FindAll<RccpFiPlan>(hql, param.ToArray());
            if (rccpFiPlanList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var rccpFiViewList = planMgr.GetMachineRccpView(rccpFiPlanList).ToList();
            //var rccpFiViewIslandTotal = from r in rccpFiViewList
            //                            group r by
            //                            new
            //                            {
            //                                Machine = r.Machine,
            //                            } into g
            //                            select new RccpFiView
            //                            {
            //                                DateIndex = "Total",
            //                                Machine = g.Key.Machine,
            //                                KitQty = g.Sum(p => p.KitQty),
            //                                MachineQty = g.Average(p => p.MachineQty),
            //                                RequiredFactQty = g.Average(p => p.RequiredFactQty),
            //                                MaxQty = g.Sum(p => p.MaxQty / p.ModelRate),
            //                                ShiftQuota = g.Average(p => p.ShiftQuota),
            //                                MaxShiftQty = g.Sum(p => p.MaxShiftQty),
            //                                MaxWorkDay = g.Sum(p => p.MaxWorkDay),
            //                                NormalWorkDay = g.Sum(p => p.NormalWorkDay),
            //                                TrialProduceTime = g.Sum(p => p.TrialProduceTime),
            //                                HaltTime = g.Sum(p => p.HaltTime),
            //                                Holiday = g.Sum(p => p.Holiday)
            //                            };
            //rccpFiViewList.AddRange(rccpFiViewIslandTotal);
            var rccpFiViewListGruopby = (from r in rccpFiViewList
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
            str.Append("<th  style=\"text-align:center;min-width:70px;width:70px;\" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Machine);
            str.Append("</th>");
            str.Append("<th  style=\"text-align:center;min-width:60px;width:60px;\" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Type);
            str.Append("</th>");
            foreach (var rccpFiView in rccpFiViewListGruopby)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(rccpFiView.DateIndex);
                str.Append("</th>");
            }
            str.Append("</tr></thead><tbody>");
            var rccpFiViewMachineGruopby = from r in rccpFiViewList
                                           group r by
                                           new
                                           {
                                               Machine = r.Machine,
                                           } into g
                                           select new RccpFiView
                                           {
                                               Machine = g.Key.Machine,
                                               Description = g.First().Description,
                                           };
            int l = 0;
            foreach (var rccpPurchasePlanMachine in rccpFiViewMachineGruopby)
            {
                l++;
                for (int i = 0; i < 11; i++)
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
                        str.Append("<td  rowspan=\"11\">");
                        str.Append(rccpPurchasePlanMachine.Machine + "<br>[" + rccpPurchasePlanMachine.Description + "]");
                        str.Append("</td>");
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_Demand);
                        str.Append("</td>");
                    }
                    else if (i == 1)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_MaxCapacity);
                        str.Append("</td>");
                    }
                    else if (i == 2)
                    {
                        str.Append("<td >");
                        str.Append(Resources.EXT.ControllerLan.Con_CurrentMaxCapacity);
                        str.Append("</td>");
                    }
                    else if (i == 3)
                    {
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_ShiftQuota);
                        str.Append("</td>");
                    }
                    else if (i == 4)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_WorkDays);
                        str.Append("</td>");
                    }
                    else if (i == 5)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_CalendarDays);
                        str.Append("</td>");
                    }
                    else if (i == 6)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_Holidays);
                        str.Append("</td>");
                    }
                    else if (i == 7)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_HaltDays);
                        str.Append("</td>");
                    }
                    else if (i == 8)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_TrailDays);
                        str.Append("</td>");
                    }
                    else if (i == 9)
                    {

                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_MachineQty);
                        str.Append("</td>");
                    }
                    else
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_RequiredFactQtyMachine);
                        str.Append("</td>");
                    }

                    #region
                    foreach (var rccpFiView in rccpFiViewListGruopby)
                    {
                        var rccpFiViewFirst = rccpFiView.List.FirstOrDefault(m => m.Machine == rccpPurchasePlanMachine.Machine);
                        if (rccpFiViewFirst != null)
                        {
                            if (i == 0)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.Qty / rccpFiViewFirst.ModelRate).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 1)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.MaxQty / rccpFiViewFirst.ModelRate).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 2)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.CurrentMaxQty / rccpFiViewFirst.ModelRate).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 3)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.KitShiftQuota.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 4)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.NormalWorkDay.ToString("0.##"));
                                str.Append("</td>");
                            }

                            else if (i == 5)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.MaxWorkDay + rccpFiViewFirst.Holiday + rccpFiViewFirst.HaltTime + rccpFiViewFirst.TrialProduceTime).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 6)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.Holiday.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 7)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.HaltTime.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 8)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.TrialProduceTime.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 9)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.MachineQty.ToString("0.##"));
                                str.Append("</td>");
                            }

                            else
                            {
                                if (rccpFiViewFirst.RequiredFactQty > rccpFiViewFirst.MachineQty)
                                {
                                    str.Append("<td   class=\"mrp-warning\">");
                                    str.Append(rccpFiViewFirst.RequiredFactQty.ToString("0.##"));
                                    str.Append("</td>");
                                }
                                else
                                {
                                    str.Append("<td>");
                                    str.Append(rccpFiViewFirst.RequiredFactQty.ToString("0.##"));
                                    str.Append("</td>");
                                }
                            }
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append("0");
                            str.Append("</td>");
                        }
                    }
                    #endregion
                }
                str.Append("</tr>");
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }

        #endregion

        #region MachineWeek

        [SconitAuthorize(Permissions = "Url_Mrp_RccpPlanFi_MachineWeek")]
        public string _GetRccpFiPlanMachineWeekView(string dateIndexTo, string dateIndex, DateTime planVersion, string machine, string productLine, string island)
        {
            IList<object> param = new List<object>();
            string hql = "  from RccpFiPlan as r where  r.PlanVersion=?   and r.DateType=?";
            param.Add(planVersion);
            param.Add(com.Sconit.CodeMaster.TimeUnit.Week);
            if (!string.IsNullOrEmpty(machine))
            {
                hql += " and r.Machine=?";
                param.Add(machine);
            }
            if (!string.IsNullOrEmpty(productLine))
            {
                hql += " and r.ProductLine=?";
                param.Add(productLine);
            }


            if (!string.IsNullOrEmpty(dateIndex))
            {
                hql += " and r.DateIndex>=?";
                param.Add(dateIndex);
            }
            if (!string.IsNullOrEmpty(dateIndexTo))
            {

                hql += "  and r.DateIndex<=?";
                param.Add(dateIndexTo);
            }

            if (!string.IsNullOrEmpty(island))
            {
                string str = string.Empty;
                IList<Machine> machineList = genericMgr.FindAll<Machine>(" from Machine m where m.Island=?", island);
                foreach (var mdMachine in machineList)
                {
                    if (str == string.Empty)
                    {
                        str += " and r.Machine in (?";
                    }
                    else
                    {
                        str += ",?";
                    }
                    param.Add(mdMachine.Code);
                }
                str += " )";

                hql += str;
            }

            IList<RccpFiPlan> rccpFiPlanList = genericMgr.FindAll<RccpFiPlan>(hql, param.ToArray());
            if (rccpFiPlanList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            IList<RccpFiView> rccpFiViewList = planMgr.GetMachineRccpView(rccpFiPlanList);

            return GetStringRccpFiPlanMachineWeekView(rccpFiViewList);
        }

        private string GetStringRccpFiPlanMachineWeekView(IList<RccpFiView> rccpFiViewList)
        {
            if (rccpFiViewList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var rccpFiViewListGruopby = (from r in rccpFiViewList
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

            str.Append("<th  style=\"text-align:center;min-width:70px;width:70px;\" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Machine);
            str.Append("</th>");
            str.Append("<th  style=\"text-align:center;min-width:60px;width:60px;\" >");
            str.Append(Resources.MRP.RccpFiPlan.RccpFiPlan_Type);
            str.Append("</th>");

            foreach (var rccpFiView in rccpFiViewListGruopby)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(rccpFiView.DateIndex);
                str.Append("</th>");
            }

            str.Append("</tr></thead><tbody>");

            var rccpFiViewMachineWeekGruopby = from r in rccpFiViewList
                                               group r by
                                               new
                                               {
                                                   Machine = r.Machine,
                                               } into g
                                               select new RccpFiView
                                               {
                                                   Machine = g.Key.Machine,
                                                   Description = g.First().Description,
                                               };

            int l = 0;
            foreach (var rccpPurchasePlanMachine in rccpFiViewMachineWeekGruopby)
            {
                l++;
                for (int i = 0; i < 12; i++)
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
                        str.Append("<td  rowspan=\"12\">");
                        str.Append(rccpPurchasePlanMachine.Machine + "<br>[" + rccpPurchasePlanMachine.Description + "]");
                        str.Append("</td>");
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_Demand);
                        str.Append("</td>");
                    }
                    else if (i == 1)
                    {
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_WeekShiftQuota);
                        str.Append("</td>");
                    }
                    else if (i == 2)
                    {
                        str.Append("<td >");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_ShiftType);
                        str.Append("</td>");
                    }
                    else if (i == 3)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_NormalWorkDay);
                        str.Append("</td>");
                    }
                    else if (i == 4)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_MaxWorkDay);
                        str.Append("</td>");
                    }
                    else if (i == 5)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_MachineQty);
                        str.Append("</td>");
                    }
                    else if (i == 6)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_NormalQty);
                        str.Append("</td>");
                    }
                    else if (i == 7)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_WeekMaxQty);
                        str.Append("</td>");
                    }
                    else if (i == 8)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_RequirementMachine);
                        str.Append("</td>");
                    }
                    else if (i == 9)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_Gap);
                        str.Append("</td>");
                    }
                    else if (i == 10)
                    {
                        str.Append("<td>");
                        str.Append(Resources.EXT.ControllerLan.Con_RequirementShift);
                        str.Append("</td>");
                    }
                    else if (i == 11)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.RccpFiPlan.RccpFiPlan_AlarmLamp);
                        str.Append("</td>");
                    }

                    #region
                    foreach (var rccpFiView in rccpFiViewListGruopby)
                    {
                        var rccpFiViewFirst = rccpFiView.List.FirstOrDefault(m => m.Machine == rccpPurchasePlanMachine.Machine);
                        if (rccpFiViewFirst != null)
                        {
                            if (i == 0)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.KitQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 1)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.KitShiftQuota.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 2)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.ShiftPerDay);
                                str.Append("</td>");
                            }
                            else if (i == 3)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.NormalWorkDay.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 4)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.MaxWorkDay.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 5)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.MachineQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 6)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.CurrentNormalQty / rccpFiViewFirst.ModelRate).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 7)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.CurrentMaxQty / rccpFiViewFirst.ModelRate).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 8)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.RequiredFactQty.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 9)
                            {
                                str.Append("<td>");
                                str.Append((rccpFiViewFirst.CurrentDiffQty / rccpFiViewFirst.ModelRate).ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 10)
                            {
                                str.Append("<td>");
                                str.Append(rccpFiViewFirst.RequiredShiftPerDay.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 11)
                            {
                                if (rccpFiViewFirst.Qty > rccpFiViewFirst.CurrentMaxQty)
                                {
                                    str.Append("<td>");
                                    str.Append("<img src='/Content/Images/Icon/Error.png'/>");
                                    str.Append("</td>");
                                }
                                else if (rccpFiViewFirst.Qty < rccpFiViewFirst.CurrentMaxQty && rccpFiViewFirst.Qty > rccpFiViewFirst.CurrentNormalQty)
                                {
                                    str.Append("<td>");
                                    str.Append("<img src='/Content/Images/Icon/Warning.png'/>");
                                    str.Append("</td>");
                                }
                                else
                                {
                                    str.Append("<td>");
                                    str.Append("<img src='/Content/Images/Icon/Success.png'/>");
                                    str.Append("</td>");
                                }
                            }
                        }
                        else
                        {
                            if (i == 11)
                            {
                                str.Append("<td>");
                                str.Append("<img src='/Content/Images/Icon/Success.png'>");
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

    }
}
