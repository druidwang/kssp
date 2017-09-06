using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.WMS
{
    public class PickWindowTimeSearchModel : SearchModelBase
    {
        public string PickScheduleNo { get; set; }

        public string ShiftCode { get; set; }
    }
}