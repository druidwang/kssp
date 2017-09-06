using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportPriceListDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }

        [Display(Name = "TransportPriceListDetail_PriceList", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public string PriceList { get; set; }

        [Display(Name = "TransportPriceListDetail_ShipFrom", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public string ShipFrom { get; set; }

        [Display(Name = "TransportPriceListDetail_ShipFromDescription", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public string ShipFromDescription { get; set; }

        [Display(Name = "TransportPriceListDetail_ShipTo", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public string ShipTo { get; set; }

        [Display(Name = "TransportPriceListDetail_ShipToDescription", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public string ShipToDescription { get; set; }
        [Display(Name = "TransportPriceListDetail_PricingMethod", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public com.Sconit.CodeMaster.TransportPricingMethod PricingMethod { get; set; }
        [Display(Name = "TransportPriceListDetail_StartDate", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public DateTime StartDate { get; set; }
        [Display(Name = "TransportPriceListDetail_EndDate", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public DateTime? EndDate { get; set; }
        [Display(Name = "TransportPriceListDetail_Currency", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public string Currency { get; set; }
        [Display(Name = "TransportPriceListDetail_UnitPrice", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public Decimal UnitPrice { get; set; }
        [Display(Name = "TransportPriceListDetail_IsProvEst", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public Boolean IsProvEst { get; set; }
        [Display(Name = "TransportPriceListDetail_Tonnage", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public string Tonnage { get; set; }
        [Display(Name = "TransportPriceListDetail_MinPrice", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public Decimal? MinPrice { get; set; }
        [Display(Name = "TransportPriceListDetail_MaxPrice", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public Decimal? MaxPrice { get; set; }
        [Display(Name = "TransportPriceListDetail_DeliveryFee", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public Decimal? DeliveryFee { get; set; }
        [Display(Name = "TransportPriceListDetail_LoadingFee", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public Decimal? LoadingFee { get; set; }
        [Display(Name = "TransportPriceListDetail_ServiceFee", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public Decimal? ServiceFee { get; set; }
        [Display(Name = "TransportPriceListDetail_StartQty", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public Decimal? StartQty { get; set; }
        [Display(Name = "TransportPriceListDetail_EndQty", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public Decimal? EndQty { get; set; }

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


        public decimal MinVolume { get; set; }

        public decimal MinWeight { get; set; }
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
            TransportPriceListDetail another = obj as TransportPriceListDetail;

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
