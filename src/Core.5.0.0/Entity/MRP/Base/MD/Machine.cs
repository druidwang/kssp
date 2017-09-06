using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class Machine : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        public int Id { get; set; }

        /// <summary>
        /// ģ��code
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_Code", ResourceType = typeof(Resources.MRP.Machine))]
        public string Code { get; set; }
        [Display(Name = "Machine_StartDate", ResourceType = typeof(Resources.MRP.Machine))]
        public DateTime StartDate { get; set; }
        [Display(Name = "Machine_EndDate", ResourceType = typeof(Resources.MRP.Machine))]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_Description", ResourceType = typeof(Resources.MRP.Machine))]
        public string Description { get; set; }


        //�������(8Сʱ)
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_ShiftQuota", ResourceType = typeof(Resources.MRP.Machine))]
        public Double ShiftQuota { get; set; }
        //����
        //[Display(Name = "Machine_Capacity", ResourceType = typeof(Resources.CUST.Machine))]
        //public Double Capacity { get; set; }

        //����
        [Range(0.000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_Qty", ResourceType = typeof(Resources.MRP.Machine))]
        public double Qty { get; set; }
        //����
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_Island", ResourceType = typeof(Resources.MRP.Machine))]
        public string Island { get; set; }

        //ģ������
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_MachineType", ResourceType = typeof(Resources.MRP.Machine))]
        public com.Sconit.CodeMaster.MachineType MachineType { get; set; }

        //���� 2��/3��

        [Display(Name = "Machine_ShiftType", ResourceType = typeof(Resources.MRP.Machine))]
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }

        //��������������
        [Range(0.01, 7, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_NormalWorkDayPerWeek", ResourceType = typeof(Resources.MRP.Machine))]
        public double NormalWorkDayPerWeek { get; set; }

        //�����������
        [Range(0.01, 7, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_MaxWorkDayPerWeek", ResourceType = typeof(Resources.MRP.Machine))]
        public double MaxWorkDayPerWeek { get; set; }

        //���/��
        [Range(1, 3, ErrorMessageResourceName = "Errors_Common_FieldLengthGreaterLess", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Machine_ShiftPerDay", ResourceType = typeof(Resources.MRP.Machine))]
        public Int32 ShiftPerDay { get; set; }


        public Int32 CreateUserId { get; set; }
        [Display(Name = "Machine_CreateUserName", ResourceType = typeof(Resources.MRP.Machine))]
        public string CreateUserName { get; set; }
        [Display(Name = "Machine_CreateDate", ResourceType = typeof(Resources.MRP.Machine))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        public int Seq { get; set; }
        #endregion
    }

}
