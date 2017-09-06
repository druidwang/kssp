using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class PickUser : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        [Display(Name = "PickUser_PickGroupCode", ResourceType = typeof(Resources.WMS.PickUser))]
        public string PickGroupCode { get; set; }
        public Int32? PickUserId { get; set; }

        [Display(Name = "PickUser_PickUserName", ResourceType = typeof(Resources.WMS.PickUser))]
        public string PickUserName { get; set; }

        [Display(Name = "PickUser_IsActive", ResourceType = typeof(Resources.WMS.PickUser))]
        public Boolean IsActive { get; set; }
        public Int32 CreateUserId { get; set; }

        [Display(Name = "PickUser_CreateUserName", ResourceType = typeof(Resources.WMS.PickUser))]
        public string CreateUserName { get; set; }

        [Display(Name = "PickUser_CreateDate", ResourceType = typeof(Resources.WMS.PickUser))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "PickUser_LastModifyUserName", ResourceType = typeof(Resources.WMS.PickUser))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "PickUser_LastModifyDate", ResourceType = typeof(Resources.WMS.PickUser))]
        public DateTime LastModifyDate { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            PickUser another = obj as PickUser;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }

}
