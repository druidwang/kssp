using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MrpExScrapSearchModel : SearchModelBase
    {
        public string Flow { get; set; }
        public int? Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}