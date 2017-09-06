using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class RccpLog
    {
        public int Id { get; set; }
       [Display(Name = "RccpLog_ErrorLevel", ResourceType = typeof(Resources.MRP.RccpLog))]
        public string ErrorLevel { get; set; }
        [Display(Name = "RccpLog_SnapTime", ResourceType = typeof(Resources.MRP.RccpLog))]
        public DateTime PlanVersion { get; set; }
        [Display(Name = "RccpLog_Logger", ResourceType = typeof(Resources.MRP.RccpLog))]
        public string Logger { get; set; }
        [Display(Name = "RccpLog_Message", ResourceType = typeof(Resources.MRP.RccpLog))]
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
            RccpLog another = obj as RccpLog;

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
