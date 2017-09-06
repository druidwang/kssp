using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class SnapProdLineEx : EntityBase
    {
        #region O/R Mapping Properties

        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_SnapTime", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public DateTime SnapTime { get; set; }

        [Export(ExportName = "EXCalendar", ExportSeq = 10)]
        [Display(Name = "ProdLineExInstance_Region", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string Region { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 20)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_ProductLine", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string ProductLine { get; set; }

        /// <summary>
        /// ���Ϻ�
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_Item", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string Item { get; set; }

        /// <summary>
        /// ʱ������ ��:2012-01 ��:2012-49 ��:2012-01-12
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 40)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_DateIndex", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string DateIndex { get; set; }

        /// <summary>
        /// �ƻ�����:��/��/��
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_DateType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public CodeMaster.TimeUnit DateType { get; set; }

        /// <summary>
        /// �����ٶ�
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 80)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_RccpSpeed", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Double RccpSpeed { get; set; }
        /// <summary>
        /// �Ų��ٶ�
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 70)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_MrpSpeed", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Double MrpSpeed { get; set; }

        /// <summary>
        /// ���ȼ�
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_ApsPriority", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public com.Sconit.CodeMaster.ApsPriorityType ApsPriority { get; set; }
 
        /// <summary>
        /// �л�ʱ��
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_SwichTime", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Double SwitchTime { get; set; }

        /// <summary>
        /// ǻ����
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 100)]
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_SpeedTimes", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Int32 SpeedTimes { get; set; }


        /// <summary>
        /// ���
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 110)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_Quota", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double Quota { get; set; }
        /// <summary>
        /// ��С����
        /// </summary>
        [Export(ExportName = "EXCalendar", ExportSeq = 120)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_MinLotSize", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double MinLotSize { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_EconomicLotSize", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double EconomicLotSize { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_MaxLotSize", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double MaxLotSize { get; set; }
        /// <summary>
        /// �л�����
        /// </summary>
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_TurnQty", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public Int32 TurnQty { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_Correction", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public double Correction { get; set; }

        //���� 2��/3��
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineExInstance_ShiftType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }


        [Display(Name = "ProdLineExInstance_ProductType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string ProductType { get; set; }

        #endregion
    }

}
