using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.WMS;

namespace com.Sconit.Service
{
    public interface IShipPlanMgr
    {
        void CreateShipPlan(string orderNo);

        void CancelShipPlan(string orderNo);

        void AssignShipPlan(IList<ShipPlan> shipPlanList, string assignUser);
    }
}
