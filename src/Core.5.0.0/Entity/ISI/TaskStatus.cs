using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class TaskStatus
    {
        #region Non O/R Mapping Properties

        [Display(Name = "TaskMaster_StatusDescription", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string StatusDescription { get; set; }

        [Display(Name = "TaskMaster_Type", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public com.Sconit.CodeMaster.TaskType? Type { get; set; }

        [Display(Name = "TaskMaster_Subject", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string Subject { get; set; }

        [Display(Name = "TaskMaster_Priority", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public CodeMaster.TaskPriority? Priority { get; set; }

        [Display(Name = "TaskMaster_TaskSubType", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string TaskSubType { get; set; }

        [Display(Name = "TaskMaster_TaskAddress", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string TaskAddress { get; set; }

        [Display(Name = "TaskMaster_Description1", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string Description1 { get; set; }

        [Display(Name = "TaskStatus_IsRemindCreateUser", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public bool IsRemindCreateUser { get; set; }

        [Display(Name = "TaskStatus_IsRemindAssignUser", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public bool IsRemindAssignUser { get; set; }

        [Display(Name = "TaskStatus_IsRemindStartUser", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public bool IsRemindStartUser { get; set; }

        [Display(Name = "TaskStatus_IsRemindCommentUser", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public bool IsRemindCommentUser { get; set; }

        private Boolean _isCurrentStatus;
        public Boolean IsCurrentStatus
        {
            get
            {
                return _isCurrentStatus;
            }
            set
            {
                _isCurrentStatus = value;
            }
        }

        #endregion
    }
}