using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpExPlanItemRate
    {
        [Display(Name = "MrpExPlanItemRate_SectionDesc", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
        public string SectionDesc { get; set; }

        [Display(Name = "MrpExPlanItemRate_ItemDesc", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
        public string ItemDesc { get; set; }

        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
    }
}