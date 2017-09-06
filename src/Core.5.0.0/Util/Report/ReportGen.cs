using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Utility;
using NPOI.HSSF.UserModel;
using System.Reflection;
using System.Net;
using System.IO;

namespace com.Sconit.Utility.Report
{
    public class ReportGen : IReportGen
    {
        public string ReportTemplateFolder { get; set; }
        public string ReportTemporaryFolder { get; set; }

        public IDictionary<string, Type> dicReportService { get; set; }

        public IReportBase GetIReportBase(String template, IList<object> list)
        {
            IReportBase reportBase = this.GetImplService(template);

            if (reportBase != null)
            {
                reportBase.FillValues(this.ReportTemplateFolder, template, list);
            }

            return reportBase;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public IReportBase GetIReportBase(String template, Dictionary<int, IList<object>> dic)
        {
            IReportBase reportBase = this.GetImplService(template);
            if (reportBase != null)
            {
                foreach (KeyValuePair<int, IList<object>> arr in dic)
                {
                    reportBase.FillValues(this.ReportTemplateFolder, template, arr.Value, arr.Key);
                }
            }
            return reportBase;
        }


        public IReportBase GetImplService(String template)
        {
            template = template.Trim();
            if (template == null || dicReportService == null || !dicReportService.ContainsKey(template)
                || dicReportService[template] == null)
            {
                throw new Exception(string.Format("模板{0}不存在", template));
            }

            ConstructorInfo ct = dicReportService[template].GetConstructor(new Type[0]);
            return (IReportBase)ct.Invoke(new Object[0]);
        }


        public string WriteToFile(String template, IList<object> list)
        {
            IReportBase reportBase = GetIReportBase(template, list);
            //reportBase.FillValues(this.ReportTemplateFolder, template, list);
            return this.WriteToFile(reportBase.GetWorkbook());
        }

        public string WriteToFile(String template, IList<object> list, String fileName)
        {
            IReportBase reportBase = GetIReportBase(template, list);
            //reportBase.FillValues(this.ReportTemplateFolder, template, list);
            return this.WriteToFile(fileName, reportBase.GetWorkbook());
        }

        public void WriteToClient(String template, IList<object> list, String fileName)
        {
            IReportBase reportBase = GetIReportBase(template, list);
            this.WriteToClient(fileName, reportBase.GetWorkbook());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="dic"></param>
        /// <param name="fileName"></param>
        public void WriteToClient(String template, Dictionary<int, IList<object>> dic, String fileName)
        {
            IReportBase reportBase = GetIReportBase(template, dic);
            this.WriteToClient(fileName, reportBase.GetWorkbook());
        }
        public void WriteXlsToClient(String template, IList<object> list, String fileName)
        {
            IReportBase reportBase = GetIReportBase(template, list);
            this.WriteXlsToClient(fileName, reportBase.GetWorkbook());
        }
        public void WriteToClient(String fileName, HSSFWorkbook workbook)
        {
            XlsHelper.WriteToClient2(fileName, workbook);
        }
        public void WriteXlsToClient(String fileName, HSSFWorkbook workbook)
        {
            XlsHelper.WriteToClient(fileName, workbook);
        }
        public string WriteToFile(HSSFWorkbook workbook)
        {
            return XlsHelper.WriteToFile(workbook);
        }
        public string WriteToFile(String fileName, HSSFWorkbook workbook)
        {
            return XlsHelper.WriteToFile(fileName, workbook);
        }

        public void SetTemplateFolder(string templateFolder)
        {
            this.ReportTemplateFolder = templateFolder;
        }
    }
}
