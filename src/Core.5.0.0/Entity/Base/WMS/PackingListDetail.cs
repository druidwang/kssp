using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.CodeMaster;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class PackingListDetail : EntityBase,IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }


        public string PackingListCode { get; set; }

        public string OrderNo { get; set; }
        public Int32 OrderSeq { get; set; }
        public Int32 ShipPlanId { get; set; }
		public string Location { get; set; }
		public string Dock { get; set; }
		public string Item { get; set; }
		public string Uom { get; set; }
		public Decimal UnitCount { get; set; }
		public Decimal Qty { get; set; }
		public string LotNo { get; set; }
        public string HuId { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
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
            PackingListDetail another = obj as PackingListDetail;

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
