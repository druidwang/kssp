using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class TaskSubType : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TaskSubType_Code", ResourceType = typeof(Resources.ISI.TaskSubType))]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "TaskSubType_Description", ResourceType = typeof(Resources.ISI.TaskSubType))]
        public string Description { get; set; }

        [Display(Name = "TaskSubType_Type", ResourceType = typeof(Resources.ISI.TaskSubType))]
        public com.Sconit.CodeMaster.TaskType Type { get; set; }


        [Display(Name = "TaskSubType_IsActive", ResourceType = typeof(Resources.ISI.TaskSubType))]
        public Boolean IsActive { get; set; }

        [Display(Name = "TaskSubType_IsAutoStart", ResourceType = typeof(Resources.ISI.TaskSubType))]
        public Boolean IsAutoStart { get; set; }

        [Display(Name = "TaskSubType_IsAutoComplete", ResourceType = typeof(Resources.ISI.TaskSubType))]
        public Boolean IsAutoComplete { get; set; }

        [Display(Name = "TaskSubType_IsAutoAssign", ResourceType = typeof(Resources.ISI.TaskSubType))]
        public Boolean IsAutoAssign { get; set; }

        [Display(Name = "TaskSubType_IsAutoClose", ResourceType = typeof(Resources.ISI.TaskSubType))]
        public Boolean IsAutoClose { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "TaskAddress_CreateUserName", ResourceType = typeof(Resources.ISI.TaskAddress))]
        public string CreateUserName { get; set; }
        [Display(Name = "TaskAddress_CreateDate", ResourceType = typeof(Resources.ISI.TaskAddress))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        public string LastModifyUserName { get; set; }

        public DateTime LastModifyDate { get; set; }

        public string RegisterNo { get; set; }
        public string ExtNo { get; set; }
        public decimal? Amount { get; set; }
        public string ECType { get; set; }
        public bool IsEC { get; set; }
        public bool IsWF { get; set; }
        public string ECUser { get; set; }
        public Int32 Version { get; set; }

        private Int32 _seq;
        public Int32 Seq
        {
            get
            {
                return _seq;
            }
            set
            {
                _seq = value;
            }
        }

        public Boolean IsPublic { get; set; }
        public Boolean IsReport { get; set; }
        public string ViewUser { get; set; }

        public Boolean IsOpen { get; set; }
        public Decimal? OpenTime { get; set; }


        public Boolean IsAutoStatus { get; set; }

        private string _assignUser;
        public string AssignUser
        {
            get
            {
                return _assignUser;
            }
            set
            {
                _assignUser = value;
            }
        }
        private string _startUser;
        public string StartUser
        {
            get
            {
                return _startUser;
            }
            set
            {
                _startUser = value;
            }
        }
        private Boolean _isassignUp;
        public Boolean IsAssignUp
        {
            get
            {
                return _isassignUp;
            }
            set
            {
                _isassignUp = value;
            }
        }
        private Decimal? _assignUpTime;
        public Decimal? AssignUpTime
        {
            get
            {
                return _assignUpTime;
            }
            set
            {
                _assignUpTime = value;
            }
        }
        private string _assignUpUser;
        public string AssignUpUser
        {
            get
            {
                return _assignUpUser;
            }
            set
            {
                _assignUpUser = value;
            }
        }
        private Boolean _isStartUp;
        public Boolean IsStartUp
        {
            get
            {
                return _isStartUp;
            }
            set
            {
                _isStartUp = value;
            }
        }
        private Decimal? _startUpTime;
        public Decimal? StartUpTime
        {
            get
            {
                return _startUpTime;
            }
            set
            {
                _startUpTime = value;
            }
        }
        private string _startUpUser;
        public string StartUpUser
        {
            get
            {
                return _startUpUser;
            }
            set
            {
                _startUpUser = value;
            }
        }
        private Boolean _isCloseUp;
        public Boolean IsCloseUp
        {
            get
            {
                return _isCloseUp;
            }
            set
            {
                _isCloseUp = value;
            }
        }
        private Decimal? _closeUpTime;
        public Decimal? CloseUpTime
        {
            get
            {
                return _closeUpTime;
            }
            set
            {
                _closeUpTime = value;
            }
        }
        private string _closeUpUser;
        public string CloseUpUser
        {
            get
            {
                return _closeUpUser;
            }
            set
            {
                _closeUpUser = value;
            }
        }

        public bool IsCompleteUp { get; set; }
        public decimal? CompleteUpTime { get; set; }

        public bool IsStart { get; set; }
        public decimal? StartPercent { get; set; }
        public string ProjectType { get; set; }
        public bool IsQuote { get; set; }
        public bool IsInitiation { get; set; }
        public string Organization { get; set; }

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
            TaskSubType another = obj as TaskSubType;

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
