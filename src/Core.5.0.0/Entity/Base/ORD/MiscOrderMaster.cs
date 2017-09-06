using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class MiscOrderMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        /// <summary>
        /// //计划外出入库单号
        /// </summary>
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 10)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 10)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 10)]
        [Export(ExportName = "Master", ExportSeq = 10)]
        [Display(Name = "MiscOrderMstr_MiscOrderNo", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string MiscOrderNo { get; set; }

        /// <summary>
        /// //计划外出入库类型
        /// </summary>
        [Display(Name = "MiscOrderMstr_Type", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public com.Sconit.CodeMaster.MiscOrderType Type { get; set; }

        public com.Sconit.CodeMaster.MiscOrderSubType SubType { get; set; }


        /// <summary>
        /// //状态
        /// </summary>
        [Display(Name = "MiscOrderMstr_Status", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public com.Sconit.CodeMaster.MiscOrderStatus Status { get; set; }

        /// <summary>
        /// //是否扫描条码
        /// </summary>
        [Display(Name = "MiscOrderMstr_IsScanHu", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public Boolean IsScanHu { get; set; }

        /// <summary>
        /// //质量状态
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_QualityType", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public CodeMaster.QualityType QualityType { get; set; }

        /// <summary>
        /// //移动类型
        /// </summary>
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 50)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "Master", ExportSeq = 40)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_MoveType", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string MoveType { get; set; }

        /// <summary>
        /// //冲销时的移动类型
        /// </summary>
        public string CancelMoveType { get; set; }

        /// <summary>
        /// //供货工厂对应的区域，跨工厂移库时代表出库区域
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_Region", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Region { get; set; }

        /// <summary>
        ///  //库存地点对应的库位
        /// </summary>
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 40)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "Master", ExportSeq = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_Location", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]              
        public string Location { get; set; }

        /// <summary>
        /// //收货地点，为SAP库位不用翻译
        /// </summary>
        [Display(Name = "MiscOrderMstr_ReceiveLocation", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string ReceiveLocation { get; set; }

        /// <summary>
        /// //移动原因
        /// </summary>
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 70)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 60)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 70)]
        [Export(ExportName = "Master", ExportSeq = 80)]
        [Display(Name = "MiscOrderMstr_Note", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Note { get; set; }

        [Display(Name = "MiscOrderMstr_GeneralLedger", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string GeneralLedger { get; set; }

        /// <summary>
        /// //成本中心
        /// </summary>
        [Export(ExportName = "Master", ExportSeq = 60)]
        [Display(Name = "MiscOrderMstr_CostCenter", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string CostCenter { get; set; }

        /// <summary>
        /// //出货通知
        /// </summary>
        [Display(Name = "MiscOrderMstr_Asn", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Asn { get; set; }

        /// <summary>
        /// //内部订单号
        /// </summary>
        [Display(Name = "MiscOrderMstr_ReferenceNo", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// 跨工厂移库时,代表收货工厂
        /// 工厂代码，为SAP代码，非跨工厂移库时，会从Region字段找到对应的Plant填到该字段上面
        /// </summary>
        [Display(Name = "MiscOrderMstr_DeliverRegion", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string DeliverRegion { get; set; }

        /// <summary>
        /// WBS元素
        /// </summary>
        [Display(Name = "MiscOrderMstr_WBS", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string WBS { get; set; }
        public string WBSRow { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 20)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 20)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 20)]
        [Export(ExportName = "Master", ExportSeq = 20)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_EffectiveDate", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public DateTime EffectiveDate { get; set; }

        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 80)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 70)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 80)]
        [Export(ExportName = "Master", ExportSeq = 90)]
        [Display(Name = "MiscOrderMstr_CreateUserName", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string CreateUserName { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 90)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 80)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 90)]
        [Export(ExportName = "Master", ExportSeq = 100)]
        [Display(Name = "MiscOrderMstr_CreateDate", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public DateTime CreateDate { get; set; }

        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public Int32 LastModifyUserId { get; set; }

        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string LastModifyUserName { get; set; }

        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public DateTime LastModifyDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public Int32? CloseUserId { get; set; }

        public string CloseUserName { get; set; }

        public DateTime? CancelDate { get; set; }

        public Int32? CancelUserId { get; set; }

        public string CancelUserName { get; set; }

        //[Display(Name = "Version", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public Int32 Version { get; set; }
        public string WMSNo { get; set; }
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 60, ExportTitle = "OrderMaster_Flow_Production", ExportTitleResourceType = typeof(@Resources.ORD.OrderMaster))]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 50,ExportTitle = "OrderMaster_Flow_Production", ExportTitleResourceType = typeof(@Resources.ORD.OrderMaster))]
        [Display(Name = "MiscOrderMstr_Flow", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Flow { get; set; }
        [Export(ExportName = "Master", ExportSeq = 70)]
        [Display(Name = "MiscOrderMstr_IsCs", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public bool IsCs { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (MiscOrderNo != null)
            {
                return MiscOrderNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MiscOrderMaster another = obj as MiscOrderMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.MiscOrderNo == another.MiscOrderNo);
            }
        }
    }

}
