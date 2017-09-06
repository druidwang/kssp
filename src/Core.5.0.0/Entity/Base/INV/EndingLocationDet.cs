using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class EndingLocationDet : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public Int32 FinanceYear { get; set; }
		public Int32 FinanceMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
		public string Location { get; set; }
		public string Item { get; set; }
        public Decimal QualifyQty { get; set; }
        public Decimal InspectQty { get; set; }
        public Decimal RejectQty { get; set; }
        
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
            EndingLocationDet another = obj as EndingLocationDet;

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
