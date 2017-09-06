using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class RoutingPositionMap : EntityBase
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "SAPPosition", ResourceType = typeof(Resources.CUST.RoutingPositionMap))]
		public string SAPPosition { get; set; }
		//[Display(Name = "Position", ResourceType = typeof(Resources.CUST.RoutingPositionMap))]
		public string Position { get; set; }
		//[Display(Name = "Location", ResourceType = typeof(Resources.CUST.RoutingPositionMap))]
		public string Location { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (SAPPosition != null)
            {
                return SAPPosition.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            RoutingPositionMap another = obj as RoutingPositionMap;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.SAPPosition == another.SAPPosition);
            }
        } 
    }
	
}
