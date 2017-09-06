using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Utility;
namespace com.Sconit.Service.SI.SAP
{
    public interface ISAPInterfaceCommonMgr
    {
        //List<ErrorMessage> GenMESData(string batchNo, string timeContrlCode);
        List<ErrorMessage> GenBusinessOrderData(DateTime curDate);
        List<ErrorMessage> GenBusinessAdjustOrderData(DateTime curDate);
        List<ErrorMessage> GenBusinessAdjustTailOrderData(DateTime curDate);
        DateTime GenMesQtyData();
        List<ErrorMessage> TransBusinessOrderData();
    }
}
