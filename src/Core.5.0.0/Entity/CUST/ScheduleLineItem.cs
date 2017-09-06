using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.CUST
{
    public partial class ScheduleLineItem
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        #endregion

        [Display(Name = "Item_ReferenceCode", ResourceType = typeof(Resources.MD.Item))]
        public string ReferenceCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Item_Uom", ResourceType = typeof(Resources.MD.Item))]
        public string Uom { get; set; }

        
        [Display(Name = "Item_Description", ResourceType = typeof(Resources.MD.Item))]
        public string Description { get; set; }
        [Display(Name = "Item_UC", ResourceType = typeof(Resources.MD.Item))]
        public Decimal UnitCount { get; set; }
    }
}