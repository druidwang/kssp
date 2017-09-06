using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace com.Sconit.Web.Models.ReportModels
{
    public class CustReportSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Sql { get; set; }
        public string ParamType { get; set; }
        public string ParamKey { get; set; }
        public string ParamText { get; set; }

        public int Seq { get; set; }
    }
}