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
        //�ƻ���ʱ
        public double RequireTime
        {
            get { return (this.TotalQty / this.CheRateQty) * this.WorkHour; }
        
        }
        //������
        public double Load
        {
            get { return RequireTime / UpTime; }
        }
        //RequireTime/UpTime
        //������ͬһ�����ж���ί�⹩Ӧ��
        public MrpFlowDetail SubFlowDetail { get; set; }
    }
}
