using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SCM
{
    [Serializable]
    public partial class FlowShiftDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.ErrorMessage))]
        [Display(Name = "FlowShiftDetail_Flow", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public string Flow { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "FlowShiftDetail_CreateUserName", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public string CreateUserName { get; set; }
        [Display(Name = "FlowShiftDetail_CreateDate", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.ErrorMessage))]
        [Display(Name = "FlowShiftDetail_Shift", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public string Shift { get; set; }

        [Display(Name = "FlowShiftDetail_WindowTime", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public string WindowTime { get; set; }

        [Display(Name = "FlowShiftDetail_OrderLeadTime", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public decimal OrderLeadTime { get; set; }
        [Display(Name = "FlowShiftDetail_OrderEMLeadTime", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public decimal OrderEMLeadTime { get; set; }
        [Display(Name = "FlowShiftDetail_UnloadLeadTime", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public decimal UnloadLeadTime { get; set; }
        [Display(Name = "FlowShiftDetail_SafeTime", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public decimal SafeTime { get; set; }

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
            FlowShiftDetail another = obj as FlowShiftDetail;

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
