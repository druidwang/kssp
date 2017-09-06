using System;
using com.Sconit.Entity;

namespace com.Sconit.Entity.SI.BAT
{
    [Serializable]
    public partial class TriggerParameter : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        public string TriggerName { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
        
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
            TriggerParameter another = obj as TriggerParameter;

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
