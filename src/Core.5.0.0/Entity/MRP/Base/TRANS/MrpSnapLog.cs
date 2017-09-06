using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpSnapLog
    {
        public int Id { get; set; }
        [Display(Name = "MrpSnapLog_Level", ResourceType = typeof(Resources.MRP.MrpSnapLog))]
        public string ErrorLevel { get; set; }
        [Display(Name = "MrpSnapLog_SnapTime", ResourceType = typeof(Resources.MRP.MrpSnapLog))]
        public DateTime SnapTime { get; set; }
        [Display(Name = "MrpSnapLog_Logger", ResourceType = typeof(Resources.MRP.MrpSnapLog))]
        public string Logger { get; set; }
        [Display(Name = "MrpSnapLog_Message", ResourceType = typeof(Resources.MRP.MrpSnapLog))]
        public string Message { get; set; }

        public override int GetHashCode()
        {
            if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MrpSnapLog another = obj as MrpSnapLog;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }


}
