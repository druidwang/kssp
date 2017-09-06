using System;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Entity.VIEW
{
    [Serializable]
    public partial class UserPermissionView : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 UserId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionCategory { get; set; }
        public com.Sconit.CodeMaster.PermissionCategoryType PermissionCategoryType { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (UserId != 0 && PermissionCode != null && PermissionCategory != null)
            {
                return UserId.GetHashCode() ^ PermissionCode.GetHashCode() ^ PermissionCategory.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            UserPermissionView another = obj as UserPermissionView;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.UserId == another.UserId) && (this.PermissionCode == another.PermissionCode) && (this.PermissionCategory == another.PermissionCategory);
            }
        }
    }

}
