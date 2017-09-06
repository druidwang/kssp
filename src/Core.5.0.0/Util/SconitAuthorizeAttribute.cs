using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using com.Sconit.Entity.ACC;
using System;
using System.Reflection;
using System.Linq;

namespace com.Sconit.Utility
{
    public class SconitAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly char AuthorizeAttributeSplitSymbol = ',';
        public string Permissions { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            User user = (User)httpContext.Session[WebConstants.UserSessionKey];
      
            if (user == null)
            {
                httpContext.Response.Redirect("~/Account/login");
                return false;
            }
            else
            {
                if (user.Code == "su")
                {
                    return true;
                }
                if (string.IsNullOrWhiteSpace(Permissions))
                {
                    return true;
                }
                else
                {
                    string[] permissionArray = Permissions.Split(AuthorizeAttributeSplitSymbol);
                    foreach (string permission in permissionArray)
                    {
                        if (user.UrlPermissions.Contains(permission))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //跳转至警告没有权限的页面
            filterContext.Result = new RedirectToRouteResult(
                                       new RouteValueDictionary  
                                   { 
                                       { "action", "Unauthorized" }, 
                                       { "controller", "ExceptionHandler" } 
                                   });
        }

    }
}
