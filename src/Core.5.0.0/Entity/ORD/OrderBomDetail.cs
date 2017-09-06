using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class OrderBomDetail
    {
        #region Non O/R Mapping Properties

        [Display(Name = "OrderBomDetail_FeedQty", ResourceType = typeof(Resources.ORD.OrderBomDetail))]
        public decimal FeedQty { get; set; }

        [Display(Name = "OrderBomDetail_FeedLocation", ResourceType = typeof(Resources.ORD.OrderBomDetail))]
        public string FeedLocation { get; set; }

        public string WorkCenter { get; set; }

        public bool IsReport { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.BackFlushMethod, ValueField = "BackFlushMethod")]
        [Display(Name = "OrderBomDetail_BackFlushMethod", ResourceType = typeof(Resources.ORD.OrderBomDetail))]
        public string BackFlushMethodDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.FeedMethod, ValueField = "FeedMethod")]
        [Display(Name = "OrderBomDetail_FeedMethod", ResourceType = typeof(Resources.ORD.OrderBomDetail))]
        public string FeedMethodDescription { get; set; }
        [Display(Name = "ProductLineLocationDetail_HuId", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public string HuId { get; set; }
        [Display(Name = "OrderBomDetail_Flow", ResourceType = typeof(Resources.ORD.OrderBomDetail))]
        public string Flow { get; set; }
        [Display(Name = "BomDetail_UnitCount", ResourceType = typeof(Resources.PRD.Bom))]
        public Decimal UnitCount { get; set; }
        #endregion
    }
}