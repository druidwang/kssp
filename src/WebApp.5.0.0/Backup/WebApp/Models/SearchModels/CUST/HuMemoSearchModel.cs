﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.CUST
{
    public class HuMemoSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ResourceGroup { get; set; }
    }
}