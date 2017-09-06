using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class UnpickShipPlan : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public Int32? ShipPlanId { get; set; }
		public string Flow { get; set; }
		public string OrderNo { get; set; }
		public Int32 OrderSeq { get; set; }
		public Int32 OrderDetId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? WindowTime { get; set; }
		public string Item { get; set; }
		public string ItemDesc { get; set; }
		public string RefItemCode { get; set; }
		public string Uom { get; set; }
		public string BaseUom { get; set; }
		public Decimal UnitQty { get; set; }
		public Decimal UnitCount { get; set; }
		public string UCDesc { get; set; }
        public Decimal UnpickQty { get; set; }
		public Int16 Priority { get; set; }
		public string LocFrom { get; set; }
		public string LocFromName { get; set; }
		public string LocTo { get; set; }
		public string LocToName { get; set; }
		public string Station { get; set; }
		public string Dock { get; set; }
		public Boolean IsActive { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
		public Int32? CloseUser { get; set; }
		public string CloseUserName { get; set; }
		public DateTime? CloseDate { get; set; }
		public Int32 Version { get; set; }
		public Int16? OrderType { get; set; }
        
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
            UnpickShipPlan another = obj as UnpickShipPlan;

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
