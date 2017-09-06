using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.INV
{
    public partial class Hu
    {
        #region Non O/R Mapping Properties
        public string ManufacturePartyDescription { get; set; }

        public string BinTo { get; set; }

        public string OldHus { get; set; }

        //public string Flow { get; set; }
        #endregion
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 46)]
        [Export(ExportName = "OutOfExpireTimeWarning", ExportSeq = 26)]
        [Export(ExportName = "ShelfLifeWarningSummary", ExportSeq = 40)]
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }
        /// <summary>
        /// 车型
        /// </summary>
        [Display(Name = "Hu_Model", ResourceType = typeof(Resources.INV.Hu))]
        public string Model { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        [Display(Name = "Hu_Length", ResourceType = typeof(Resources.INV.Hu))]
        public double Length { get; set; }

        public HuStatus HuStatus { get; set; }
        //[Export(ExportName = "ShelfLifeWarning", ExportSeq = 110, ExportTitle = "Hu_HuStatusDescription", ExportTitleResourceType = typeof(@Resources.INV.Hu))]
        [Display(Name = "Hu_HuStatusDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string ExpireStatus { get; set; }

        [com.Sconit.Entity.SYS.CodeDetailDescription(CodeMaster = com.Sconit.CodeMaster.CodeMaster.HuOption, ValueField = "HuOption")]
        [Display(Name = "Hu_HuOptionTypeDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string HuOptionTypeDescription { get; set; }

        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 25)]
        [Display(Name = "LocationLotDetail_Location", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string Location { get; set; }

        [Export(ExportName = "Ageing", ExportSeq = 6)]
        [Display(Name = "LocationLotDetail_Bin", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string Bin { get; set; }

        [Export(ExportName = "Ageing", ExportSeq = 65)]
        [Display(Name = "LocationLotDetail_IsFreeze", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public int IsFreeze { get; set; }

        //TotalQty,0 UnAgingQty,0 AgedQty,0 AgingQty
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 30)]
        [Display(Name = "Hu_TotalQty", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal TotalQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 40)]
        [Display(Name = "Hu_UnAgingQty", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal UnAgingQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 60)]
        [Display(Name = "Hu_AgedQty", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal AgedQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 50)]
        [Display(Name = "Hu_AgingQty", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal AgingQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 70)]
        [Display(Name = "Hu_SQty", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal SQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 80)]
        [Display(Name = "Hu_NoNeedAgingQty", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal NoNeedAgingQty { get; set; }

        //FreezedQty,NonFreezeQty,QulifiedQty,InspectQty,InQulifiedQty
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Export(ExportName = "Ageing", ExportSeq = 85, ExportTitleResourceType = typeof(Resources.INV.Hu), ExportTitle = "QualityType")]
        [com.Sconit.Entity.SYS.CodeDetailDescription(CodeMaster = com.Sconit.CodeMaster.CodeMaster.QualityType, ValueField = "QualityType")]
        [Display(Name = "QualityType", ResourceType = typeof(Resources.INV.Hu))]
        public string QualityTypeDescription { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 90)]
        [Display(Name = "LocationLotDetail_IsFreeze", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public Decimal FreezedQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 100)]
        [Display(Name = "Hu_NonFreezeQty", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal NonFreezeQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 110)]
        [Display(Name = "LocationDetailIOB_EndNml", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public Decimal QulifiedQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 120)]
        [Display(Name = "LocationDetailIOB_EndInp", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public Decimal InspectQty { get; set; }
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 130)]
        [Display(Name = "LocationDetailIOB_EndRej", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public Decimal InQulifiedQty { get; set; }

        [Export(ExportName = "ShelfLifeWarningSummary", ExportSeq = 50)]
        [Display(Name = "Hu_Qty0", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal Qty0 { get; set; }
        [Export(ExportName = "ShelfLifeWarningSummary", ExportSeq = 60)]
        [Display(Name = "Hu_Qty1", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal Qty1 { get; set; }
        [Export(ExportName = "ShelfLifeWarningSummary", ExportSeq = 70)]
        [Display(Name = "Hu_Qty2", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal Qty2 { get; set; }
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 100)]
        [Display(Name = "Hu_ExpireDate", ResourceType = typeof(Resources.INV.Hu))]
        public string ExpireDateValue { get; set; }
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 90)]
        [Display(Name = "Hu_RemindExpireDate", ResourceType = typeof(Resources.INV.Hu))]
        public string RemindExpireDateValue { get; set; }


        /// <summary>
        /// 拆箱数字符串
        /// </summary>
        [Display(Name = "Hu_DevanningQty1", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal DevanningQty1 { get; set; }

        [Display(Name = "Hu_DevanningQty2", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal DevanningQty2 { get; set; }

        [Display(Name = "Hu_DevanningQty3", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal DevanningQty3 { get; set; }

        [Display(Name = "Hu_DevanningQty4", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal DevanningQty4 { get; set; }

        [Display(Name = "Hu_DevanningQty", ResourceType = typeof(Resources.INV.Hu))]
        public string DevanningQtyStr { get; set; }
    }
}