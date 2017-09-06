using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpExPlan
    {

        public double UsedQty { get; set; }

        public double CurrentQty { get; set; }

        /// <summary>
        /// ��ƽ��
        /// </summary>
        public double PlanQtyRate { get; set; }

        /// <summary>
        /// �����
        /// </summary>
        public double RequirePlanRate
        {
            get
            {
                if (NetQty == 0)
                {
                    return 0;
                }
                return CurrentQty / NetQty;
            }
        }
        //Bom��ʱ��
    }

}
