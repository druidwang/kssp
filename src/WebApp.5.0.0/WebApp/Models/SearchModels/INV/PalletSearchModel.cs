using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class PalletSearchModel : SearchModelBase
    {
        public String HuId { get; set; }
        public String Code { get; set; }
        public String Description { get; set; }
     
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
      
    }
}