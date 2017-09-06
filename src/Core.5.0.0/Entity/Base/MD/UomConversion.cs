using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class UomConversion : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Display(Name = "UomConvert_Item", ResourceType = typeof(Resources.MD.UomConvert))]
		public Item Item { get; set; }
        [Export(ExportName = "UomConversion", ExportSeq = 10)]
        [Display(Name = "UomConvert_BaseUom", ResourceType = typeof(Resources.MD.UomConvert))]
		public string BaseUom { get; set; }
        [Export(ExportName = "UomConversion", ExportSeq = 30)]
        [Display(Name = "UomConvert_AltUom", ResourceType = typeof(Resources.MD.UomConvert))]
		public string AlterUom { get; set; }
        [Display(Name = "UomConvert_BaseQty", ResourceType = typeof(Resources.MD.UomConvert))]
        [Export(ExportName = "UomConversion", ExportSeq = 20)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
		public Decimal BaseQty { get; set; }
        [Display(Name = "UomConvert_AltQty", ResourceType = typeof(Resources.MD.UomConvert))]
        [Export(ExportName = "UomConversion", ExportSeq = 40)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
		public Decimal AlterQty { get; set; }
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
            UomConversion another = obj as UomConversion;

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
