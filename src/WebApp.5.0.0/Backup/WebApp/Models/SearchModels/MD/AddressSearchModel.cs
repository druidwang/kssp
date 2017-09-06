using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class AddressSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public int? Type { get; set; }
        public string AddressContent { get; set; }
        public string PostCode { get; set; }
        public string TelPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string ContactPersonName { get; set; }
        public string AddressTypeDescription { get; set; }
    }
}