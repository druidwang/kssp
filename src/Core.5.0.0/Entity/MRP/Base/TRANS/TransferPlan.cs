using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class TransferPlan : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        public DateTime PlanVersion { get; set; }
        public string Flow { get; set; }
        public string Item { get; set; }
        public double Qty { get; set; }
        public DateTime WindowTime { get; set; }
        public DateTime StartTime { get; set; }
        //public string LocationTo { get; set; }
        //public double PlanQty { get; set; }
        //public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        //public Sconit.CodeMaster.TimeUnit DateType { get; set; }
     
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
            TransferPlan another = obj as TransferPlan;

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
