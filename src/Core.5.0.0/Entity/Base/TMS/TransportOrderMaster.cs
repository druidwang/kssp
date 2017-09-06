using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.CodeMaster;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportOrderMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Display(Name = "TransportOrderMaster_OrderNo", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string OrderNo { get; set; }
        [Display(Name = "TransportOrderMaster_ExternalOrderNo", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string ExternalOrderNo { get; set; }
        [Display(Name = "TransportOrderMaster_ReferenceOrderNo", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string ReferenceOrderNo { get; set; }
        [Display(Name = "TransportOrderMaster_Flow", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string Flow { get; set; }
        [Display(Name = "TransportOrderMaster_FlowDescription", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string FlowDescription { get; set; }
        [Display(Name = "TransportOrderMaster_Status", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public TransportStatus Status { get; set; }
        [Display(Name = "TransportOrderMaster_Carrier", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string Carrier { get; set; }
        [Display(Name = "TransportOrderMaster_CarrierName", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string CarrierName { get; set; }
        [Display(Name = "TransportOrderMaster_Vehicle", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string Vehicle { get; set; }
        [Display(Name = "TransportOrderMaster_Tonnage", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string Tonnage { get; set; }
        [Display(Name = "TransportOrderMaster_DrivingNo", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string DrivingNo { get; set; }
        [Display(Name = "TransportOrderMaster_Driver", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string Driver { get; set; }
        [Display(Name = "TransportOrderMaster_DriverMobilePhone", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string DriverMobilePhone { get; set; }
        [Display(Name = "TransportOrderMaster_LoadVolume", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? LoadVolume { get; set; }
        [Display(Name = "TransportOrderMaster_LoadWeight", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? LoadWeight { get; set; }
        [Display(Name = "TransportOrderMaster_MinLoadRate", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? MinLoadRate { get; set; }
        [Display(Name = "TransportOrderMaster_IsAutoRelease", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Boolean IsAutoRelease { get; set; }
        [Display(Name = "TransportOrderMaster_IsAutoStart", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Boolean IsAutoStart { get; set; }
        [Display(Name = "TransportOrderMaster_MultiSitePick", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public Boolean MultiSitePick { get; set; }
        [Display(Name = "TransportOrderMaster_ShipFrom", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipFrom { get; set; }
        [Display(Name = "TransportOrderMaster_ShipFromAddress", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string ShipFromAddress { get; set; }
        [Display(Name = "TransportOrderMaster_ShipTo", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string ShipTo { get; set; }
        [Display(Name = "TransportOrderMaster_ShipToAddress", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string ShipToAddress { get; set; }
        [Display(Name = "TransportOrderMaster_TransportMode", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public com.Sconit.CodeMaster.TransportMode TransportMode { get; set; }
        [Display(Name = "TransportOrderMaster_PriceList", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string PriceList { get; set; }
        [Display(Name = "TransportOrderMaster_BillAddress", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string BillAddress { get; set; }
        [Display(Name = "Common_CreateUserName", ResourceType = typeof(Resources.SYS.Global))]
        public string CreateUserName { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "Common_CreateDate", ResourceType = typeof(Resources.SYS.Global))]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Common_LastModifyUserName", ResourceType = typeof(Resources.SYS.Global))]
        public string LastModifyUserName { get; set; }

        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "Common_LastModifyDate", ResourceType = typeof(Resources.SYS.Global))]
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "Common_SubmitDate", ResourceType = typeof(Resources.SYS.Global))]
		public DateTime? SubmitDate { get; set; }

        public Int32 SubmitUserId { get; set; }
        [Display(Name = "Common_SubmitUserName", ResourceType = typeof(Resources.SYS.Global))]
		public string SubmitUserName { get; set; }
        [Display(Name = "Common_StartDate", ResourceType = typeof(Resources.SYS.Global))]
		public DateTime? StartDate { get; set; }

        public Int32 StartUserId { get; set; }
        [Display(Name = "Common_StartUserName", ResourceType = typeof(Resources.SYS.Global))]
		public string StartUserName { get; set; }
        [Display(Name = "Common_CloseDate", ResourceType = typeof(Resources.SYS.Global))]
		public DateTime? CloseDate { get; set; }
        [Display(Name = "Common_CloseUserName", ResourceType = typeof(Resources.SYS.Global))]
		public string CloseUserName { get; set; }

        public Int32 CloseUserId { get; set; }
        [Display(Name = "Common_CancelDate", ResourceType = typeof(Resources.SYS.Global))]
		public DateTime? CancelDate { get; set; }

        public Int32 CancelUserId { get; set; }
        [Display(Name = "Common_CancelUserName", ResourceType = typeof(Resources.SYS.Global))]
		public string CancelUserName { get; set; }

		public Int32? Version { get; set; }
        [Display(Name = "TransportOrderMaster_LicenseNo", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string LicenseNo { get; set; }
        public Int32? CurrentArriveSiteId { get; set; }
        public string CurrentArriveShipAddress { get; set; }
        public string CurrentArriveShipAddressDescription { get; set; }

        public bool IsValuated { get; set; }

        public string Expense { get; set; }

        [Display(Name = "TransportOrderMaster_PricingMethod", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public TransportPricingMethod PricingMethod { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (OrderNo != null)
            {
                return OrderNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            TransportOrderMaster another = obj as TransportOrderMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.OrderNo == another.OrderNo);
            }
        } 
    }
	
}
