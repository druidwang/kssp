using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class Island : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        /// <summary>
        /// 岛区code
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Island_Code", ResourceType = typeof(Resources.MRP.Island))]
        public string Code { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Island_Description", ResourceType = typeof(Resources.MRP.Island))]
        public string Description { get; set; }

        //数量
        [Range(0.000000000001, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Island_Qty", ResourceType = typeof(Resources.MRP.Island))]
        public Double Qty { get; set; }

        //区域
        [Display(Name = "Island_Region", ResourceType = typeof(Resources.MRP.Island))]
        public string Region { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "Island_CreateUserName", ResourceType = typeof(Resources.MRP.Island))]
        public string CreateUserName { get; set; }
        [Display(Name = "Island_CreateDate", ResourceType = typeof(Resources.MRP.Island))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "Island_IsActive", ResourceType = typeof(Resources.MRP.Island))]
        public bool IsActive { get; set; }
        [Range(0, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        public int Seq { get; set; }
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
            Island another = obj as Island;

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
