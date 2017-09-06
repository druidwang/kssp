using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class OrderMasterSearchModel : SearchModelBase
    {
        public string OrderNo { get; set; }
        public string Flow { get; set; }
        public Int16? Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Int16? Priority { get; set; }
        public Int16? Status { get; set; }
        public string PartyFrom { get; set; }
        public string PartyFromName { get; set; }
        public string PartyTo { get; set; }
        public string PartyToName { get; set; }
        public string Shift { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string CreateUserName { get; set; }
        public Int16? SubType { get; set; }
        public bool? IsPause { get; set; }
        public string Dock { get; set; }
        public string LocationTo { get; set; }
        public string LocationFrom { get; set; }
        public string LocationToTo { get; set; }
        public string LocationFromTo { get; set; }
        public string Item { get; set; }
        public string ReferenceOrderNo { get; set; }
        public string ExternalOrderNo { get; set; }
        public com.Sconit.CodeMaster.ScheduleType? ScheduleType { get; set; }
        public string WMSNO { get; set; }
        public string WmSSeq { get; set; }
        public Int64? Sequence { get; set; }
        public string TraceCode { get; set; }
        public string ManufactureParty { get; set; }
        public string Checker { get; set; }
        public Int16? QueryOrderType { get; set; }
        public string DateType { get; set; }
        public string SearchForceMaterialOrder { get; set; }
        public string MultiStatus { get; set; }

    }
}