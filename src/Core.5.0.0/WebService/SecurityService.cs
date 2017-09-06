using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.VIEW;
using com.Sconit.Service;

namespace com.Sconit.WebService
{
    [WebService(Namespace = "http://com.Sconit.WebService.SecurityService/")]
    public class SecurityService : BaseWebService
    {
        //private ISecurityMgr securityMgr
        //{
        //    get
        //    {
        //        return GetService<ISecurityMgr>();
        //    }
        //}

        [WebMethod]
        public bool VerifyUserPassword(string userCode, string hashedPassword)
        {
            return securityMgr.VerifyUserPassword(userCode, hashedPassword);
        }

        [WebMethod]
        public List<string> GetUserPermissionCodes(string userCode)
        {
            IList<UserPermissionView> permissionList = securityMgr.GetUserPermissions(userCode);

            if (permissionList != null && permissionList.Count > 0)
            {
                return permissionList.Select(p => p.PermissionCode).Distinct().ToList();
            }

            return null;
        }

        //webservice不支持重载方法,就算是加了MessageName,到客户端那里的方法名就变成了MessageName的方法名
        [WebMethod]
        public List<string> GetUserPermissionCodesByType(string userCode, com.Sconit.CodeMaster.PermissionCategoryType permissionType)
        {
            IList<UserPermissionView> permissionList = securityMgr.GetUserPermissions(userCode, permissionType);

            if (permissionList != null && permissionList.Count > 0)
            {
                return permissionList.Select(p => p.PermissionCode).Distinct().ToList();
            }

            return null;
        }

        [WebMethod]
        public List<string> GetUserPermissionCodesByTypes(string userCode, com.Sconit.CodeMaster.PermissionCategoryType[] permissionType)
        {
            IList<UserPermissionView> permissionList = securityMgr.GetUserPermissions(userCode, permissionType);

            if (permissionList != null && permissionList.Count > 0)
            {
                return permissionList.Select(p => p.PermissionCode).Distinct().ToList();
            }

            return null;
        }
    }
}
