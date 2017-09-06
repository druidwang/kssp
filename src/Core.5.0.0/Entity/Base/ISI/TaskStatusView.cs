using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class TaskStatusView : EntityBase
    {
        #region O/R Mapping Properties

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
        public Boolean IsAutoComplete { get; set; }
        public string FocusUser { get; set; }
        private DateTime _createDate;
        public DateTime CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                _createDate = value;
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
        private DateTime? _submitDate;
        public DateTime? SubmitDate
        {
            get
            {
                return _submitDate;
            }
            set
            {
                _submitDate = value;
            }
        }
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
        private string _assignUserNm;
        public string AssignUserNm
        {
            get
            {
                return _assignUserNm;
            }
            set
            {
                _assignUserNm = value;
            }
        }
        private string _assignStartUserNm;
        public string AssignStartUserNm
        {
            get
            {
                return _assignStartUserNm;
            }
            set
            {
                _assignStartUserNm = value;
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
        private string _assignStartUser;
        public string AssignStartUser
        {
            get
            {
                return _assignStartUser;
            }
            set
            {
                _assignStartUser = value;
            }
        }
        private string _startedUser;
        public string StartedUser
        {
            get
            {
                return _startedUser;
            }
            set
            {
                _startedUser = value;
            }
        }
        private DateTime? _assignDate;
        public DateTime? AssignDate
        {
            get
            {
                return _assignDate;
            }
            set
            {
                _assignDate = value;
            }
        }
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }
        private Int32? _id;
        public Int32? Id
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
        private string _statusDesc;
        public string StatusDesc
        {
            get
            {
                return _statusDesc;
            }
            set
            {
                _statusDesc = value;
            }
        }
        private DateTime? _statusStartDate;
        public DateTime? StatusStartDate
        {
            get
            {
                return _statusStartDate;
            }
            set
            {
                _statusStartDate = value;
            }
        }
        private DateTime? _statusEndDate;
        public DateTime? StatusEndDate
        {
            get
            {
                return _statusEndDate;
            }
            set
            {
                _statusEndDate = value;
            }
        }
        private string _statusUser;
        public string StatusUser
        {
            get
            {
                return _statusUser;
            }
            set
            {
                _statusUser = value;
            }
        }
        private string _statusUserNm;
        public string StatusUserNm
        {
            get
            {
                return _statusUserNm;
            }
            set
            {
                _statusUserNm = value;
            }
        }
        private DateTime? _statusDate;
        public DateTime? StatusDate
        {
            get
            {
                return _statusDate;
            }
            set
            {
                _statusDate = value;
            }
        }
        private Int32? _statusCount;
        public Int32? StatusCount
        {
            get
            {
                return _statusCount;
            }
            set
            {
                _statusCount = value;
            }
        }
        private Int32? _currentStatusCount;
        public Int32? CurrentStatusCount
        {
            get
            {
                return _currentStatusCount;
            }
            set
            {
                _currentStatusCount = value;
            }
        }
        private string _comment;
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }
        private string _commentCreateUserNm;
        public string CommentCreateUserNm
        {
            get
            {
                return _commentCreateUserNm;
            }
            set
            {
                _commentCreateUserNm = value;
            }
        }
        private string _commentCreateUser;
        public string CommentCreateUser
        {
            get
            {
                return _commentCreateUser;
            }
            set
            {
                _commentCreateUser = value;
            }
        }
        private Int32? _commentCount;
        public Int32? CommentCount
        {
            get
            {
                return _commentCount;
            }
            set
            {
                _commentCount = value;
            }
        }
        private Int32? _currentCommentCount;
        public Int32? CurrentCommentCount
        {
            get
            {
                return _currentCommentCount;
            }
            set
            {
                _currentCommentCount = value;
            }
        }
        private DateTime? _commentCreateDate;
        public DateTime? CommentCreateDate
        {
            get
            {
                return _commentCreateDate;
            }
            set
            {
                _commentCreateDate = value;
            }
        }
        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }
        private string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }
        private string _fileExtension;
        public string FileExtension
        {
            get
            {
                return _fileExtension;
            }
            set
            {
                _fileExtension = value;
            }
        }
        private string _contentType;
        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }
        private DateTime? _attachmentCreateDate;
        public DateTime? AttachmentCreateDate
        {
            get
            {
                return _attachmentCreateDate;
            }
            set
            {
                _attachmentCreateDate = value;
            }
        }
        private string _attachmentCreateUserNm;
        public string AttachmentCreateUserNm
        {
            get
            {
                return _attachmentCreateUserNm;
            }
            set
            {
                _attachmentCreateUserNm = value;
            }
        }
        private Int32? _attachmentCount;
        public Int32? AttachmentCount
        {
            get
            {
                return _attachmentCount;
            }
            set
            {
                _attachmentCount = value;
            }
        }
        private Int32? _currentAttachmentCount;
        public Int32? CurrentAttachmentCount
        {
            get
            {
                return _currentAttachmentCount;
            }
            set
            {
                _currentAttachmentCount = value;
            }
        }
        private Int32? _refTaskCount;
        public Int32? RefTaskCount
        {
            get
            {
                return _refTaskCount;
            }
            set
            {
                _refTaskCount = value;
            }
        }
        private string _taskAddress;
        public string TaskAddress
        {
            get
            {
                return _taskAddress;
            }
            set
            {
                _taskAddress = value;
            }
        }
        private string _seq;
        public string Seq
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
        private string _phase;
        public string Phase
        {
            get
            {
                return _phase;
            }
            set
            {
                _phase = value;
            }
        }
        private string _status;
        public string Status
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
        private string _desc1;
        public string Desc1
        {
            get
            {
                return _desc1;
            }
            set
            {
                _desc1 = value;
            }
        }
        private string _desc2;
        public string Desc2
        {
            get
            {
                return _desc2;
            }
            set
            {
                _desc2 = value;
            }
        }
        private string _taskSubTypeCode;
        public string TaskSubTypeCode
        {
            get
            {
                return _taskSubTypeCode;
            }
            set
            {
                _taskSubTypeCode = value;
            }
        }
        private string _taskSubTypeDesc;
        public string TaskSubTypeDesc
        {
            get
            {
                return _taskSubTypeDesc;
            }
            set
            {
                _taskSubTypeDesc = value;
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
        private string _priority;
        public string Priority
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
        private DateTime? _completeDate;
        public DateTime? CompleteDate
        {
            get
            {
                return _completeDate;
            }
            set
            {
                _completeDate = value;
            }
        }
        private string _createUser;
        public string CreateUser
        {
            get
            {
                return _createUser;
            }
            set
            {
                _createUser = value;
            }
        }
        private string _submitUser;
        public string SubmitUser
        {
            get
            {
                return _submitUser;
            }
            set
            {
                _submitUser = value;
            }
        }
        private string _createUserNm;
        public string CreateUserNm
        {
            get
            {
                return _createUserNm;
            }
            set
            {
                _createUserNm = value;
            }
        }
        private string _submitUserNm;
        public string SubmitUserNm
        {
            get
            {
                return _submitUserNm;
            }
            set
            {
                _submitUserNm = value;
            }
        }
        private string _taskSubTypeAssignUser;
        public string TaskSubTypeAssignUser
        {
            get
            {
                return _taskSubTypeAssignUser;
            }
            set
            {
                _taskSubTypeAssignUser = value;
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
        public Boolean IsReport { get; set; }
        private Boolean _isAutoAssign;
        public Boolean IsAutoAssign
        {
            get
            {
                return _isAutoAssign;
            }
            set
            {
                _isAutoAssign = value;
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
        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        private string _org;
        public string Org
        {
            get
            {
                return _org;
            }
            set
            {
                _org = value;
            }
        }
        private string _eCUser;
        public string ECUser
        {
            get
            {
                return _eCUser;
            }
            set
            {
                _eCUser = value;
            }
        }
        public string ViewUser { get; set; }
        private Decimal? _startPercent;
        public Decimal? StartPercent
        {
            get
            {
                return _startPercent;
            }
            set
            {
                _startPercent = value;
            }
        }

        #endregion


        public override int GetHashCode()
        {
            if (TaskCode != null)
            {
                return TaskCode.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            TaskStatusView another = obj as TaskStatusView;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.TaskCode == another.TaskCode);
            }
        }
    }

}
