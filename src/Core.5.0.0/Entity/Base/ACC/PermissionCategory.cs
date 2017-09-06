using System;

namespace com.Sconit.Entity.ACC
{
    [Serializable]
    public partial class PermissionCategory : EntityBase
    {
        #region O/R Mapping Properties
		
		public string Code { get; set; }
		public string Description { get; set; }
        public com.Sconit.CodeMaster.PermissionCategoryType Type { get; set; }
        public Int32 Sequence { get; set; }
        
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
            PermissionCategory another = obj as PermissionCategory;

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
