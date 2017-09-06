using System;

namespace com.Sconit.PrintModel.INP
{
    [Serializable]
    public partial class PrintInspectResult : PrintBase
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.INP.InspectResult))]
		public Int32 Id { get; set; }
        public string InspectNo { get; set; }
        public Int32 InspectDetailId { get; set; }
        public Int32 InspectDetailSequence { get; set; }
		public string Item { get; set; }
		public string ItemDescription { get; set; }
		public string ReferenceItemCode { get; set; }
		public string Uom { get; set; }
		public Decimal UnitCount { get; set; }
        public string BaseUom { get; set; }
        public Decimal UnitQty { get; set; }
		public string HuId { get; set; }
		public string LotNo { get; set; }
		public string LocationFrom { get; set; }
		public string CurrentLocation { get; set; }
        public Decimal JudgeQty { get; set; }
        public Decimal HandleQty { get; set; }
        public Boolean IsHandle { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
		public Int32 Version { get; set; }
        public string Defect { get; set; }
        public string ManufactureParty { get; set; }
        public string WMSNo { get; set; }
        public string WMSSeq { get; set; }
        public string IpNo { get; set; }
        public Int32 IpDetailSequence { get; set; }
        public string FailCode { get; set; }
        public string ReceiptNo { get; set; }
        public Int32 ReceiptDetailSequence { get; set; }
        public string Note { get; set; }
        #endregion

    }
	
}
