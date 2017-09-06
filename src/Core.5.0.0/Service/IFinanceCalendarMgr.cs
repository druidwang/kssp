using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MD;

namespace com.Sconit.Service
{
    public interface IFinanceCalendarMgr
    {
        FinanceCalendar GetNowEffectiveFinanceCalendar();
        void CloseFinanceCalendar();
    }
}
