using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.Sconit.Web.Models.SearchModels.ISI
{
    public class TaskStatusSearchModel : SearchModelBase
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public int? CreateUserId { get; set; }
    }
}
