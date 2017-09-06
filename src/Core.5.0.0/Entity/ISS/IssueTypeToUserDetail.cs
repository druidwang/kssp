using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISS
{
    public partial class IssueTypeToUserDetail
    {
        #region Non O/R Mapping Properties

        private string _userCode;
        [Display(Name = "UserCode", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
        public string UserCode
        {
            get
            {
                if (string.IsNullOrEmpty(_userCode) && this.User != null)
                {
                    _userCode = this.User.Code;
                }
                return _userCode;

            }
            set
            {
                _userCode = value;
            }
        }
        
        private string _userName;
        [Display(Name = "UserName", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(_userName) && this.User != null)
                {
                    _userName = this.User.FullName;
                }
                return _userName;

            }
            set
            {
                _userName = value;
            }
        }
        private string _mobilePhone;
        [Display(Name = "MobilePhone", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
        public string MobilePhone
        {
            get
            {
                if (string.IsNullOrEmpty(_mobilePhone) && this.User != null)
                {
                    _mobilePhone = this.User.MobilePhone;
                }
                return _mobilePhone;

            }
            set
            {
                _mobilePhone = value;
            }
        }
        private string _email;
        [Display(Name = "Email", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
        public string Email
        {
            get
            {
                if (string.IsNullOrEmpty(_email) && this.User != null)
                {
                    _email = this.User.Email;
                }
                return _email;

            }
            set
            {
                _email = value;
            }
        }

        public bool HasEmail
        {
            get
            {
                if (this.User!=null)
                {
                    return this.User.HasEmail;
                    
                }
                return false;
            }
        }

        public bool HasMobilePhone
        {
            get
            {
                if (this.User != null)
                {
                    return this.User.HasMobilePhone;

                }
                return false;
            }
        }

        #endregion
    }
}