using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using com.Sconit.Entity.MSG;
using com.Sconit.Entity.CUST;

namespace com.Sconit.Service
{
    public class BaseMgr
    {
        protected static log4net.ILog pubSubLog = log4net.LogManager.GetLogger("Log.PubSubErrLog");
    }
}
