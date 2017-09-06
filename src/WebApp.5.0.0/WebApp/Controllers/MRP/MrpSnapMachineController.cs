using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Services.Transaction;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.SYS;
using System.Text;
using com.Sconit.Entity.MRP.TRANS;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MrpSnapMachineController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        private static string selectCountStatement = "select count(*) from Machine as m";

        private static string selectStatement = "select m from Machine as m";


        private static string selectCountInstanceStatement = "select count(*) from SnapMachine as m";

        private static string selectInstanceStatement = "select m from SnapMachine as m";

        //
        // GET: /ProdLineEx/

        #region MachineInstance
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public ActionResult InstanceIndex()
        {
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public ActionResult InstanceList(GridCommand command, MachineInstanceSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            ViewBag.DateType = searchModel.DateType == null ? null : searchModel.DateType.ToString();
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public ActionResult _AjaxInstanceList(GridCommand command, MachineInstanceSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareInstanceSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<SnapMachine>(searchStatementModel, command));
        }
        #region 导出后加工日历
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public void ExportXLS(MachineInstanceSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(com.Sconit.Entity.SYS.EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareInstanceSearchStatement(command, searchModel);
            var fileName = string.Format("FICalendar.xls");
            ExportToXLS<SnapMachine>(fileName, GetAjaxPageData<SnapMachine>(searchStatementModel, command).Data.ToList());
        }

        #endregion
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public ActionResult InstanceEdit(int id)
        {
            SnapMachine machineInstance = this.genericMgr.FindById<SnapMachine>(id);
                return View(machineInstance);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public ActionResult InstanceEdit(SnapMachine machineInstance)
        {
            if (ModelState.IsValid)
            {
                machineInstance.Region = genericMgr.FindById<Island>(machineInstance.Island).Region;
                machineInstance.SnapTime = genericMgr.FindById<SnapMachine>(machineInstance.Id).SnapTime;
                this.genericMgr.UpdateWithTrim(machineInstance);
                SaveSuccessMessage(Resources.MRP.MachineInstance.MachineInstance_Updated);
            }

            return View(machineInstance);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public ActionResult InstanceNew()
        {
            return View();
        }


        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public ActionResult InstanceNew(SnapMachine machineInstance)
        {
            try
            {
                if (machineInstance.DateType == com.Sconit.CodeMaster.TimeUnit.Day)
                {
                    ModelState.Remove("DateIndex");
                }
                if (ModelState.IsValid)
                {

                    if (machineInstance.DateType == com.Sconit.CodeMaster.TimeUnit.Day)
                    {
                        machineInstance.DateIndex = Convert.ToDateTime(machineInstance.DateIndexDate).ToString("yyyy-MM-dd");
                    }
                    if (this.genericMgr.FindAll<long>("select count(*)  from SnapMachine as m where m.Code=? and m.DateIndex=? and  m.DateType=? and  m.SnapTime=? ",
                         new object[] { machineInstance.Code, machineInstance.DateIndex, machineInstance.DateType, machineInstance.SnapTime })[0] > 0)
                    {
                        base.SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_AlreadyExistsSameMachineCodePlanTypeTypeTimeIndexVersion, machineInstance.Code, machineInstance.DateType, machineInstance.DateIndex, machineInstance.SnapTime));
                        return View(machineInstance);
                    }
                    machineInstance.Region = genericMgr.FindById<Island>(machineInstance.Island).Region;
 
                    this.genericMgr.CreateWithTrim(machineInstance);
                    SaveSuccessMessage(Resources.MRP.MachineInstance.MachineInstance_Added);
                    string code = machineInstance.Code;

                    string dateIndex = machineInstance.DateIndex;
                    int dateType = (int)machineInstance.DateType;
                    DateTime snapTime = machineInstance.SnapTime;
                    IList<object> iList;
                    string sql = "select top 1 id from MRP_SnapMachine where code=? and dateIndex=? and dateType=? and SnapTime=? ";
                    object[] para = new object[] { code, dateIndex, dateType, snapTime };
                    iList = queryMgr.FindAllWithNativeSql<object>(sql, para);
                    int id = (int)iList.FirstOrDefault();
                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "InstanceEdit" }, { "controller", "MrpSnapMachine" },
                     { "id", id }});
                }
            }
            catch (Exception e)
            {
                if (e is CommitResourceException)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_AlreadyExitsSameData);
                }

            }

            return View(machineInstance);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public ActionResult InstanceDelete(int id)
        {
                SnapMachine machineInstance = this.genericMgr.FindById<SnapMachine>(id);
                this.genericMgr.Delete(machineInstance);
                SaveSuccessMessage(Resources.MRP.MachineInstance.MachineInstance_Deleted);
                return RedirectToAction("InstanceList");
        }


        private SearchStatementModel PrepareInstanceSearchStatement(GridCommand command, MachineInstanceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            if (searchModel.DateType != null)
            {
                if (searchModel.DateType.Value == 4)
                {
                    if (searchModel.DateIndexDate != null & searchModel.DateIndexToDate != null)
                    {
                        HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.DateIndexDate.Value.ToString("yyyy-MM-dd"), searchModel.DateIndexToDate.Value.ToString("yyyy-MM-dd"), "m", ref whereStatement, param);
                    }
                    else if (searchModel.DateIndexDate != null & searchModel.DateIndexToDate == null)
                    {
                        HqlStatementHelper.AddGeStatement("DateIndex", searchModel.DateIndexDate.Value.ToString("yyyy-MM-dd"), "m", ref whereStatement, param);
                    }
                    else if (searchModel.DateIndexDate == null & searchModel.DateIndexToDate != null)
                    {
                        HqlStatementHelper.AddLeStatement("DateIndex", searchModel.DateIndexToDate.Value.ToString("yyyy-MM-dd"), "m", ref whereStatement, param);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(searchModel.DateIndex) & !string.IsNullOrEmpty(searchModel.DateIndexTo))
                    {
                        HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.DateIndex, searchModel.DateIndexTo, "m", ref whereStatement, param);
                    }
                    else if (!string.IsNullOrEmpty(searchModel.DateIndex) & string.IsNullOrEmpty(searchModel.DateIndexTo))
                    {
                        HqlStatementHelper.AddGeStatement("DateIndex", searchModel.DateIndex, "m", ref whereStatement, param);
                    }
                    else if (string.IsNullOrEmpty(searchModel.DateIndex) & !string.IsNullOrEmpty(searchModel.DateIndexTo))
                    {
                        HqlStatementHelper.AddLeStatement("DateIndex", searchModel.DateIndexTo, "m", ref whereStatement, param);
                    }
                }
            }

            HqlStatementHelper.AddEqStatement("Code", searchModel.Code, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MachineType", searchModel.MachineType, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("SnapTime", searchModel.SnapTime, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ShiftType", searchModel.ShiftType, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("DateType", searchModel.DateType, "m", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by m.Code desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountInstanceStatement;
            searchStatementModel.SelectStatement = selectInstanceStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public JsonResult _MachineJson(string code)
        {
            IList<Machine> machineList = genericMgr.FindAll<Machine>("from Machine as p where p.Code=? ",code);
            Machine machine = machineList[0];
            Island island = genericMgr.FindById<Island>(machine.Island);
            machine.IslandQty = island.Qty;
            machine.IslandDescription = island.Description;

            IList<CodeDetail> codeDetailList = genericMgr.FindAll<CodeDetail>(" from CodeDetail as c where c.Code='ShiftType' and c.Value=?", new object[] { (int)machine.ShiftType });
            IList<CodeDetail> codeDetailListTo = genericMgr.FindAll<CodeDetail>(" from CodeDetail as c where c.Code='MachineType' and c.Value=?", new object[] { (int)machine.MachineType });


            if (codeDetailList.Count > 0)
            {
                machine.ShiftTypeDescription = Resources.SYS.CodeDetail.ResourceManager.GetString(codeDetailList[0].Description);
                machine.ShiftTypeDescription = machine.ShiftTypeDescription != null ? machine.ShiftTypeDescription : codeDetailList[0].Description;
            }
            if (codeDetailListTo.Count > 0)
            {
                machine.MachineTypeDescription = Resources.SYS.CodeDetail.ResourceManager.GetString(codeDetailListTo[0].Description);
                machine.MachineTypeDescription = machine.MachineTypeDescription != null ? machine.MachineTypeDescription : codeDetailListTo[0].Description;
            }

            return new JsonResult { Data = machine };
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineAdjust")]
        public JsonResult _IslandJson(string code)
        {
            Island island = genericMgr.FindById<Island>(code);
            return new JsonResult { Data = island };
        }


        #endregion

        #region
        #region Machine
        #region Public Method
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_MRP_Machine_View")]
        public ActionResult List(GridCommand command, MachineSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MRP_Machine_View")]
        public ActionResult _AjaxList(GridCommand command, MachineSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Machine>(searchStatementModel, command));
        }
        #endregion

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MRP_Machine_View")]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            else
            {
                Machine machine = this.genericMgr.FindById<Machine>(id);
                return View(machine);
            }
        }

        [SconitAuthorize(Permissions = "Url_MRP_Machine_View")]
        public ActionResult Edit(Machine machine)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(machine);
                SaveSuccessMessage(Resources.MRP.Machine.Machine_Updated);
            }

            return View(machine);
        }


        [SconitAuthorize(Permissions = "Url_MRP_Machine_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MRP_Machine_View")]
        public ActionResult New(Machine machine)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this.genericMgr.CreateWithTrim(machine);
                    SaveSuccessMessage(Resources.MRP.Machine.Machine_Added);
                    string Code = machine.Code;

                    //return RedirectToAction("Edit", new object[] { ProductLine, Item });
                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "MrpSnapMachine" }, { "Code", Code } });
                }
            }
            catch (Exception e)
            {
                if (e is CommitResourceException)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheMachineAlreadyExits);
                }

            }

            return View(machine);
        }

        [SconitAuthorize(Permissions = "Url_MRP_Machine_View")]
        public ActionResult Delete(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return HttpNotFound();
            }
            else
            {
                Machine machine = this.genericMgr.FindAll<Machine>("select m from Machine as m where m.Code=? ", new object[] { Code })[0];
                this.genericMgr.Delete(machine);
                SaveSuccessMessage(Resources.MRP.Machine.Machine_Deleted);
                return RedirectToAction("List");

            }
        }

        #endregion
        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, MachineSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Code", searchModel.Code, "m", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MachineType", searchModel.MachineType, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ShiftType", searchModel.ShiftType, "m", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by m.CreateDate desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion

        #region View
        [SconitAuthorize(Permissions = "Url_MRP_MachineInstanceView_View")]
        public ActionResult InstanceViewIndex()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            return View();
        }
        [SconitAuthorize(Permissions = "Url_MRP_MachineInstanceView_View")]
        public string _GetMachineInstanceView(DateTime startTime, string dateIndex, com.Sconit.CodeMaster.TimeUnit dateType,
          string code, string island, bool isShiftQuota, bool isShiftPerDay, bool isNormalWorkDayPerWeek, bool isMaxWorkDayPerWeek
            , bool isQty, bool isIslandQty, bool isShiftType, bool isMachineType, int checkboxcheckedCount)
        {
            IList<object> param = new List<object>();
            string startDateIndex = string.Empty;
            string endDateIndex = string.Empty;
            if (dateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                startDateIndex = startTime.ToString("yyyy-MM-dd");
                endDateIndex = startTime.AddDays(14).ToString("yyyy-MM-dd");
            }
            else if (dateType == com.Sconit.CodeMaster.TimeUnit.Week)
            {
                startDateIndex = dateIndex;
                endDateIndex = Utility.DateTimeHelper.GetWeekOfYear((Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex).AddDays(7 * 16)));
            }
            else if (dateType == com.Sconit.CodeMaster.TimeUnit.Month)
            {
                startDateIndex = dateIndex;
                endDateIndex = DateTime.Parse(dateIndex + "-01").AddMonths(12).ToString("yyyy-MM");
            }

            string hql = "  from SnapMachine as m where m.DateType=? and  DateIndex between ? and ? ";
            param.Add(dateType);
            param.Add(startDateIndex);
            param.Add(endDateIndex);
            if (!string.IsNullOrEmpty(code))
            {
                hql += " and m.Code=?";
                param.Add(code);
            }
            if (!string.IsNullOrEmpty(island))
            {
                hql += " and m.Island=?";
                param.Add(island);
            }
            IList<SnapMachine> machineInstanceList = genericMgr.FindAll<SnapMachine>(hql, param.ToArray());
            foreach (var machineInstance in machineInstanceList)
            {
                machineInstance.MachineTypeDescription = (int)machineInstance.MachineType + "[" + systemMgr.GetCodeDetailDescription(CodeMaster.CodeMaster.MachineType, (int)machineInstance.MachineType) + "]";
                machineInstance.ShiftTypeDescription = (int)machineInstance.ShiftType + "[" + systemMgr.GetCodeDetailDescription(CodeMaster.CodeMaster.ShiftType, (int)machineInstance.ShiftType) + "]";
            }

            return GetStringMachineInstance(machineInstanceList, dateType, startTime, dateIndex, isShiftQuota, isShiftPerDay, isNormalWorkDayPerWeek, isMaxWorkDayPerWeek
                , isQty, isIslandQty, isShiftType, isMachineType, checkboxcheckedCount);
        }


        private string GetStringMachineInstance(IList<SnapMachine> machineInstanceList, com.Sconit.CodeMaster.TimeUnit DateType,
            DateTime startTime, string dataIndex, bool isShiftQuota, bool isShiftPerDay, bool isNormalWorkDayPerWeek, bool isMaxWorkDayPerWeek
            , bool isQty, bool isIslandQty, bool isShiftType, bool isMachineType, int checkboxcheckedCount)
        {
            if (machineInstanceList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var firstMachineInstance = machineInstanceList.First();
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head

            str.Append("<th>");
            str.Append(Resources.MRP.MachineInstance.MachineInstance_Island);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MRP.MachineInstance.MachineInstance_Code);
            str.Append("</th>");



            str.Append("<th>");
            str.Append(Resources.MRP.MachineInstance.MachineInstance_Type);
            str.Append("</th>");

            if (DateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                //14天
                DateTime dt = startTime;
                for (int i = 0; i < 14; i++)
                {
                    str.Append("<th>");
                    str.Append(dt.ToString("MM-dd"));
                    str.Append("</th>");
                    dt = dt.AddDays(1);
                }
            }
            else if (DateType == com.Sconit.CodeMaster.TimeUnit.Month)
            {
                //12月
                string[] strArray = dataIndex.Split('-');
                int years = Convert.ToInt32(strArray[0]);
                int month = Convert.ToInt32(strArray[1]);
                if (month == 12)
                {
                    month = 1;
                    ++years;
                }
                for (int i = 0; i < 12; i++)
                {
                    str.Append("<th >");
                    str.Append(years + "-" + month.ToString("D2"));
                    str.Append("</th>");
                    if (month == 12)
                    {
                        month = 0;
                        ++years;
                    }
                    month++;
                }
            }
            else
            {
                //16周
                string[] wky = dataIndex.Split('-');
                int weekIndex = int.Parse(wky[1]);
                int yearIndex = int.Parse(wky[0]);
                string newWeekOfyear = string.Empty;
                for (int i = 0; i < 16; i++)
                {
                    if (weekIndex <= 0)
                    {
                        newWeekOfyear = (yearIndex - 1).ToString();
                        newWeekOfyear += "-" + (52 + weekIndex).ToString("D2");
                    }
                    else if (weekIndex > 52)
                    {
                        newWeekOfyear = (yearIndex + 1).ToString();
                        newWeekOfyear += "-" + (weekIndex - 52).ToString("D2");
                    }
                    else
                    {
                        newWeekOfyear = yearIndex.ToString();
                        newWeekOfyear += "-" + weekIndex.ToString("D2");
                    }
                    str.Append("<th>");
                    str.Append(newWeekOfyear);
                    str.Append("</th>");
                    weekIndex++;



                }
            }

            #endregion

            str.Append("</tr></thead><tbody>");

            if (machineInstanceList != null && machineInstanceList.Count > 0)
            {
                var machineInstanceListGruopby = from mi in machineInstanceList
                                                 group mi by
                                                 new
                                                 {
                                                     Code = mi.Code,
                                                     Island = mi.Island,
                                                 } into g
                                                 select new
                                                 {
                                                     Code = g.Key.Code,
                                                     Description = g.First().Description,
                                                     Island = g.Key.Island,
                                                     List = g
                                                 };
                int l = 0;
                foreach (var machineInstancegroup in machineInstanceListGruopby)
                {
                    l++;
                    #region  一个多少行
                    for (int k = 0; k < 8; k++)
                    {
                        #region
                        if (l % 2 == 0)
                        {
                            if (k == 0)
                            {
                                if (isShiftQuota)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            if (k == 1)
                            {
                                if (isShiftPerDay)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 2)
                            {
                                if (isNormalWorkDayPerWeek)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 3)
                            {
                                if (isMaxWorkDayPerWeek)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 4)
                            {
                                if (isQty)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 5)
                            {
                                if (isIslandQty)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 6)
                            {
                                if (isShiftType)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 7)
                            {
                                if (isMachineType)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                        }
                        else
                        {
                            if (k == 0)
                            {
                                if (isShiftQuota)
                                {
                                    str.Append("<tr>");
                                }
                            }

                            else if (k == 1)
                            {
                                if (isShiftPerDay)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 2)
                            {
                                if (isNormalWorkDayPerWeek)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 3)
                            {
                                if (isMaxWorkDayPerWeek)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 4)
                            {
                                if (isQty)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 5)
                            {
                                if (isIslandQty)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 6)
                            {
                                if (isShiftType)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 7)
                            {
                                if (isMachineType)
                                {
                                    str.Append("<tr>");
                                }
                            }
                        }
                        if (k == 0)
                        {

                            if (isShiftQuota)
                            {
                                str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                str.Append(machineInstancegroup.Island);
                                str.Append("</td>");
                                str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                str.Append(machineInstancegroup.Code + "<br>" + machineInstancegroup.Description);
                                str.Append("</td>");
                                str.Append("<td>");
                                str.Append(Resources.MRP.MachineInstance.MachineInstance_ShiftQuota);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 1)
                        {
                            if (isShiftPerDay)
                            {
                                if (!isShiftQuota)
                                {
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Island);
                                    str.Append("</td>");
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Code + "<br>" + machineInstancegroup.Description);
                                    str.Append("</td>");

                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.MachineInstance.MachineInstance_ShiftPerDay);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 2)
                        {
                            if (isNormalWorkDayPerWeek)
                            {
                                if (!isShiftQuota && !isShiftPerDay)
                                {
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Island);
                                    str.Append("</td>");
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Code + "<br>" + machineInstancegroup.Description);
                                    str.Append("</td>");

                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.MachineInstance.MachineInstance_NormalWorkDayPerWeek);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 3)
                        {
                            if (isMaxWorkDayPerWeek)
                            {
                                if (!isShiftQuota && !isShiftPerDay && !isNormalWorkDayPerWeek)
                                {
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Island);
                                    str.Append("</td>");
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Code + "<br>" + machineInstancegroup.Description);
                                    str.Append("</td>");

                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.MachineInstance.MachineInstance_MaxWorkDayPerWeek);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 4)
                        {
                            if (isQty)
                            {
                                if (!isShiftQuota && !isShiftPerDay && !isNormalWorkDayPerWeek && !isMaxWorkDayPerWeek)
                                {
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Island);
                                    str.Append("</td>");
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Code + "<br>" + machineInstancegroup.Description);
                                    str.Append("</td>");

                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.MachineInstance.MachineInstance_Qty);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 5)
                        {
                            if (isIslandQty)
                            {
                                if (!isShiftQuota && !isShiftPerDay && !isNormalWorkDayPerWeek && !isMaxWorkDayPerWeek && !isQty)
                                {
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Island);
                                    str.Append("</td>");
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Code + "<br>" + machineInstancegroup.Description);
                                    str.Append("</td>");

                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.MachineInstance.MachineInstance_IslandQty);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 6)
                        {
                            if (isShiftType)
                            {
                                if (!isShiftQuota && !isShiftPerDay && !isNormalWorkDayPerWeek && !isMaxWorkDayPerWeek && !isQty && !isIslandQty)
                                {
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Island);
                                    str.Append("</td>");
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Code + "<br>" + machineInstancegroup.Description);
                                    str.Append("</td>");

                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.MachineInstance.MachineInstance_ShiftType);
                                str.Append("</td>");
                            }
                        }
                        else
                        {
                            if (isMachineType)
                            {
                                if (!isShiftQuota && !isShiftPerDay && !isNormalWorkDayPerWeek && !isMaxWorkDayPerWeek && !isQty && !isIslandQty && !isShiftType)
                                {
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Island);
                                    str.Append("</td>");
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(machineInstancegroup.Code + "<br>" + machineInstancegroup.Description);
                                    str.Append("</td>");

                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.MachineInstance.MachineInstance_MachineType);
                                str.Append("</td>");
                            }
                        }
                        #endregion

                        #region 如果是天
                        if (DateType == com.Sconit.CodeMaster.TimeUnit.Day)
                        {
                            //14天
                            DateTime dt = startTime;
                            for (int j = 0; j < 14; j++)
                            {

                                var machineInstanceFirst = machineInstancegroup.List.FirstOrDefault(m => m.DateIndex == dt.ToString("yyyy-MM-dd") & m.Code == machineInstancegroup.Code);
                                if (k == 0)
                                {
                                    if (isShiftQuota)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftQuota);
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

                                else if (k == 1)
                                {
                                    if (isShiftPerDay)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftPerDay);
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

                                else if (k == 2)
                                {
                                    if (isNormalWorkDayPerWeek)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.NormalWorkDayPerWeek);
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

                                else if (k == 3)
                                {
                                    if (isMaxWorkDayPerWeek)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.MaxWorkDayPerWeek);
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

                                else if (k == 4)
                                {
                                    if (isQty)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.Qty);
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


                                else if (k == 5)
                                {
                                    if (isIslandQty)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.IslandQty);
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
                                else if (k == 6)
                                {
                                    if (isShiftType)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                else if (k == 7)
                                {
                                    if (isMachineType)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.MachineTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                dt = dt.AddDays(1);

                            }

                        }
                        #endregion
                        #region 如果是年月
                        else if (DateType == com.Sconit.CodeMaster.TimeUnit.Month)
                        {
                            //12月
                            string[] strArray = dataIndex.Split('-');
                            int years = Convert.ToInt32(strArray[0]);
                            int month = Convert.ToInt32(strArray[1]);
                            if (month == 12)
                            {
                                month = 1;
                                ++years;
                            }
                            for (int j = 0; j < 12; j++)
                            {
                                string yearsmonthTime = years + "-" + month.ToString("D2");
                                var machineInstanceFirst = machineInstancegroup.List.FirstOrDefault(m => m.DateIndex == yearsmonthTime & m.Code == machineInstancegroup.Code);
                                if (k == 0)
                                {
                                    if (isShiftQuota)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftQuota);
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

                                else if (k == 1)
                                {
                                    if (isShiftPerDay)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftPerDay);
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

                                else if (k == 2)
                                {
                                    if (isNormalWorkDayPerWeek)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.NormalWorkDayPerWeek);
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

                                else if (k == 3)
                                {
                                    if (isMaxWorkDayPerWeek)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.MaxWorkDayPerWeek);
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

                                else if (k == 4)
                                {
                                    if (isQty)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.Qty);
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


                                else if (k == 5)
                                {
                                    if (isIslandQty)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.IslandQty);
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
                                else if (k == 6)
                                {
                                    if (isShiftType)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                else if (k == 7)
                                {
                                    if (isMachineType)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.MachineTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                if (month == 12)
                                {
                                    month = 0;
                                    ++years;
                                }
                                month++;
                            }
                        }
                        #endregion
                        #region 如果是周
                        else
                        {
                            //16周
                            string[] wky = dataIndex.Split('-');
                            int weekIndex = int.Parse(wky[1]);
                            int yearIndex = int.Parse(wky[0]);
                            string newWeekOfyear = string.Empty;
                            for (int j = 0; j < 16; j++)
                            {
                                if (weekIndex <= 0)
                                {
                                    newWeekOfyear = (yearIndex - 1).ToString();
                                    newWeekOfyear += "-" + (52 + weekIndex).ToString("D2");
                                }
                                else if (weekIndex > 52)
                                {
                                    newWeekOfyear = (yearIndex + 1).ToString();
                                    newWeekOfyear += "-" + (weekIndex - 52).ToString("D2");
                                }
                                else
                                {
                                    newWeekOfyear = yearIndex.ToString();
                                    newWeekOfyear += "-" + weekIndex.ToString("D2");
                                }



                                var machineInstanceFirst = machineInstancegroup.List.FirstOrDefault(m => m.DateIndex == newWeekOfyear & m.Code == machineInstancegroup.Code);
                                if (k == 0)
                                {
                                    if (isShiftQuota)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftQuota);
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

                                else if (k == 1)
                                {
                                    if (isShiftPerDay)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftPerDay);
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

                                else if (k == 2)
                                {
                                    if (isNormalWorkDayPerWeek)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.NormalWorkDayPerWeek);
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

                                else if (k == 3)
                                {
                                    if (isMaxWorkDayPerWeek)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.MaxWorkDayPerWeek);
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

                                else if (k == 4)
                                {
                                    if (isQty)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.Qty);
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


                                else if (k == 5)
                                {
                                    if (isIslandQty)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.IslandQty);
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
                                else if (k == 6)
                                {
                                    if (isShiftType)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.ShiftTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                else if (k == 7)
                                {
                                    if (isMachineType)
                                    {
                                        if (machineInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(machineInstanceFirst.MachineTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                weekIndex++;



                            }
                        }
                        #endregion

                        if (k == 1)
                        {
                            if (isShiftPerDay)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 2)
                        {
                            if (isNormalWorkDayPerWeek)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 3)
                        {
                            if (isMaxWorkDayPerWeek)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 4)
                        {
                            if (isQty)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 5)
                        {
                            if (isIslandQty)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 6)
                        {
                            if (isShiftType)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 7)
                        {
                            if (isMachineType)
                            {
                                str.Append("</tr>");
                            }
                        }


                    }
                    #endregion


                }

                //表尾
                str.Append("</tbody>");
                str.Append("</table>");

            }


            return str.ToString();
        }

        #endregion View View


    }
}
