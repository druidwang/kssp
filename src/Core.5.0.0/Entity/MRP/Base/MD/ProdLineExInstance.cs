using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class ProdLineExInstance : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Export(ExportName = "EXCalendar", ExportSeq = 10)]
        [Display(Name = "ProdLineExInstance_Region", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string Region { get; set; }
        /// <summary>
        /// 生产线
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 20)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_ProductLine", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string ProductLine { get; set; }

        /// <summary>
        /// 物料号
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_Item", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string Item { get; set; }


        /// <summary>
        /// 时间索引 月:2012-01 周:2012-49 日:2012-01-12
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 40)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_DateIndex", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string DateIndex { get; set; }

        /// <summary>
        /// 计划类型:月/周/天
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_DateType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public CodeMaster.TimeUnit DateType { get; set; }

        /// <summary>
        /// 工艺速度
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 80)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_RccpSpeed", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Double RccpSpeed { get; set; }
        /// <summary>
        /// 排产速度
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 70)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_MrpSpeed", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Double MrpSpeed { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_ApsPriority", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public com.Sconit.CodeMaster.ApsPriorityType ApsPriority { get; set; }
        ///// <summary>
        ///// 设置时间
        ///// </summary>
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        //[Display(Name = "ProdLineEx_SetupTime", ResourceType = typeof(Resources.CUST.ProdLineEx))]
        //public Double SetupTime { get; set; }
        /// <summary>
        /// 切换时间
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_SwichTime", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Double SwitchTime { get; set; }
        ///// <summary>
        ///// 维护时间
        ///// </summary>
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        //[Display(Name = "ProdLineEx_MaintenanceTime", ResourceType = typeof(Resources.CUST.ProdLineEx))]
        //public Double MaintenanceTime { get; set; }
        /// <summary>
        /// 腔口数
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 100)]
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_SpeedTimes", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Int32 SpeedTimes { get; set; }

        // /// <summary>
        // /// 废品率
        // /// </summary>
        //[Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        //[Display(Name = "ProdLineExInstance_ScrapPercent", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
         //public double ScrapPercent { get; set; }

        /// <summary>
        /// 配额
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 110)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_Quota", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double Quota { get; set; }
        /// <summary>
        /// 最小批量
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 120)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_MinLotSize", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double MinLotSize { get; set; }
        /// <summary>
        /// 经济批量
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_EconomicLotSize", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double EconomicLotSize { get; set; }
        /// <summary>
        /// 最大批量
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_MaxLotSize", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double MaxLotSize { get; set; }
        /// <summary>
        /// 切换倍数
        /// </summary>
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_TurnQty", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Int32 TurnQty { get; set; }
        /// <summary>
        /// 修正因子
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_Correction", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double Correction { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        /// <summary>
        /// 已释放
        /// </summary>
        [Display(Name = "ProdLineExInstance_IsRelease", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public bool IsRelease { get; set; }
        /// <summary>
        /// 手工
        /// </summary>
        [Display(Name = "ProdLineExInstance_IsManualCreate", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public bool IsManualCreate { get; set; }

        //班制 2班/3班
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_ShiftType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }


        [Display(Name = "ProdLineExInstance_ProductType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string ProductType { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (ProductLine != null && Item != null && DateIndex != null && (int)DateType != 0)
            {
                return ProductLine.GetHashCode()
                    ^ Item.GetHashCode()
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
            ProdLineExInstance another = obj as ProdLineExInstance;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.ProductLine == another.ProductLine)
                    && (this.Item == another.Item)
                    && (this.DateIndex == another.DateIndex)
                    && (this.DateType == another.DateType);
            }
        }
    }

}
