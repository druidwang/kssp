using System;
using Telerik.Web.Mvc;

namespace com.Sconit.Web.Models
{
    [Serializable]
    public class SearchCacheModel
    {
        public Object SearchObject { get; set; }
        public GridCommand Command { get; set; }
        public DateTime CachedTime { get; set; }
        public bool? isBack { get; set; }
    }
}
