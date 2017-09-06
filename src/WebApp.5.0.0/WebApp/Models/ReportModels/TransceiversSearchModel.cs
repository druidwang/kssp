using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace com.Sconit.Web.Models.ReportModels
{
    public class TransceiversSearchModel:SearchModelBase
    {
        public string Level { get; set; }

        public string TheFactoryFrom { get; set; }
        public string TheFactoryTo { get; set; }

        public string plantFrom { get; set; }
        public string plantTo { get; set; }

        public string regionFrom { get; set; }
        public string regionTo { get; set; }

        public string locationFrom { get; set; }
        public string locationTo { get; set; }

        public string itemFrom { get; set; }

        public string itemTo { get; set; }

        public string SAPLocation { get; set; }

        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string TypeLocation { get; set; }
    }
}