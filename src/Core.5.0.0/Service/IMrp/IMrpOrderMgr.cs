using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.MRP.VIEW;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.SCM;

namespace com.Sconit.Service.MRP
{
    public interface IMrpOrderMgr
    {
        OrderMaster CreateFiOrder(string flow, DateTime planVersion, DateTime planDate, string shift);

        OrderMaster CreateExScrapOrder(MrpExScrap mrpExScrap);
        void CancelExScrapOrder(MrpExScrap mrpExScrap);

        OrderMaster CreateMiOrder(IList<MrpMiShiftPlan> mrpMiShiftPlan);

        IList<string> GetActiveOrder(string flow, DateTime effDate, string shift);

        ReceiptMaster ReceiveExOrder(MrpExShiftPlan mrpExShiftPlan);

        ReceiptMaster ReceiveUrgentExOrder(FlowDetail flowDetail);
    }
}

