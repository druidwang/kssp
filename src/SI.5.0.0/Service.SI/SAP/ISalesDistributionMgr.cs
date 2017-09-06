using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Utility;

namespace com.Sconit.Service.SI.SAP
{
    public interface ISalesDistributionMgr
    {
        List<ErrorMessage> ExportSDMES0001();

        List<ErrorMessage> ExportSDMES0002();

        List<ErrorMessage> GenSDMESData();

        List<ErrorMessage> ExportMESQTY0001();
        List<ErrorMessage> ExportMESQTY0001(DateTime mesInvSnap);
    }
}
