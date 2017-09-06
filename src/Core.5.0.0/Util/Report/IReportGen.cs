using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;

namespace com.Sconit.Utility.Report
{
    public interface IReportGen
    {
        IReportBase GetIReportBase(String template, IList<object> list);
        IReportBase GetIReportBase(String template, Dictionary<int, IList<object>> dic);
        string WriteToFile(String template, IList<object> list);
        string WriteToFile(String template, IList<object> list, String fileName);
        string WriteToFile(HSSFWorkbook workbook);
        string WriteToFile(String fileName, HSSFWorkbook workbook);
        void WriteToClient(String template, IList<object> list, String fileName);
        void WriteXlsToClient(String template, IList<object> list, String fileName);
        void SetTemplateFolder(string templateFolder);
        void WriteToClient(String template, Dictionary<int, IList<object>> dic, String fileName);
    }
}
