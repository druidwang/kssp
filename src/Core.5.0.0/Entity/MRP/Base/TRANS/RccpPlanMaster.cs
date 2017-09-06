using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class RccpPlanMaster : EntityBase
    {
        [Display(Name = "RccpPlanMaster_PlanVersion", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public DateTime PlanVersion { get; set; }//Ö÷¼ü
        [Display(Name = "RccpPlanMaster_SnapTime", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public DateTime SnapTime { get; set; }
        [Display(Name = "RccpPlanMaster_Status", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public CodeMaster.MessageType Status { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "RccpPlanMaster_CreateUserName", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public string CreateUserName { get; set; }
        [Display(Name = "RccpPlanMaster_CreateDate", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public DateTime CreateDate { get; set; }
        [Display(Name = "RccpPlanMaster_DateType", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public CodeMaster.TimeUnit DateType { get; set; }

        [Display(Name = "RccpPlanMaster_RccpPlanVersion", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public int RccpPlanVersion { get; set; }

        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
         [Display(Name = "RccpPlanMaster_IsRelease", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public bool IsRelease { get; set; }
    }
}
