using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class RccpCapacity
    {
        #region O/R Mapping Properties
        /// <summary>
        /// 生产线
        /// </summary>
        public string ProductLine { get; set; }
        /// <summary>
        /// 模具
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public string WeekOfYear { get; set; }

        public Int32 PlanVersion { get; set; }
        /// <summary>
        /// 产能 可用工时
        /// </summary>
        public Double Capacity { get; set; }
        /// <summary>
        /// 需求 计划工时
        /// </summary>
        public Double Qty { get; set; }
        /// <summary>
        /// 委外需求
        /// </summary>
        public Double SubQty { get; set; }
        /// <summary>
        /// 数量 计划产量
        /// </summary>
        public Double PlanQty { get; set; }
        #endregion
        public Double LoadBalance
        {
            get
            {
                if (Capacity == 0)
                {
                    return 0;
                }
                else
                {
                    return Qty / Capacity;
                }
            }
        }
    }
}
