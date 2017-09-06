


namespace com.Sconit.Utility.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using System.IO;

    public interface IReportBase
    {
        bool FillValues(String templateFileFolder, String templateFileName, IList<object> list);
        bool FillValues(String templateFileFolder, String templateFileName, IList<object> list, int sheetIndex);
        int CopyPage(int pageCount, int columnCount);
        void CopyPageValues(int pageIndex);
        HSSFWorkbook GetWorkbook();
    }
}


