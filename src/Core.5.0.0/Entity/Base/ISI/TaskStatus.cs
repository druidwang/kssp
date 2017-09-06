using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class TaskStatus : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "TaskStatus_CreateUserName", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public string CreateUserName { get; set; }
        [Display(Name = "TaskStatus_CreateDate", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "TaskStatus_LastModifyUserName", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "TaskStatus_LastModifyDate", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public DateTime LastModifyDate { get; set; }

        [Display(Name = "TaskStatus_StartDate", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public DateTime StartDate { get; set; }

        [Display(Name = "TaskStatus_EndDate", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public DateTime EndDate { get; set; }

        [Display(Name = "TaskStatus_TaskCode", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public string TaskCode { get; set; }

        [Display(Name = "TaskStatus_Description", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public string Description { get; set; }

        [Display(Name = "TaskStatus_Flag", ResourceType = typeof(Resources.ISI.TaskStatus))]
        public string Flag { get; set; }

        public string Color { get; set; }

        public Int32 Version { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Id != null)
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
            TaskStatus another = obj as TaskStatus;

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
