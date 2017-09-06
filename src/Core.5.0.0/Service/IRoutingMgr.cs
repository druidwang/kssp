using System;
using System.Collections.Generic;
using com.Sconit.Entity.PRD;

namespace com.Sconit.Service
{
    public interface IRoutingMgr
    {
        IList<RoutingDetail> GetRoutingDetails(string routing, DateTime? effectiveDate);
    }
}
