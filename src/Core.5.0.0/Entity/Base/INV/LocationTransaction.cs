using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class LocationTransaction : EntityBase
    {
        #region O/R Mapping Properties

        [Display(Name = "LocationTransaction_Id", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Int64 Id { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 30)]
        [Display(Name = "LocationTransaction_OrderNo", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string OrderNo { get; set; }
        //[Display(Name = "OrderType", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public com.Sconit.CodeMaster.OrderType? OrderType { get; set; }
        public com.Sconit.CodeMaster.OrderSubType? OrderSubType { get; set; }
        //[Display(Name = "OrderDetailSequence", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Int32 OrderDetailSequence { get; set; }
        //[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Int32 OrderDetailId { get; set; }
        //[Display(Name = "OrderBomDetailId", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Int32 OrderBomDetailId { get; set; }
        public Int32 OrderBomDetailSequence { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 40)]
        [Display(Name = "LocationTransaction_IpNo", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string IpNo { get; set; }
        //[Display(Name = "IpDetailSequence", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Int32 IpDetailId { get; set; }
        public Int32 IpDetailSequence { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 50)]
        [Display(Name = "LocationTransaction_ReceiptNo", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string ReceiptNo { get; set; }
        //[Display(Name = "ReceiptDetailSequence", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Int32 ReceiptDetailId { get; set; }
        public Int32 ReceiptDetailSequence { get; set; }
        [Display(Name = "LocationTransaction_SequenceNo", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string SequenceNo { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 100)]
        [Display(Name = "LocationTransaction_Item", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string Item { get; set; }
        //[Display(Name = "ItemDescription", ResourceType = typeof(Resources.INV.LocationTransaction))]
        //public string ItemDescription { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 140)]
        [Display(Name = "LocationTransaction_Uom", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string Uom { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 150)]
        [Display(Name = "LocationTransaction_BaseUom", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string BaseUom { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 130)]
        [Display(Name = "LocationTransaction_Qty", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Decimal Qty { get; set; }
        public Boolean IsConsignment { get; set; }
        public Int32 PlanBill { get; set; }
        public Decimal PlanBillQty { get; set; }
        public Int32 ActingBill { get; set; }
        public Decimal ActingBillQty { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 160)]
        [Display(Name = "LocationTransaction_UnitQty", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Decimal UnitQty { get; set; }
        [Display(Name = "LocationTransaction_QualityType", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 110)]
        [Display(Name = "LocationTransaction_HuId", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string HuId { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 120, ExportTitle = "Hu_manufacture_date",ExportTitleResourceType=typeof(Resources.INV.Hu))]
        [Display(Name = "LocationTransaction_LotNo", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string LotNo { get; set; }
        [Display(Name = "LocationTransaction_TransactionType", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public com.Sconit.CodeMaster.TransactionType TransactionType { get; set; }
        [Display(Name = "LocationTransaction_IoType", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public com.Sconit.CodeMaster.TransactionIOType IOType { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 60)]
        [Display(Name = "LocationTransaction_PartyFrom", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string PartyFrom { get; set; }
        //[Display(Name = "PartyFromName", ResourceType = typeof(Resources.INV.LocationTransaction))]
        //public string PartyFromName { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 70)]
        [Display(Name = "LocationTransaction_PartyTo", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string PartyTo { get; set; }
        //[Display(Name = "PartyToName", ResourceType = typeof(Resources.INV.LocationTransaction))]
        //public string PartyToName { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 80)]
        [Display(Name = "LocationTransaction_LocationFrom", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string LocationFrom { get; set; }
        //[Display(Name = "LocationFromName", ResourceType = typeof(Resources.INV.LocationTransaction))]
        //public string LocationFromName { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 90)]
        [Display(Name = "LocationTransaction_LocationTo", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string LocationTo { get; set; }
        //[Display(Name = "LocationToName", ResourceType = typeof(Resources.INV.LocationTransaction))]
        //public string LocationToName { get; set; }
        //[Display(Name = "LocationIOReason", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string LocationIOReason { get; set; }
        //[Display(Name = "LocationIOReasonDesc", ResourceType = typeof(Resources.INV.LocationTransaction))]
        //public string LocationIOReasonDesc { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 20)]
        [Display(Name = "LocationTransaction_EffectiveDate", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public DateTime EffectiveDate { get; set; }
        [Display(Name = "LocationTransaction_CreateUserId", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public Int32 CreateUserId { get; set; }
        //[Display(Name = "CreateUserName", ResourceType = typeof(Resources.INV.LocationTransaction))]
        //public string CreateUserName { get; set; }
        [Export(ExportName = "LocationTransReport", ExportSeq = 25)]
        [Display(Name = "LocationTransaction_CreateDate", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public DateTime CreateDate { get; set; }

        public string TraceCode { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            LocationTransaction another = obj as LocationTransaction;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }

}
