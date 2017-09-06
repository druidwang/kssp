using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MrpSnapMasterSearchModel : SearchModelBase
    {
        public DateTime? SnapTimeStart { get; set; }
        public DateTime? SnapTimeEnd { get; set; }
        public bool IsRelease { get; set; }
        public DateTime? SnapTime { get; set; }
        public int SnapType { get; set; }
    }
}