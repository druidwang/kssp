/// <summary>
/// Summary description for BaseController
/// </summary>
namespace com.Sconit.Web.Controllers
{
    #region reference
    using System.Web.Mvc;
    using com.Sconit.Entity;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Web.Util;
    #endregion

    /// <summary>
    /// BaseController for all controller inherit
    /// </summary>
    public class BaseController : System.Web.Mvc.Controller
    {
        /// <summary>
        /// Gets the Current User
        /// </summary>
        protected User CurrentUser
        {
            get 
            { 
                return (User)Session[WebConstants.UserSessionKey]; 
            }
        }

        #region override methods
        /// <summary>
        /// override the OnActionExecuting will set the current user to securitycontext holder 
        /// </summary>
        /// <param name="filterContext">the ActionExecutingContext param</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //MessageHolder.CleanMessage();
            SecurityContextHolder.Set(this.CurrentUser);
            base.OnActionExecuting(filterContext);
        }

        protected T GetService<T>(string serviceName)
        {
            return ServiceLocator.GetService<T>(serviceName);
        }
        #endregion        
    }
}