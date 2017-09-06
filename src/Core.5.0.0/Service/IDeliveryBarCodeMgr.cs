using System;
using System.Collections.Generic;
using com.Sconit.Entity.WMS;

namespace com.Sconit.Service
{
    public interface IDeliveryBarCodeMgr
    {
        IList<DeliveryBarCode> CreateDeliveryBarCode(IList<ShipPlan> shipPlanList);
    }
}
