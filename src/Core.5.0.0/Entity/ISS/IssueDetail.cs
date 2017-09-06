using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISS
{
    public partial class IssueDetail
    {
        #region Non O/R Mapping Properties

        

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.SendStatus, ValueField = "EmailStatus")]
        [Display(Name = "EmailStatus", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public string EmailStatusDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.SendStatus, ValueField = "SMSStatus")]
        [Display(Name = "SMSStatus", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public string SMSStatusDescription { get; set; }



        [Display(Name = "UserCode", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public string UserCode
        {
            get
            {
                if (this.User!=null)
                {
                    return User.Code;
                }
                return string.Empty ;

            }
        }
        [Display(Name = "UserName", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public string UserName
        {
            get
            {
                if (this.User != null)
                {
                    return User.FullName;
                }
                return string.Empty;

            }
        }
        #endregion
    }
}