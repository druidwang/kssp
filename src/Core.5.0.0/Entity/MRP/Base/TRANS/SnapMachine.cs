using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class SnapMachine : EntityBase
    {
        #region O/R Mapping Properties

        public int Id { get; set; }
        [Display(Name = "ProdLineExInstance_SnapTime", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public DateTime SnapTime { get; set; }

        /// <summary>
        /// ģ��code
        /// </summary>
        [Export(ExportName = "FICalendar", ExportSeq = 10)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_Code", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string Code { get; set; }

        /// <summary>
        /// ʱ������ ��:2012-01 ��:2012-49 ��:2012-01-12
        /// </summary>
        [Export(ExportName = "FICalendar", ExportSeq = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_DateIndex", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string DateIndex { get; set; }

        /// <summary>
        /// �ƻ�����:��/��/��
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_DateType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public CodeMaster.TimeUnit DateType { get; set; }
        [Export(ExportName = "FICalendar", ExportSeq = 20)]
        [Display(Name = "MachineInstance_Region", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string Region { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Export(ExportName = "FICalendar", ExportSeq = 60)]
        [Display(Name = "MachineInstance_Description", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string Description { get; set; }

        //�������(8Сʱ)
        [Export(ExportName = "FICalendar", ExportSeq = 70)]
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_ShiftQuota", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double ShiftQuota { get; set; }

        //����
        [Export(ExportName = "FICalendar", ExportSeq = 80)]
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_Qty", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double Qty { get; set; }
        //����
        [Export(ExportName = "FICalendar", ExportSeq = 90)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_Island", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string Island { get; set; }

        //ģ������
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_MachineType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public com.Sconit.CodeMaster.MachineType MachineType { get; set; }

        //���� 2��/3��
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_ShiftType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }

        //��������������
        [Export(ExportName = "FICalendar", ExportSeq = 110)]
        [Range(1, 7, ErrorMessageResourceName = "Errors_Common_FieldEqualsTo", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_NormalWorkDayPerWeek", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double NormalWorkDayPerWeek { get; set; }

        //�����������
        [Export(ExportName = "FICalendar", ExportSeq = 120)]
        [Range(1, 7, ErrorMessageResourceName = "Errors_Common_FieldEqualsTo", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_MaxWorkDayPerWeek", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double MaxWorkDayPerWeek { get; set; }

        //���/��
        [Export(ExportName = "FICalendar", ExportSeq = 130)]
        [Range(1, 3, ErrorMessageResourceName = "Errors_Common_FieldLengthGreaterLess", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_ShiftPerDay", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public int ShiftPerDay { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [Range(1, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MachineInstance_IslandQty", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public Double IslandQty { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [Display(Name = "MachineInstance_IslandDescription", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string IslandDescription { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public Double TrailTime { get; set; }
        /// <summary>
        /// ͣ��ʱ��
        /// </summary>
        public Double HaltTime { get; set; }
        /// <summary>
        /// �ڼ���
        /// </summary>
        public Double Holiday { get; set; }

        //������
        public string Flow { get; set; }

        #endregion
    }
}
