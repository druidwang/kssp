using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Entity.SCM
{
    [Serializable]
    public partial class FlowDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "FlowDetail_Id", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Int32 Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowDetail_Flow", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Flow { get; set; }

        [Display(Name = "FlowDetail_Strategy", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public com.Sconit.CodeMaster.FlowStrategy Strategy { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowDetail_Sequence", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Int32 Sequence { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowDetail_Item", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Item { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowDetail_ReferenceItemCode", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string ReferenceItemCode { get; set; }

        public string BaseUom { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowDetail_Uom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Uom { get; set; }

        //[Display(Name = "FlowDetail_Uom1", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public string Uom1 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Range(0.000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowDetail_UnitCount", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal UnitCount { get; set; }

        [Display(Name = "FlowDetail_UnitCountDescription", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string UnitCountDescription { get; set; }
        [Display(Name = "FlowDetail_MinUnitCount", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal MinUnitCount { get; set; }


        [Display(Name = "FlowDetail_StartDate", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public DateTime? StartDate { get; set; }

        [Display(Name = "FlowDetail_EndDate", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public DateTime? EndDate { get; set; }

        //[Display(Name = "FlowDetail_HuLotSize", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public Decimal? HuLotSize { get; set; }

        [Display(Name = "FlowDetail_Bom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Bom { get; set; }

        [Display(Name = "FlowDetail_LocationFrom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string LocationFrom { get; set; }

        [Display(Name = "FlowDetail_LocationTo", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string LocationTo { get; set; }

        [Display(Name = "FlowDetail_Machine", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Machine { get; set; }

        //[Display(Name = "FlowDetail_ShiftQuota", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public Double ShiftQuota { get; set; }

        //[Display(Name = "FlowDetail_InspectLocationFrom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public string InspectLocationFrom { get; set; }

        //[Display(Name = "FlowDetail_InspectLocationTo", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public string InspectLocationTo { get; set; }

        //[Display(Name = "FlowDetail_RejectLocationFrom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public string RejectLocationFrom { get; set; }

        //[Display(Name = "FlowDetail_RejectLocationTo", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public string RejectLocationTo { get; set; }
        //[Display(Name = "FlowDetail_PartyFrom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public string PartyFrom { get; set; }

        [Display(Name = "FlowDetail_BillAddress", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string BillAddress { get; set; }

        [Display(Name = "FlowDetail_PriceList", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string PriceList { get; set; }

        [Display(Name = "FlowDetail_Routing", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Routing { get; set; }

        [Display(Name = "FlowDetail_ReturnRouting", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string ReturnRouting { get; set; }

        [Display(Name = "FlowDetail_IsAutoCreate", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Boolean IsAutoCreate { get; set; }

        [Display(Name = "FlowDetail_IsInspect", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Boolean IsInspect { get; set; }

        [Display(Name = "FlowDetail_IsRejectInspect", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Boolean IsRejectInspect { get; set; }

        [Display(Name = "FlowDetail_SafeStock", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal SafeStock { get; set; }

        [Display(Name = "FlowDetail_MaxStock", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal MaxStock { get; set; }

        [Display(Name = "FlowDetail_MinLotSize", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal MinLotSize { get; set; }

        [Display(Name = "FlowDetail_OrderLotSize", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal OrderLotSize { get; set; }

        [Display(Name = "FlowDetail_ReceiveLotSize", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal ReceiveLotSize { get; set; }

        [Display(Name = "FlowDetail_BatchSize", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal BatchSize { get; set; }

        [Display(Name = "FlowDetail_RoundUpOption", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public com.Sconit.CodeMaster.RoundUpOption RoundUpOption { get; set; }

        //[Display(Name = "FlowDetail_BillTerm", ResourceType = typeof(Resources.SCM.FlowDetail))]
        //public com.Sconit.CodeMaster.OrderBillTerm FlowDetailBillTerm { get; set; }

        [Display(Name = "FlowDetail_BillTerm", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public com.Sconit.CodeMaster.OrderBillTerm BillTerm { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowDetail_MrpWeight", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Int32 MrpWeight { get; set; }

        [Display(Name = "FlowDetail_MrpTotal", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public decimal MrpTotal { get; set; }

        public decimal MrpTotalAdjust { get; set; }

        [Display(Name = "FlowDetail_ExtraDemandSource", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string ExtraDemandSource { get; set; }

        [Display(Name = "FlowDetail_Container", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Container { get; set; }

        [Display(Name = "FlowDetail_ContainerDescription", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string ContainerDescription { get; set; }

        public Int32 CreateUserId { get; set; }

        public string CreateUserName { get; set; }

        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        public string LastModifyUserName { get; set; }

        public DateTime LastModifyDate { get; set; }

        public string ProductionScan { get; set; }
        [Display(Name = "FlowDetail_PickStrategy", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string PickStrategy { get; set; }
        [Display(Name = "FlowDetail_EBELN", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string EBELN { get; set; }
        [Display(Name = "FlowDetail_EBELP", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string EBELP { get; set; }
        [Display(Name = "FlowDetail_IsChangeUnitCount", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Boolean IsChangeUnitCount { get; set; }

        public string BinTo { get; set; }

        [Display(Name = "FlowDetail_MrpPriority", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public int MrpPriority { get; set; }

        [Display(Name = "FlowDetail_ExtraLocationTo", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string ExtraLocationTo { get; set; }
        [Display(Name = "FlowDetail_ExtraLocationFrom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string ExtraLocationFrom { get; set; }

        public string PalletCode { get; set; }
        public Decimal PalletLotSize { get; set; }
        public Decimal PackageVolume { get; set; }
        public Decimal PackageWeight { get; set; }
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
            FlowDetail another = obj as FlowDetail;

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
