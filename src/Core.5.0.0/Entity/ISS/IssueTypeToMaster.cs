using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISS
{
    public partial class IssueTypeToMaster
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 


        private string _issueTypeCode;
        [Display(Name = "IssueType", ResourceType = typeof(Resources.ISS.IssueTypeToMaster))]
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

        private string _issueLevelCode;
        [Display(Name = "IssueLevel", ResourceType = typeof(Resources.ISS.IssueTypeToMaster))]
        public string IssueLevelCode
        {
            get
            {
                if (string.IsNullOrEmpty(_issueLevelCode) && this.IssueLevel != null)
                {
                    _issueLevelCode = this.IssueLevel.Code;
                }
                return _issueLevelCode;

            }
            set
            {
                _issueLevelCode = value;
            }
        }
        #endregion
    }
}