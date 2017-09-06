using System;

namespace com.Sconit.Entity.ACC
{
    [Serializable]
    public partial class Permission : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public string PermissionCategory { get; set; }
        public Int32 Sequence { get; set; }
        
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
            Permission another = obj as Permission;

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
