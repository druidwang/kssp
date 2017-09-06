
namespace com.Sconit.Entity.PRD
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using com.Sconit.Entity.SYS;

    [Serializable]
    public partial class BomMaster : EntityBase,IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "BomMaster_Code", ResourceType = typeof(Resources.PRD.Bom))]
        public string Code { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 20)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "BomMaster_Desc", ResourceType = typeof(Resources.PRD.Bom))]
        public string Description { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 40)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "BomMaster_Uom", ResourceType = typeof(Resources.PRD.Bom))]
        public string Uom { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 30)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "BomMaster_Qty", ResourceType = typeof(Resources.PRD.Bom))]
        public decimal Qty { get; set; }
        [Export(ExportName = "BomDet", ExportSeq = 50)]
        [Display(Name = "BomMaster_IsActive", ResourceType = typeof(Resources.PRD.Bom))]
        public Boolean IsActive { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        
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
            BomMaster another = obj as BomMaster;

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
