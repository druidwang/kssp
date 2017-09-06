using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.SI
{
    public class SAPInterfaceSearchModel : SearchModelBase
    {
        public string BatchNo { get; set; }
        public string ZMESSO { get; set; }
        public string ZMESSC { get; set; }
        public string ZMESPO { get; set; }
        public string ZMESKO { get; set; }
        public string MATNR1 { get; set; }
        public string MATNR { get; set; }
        public string MATNR_TH { get; set; }
        public string MATNR_C { get; set; }
        public string MATNR_H { get; set; }
        public string MATNR_I { get; set; }
        public string LGORT_H { get; set; }
        public string LGORT_I { get; set; }
        public string LGORT { get; set; }
        public string MTSNR { get; set; }
        public string LFSNR { get; set; }
        public string LIFNR { get; set; }
        public string UMLGO { get; set; }
        public string MATERIAL { get; set; }
        public string PARTNNUMB { get; set; }
        public string ZComnum { get; set; }
        public string BWART_H { get; set; }
        public string BWART_C { get; set; }
        public string BWART_I { get; set; }
        public string BWARTWA { get; set; }
        public string BWART { get; set; }
        public string Item { get; set; }
        
        public string Interface { get; set; }
        public string SysCode { get; set; }
        public string Status { get; set; }
        public DateTime? TransStartDate { get; set; }
        public DateTime? TransEndDate { get; set; }
        public string TimeType { get; set; }
        public string MultiInterfaces { get; set; }
        public string ItemCode { get; set; }
        public string BomCode { get; set; }
        public string SupplierCode { get; set; }
        public string Plan { get; set; }
        public string EKORG { get; set; }
        public string BUKRS { get; set; }
        public string CustomerCode { get; set; }
        public string successMessage { get; set; }
        public string errorMessage { get; set; }
    }
}