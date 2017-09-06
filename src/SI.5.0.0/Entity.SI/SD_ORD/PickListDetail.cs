using System;
using System.Collections.Generic;

namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public partial class PickListDetail
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string PickListNo { get; set; }
        public Int32 OrderDetailId { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ReferenceItemCode { get; set; }
        public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public string LocationFrom { get; set; }
        public string Bin { get; set; }
        public Decimal Qty { get; set; }
        public Decimal PickedQty { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public string LocationTo { get; set; }
        public Boolean IsInspect { get; set; }
        public Boolean IsOdd { get; set; }
        public Boolean IsInventory { get; set; }
        public string ManufactureParty { get; set; }
        public string OrderNo { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public Boolean IsMatchDirection { get; set; }
        public string Direction { get; set; }
        public decimal UcDeviation { get; set; }
        public List<string> ItemDisconList { get; set; }
        #endregion

        #region ¸¨Öú×Ö¶Î
        public List<PickListDetailInput> PickListDetailInputs { get; set; }
        public decimal CurrentQty { get; set; }
        public int Carton { get; set; }
        public decimal RemainShippedQty { get; set; }
        #endregion

    }
    public class PickListDetailInput
    {
        public int Id { get; set; }
        public string HuId { get; set; }
    }
}
