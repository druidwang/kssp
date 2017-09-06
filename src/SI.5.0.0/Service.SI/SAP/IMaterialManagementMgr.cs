using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Utility;

namespace com.Sconit.Service.SI.SAP
{
    public interface IMaterialManagementMgr 
    {
        List<ErrorMessage> ExportMMMES0001Data();

        List<ErrorMessage> ExportMMMES0002Data();

        List<ErrorMessage> ExportSTMES0001Data();
    }
}
