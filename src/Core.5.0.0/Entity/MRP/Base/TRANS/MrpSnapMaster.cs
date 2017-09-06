using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpSnapMaster : EntityBase
    {
        [Display(Name = "MrpSnapMaster_SnapTime", ResourceType = typeof(Resources.MRP.MrpSnapMaster))]
        public DateTime SnapTime { get; set; }//Ö÷¼ü
        [Display(Name = "MrpSnapMaster_Status", ResourceType = typeof(Resources.MRP.MrpSnapMaster))]
        public CodeMaster.MessageType Status { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "MrpSnapMaster_CreateUserName", ResourceType = typeof(Resources.MRP.MrpSnapMaster))]
        public string CreateUserName { get; set; }
        [Display(Name = "MrpSnapMaster_CreateDate", ResourceType = typeof(Resources.MRP.MrpSnapMaster))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "MrpSnapMaster_IsRelease", ResourceType = typeof(Resources.MRP.MrpSnapMaster))]
        public bool IsRelease { get; set; }

        public CodeMaster.SnapType Type { get; set; }

        public override int GetHashCode()
        {
            if (SnapTime != null)
            {
                return SnapTime.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MrpSnapMaster another = obj as MrpSnapMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.SnapTime == another.SnapTime);
            }
        }
    }
}
