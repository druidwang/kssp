using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Service.MRP
{
    public interface IRccpMgr
    {
        void RunRccp(DateTime planVersion, DateTime snapTime, CodeMaster.TimeUnit dateType, string dateIndex, User user);

        void RunRccp(CodeMaster.TimeUnit dateType);
    }
}

