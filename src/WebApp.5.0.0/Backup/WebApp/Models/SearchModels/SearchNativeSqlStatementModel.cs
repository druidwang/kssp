using System;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Web.Models
{
    public class SearchNativeSqlStatementModel : BaseModel
    {
        public string SelectSql { get; set; }
        public string SortingStatement { get; set; }
        public object[] Parameters { get; set; }
    }
}
