using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpFiPlan
    {
        #region 
        public double TotalQty
        {
            get
            {
                return AdjustQty + Qty;
            }
        }

        //public double NetShiftQuota
        //{
        //    get
        //    {
        //        if (ShiftType == 0)
        //        {
        //            return 99999999;
        //        }
        //        return ShiftQuota / (int)ShiftType * 3;
        //    }
        //}

        public double MaxQtyPerDay
        {
            get
            {
                //班产定额 * 每天几个班 * 模具数
                return ShiftQuota * ShiftPerDay * MachineQty;
            }
        }

        //辅助字段
        public double UsedQty { get; set; }
        #endregion
    }
}
