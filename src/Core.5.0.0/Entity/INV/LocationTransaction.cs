
//TODO: Add other using statements here

using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.INV
{
    public partial class LocationTransaction
    {
        #region Non O/R Mapping Properties
        [Export(ExportName = "LocationTransReport", ExportSeq = 109)]
        [Display(Name = "Hu_SupplierLotNo", ResourceType = typeof(Resources.INV.Hu))]
        public string SupplierLotNo { get; set; }

        //[Export(ExportName = "LocationTransReport", ExportSeq = 103)]
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }

        //[Export(ExportName = "LocationTransReport", ExportSeq = 106)]
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TransactionIOType, ValueField = "IOType")]
        [Display(Name = "LocationTransaction_IoType", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string IOTypeDescription { get; set; }

        [Export(ExportName = "LocationTransReport", ExportSeq = 100)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.QualityType, ValueField = "QualityType")]
        [Display(Name = "LocationTransaction_QualityType", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string QualityTypeDescription { get; set; }
		
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TransactionType, ValueField = "TransactionType")]
        [Display(Name = "LocationTransaction_TransactionType", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string TransactionTypeDescription { get; set; }

        [Export(ExportName = "LocationTransReport", ExportSeq = 10)]
        [Display(Name = "LocationTransaction_TransactionType", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string TransactionTypeFullDescription
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.LocationIOReason))
                {
                    return TransactionTypeDescription + "/" + LocationIOReason;
                }
                else
                {
                    return TransactionTypeDescription;
                }
            }
        }


        [Export(ExportName = "LocationTransReport", ExportSeq = 101)]
        [Display(Name = "LocationTransaction_ItemDescription", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string ItemDescription { get; set; }

        #region ±®±Ì”√

        [Display(Name = "LocationTransaction_Location", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string Location
        {
            get
            {
                return this.IOType == com.Sconit.CodeMaster.TransactionIOType.In ? this.LocationTo : this.LocationFrom;
            }
        }
        public decimal ProcurementInQty
        {
            get
            {
                decimal qty = 0;
                if (this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_PO
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_PO_VOID
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_PO
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_PO_VOID)
                {
                    qty = this.Qty;
                }
                return qty;
            }
        }

        public decimal DistributionOutQty
        {
            get
            {
                decimal qty = 0;
                if (this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SO
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SO_VOID
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_SO
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_SO_VOID)
                {
                    qty = this.Qty;
                }
                return qty;
            }
        }

        public decimal TransferInQty
        {
            get
            {
                decimal qty = 0;
                if (this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_TR
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_TR_VOID
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_TR_RTN
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_TR_RTN_VOID)
                {
                    qty = this.Qty;
                }
                return qty;
            }
        }

        public decimal TransferOutQty
        {
            get
            {
                decimal qty = 0;
                if (this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_TR
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_TR_VOID
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_TR_RTN
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_TR_RTN_VOID)
                {
                    qty = this.Qty;
                }
                return qty;
            }
        }

        public decimal ProductionInQty
        {
            get
            {
                decimal qty = 0;
                if (this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_WO
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_WO_VOID)
                {
                    qty = this.Qty;
                }
                return qty;
            }
        }

        public decimal ProductionOutQty
        {
            get
            {
                decimal qty = 0;
                if (this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_WO
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_WO_VOID
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_WO_BF
                    || this.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_WO_BF_VOID)
                {
                    qty = this.Qty;
                }
                return qty;
            }
        }
        #endregion
        #endregion
    }
}