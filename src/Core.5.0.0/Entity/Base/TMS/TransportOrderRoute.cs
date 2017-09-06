using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportOrderRoute : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Display(Name = "TransportOrderMaster_OrderNo", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string OrderNo { get; set; }
        [Display(Name = "TransportOrderRoute_Sequence", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Int32 Sequence { get; set; }
        [Display(Name = "TransportOrderRoute_ShipAddress", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string ShipAddress { get; set; }
        [Display(Name = "TransportOrderRoute_ShipAddressDescription", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string ShipAddressDescription { get; set; }
        [Display(Name = "TransportOrderRoute_Distance", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? Distance { get; set; }
        [Display(Name = "TransportOrderRoute_EstDepartTime", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public DateTime? EstDepartTime { get; set; }
        [Display(Name = "TransportOrderRoute_EstArriveTime", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public DateTime? EstArriveTime { get; set; }
        [Display(Name = "TransportOrderRoute_DepartTime", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public DateTime? DepartTime { get; set; }
        [Display(Name = "TransportOrderRoute_DepartInputUser", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string DepartInputUser { get; set; }
        [Display(Name = "TransportOrderRoute_DepartInputUserName", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string DepartInputUserName { get; set; }
        [Display(Name = "TransportOrderRoute_ArriveTime", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public DateTime? ArriveTime { get; set; }
        [Display(Name = "TransportOrderRoute_ArriveInputUser", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string ArriveInputUser { get; set; }
        [Display(Name = "TransportOrderRoute_ArriveInputUserName", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public string ArriveInputUserName { get; set; }
        [Display(Name = "TransportOrderRoute_LoadRate", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? LoadRate { get; set; }
        [Display(Name = "TransportOrderRoute_WeightRate", ResourceType = typeof(Resources.TMS.TransportOrder))]
		public Decimal? WeightRate { get; set; }

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
        [Display(Name = "TransportOrderRoute_IsArrive", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public Boolean IsArrive { get; set; }
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
            TransportOrderRoute another = obj as TransportOrderRoute;

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
