using System;

namespace com.Sconit.Entity.SYS
{
    [Serializable]
    public partial class NumberControl : EntityBase
    {
        #region O/R Mapping Properties
		
		public string Code { get; set; }
		public Int32 Value { get; set; }
        
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
            NumberControl another = obj as NumberControl;

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
