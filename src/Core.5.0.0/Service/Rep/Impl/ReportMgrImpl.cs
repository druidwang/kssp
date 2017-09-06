namespace com.Sconit.Service.Report.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using com.Sconit.Utility;
    using NPOI.HSSF.UserModel;
    using com.Sconit.Utility;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Util.Report;

    public class ReportMgrImpl : IReportMgr
    {
        public IDictionary<string, string> dicReportService { get; set; }

        public IReportBase GetIReportBaseMgr(string reportTemplateFolder ,string template, IList<object> list)
        {
            IReportBase reportMgr = this.GetImplService(template);

            if (reportMgr != null)
            {
                reportMgr.FillValues(reportTemplateFolder,template, list);
            }

            return reportMgr;

        }

        public IReportBase GetImplService(String template)
        {
            if (template == null || dicReportService == null || !dicReportService.ContainsKey(template)
                || dicReportService[template] == null || dicReportService[template].Length == 0)
            {
                throw new BusinessException("Common.Business.Error.EntityNotExist", template);
            }

            //return ServiceLocator.GetService<IReportBase>(dicReportService[template]);
            return null;
        }

        public void WriteToClient(string reportTemplateFolder,string template, IList<object> list, String fileName)
        {
            IReportBase iReportMgr = GetIReportBaseMgr(reportTemplateFolder,template, list);
            this.WriteToClient(fileName, iReportMgr.GetWorkbook());
        }

        public void WriteToClient(String fileName, HSSFWorkbook workbook)
        {
            ReportHelper.WriteToClient(fileName, workbook);
        }
    }
}
