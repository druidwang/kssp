﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;

namespace com.Sconit.Web.Models
{
    /// <summary>
    /// Summary description for AccountModel
    /// </summary>

    public class ChangePasswordModel : BaseModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthNotInRange", ErrorMessageResourceType = typeof(Resources.ErrorMessage), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "User_NewPassword", ResourceType = typeof(Resources.ACC.User))]
        public string NewPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.ErrorMessage))]
        [DataType(DataType.Password)]
        [Display(Name = "User_ConfirmNewPassword", ResourceType = typeof(Resources.ACC.User))]
        [Compare("NewPassword", ErrorMessageResourceName = "Errors_New_Password_And_Confirm_Password_NotEq", ErrorMessageResourceType = typeof(Resources.ErrorMessage))]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel : BaseModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.ErrorMessage))]
        [Display(Name = "User_Code", ResourceType = typeof(Resources.ACC.User))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.ErrorMessage))]
        [DataType(DataType.Password)]
        [Display(Name = "User_Password", ResourceType = typeof(Resources.ACC.User))]
        public string Password { get; set; }

        public string HashedPassword { get { return FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5"); } }

        [Display(Name = "User_RememberMe", ResourceType = typeof(Resources.ACC.User))]
        public bool RememberMe { get; set; }
    }
}