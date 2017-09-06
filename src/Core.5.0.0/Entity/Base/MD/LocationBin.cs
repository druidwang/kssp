using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class LocationBin : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Export(ExportName = "LocationBin", ExportSeq = 10)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "LocationBin_Code", ResourceType = typeof(Resources.MD.LocationBin))]
		public string Code { get; set; }
        [Export(ExportName = "LocationBin", ExportSeq = 20)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(100, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "LocationBin_Name", ResourceType = typeof(Resources.MD.LocationBin))]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "LocationArea_Code", ResourceType = typeof(Resources.MD.LocationArea))]
		public string Area { get; set; }
        [Display(Name = "Location_Code", ResourceType = typeof(Resources.MD.Location))]
		public string Location { get; set; }
        [Export(ExportName = "LocationBin", ExportSeq = 25)]
        [Display(Name = "LocationBin_Sequence", ResourceType = typeof(Resources.MD.LocationBin))]
		public Int32 Sequence { get; set; }
        [Export(ExportName = "LocationBin", ExportSeq = 30)]
        [Display(Name = "LocationBin_IsActive", ResourceType = typeof(Resources.MD.LocationBin))]
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
            LocationBin another = obj as LocationBin;

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
