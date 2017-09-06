using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ISS
{
    [Serializable]
    public partial class SMSStatus : EntityBase
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public Int32 Id { get; set; }
		[Display(Name = "Issue", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string Issue { get; set; }
		//[Display(Name = "IssueDetail", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public Int32? IssueDetail { get; set; }
		[Display(Name = "IssueLevel", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string IssueLevel { get; set; }
		[Display(Name = "MsgID", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string MsgID { get; set; }
		[Display(Name = "SeqID", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public Int32? SeqID { get; set; }
		[Display(Name = "SrcID", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string SrcID { get; set; }
		[Display(Name = "DestID", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string DestID { get; set; }
		[Display(Name = "ServiceID", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string ServiceID { get; set; }
		[Display(Name = "SrcTerminalId", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string SrcTerminalId { get; set; }
		[Display(Name = "SrcTerminalType", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public Int32? SrcTerminalType { get; set; }
		[Display(Name = "MsgFmt", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public Int32? MsgFmt { get; set; }
		[Display(Name = "MsgLength", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public Int32? MsgLength { get; set; }
		[Display(Name = "Status", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string Status { get; set; }
		[Display(Name = "Content", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string Content { get; set; }
		[Display(Name = "DoneDatetime", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public DateTime? DoneDatetime { get; set; }
		[Display(Name = "SubmitDatetime", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public DateTime? SubmitDatetime { get; set; }
		[Display(Name = "EventHandler", ResourceType = typeof(Resources.ISS.SMSStatus))]
		public string EventHandler { get; set; }

        public Int32? CreateUserId { get; set; }
        [Display(Name = "Common_CreateUserName", ResourceType = typeof(Resources.SYS.Global))]
        public string CreateUserName { get; set; }
        [Display(Name = "Common_CreateDate", ResourceType = typeof(Resources.SYS.Global))]
        public DateTime CreateDate { get; set; }
        public Int32? LastModifyUserId { get; set; }
        [Display(Name = "Common_LastModifyUserName", ResourceType = typeof(Resources.SYS.Global))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "Common_LastModifyDate", ResourceType = typeof(Resources.SYS.Global))]
        public DateTime LastModifyDate { get; set; }
        #endregion

		public override int GetHashCode()
        {
			if (Id != 0)
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
            SMSStatus another = obj as SMSStatus;

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
