using System;
using System.Collections.Generic;

namespace com.Sconit.PrintModel.INV
{
    [Serializable]
    public partial class PrintStockTakeMaster 
    {
        #region O/R Mapping Properties

        public String StNo { get; set; }

        public String Region { get; set; }

        //是否需要扫描条码
        public Boolean IsScanHu { get; set; }

        public Int16 Type { get; set; }

        //生效时间,用于调节历史库存
        public DateTime? EffectiveDate { get; set; }

        public DateTime? BaseInventoryDate { get; set; }
        public string CostCenter { get; set; }

        #region 审计字段
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        #endregion

        #endregion

    }

}
