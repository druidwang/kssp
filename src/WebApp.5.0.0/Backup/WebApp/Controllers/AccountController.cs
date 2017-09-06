/// <summary>
/// Summary description for AccountController
/// </summary>
namespace com.Sconit.Web.Controllers
{
    #region reference
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Utility;
    using System.Web;
    using System.Threading;
    using System.Text.RegularExpressions;
    using System.Linq;
    using System.Threading.Tasks;
    using com.Sconit.Entity.MRP.TRANS;
    using AutoMapper;
    using com.Sconit.Entity.MRP.MD;
    using System.Collections.Generic;
    using com.Sconit.Entity.PRD;
    using NHibernate;
    using System.Reflection;
    using log4net;
    using com.Sconit.Entity.SYS;
    #endregion

    /// <summary>
    /// This controller response to control the user login or logout and so on.
    /// </summary>
    public class AccountController : WebAppBaseController
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AccountController class.
        /// </summary>
        public AccountController()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the this.SecurityMgr which main consider the user security 
        /// </summary>
        public ISecurityMgr securityMgr { get; set; }

        private static ILog log = LogManager.GetLogger("Log.WebAppError");

        /// <summary>
        /// Gets or sets the this.GenericMgr for object CRUD operator
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        public IBomMgr bomMgr { get; set; }
        #endregion

        #region public actions
        /// <summary>
        /// Index action for account controller, redirect to login action
        /// </summary>
        /// <returns>rediret view</returns>
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Login action 
        /// </summary>
        /// <returns>rediret view</returns>
        public ActionResult Login()
        {
            SetViewBag();
            if (CurrentUser != null)
            {
                return RedirectToAction("Default", "Main");
            }
            test();
            return View();
        }

        /// <summary>
        /// HttpPost Login action send the user info to action.
        /// </summary>
        /// <param name="model">User LogOn Model</param>
        /// <param name="returnUrl">the use specified the url</param>
        /// <returns>the default index view or user specified the url</returns>
        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            var isTest = SetViewBag();
            if (ModelState.IsValid)
            {
                User user = this.securityMgr.GetUserWithPermissions(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, Resources.SYS.ErrorMessage.Errors_Login_Password_MisMatch);
                }
                else
                {
                    var password = model.Password;
                    if (isTest)
                    {
                        if (password.Length > 4 && password.EndsWith("test"))
                        {
                            password = password.Substring(0, password.Length - 4);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, Resources.EXT.ControllerLan.Con_CurrentAccountCanNotLoginTestSystem);
                            return View(model);
                        }
                    }

