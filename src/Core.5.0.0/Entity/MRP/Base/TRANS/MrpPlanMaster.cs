using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpPlanMaster : EntityBase
    {
        #region O/R Mapping Properties
        [Display(Name = "MrpPlanMaster_PlanVersion", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public DateTime PlanVersion { get; set; }//Ö÷¼ü
        [Display(Name = "MrpPlanMaster_SnapTime", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public DateTime SnapTime { get; set; }
        [Display(Name = "MrpPlanMaster_SourcePlanVersion", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public DateTime SourcePlanVersion { get; set; }
        [Display(Name = "MrpPlanMaster_DateIndex", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public string DateIndex { get; set; }

        public CodeMaster.ResourceGroup ResourceGroup { get; set; }
        [Display(Name = "MrpPlanMaster_Status", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public CodeMaster.MessageType Status { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "MrpPlanMaster_CreateUserName", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public string CreateUserName { get; set; }
        [Display(Name = "MrpPlanMaster_CreateDate", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "MrpPlanMaster_IsRelease", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public bool IsRelease { get; set; }

        #endregion


    }

}
