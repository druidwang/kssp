using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ISS
{
    public class SMSRecSearchModel : SearchModelBase
    {
        public string Issue { get; set; }
        public string Content { get; set; }
        public string MsgID { get; set; }
        public string SrcID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}