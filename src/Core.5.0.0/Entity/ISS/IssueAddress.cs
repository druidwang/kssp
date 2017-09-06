using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISS
{
    public partial class IssueAddress
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        private string _parentIssueAddressCode;
        [Display(Name = "ParentIssueAddress", ResourceType = typeof(Resources.ISS.IssueAddress))]
        public string ParentIssueAddressCode
        {
            get
            {
                if (string.IsNullOrEmpty(_parentIssueAddressCode) && this.ParentIssueAddress != null)
                {
                    _parentIssueAddressCode = this.ParentIssueAddress.Code;
                }
                return _parentIssueAddressCode;
            }
            set
            {
                _parentIssueAddressCode = value;
            }
        }

        private string _parentIssueAddressDescription;
        [Display(Name = "ParentIssueAddressDescription", ResourceType = typeof(Resources.ISS.IssueAddress))]
        public string ParentIssueAddressDescription
        {
            get
            {
                if (string.IsNullOrEmpty(_parentIssueAddressDescription) && this.ParentIssueAddress != null)
                {
                    _parentIssueAddressDescription = this.ParentIssueAddress.Description;
                }
                return _parentIssueAddressDescription;

            }
            set
            {
                _parentIssueAddressDescription = value;
            }
        }

        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Code) && string.IsNullOrWhiteSpace(this.Description))
                {
                    return string.Empty;
                }
                else if (string.IsNullOrWhiteSpace(this.Code))
                {
                    return this.Description;
                }
                else if (string.IsNullOrWhiteSpace(this.Description))
                {
                    return this.Code;
                }
                return this.Code + "[" + this.Description + "]";

            }
        }
        #endregion
    }
}