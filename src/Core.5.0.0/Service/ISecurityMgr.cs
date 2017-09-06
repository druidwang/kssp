using System.Collections.Generic;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.VIEW;

namespace com.Sconit.Service
{
    public interface ISecurityMgr
    {
        User GetUser(string userCode);

        User GetUserWithPermissions(string userCode);

        IList<UserPermissionView> GetUserPermissions(string userCode);

        IList<UserPermissionView> GetUserPermissions(string userCode, com.Sconit.CodeMaster.PermissionCategoryType categoryType);

        IList<UserPermissionView> GetUserPermissions(string userCode, com.Sconit.CodeMaster.PermissionCategoryType[] categoryTypes);

        void UpdateUserRoles(int userId, IList<int> assignedRoleIdList);

        void UpdateUserPermissionGroups(int userId, IList<int> assignedPermissionGroupIdList);

        void UpdateUserPermissions(int userId, string permissionCategoryCode, IList<int> assignedPermissionIdList);

        void UpdateRoleUsers(int roleId, IList<int> assignedUserIdList);

        void UpdateRolePermissionGroups(int roleId, IList<int> assignedPermissionGroupIdList);

        void UpdateRolePermissions(int roleId, string permissionCategoryCode, IList<int> assignedPermissionIdList);

        void UpdatePermissionGroupUsers(int permissionGroupId, IList<int> assignedUserIdList);

        void UpdatePermissionGroupRoles(int permissionGroupId, IList<int> assignedRoleIdList);

        void UpdatePermissionGroupPermissions(int permissionGroupId, string permissionCategoryCode, IList<int> assignedPermissionIdList);

        void TranslatePermission(IList<Permission> permissionList);

        void TranslatePermissionCategory(IList<PermissionCategory> permissionCategoryList);

        bool VerifyUserPassword(string user, string hashedPassword);

        bool VerifyUserPassword(User user, string hashedPassword);

        void ChangePassword(string userCode, string oldPassword, string newPassword);
    }
}
