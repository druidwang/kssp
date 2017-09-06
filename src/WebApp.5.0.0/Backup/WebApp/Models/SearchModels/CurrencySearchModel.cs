using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models
{
    public class CurrencySearchModel:SearchModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Boolean IsBase { get; set; }
    }
}