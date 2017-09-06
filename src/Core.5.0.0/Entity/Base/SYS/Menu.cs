using System;

namespace com.Sconit.Entity.SYS
{
    [Serializable]
    public partial class Menu : EntityBase
    {
        #region O/R Mapping Properties
		
		public string Code { get; set; }
        public string Name { get; set; }
        public string ParentMenuCode { get; set; }
        public Int32 Sequence { get; set; }
		public string Description { get; set; }
		public string PageUrl { get; set; }
		public string ImageUrl { get; set; }
		public Boolean IsActive { get; set; }
        
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
            Menu another = obj as Menu;

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
