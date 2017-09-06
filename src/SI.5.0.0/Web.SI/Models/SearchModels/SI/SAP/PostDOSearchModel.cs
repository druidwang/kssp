using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.SI.SAP
{
    public class PostDOSearchModel : SearchModelBase
    {
        //Id, OrderNo, ReceiptNo, Result, ZTCODE, Success, LastModifyDate, Status, CreateDate, ErrorCount
        public string OrderNo { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
    }
}