

namespace com.Sconit.Service.Report
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NPOI.HSSF.UserModel;

    public interface IReportMgr
    {
        void WriteToClient(string reportTemplateFolder,string template, IList<object> list, String fileName);
        //void WriteToClient(String fileName, HSSFWorkbook workbook);
    }
}


