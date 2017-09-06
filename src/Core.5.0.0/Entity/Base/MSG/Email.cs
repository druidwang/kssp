using System;
using System.Net.Mail;
using com.Sconit.CodeMaster;

namespace com.Sconit.Entity.MSG
{
    [Serializable]
    public partial class Email : EntityBase
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.MSG.Email))]
		public Int32 Id { get; set; }
		//[Display(Name = "Subject", ResourceType = typeof(Resources.MSG.Email))]
		public string Subject { get; set; }
		//[Display(Name = "Body", ResourceType = typeof(Resources.MSG.Email))]
		public string Body { get; set; }
		//[Display(Name = "MailTo", ResourceType = typeof(Resources.MSG.Email))]
		public string MailTo { get; set; }
		//[Display(Name = "ReplayTo", ResourceType = typeof(Resources.MSG.Email))]
		public string ReplayTo { get; set; }
		//[Display(Name = "Priority", ResourceType = typeof(Resources.MSG.Email))]
        public MailPriority Priority { get; set; }
		//[Display(Name = "Status", ResourceType = typeof(Resources.MSG.Email))]
        public EmailStatus Status { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.MSG.Email))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.MSG.Email))]
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
            Email another = obj as Email;

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
