using System;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.Exception;
using com.Sconit.Service;
using com.Sconit.Utility;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;


namespace com.Sconit.WebService
{
    public class BaseWebService : System.Web.Services.WebService
    {
        public Authentication Authentication { get; set; }

        protected ISecurityMgr securityMgr { get { return GetService<ISecurityMgr>(); } }
        protected ISystemMgr systemMgr { get { return GetService<ISystemMgr>(); } }

        protected T GetService<T>()
        {
            return ServiceLocator.GetService<T>();
        }

        protected void Authenticate()
        {
            if (Authentication == null || string.IsNullOrWhiteSpace(Authentication.UserName))
            {
                throw new BusinessException("Soap Header没有设置用户权限。");
            }

            User user = securityMgr.GetUser(Authentication.UserName);
            if (user == null || !EncryptHelper.Md5(Authentication.Password).Equals(user.Password, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException("没有访问Web服务的权限。");
            }

            SecurityContextHolder.Set(user);
        }
    }
}