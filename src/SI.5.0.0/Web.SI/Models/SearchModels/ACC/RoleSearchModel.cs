using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ACC
{
    public class RoleSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int? Type { get; set; }

    }
}