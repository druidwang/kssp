namespace com.Sconit.Entity.SI.SD_ORD
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class InspectDetail
    {
        public Int32 Id { get; set; }
        public string InspectNo { get; set; }
        public Int32 Sequence { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ReferenceItemCode { get; set; }
        public Decimal UnitCount { get; set; }
        public string Uom { get; set; }
        public string BaseUom { get; set; }
        public Decimal UnitQty { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }

        public string LocationFrom { get; set; }
        public string CurrentLocation { get; set; }
        public Decimal InspectQty { get; set; }
        public Decimal QualifyQty { get; set; }
        public Decimal RejectQty { get; set; }
        public Boolean IsJudge { get; set; }

        #region 辅助字段
        public decimal CurrentQty { get; set; }
        public decimal Carton { get; set; }
        #endregion

    }
}
