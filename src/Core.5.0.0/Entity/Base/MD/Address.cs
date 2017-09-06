using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class Address : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Address_Code", ResourceType = typeof(Resources.MD.Address))]
        public string Code { get; set; }

        //[Display(Name = "Address_Type", ResourceType = typeof(Resources.MD.Address))]
        //public com.Sconit.CodeMaster.AddressType Type { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(256, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Address_Address", ResourceType = typeof(Resources.MD.Address))]
        public string AddressContent { get; set; }


        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Address_PostCode", ResourceType = typeof(Resources.MD.Address))]
        public string PostCode { get; set; }


        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Address_TelPhone", ResourceType = typeof(Resources.MD.Address))]
        [RegularExpression(@"^((0\d{2,5}-)|\(0\d{2,5}\))?\d{7,8}(-\d{3,4})?$", ErrorMessage = "电话格式有误!")]
        public string TelPhone { get; set; }


        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Address_MobilePhone", ResourceType = typeof(Resources.MD.Address))]
        //[RegularExpression(@"^\d{11}$",ErrorMessage="手机号码格式有误!")]
        public string MobilePhone { get; set; }


        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Address_Fax", ResourceType = typeof(Resources.MD.Address))]
        public string Fax { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Address_Email", ResourceType = typeof(Resources.MD.Address))]
        [RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$", ErrorMessage = "请输入正确的Email格式\n示例：abc@123.com")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Address_ContactPsnNm", ResourceType = typeof(Resources.MD.Address))]
        public string ContactPersonName { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            Address another = obj as Address;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Code == another.Code);
            }
        }
    }

}
