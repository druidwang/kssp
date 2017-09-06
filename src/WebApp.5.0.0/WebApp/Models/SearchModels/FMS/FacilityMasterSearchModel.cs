using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.FMS
{
    public class FacilityMasterSearchModel : SearchModelBase
    {
        public string FCID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string AssetNo { get; set; }
        public string ChargePerson { get; set; }
        public string ChargeSite { get; set; }
        public string ChargeOrganization { get; set; }
        public string RefenceCode { get; set; }
        public string MaintainGroup { get; set; }
        public string OwnerDescription { get; set; }
    }
}