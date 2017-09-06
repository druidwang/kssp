using System;

namespace com.Sconit.Entity.SI.SD_INV
{
    [Serializable]
    public class Hu
    {
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ReferenceItemCode { get; set; }
        public string Uom { get; set; }
        public string BaseUom { get; set; }
        public Decimal UnitCount { get; set; }
        public Decimal Qty { get; set; }
        public Decimal UnitQty { get; set; }
        public DateTime ManufactureDate { get; set; }
        public string ManufactureParty { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? FirstInventoryDate { get; set; }
        public Int16 PrintCount { get; set; }
        public com.Sconit.CodeMaster.HuStatus Status { get; set; }
        public string Location { get; set; }
        public string Region { get; set; }
        public string Bin { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public Boolean IsConsignment { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public Boolean IsFreeze { get; set; }
        public Boolean IsATP { get; set; }
        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }
        public string OccupyReferenceNo { get; set; }
        public string SupplierLotNo { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string OrderNo { get; set; }
        public string ReceiptNo { get; set; }
        public string Memo { get; set; }
        public string Shift { get; set; }
        public string Flow { get; set; }
        public string ItemVersion { get; set; }
        public string RefHu { get; set; }
        public string Direction { get; set; }
        public Boolean IsOdd { get; set; }
        public CodeMaster.HuOption HuOption { get; set; }
        public string AgingLocation { get; set; }

        public string PalletCode { get; set; }

        public Boolean IsExternal { get; set; }

        public Boolean IsPallet { get; set; }

        #region  ¸¨Öú×Ö¶Î
        public decimal CurrentQty { get; set; }
        public string Station { get; set; }
        public int OrderDetId { get; set; }
        public bool IsEffective { get; set; }
        #endregion
    }
}