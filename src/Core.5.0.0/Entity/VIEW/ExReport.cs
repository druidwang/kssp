using System;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Entity.VIEW
{
    [Serializable]
    public partial class ExReport
    {
        //ProductLine
        //PlanNo
        //PlanNoNeedTime
        //Seq
        //IType :1代表计划的生产时间段,2代表计划的空闲时间段，3代表实际的生产时间段,4代表实际的空闲时间段
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        public string ProductLine { get; set; }
        public string PlanNo { get; set; }
        public decimal PlanNoNeedTime { get; set; }
        public int BatchNo { get; set; }
        public string Color { get; set; }
        public decimal Theocapacity { get; set; }
        #endregion

       
    }

}
