using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SCM
{
    [Serializable]
    public partial class FlowStrategy : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowStrategy_Flow", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public string Flow { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        //[StringLength(100, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowStrategy_Desc1", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public string Description { get; set; }
        [Display(Name = "FlowStrategy_Strategy", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public com.Sconit.CodeMaster.FlowStrategy Strategy { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        public Decimal LeadTime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        public Decimal EmergencyLeadTime { get; set; }
        public com.Sconit.CodeMaster.TimeUnit TimeUnit { get; set; }
        public string WindowTime1 { get; set; }
        public string WindowTime2 { get; set; }
        public string WindowTime3 { get; set; }
        public string WindowTime4 { get; set; }
        public string WindowTime5 { get; set; }
        public string WindowTime6 { get; set; }
        public string WindowTime7 { get; set; }
        public Int32 WeekInterval { get; set; }
        [Display(Name = "FlowStrategy_WaitTime", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public Decimal WaitTime { get; set; }
        [Display(Name = "FlowStrategy_WaitBatch", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public Int32 WaitBatch { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public DateTime? NextOrderTime { get; set; }
        public DateTime? NextWindowTime { get; set; }
        [Display(Name = "FlowStrategy_WindowTimeType", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public com.Sconit.CodeMaster.WindowTimeType WindowTimeType { get; set; }
        [Display(Name = "FlowStrategy_WinTimeDiff", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public Decimal WinTimeDiff { get; set; }

        [Display(Name = "FlowStrategy_RccpLeadTime", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public decimal RccpLeadTime { get; set; }

        //停机时间(每月)
        [Display(Name = "FlowStrategy_HaltTime", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public double HaltTime { get; set; }

        //试制时间(每月)
        [Display(Name = "FlowStrategy_TrialProduceTime", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public double TrialProduceTime { get; set; }

        [Display(Name = "FlowStrategy_IsCheckMrpDailyPlan", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public bool IsCheckMrpDailyPlan { get; set; }
        [Display(Name = "FlowStrategy_IsCheckMrpWeeklyPlan", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public bool IsCheckMrpWeeklyPlan { get; set; }
        [Display(Name = "FlowStrategy_IsCheckMrpMonthlyPlan", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public bool IsCheckMrpMonthlyPlan { get; set; }

        public Int32 MrpWeight { get; set; }

        public decimal MrpTotal { get; set; }

        public decimal MrpTotalAdjust { get; set; }


        [Display(Name = "FlowStrategy_SeqGroup", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public string SeqGroup { get; set; }

        [Display(Name = "FlowStrategy_SafeTime", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        public decimal? SafeTime { get; set; }

        public com.Sconit.CodeMaster.LeadTimeOption LeadTimeOption { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Flow != null)
            {
                return Flow.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            FlowStrategy another = obj as FlowStrategy;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Flow == another.Flow);
            }
        }
    }

}
