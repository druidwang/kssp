using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class Mileage : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public string Description { get; set; }
        public com.Sconit.CodeMaster.TransportMode TransportMode { get; set; }
		public string ShipFrom { get; set; }
		public string ShipFromDescription { get; set; }
		public string ShipTo { get; set; }
		public string ShipToDescription { get; set; }
		public Boolean? IsActive { get; set; }
		public Decimal? Distance { get; set; }
		public string CreateUserName { get; set; }
        public Int32 CreateUserId { get; set; }
		public DateTime CreateDate { get; set; }
		public string LastModifyUserName { get; set; }
        public Int32 LastModifyUserId { get; set; }
		public DateTime LastModifyDate { get; set; }
        
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
            Mileage another = obj as Mileage;

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
