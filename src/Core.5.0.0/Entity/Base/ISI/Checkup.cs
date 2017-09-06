using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class Checkup : EntityBase
    {
        #region O/R Mapping Properties

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
        public string Department { get; set; }
        public string Dept2 { get; set; }
        public string JobNo { get; set; }
        private string _checkupUser;
        public string CheckupUser
        {
            get
            {
                return _checkupUser;
            }
            set
            {
                _checkupUser = value;
            }
        }
        private string _checkupUserNm;
        public string CheckupUserNm
        {
            get
            {
                return _checkupUserNm;
            }
            set
            {
                _checkupUserNm = value;
            }
        }
        private CheckupProject _checkupProject;
        public CheckupProject CheckupProject
        {
            get
            {
                return _checkupProject;
            }
            set
            {
                _checkupProject = value;
            }
        }
        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }
        private Decimal? _amount;
        public Decimal? Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }
        private string _approvalUser;
        public string ApprovalUser
        {
            get
            {
                return _approvalUser;
            }
            set
            {
                _approvalUser = value;
            }
        }
        private string _approvalUserNm;
        public string ApprovalUserNm
        {
            get
            {
                return _approvalUserNm;
            }
            set
            {
                _approvalUserNm = value;
            }
        }
        private string _auditInstructions;
        public string AuditInstructions
        {
            get
            {
                return _auditInstructions;
            }
            set
            {
                _auditInstructions = value;
            }
        }
        private DateTime _checkupDate;
        public DateTime CheckupDate
        {
            get
            {
                return _checkupDate;
            }
            set
            {
                _checkupDate = value;
            }
        }
        private DateTime? _approvalDate;
        public DateTime? ApprovalDate
        {
            get
            {
                return _approvalDate;
            }
            set
            {
                _approvalDate = value;
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
        private string _lastModifyUser;
        public string LastModifyUser
        {
            get
            {
                return _lastModifyUser;
            }
            set
            {
                _lastModifyUser = value;
            }
        }
        private string _lastModifyUserNm;
        public string LastModifyUserNm
        {
            get
            {
                return _lastModifyUserNm;
            }
            set
            {
                _lastModifyUserNm = value;
            }
        }
        private DateTime _lastModifyDate;
        public DateTime LastModifyDate
        {
            get
            {
                return _lastModifyDate;
            }
            set
            {
                _lastModifyDate = value;
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
        private string _cancelUser;
        public string CancelUser
        {
            get
            {
                return _cancelUser;
            }
            set
            {
                _cancelUser = value;
            }
        }
        private string _cancelUserNm;
        public string CancelUserNm
        {
            get
            {
                return _cancelUserNm;
            }
            set
            {
                _cancelUserNm = value;
            }
        }
        private DateTime? _cancelDate;
        public DateTime? CancelDate
        {
            get
            {
                return _cancelDate;
            }
            set
            {
                _cancelDate = value;
            }
        }
        private string _closeUser;
        public string CloseUser
        {
            get
            {
                return _closeUser;
            }
            set
            {
                _closeUser = value;
            }
        }
        private string _closeUserNm;
        public string CloseUserNm
        {
            get
            {
                return _closeUserNm;
            }
            set
            {
                _closeUserNm = value;
            }
        }
        private DateTime? _closeDate;
        public DateTime? CloseDate
        {
            get
            {
                return _closeDate;
            }
            set
            {
                _closeDate = value;
            }
        }
        public string Type { get; set; }

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
            Checkup another = obj as Checkup;

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
