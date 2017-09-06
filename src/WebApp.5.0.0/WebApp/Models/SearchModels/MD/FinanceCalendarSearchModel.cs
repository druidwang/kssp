using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class FinanceCalendarSearchModel:SearchModelBase
    {
        public Int32? FinanceYear { get; set; }
        public Int32? FinanceMonth { get; set; }
    }
}