using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class RepackOccupy : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string UUID { get; set; }
		public string OrderNo { get; set; }
		public Int32 OrderSeq { get; set; }
		public Int32 ShipPlanId { get; set; }
		public string TargetDock { get; set; }
        public Decimal OccupyQty { get; set; }
        public Decimal ReleaseQty { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
		public Int32 Version { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Id != null)
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
            RepackOccupy another = obj as RepackOccupy;

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
