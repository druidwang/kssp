using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MD
{
    public partial class Custodian
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Custodian_ItemCodes", ResourceType = typeof(Resources.MD.Custodian))]
        public string ItemCodes { get; set; }
    }
}
