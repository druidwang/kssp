using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace com.Sconit.Web.Models.ReportModels
{
    public class InventoryAgeSearchModel
    {
        public string Level { get; set; }

        public string plantFrom { get; set; }
        public string plantTo { get; set; }

        public string regionFrom { get; set; }
        public string regionTo { get; set; }

        public string locationFrom { get; set; }
        public string locationTo { get; set; }

        public string itemFrom { get; set; }

        public string itemTo { get; set; }

        public string TypeLocation { get; set; }

        public string SAPLocation { get; set; }

        public string TheFactoryFrom { get; set; }

        public string TheFactoryTo { get; set; }

        public string Range1 { get; set; }
        public string Range2 { get; set; }
        public string Range3 { get; set; }
        public string Range4 { get; set; }
        public string Range5 { get; set; }
        public string Range6 { get; set; }
        public string Range7 { get; set; }
        public string Range8 { get; set; }
        public string Range9 { get; set; }
        public string Range10 { get; set; }
        public string Range11 { get; set; }
      

       

    }
}