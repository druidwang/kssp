using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.SI.SD_TMS;

namespace com.Sconit.Service.SI
{
    public interface ISD_TMSMgr
    {
        TransportOrderMaster GetTransOrder(string orderNo);

        void Ship(string transOrder, List<string> huIds);
    }
}
