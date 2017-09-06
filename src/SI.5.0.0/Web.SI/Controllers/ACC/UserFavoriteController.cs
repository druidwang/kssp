/// <summary>
/// Summary description for AddressController
/// </summary>
namespace com.Sconit.Web.Controllers.ACC
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Util;
    using Telerik.Web.Mvc;

    /// <summary>
    /// This controller response to control the UserFavorite.
    /// </summary>
    public class UserFavoriteController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the UserFavorite security 
        /// </summary>
        public IGenericMgr GeneMgr { get; set; }
        #endregion

        /// <summary>
        /// Index action for UserFavorite controller
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_UserFav_View")]
        public ActionResult Index()
        {
            int? id = CurrentUser.Id;
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                User user = this.GeneMgr.FindById<User>(id);
                user.NewPassword = user.Password;
                user.ConfirmPassword = user.Password;
                return View(user);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="user">user model</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_UserFav_Edit")]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                this.GeneMgr.Update(user);
                SaveSuccessMessage(Resources.ACC.User.User_Updated);
            }

            return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "Index" }, 
                                                       { "controller", "UserFavorite" },
                                                       { "id", user.Id }
                                                   });
        }

        /// <summary>
        /// ChangePassword action 
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_User_Edit")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// ChangePassword action
        /// </summary>
        /// <param name="model">Change Password Model</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_User_Edit")]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                CurrentUser.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(model.ConfirmPassword, "MD5");
                this.GeneMgr.Update(CurrentUser);
                SaveSuccessMessage(Resources.ACC.User.User_PasswordChanged);
            }

            return View(model);
        }
    }
}
