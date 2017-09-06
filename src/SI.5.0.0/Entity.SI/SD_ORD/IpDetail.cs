using System;
using System.Collections.Generic;

namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public partial class IpDetail 
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        public com.Sconit.CodeMaster.IpType Type { get; set; }
		public string IpNo { get; set; }
        public Int32 Sequence { get; set; }
        public string OrderNo { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }
        public Int32? OrderDetailId { get; set; }
        public Int32 OrderDetailSequence { get; set; }
		public string Item { get; set; }
		public string ItemDescription { get; set; }
		public string ReferenceItemCode { get; set; }
        //public string BaseUom { get; set; }
		public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
		public Decimal Qty { get; set; }
		public Decimal ReceivedQty { get; set; }
		public Decimal UnitQty { get; set; }
		public string LocationFrom { get; set; }
		//public string LocationFromName { get; set; }
		public string LocationTo { get; set; }
		//public string LocationToName { get; set; }
		public Boolean IsInspect { get; set; }
		//public string BillAddress { get; set; }
		//public string PriceList { get; set; }
		public Decimal? UnitPrice { get; set; }
		public string Currency { get; set; }
        //public Boolean IsProvisionalEstimate { get; set; }
		//public string Tax { get; set; }
		//public Boolean IsIncludeTax { get; set; }
        public com.Sconit.CodeMaster.OrderBillTerm BillTerm { get; set; }
        public Boolean IsClose { get; set; }
        public string GapReceiptNo { get; set; }
        public Boolean IsScanHu { get; set; }
        public Boolean IsChangeUnitCount { get; set; }
        //public Int32 CreateUserId { get; set; }
        //public string CreateUserName { get; set; }
        //public DateTime CreateDate { get; set; }
        //public Int32 LastModifyUserId { get; set; }
        //public string LastModifyUserName { get; set; }
        //public DateTime LastModifyDate { get; set; }
        //public Int32 Version { get; set; }
        public string ManufactureParty { get; set; }
        #endregion

        #region ¸¨Öú×Ö¶Î
        //public List<IpDetailInput> IpDetailInputs { get; set; }
        public decimal CurrentQty { get; set; }
        public decimal RemainReceivedQty { get; set; }
        public decimal Carton { get; set; }
        #endregion
    }

    public class IpDetailInput
    {
        public int Id { get; set; }
        public string HuId { get; set; }
        public decimal Qty { get; set; }
        public decimal ShipQty { get; set; }
        public decimal ReceiveQty { get; set; }
        public string LotNo { get; set; }
        public string Bin { get; set; }
        public bool IsMatchedHu { get; set; }
        public bool IsOriginal { get; set; }
    }	
}
