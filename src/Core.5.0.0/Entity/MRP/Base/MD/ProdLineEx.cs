using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class ProdLineEx : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        /// <summary>
        /// 生产线
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_ProductLine", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string ProductLine { get; set; }
        /// <summary>
        /// 物料号
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_Item", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string Item { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_StartDate", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public DateTime StartDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_EndDate", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public DateTime EndDate { get; set; }

        [Display(Name = "ProdLineEx_Region", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string Region { get; set; }
        /// <summary>
        /// 排产速度
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_MrpSpeed", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public Double MrpSpeed { get; set; }

        /// <summary>
        /// 工艺速度
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_RccpSpeed", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public Double RccpSpeed { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_ApsPriority", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public com.Sconit.CodeMaster.ApsPriorityType ApsPriority { get; set; }

        /// <summary>
        /// 废品率
        /// </summary>
        //public double ScrapPercent { get; set; }

        /// <summary>
        /// 配额
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_Quota", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public double Quota { get; set; }

        ///// <summary>
        ///// 设置时间
        ///// </summary>
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        //[Display(Name = "ProdLineEx_SetupTime", ResourceType = typeof(Resources.CUST.ProdLineEx))]
        //public Double SetupTime { get; set; }
        /// <summary>
        /// 切换时间
        /// </summary>

        [Display(Name = "ProdLineEx_SwichTime", ResourceType = typeof(Resources.MRP.ProdLineEx))]
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
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_SpeedTimes", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public Int32 SpeedTimes { get; set; }

        /// <summary>
        /// 最小批量
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_MinLotSize", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public double MinLotSize { get; set; }
        /// <summary>
        /// 经济批量
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_EconomicLotSize", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public double EconomicLotSize { get; set; }
        /// <summary>
        /// 最大批量
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_MaxLotSize", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public double MaxLotSize { get; set; }
        /// <summary>
        /// 切换倍数
        /// </summary>
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_TurnQty", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public Int32 TurnQty { get; set; }
        /// <summary>
        /// 修正因子
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_Correction", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public double Correction { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_ShiftType", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_ProductType", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string ProductType { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        #endregion

    }

}
