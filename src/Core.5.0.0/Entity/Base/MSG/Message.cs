using System;
using com.Sconit.CodeMaster;

namespace com.Sconit.Entity.MSG
{
    [Serializable]
    public partial class Message : EntityBase
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.MSG.Message))]
		public Int32 Id { get; set; }
		//[Display(Name = "SendTo", ResourceType = typeof(Resources.MSG.Message))]
		public string SendTo { get; set; }
		//[Display(Name = "Message", ResourceType = typeof(Resources.MSG.Message))]
		public string Content { get; set; }
		//[Display(Name = "Priority", ResourceType = typeof(Resources.MSG.Message))]
        public MessagePriority Priority { get; set; }
		//[Display(Name = "Status", ResourceType = typeof(Resources.MSG.Message))]
		public MessageStatus Status { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.MSG.Message))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.MSG.Message))]
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
            Message another = obj as Message;

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
