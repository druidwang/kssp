using System;
using com.Sconit.Entity;

namespace com.Sconit.Entity.SI.BAT
{
    [Serializable]
    public partial class JobDetail : EntityBase
    {
        #region O/R Mapping Properties
		
		public string  Code { get; set; }
		public string Description { get; set; }
		public string ServiceType { get; set; }
        
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
            JobDetail another = obj as JobDetail;

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
