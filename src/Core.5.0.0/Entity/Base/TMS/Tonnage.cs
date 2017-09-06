using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class Tonnage : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Tonnage_Code", ResourceType = typeof(Resources.TMS.Tonnage))]
		public string Code { get; set; }

        [Display(Name = "Tonnage_Description", ResourceType = typeof(Resources.TMS.Tonnage))]
        public string Description { get; set; }

        [Display(Name = "Tonnage_LoadVolume", ResourceType = typeof(Resources.TMS.Tonnage))]
		public Decimal LoadVolume { get; set; }

        [Display(Name = "Tonnage_LoadWeight", ResourceType = typeof(Resources.TMS.Tonnage))]
        public Decimal LoadWeight { get; set; }

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
            Tonnage another = obj as Tonnage;

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
