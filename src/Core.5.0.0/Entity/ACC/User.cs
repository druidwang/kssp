using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.VIEW;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ACC
{
    public partial class User : IIdentity
    {
        public User()
        {
        }

        public User(Int32 id)
        {
            this.Id = id;
        }

        #region Non O/R Mapping Properties

        [Display(Name = "User_FullName", ResourceType = typeof(Resources.ACC.User))]
        public string FullName
        {
            get
            {
                if (LastName == null)
                {
                    return FirstName;
                }
                else
                {
                    return FirstName + LastName;
                }
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthNotInRange", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "User_NewPassword", ResourceType = typeof(Resources.ACC.User))]
        public string NewPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [DataType(DataType.Password)]
        [Display(Name = "User_ConfirmNewPassword", ResourceType = typeof(Resources.ACC.User))]
        public string ConfirmPassword { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.Language, ValueField = "Language")]
        [Display(Name = "User_Language", ResourceType = typeof(Resources.ACC.User))]
        public string LanguageDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.UserType, ValueField = "Type")]
        [Display(Name = "User_Type", ResourceType = typeof(Resources.ACC.User))]
        public string UserTypeDescription { get; set; }

        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.FullName))
                {
                    return this.Code;
                }
                else
                {
                    return this.Code + " [" + this.FullName + "]";
                }
            }
        }

        public IList<UserPermissionView> Permissions { get; set; }

        public IList<string> UrlPermissions
        {
            get
            {

                if (Permissions != null && Permissions.Count > 0)
                {
                    return (from p in this.Permissions
                            where p.PermissionCategoryType == com.Sconit.CodeMaster.PermissionCategoryType.Url
                            || p.PermissionCategoryType == com.Sconit.CodeMaster.PermissionCategoryType.Menu
                            select p.PermissionCode).ToList();
                }

                return null;
            }
        }

        public IList<string> MenuPermissions
        {
            get
            {
                if (Permissions != null && Permissions.Count > 0)
                {
                    return (from p in this.Permissions
                            where p.PermissionCategoryType == com.Sconit.CodeMaster.PermissionCategoryType.Menu
                            || p.PermissionCategoryType == com.Sconit.CodeMaster.PermissionCategoryType.SI
                            select p.PermissionCode).ToList();
                }

                return null;
            }
        }

        public IList<string> SupplierPersmissions
        {
            get
            {

                if (Permissions != null && Permissions.Count > 0)
                {
                    return (from p in this.Permissions
                            where p.PermissionCategoryType == com.Sconit.CodeMaster.PermissionCategoryType.Supplier
                            select p.PermissionCode).ToList();
                }

                return null;
            }
        }

        public IList<string> CustomerPermissions
        {
            get
            {

                if (Permissions != null && Permissions.Count > 0)
                {
                    return (from p in this.Permissions
                            where p.PermissionCategoryType == com.Sconit.CodeMaster.PermissionCategoryType.Customer
                            select p.PermissionCode).ToList();
                }

                return null;
            }
        }

        public IList<string> CarrierPermissions
        {
            get
            {

                if (Permissions != null && Permissions.Count > 0)
                {
                    return (from p in this.Permissions
                            where p.PermissionCategoryType == com.Sconit.CodeMaster.PermissionCategoryType.Carrier
                            select p.PermissionCode).ToList();
                }

                return null;
            }
        }

        public IList<string> RegionPermissions
        {
            get
            {
                if (Permissions != null && Permissions.Count > 0)
                {
                    return (from p in this.Permissions
                            where p.PermissionCategoryType == com.Sconit.CodeMaster.PermissionCategoryType.Region
                            select p.PermissionCode).ToList();
                }
                return null;
            }
        }

        public IList<CodeMaster.OrderType> OrderTypePermissions
        {
            get
            {
                if (Permissions != null && Permissions.Count > 0)
                {
                    IList<CodeMaster.OrderType> orderTypes = new List<CodeMaster.OrderType>();
                    foreach (var permisssion in this.Permissions)
                    {
                        switch (permisssion.PermissionCode)
                        {
                            case "OrderType_Procurement":
                                orderTypes.Add(CodeMaster.OrderType.Procurement);
                                break;
                            case "OrderType_Transfer":
                                orderTypes.Add(CodeMaster.OrderType.Transfer);
                                break;
                            case "OrderType_Distribution":
                                orderTypes.Add(CodeMaster.OrderType.Distribution);
                                break;
                            case "OrderType_Production":
                                orderTypes.Add(CodeMaster.OrderType.Production);
                                break;
                            case "OrderType_SubContract":
                                orderTypes.Add(CodeMaster.OrderType.SubContract);
                                break;
                            case "OrderType_CustomerGoods":
                                orderTypes.Add(CodeMaster.OrderType.CustomerGoods);
                                break;
                            case "OrderType_SubContractTransfer":
                                orderTypes.Add(CodeMaster.OrderType.SubContractTransfer);
                                break;
                            case "OrderType_ScheduleLine":
                                orderTypes.Add(CodeMaster.OrderType.ScheduleLine);
                                break;
                            default:
                                break;
                        }
                    }
                    return orderTypes;
                }
                return null;
            }
        }

        public bool HasEmail
        {
            get
            {
                if (string.IsNullOrEmpty(this.Email))
                {
                    return false;
                }
                return true;
            }
        }

        public bool HasMobilePhone
        {
            get
            {
                if (string.IsNullOrEmpty(this.MobilePhone))
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region IIdentity Implement
        public string AuthenticationType
        {
            get
            {

                //throw new NotImplementedException();
                return string.Empty;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                //throw new NotImplementedException(); 
                //always return true;
                return true;
            }
        }

        public string Name
        {
            get { return Code; }
        }
        #endregion


        [Display(Name = "User_LastLoginDate", ResourceType = typeof(Resources.ACC.User))]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "User_LastIpAddress", ResourceType = typeof(Resources.ACC.User))]
        public string LastIpAddress { get; set; }
    }
}