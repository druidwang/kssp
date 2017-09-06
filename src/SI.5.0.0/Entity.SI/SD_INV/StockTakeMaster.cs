namespace com.Sconit.Entity.SI.SD_INV
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class StockTakeMaster
    {
        #region O/R Mapping Properties
        public String StNo { get; set; }
        //全盘,抽盘
        public com.Sconit.CodeMaster.StockTakeType StockTakeType { get; set; }
        //动态盘点,静态盘点
        //public com.Sconit.CodeMaster.StockTakeType StockTakeDsType { get; set; }
        public String Location { get; set; }
        public com.Sconit.CodeMaster.StockTakeStatus Status { get; set; }
        //是否需要扫描条码
        public Boolean IsScan { get; set; }
        //生效时间,用于调节历史库存
        public DateTime EffectiveDate { get; set; }
        //取库存比较的时间,以此时间点的库存和盘点结果作比较得出盘点差异
        public DateTime StockTakeDate { get; set; }
        public String Region { get; set; }
        #endregion

        #region 辅助字段
        public string CurrentBin { get; set; }
        public List<StockTakeDetail> StockTakeDetails { get; set; }
        #endregion

    }
}
