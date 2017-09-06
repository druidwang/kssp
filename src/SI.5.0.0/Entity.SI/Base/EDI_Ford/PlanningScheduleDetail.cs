using System;

namespace com.Sconit.Entity.SI.EDI_Ford
{
    [Serializable]
    public partial class PlanningScheduleDetail : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public Int32 PlanningId { get; set; }
		public string ControlNum { get; set; }
		public string ReleaseNum { get; set; }
		public string Purpose { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public string ForecastType { get; set; }
		public string ScheduleQty { get; set; }
		public string ReleaseDate { get; set; }
		public string ShipTo { get; set; }
		public string ShipFrom { get; set; }
		public string Item { get; set; }
		public string Po { get; set; }
		public string Uom { get; set; }
		public string ContactName { get; set; }
		public string ContactTelephone { get; set; }
		public string DeliverPattern { get; set; }
		public string DeliveryPatternTime { get; set; }
		public string FI { get; set; }
		public string FIEndDate { get; set; }
		public string FICumQty { get; set; }
		public string FIStartDate { get; set; }
		public string MT { get; set; }
		public string MTEndDate { get; set; }
		public string MTCumQty { get; set; }
		public string MTStartDate { get; set; }
		public string LastIpQty { get; set; }
		public string LastIpDate { get; set; }
		public string IpCumQty { get; set; }
		public string IpEndDate { get; set; }
		public string LastIpNo { get; set; }
		public string ScheduleTiming { get; set; }
		public string ScheduleWhen { get; set; }
		public string Qty { get; set; }
		public string CumQty { get; set; }
        
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
            PlanningScheduleDetail another = obj as PlanningScheduleDetail;

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
