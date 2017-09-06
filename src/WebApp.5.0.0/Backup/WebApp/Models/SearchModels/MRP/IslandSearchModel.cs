﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class IslandSearchModel : SearchModelBase
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string Region { get; set; }

        public bool IsActive { get; set; }
    }
}