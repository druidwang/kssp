using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class WorkCalendar : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "MrpWorkCalendar_ProductLine", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public string Flow { get; set; }
        [Display(Name = "MrpWorkCalendar_DateIndex", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public string DateIndex { get; set; }
        [Display(Name = "MrpWorkCalendar_DateType", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public CodeMaster.TimeUnit DateType { get; set; }

        /// <summary>
        /// 试制时间
        /// </summary>
        [Display(Name = "MrpWorkCalendar_TrialTime", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public Double TrialTime { get; set; }
        /// <summary>
        /// 停机时间
        /// </summary>
        [Display(Name = "MrpWorkCalendar_HaltTime", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public Double HaltTime { get; set; }
        /// <summary>
        /// 节假日
        /// </summary>
        [Display(Name = "MrpWorkCalendar_Holiday", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public Double Holiday { get; set; }
        /// <summary>
        /// 可用时间
        /// </summary>
        [Display(Name = "MrpWorkCalendar_UpTime", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public double UpTime { get; set; }
        /// <summary>
        /// 已锁定
        /// </summary>
        [Display(Name = "MrpWorkCalendar_IsLock", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public bool IsLock { get; set; }

        public CodeMaster.ResourceGroup ResourceGroup { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (Flow != null && DateIndex != null && (int)DateType != 0)
            {
                return Flow.GetHashCode()
                    ^ DateIndex.GetHashCode()
                    ^ DateType.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            WorkCalendar another = obj as WorkCalendar;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Flow == another.Flow)
                    && (this.DateIndex == another.DateIndex)
                    && (this.DateType == another.DateType);
            }
        }
    }

}
