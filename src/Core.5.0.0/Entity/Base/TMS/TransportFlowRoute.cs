using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportFlowRoute : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }

        [Display(Name = "TransportFlowMaster_Code", ResourceType = typeof(Resources.TMS.TransportFlow))]
		public string Flow { get; set; }

        [Display(Name = "TransportFlow_Sequence", ResourceType = typeof(Resources.TMS.TransportFlow))]
		public Int32 Sequence { get; set; }

        [Display(Name = "TransportFlowRoute_ShipAddress", ResourceType = typeof(Resources.TMS.TransportFlow))]
        public string ShipAddress { get; set; }

        [Display(Name = "TransportFlowRoute_ShipAddressDescription", ResourceType = typeof(Resources.TMS.TransportFlow))]
        public string ShipAddressDescription { get; set; }

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
            TransportFlowRoute another = obj as TransportFlowRoute;

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
