using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class ReceiptMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Export(ExportName = "ProductionReceiptOrderMaster", ExportSeq = 10)]
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 5)]
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 10)]
        [Export(ExportName = "SupplierReturnReceiptOrderMaster", ExportSeq = 10)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 10)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ReceiptMaster_ReceiptNo", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ReceiptNo { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 65)]
        [Display(Name = "ReceiptMaster_ExternalReceiptNo", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ExternalReceiptNo { get; set; }
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 20)]
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 20)]
        [Export(ExportName = "SupplierReturnReceiptOrderMaster", ExportSeq = 20)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 30)]
        [Display(Name = "ReceiptMaster_IpNo", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string IpNo { get; set; }
        public string SequenceNo { get; set; }
        [Export(ExportName = "ProductionReceiptOrderMaster", ExportSeq = 20)]
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "SupplierReturnReceiptOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 10)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 20)]
        [Display(Name = "ReceiptMaster_Flow", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string Flow { get; set; }

        [Display(Name = "ReceiptMaster_Status", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public CodeMaster.ReceiptStatus Status { get; set; }

        //[Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 80)]
        [Display(Name = "ReceiptMaster_Type", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public com.Sconit.CodeMaster.IpDetailType Type { get; set; }

        [Display(Name = "ReceiptMaster_OrderType", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }

        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }

        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }

        [Display(Name = "ReceiptMaster_PartyFrom", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string PartyFrom { get; set; }
        [Export(ExportName = "ProductionReceiptOrderMaster", ExportSeq = 40, ExportTitle = "ReceiptMaster_PartyFrom_Procurement", ExportTitleResourceType = typeof(@Resources.ORD.ReceiptMaster))]
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 30, ExportTitle = "ReceiptMaster_PartyFrom_Procurement", ExportTitleResourceType = typeof(@Resources.ORD.ReceiptMaster))]
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 40)]
        [Display(Name = "ReceiptMaster_PartyFromName", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string PartyFromName { get; set; }

        [Display(Name = "ReceiptMaster_PartyTo", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string PartyTo { get; set; }
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "SupplierReturnReceiptOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 50)]
        [Display(Name = "ReceiptMaster_PartyToName", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string PartyToName { get; set; }

        [Display(Name = "ReceiptMaster_ShipFrom", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ShipFrom { get; set; }

        [Display(Name = "ReceiptMaster_ShipFromAddress", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ShipFromAddress { get; set; }
        [Display(Name = "ReceiptMaster_ShipFromTel", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ShipFromTel { get; set; }

        public string ShipFromCell { get; set; }

        public string ShipFromFax { get; set; }
        [Display(Name = "ReceiptMaster_ShipFromContact", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ShipFromContact { get; set; }

        [Display(Name = "ReceiptMaster_ShipTo", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ShipTo { get; set; }
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 60)]
        [Export(ExportName = "SupplierReturnReceiptOrderMaster", ExportSeq = 50)]
        [Display(Name = "ReceiptMaster_ShipToAddress", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ShipToAddress { get; set; }
        [Display(Name = "ReceiptMaster_ShipToTel", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ShipToTel { get; set; }

        public string ShipToCell { get; set; }

        public string ShipToFax { get; set; }
        [Display(Name = "ReceiptMaster_ShipToContact", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ShipToContact { get; set; }

        [Display(Name = "ReceiptMaster_Dock", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string Dock { get; set; }

        public DateTime EffectiveDate { get; set; }
        [Display(Name = "ReceiptMaster_IsReceiveScanHu", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public Boolean IsReceiveScanHu { get; set; }

        public CodeMaster.CreateHuOption CreateHuOption { get; set; }

        [Display(Name = "ReceiptMaster_IsPrintReceipt", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public Boolean IsPrintReceipt { get; set; }

        public Boolean IsReceiptPrinted { get; set; }
        [Display(Name = "ReceiptMaster_ReceiptTemplate", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ReceiptTemplate { get; set; }

        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "ProductionReceiptOrderMaster", ExportSeq = 60)]
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 70)]
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 90)]
        [Export(ExportName = "SupplierReturnReceiptOrderMaster", ExportSeq = 80)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 80)]
        [Display(Name = "ReceiptMaster_CreateUserName", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string CreateUserName { get; set; }
        [Export(ExportName = "ProductionReceiptOrderMaster", ExportSeq = 70)]
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 80)]
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 100)]
        [Export(ExportName = "SupplierReturnReceiptOrderMaster", ExportSeq = 70)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 90)]
        [Display(Name = "ReceiptMaster_CreateDate", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "ReceiptMaster_LastModifyUserName", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "ReceiptMaster_LastModifyDate", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "ReceiptMaster_IsCheckPartyFromAuthority", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public Boolean IsCheckPartyFromAuthority { get; set; }
        [Display(Name = "ReceiptMaster_IsCheckPartyToAuthority", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public Boolean IsCheckPartyToAuthority { get; set; }

        public Int32 Version { get; set; }
        [Display(Name = "ReceiptMaster_WMSNo", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string WMSNo { get; set; }
        [Display(Name = "ReceiptMaster_CancelReason", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string CancelReason { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (ReceiptNo != null)
            {
                return ReceiptNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ReceiptMaster another = obj as ReceiptMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.ReceiptNo == another.ReceiptNo);
            }
        }
    }

}
