namespace com.Sconit.Entity.SI.SD_INV
{
    using System;

    [Serializable]
    public class StockTakeDetail
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        public string StNo { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string HuId { get; set; }
        public Decimal Qty { get; set; }
        public string Location { get; set; }
        public string Bin { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime StockTakeDate { get; set; }
        #endregion
    }
}
