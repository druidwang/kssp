using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.MD;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class RccpMiPlan
    {

        public double CheQty
        {
            get { return TotalQty / CheRateQty; }
        }

        public double TotalQty
        {
            get { return Qty + SubQty; }
        }
        //计划工时
        public double RequireTime
        {
            get { return (this.TotalQty / this.CheRateQty) * this.WorkHour; }
        
        }
        //负荷率
        public double Load
        {
            get { return RequireTime / UpTime; }
        }
        //RequireTime/UpTime
        //不考虑同一物料有多条委外供应商
        public MrpFlowDetail SubFlowDetail { get; set; }
    }
}
