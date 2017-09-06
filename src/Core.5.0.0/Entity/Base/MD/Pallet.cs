using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class Pallet : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "Pallet_Code", ResourceType = typeof(Resources.INV.Pallet))]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Pallet_Description", ResourceType = typeof(Resources.INV.Pallet))]
        public string Description { get; set; }

        [Display(Name = "Pallet_Volume", ResourceType = typeof(Resources.INV.Pallet))]
        public Decimal Volume { get; set; }

        [Display(Name = "Pallet_Weight", ResourceType = typeof(Resources.INV.Pallet))]
        public Decimal Weight { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "Pallet_CreateUserName", ResourceType = typeof(Resources.INV.Pallet))]
        public string CreateUserName { get; set; }

        [Display(Name = "Pallet_CreateDate", ResourceType = typeof(Resources.INV.Pallet))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "Pallet_LastModifyUserName", ResourceType = typeof(Resources.INV.Pallet))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "Pallet_LastModifyDate", ResourceType = typeof(Resources.INV.Pallet))]
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
            Pallet another = obj as Pallet;

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
