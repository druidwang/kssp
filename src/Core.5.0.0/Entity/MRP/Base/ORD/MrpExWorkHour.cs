using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class MrpExWorkHour : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExWorkHour_Flow", ResourceType = typeof(Resources.MRP.MrpExWorkHour))]
        public string Flow { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExWorkHour_Item", ResourceType = typeof(Resources.MRP.MrpExWorkHour))]
        public string Item { get; set; }
        [Display(Name = "MrpExWorkHour_ItemDescription", ResourceType = typeof(Resources.MRP.MrpExWorkHour))]
        public string ItemDescription { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExWorkHour_StartTime", ResourceType = typeof(Resources.MRP.MrpExWorkHour))]
        public DateTime StartTime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExWorkHour_WindowTime", ResourceType = typeof(Resources.MRP.MrpExWorkHour))]
        public DateTime WindowTime { get; set; }
        [Display(Name = "MrpExWorkHour_HaltTime", ResourceType = typeof(Resources.MRP.MrpExWorkHour))]
        public double HaltTime { get; set; }

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
            MrpExWorkHour another = obj as MrpExWorkHour;

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
