using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISS
{
    public partial class IssueMaster
    {
        #region Non O/R Mapping Properties

        public static readonly string CODE_PREFIX_ISSUE = "ISS";
        public static readonly string Menu_Issue_Admin = "Menu_Issue_Admin";

        [Display(Name = "ReleaseIssue", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public Boolean ReleaseIssue { get; set; }

        [Display(Name = "ContinuousCreate", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public Boolean ContinuousCreate { get; set; }

        public Boolean IsRedirect { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.IssuePriority, ValueField = "Priority")]
        [Display(Name = "Priority", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string PriorityDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.IssueStatus, ValueField = "Status")]
        [Display(Name = "Status", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string StatusDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.IssueType, ValueField = "Type")]
        [Display(Name = "Type", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string TypeDescription { get; set; }


        //TODO: Add Non O/R Mapping Properties here. 
        private string _issueTypeCode;
        [Display(Name = "IssueType", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string IssueTypeCode
        {
            get
            {
                if (string.IsNullOrEmpty(_issueTypeCode) && this.IssueType != null)
                {
                    _issueTypeCode = this.IssueType.Code;
                }
                return _issueTypeCode;

            }
            set
            {
                _issueTypeCode = value;
            }
        }
        private string _issueTypeDescription;
        [Display(Name = "IssueTypeDescription", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string IssueTypeDescription
        {
            get
            {
                if (string.IsNullOrEmpty(_issueTypeDescription) && this.IssueType != null)
                {
                    _issueTypeDescription = this.IssueType.Description;
                }
                return _issueTypeDescription;

            }
            set
            {
                _issueTypeDescription = value;
            }
        }

        private string _issueNoCode;
        [Display(Name = "IssueNo", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string IssueNoCode
        {
            get
            {
                if (string.IsNullOrEmpty(_issueNoCode) && this.IssueNo != null)
                {
                    _issueNoCode = this.IssueNo.Code;
                }
                return _issueNoCode;

            }
            set
            {
                _issueNoCode = value;
            }
        }
        private string _issueNoDescription;
        [Display(Name = "IssueNoDescription", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string IssueNoDescription
        {
            get
            {
                if (string.IsNullOrEmpty(_issueNoDescription) && this.IssueNo != null)
                {
                    _issueNoDescription = this.IssueNo.Description;
                }
                return _issueNoDescription;

            }
            set
            {
                _issueNoDescription = value;
            }
        }
        /*
        private string _issueAddressCode;
        [Display(Name = "IssueAddress", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string IssueAddressCode
        {
            get
            {
                if (string.IsNullOrEmpty(_issueAddressCode) && this.IssueAddress != null)
                {
                    _issueAddressCode = this.IssueAddress.Code;
                }
                return _issueAddressCode;

            }
            set
            {
                _issueAddressCode = value;
            }
        }

        private string _issueAddressDescription;
        [Display(Name = "IssueAddressDescription", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string IssueAddressDescription
        {
            get
            {
                if (string.IsNullOrEmpty(_issueAddressDescription) && this.IssueAddress != null)
                {
                    _issueAddressDescription = this.IssueAddress.Description;
                }
                return _issueAddressDescription;

            }
            set
            {
                _issueAddressDescription = value;
            }
        }
        */
        private string _finishedUserCode;
        [Display(Name = "FinishedUser", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string FinishedUserCode
        {
            get
            {
                if (string.IsNullOrEmpty(_finishedUserCode) && this.FinishedUser != null)
                {
                    _finishedUserCode = this.FinishedUser.Code;
                }
                return _finishedUserCode;

            }
            set
            {
                _finishedUserCode = value;
            }
        }
        private string _finishedUserName;
        [Display(Name = "FinishedUser", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string FinishedUserName
        {
            get
            {
                if (string.IsNullOrEmpty(_finishedUserName) && this.FinishedUser != null)
                {
                    _finishedUserName = this.FinishedUser.FullName;
                }
                return _finishedUserName;

            }
            set
            {
                _finishedUserName = value;
            }
        }

       #endregion
    }
}