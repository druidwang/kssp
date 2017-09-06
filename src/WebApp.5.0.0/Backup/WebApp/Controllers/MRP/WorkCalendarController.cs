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
    public class WorkCalendarController : WebAppBaseController
    {
        //
        // GET: /CustomerPlan/
        public IPlanMgr planMgr { get; set; }
        public IRccpMgr rccpMgr { get; set; }
        //public IGenericMgr genericMgr { get; set; }
        private static string selectCountStatement = "select count(*) from WorkCalendar as w";

        private static string selectStatement = "select w from WorkCalendar as w";

        #region
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExView")]
        public ActionResult ExView()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExEdit")]
        public ActionResult ExEdit()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_FiView")]
        public ActionResult FiView()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_FiEdit")]
        public ActionResult FiEdit()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_MiView")]
        public ActionResult MiView()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_MiEdit")]
        public ActionResult MiEdit()
        {
            return View();
        }
        #region Export FiWorkCalendar
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExView,Url_Mrp_WorkCalendar_MiView,Url_Mrp_WorkCalendar_FiView")]
        public ActionResult ExportFiWorkCalendar(string dateIndexTo, string dateIndex, DateTime? dateIndexDate,
            DateTime? dateIndexToDate, string productLine, com.Sconit.CodeMaster.TimeUnit dateType )
        {
            com.Sconit.CodeMaster.ResourceGroup resourceGroup = com.Sconit.CodeMaster.ResourceGroup.FI;
            var table = _GetMrpWorkingCalendarView(dateIndexTo, dateIndex, dateIndexDate, dateIndexToDate, productLine, dateType, resourceGroup);
            return new DownloadFileActionResult(table, "FiWorkCalendar.xls");
        }
        #endregion
        #region Export ExWorkCalendar
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExView,Url_Mrp_WorkCalendar_MiView,Url_Mrp_WorkCalendar_FiView")]
        public ActionResult ExportExWorkCalendar(string dateIndexTo, string dateIndex, DateTime? dateIndexDate,
            DateTime? dateIndexToDate, string productLine, com.Sconit.CodeMaster.TimeUnit dateType )
        {
            com.Sconit.CodeMaster.ResourceGroup resourceGroup = com.Sconit.CodeMaster.ResourceGroup.EX;
            var table = _GetMrpWorkingCalendarView(dateIndexTo, dateIndex, dateIndexDate, dateIndexToDate, productLine, dateType, resourceGroup);
            return new DownloadFileActionResult(table, "ExWorkCalendar.xls");
        }
        #endregion
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExView,Url_Mrp_WorkCalendar_MiView,Url_Mrp_WorkCalendar_FiView")]
        public string _GetMrpWorkingCalendarView(string dateIndexTo, string dateIndex, DateTime? dateIndexDate,
            DateTime? dateIndexToDate, string productLine, com.Sconit.CodeMaster.TimeUnit dateType, com.Sconit.CodeMaster.ResourceGroup resourceGroup)
        {
            IList<object> param = new List<object>();
            string hql = "select w.* from MRP_WorkCalendar w Left join  SCM_FlowMstr s on w.Flow=s.Code where s.ResourceGroup=? and  w.DateType=?";
            param.Add(resourceGroup);
            param.Add(dateType);
            if (dateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                hql += " and w.DateIndex>=? and w.DateIndex<=? ";
                param.Add(dateIndexDate.Value.ToString("yyyy-MM-dd"));
                param.Add(dateIndexToDate.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                hql += " and w.DateIndex>=? and w.DateIndex<=?  ";
                param.Add(dateIndex);
                param.Add(dateIndexTo);
            }
            if (!string.IsNullOrEmpty(productLine))
            {
                hql += " and w.Flow=?";
                param.Add(productLine);
            }

            IList<WorkCalendar> workCalendarList = genericMgr.FindEntityWithNativeSql<WorkCalendar>(hql, param.ToArray());

            if (workCalendarList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }


            return GetStringMrpWorkingCalendarView(workCalendarList);

        }
        #region Export MiWorkCalendar
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_MiView")]
        public ActionResult ExportMiWorkCalendar(string dateIndexTo, string dateIndex, DateTime? dateIndexDate, DateTime? dateIndexToDate, string productLine, com.Sconit.CodeMaster.TimeUnit dateType)
        {
            var table = _GetMrpWorkingCalendarMiView(dateIndexTo, dateIndex, dateIndexDate, dateIndexToDate, productLine, dateType);
            return new DownloadFileActionResult(table, "MiWorkCalendar.xls");
        }
        #endregion
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_MiView")]
        public string _GetMrpWorkingCalendarMiView(string dateIndexTo, string dateIndex, DateTime? dateIndexDate, DateTime? dateIndexToDate, string productLine, com.Sconit.CodeMaster.TimeUnit dateType)
        {
            IList<object> param = new List<object>();
            string hql = "select w.* from MRP_WorkCalendar w Left join  SCM_FlowMstr s on w.Flow=s.Code where s.ResourceGroup=? and  w.DateType=?";
            param.Add(com.Sconit.CodeMaster.ResourceGroup.MI);
            param.Add(dateType);
            if (dateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                hql += " and w.DateIndex>=? and w.DateIndex<=? ";
                param.Add(dateIndexDate.Value.ToString("yyyy-MM-dd"));
                param.Add(dateIndexToDate.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                hql += " and w.DateIndex>=? and w.DateIndex<=?  ";
                param.Add(dateIndex);
                param.Add(dateIndexTo);
            }
            if (!string.IsNullOrEmpty(productLine))
            {
                hql += " and w.Flow=?";
                param.Add(productLine);
            }

            IList<WorkCalendar> workCalendarList = genericMgr.FindEntityWithNativeSql<WorkCalendar>(hql, param.ToArray());

            if (workCalendarList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            return GetStringMrpWorkingCalendarView(workCalendarList);
        }


        #endregion
        #region MrpWorkingCalendarView
        private string GetStringMrpWorkingCalendarView(IList<WorkCalendar> workCalendarList)
        {
            if (workCalendarList.Count() == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var workCalendarGroupByDateIndex = (from r in workCalendarList
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
            str.Append(Resources.MRP.MrpWorkCalendar.MrpWorkCalendar_ProductLine);
            str.Append("</th>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.MRP.MrpWorkCalendar.MrpWorkCalendar_Type);
            str.Append("</th>");
            foreach (var workCalendarDateIndex in workCalendarGroupByDateIndex)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(workCalendarDateIndex.DateIndex);
                str.Append("</th>");
            }
            str.Append("</tr>");


            var workCalendarGroupByFlow = from r in workCalendarList
                                          group r by
                                          new
                                          {
                                              Flow = r.Flow,

                                          } into g
                                          select new
                                          {
                                              Flow = g.Key.Flow,
                                              List = g
                                          };


            int l = 0;
            foreach (var workCalendar in workCalendarGroupByFlow)
            {
                l++;
                for (int i = 0; i < 4; i++)
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
                        str.Append("<td  rowspan=\"4\">");
                        str.Append(workCalendar.Flow);
                        str.Append("</td>");
                        str.Append("<td>");
                        str.Append(@Resources.MRP.MrpWorkCalendar.MrpWorkCalendar_UpTime);
                        str.Append("</td>");

                    }
                    else if (i == 1)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.MrpWorkCalendar.MrpWorkCalendar_TrialTime);
                        str.Append("</td>");
                    }
                    else if (i == 2)
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.MrpWorkCalendar.MrpWorkCalendar_HaltTime);
                        str.Append("</td>");
                    }
                    else
                    {
                        str.Append("<td>");
                        str.Append(@Resources.MRP.MrpWorkCalendar.MrpWorkCalendar_Holiday);
                        str.Append("</td>");
                    }



                    #region
                    foreach (var workCalendarDateIndex in workCalendarGroupByDateIndex)
                    {
                        var workCalendarFirst = workCalendar.List.FirstOrDefault(m => m.DateIndex == workCalendarDateIndex.DateIndex);
                        if (workCalendarFirst != null)
                        {
                            if (i == 0)
                            {
                                str.Append("<td>");
                                str.Append(workCalendarFirst.UpTime.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 1)
                            {
                                str.Append("<td>");
                                str.Append(workCalendarFirst.TrialTime.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else if (i == 2)
                            {
                                str.Append("<td>");
                                str.Append(workCalendarFirst.HaltTime.ToString("0.##"));
                                str.Append("</td>");
                            }
                            else
                            {
                                str.Append("<td>");
                                str.Append(workCalendarFirst.Holiday.ToString("0.##"));
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

        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExEdit,Url_Mrp_WorkCalendar_FiEdit")]
        public ActionResult _GetWorkCalendarList(WorkCalendarSearchModel searchModel)
        {
            ViewBag.PageSize = 20;
            ViewBag.ProductLine = searchModel.ProductLine;
            ViewBag.DateType = searchModel.DateType;
            ViewBag.DateIndexDate = searchModel.DateIndexDate;
            ViewBag.DateIndexTo = searchModel.DateIndexTo;
            ViewBag.DateIndexToDate = searchModel.DateIndexToDate;
            ViewBag.DateIndex = searchModel.DateIndex;
            ViewBag.ResourceGroup = searchModel.ResourceGroup;
            return PartialView();

        }
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_MiEdit")]
        public ActionResult _GetWorkCalendarMiList(WorkCalendarSearchModel searchModel)
        {
            ViewBag.PageSize = 20;
            ViewBag.ProductLine = searchModel.ProductLine;
            ViewBag.DateType = searchModel.DateType;
            ViewBag.DateIndexDate = searchModel.DateIndexDate;
            ViewBag.DateIndexTo = searchModel.DateIndexTo;
            ViewBag.DateIndexToDate = searchModel.DateIndexToDate;
            ViewBag.DateIndex = searchModel.DateIndex;
            return PartialView();

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_MiEdit")]
        public ActionResult _AjaxMiList(GridCommand command, WorkCalendarSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchMIStatement(command, searchModel);
            GridModel<WorkCalendar> WorkCalendarList = GetAjaxPageData<WorkCalendar>(searchStatementModel, command);
            foreach (var WorkCalendar in WorkCalendarList.Data)
            {
                WorkCalendar.UpTime = Math.Round(WorkCalendar.UpTime, 3);
                WorkCalendar.TrialTime = Math.Round(WorkCalendar.TrialTime, 3);
                WorkCalendar.HaltTime = Math.Round(WorkCalendar.HaltTime, 3);
                WorkCalendar.Holiday = Math.Round(WorkCalendar.Holiday, 3);
            }
            return PartialView(WorkCalendarList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExEdit,Url_Mrp_WorkCalendar_FiEdit")]
        public ActionResult _AjaxList(GridCommand command, WorkCalendarSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<WorkCalendar> WorkCalendarList = GetAjaxPageData<WorkCalendar>(searchStatementModel, command);
            foreach (var WorkCalendar in WorkCalendarList.Data)
            {
                WorkCalendar.UpTime = Math.Round(WorkCalendar.UpTime, 3);
                WorkCalendar.TrialTime = Math.Round(WorkCalendar.TrialTime, 3);
                WorkCalendar.HaltTime = Math.Round(WorkCalendar.HaltTime, 3);
                WorkCalendar.Holiday = Math.Round(WorkCalendar.Holiday, 3);
            }
            return PartialView(WorkCalendarList);
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, WorkCalendarSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            whereStatement += " where exists (select 1 from FlowMaster s where s.ResourceGroup=? and w.Flow=s.Code) ";
            param.Add(searchModel.ResourceGroup);
            if (searchModel.DateType == (int)com.Sconit.CodeMaster.TimeUnit.Day)
            {
                HqlStatementHelper.AddEqStatement("DateType", com.Sconit.CodeMaster.TimeUnit.Day, "w", ref whereStatement, param);
                HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.DateIndexDate.Value.ToString("yyyy-MM-dd"), searchModel.DateIndexToDate.Value.ToString("yyyy-MM-dd"), "w", ref whereStatement, param);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchModel.DateIndexTo))
                {
                    searchModel.DateIndexTo = "9999-01";
                }
                HqlStatementHelper.AddEqStatement("DateType", searchModel.DateType, "w", ref whereStatement, param);
                HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.DateIndex, searchModel.DateIndexTo, "w", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("Flow", searchModel.ProductLine, "w", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel PrepareSearchMIStatement(GridCommand command, WorkCalendarSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            whereStatement += " where exists (select 1 from FlowMaster s where s.ResourceGroup=? and w.Flow=s.Code) ";
            param.Add((int)com.Sconit.CodeMaster.ResourceGroup.MI);
            if (searchModel.DateType == (int)com.Sconit.CodeMaster.TimeUnit.Day)
            {
                HqlStatementHelper.AddEqStatement("DateType", com.Sconit.CodeMaster.TimeUnit.Day, "w", ref whereStatement, param);
                HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.DateIndexDate.Value.ToString("yyyy-MM-dd"), searchModel.DateIndexToDate.Value.ToString("yyyy-MM-dd"), "w", ref whereStatement, param);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchModel.DateIndexTo))
                {
                    searchModel.DateIndexTo = "9999-01";
                }
                HqlStatementHelper.AddEqStatement("DateType", searchModel.DateType, "w", ref whereStatement, param);
                HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.DateIndex, searchModel.DateIndexTo, "w", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("Flow", searchModel.ProductLine, "w", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExEdit,Url_Mrp_WorkCalendar_MiEdit,Url_Mrp_WorkCalendar_FiEdit")]
        public ActionResult _Update(GridCommand command, WorkCalendar workCalendar, string NDateIndexTo, string NDateIndex
            , string NProductLine, DateTime? NDateIndexDate, DateTime? NDateIndexToDate, com.Sconit.CodeMaster.TimeUnit NDateType,
            int resourceGroup)
        {
            WorkCalendarSearchModel searchModel = new WorkCalendarSearchModel();
            searchModel.ProductLine = NProductLine;
            searchModel.DateType = (short)NDateType;
            searchModel.DateIndex = NDateIndex;
            searchModel.DateIndexTo = NDateIndexTo;
            searchModel.DateIndexDate = NDateIndexDate;
            searchModel.DateIndexToDate = NDateIndexToDate;
            searchModel.ResourceGroup = resourceGroup;

            WorkCalendar newWorkCalendar = genericMgr.FindAll<WorkCalendar>(" from WorkCalendar as w where  w.DateIndex=? and w.Flow=? and w.DateType=? ", new object[] { workCalendar.DateIndex, workCalendar.Flow, workCalendar.DateType })[0];
            if (workCalendar.DateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                newWorkCalendar.UpTime = 24 - (workCalendar.TrialTime + workCalendar.Holiday + workCalendar.HaltTime);
            }
            else if (workCalendar.DateType == com.Sconit.CodeMaster.TimeUnit.Week)
            {
                newWorkCalendar.UpTime = 7 - (workCalendar.TrialTime + workCalendar.Holiday + workCalendar.HaltTime);
            }
            else if (workCalendar.DateType == com.Sconit.CodeMaster.TimeUnit.Month)
            {
                string[] strArray = workCalendar.DateIndex.Split('-');
                int monthDay = DateTime.DaysInMonth(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]));
                newWorkCalendar.UpTime = monthDay - (workCalendar.TrialTime + workCalendar.Holiday + workCalendar.HaltTime);
            }
            newWorkCalendar.TrialTime = workCalendar.TrialTime;
            newWorkCalendar.Holiday = workCalendar.Holiday;
            newWorkCalendar.HaltTime = workCalendar.HaltTime;
            genericMgr.Update(newWorkCalendar);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<WorkCalendar>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_WorkCalendar_ExEdit,Url_Mrp_WorkCalendar_MiEdit,Url_Mrp_WorkCalendar_FiEdit")]
        public ActionResult _MiUpdate(GridCommand command, WorkCalendar workCalendar, string NDateIndexTo, string NDateIndex
            , string NProductLine, DateTime? NDateIndexDate, DateTime? NDateIndexToDate, com.Sconit.CodeMaster.TimeUnit NDateType)
        {
            WorkCalendarSearchModel searchModel = new WorkCalendarSearchModel();
            searchModel.ProductLine = NProductLine;
            searchModel.DateType = (short)NDateType;
            searchModel.DateIndex = NDateIndex;
            searchModel.DateIndexTo = NDateIndexTo;
            searchModel.DateIndexDate = NDateIndexDate;
            searchModel.DateIndexToDate = NDateIndexToDate;

            WorkCalendar newWorkCalendar = genericMgr.FindAll<WorkCalendar>(" from WorkCalendar as w where  w.DateIndex=? and w.Flow=? and w.DateType=? ", new object[] { workCalendar.DateIndex, workCalendar.Flow, workCalendar.DateType })[0];
            if (workCalendar.DateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                newWorkCalendar.UpTime = 24 - (workCalendar.TrialTime + workCalendar.Holiday + workCalendar.HaltTime);
            }
            else if (workCalendar.DateType == com.Sconit.CodeMaster.TimeUnit.Week)
            {
                newWorkCalendar.UpTime = 7 - (workCalendar.TrialTime + workCalendar.Holiday + workCalendar.HaltTime);
            }
            else if (workCalendar.DateType == com.Sconit.CodeMaster.TimeUnit.Month)
            {
                string[] strArray = workCalendar.DateIndex.Split('-');
                int monthDay = DateTime.DaysInMonth(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]));
                newWorkCalendar.UpTime = monthDay - (workCalendar.TrialTime + workCalendar.Holiday + workCalendar.HaltTime);
            }
            newWorkCalendar.TrialTime = workCalendar.TrialTime;
            newWorkCalendar.Holiday = workCalendar.Holiday;
            newWorkCalendar.HaltTime = workCalendar.HaltTime;
            genericMgr.Update(newWorkCalendar);
            SearchStatementModel searchStatementModel = PrepareSearchMIStatement(command, searchModel);
            return PartialView(GetAjaxPageData<WorkCalendar>(searchStatementModel, command));
        }
    }
}
