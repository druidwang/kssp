namespace com.Sconit.Entity.SI.SD_ORD
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class OrderDetail
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        public string OrderNo { get; set; }
        public Int32 Sequence { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ReferenceItemCode { get; set; }
        public string BaseUom { get; set; }
        public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public string ManufactureParty { get; set; }
        public Decimal OrderedQty { get; set; }
        public Decimal ShippedQty { get; set; }
        public Decimal ReceivedQty { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public Boolean IsScanHu { get; set; }
        public Boolean IsChangeUnitCount { get; set; }
        public string Direction { get; set; }
        #endregion

        #region 辅助字段
        public List<OrderDetailInput> OrderDetailInputs { get; set; }
        public decimal CurrentQty { get; set; }
        public int Carton { get; set; }
        public decimal RemainShippedQty { get; set; }
        public decimal RemainReceivedQty { get; set; }
        #endregion
    }

    public class OrderDetailInput
    {
        public int Id { get; set; }
        public string HuId { get; set; }
        public decimal Qty { get; set; }
        public decimal ShipQty { get; set; }
        public decimal ReceiveQty { get; set; }
        public string LotNo { get; set; }
        public string Bin { get; set; }
        public bool IsHuInLocation { get; set; }
        public string Direction { get; set; }
    }

    public class AnDonInput
    {
        public string CardNo { get; set; }
        public string Flow { get; set; }
        public string LocationTo { get; set; }
        public string Item { get; set; }
        public string Uom { get; set; }
        public string ManufactureParty { get; set; }
        public decimal UnitCount { get; set; }
        public string Note { get; set; }
    }
}
