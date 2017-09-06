using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.MD;

//TODO: Add other using statements here

namespace com.Sconit.Entity.PRD
{
    public partial class BomDetail
    {
        #region Non O/R Mapping Properties

        //缓存单位成品的BOM用量 已包含废品率
        public decimal CalculatedQty { get; set; }
        //成品的BOM单位用量 不包含废品率
        public Decimal UnitBomQty { get; set; }

        //缓存投料/退料的库位
        [Display(Name = "BomDetail_FeedLocation", ResourceType = typeof(Resources.PRD.Bom))]
        public string FeedLocation { get; set; }

        //缓存本次投料数
        [Display(Name = "BomDetail_FeedQty", ResourceType = typeof(Resources.PRD.Bom))]
        public decimal FeedQty { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.BomStructureType, ValueField = "StructureType")]
        [Display(Name = "BomDetail_StructureType", ResourceType = typeof(Resources.PRD.Bom))]
        public string StructureTypeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.BackFlushMethod, ValueField = "BackFlushMethod")]
        [Display(Name = "BomDetail_BackFlushMethod", ResourceType = typeof(Resources.PRD.Bom))]
        public string BackFlushMethodDescription { get; set; }
        

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.FeedMethod, ValueField = "FeedMethod")]
        [Display(Name = "BomDetail_FeedMethod", ResourceType = typeof(Resources.PRD.Bom))]
        public string FeedMethodDescription { get; set; }

        public Item CurrentItem { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 70)]
        [Display(Name = "BomDetail_ItemDescription", ResourceType = typeof(Resources.PRD.Bom))]
        public string ItemDescription { get; set; }

        [Display(Name = "BomDetail_ReferenceItemCode", ResourceType = typeof(Resources.PRD.Bom))]
        public string ReferenceItemCode { get; set; }

        [Display(Name = "BomDetail_UnitCount", ResourceType = typeof(Resources.PRD.Bom))]
        public Decimal UnitCount { get; set; }
        [Display(Name = "BomDetail_UnitCountDescription", ResourceType = typeof(Resources.PRD.Bom))]
        public string UnitCountDescription { get; set; }
        [Display(Name = "BomDetail_MinUnitCount", ResourceType = typeof(Resources.PRD.Bom))]
        public Decimal MinUnitCount { get; set; }
        [Display(Name = "BomDetail_ManufactureParty", ResourceType = typeof(Resources.PRD.Bom))]
        public Decimal ManufactureParty { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.BomMrpOption, ValueField = "BomMrpOption")]
        [Display(Name = "BomDetail_BomMrpOption", ResourceType = typeof(Resources.PRD.Bom))]
        public string BomMrpOptionDescription { get; set; }

        public BomMaster CurrentBom { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 20)]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "BomMaster_Desc", ResourceType = typeof(Resources.PRD.Bom))]
        public string Description { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 30)]
        [Display(Name = "BomMaster_Qty", ResourceType = typeof(Resources.PRD.Bom))]
        public decimal Qty { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 40)]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "BomMaster_Uom", ResourceType = typeof(Resources.PRD.Bom))]
        public string MstrUom { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 50)]
        [Display(Name = "BomMaster_IsActive", ResourceType = typeof(Resources.PRD.Bom))]
        public Boolean IsActive { get; set; }
        #endregion
    }
}