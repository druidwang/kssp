using System;
using com.Sconit.Entity.Exception;
using System.Data.SqlClient;

namespace com.Sconit.Web.Models
{
    public class ReportSearchStatementModel : BaseModel
    {
        public string ProcedureName { get; set; }
        //public object[] Parameters { get; set; }
        public SqlParameter[] Parameters { get; set; }
        public DateTime CachedDateTime { get; set; }
    }
}
