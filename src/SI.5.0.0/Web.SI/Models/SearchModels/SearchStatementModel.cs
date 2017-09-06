using System;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Web.Models
{
    public class SearchStatementModel : BaseModel
    {
        public string SelectCountStatement { get; set; }
        public string SelectStatement { get; set; }
        public string WhereStatement { get; set; }
        public string SortingStatement { get; set; }
        public object[] Parameters { get; set; }
        public DateTime CachedDateTime { get; set; }

        public string GetSearchCountStatement()
        {
            if (!string.IsNullOrWhiteSpace(SelectCountStatement))
            {
                return SelectCountStatement 
                    + (!string.IsNullOrWhiteSpace(WhereStatement) ? " " + WhereStatement : string.Empty);
            }
            else
            {
                throw new TechnicalException("SelectCountStatement not initialize.");
            }
        }

        public string GetSearchStatement()
        {
            if (!string.IsNullOrWhiteSpace(SelectStatement))
            {
            return SelectStatement 
                + (!string.IsNullOrWhiteSpace(WhereStatement) ? " " + WhereStatement : string.Empty)
                + (!string.IsNullOrWhiteSpace(SortingStatement) ? " " + SortingStatement : string.Empty);
            }
            else
            {
                throw new TechnicalException("SelectStatement not initialize.");
            }
        }
    }
}
