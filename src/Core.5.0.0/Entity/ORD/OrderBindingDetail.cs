using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class OrderBindingDetail
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        #endregion
    }

    public class BindDemand
    {
        public string OrderNo { get; set; }
        public int OrderDetailId { get; set; }
        public int? OrderBomDetailId { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public string Uom { get; set; }
        public string BaseUom { get; set; }
        public string ManufactureParty { get; set; }
        public CodeMaster.QualityType QualityType { get; set; }
        public decimal UnitQty { get; set; }
        public decimal UnitCount { get; set; }
        public decimal Qty { get; set; }
        public string ExtraDemandSource { get; set; }
        public DateTime WindowTime { get; set; }
    }
}