using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportOrderDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Display(Name = "TransportOrderMaster_OrderNo", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string OrderNo { get; set; }
        [Display(Name = "TransportOrderDetail_Sequence", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Int32 Sequence { get; set; }
        [Display(Name = "TransportOrderDetail_IpNo", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string IpNo { get; set; }
        [Display(Name = "TransportOrderDetail_OrderRouteFrom", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public Int32 OrderRouteFrom { get; set; }
        [Display(Name = "TransportOrderDetail_OrderRouteTo", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public Int32 OrderRouteTo { get; set; }
        [Display(Name = "TransportOrderDetail_EstPalletQty", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public Int32? EstPalletQty { get; set; }
        [Display(Name = "TransportOrderDetail_PalletQty", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Int32? PalletQty { get; set; }
        [Display(Name = "TransportOrderDetail_EstVolume", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? EstVolume { get; set; }
        [Display(Name = "TransportOrderDetail_Volume", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? Volume { get; set; }
        [Display(Name = "TransportOrderDetail_EstWeight", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? EstWeight { get; set; }
        [Display(Name = "TransportOrderDetail_Weight", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? Weight { get; set; }
        [Display(Name = "TransportOrderDetail_EstBoxCount", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Int32? EstBoxCount { get; set; }
        [Display(Name = "TransportOrderDetail_BoxCount", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Int32? BoxCount { get; set; }
        [Display(Name = "TransportOrderDetail_LoadTime", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public DateTime? LoadTime { get; set; }
        [Display(Name = "TransportOrderDetail_UnloadTime", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public DateTime? UnloadTime { get; set; }
        [Display(Name = "TransportOrderDetail_PartyFrom", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string PartyFrom { get; set; }
        [Display(Name = "TransportOrderDetail_PartyFromName", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string PartyFromName { get; set; }
        [Display(Name = "TransportOrderDetail_PartyTo", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string PartyTo { get; set; }
        [Display(Name = "TransportOrderDetail_PartyToName", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string PartyToName { get; set; }
        [Display(Name = "TransportOrderDetail_ShipFrom", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipFrom { get; set; }
        [Display(Name = "TransportOrderDetail_ShipFromAddress", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipFromAddress { get; set; }
        [Display(Name = "TransportOrderDetail_ShipFromTel", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipFromTel { get; set; }
        [Display(Name = "TransportOrderDetail_ShipFromCell", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipFromCell { get; set; }
        [Display(Name = "TransportOrderDetail_ShipFromFax", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipFromFax { get; set; }
        [Display(Name = "TransportOrderDetail_ShipFromContact", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipFromContact { get; set; }
        [Display(Name = "TransportOrderDetail_ShipTo", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipTo { get; set; }
        [Display(Name = "TransportOrderDetail_ShipToAddress", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipToAddress { get; set; }
        [Display(Name = "TransportOrderDetail_ShipToTel", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipToTel { get; set; }
        [Display(Name = "TransportOrderDetail_ShipToCell", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipToCell { get; set; }
        [Display(Name = "TransportOrderDetail_ShipToFax", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipToFax { get; set; }
        [Display(Name = "TransportOrderDetail_ShipToContact", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipToContact { get; set; }
        [Display(Name = "TransportOrderDetail_Dock", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string Dock { get; set; }
        [Display(Name = "TransportOrderDetail_Distance", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? Distance { get; set; }
        [Display(Name = "TransportOrderDetail_IsReceived", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Boolean IsReceived { get; set; }

        [Display(Name = "TransportOrderDetail_IsValuated", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public bool IsValuated { get; set; }

        [Display(Name = "TransportOrderDetail_PalletCode", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string PalletCode { get; set; }

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
		public Int32 Version { get; set; }
        
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
            TransportOrderDetail another = obj as TransportOrderDetail;

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
