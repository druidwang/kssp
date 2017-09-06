using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.BIL
{
    [Serializable]
    public partial class PriceListMaster : EntityBase,IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListMaster_Code", ResourceType = typeof(Resources.BIL.PriceListMaster))]
		public string Code { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListMaster_Type", ResourceType = typeof(Resources.BIL.PriceListMaster))]
        public com.Sconit.CodeMaster.PriceListType Type { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListMaster_Party", ResourceType = typeof(Resources.BIL.PriceListMaster))]
		public string Party { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListMaster_Currency", ResourceType = typeof(Resources.BIL.PriceListMaster))]
		public string Currency { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListMaster_IsIncludeTax", ResourceType = typeof(Resources.BIL.PriceListMaster))]
		public Boolean IsIncludeTax { get; set; }
        [Display(Name = "PriceListMaster_Tax", ResourceType = typeof(Resources.BIL.PriceListMaster))]
		public string Tax { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListMaster_IsActive", ResourceType = typeof(Resources.BIL.PriceListMaster))]
		public Boolean IsActive { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }

        [Display(Name = "PriceListMaster_InterfacePriceType", ResourceType = typeof(Resources.BIL.PriceListMaster))]
        public com.Sconit.CodeMaster.InterfacePriceType InterfacePriceType { get; set; }

        
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
            PriceListMaster another = obj as PriceListMaster;

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
