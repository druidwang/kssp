using System;

namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public partial class MiscOrderDetail 
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public string MiscOrderNo { get; set; }
		public Int32 Sequence { get; set; }
		public string Item { get; set; }
		public string ItemDescription { get; set; }
		public string ReferenceItemCode { get; set; }
		public string Uom { get; set; }
		public string BaseUom { get; set; }
		public Decimal UnitCount { get; set; }
		public Decimal UnitQty { get; set; }
		public string Location { get; set; }
		public string ReserveNo { get; set; }
		public string ReserveLine { get; set; }
		public Decimal Qty { get; set; }
        //public Int32 CreateUserId { get; set; }
        //public string CreateUserName { get; set; }
        //public DateTime CreateDate { get; set; }
        //public Int32 LastModifyUserId { get; set; }
        //public string LastModifyUserName { get; set; }
        //public DateTime LastModifyDate { get; set; }

        public string ExternalOrderNo { get; set; }

        public string ExternalSequence { get; set; }
        #endregion

    }
	
}
