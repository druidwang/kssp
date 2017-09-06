using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class RccpFiPlan : EntityBase
    {
        #region O/R Mapping Properties
        public string ProductLine { get; set; }
        public string Machine { get; set; }
        public string Item { get; set; }
        public string DateIndex { get; set; }
        public CodeMaster.TimeUnit DateType { get; set; }
        public DateTime PlanVersion { get; set; }

        public Double Qty { get; set; }

        public string Model { get; set; }
        public Double ModelRate { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (ProductLine != null
                && Machine != null
                && Item != null
                && DateIndex != null
                && (int)DateType != 0
                && PlanVersion != DateTime.MinValue
                && Model != null)
            {
                return ProductLine.GetHashCode()
                    ^ Machine.GetHashCode()
                    ^ Item.GetHashCode()
                    ^ DateIndex.GetHashCode()
                    ^ DateType.GetHashCode()
                    ^ PlanVersion.GetHashCode()
                    ^ Model.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            RccpFiPlan another = obj as RccpFiPlan;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.ProductLine == another.ProductLine)
                    && (this.Machine == another.Machine)
                    && (this.Item == another.Item)
                    && (this.DateIndex == another.DateIndex)
                    && (this.DateType == another.DateType)
                    && (this.PlanVersion == another.PlanVersion)
                    && (this.Model == another.Model);
            }
        }
    }

}
