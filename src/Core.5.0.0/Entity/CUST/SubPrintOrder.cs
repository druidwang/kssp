using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    public partial class SubPrintOrder
    {
        [Display(Name = "UserId", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string UserCode { get; set; }

        [Display(Name = "ExcelTemplate", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string ExcelTemplateDescription { get; set; }
    }
}
