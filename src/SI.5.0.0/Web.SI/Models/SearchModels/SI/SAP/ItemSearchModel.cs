using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.SI.SAP
{
    public class ItemSearchModel : SearchModelBase
    {
        //Id, Code, ReferenceCode, Description, Uom, Plant, IOStatus, InboundDate, OutboundDate
        public int Id { get; set; }

        public string Code { get; set; }

        public string ReferenceCode { get; set; }

        public string Description { get; set; }

        public string Uom { get; set; }

        public string Plant { get; set; }

        public int? IOStatus { get; set; }

        public DateTime? InboundDateStart { get; set; }

        public DateTime? OutboundDateStart { get; set; }


        public DateTime? EndOutboundDate { get; set; }
        public DateTime? EndInboundDate { get; set; }
    }
}