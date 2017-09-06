using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class HuTo : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        /// <summary>
        /// …Ë±∏code
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "HuTo_Code", ResourceType = typeof(Resources.MRP.HuTo))]
        public string Code { get; set; }
        /// <summary>
        /// √Ë ˆ
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "HuTo_Description", ResourceType = typeof(Resources.MRP.HuTo))]
        public string Description { get; set; }


        public Int32 CreateUserId { get; set; }
        [Display(Name = "HuTo_CreateUserName", ResourceType = typeof(Resources.MRP.HuTo))]
        public string CreateUserName { get; set; }
        [Display(Name = "HuTo_CreateDate", ResourceType = typeof(Resources.MRP.HuTo))]
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
            HuTo another = obj as HuTo;

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
