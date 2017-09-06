using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.TMS;

namespace com.Sconit.Service
{
    public interface ITransportMgr
    {
        string CreateTransportOrder(TransportOrderMaster transportOrderMaster, IDictionary<int, string> shipAddressDic, IList<string> ipNoList);

        void AddTransportOrderRoute(string orderNo, int seq, string shipAddress);

        void AddTransportOrderDetail(string orderNo, IList<string> ipNoList);

        void DeleteTransportOrderRoute(string orderNo, int transportOrderRouteId);

        void DeleteTransportOrderDetail(string orderNo, IList<string> ipNoList);

        void ReleaseTransportOrderMaster(string orderNo);

        void StartTransportOrderMaster(string orderNo);

        void Calculate(string orderNo);
    }
}
