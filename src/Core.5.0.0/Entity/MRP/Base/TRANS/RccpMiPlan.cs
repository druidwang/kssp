using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class RccpMiPlan : EntityBase
    {
        #region O/R Mapping Properties
        public string ProductLine { get; set; }
        public string Item { get; set; }
        public string DateIndex { get; set; }
        public CodeMaster.TimeUnit DateType { get; set; }
        public DateTime PlanVersion { get; set; }

        public Double WorkHour { get; set; }
        public double CheRateQty { get; set; }
        //�ƻ�����	
        public Double Qty { get; set; }
        //��Э����
        public Double SubQty { get; set; }
        //��λ����
        public double HaltTime { get; set; }
        //��λ����
        public double TrialProduceTime { get; set; }
        //��λ����
        public double Holiday { get; set; }
        //        ���ù�ʱ
        public double UpTime { get; set; }
        #endregion


        public override int GetHashCode()
        {
            if (ProductLine != null  
                && Item != null 
                && DateIndex != null 
                && (int)DateType != 0
                && PlanVersion != DateTime.MinValue)
            {
                return ProductLine.GetHashCode()
                    ^ Item.GetHashCode()
                    ^ DateIndex.GetHashCode()
                    ^ DateType.GetHashCode()
                    ^ PlanVersion.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            RccpMiPlan another = obj as RccpMiPlan;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.ProductLine == another.ProductLine)
                    && (this.Item == another.Item)
                    && (this.DateIndex == another.DateIndex)
                    && (this.DateType == another.DateType)
                    && (this.PlanVersion == another.PlanVersion);
            }
        }
    }

}
