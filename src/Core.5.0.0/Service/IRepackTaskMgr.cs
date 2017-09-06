using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.WMS;
using com.Sconit.Entity.INV;

namespace com.Sconit.Service
{
    public interface IRepackTaskMgr
    {
        void AssignRepackTask(IList<RepackTask> repackTaskList,string assignUser);

        IList<Hu> SuggestRepackHu(int repackTaskId);

        void ProcessRepackResult(int repackTaskId, IList<string> repackResultIn, IList<string> repackResultOut, DateTime? effectiveDate);
    }
}
