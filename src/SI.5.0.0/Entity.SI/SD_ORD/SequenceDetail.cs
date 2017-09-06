using System;

namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public class SequenceDetail 
    {	
		public Int32 Id { get; set; }
		public string SequenceNo { get; set; }
		public string OrderNo { get; set; }
		public string TraceCode { get; set; }
		public Int32 OrderDetailId { get; set; }
		public Int32 OrderDetailSequence { get; set; }
		public Int64 Sequence { get; set; }
		public string Item { get; set; }
		public string ItemDescription { get; set; }
		public string ReferenceItemCode { get; set; }
		public string Uom { get; set; }
		//public Decimal UnitQty { get; set; }
		//public string BaseUom { get; set; }
		public Decimal UnitCount { get; set; }
		public CodeMaster.QualityType QualityType { get; set; }
		public string ManufactureParty { get; set; }
		public Decimal Qty { get; set; }
		public Boolean IsClose { get; set; }
		public string HuId { get; set; }
		public string LotNo { get; set; }
        
    }
	
}
