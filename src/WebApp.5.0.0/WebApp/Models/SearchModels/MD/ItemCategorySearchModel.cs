using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class ItemCategorySearchModel:SearchModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public Boolean IsActive { get; set; }
        public string ParentCategory { get; set; }
        public string SubCategory { get; set; }
    }
}