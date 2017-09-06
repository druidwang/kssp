using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MachineSearchModel : SearchModelBase
    {
        public string Code { get; set; }

        public string Description { get; set; }
        public string MachineType { get; set; }
        public string ShiftType { get; set; }
    }
}