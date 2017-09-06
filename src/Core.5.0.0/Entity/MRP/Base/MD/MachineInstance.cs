using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class MachineInstance : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        /// <summary>
        /// 模具code
        /// </summary>
        [Export(ExportName = "FICalendar", ExportSeq = 10)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_Code", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string Code { get; set; }

        /// <summary>
        /// 时间索引 月:2012-01 周:2012-49 日:2012-01-12
        /// </summary>
        [Export(ExportName = "FICalendar", ExportSeq = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_DateIndex", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string DateIndex { get; set; }

        /// <summary>
        /// 计划类型:月/周/天
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_DateType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public CodeMaster.TimeUnit DateType { get; set; }
        [Export(ExportName = "FICalendar", ExportSeq = 20)]
        [Display(Name = "MachineInstance_Region", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string Region { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Export(ExportName = "FICalendar", ExportSeq = 60)]
        [Display(Name = "MachineInstance_Description", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string Description { get; set; }

        //班产定额(8小时)
        [Export(ExportName = "FICalendar", ExportSeq = 70)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_ShiftQuota", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double ShiftQuota { get; set; }

        //数量
        [Export(ExportName = "FICalendar", ExportSeq = 80)]
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_Qty", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double Qty { get; set; }
        //岛区
        [Export(ExportName = "FICalendar", ExportSeq = 90)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_Island", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string Island { get; set; }

        //模具类型
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_MachineType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public com.Sconit.CodeMaster.MachineType MachineType { get; set; }

        //班制 2班/3班
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_ShiftType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }

        //周正常工作天数
        [Export(ExportName = "FICalendar", ExportSeq = 110)]
        [Range(1, 7, ErrorMessageResourceName = "Errors_Common_FieldEqualsTo", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_NormalWorkDayPerWeek", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double NormalWorkDayPerWeek { get; set; }

        //周最大工作天数
        [Export(ExportName = "FICalendar", ExportSeq = 120)]
        [Range(1, 7, ErrorMessageResourceName = "Errors_Common_FieldEqualsTo", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_MaxWorkDayPerWeek", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double MaxWorkDayPerWeek { get; set; }

        //班次/天
        [Export(ExportName = "FICalendar", ExportSeq = 130)]
        [Range(1, 3, ErrorMessageResourceName = "Errors_Common_FieldLengthGreaterLess", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_ShiftPerDay", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public int ShiftPerDay { get; set; }
        /// <summary>
        /// 已释放
        /// </summary>
        [Display(Name = "MachineInstance_IsRelease", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public bool IsRelease { get; set; }
        /// <summary>
        /// 手工
        /// </summary>
        [Display(Name = "MachineInstance_IsManualCreate", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public bool IsManualCreate { get; set; }
        /// <summary>
        /// 岛区数量
        /// </summary>
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_IslandQty", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double IslandQty { get; set; }
        /// <summary>
        /// 岛区描述
        /// </summary>
        [Display(Name = "MachineInstance_IslandDescription", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string IslandDescription { get; set; }


        /// <summary>
        /// 试制时间
        /// </summary>
        public Double TrailTime { get; set; }
        /// <summary>
        /// 停机时间
        /// </summary>
        public Double HaltTime { get; set; }
        /// <summary>
        /// 节假日
        /// </summary>
        public Double Holiday { get; set; }

        //生产线
        public string Flow { get; set; }

        public Int32 CreateUserId { get; set; }
        // [Display(Name = "Machine_CreateUserName", ResourceType = typeof(Resources.CUST.Machine))]
        public string CreateUserName { get; set; }
        // [Display(Name = "Machine_CreateDate", ResourceType = typeof(Resources.CUST.Machine))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (Code != null && DateIndex != null && (int)DateType != 0)
            {
                return Code.GetHashCode()
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
            MachineInstance another = obj as MachineInstance;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Code == another.Code)
                    && (this.DateIndex == another.DateIndex)
                    && (this.DateType == another.DateType);
            }
        }
    }

}
