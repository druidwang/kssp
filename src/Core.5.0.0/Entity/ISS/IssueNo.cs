using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISS
{
    public partial class IssueNo
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        private string _issueTypeCode;
        [Display(Name = "IssueType", ResourceType = typeof(Resources.ISS.IssueNo))]
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
        [Display(Name = "IssueTypeDescription", ResourceType = typeof(Resources.ISS.IssueNo))]
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
        #endregion
    }
}