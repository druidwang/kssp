using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Entity.SCM
{
    [Serializable]
    public partial class FlowMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowMaster_Code", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(100, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowMaster_Description", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string Description { get; set; }

        [Display(Name = "FlowMaster_IsActive", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsActive { get; set; }

        [Display(Name = "FlowMaster_Type", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public com.Sconit.CodeMaster.OrderType Type { get; set; }

        [Display(Name = "FlowMaster_ReferenceFlow", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string ReferenceFlow { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowMaster_PartyFrom", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string PartyFrom { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowMaster_PartyTo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string PartyTo { get; set; }

        [Display(Name = "FlowMaster_ShipFrom", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string ShipFrom { get; set; }

        [Display(Name = "FlowMaster_ShipTo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string ShipTo { get; set; }

        [Display(Name = "FlowMaster_LocationFrom", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string LocationFrom { get; set; }

        [Display(Name = "FlowMaster_LocationTo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string LocationTo { get; set; }


        //public string ResourceGroup { get; set; }

        [Display(Name = "FlowMaster_ResourceGroup", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public CodeMaster.ResourceGroup ResourceGroup { get; set; }
        //[Display(Name = "FlowMaster_InspectLocationFrom", ResourceType = typeof(Resources.SCM.FlowMaster))]
        //public string InspectLocationFrom { get; set; }

        //[Display(Name = "FlowMaster_InspectLocationTo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        //public string InspectLocationTo { get; set; }

        //[Display(Name = "FlowMaster_RejectLocationFrom", ResourceType = typeof(Resources.SCM.FlowMaster))]
        //public string RejectLocationFrom { get; set; }

        //[Display(Name = "FlowMaster_RejectLocationTo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        //public string RejectLocationTo { get; set; }

        [Display(Name = "FlowMaster_BillAddress", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string BillAddress { get; set; }

        [Display(Name = "FlowMaster_PriceList", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string PriceList { get; set; }

        [Display(Name = "FlowMaster_Dock", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string Dock { get; set; }

        [Display(Name = "FlowMaster_Routing", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string Routing { get; set; }

        [Display(Name = "FlowMaster_ReturnRouting", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string ReturnRouting { get; set; }

        [Display(Name = "FlowMaster_IsAutoCreate", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsAutoCreate { get; set; }

        [Display(Name = "FlowMaster_IsAutoRelease", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsAutoRelease { get; set; }

        [Display(Name = "FlowMaster_IsAutoStart", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsAutoStart { get; set; }

        [Display(Name = "FlowMaster_IsAutoShip", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsAutoShip { get; set; }

        [Display(Name = "FlowMaster_IsAutoReceive", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsAutoReceive { get; set; }

        [Display(Name = "FlowMaster_IsAutoBill", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsAutoBill { get; set; }

        [Display(Name = "FlowMaster_IsListDet", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsListDet { get; set; }

        [Display(Name = "FlowMaster_IsManualCreateDetail", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsManualCreateDetail { get; set; }

        [Display(Name = "FlowMaster_IsListPrice", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsListPrice { get; set; }

        [Display(Name = "FlowMaster_IsPrintOrder", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsPrintOrder { get; set; }

        [Display(Name = "FlowMaster_IsPrintAsn", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsPrintAsn { get; set; }

        [Display(Name = "FlowMaster_IsPrintRceipt", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsPrintRceipt { get; set; }

        [Display(Name = "FlowMaster_IsShipExceed", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsShipExceed { get; set; }

        [Display(Name = "FlowMaster_IsReceiveExceed", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsReceiveExceed { get; set; }

        [Display(Name = "FlowMaster_IsOrderFulfillUC", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsOrderFulfillUC { get; set; }

        [Display(Name = "FlowMaster_IsShipFulfillUC", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsShipFulfillUC { get; set; }

        [Display(Name = "FlowMaster_IsReceiveFulfillUC", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsReceiveFulfillUC { get; set; }

        [Display(Name = "FlowMaster_IsShipScanHu", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsShipScanHu { get; set; }

        [Display(Name = "FlowMaster_IsReceiveScanHu", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsReceiveScanHu { get; set; }

        [Display(Name = "FlowMaster_IsCreatePickList", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsCreatePickList { get; set; }

        [Display(Name = "FlowMaster_IsInspect", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsInspect { get; set; }

        [Display(Name = "FlowMaster_IsRejectInspect", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsRejectInspect { get; set; }

        [Display(Name = "FlowMaster_IsReceiveFifo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsReceiveFifo { get; set; }

        //[Display(Name = "FlowMaster_IsPickFromBin", ResourceType = typeof(Resources.SCM.FlowMaster))]
        //public Boolean IsPickFromBin { get; set; }

        [Display(Name = "FlowMaster_IsShipByOrder", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsShipByOrder { get; set; }

        //[Display(Name = "FlowMaster_IsReceiveByOrder", ResourceType = typeof(Resources.SCM.FlowMaster))]
        //public Boolean IsReceiveByOrder { get; set; }

        [Display(Name = "FlowMaster_IsAsnUniqueReceive", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsAsnUniqueReceive { get; set; }

        [Display(Name = "FlowMaster_IsMRP", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsMRP { get; set; }

        [Display(Name = "FlowMaster_ReceiveGapTo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public com.Sconit.CodeMaster.ReceiveGapTo ReceiveGapTo { get; set; }

        [Display(Name = "FlowMaster_ReceiptTemplate", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string ReceiptTemplate { get; set; }

        [Display(Name = "FlowMaster_OrderTemplate", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string OrderTemplate { get; set; }

        [Display(Name = "FlowMaster_AsnTemplate", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string AsnTemplate { get; set; }

        [Display(Name = "FlowMaster_HuTemplate", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string HuTemplate { get; set; }

        [Display(Name = "FlowMaster_BillTerm", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public com.Sconit.CodeMaster.OrderBillTerm BillTerm { get; set; }

        [Display(Name = "FlowMaster_CreateHuOption", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public com.Sconit.CodeMaster.CreateHuOption CreateHuOption { get; set; }

        [Display(Name = "FlowMaster_MaxOrderCount", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Int32 MaxOrderCount { get; set; }

        [Display(Name = "FlowMaster_MRPOption", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public com.Sconit.CodeMaster.MRPOption MRPOption { get; set; }

        public Int32 CreateUserId { get; set; }

        public string CreateUserName { get; set; }

        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        public string LastModifyUserName { get; set; }

        public DateTime LastModifyDate { get; set; }

        [Display(Name = "FlowMaster_IsPause", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsPause { get; set; }
        [Display(Name = "FlowMaster_PauseTime", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public DateTime? PauseTime { get; set; }
        [Display(Name = "FlowMaster_IsCheckPartyFromAuth", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsCheckPartyFromAuthority { get; set; }
        [Display(Name = "FlowMaster_IsCheckPartyToAuth", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsCheckPartyToAuthority { get; set; }
        [Display(Name = "FlowMaster_IsShipFifo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsShipFifo { get; set; }
        [Display(Name = "FlowMaster_ExtraDemandSource", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string ExtraDemandSource { get; set; }



        public com.Sconit.CodeMaster.FlowStrategy FlowStrategy { get; set; }

        [Display(Name = "FlowMaster_PickStrategy", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string PickStrategy { get; set; }

        [Display(Name = "FlowMaster_DAUAT", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string DAUAT { get; set; }

        public DateTime? LastRefreshDate { get; set; }

        //允许暂估价收货
        [Display(Name = "FlowMaster_IsAllowProvEstRec", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public Boolean IsAllowProvEstRec { get; set; }
        [Display(Name = "FlowMaster_UcDeviation", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public double UcDeviation { get; set; }
        [Display(Name = "FlowMaster_OrderDeviation", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public double OrderDeviation { get; set; }

        [Display(Name = "FlowMaster_ExtraLocationTo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string ExtraLocationTo { get; set; }
        [Display(Name = "FlowMaster_ExtraLocationFrom", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string ExtraLocationFrom { get; set; }

        /// <summary>
        /// 销售组织
        /// </summary>
        [Display(Name = "FlowMaster_SalesOrg", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string SalesOrg { get; set; }

        /// <summary>
        /// 分销渠道
        /// </summary>
        [Display(Name = "FlowMaster_DistrChan", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string DistrChan { get; set; }

        public string CostCenter { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            FlowMaster another = obj as FlowMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Code == another.Code);
            }
        }
    }

}
