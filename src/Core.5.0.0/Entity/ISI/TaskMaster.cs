using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class TaskMaster 
    {
        #region Non O/R Mapping Properties


        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TaskStatus, ValueField = "Status")]
        [Display(Name = "TaskMaster_StatusDescription", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string StatusDescription { get; set; }


        public bool IsCompleteNoRemind { get; set; }
        public bool IsAutoRelease { get; set; }

        public bool IsNoSend { get; set; }

        public string HelpContent { get; set; }

        public CommentDetail CommentDetail { get; set; }

        public TaskStatus TaskStatus { get; set; }

        public String TaskSubTypeCode { get; set; }

        public String TaskSubTypeDesc { get; set; }

        public String TaskSubTypeAssignUser { get; set; }

        public String FailureModeCode { get; set; }

        public String FailureModeDesc { get; set; }

        public decimal? StartPercent { get; set; }

        public string StartedUser
        {
            get
            {
                if (string.IsNullOrEmpty(AssignStartUser))
                {
                    return SchedulingStartUser;
                }
                else
                {
                    return AssignStartUser;
                }
            }
        }

        #endregion
    }
}