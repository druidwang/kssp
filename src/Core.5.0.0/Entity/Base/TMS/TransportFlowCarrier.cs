using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportFlowCarrier : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }

        [Display(Name = "TransportFlowMaster_Code", ResourceType = typeof(Resources.TMS.TransportFlow))]
		public string Flow { get; set; }

        [Display(Name = "TransportFlow_Sequence", ResourceType = typeof(Resources.TMS.TransportFlow))]
		public Int32 Sequence { get; set; }

        public com.Sconit.CodeMaster.TransportMode TransportMode { get; set; }

        [Display(Name = "TransportFlowCarrier_Carrier", ResourceType = typeof(Resources.TMS.TransportFlow))]
		public string Carrier { get; set; }

        [Display(Name = "TransportFlowCarrier_CarrierName", ResourceType = typeof(Resources.TMS.TransportFlow))]
        public string CarrierName { get; set; }

        [Display(Name = "TransportFlowCarrier_PriceList", ResourceType = typeof(Resources.TMS.TransportFlow))]
        public string PriceList { get; set; }

        [Display(Name = "TransportFlowCarrier_BillAddress", ResourceType = typeof(Resources.TMS.TransportFlow))]
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
            TransportFlowCarrier another = obj as TransportFlowCarrier;

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
