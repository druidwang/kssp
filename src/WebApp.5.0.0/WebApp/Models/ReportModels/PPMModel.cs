using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace com.Sconit.Web.Models.ReportModels
{
    public class ReceiveReturnModel
    {
        [Display(Name = "PPM_Supplier", ResourceType = typeof(Resources.RPT.PPM))]
        public string Supplier { get; set; }
        [Display(Name = "PPM_SupplierName", ResourceType = typeof(Resources.RPT.PPM))]
        public string SupplierName { get; set; }
        [Display(Name = "PPM_Item", ResourceType = typeof(Resources.RPT.PPM))]
        public string Item { get; set; }
        [Display(Name = "PPM_ItemDescription", ResourceType = typeof(Resources.RPT.PPM))]
        public string ItemDescription { get; set; }
        [Display(Name = "PPM_ReferenceItemCode", ResourceType = typeof(Resources.RPT.PPM))]
        public string ReferenceItemCode { get; set; }
        [Display(Name = "PPM_Uom", ResourceType = typeof(Resources.RPT.PPM))]
        public string Uom { get; set; }
        [Display(Name = "PPM_ReceivedQty", ResourceType = typeof(Resources.RPT.PPM))]
        public decimal? ReceivedQty { get; set; }
        [Display(Name = "PPM_RejectedQty", ResourceType = typeof(Resources.RPT.PPM))]
        public decimal? RejectedQty { get; set; }
        [Display(Name = "PPM_PPM", ResourceType = typeof(Resources.RPT.PPM))]
        public string PPM
        {
            get
            {
                string ppmString = string.Empty;
                if (ReceivedQty == null || ReceivedQty == 0)
                {
                    ppmString = "NA";
                }
                else if (ReceivedQty == null || RejectedQty == 0)
                {
                    ppmString = "0";
                }
                else
                {
                    ppmString = (1000000 * RejectedQty.Value / ReceivedQty.Value).ToString("0.##");
                }
                return ppmString;
            }
        }
    }
}