using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class SubPrintOrder : EntityBase, IAuditable
    {
        public int Id { get; set; }
        [Display(Name = "UserId", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public int UserId { get; set; }
        [Display(Name = "Region", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string Region { get; set; }
        [Display(Name = "Flow", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string Flow { get; set; }
        [Display(Name = "Location", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string Location { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ExcelTemplate", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string ExcelTemplate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Printer", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string Printer { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Client", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string Client { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "CreateUserName", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public string CreateUserName { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(Resources.CUST.SubPrintOrder))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
    }
}
