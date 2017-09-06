using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class TaskWeekStatisticsView : EntityBase
    {
        #region O/R Mapping Properties

        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private string _dept;
        public string Dept
        {
            get
            {
                return _dept;
            }
            set
            {
                _dept = value;
            }
        }
        private string _dept2;
        public string Dept2
        {
            get
            {
                return _dept2;
            }
            set
            {
                _dept2 = value;
            }
        }
        private string _jobNo;
        public string JobNo
        {
            get
            {
                return _jobNo;
            }
            set
            {
                _jobNo = value;
            }
        }
        private Int32 _createCount;
        public Int32 CreateCount
        {
            get
            {
                return _createCount;
            }
            set
            {
                _createCount = value;
            }
        }
        private Int32 _submitCount;
        public Int32 SubmitCount
        {
            get
            {
                return _submitCount;
            }
            set
            {
                _submitCount = value;
            }
        }
        public Int32 SubmitFirstCount { get; set; }
        private Int32 _cancelCount;
        public Int32 CancelCount
        {
            get
            {
                return _cancelCount;
            }
            set
            {
                _cancelCount = value;
            }
        }
        private Int32 _firstCount;
        public Int32 FirstCount
        {
            get
            {
                return _firstCount;
            }
            set
            {
                _firstCount = value;
            }
        }
        private Int32 _assignCount;
        public Int32 AssignCount
        {
            get
            {
                return _assignCount;
            }
            set
            {
                _assignCount = value;
            }
        }
        private Int32 _statusCount;
        public Int32 StatusCount
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
        private Int32 _fileCount;
        public Int32 FileCount
        {
            get
            {
                return _fileCount;
            }
            set
            {
                _fileCount = value;
            }
        }
        private Int32 _commentCount;
        public Int32 CommentCount
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
        private Int32 _closeCount;
        public Int32 CloseCount
        {
            get
            {
                return _closeCount;
            }
            set
            {
                _closeCount = value;
            }
        }
        private Int32 _openCount;
        public Int32 OpenCount
        {
            get
            {
                return _openCount;
            }
            set
            {
                _openCount = value;
            }
        }
        public bool IsActive { get; set; }
        public string Position { get; set; }
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
            TaskWeekStatisticsView another = obj as TaskWeekStatisticsView;

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
