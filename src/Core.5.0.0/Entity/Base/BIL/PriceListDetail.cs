using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.BIL
{
    [Serializable]
    public partial class PriceListDetail : EntityBase,IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListDetail_Id", ResourceType = typeof(Resources.BIL.PriceListDetail))]
		public Int32 Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListDetail_PriceList", ResourceType = typeof(Resources.BIL.PriceListDetail))]
        public PriceListMaster PriceList { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListDetail_Item", ResourceType = typeof(Resources.BIL.PriceListDetail))]
		public string Item { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListDetail_Uom", ResourceType = typeof(Resources.BIL.PriceListDetail))]
		public string Uom { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListDetail_StartDate", ResourceType = typeof(Resources.BIL.PriceListDetail))]
		public DateTime StartDate { get; set; }
        [Display(Name = "PriceListDetail_EndDate", ResourceType = typeof(Resources.BIL.PriceListDetail))]
		public DateTime? EndDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListDetail_UnitPrice", ResourceType = typeof(Resources.BIL.PriceListDetail))]
		public Decimal UnitPrice { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PriceListDetail_IsProvEst", ResourceType = typeof(Resources.BIL.PriceListDetail))]
        public Boolean IsProvisionalEstimate { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
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
            PriceListDetail another = obj as PriceListDetail;

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
