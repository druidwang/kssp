

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    #region Retrive
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    #endregion

    public class QuotaSearchModel : SearchModelBase
    {
        public string Location { get; set; }

        public string Item { get; set; }
    }
}