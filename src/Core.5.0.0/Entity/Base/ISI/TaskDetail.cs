using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class TaskDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties



        public Int32 CreateUserId { get; set; }
        [Display(Name = "TaskMaster_CreateUserName", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public string CreateUserName { get; set; }
        [Display(Name = "TaskMaster_CreateDate", ResourceType = typeof(Resources.ISI.TaskMaster))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        public string LastModifyUserName { get; set; }

        public DateTime LastModifyDate { get; set; }

        private Int32 _id;
        public Int32 Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        private string _taskCode;
        public string TaskCode
        {
            get
            {
                return _taskCode;
            }
            set
            {
                _taskCode = value;
            }
        }
        private string _taskSubType;
        public string TaskSubType
        {
            get
            {
                return _taskSubType;
            }
            set
            {
                _taskSubType = value;
            }
        }
        private string _subject;
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }
        private string _description1;
        public string Description1
        {
            get
            {
                return _description1;
            }
            set
            {
                _description1 = value;
            }
        }
        private string _description2;
        public string Description2
        {
            get
            {
                return _description2;
            }
            set
            {
                _description2 = value;
            }
        }
        private string _failureMode;
        public string FailureMode
        {
            get
            {
                return _failureMode;
            }
            set
            {
                _failureMode = value;
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
        private string _userEmail;
        public string UserEmail
        {
            get
            {
                return _userEmail;
            }
            set
            {
                _userEmail = value;
            }
        }
        private string _userMobilePhone;
        public string UserMobilePhone
        {
            get
            {
                return _userMobilePhone;
            }
            set
            {
                _userMobilePhone = value;
            }
        }
        private string _flag;
        public string Flag
        {
            get
            {
                return _flag;
            }
            set
            {
                _flag = value;
            }
        }
        private string _color;
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        private DateTime? _planStartDate;
        public DateTime? PlanStartDate
        {
            get
            {
                return _planStartDate;
            }
            set
            {
                _planStartDate = value;
            }
        }
        private DateTime? _planCompleteDate;
        public DateTime? PlanCompleteDate
        {
            get
            {
                return _planCompleteDate;
            }
            set
            {
                _planCompleteDate = value;
            }
        }
        private string _expectedResults;
        public string ExpectedResults
        {
            get
            {
                return _expectedResults;
            }
            set
            {
                _expectedResults = value;
            }
        }
        private CodeMaster.TaskStatus _status;
        public CodeMaster.TaskStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        private string _level;
        public string Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }
        private CodeMaster.TaskPriority _priority;
        public CodeMaster.TaskPriority Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }
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
        private Boolean _isSMS;
        public Boolean IsSMS
        {
            get
            {
                return _isSMS;
            }
            set
            {
                _isSMS = value;
            }
        }
        private Boolean _isEmail;
        public Boolean IsEmail
        {
            get
            {
                return _isEmail;
            }
            set
            {
                _isEmail = value;
            }
        }
        private string _receiver;
        public string Receiver
        {
            get
            {
                return _receiver;
            }
            set
            {
                _receiver = value;
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
        private Int32 _emailCount;
        public Int32 EmailCount
        {
            get
            {
                return _emailCount;
            }
            set
            {
                _emailCount = value;
            }
        }
        private string _emailStatus;
        public string EmailStatus
        {
            get
            {
                return _emailStatus;
            }
            set
            {
                _emailStatus = value;
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
        private string _sMSStatus;
        public string SMSStatus
        {
            get
            {
                return _sMSStatus;
            }
            set
            {
                _sMSStatus = value;
            }
        }
        private Int32 _sMSCount;
        public Int32 SMSCount
        {
            get
            {
                return _sMSCount;
            }
            set
            {
                _sMSCount = value;
            }
        }
        private Boolean _isActive;
        public Boolean IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }
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
            TaskDetail another = obj as TaskDetail;

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
