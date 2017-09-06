using System;
using System.Collections.Generic;
using com.Sconit.Entity.Exception;
using NHibernate.Type;

namespace com.Sconit.Web.Models
{
    public class ProcedureSearchStatementModel : BaseModel
    {
        public string CountProcedure { get; set; }
        public string SelectProcedure { get; set; }
        public IList<ProcedureParameter> Parameters { get; set; }

        public IList<ProcedureParameter> PageParameters { get; set; }

        public DateTime CachedDateTime { get; set; }

        public string GetSearchCountStatement()
        {
            if (!string.IsNullOrWhiteSpace(CountProcedure))
            {
                return CountProcedure;
            }
            else
            {
                throw new TechnicalException("SelectCountProcedure not initialize.");
            }
        }

        public string GetSearchStatement()
        {
            if (!string.IsNullOrWhiteSpace(SelectProcedure))
            {
                return SelectProcedure;
            }
            else
            {
                throw new TechnicalException("SelectProcedure not initialize.");
            }
        }

        public List<object> GetParameterValues()
        {
            List<object> values = new List<object>();
            if (Parameters.Count > 0)
            {
                foreach (var parameter in Parameters)
                {
                    values.Add(parameter.Parameter);
                }
            }
            return values;
        }

        public List<object> GetAllParameterValues()
        {
            List<object> values = new List<object>();
            if (Parameters.Count > 0)
            {
                foreach (var parameter in Parameters)
                {
                    values.Add(parameter.Parameter);
                }
            }
            if (PageParameters.Count > 0)
            {
                foreach (var parameter in PageParameters)
                {
                    values.Add(parameter.Parameter);
                }
            }
            return values;
        }

        public List<IType> GetParameterTypes()
        {
            List<IType> types = new List<IType>();
            if (Parameters.Count > 0)
            {
                foreach (var parameter in Parameters)
                {
                    types.Add(parameter.Type);
                }
            }
            return types;
        }

        public List<IType> GetAllParameterTypes()
        {
            List<IType> types = new List<IType>();
            if (Parameters.Count > 0)
            {
                foreach (var parameter in Parameters)
                {
                    types.Add(parameter.Type);
                }
            }
            if (PageParameters.Count > 0)
            {
                foreach (var parameter in PageParameters)
                {
                    types.Add(parameter.Type);
                }
            }
            return types;
        }
    }

    public class ProcedureParameter
    {
        public object Parameter { get; set; }

        public IType Type { get; set; }
    }
}
