using System.Collections.Generic;
using System.Linq;
using Castle.Services.Transaction;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.PRD;
using NHibernate;
using com.Sconit.Entity.VIEW;
using System;
using NHibernate.Criterion;
using AutoMapper;
using com.Sconit.CodeMaster;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class WorkingCalendarMgrImpl : BaseMgr, IWorkingCalendarMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public IQueryMgr queryMgr { get; set; }

        private static string selectWorkingShiftByWorkingCalendarId = "select w from WorkingShift as w where w.WorkingCalendar = ?";
        private static string deleteShiftDetailsByShift = "from ShiftDetail where Shift = ?";
        private static string deleteShiftMaster = "from ShiftMaster where Code = ?";
        private static string selectWorkingCalendar = "select w from WorkingCalendar as w where w.Region is null and w.Flow is null";
        private static string selectWorkingCalendarByRegion = "select w from WorkingCalendar as w where w.Region = ? and w.Flow is null";
        private static string selectWorkingCalendarByFlow = "select w from WorkingCalendar as w where w.Flow = ?";

        private static string selectSpecialTime = "select s from SpecialTime as s where s.Region is null and s.Flow is null and StartTime <= ? and EndTime >= ?";
        private static string selectSpecialTimeByRegion = "select s from SpecialTime as s where s.Region = ? and s.Flow is null and StartTime <= ? and EndTime >= ?";
        private static string selectSpecialTimeByFlow = "select s from SpecialTime as s where s.Flow = ? and StartTime <= ? and EndTime >= ?";

        #region public methods
        [Transaction(TransactionMode.Requires)]
        public void DeleteShiftMaster(string shiftMasterCode)
        {
            IList<ShiftDetail> shiftDetailList = genericMgr.FindAll<ShiftDetail>(deleteShiftDetailsByShift, shiftMasterCode);
            IList<ShiftMaster> shiftMasterList = genericMgr.FindAll<ShiftMaster>(deleteShiftMaster, shiftMasterCode);
            this.genericMgr.Delete(shiftDetailList);
            this.genericMgr.Delete(shiftMasterList);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateWorkingCalendar(WorkingCalendar workingCalendar, IList<string> ShiftList)
        {
            #region 保存工作日历头
            this.genericMgr.Update(workingCalendar);
            #endregion

            #region 保存工作日历明细
            IList<ShiftMaster> assignedShiftMasterList = new List<ShiftMaster>();

            if (ShiftList != null && ShiftList.Count > 0)
            {
                assignedShiftMasterList = (from code in ShiftList
                                           select new ShiftMaster
                                            {
                                                Code = code
                                            }).ToList();
            }

            IList<WorkingShift> oldAssingedWorkingShiftList = this.genericMgr.FindAll<WorkingShift>(selectWorkingShiftByWorkingCalendarId, workingCalendar.Id);
            IList<ShiftMaster> oldAssingedShiftMasterList = new List<ShiftMaster>();

            if (oldAssingedWorkingShiftList != null && oldAssingedWorkingShiftList.Count > 0)
            {
                oldAssingedShiftMasterList = (from ws in oldAssingedWorkingShiftList
                                              select new ShiftMaster
                                           {
                                               Code = ws.Shift.Code
                                           }).ToList();
            }

            #region 删除没有授权的班次
            IList<ShiftMaster> deleteShiftMasterList = oldAssingedShiftMasterList.Except<ShiftMaster>(assignedShiftMasterList).ToList();
            if (deleteShiftMasterList.Count > 0)
            {
                foreach (ShiftMaster shiftMaster in deleteShiftMasterList)
                {
                    WorkingShift deletingWorkingShift = oldAssingedWorkingShiftList.Where(ur => ur.Shift.Code == shiftMaster.Code).SingleOrDefault();
                    if (deletingWorkingShift != null)
                    {
                        this.genericMgr.Delete(deletingWorkingShift);
                    }
                }
            }
            #endregion

            #region 保存新增授权的班次
            IList<ShiftMaster> insertingShiftMasterList = assignedShiftMasterList.Except<ShiftMaster>(oldAssingedShiftMasterList).ToList();
            if (insertingShiftMasterList.Count > 0)
            {
                IList<WorkingShift> insertingWorkingShiftList = (from shiftMaster in insertingShiftMasterList
                                                                 select new WorkingShift
                                                                 {
                                                                     WorkingCalendar = workingCalendar.Id,
                                                                     Shift = shiftMaster
                                                                 }).ToList();

                foreach (WorkingShift workingShift in insertingWorkingShiftList)
                {
                    this.genericMgr.Create(workingShift);
                }
            }
            #endregion
            #endregion
        }

        public IList<WorkingCalendarView> GetWorkingCalendarViewList(string regionCode, string flowCode, DateTime dateFrom, DateTime dateTo)
        {
            #region 获取全局的工作日历
            IList<WorkingCalendar> workingCalendarList = this.queryMgr.FindAll<WorkingCalendar>(selectWorkingCalendar);
            #endregion

            if (string.IsNullOrWhiteSpace(regionCode) && !string.IsNullOrWhiteSpace(flowCode))
            {
                regionCode = this.genericMgr.FindById<com.Sconit.Entity.SCM.FlowMaster>(flowCode).PartyFrom;
            }

            #region 合并Region和全局的工作日历
            if (!string.IsNullOrEmpty(regionCode))
            {
                IList<WorkingCalendar> regionWorkingCalendarList = this.queryMgr.FindAll<WorkingCalendar>(selectWorkingCalendarByRegion, regionCode);

                foreach (var regionWorkingCalendar in regionWorkingCalendarList)
                {
                    var currentWorkingCalendar = workingCalendarList.FirstOrDefault(w => w.DayOfWeek == regionWorkingCalendar.DayOfWeek);
                    if (currentWorkingCalendar != null)
                    {
                        workingCalendarList.Remove(currentWorkingCalendar);
                    }
                    workingCalendarList.Add(regionWorkingCalendar);
                }
                workingCalendarList = workingCalendarList.OrderBy(w => w.DayOfWeek).ToList();
            }
            #endregion

            #region 合并Flow的工作日历
            if (!string.IsNullOrEmpty(regionCode) && !string.IsNullOrEmpty(flowCode))
            {
                IList<WorkingCalendar> flowWorkingCalendarList = this.queryMgr.FindAll<WorkingCalendar>(selectWorkingCalendarByFlow, flowCode);

                foreach (var flowWorkingCalendar in flowWorkingCalendarList)
                {
                    var currentWorkingCalendar = workingCalendarList.FirstOrDefault(w => w.DayOfWeek == flowWorkingCalendar.DayOfWeek);
                    if (currentWorkingCalendar != null)
                    {
                        workingCalendarList.Remove(currentWorkingCalendar);
                    }
                    workingCalendarList.Add(flowWorkingCalendar);
                }
                workingCalendarList = workingCalendarList.OrderBy(w => w.DayOfWeek).ToList();
            }
            #endregion

            if (workingCalendarList != null && workingCalendarList.Count > 0)
            {
                #region 获取工作班次
                DetachedCriteria criteria = DetachedCriteria.For<WorkingShift>();
                criteria.Add(Expression.In("WorkingCalendar", (from w in workingCalendarList select w.Id).ToArray()));
                IList<WorkingShift> workingShiftList = this.queryMgr.FindAll<WorkingShift>(criteria).Distinct().ToList();
                #endregion

                #region 获取班次明细
                criteria = DetachedCriteria.For<ShiftDetail>();
                criteria.Add(Expression.In("Shift", (from w in workingShiftList select w.Shift.Code).ToArray()));

                IList<ShiftDetail> shiftDetailList = this.queryMgr.FindAll<ShiftDetail>(criteria);
                #endregion

                IList<SpecialTime> specialTimeList = GetSpecialTime(regionCode, flowCode, dateFrom, dateTo);

                #region 循环查询日期区间获取工作和休息信息
                WorkingCalendarView firstWorkingCalendarView = null;
                WorkingCalendarView lastWorkingCalendarView = null;
                List<WorkingCalendarView> workingCalendarViewList = new List<WorkingCalendarView>();
                DateTime cycleDateTime = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day);
                for (; DateTime.Compare(cycleDateTime, dateTo) <= 0; cycleDateTime = cycleDateTime.AddDays(1))
                {
                    #region 获取当前是星期几
                    WorkingCalendar workingCalendar = (from w in workingCalendarList
                                                       where w.DayOfWeek == cycleDateTime.DayOfWeek
                                                       select w).SingleOrDefault();
                    #endregion

                    if (workingCalendar != null)
                    {
                        #region 获取班次明细
                        IList<WorkingShift> currentWorkingShiftList = (from w in workingShiftList
                                                                       where w.WorkingCalendar == workingCalendar.Id
                                                                       select w).ToList();

                        IList<ShiftDetail> currentShiftDetailList = (from w in currentWorkingShiftList
                                                                     join s in shiftDetailList on w.Shift.Code equals s.Shift
                                                                     where (!s.StartDate.HasValue || s.StartDate <= cycleDateTime)
                                                                            && (!s.EndDate.HasValue || s.EndDate >= cycleDateTime)
                                                                     select s).OrderBy(s => int.Parse(s.ShiftTime.Split(ShiftDetail.ShiftTimeSplitSymbol)[0].Split(':')[0])).ToList();
                        #endregion

                        if (currentShiftDetailList.Count > 0)
                        {
                            #region 处理工作日,循环班次明细
                            foreach (ShiftDetail currentShiftDetail in currentShiftDetailList)
                            {
                                WorkingCalendarView currentWorkingCalendarView = new WorkingCalendarView();
                                currentWorkingCalendarView.ShiftCode = currentShiftDetail.Shift;
                                currentWorkingCalendarView.ShiftName = (from w in currentWorkingShiftList where w.Shift.Code == currentShiftDetail.Shift select w.Shift.Name).Single();
                                currentWorkingCalendarView.DayOfWeek = cycleDateTime.DayOfWeek;
                                currentWorkingCalendarView.Date = cycleDateTime;
                                string[] splitedShiftTime = currentShiftDetail.ShiftTime.Split(ShiftDetail.ShiftTimeSplitSymbol);
                                currentWorkingCalendarView.DateFrom = Convert.ToDateTime(cycleDateTime.ToString("yyyy-MM-dd") + " " + splitedShiftTime[0]);
                                currentWorkingCalendarView.DateTo = Convert.ToDateTime(cycleDateTime.ToString("yyyy-MM-dd") + " " + splitedShiftTime[1]);
                                if (DateTime.Compare(currentWorkingCalendarView.DateFrom, currentWorkingCalendarView.DateTo) >= 0)
                                {
                                    //如果开始日期大于等于结束日期，结束日期+1
                                    currentWorkingCalendarView.DateTo = currentWorkingCalendarView.DateTo.AddDays(1);
                                }
                                currentWorkingCalendarView.Type = workingCalendar.Type;

                                lastWorkingCalendarView = currentWorkingCalendarView;
                                workingCalendarViewList.Add(currentWorkingCalendarView);
                            }
                            #endregion
                        }
                        else
                        {
                            #region 休息日
                            if (lastWorkingCalendarView != null)
                            {
                                #region 如果上次的工作日历的结束时间小于当前的工作日历，插入一条休息日记录
                                if (DateTime.Compare(lastWorkingCalendarView.DateTo, cycleDateTime.Date) < 0)
                                {
                                    WorkingCalendarView insertedWorkingCalendarView = new WorkingCalendarView();
                                    insertedWorkingCalendarView.DayOfWeek = lastWorkingCalendarView.DayOfWeek;
                                    insertedWorkingCalendarView.Date = lastWorkingCalendarView.Date;
                                    insertedWorkingCalendarView.DateFrom = lastWorkingCalendarView.DateTo;
                                    insertedWorkingCalendarView.DateTo = cycleDateTime;
                                    insertedWorkingCalendarView.Type = com.Sconit.CodeMaster.WorkingCalendarType.Rest;

                                    lastWorkingCalendarView = insertedWorkingCalendarView;
                                    workingCalendarViewList.Add(lastWorkingCalendarView);
                                }
                                #endregion

                                #region 新增休息日记录
                                WorkingCalendarView currentWorkingCalendarView = new WorkingCalendarView();
                                currentWorkingCalendarView.DayOfWeek = cycleDateTime.DayOfWeek;
                                currentWorkingCalendarView.Date = cycleDateTime;
                                currentWorkingCalendarView.DateFrom = lastWorkingCalendarView.DateTo;
                                currentWorkingCalendarView.DateTo = cycleDateTime.AddDays(1);
                                currentWorkingCalendarView.Type = com.Sconit.CodeMaster.WorkingCalendarType.Rest;

                                lastWorkingCalendarView = currentWorkingCalendarView;
                                workingCalendarViewList.Add(currentWorkingCalendarView);
                                #endregion
                            }
                            #endregion
                        }

                        #region 如果第一条工作日历不是从DateFrom开始，新增工作日历补齐
                        if (firstWorkingCalendarView == null && workingCalendarViewList.Count > 0)
                        {
                            firstWorkingCalendarView = workingCalendarViewList.OrderBy(c => c.DateFrom).Take(1).Single();
                        }

                        if (firstWorkingCalendarView == null || firstWorkingCalendarView.DateFrom > dateFrom)
                        {
                            #region 新增昨天的隔夜班次
                            WorkingCalendarView overNightWorkingCalendarView = null;
                            #region 查找昨天星期几
                            WorkingCalendar lastWorkingCalendar = (from w in workingCalendarList
                                                                   where w.DayOfWeek == cycleDateTime.AddDays(-1).DayOfWeek
                                                                   select w).SingleOrDefault();
                            #endregion
                            if (lastWorkingCalendar != null)
                            {
                                IList<WorkingShift> lastWorkingShiftList = (from w in workingShiftList
                                                                            where w.WorkingCalendar == lastWorkingCalendar.Id
                                                                            select w).ToList();

                                IList<ShiftDetail> lastShiftDetailList = (from w in lastWorkingShiftList
                                                                          join s in shiftDetailList on w.Shift.Code equals s.Shift
                                                                          where (!s.StartDate.HasValue || s.StartDate <= cycleDateTime)
                                                                                 && (!s.EndDate.HasValue || s.EndDate >= cycleDateTime)
                                                                          select s).OrderBy(s => s.ShiftTime).ToList();

                                if (lastShiftDetailList != null && lastShiftDetailList.Count > 0)
                                {
                                    foreach (ShiftDetail lastShiftDetail in lastShiftDetailList)
                                    {
                                        string[] splitedShiftTime = lastShiftDetail.ShiftTime.Split(ShiftDetail.ShiftTimeSplitSymbol);

                                        DateTime shiftDateFrom = Convert.ToDateTime(cycleDateTime.ToString("yyyy-MM-dd") + " " + splitedShiftTime[0]);
                                        DateTime shiftDateTo = Convert.ToDateTime(cycleDateTime.ToString("yyyy-MM-dd") + " " + splitedShiftTime[1]);
                                        if (DateTime.Compare(shiftDateFrom, shiftDateTo) >= 0  //如果开始日期大于等于结束日期，代表为隔夜的班次
                                            && shiftDateTo > dateFrom)
                                        {
                                            DateTime lastDate = cycleDateTime.AddDays(-1);
                                            overNightWorkingCalendarView = new WorkingCalendarView();
                                            overNightWorkingCalendarView.ShiftCode = lastShiftDetail.Shift;
                                            overNightWorkingCalendarView.ShiftName = (from w in lastWorkingShiftList where w.Shift.Code == lastShiftDetail.Shift select w.Shift.Name).Single();
                                            overNightWorkingCalendarView.DayOfWeek = lastDate.DayOfWeek;
                                            overNightWorkingCalendarView.Date = lastDate;
                                            overNightWorkingCalendarView.DateFrom = dateFrom;
                                            overNightWorkingCalendarView.DateTo = shiftDateTo;
                                            overNightWorkingCalendarView.Type = lastWorkingCalendar.Type;

                                            workingCalendarViewList.Add(overNightWorkingCalendarView);
                                            break;
                                        }
                                    }
                                }
                            }
                            #endregion

                            if (overNightWorkingCalendarView == null || //没有隔夜班
                                (firstWorkingCalendarView != null && overNightWorkingCalendarView.DateTo < firstWorkingCalendarView.DateFrom))
                            {
                                WorkingCalendarView insertedWorkingCalendarView = new WorkingCalendarView();
                                insertedWorkingCalendarView.DayOfWeek = cycleDateTime.DayOfWeek;
                                insertedWorkingCalendarView.Date = cycleDateTime;
                                insertedWorkingCalendarView.DateFrom = overNightWorkingCalendarView != null ? overNightWorkingCalendarView.DateTo : dateFrom;
                                insertedWorkingCalendarView.DateTo = firstWorkingCalendarView != null ? firstWorkingCalendarView.DateFrom : cycleDateTime.AddDays(1);
                                insertedWorkingCalendarView.Type = com.Sconit.CodeMaster.WorkingCalendarType.Rest;

                                workingCalendarViewList.Add(insertedWorkingCalendarView);
                            }

                            firstWorkingCalendarView = workingCalendarViewList.OrderBy(c => c.DateFrom).Take(1).Single();
                            lastWorkingCalendarView = workingCalendarViewList.OrderByDescending(c => c.DateTo).Take(1).Single();
                        }
                        #endregion
                    }
                    else
                    {
                        WorkingCalendarView insertedWorkingCalendarView = new WorkingCalendarView();
                        insertedWorkingCalendarView.DayOfWeek = cycleDateTime.DayOfWeek;
                        insertedWorkingCalendarView.Date = cycleDateTime;
                        insertedWorkingCalendarView.DateFrom = cycleDateTime;
                        insertedWorkingCalendarView.DateTo = cycleDateTime.AddDays(1);
                        insertedWorkingCalendarView.Type = com.Sconit.CodeMaster.WorkingCalendarType.Rest;

                        workingCalendarViewList.Add(insertedWorkingCalendarView);
                    }
                }
                #endregion


                #region 新 循环特殊日期覆盖工作日历视图
                if (specialTimeList != null && specialTimeList.Count > 0)
                {
                    var specialWorkingCalendarViewList = new List<WorkingCalendarView>();
                    foreach (var workingCalendarView in workingCalendarViewList)
                    {
                        var specialTimes = specialTimeList.Where(
                            p => (p.StartTime >= workingCalendarView.DateFrom && p.EndTime < workingCalendarView.DateTo) ||
                                (p.EndTime > workingCalendarView.DateFrom && p.EndTime <= workingCalendarView.DateTo) ||
                                (p.StartTime <= workingCalendarView.DateFrom && p.EndTime >= workingCalendarView.DateTo)
                            );
                        for (int i = 0; i < specialTimes.Count(); i++)
                        {
                            var newWorkingCalendarView = new WorkingCalendarView();
                            newWorkingCalendarView.ShiftCode = workingCalendarView.ShiftCode;
                            newWorkingCalendarView.ShiftName = workingCalendarView.ShiftName;
                            newWorkingCalendarView.DayOfWeek = workingCalendarView.DayOfWeek;
                            newWorkingCalendarView.Date = workingCalendarView.Date;
                            newWorkingCalendarView.DateFrom = workingCalendarView.DateFrom;
                            newWorkingCalendarView.DateTo = workingCalendarView.DateTo;

                            newWorkingCalendarView.Type = specialTimes.ElementAt(i).Type;

                            if (specialTimes.ElementAt(i).StartTime > workingCalendarView.DateFrom
                                && specialTimes.ElementAt(i).StartTime <= workingCalendarView.DateTo)
                            {
                                //填充第一个
                                if (i == 0)
                                {
                                    var newFirstWorkingCalendarView = new WorkingCalendarView();
                                    newFirstWorkingCalendarView.DayOfWeek = workingCalendarView.DayOfWeek;
                                    newFirstWorkingCalendarView.Date = workingCalendarView.Date;
                                    newFirstWorkingCalendarView.Type = workingCalendarView.Type;
                                    newFirstWorkingCalendarView.DateFrom = workingCalendarView.DateFrom;
                                    newFirstWorkingCalendarView.DateTo = specialTimes.ElementAt(i).StartTime;
                                    newFirstWorkingCalendarView.ShiftCode = workingCalendarView.ShiftCode;
                                    newFirstWorkingCalendarView.ShiftName = workingCalendarView.ShiftName;
                                    specialWorkingCalendarViewList.Add(newFirstWorkingCalendarView);
                                }
                                newWorkingCalendarView.DateFrom = specialTimes.ElementAt(i).StartTime;
                            }

                            if (specialTimes.ElementAt(i).EndTime < newWorkingCalendarView.DateTo
                                && specialTimes.ElementAt(i).EndTime >= newWorkingCalendarView.DateFrom)
                            {
                                //填充最后一个
                                if (i == specialTimes.Count() - 1)
                                {
                                    var newLastWorkingCalendarView = new WorkingCalendarView();
                                    //newfirstWorkingCalendarView.ShiftCode =;
                                    //newfirstWorkingCalendarView.ShiftName =;
                                    newLastWorkingCalendarView.DayOfWeek = workingCalendarView.DayOfWeek;
                                    newLastWorkingCalendarView.Date = workingCalendarView.Date;
                                    newLastWorkingCalendarView.Type = workingCalendarView.Type;
                                    newLastWorkingCalendarView.DateFrom = specialTimes.ElementAt(i).EndTime;
                                    newLastWorkingCalendarView.DateTo = workingCalendarView.DateTo;
                                    newLastWorkingCalendarView.ShiftCode = workingCalendarView.ShiftCode;
                                    newLastWorkingCalendarView.ShiftName = workingCalendarView.ShiftName;
                                    specialWorkingCalendarViewList.Add(newLastWorkingCalendarView);

                                }
                                newWorkingCalendarView.DateTo = specialTimes.ElementAt(i).EndTime;
                            }
                            //填充中间的不连续的
                            if (i < specialTimes.Count() - 1)
                            {
                                if (specialTimes.ElementAt(i).EndTime != specialTimes.ElementAt(i + 1).StartTime)
                                {
                                    var newMidWorkingCalendarView = new WorkingCalendarView();
                                    //newfirstWorkingCalendarView.ShiftCode =;
                                    //newfirstWorkingCalendarView.ShiftName =;
                                    newMidWorkingCalendarView.DayOfWeek = workingCalendarView.DayOfWeek;
                                    newMidWorkingCalendarView.Date = workingCalendarView.Date;
                                    newMidWorkingCalendarView.Type = workingCalendarView.Type;
                                    newMidWorkingCalendarView.DateFrom = specialTimes.ElementAt(i).EndTime;
                                    newMidWorkingCalendarView.DateTo = specialTimes.ElementAt(i + 1).StartTime;
                                    newMidWorkingCalendarView.ShiftCode = workingCalendarView.ShiftCode;
                                    newMidWorkingCalendarView.ShiftName = workingCalendarView.ShiftName;
                                    specialWorkingCalendarViewList.Add(newMidWorkingCalendarView);
                                }
                            }

                            if (i > 0)
                            {
                                specialWorkingCalendarViewList.Add(newWorkingCalendarView);
                            }
                            else
                            {
                                workingCalendarView.DateFrom = newWorkingCalendarView.DateFrom;
                                workingCalendarView.DateTo = newWorkingCalendarView.DateTo;
                                workingCalendarView.Type = newWorkingCalendarView.Type;
                                workingCalendarView.ShiftCode = newWorkingCalendarView.ShiftCode;
                                workingCalendarView.ShiftName = newWorkingCalendarView.ShiftName;
                            }
                        }
                    }
                    workingCalendarViewList.AddRange(specialWorkingCalendarViewList);
                }
                #endregion

                var mergedWorkingCalendarViewList = new List<WorkingCalendarView>();
                var orderByWorkingCalendarViewList = workingCalendarViewList.OrderBy(p => p.DateFrom).ThenBy(p => p.DateTo);
                for (int i = 0; i < orderByWorkingCalendarViewList.Count(); i++)
                {
                    if (((i < orderByWorkingCalendarViewList.Count() - 1 &&
                        orderByWorkingCalendarViewList.ElementAt(i).Type == orderByWorkingCalendarViewList.ElementAt(i + 1).Type &&
                        orderByWorkingCalendarViewList.ElementAt(i).Date == orderByWorkingCalendarViewList.ElementAt(i + 1).Date)) &&
                        (orderByWorkingCalendarViewList.ElementAt(i).Type == WorkingCalendarType.Rest ||
                        (orderByWorkingCalendarViewList.ElementAt(i).Type == WorkingCalendarType.Work &&
                         orderByWorkingCalendarViewList.ElementAt(i).ShiftCode == orderByWorkingCalendarViewList.ElementAt(i + 1).ShiftCode))
                        )
                    {
                        orderByWorkingCalendarViewList.ElementAt(i + 1).DateFrom = orderByWorkingCalendarViewList.ElementAt(i).DateFrom;
                    }
                    else
                    {
                        if (orderByWorkingCalendarViewList.ElementAt(i).Type == WorkingCalendarType.Rest)
                        {
                            orderByWorkingCalendarViewList.ElementAt(i).ShiftCode = null;
                            orderByWorkingCalendarViewList.ElementAt(i).ShiftName = null;
                        }
                        mergedWorkingCalendarViewList.Add(orderByWorkingCalendarViewList.ElementAt(i));
                    }
                }
                return mergedWorkingCalendarViewList;
            }
            else
            {
                return new List<WorkingCalendarView>();
            }
        }

        public IList<SpecialTime> GetSpecialTime(IList<SpecialTime> specialTimeList, IList<SpecialTime> regionSpecialTimeList, IList<SpecialTime> flowSpecialTimeList)
        {
            #region 获取特殊时间

            #region 合并Region和全局的特殊时间
            if (regionSpecialTimeList != null && regionSpecialTimeList.Count > 0)
            {
                specialTimeList = MergeSpecialTime(specialTimeList, regionSpecialTimeList);
            }
            #endregion

            #region 合并Flow和全局的特殊时间
            if (flowSpecialTimeList != null && flowSpecialTimeList.Count > 0)
            {
                specialTimeList = MergeSpecialTime(specialTimeList, flowSpecialTimeList);
            }
            #endregion

            #endregion
            return specialTimeList;
        }

        public IList<SpecialTime> GetSpecialTime(string regionCode, string flowCode, DateTime dateFrom, DateTime dateTo)
        {
            #region 获取特殊时间
            #region 获取全局的特殊时间
            IList<SpecialTime> specialTimeList = this.queryMgr.FindAll<SpecialTime>(selectSpecialTime, new object[] { dateTo.AddDays(1), dateFrom });
            #endregion

            #region 合并Region和全局的特殊时间
            if (!string.IsNullOrEmpty(regionCode))
            {
                IList<SpecialTime> regionSpecialTimeList = this.queryMgr.FindAll<SpecialTime>(selectSpecialTimeByRegion, new object[] { regionCode, dateTo.AddDays(1), dateFrom });

                specialTimeList = MergeSpecialTime(specialTimeList, regionSpecialTimeList);
            }
            #endregion

            #region 合并Flow和全局的特殊时间
            if (!string.IsNullOrEmpty(regionCode) && !string.IsNullOrEmpty(flowCode))
            {
                IList<SpecialTime> flowSpecialTimeList = this.queryMgr.FindAll<SpecialTime>(selectSpecialTimeByFlow, new object[] { flowCode, dateTo.AddDays(1), dateFrom });

                specialTimeList = MergeSpecialTime(specialTimeList, flowSpecialTimeList);
            }
            #endregion

            #endregion
            foreach (var specialTime in specialTimeList)
            {
                specialTime.StartTime = specialTime.StartTime <= dateFrom ? dateFrom : specialTime.StartTime;
                specialTime.EndTime = specialTime.EndTime >= dateTo ? dateTo : specialTime.EndTime;
            }
            return specialTimeList;
        }

        public WorkingCalendarType GetWorkingCalendarType(string region, string flowCode, DateTime dateTime)
        {
            IList<WorkingCalendarView> workingCalendarViewList = GetWorkingCalendarViewList(region, flowCode, dateTime.AddSeconds(-1), dateTime.AddSeconds(1));
            WorkingCalendarView workingCalendarView = workingCalendarViewList.Where(c => c.DateFrom <= dateTime && c.DateTo > dateTime).SingleOrDefault();

            if (workingCalendarView != null)
            {
                return workingCalendarView.Type;
            }

            return WorkingCalendarType.Rest;
        }

        #region 根据工作日历获取窗口时间
        public DateTime GetWindowTimeAtWorkingDate(DateTime baseDate, Double intervel, CodeMaster.TimeUnit intervelTimeUnit, string partyCode, string flowCode, IList<WorkingCalendarView> workingCalendarViewList)
        {
            if (intervel == 0)
            {
                return baseDate;
            }

            #region 先不考虑工作日历获取目标日期
            DateTime targetDateTime = baseDate;
            switch (intervelTimeUnit)
            {
                case com.Sconit.CodeMaster.TimeUnit.Day:
                    targetDateTime = baseDate.Add(TimeSpan.FromDays(intervel));
                    break;
                case com.Sconit.CodeMaster.TimeUnit.Hour:
                    targetDateTime = baseDate.Add(TimeSpan.FromHours(intervel));
                    break;
                case com.Sconit.CodeMaster.TimeUnit.Minute:
                    targetDateTime = baseDate.Add(TimeSpan.FromMinutes(intervel));
                    break;
                case com.Sconit.CodeMaster.TimeUnit.Second:
                    targetDateTime = baseDate.Add(TimeSpan.FromSeconds(intervel));
                    break;
            };
            #endregion

            #region 考虑工作日历重新在获取目标日期
            //DateTime dateTimeNow = DateTime.Now;
            //IList<WorkingCalendarView> workingCalendarViewList = this.GetWorkingCalendarView(partyCode, dateTimeNow, dateTimeNow.Add(TimeSpan.FromDays(7)));
            return NestGetWindowTimeAtWorkingDate(baseDate, targetDateTime, partyCode, flowCode, workingCalendarViewList);
            #endregion
        }

        private DateTime NestGetWindowTimeAtWorkingDate(DateTime baseDate, DateTime targetDateTime, string partyCode, string flowCode, IList<WorkingCalendarView> workingCalendarViewList)
        {
            DateTime nextBaseDate = targetDateTime;

            //1. 查看目标日期落在工作日历哪里，如果是在工作期间内要在加上从基准日期至工作日期中的休息时间。
            //   如果中间没有休息时间，就得到目标日期。如果加上休息时间之后落在休息时间重复2，如果落在工作时间重复1
            //2. 如果落在休息日期中，则把目标日期改为之后离休息日期最近的工作日期，在加上从基准日期至工作日期中的休息时间。
            WorkingCalendarView workingCalendarView = workingCalendarViewList.Where(c => c.DateFrom < targetDateTime && c.DateTo >= targetDateTime).SingleOrDefault();

            if (workingCalendarView == null)
            {
                //如果没有找到工作日历，说明取的工作日历范围太小，需要重新加载工作日历
                workingCalendarViewList = ReloadWorkingCalendar(baseDate, targetDateTime, partyCode, flowCode);

                //使用原参数重新计算
                return NestGetWindowTimeAtWorkingDate(baseDate, targetDateTime, partyCode, flowCode, workingCalendarViewList);
            }
            else
            {
                if (workingCalendarView.Type == CodeMaster.WorkingCalendarType.Work)
                {
                    //累加休息日期
                    return AccumulateWindowTimeRestTime(baseDate, nextBaseDate, targetDateTime, workingCalendarViewList, partyCode, flowCode);
                }
                else
                {
                    //查找离休息日期最新的工作日期
                    WorkingCalendarView nextWorkworkingCalendarView = workingCalendarViewList.Where(c => c.Type == CodeMaster.WorkingCalendarType.Work
                        //要用大于等于，考虑到下一个日期正好是工作日期
                        && c.DateFrom >= workingCalendarView.DateTo).OrderBy(c => c.DateFrom).Take(1).SingleOrDefault();

                    if (nextWorkworkingCalendarView == null)
                    {
                        //如果没有找到工作日历，说明取的工作日历范围太小，需要重新加载工作日历
                        WorkingCalendarView lastWorkingCalendarView = workingCalendarViewList.OrderByDescending(c => c.DateTo).Take(1).Single();

                        workingCalendarViewList = ReloadWorkingCalendar(baseDate, lastWorkingCalendarView.DateTo, partyCode, flowCode);

                        //使用原参数重新计算
                        return NestGetWindowTimeAtWorkingDate(baseDate, targetDateTime, partyCode, flowCode, workingCalendarViewList);
                    }
                    else
                    {
                        targetDateTime = nextWorkworkingCalendarView.DateFrom;

                        //累加休息日期
                        return AccumulateWindowTimeRestTime(baseDate, nextBaseDate, targetDateTime, workingCalendarViewList, partyCode, flowCode);
                    }
                }
            }
        }

        private DateTime AccumulateWindowTimeRestTime(DateTime baseDateTime, DateTime orgTargetDateTime, DateTime targetDateTime, IList<WorkingCalendarView> workingCalendarViewList, string partyCode, string flowCode)
        {
            //查找休息日期
            //1. 结束日期大于基准日期
            //2. 结束日期小于等于目标日期
            IList<WorkingCalendarView> restWorkingCalendarViewList = workingCalendarViewList.Where(
                c => c.DateTo > baseDateTime && c.Type == CodeMaster.WorkingCalendarType.Rest && c.DateFrom < orgTargetDateTime
                ).ToList();

            DateTime nextBaseDateTime = targetDateTime;
            if (restWorkingCalendarViewList != null && restWorkingCalendarViewList.Count > 0)
            {
                foreach (WorkingCalendarView restWorkingCalendarView in restWorkingCalendarViewList)
                {
                    if (restWorkingCalendarView.DateFrom < baseDateTime)
                    {
                        if (restWorkingCalendarView.DateTo > orgTargetDateTime)
                        {
                            //基准时间和原目标时间落在休息日期区间之内，只增加原目标时间和基准时间的休息时间间隔
                            targetDateTime = targetDateTime.Add(orgTargetDateTime.Subtract(baseDateTime));
                        }
                        else
                        {
                            //基准时间大于休息日期的开始时间，原目标日期大于休息日期的结束时间，增加基准时间和休息日期结束时间的时间间隔
                            targetDateTime = targetDateTime.Add(restWorkingCalendarView.DateTo.Subtract(baseDateTime));
                        }
                    }
                    else
                    {
                        if (restWorkingCalendarView.DateTo > orgTargetDateTime)
                        {
                            //基准时间小于休息日期的开始时间，原目标日期小于休息日期的结束时间，增加休息日期开始时间和原目标日期的时间间隔
                            targetDateTime = targetDateTime.Add(orgTargetDateTime.Subtract(restWorkingCalendarView.DateFrom));
                        }
                        else
                        {
                            //基准时间大于休息日期的开始时间，原目标日期小于休息日期的结束时间，增加休息日期开始时间和结束时间的时间间隔
                            targetDateTime = targetDateTime.Add(restWorkingCalendarView.DateTo.Subtract(restWorkingCalendarView.DateFrom));
                        }
                    }
                }

                //用原目标日期作为基准日期重新迭代计算
                return NestGetWindowTimeAtWorkingDate(nextBaseDateTime, targetDateTime, partyCode, flowCode, workingCalendarViewList);
            }
            else
            {
                //目标日期和基准日期没有休息日期间隔，得到最终计算结果
                return targetDateTime;
            }
        }

        private IList<WorkingCalendarView> ReloadWorkingCalendar(DateTime baseDate, DateTime dateTo, string partyCode, string flowCode)
        {
            //重新加载工作日历
            if (baseDate < dateTo)
            {
                DateTime endDateTime = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day).AddDays(7); //向后加载一周的工作日历
                return this.GetWorkingCalendarViewList(partyCode, flowCode, baseDate, endDateTime);
            }
            else
            {
                DateTime startDateTime = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day).AddDays(-7); //向前加载一周的工作日历
                return this.GetWorkingCalendarViewList(partyCode, flowCode, startDateTime, baseDate);
            }
        }
        #endregion

        #region 根据工作日历获取开始时间
        /// <summary>
        /// 生产排程,往后推算
        /// </summary>
        public DateTime GetStartTimeAtWorkingDate(DateTime baseDate, string partyCode, string flowCode, IList<WorkingCalendarView> workingCalendarViewList)
        {
            #region 考虑工作日历重新在获取目标日期
            return NestGetStartTimeAtWorkingDate(baseDate, partyCode, flowCode, workingCalendarViewList);
            #endregion
        }

        private DateTime NestGetStartTimeAtWorkingDate(DateTime baseDate, string partyCode, string flowCode, IList<WorkingCalendarView> workingCalendarViewList)
        {
            WorkingCalendarView nextWorkworkingCalendarView = workingCalendarViewList.Where(c => c.Type == CodeMaster.WorkingCalendarType.Work
                    && (c.DateFrom <= baseDate && c.DateTo > baseDate || c.DateFrom >= baseDate))
                    .OrderBy(c => c.DateFrom).FirstOrDefault();
            if (nextWorkworkingCalendarView != null)
            {
                return nextWorkworkingCalendarView.DateFrom;
            }
            else
            {
                DateTime DateTo = workingCalendarViewList.Max(p => p.DateTo);
                workingCalendarViewList = ReloadWorkingCalendar(baseDate, DateTo, partyCode, flowCode);
                return NestGetStartTimeAtWorkingDate(baseDate, partyCode, flowCode, workingCalendarViewList);
            }
        }

        /// <summary>
        /// 物流,往前推算
        /// </summary>
        public DateTime GetStartTimeAtWorkingDate(DateTime baseDate, Double intervel, CodeMaster.TimeUnit intervelTimeUnit, string partyCode, string flowCode, IList<WorkingCalendarView> workingCalendarViewList)
        {
            #region 先不考虑工作日历获取目标日期
            DateTime targetDateTime = baseDate;
            switch (intervelTimeUnit)
            {
                case com.Sconit.CodeMaster.TimeUnit.Day:
                    targetDateTime = baseDate.Add(TimeSpan.FromDays(-intervel));
                    break;
                case com.Sconit.CodeMaster.TimeUnit.Hour:
                    targetDateTime = baseDate.Add(TimeSpan.FromHours(-intervel));
                    break;
                case com.Sconit.CodeMaster.TimeUnit.Minute:
                    targetDateTime = baseDate.Add(TimeSpan.FromMinutes(-intervel));
                    break;
                case com.Sconit.CodeMaster.TimeUnit.Second:
                    targetDateTime = baseDate.Add(TimeSpan.FromSeconds(-intervel));
                    break;
            };
            #endregion

            #region 考虑工作日历重新在获取目标日期
            return NestGetStartTimeAtWorkingDate(baseDate, targetDateTime, partyCode, flowCode, workingCalendarViewList);
            #endregion
        }

        private DateTime NestGetStartTimeAtWorkingDate(DateTime baseDate, DateTime targetDateTime, string partyCode, string flowCode, IList<WorkingCalendarView> workingCalendarViewList)
        {
            DateTime nextBaseDate = targetDateTime;

            //1. 查看目标日期落在工作日历哪里，如果是在工作期间内要在加上从基准日期至工作日期中的休息时间。
            //   如果中间没有休息时间，就得到目标日期。如果加上休息时间之后落在休息时间重复2，如果落在工作时间重复1
            //2. 如果落在休息日期中，则把目标日期改为之后离休息日期最近的工作日期，在加上从基准日期至工作日期中的休息时间。
            WorkingCalendarView workingCalendarView = workingCalendarViewList.Where(c => c.DateFrom < targetDateTime && c.DateTo >= targetDateTime).SingleOrDefault();

            if (workingCalendarView == null)
            {
                //如果没有找到工作日历，说明取的工作日历范围太小，需要重新加载工作日历
                workingCalendarViewList = ReloadWorkingCalendar(baseDate, targetDateTime, partyCode, flowCode);

                //使用原参数重新计算
                return NestGetStartTimeAtWorkingDate(baseDate, targetDateTime, partyCode, flowCode, workingCalendarViewList);
            }
            else
            {
                if (workingCalendarView.Type == CodeMaster.WorkingCalendarType.Work)
                {
                    //累加休息日期
                    return AccumulateStartTimeRestTime(baseDate, nextBaseDate, targetDateTime, workingCalendarViewList, partyCode, flowCode);
                }
                else
                {
                    //查找离休息日期最新的工作日期
                    WorkingCalendarView previousWorkworkingCalendarView = workingCalendarViewList.Where(c => c.Type == CodeMaster.WorkingCalendarType.Work
                        //要用小于等于，考虑到上一个日期正好是工作日期
                        && c.DateTo <= workingCalendarView.DateFrom).OrderBy(c => c.DateTo).LastOrDefault();

                    if (previousWorkworkingCalendarView == null)
                    {
                        //如果没有找到工作日历，说明取的工作日历范围太小，需要重新加载工作日历
                        WorkingCalendarView lastWorkingCalendarView = workingCalendarViewList.OrderBy(c => c.DateFrom).First();
                        workingCalendarViewList = ReloadWorkingCalendar(baseDate, lastWorkingCalendarView.DateFrom, partyCode, flowCode);
                        //使用原参数重新计算
                        return NestGetStartTimeAtWorkingDate(baseDate, targetDateTime, partyCode, flowCode, workingCalendarViewList);
                    }
                    else
                    {
                        targetDateTime = previousWorkworkingCalendarView.DateTo;

                        //累加休息日期
                        return AccumulateStartTimeRestTime(baseDate, nextBaseDate, targetDateTime, workingCalendarViewList, partyCode, flowCode);
                    }
                }
            }
        }

        private DateTime AccumulateStartTimeRestTime(DateTime baseDateTime, DateTime orgTargetDateTime, DateTime targetDateTime, IList<WorkingCalendarView> workingCalendarViewList, string partyCode, string flowCode)
        {
            //查找休息日期
            //1. 结束日期大于基准日期
            //2. 结束日期小于等于目标日期
            IList<WorkingCalendarView> restWorkingCalendarViewList = workingCalendarViewList.Where(
                c => c.DateTo > orgTargetDateTime && c.Type == CodeMaster.WorkingCalendarType.Rest && c.DateFrom < baseDateTime
                ).ToList();

            DateTime previousBaseDateTime = targetDateTime;
            if (restWorkingCalendarViewList != null && restWorkingCalendarViewList.Count > 0)
            {
                foreach (WorkingCalendarView restWorkingCalendarView in restWorkingCalendarViewList)
                {
                    if (restWorkingCalendarView.DateFrom < orgTargetDateTime)
                    {
                        if (restWorkingCalendarView.DateTo > baseDateTime)
                        {
                            //基准时间和原目标时间落在休息日期区间之内，只增加原目标时间和基准时间的休息时间间隔
                            targetDateTime = targetDateTime.Subtract(baseDateTime.Subtract(orgTargetDateTime));
                        }
                        else
                        {
                            //基准时间大于休息日期的开始时间，原目标日期大于休息日期的结束时间，增加基准时间和休息日期结束时间的时间间隔
                            targetDateTime = targetDateTime.Subtract(restWorkingCalendarView.DateTo.Subtract(orgTargetDateTime));
                        }
                    }
                    else
                    {
                        if (restWorkingCalendarView.DateTo > baseDateTime)
                        {
                            //基准时间小于休息日期的开始时间，原目标日期小于休息日期的结束时间，增加休息日期开始时间和原目标日期的时间间隔
                            targetDateTime = targetDateTime.Subtract(baseDateTime.Subtract(restWorkingCalendarView.DateFrom));
                        }
                        else
                        {
                            //基准时间大于休息日期的开始时间，原目标日期小于休息日期的结束时间，增加休息日期开始时间和结束时间的时间间隔
                            targetDateTime = targetDateTime.Subtract(restWorkingCalendarView.DateTo.Subtract(restWorkingCalendarView.DateFrom));
                        }
                    }
                }

                //用原目标日期作为基准日期重新迭代计算
                return NestGetWindowTimeAtWorkingDate(previousBaseDateTime, targetDateTime, partyCode, flowCode, workingCalendarViewList);
            }
            else
            {
                //目标日期和基准日期没有休息日期间隔，得到最终计算结果
                return targetDateTime;
            }
        }

        #endregion

        private IList<SpecialTime> MergeSpecialTime(IList<SpecialTime> specialTimeList, IList<SpecialTime> subSpecialTimeList)
        {
            IList<SpecialTime> newSpecialTimeList = new List<SpecialTime>();
            //时间分割
            var dateTimeDic = new Dictionary<DateTime, int>();
            foreach (var subSpecialTime in subSpecialTimeList)
            {
                if (!dateTimeDic.ContainsKey(subSpecialTime.StartTime))
                {
                    dateTimeDic.Add(subSpecialTime.StartTime, 1);
                }
                if (!dateTimeDic.ContainsKey(subSpecialTime.EndTime))
                {
                    dateTimeDic.Add(subSpecialTime.EndTime, 2);
                }
            }
            foreach (var specialTime in specialTimeList)
            {
                if (!dateTimeDic.ContainsKey(specialTime.StartTime))
                {
                    dateTimeDic.Add(specialTime.StartTime, 3);
                }
                if (!dateTimeDic.ContainsKey(specialTime.EndTime))
                {
                    dateTimeDic.Add(specialTime.EndTime, 4);
                }
            }
            var dateTimeDicOrderByKey = dateTimeDic.OrderBy(p => p.Key);
            //分段填充
            for (int i = 0; i < dateTimeDic.Count - 1; i++)
            {
                var startTimeDic = dateTimeDicOrderByKey.ElementAt(i);
                var endTimeDic = dateTimeDicOrderByKey.ElementAt(i + 1);
                SpecialTime newSpecialTime = new SpecialTime();
                //是subSpecialTimeList的StartTime
                if (startTimeDic.Value == 1)
                {
                    newSpecialTime = subSpecialTimeList.First(p => p.StartTime == startTimeDic.Key);
                }
                //是subSpecialTimeList的EndTime
                else if (startTimeDic.Value == 2)
                {
                    var specialTime = subSpecialTimeList.FirstOrDefault(p => p.StartTime == startTimeDic.Key);
                    if (specialTime == null)
                    {
                        specialTime = specialTimeList.FirstOrDefault(p => p.StartTime <= startTimeDic.Key);
                        if (specialTime != null)
                        {
                            newSpecialTime.StartTime = startTimeDic.Key;
                            newSpecialTime.EndTime = endTimeDic.Key;
                            newSpecialTime.Region = specialTime.Region;
                            newSpecialTime.Type = specialTime.Type;
                            newSpecialTime.Description = specialTime.Description;
                            newSpecialTime.Remarks = specialTime.Remarks;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        newSpecialTime = specialTime;
                    }
                }
                //是specialTimeList的StartTime
                //是specialTimeList的EndTime
                else if (startTimeDic.Value == 3 || startTimeDic.Value == 4)
                {
                    var subSpecialTime = subSpecialTimeList.FirstOrDefault(p => p.StartTime <= startTimeDic.Key && p.EndTime >= endTimeDic.Key);
                    if (subSpecialTime != null)
                    {
                        continue;
                        //newSpecialTime.StartTime = startTimeDic.Key;
                        //newSpecialTime.EndTime = endTimeDic.Key;
                        //newSpecialTime.Description = subSpecialTime.Region;
                        //newSpecialTime.Type = subSpecialTime.Type;
                        //newSpecialTime.Description = subSpecialTime.Description;
                        //newSpecialTime.Remarks = subSpecialTime.Remarks;
                    }
                    else
                    {
                        var specialTime = specialTimeList.FirstOrDefault(p => p.StartTime <= startTimeDic.Key && p.EndTime >= endTimeDic.Key);

                        if (specialTime != null)
                        {
                            newSpecialTime.StartTime = startTimeDic.Key;
                            newSpecialTime.EndTime = endTimeDic.Key;
                            newSpecialTime.Description = specialTime.Region;
                            newSpecialTime.Type = specialTime.Type;
                            newSpecialTime.Description = specialTime.Description;
                            newSpecialTime.Remarks = specialTime.Remarks;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                if (newSpecialTimeList.Count > 0 && newSpecialTimeList.Last().Type == newSpecialTime.Type)
                {
                    newSpecialTimeList.Last().EndTime = endTimeDic.Key;
                }
                else
                {
                    newSpecialTimeList.Add(newSpecialTime);
                }
            }

            return newSpecialTimeList;
        }

        private IList<WorkingCalendarView> GetWorkingCalendarViewList(IList<WorkingCalendar> workingCalendarList,
            IList<WorkingShift> workingShiftList, IList<ShiftDetail> shiftDetailList, WorkingCalendarView workingCalendarView)
        {
            var workingCalendarViewList = new List<WorkingCalendarView>();

            #region 获取当前是星期几
            WorkingCalendar workingCalendar = (from w in workingCalendarList
                                               where w.DayOfWeek == workingCalendarView.Date.DayOfWeek
                                               select w).SingleOrDefault();
            #endregion

            if (workingCalendar != null)
            {
                #region 获取班次明细
                IList<WorkingShift> currentWorkingShiftList = (from w in workingShiftList
                                                               where w.WorkingCalendar == workingCalendar.Id
                                                               select w).ToList();

                IList<ShiftDetail> currentShiftDetailList = (from w in currentWorkingShiftList
                                                             join s in shiftDetailList on w.Shift.Code equals s.Shift
                                                             where (!s.StartDate.HasValue || s.StartDate <= workingCalendarView.Date)
                                                                    && (!s.EndDate.HasValue || s.EndDate >= workingCalendarView.Date)
                                                             select s).OrderBy(s => int.Parse(s.ShiftTime.Split(ShiftDetail.ShiftTimeSplitSymbol)[0].Split(':')[0])).ToList();
                #endregion

                if (currentShiftDetailList.Count > 0)
                {
                    #region 处理工作日,循环班次明细
                    foreach (ShiftDetail currentShiftDetail in currentShiftDetailList)
                    {
                        WorkingCalendarView currentWorkingCalendarView = new WorkingCalendarView();
                        currentWorkingCalendarView.ShiftCode = currentShiftDetail.Shift;
                        currentWorkingCalendarView.ShiftName = (from w in currentWorkingShiftList where w.Shift.Code == currentShiftDetail.Shift select w.Shift.Name).Single();
                        currentWorkingCalendarView.DayOfWeek = workingCalendarView.Date.DayOfWeek;
                        currentWorkingCalendarView.Date = workingCalendarView.Date;
                        string[] splitedShiftTime = currentShiftDetail.ShiftTime.Split(ShiftDetail.ShiftTimeSplitSymbol);
                        currentWorkingCalendarView.DateFrom = Convert.ToDateTime(workingCalendarView.Date.ToString("yyyy-MM-dd") + " " + splitedShiftTime[0]);
                        currentWorkingCalendarView.DateTo = Convert.ToDateTime(workingCalendarView.Date.ToString("yyyy-MM-dd") + " " + splitedShiftTime[1]);
                        if (DateTime.Compare(currentWorkingCalendarView.DateFrom, currentWorkingCalendarView.DateTo) >= 0)
                        {
                            //如果开始日期大于等于结束日期，结束日期+1
                            currentWorkingCalendarView.DateTo = currentWorkingCalendarView.DateTo.AddDays(1);
                        }
                        currentWorkingCalendarView.Type = com.Sconit.CodeMaster.WorkingCalendarType.Work;

                        workingCalendarViewList.Add(currentWorkingCalendarView);
                    }
                    #endregion
                }

                #region 如果第一条工作日历不是从DateFrom开始，新增工作日历补齐
                if (workingCalendarViewList == null && workingCalendarViewList.Count > 0)
                {
                    var firstWorkingCalendarView = workingCalendarViewList.OrderBy(c => c.DateFrom).FirstOrDefault();
                    if (firstWorkingCalendarView == null || firstWorkingCalendarView.DateFrom > workingCalendarView.DateFrom)
                    {
                        #region 新增昨天的隔夜班次
                        WorkingCalendarView overNightWorkingCalendarView = null;
                        #region 查找昨天星期几
                        WorkingCalendar lastWorkingCalendar = (from w in workingCalendarList
                                                               where w.DayOfWeek == workingCalendarView.Date.AddDays(-1).DayOfWeek
                                                               select w).SingleOrDefault();
                        #endregion
                        if (lastWorkingCalendar != null)
                        {
                            IList<WorkingShift> lastWorkingShiftList = (from w in workingShiftList
                                                                        where w.WorkingCalendar == lastWorkingCalendar.Id
                                                                        select w).ToList();

                            IList<ShiftDetail> lastShiftDetailList = (from w in lastWorkingShiftList
                                                                      join s in shiftDetailList on w.Shift.Code equals s.Shift
                                                                      where (!s.StartDate.HasValue || s.StartDate <= workingCalendarView.Date)
                                                                             && (!s.EndDate.HasValue || s.EndDate >= workingCalendarView.Date)
                                                                      select s).OrderBy(s => s.ShiftTime).ToList();

                            if (lastShiftDetailList != null && lastShiftDetailList.Count > 0)
                            {
                                foreach (ShiftDetail lastShiftDetail in lastShiftDetailList)
                                {
                                    string[] splitedShiftTime = lastShiftDetail.ShiftTime.Split(ShiftDetail.ShiftTimeSplitSymbol);

                                    DateTime shiftDateFrom = Convert.ToDateTime(workingCalendarView.Date.ToString("yyyy-MM-dd") + " " + splitedShiftTime[0]);
                                    DateTime shiftDateTo = Convert.ToDateTime(workingCalendarView.Date.ToString("yyyy-MM-dd") + " " + splitedShiftTime[1]);
                                    if (DateTime.Compare(shiftDateFrom, shiftDateTo) >= 0  //如果开始日期大于等于结束日期，代表为隔夜的班次
                                        && shiftDateTo > workingCalendarView.DateFrom)
                                    {
                                        DateTime lastDate = workingCalendarView.Date.AddDays(-1);
                                        overNightWorkingCalendarView = new WorkingCalendarView();
                                        overNightWorkingCalendarView.ShiftCode = lastShiftDetail.Shift;
                                        overNightWorkingCalendarView.ShiftName = (from w in lastWorkingShiftList where w.Shift.Code == lastShiftDetail.Shift select w.Shift.Name).Single();
                                        overNightWorkingCalendarView.DayOfWeek = lastDate.DayOfWeek;
                                        overNightWorkingCalendarView.Date = lastDate;
                                        overNightWorkingCalendarView.DateFrom = workingCalendarView.DateFrom;
                                        overNightWorkingCalendarView.DateTo = shiftDateTo;
                                        overNightWorkingCalendarView.Type = com.Sconit.CodeMaster.WorkingCalendarType.Work;

                                        workingCalendarViewList.Add(overNightWorkingCalendarView);
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion

                        if (overNightWorkingCalendarView == null || //没有隔夜班
                            (firstWorkingCalendarView != null && overNightWorkingCalendarView.DateTo < firstWorkingCalendarView.DateFrom))
                        {
                            WorkingCalendarView insertedWorkingCalendarView = new WorkingCalendarView();
                            insertedWorkingCalendarView.DayOfWeek = workingCalendarView.Date.DayOfWeek;
                            insertedWorkingCalendarView.Date = workingCalendarView.Date;
                            insertedWorkingCalendarView.DateFrom = overNightWorkingCalendarView != null ? overNightWorkingCalendarView.DateTo : workingCalendarView.DateFrom;
                            insertedWorkingCalendarView.DateTo = firstWorkingCalendarView != null ? firstWorkingCalendarView.DateFrom : workingCalendarView.Date.AddDays(1);
                            insertedWorkingCalendarView.Type = com.Sconit.CodeMaster.WorkingCalendarType.Rest;

                            workingCalendarViewList.Add(insertedWorkingCalendarView);
                        }
                    }
                }
                #endregion
            }
            return workingCalendarViewList;
        }

        #endregion


        [Transaction(TransactionMode.Requires)]
        public void CreateShiftMasterAndShiftDetail(ShiftMaster shiftMaster, ShiftDetail shiftDetail)
        {
            this.genericMgr.Create(shiftMaster);
            this.genericMgr.Create(shiftDetail);
        }

        [Transaction(TransactionMode.Requires)]
        public void GetStartTimeAndWindowTime(string shift, DateTime planDate, out DateTime startTime, out DateTime windowTime)
        {
            startTime = planDate.Date;
            windowTime = planDate.Date.AddDays(1);
            var shiftDetails = this.genericMgr.FindAll<ShiftDetail>("from ShiftDetail where Shift = ? ", shift);
            if (shiftDetails != null && shiftDetails.Count > 0)
            {
                windowTime = planDate.Date;
                startTime = planDate.Date.AddDays(1);

                foreach (var shiftDetail in shiftDetails)
                {
                    string[] splitedShiftTime = shiftDetail.ShiftTime.Split(ShiftDetail.ShiftTimeSplitSymbol);
                    DateTime shiftDateFrom = Convert.ToDateTime(planDate.Date.ToString("yyyy-MM-dd") + " " + splitedShiftTime[0]);
                    startTime = shiftDateFrom < startTime ? shiftDateFrom : startTime;

                    DateTime shiftDateTo = Convert.ToDateTime(planDate.Date.ToString("yyyy-MM-dd") + " " + splitedShiftTime[1]);
                    shiftDateTo = shiftDateTo > shiftDateFrom ? shiftDateTo : shiftDateTo.AddDays(1);
                    windowTime = shiftDateTo > windowTime ? shiftDateTo : windowTime;
                }
            }
        }
    }

    class WorkingCalendarComparer : IEqualityComparer<WorkingCalendar>
    {

        public bool Equals(WorkingCalendar x, WorkingCalendar y)
        {
            return x.DayOfWeek == y.DayOfWeek;
        }

        public int GetHashCode(WorkingCalendar obj)
        {
            return obj.DayOfWeek.GetHashCode();
        }
    }

    class SpecialTimeComparer : IEqualityComparer<SpecialTime>
    {

        public bool Equals(SpecialTime x, SpecialTime y)
        {
            return x.StartTime == y.StartTime;
        }

        public int GetHashCode(SpecialTime obj)
        {
            return obj.StartTime.GetHashCode();
        }
    }
}
