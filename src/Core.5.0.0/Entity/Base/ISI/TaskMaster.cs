using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class TaskMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "TaskMaster_Code", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string Code { get; set; }

        [Display(Name = "TaskMaster_Description1", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string Description1 { get; set; }

        [Display(Name = "TaskMaster_Description2", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string Description2 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TaskMaster_Status", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public com.Sconit.CodeMaster.TaskStatus Status { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TaskMaster_TaskAddress", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string TaskAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TaskMaster_Type", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public com.Sconit.CodeMaster.TaskType Type { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TaskMaster_Subject", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string Subject { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TaskMaster_Priority", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public CodeMaster.TaskPriority Priority { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TaskMaster_TaskSubType", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string TaskSubType { get; set; }

        [Display(Name = "TaskMaster_AssignUpUser", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string AssignUpUser { get; set; }

        [Display(Name = "TaskMaster_Flag", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string Flag { get; set; }


        [Display(Name = "TaskMaster_Color", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string Color { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "TaskMaster_CreateUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string CreateUserName { get; set; }
        [Display(Name = "TaskMaster_CreateDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "TaskMaster_LastModifyUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "TaskMaster_LastModifyDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime LastModifyDate { get; set; }

        public Int32 SubmitUserId { get; set; }
        [Display(Name = "TaskMaster_SubmitUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string SubmitUserName { get; set; }
        [Display(Name = "TaskMaster_SubmitDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? SubmitDate { get; set; }

        public Int32 CancelUserId { get; set; }
        [Display(Name = "TaskMaster_CancelUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string CancelUserName { get; set; }
        [Display(Name = "TaskMaster_CancelDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? CancelDate { get; set; }

        public Int32 RejectUserId { get; set; }
        [Display(Name = "TaskMaster_RejectUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string RejectUserName { get; set; }
        [Display(Name = "TaskMaster_RejectDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? RejectDate { get; set; }

        public Int32 AssignUserId { get; set; }
        [Display(Name = "TaskMaster_AssignUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string AssignUserName { get; set; }
        [Display(Name = "TaskMaster_AssignDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? AssignDate { get; set; }

        public Int32 StartUserId { get; set; }
        [Display(Name = "TaskMaster_StartUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string StartUserName { get; set; }
        [Display(Name = "TaskMaster_StartDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? StartDate { get; set; }

        public Int32 CompleteUserId { get; set; }
        [Display(Name = "TaskMaster_CompleteUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string CompleteUserName { get; set; }
        [Display(Name = "TaskMaster_CompleteDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? CompleteDate { get; set; }


        public Int32 CloseUserId { get; set; }
        [Display(Name = "TaskMaster_CloseUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string CloseUserName { get; set; }
        [Display(Name = "TaskMaster_CloseDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? CloseDate { get; set; }

        public Int32 OpenUserId { get; set; }
        [Display(Name = "TaskMaster_OpenUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string OpenUserName { get; set; }
        [Display(Name = "TaskMaster_OpenDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? OpenDate { get; set; }

        [Display(Name = "TaskMaster_PlanStartDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? PlanStartDate { get; set; }

        [Display(Name = "TaskMaster_PlanCompleteDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime? PlanCompleteDate { get; set; }

        [Display(Name = "TaskMaster_TraceCode", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string TraceCode { get; set; }

        [Display(Name = "TaskMaster_ExpectedResults", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string ExpectedResults { get; set; }

        [Display(Name = "TaskMaster_AssignStartUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string AssignStartUserName { get; set; }

        [Display(Name = "TaskMaster_AssignStartUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string AssignStartUser { get; set; }

        public bool IsAutoStart { get; set; }

        public bool IsAutoComplete { get; set; }

        public bool IsAutoClose { get; set; }

        public bool IsAutoAssign { get; set; }

        public bool IsAutoStatus { get; set; }
        /// <summary>
        /// ¹Ø×¢ÈË
        /// </summary>
        public string FocusUser { get; set; }

        private string FailureMode { get; set; }

        private string _backYards;
        public string BackYards
        {
            get
            {
                return _backYards;
            }
            set
            {
                _backYards = value;
            }
        }
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }
        private string _mobilePhone;
        public string MobilePhone
        {
            get
            {
                return _mobilePhone;
            }
            set
            {
                _mobilePhone = value;
            }
        }


        private string _wiki;
        public string Wiki
        {
            get
            {
                return _wiki;
            }
            set
            {
                _wiki = value;
            }
        }

        private Int32? _scheduling;
        public Int32? Scheduling
        {
            get
            {
                return _scheduling;
            }
            set
            {
                _scheduling = value;
            }
        }
        private string _schedulingStartUser;
        public string SchedulingStartUser
        {
            get
            {
                return _schedulingStartUser;
            }
            set
            {
                _schedulingStartUser = value;
            }
        }
        private string _schedulingShift;
        public string SchedulingShift
        {
            get
            {
                return _schedulingShift;
            }
            set
            {
                _schedulingShift = value;
            }
        }
        private string _schedulingShiftTime;
        public string SchedulingShiftTime
        {
            get
            {
                return _schedulingShiftTime;
            }
            set
            {
                _schedulingShiftTime = value;
            }
        }


        public decimal? Amount { get; set; }
        public decimal? PlanAmount { get; set; }







        //public string PatrolTime { get; set; }
        //public DateTime? LastSendEmailTime { get; set; }
        public string Seq { get; set; }
        public string Phase { get; set; }
        public Int32 ProjectTask { get; set; }
        public Int32 Version { get; set; }




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
            TaskMaster another = obj as TaskMaster;

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
