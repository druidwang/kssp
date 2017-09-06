using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class Region : Party
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Region_Plant", ResourceType = typeof(Resources.MD.Region))]
        public string Plant { get; set; }

        [Display(Name = "Region_Workshop", ResourceType = typeof(Resources.MD.Region))]
        public string Workshop { get; set; }
        
        #region Non O/R Mapping Properties
        #endregion
    }
}