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
        /// ������
        /// </summary>
        public string ProductLine { get; set; }
        /// <summary>
        /// ģ��
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// ��
        /// </summary>
        public string WeekOfYear { get; set; }

        public Int32 PlanVersion { get; set; }
        /// <summary>
        /// ���� ���ù�ʱ
        /// </summary>
        public Double Capacity { get; set; }
        /// <summary>
        /// ���� �ƻ���ʱ
        /// </summary>
        public Double Qty { get; set; }
        /// <summary>
        /// ί������
        /// </summary>
        public Double SubQty { get; set; }
        /// <summary>
        /// ���� �ƻ�����
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
