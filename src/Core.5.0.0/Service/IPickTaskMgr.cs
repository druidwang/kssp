using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.WMS;

namespace com.Sconit.Service
{
    public interface IPickTaskMgr
    {
        void CreatePickTask(IDictionary<int, decimal> shipPlanIdAndQtyDic);

        void PorcessPickResult4PickQty(Dictionary<int, decimal> pickResults);

        void PorcessPickResult4PickLotNoAndHu(Dictionary<int, List<string>> pickResults);

        void AssignPickTask(IList<PickTask> pickTaskList,string assignUser);

        void PorcessDeliverBarCode2Hu(string deliverBarCode, string HuId);
    }
}
