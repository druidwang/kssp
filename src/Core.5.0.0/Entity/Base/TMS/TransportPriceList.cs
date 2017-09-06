using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportPriceList : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TransportPriceList_Code", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public string Code { get; set; }

        [Display(Name = "TransportPriceList_Description", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public string Description { get; set; }

        [Display(Name = "TransportPriceList_TransportMode", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public com.Sconit.CodeMaster.TransportMode TransportMode { get; set; }

        [Display(Name = "TransportPriceList_Carrier", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public string Carrier { get; set; }

        [Display(Name = "TransportPriceList_CarrierName", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public string CarrierName { get; set; }

        [Display(Name = "TransportPriceList_IsActive", ResourceType = typeof(Resources.TMS.TransportPriceList))]
		public Boolean IsActive { get; set; }

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

        public string Currency { get; set; }

        public string Tax { get; set; }

        public Boolean IsIncludeTax { get; set; }
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
            TransportPriceList another = obj as TransportPriceList;

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
