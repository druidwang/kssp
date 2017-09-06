using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.PRD
{
    [Serializable]
    public partial class RoutingDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "RoutingDetail_Id", ResourceType = typeof(Resources.PRD.Routing))]
        public Int32 Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "RoutingDetail_Routing", ResourceType = typeof(Resources.PRD.Routing))]
        public string Routing { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "RoutingDetail_Op", ResourceType = typeof(Resources.PRD.Routing))]
        public Int32 Operation { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "RoutingDetail_OpRef", ResourceType = typeof(Resources.PRD.Routing))]
        public string OpReference { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "RoutingDetail_LeadTime", ResourceType = typeof(Resources.PRD.Routing))]
        public Double LeadTime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "RoutingDetail_TimeUnit", ResourceType = typeof(Resources.PRD.Routing))]
        public com.Sconit.CodeMaster.TimeUnit TimeUnit { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "RoutingDetail_StartDate", ResourceType = typeof(Resources.PRD.Routing))]
        public DateTime? StartDate { get; set; }
        [Display(Name = "RoutingDetail_EndDate", ResourceType = typeof(Resources.PRD.Routing))]
        public DateTime? EndDate { get; set; }
        [Display(Name = "RoutingDetail_Location", ResourceType = typeof(Resources.PRD.Routing))]
        public string Location { get; set; }
        [Display(Name = "RoutingDetail_WorkCenter", ResourceType = typeof(Resources.PRD.Routing))]
        public string WorkCenter { get; set; }
        [Display(Name = "RoutingDetail_IsReport", ResourceType = typeof(Resources.PRD.Routing))]
        public Boolean IsReport { get; set; }
        public CodeMaster.BackFlushMethod BackFlushMethod { get; set; }
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
            RoutingDetail another = obj as RoutingDetail;

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
