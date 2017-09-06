using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpReceivePlan : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        public string Flow { get; set; }
        public string Item { get; set; }
        public Double Qty { get; set; }
        public string LocationFrom { get; set; }
        public Int32 SourceId { get; set; }
        public DateTime ReceiveTime { get; set; }
        public DateTime StartTime { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public com.Sconit.CodeMaster.MrpSourceType SourceType { get; set; }
        public DateTime PlanVersion { get; set; }
        public string SourceParty { get; set; }
        public string SourceFlow { get; set; }
        public string ParentItem { get; set; }
        public string Bom { get; set; }
        public string ExtraLocationFrom { get; set; }

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
            MrpShipPlan another = obj as MrpShipPlan;

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
