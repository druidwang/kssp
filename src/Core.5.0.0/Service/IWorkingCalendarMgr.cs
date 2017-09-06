using System;
using System.Collections.Generic;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.PRD;
using com.Sconit.CodeMaster;

namespace com.Sconit.Service
{
    public interface IWorkingCalendarMgr
    {
        void DeleteShiftMaster(string shiftMasterCode);
        void UpdateWorkingCalendar(WorkingCalendar workingCalendar, IList<string> ShiftList);
        IList<WorkingCalendarView> GetWorkingCalendarViewList(string regionCode, string flowCode,DateTime dateFrom, DateTime dateTo);
        void CreateShiftMasterAndShiftDetail(ShiftMaster shiftMaster, ShiftDetail shiftDetail);
        DateTime GetWindowTimeAtWorkingDate(DateTime baseDate, Double intervel, CodeMaster.TimeUnit intervelTimeUnit, string partyCode,string flowCode, IList<WorkingCalendarView> workingCalendarViewList);
        DateTime GetStartTimeAtWorkingDate(DateTime baseDate, Double intervel, CodeMaster.TimeUnit intervelTimeUnit, string partyCode,string flowCode, IList<WorkingCalendarView> workingCalendarViewList);
        DateTime GetStartTimeAtWorkingDate(DateTime baseDate, string partyCode, string flowCode, IList<WorkingCalendarView> workingCalendarViewList);
        WorkingCalendarType GetWorkingCalendarType(string region,string flowCode, DateTime dateTime);
        IList<SpecialTime> GetSpecialTime(string regionCode, string flowCode, DateTime dateFrom, DateTime dateTo);
        IList<SpecialTime> GetSpecialTime(IList<SpecialTime> specialTimeList, IList<SpecialTime> regionSpecialTimeList, IList<SpecialTime> flowSpecialTimeList);
        void GetStartTimeAndWindowTime(string shift, DateTime planDate, out DateTime startTime, out DateTime windowTime);
    }
}
