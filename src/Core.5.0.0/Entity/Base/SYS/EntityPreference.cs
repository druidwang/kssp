using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.SYS
{
    [Serializable]
    public partial class EntityPreference : EntityBase,IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "EntityPreference_Id", ResourceType = typeof(Resources.SYS.EntityPreference))]
		public Int32 Id { get; set; }
        [Display(Name = "EntityPreference_Seq", ResourceType = typeof(Resources.SYS.EntityPreference))]
		public Int32 Sequence { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "EntityPreference_Value", ResourceType = typeof(Resources.SYS.EntityPreference))]
		public string Value { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "EntityPreference_Desc", ResourceType = typeof(Resources.SYS.EntityPreference))]
		public string Description { get; set; }
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
            EntityPreference another = obj as EntityPreference;

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
