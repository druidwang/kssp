using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public partial class OrderBomDetail
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string OrderNo { get; set; }
        public Int32 OrderDetailId { get; set; }
        public string Bom { get; set; }
        public string Item { get; set; }
        public string ReferenceItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }  //Bom单位
        public string BaseUom { get; set; }
        public string ManufactureParty { get; set; }
        public Int32 Operation { get; set; }
        public string OpReference { get; set; }
        public Decimal OrderedQty { get; set; }
        public Decimal BackflushedQty { get; set; }
        public Decimal BackflushedRejectQty { get; set; }
        public Decimal BackflushedScrapQty { get; set; }
        public Decimal UnitQty { get; set; }
        public Decimal BomUnitQty { get; set; }
        public string Location { get; set; }
        public Boolean IsPrint { get; set; }
        public com.Sconit.CodeMaster.BackFlushMethod BackFlushMethod { get; set; }
        public com.Sconit.CodeMaster.FeedMethod FeedMethod { get; set; }
        public Boolean IsScanHu { get; set; }
        public Boolean IsAutoFeed { get; set; }
        public DateTime EstimateConsumeTime { get; set; }
        //public Int32 CreateUserId { get; set; }
        //public string CreateUserName { get; set; }
        //public DateTime CreateDate { get; set; }
        //public Int32 LastModifyUserId { get; set; }
        //public string LastModifyUserName { get; set; }
        //public DateTime LastModifyDate { get; set; }
        //public Int32 Version { get; set; }

        #endregion

    }
}
