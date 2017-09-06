using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class HuSearchModel : SearchModelBase
    {
        public String HuId { get; set; }
        public String OldHu { get; set; }
        public String NewHu { get; set; }
        public String Flow { get; set; }
        public String OrderNo { get; set; }
        public String Item { get; set; }
        public String HuItem { get; set; }
        public String SpareItem { get; set; }
        public String CreateUserName { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public string LotNo { get; set; }
        public string Location { get; set; }
        public string LotNoTo { get; set; }
        public string ManufactureParty { get; set; }
        public DateTime? RemindExpireDate_start { get; set; }
        public bool IsExport { get; set; }
        public bool IsSumByItem { get; set; }
        public DateTime? RemindExpireDate_End { get; set; }
        public int SearchCondition { get; set; }
        public string SupplierLotNo { get; set; }

        public string PalletCode { get; set; }
    }
}