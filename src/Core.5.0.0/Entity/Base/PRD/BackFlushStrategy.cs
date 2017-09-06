using System;

namespace com.Sconit.Entity.PRD
{
    [Serializable]
    public partial class BackFlushStrategy : EntityBase
    {
        #region O/R Mapping Properties
		
		public string Code { get; set; }
		public string Description { get; set; }
        public Byte BackFlushMethod { get; set; }
        public Byte FeedMethod { get; set; }
        //public Boolean IsScanHu { get; set; }
		public Boolean IsAutoFeed { get; set; }
        //public Byte BackFlushInShortHandle { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            BackFlushStrategy another = obj as BackFlushStrategy;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Code == another.Code);
            }
        } 
    }
	
}
