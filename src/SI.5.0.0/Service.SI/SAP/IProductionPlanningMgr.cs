using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Utility;
namespace com.Sconit.Service.SI.SAP
{
    public interface IProductionPlanningMgr 
    {
        List<ErrorMessage> ExportPPMES0001Data();
        List<ErrorMessage> ExportPPMES0002Data();
        List<ErrorMessage> ExportPPMES0003Data();
        List<ErrorMessage> ExportPPMES0004Data();
        List<ErrorMessage> ExportPPMES0005Data();
        List<ErrorMessage> ExportPPMES0006Data();
    }
}
