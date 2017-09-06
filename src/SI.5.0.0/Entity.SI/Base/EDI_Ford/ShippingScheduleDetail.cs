using System;

namespace com.Sconit.Entity.SI.EDI_Ford
{
    [Serializable]
    public partial class ShippingScheduleDetail : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public Int32? ShippingId { get; set; }
		public string ControlNum { get; set; }
		public string ReleaseNum { get; set; }
		public string ReleaseDate { get; set; }
		public string Purpose { get; set; }
		public string ForecastType { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public string ReferenceNum { get; set; }
		public string ShipTo { get; set; }
		public string ShipFrom { get; set; }
		public string DeliverPattern { get; set; }
		public string Item { get; set; }
		public string Po { get; set; }
		public string Dock { get; set; }
		public string LineFeed { get; set; }
		public string ReserveLineFeed { get; set; }
		public string ContactName { get; set; }
		public string ContactTelephone { get; set; }
		public string LastIpNo { get; set; }
		public string LastIpQty { get; set; }
		public string LastIpDate { get; set; }
		public string CumIpQty { get; set; }
		public string CumStartDate { get; set; }
		public string CumEndDate { get; set; }
		public string CumQty { get; set; }
		public string Qty { get; set; }
		public string Uom { get; set; }
		public string ForecastStatus { get; set; }
		public string ForecastDate { get; set; }
		public string ForecastTime { get; set; }
        
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
            ShippingScheduleDetail another = obj as ShippingScheduleDetail;

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
