using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class DeferredFeedCounter : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Flow", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
		public string Flow { get; set; }
		//[Display(Name = "Count", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
		public Int32 Counter { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.CUST.DeferredFeedCount))]
        
        #endregion

		public override int GetHashCode()
        {
			if (Flow != null)
            {
                return Flow.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            DeferredFeedCounter another = obj as DeferredFeedCounter;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Flow == another.Flow);
            }
        } 
    }
	
}