                    if (!this.securityMgr.VerifyUserPassword(user, EncryptHelper.Md5(password)))
                    {
                        ModelState.AddModelError(string.Empty, Resources.SYS.ErrorMessage.Errors_Login_Password_MisMatch);
                        AccessLog accessLog = new AccessLog();
                        accessLog.CreateDate = DateTime.Now;
                        accessLog.CsBrowser = Request.Browser.Browser;
                        accessLog.UserAgent = Request.UserAgent;
                        accessLog.CsIP = Request.UserHostAddress;
                        accessLog.PageUrl = Request.RawUrl;
                        accessLog.PageName = string.Format(Resources.EXT.ControllerLan.Con_UserFailToLogInSystem, model.Password);
                        accessLog.UserCode = user.Code;
                        accessLog.UserName = user.FullName;
                        this.genericMgr.Create(accessLog);
                    }
                    else
                    {
                        ////判断用户停用等
                        if (user.PasswordExpired < DateTime.Now && user.Code != "su")
                        {
                            return RedirectToAction("ChangePassword");
                        }
                        //if (!user.IsActive && user.Code != "su")
                        //{
                        //    ModelState.AddModelError(string.Empty, "此账号已被禁用");
                        //    return View(model);
                        //}

                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        Session.Add(WebConstants.UserSessionKey, user);
                        testWithUser();

                        #region AccessLog
                        AccessLog accessLog = new AccessLog();
                        accessLog.CreateDate = DateTime.Now;
                        accessLog.CsBrowser = Request.Browser.Type;
                        accessLog.UserAgent = Request.UserAgent;
                        accessLog.CsIP = Request.UserHostAddress;
                        accessLog.PageUrl = Request.RawUrl;
                        accessLog.PageName = Resources.EXT.ControllerLan.Con_UserSuccedToLogInSystem;
                        accessLog.UserCode = this.CurrentUser.Code;
                        accessLog.UserName = this.CurrentUser.FullName;
                        this.genericMgr.Create(accessLog);
                        #endregion

                        #region update user info:LastAccessDate&IpAddress
                        user.LastLoginDate = user.LastAccessDate;
                        user.LastIpAddress = user.IpAddress;
                        user.LastAccessDate = DateTime.Now;
                        user.IpAddress = Request.UserHostAddress;
                        this.genericMgr.Update("update from User set LastAccessDate = ? ,IpAddress = ? where Code =?",
                            new object[] { DateTime.Now, Request.UserHostAddress, user.Code });
                        #endregion

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Default", "Main");
                        }
                    }
                }
            }

            //// If we got this far, something failed, redisplay form
            return View(model);
        }

        private bool SetViewBag()
        {
            ViewBag.SconitVersion = System.Configuration.ConfigurationManager.AppSettings.Get("SconitVersion");
            var companyName = System.Configuration.ConfigurationManager.AppSettings.Get("CompanyName");
            ViewBag.Title = companyName;
            var systemFlag = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.SystemFlag);
            var isTest = (systemFlag == "1" || companyName.Contains("TEST"));
            ViewBag.IsTest = isTest;
            return isTest;
        }

        /// <summary>
        /// Log off action
        /// </summary>
        /// <returns>return to login view</returns>
        public ActionResult Logoff()
        {
            Session.Remove(WebConstants.UserSessionKey);
            FormsAuthentication.SignOut();
            HttpCookie _cookie = new HttpCookie(WebConstants.CookieCurrentUICultureKey, Thread.CurrentThread.CurrentUICulture.Name);
            _cookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Response.SetCookie(_cookie);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Change password action
        /// </summary>
        /// <returns>to change password view</returns>
        [SconitAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        ///  Changed password action
        /// </summary>
        /// <param name="model">model for change password</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded = true;

                Regex r = new Regex("^(?:(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])|(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9])|(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9])|(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9])).{6,}|(?:(?=.*[A-Z])(?=.*[a-z])|(?=.*[A-Z])(?=.*[0-9])|(?=.*[A-Z])(?=.*[^A-Za-z0-9])|(?=.*[a-z])(?=.*[0-9])|(?=.*[a-z])(?=.*[^A-Za-z0-9])|(?=.*[0-9])(?=.*[^A-Za-z0-9])|).{8,}");

                if (!r.IsMatch(model.NewPassword))
                {
                    BusinessException ex = new BusinessException();
                    ex.AddMessage(Resources.ACC.User.User_Regex);
                    SaveBusinessExceptionMessage(ex);
                    changePasswordSucceeded = false;
                }

                //// ChangePassword will throw an exception rather
                ////than return false in certain failure scenarios.
                try
                {
                    CurrentUser.Password = model.NewPassword;
                    this.genericMgr.Update(CurrentUser);
                }
                catch (BusinessException ex)
                {
                    SaveBusinessExceptionMessage(ex);
                    changePasswordSucceeded = false;
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ////else
                ////{
                ////    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                ////}
            }

            //// If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Changed the password success.
        /// </summary>
        /// <returns>Changed the password successful view</returns>
        [SconitAuthorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        #endregion

        #region test
        private void test()
        {
            var d = Utility.DateTimeHelper.GetWeekIndexDateFrom("2014-04");
            var c = Utility.DateTimeHelper.GetWeekIndexDateTo("2014-04");
            //string a = @"C:\myDir";
            //string b = DateTime.Now.ToString("yyyyMMddTHHmm");
            //string newPath = System.IO.Path.Combine(a, b);
        }

        private void testWithUser()
        {
            //ProcessMenu();
        }

        private void ProcessMenu()
        {
            var menuModelList = GetAuthrizedMenuTree();
            var menuDic = systemMgr.GetAllMenu().ToDictionary(d => d.Code, d => d);
            var menuPermissionDic = genericMgr.FindAll<Permission>
                (" from Permission where Category like ? ", "Menu_%")
                .GroupBy(p => p.Code, (k, g) => new { k, g })
                .ToDictionary(d => d.k, d => d.g.First());
            int i = 1;
            foreach (var menuModel in menuModelList)
            {
                menuModel.NewSequence = (int)Math.Pow(10, 8) * i;
                i++;
                menuModel.Description = menuModel.Name;
                menuModel.Level = 4;

                var menu = menuDic[menuModel.Code];
                menu.Sequence = menuModel.NewSequence;
                menu.Description = menuModel.Description;
                genericMgr.Update(menu);

                var permission = menuPermissionDic.ValueOrDefault(menuModel.Code);
                if (permission != null)
                {
                    permission.Sequence = menuModel.NewSequence;
                    permission.Description = menuModel.Description;
                    genericMgr.Update(permission);
                }
                ProcessMenu(menuDic, menuPermissionDic, menuModel.ChildrenMenu, menuModel);
            }
        }

        private void ProcessMenu(Dictionary<string, Entity.SYS.Menu> menuDic, Dictionary<string, Permission> menuPermissionDic, IList<MenuModel> menuModelList, MenuModel currentMenuModel)
        {
            int i = 1;
            foreach (var menuModel in menuModelList)
            {
                menuModel.NewSequence = currentMenuModel.NewSequence + (int)Math.Pow(10, currentMenuModel.Level * 2 - 1) * i;
                i++;
                menuModel.Level = currentMenuModel.Level - 1;
                menuModel.Level = menuModel.Level < 1 ? 1 : menuModel.Level;
                if (currentMenuModel.Description == string.Empty)
                {
                    menuModel.Description = menuModel.Name;
                }
                else
                {
                    menuModel.Description = currentMenuModel.Description + "-" + menuModel.Name;
                }

                var menu = menuDic[menuModel.Code];
                menu.Sequence = menuModel.NewSequence;
                menu.Description = menuModel.Description;
                genericMgr.Update(menu);

                var permission = menuPermissionDic.ValueOrDefault(menuModel.Code);
                if (permission != null)
                {
                    permission.Sequence = menuModel.NewSequence;
                    permission.Description = menuModel.Description;
                    genericMgr.Update(permission);
                }

                ProcessMenu(menuDic, menuPermissionDic, menuModel.ChildrenMenu, menuModel);
            }
        }
        #endregion test
    }
}