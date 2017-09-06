using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class ItemKitSearchModel:SearchModelBase
    {
        public string KitItem { get; set; }
        public string ChildItem { get; set; }
        public Boolean IsActive { get; set; }
        public string CreateUserName { get; set; }
    }
}