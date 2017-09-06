using System;
using System.Linq;
using System.Web.Mvc;
using com.Sconit.Entity.SYS;
using com.Sconit.Utility;

/// <summary>
///MainController 的摘要说明
/// </summary>
namespace com.Sconit.Web.Controllers
{
    [SconitAuthorize]
    public class MainController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public MainController()
        {

        }

        public ActionResult Default()
        {
            ViewBag.Title = System.Configuration.ConfigurationManager.AppSettings.Get("CompanyName");
            //if (Request.Cookies[WebConstants.CookieMainPageUrlKey] != null && this.CurrentUser != null)
            //{
            //    string mainPage = Request.Cookies[WebConstants.CookieMainPageUrlKey].Values[this.CurrentUser.Code];
            //    if (string.IsNullOrWhiteSpace(mainPage))
            //    {
            //        ViewBag.MainPageUrl = "../UserFavorite/Index";
            //    }
            //    else
            //    {
            //        ViewBag.MainPageUrl = mainPage;
            //    }
            //}
            //else
            //{
            //    ViewBag.MainPageUrl = "../UserFavorite/Index";
            //}
            return View();
        }

        public ActionResult Main()
        {
            ViewBag.CurrentUserCode = this.CurrentUser.Code;
            if (Request.Cookies[WebConstants.CookieMainPageUrlKey + this.CurrentUser.Code] != null && this.CurrentUser != null)
            {
                string mainPage = Request.Cookies[WebConstants.CookieMainPageUrlKey + this.CurrentUser.Code].Value;
                if (string.IsNullOrWhiteSpace(mainPage))
                {
                    ViewBag.MainPageUrl = "/UserFavorite/Index";
                }
                else
                {
                    ViewBag.MainPageUrl = mainPage;
                }
            }
            else
            {
                ViewBag.MainPageUrl = "/UserFavorite/Index";
            }
            var allmenu = systemMgr.GetAllMenu().Where(p => p.PageUrl != null);
            var menu = allmenu.Where(p => p.PageUrl.EndsWith(ViewBag.MainPageUrl)).FirstOrDefault();
            if (menu != null)
            {
                ViewBag.MainPageName = menu.Description;
            }
            else
            {
                ViewBag.MainPageUrl = "/UserFavorite/Index";
                ViewBag.MainPageName = allmenu.Where(p => p.PageUrl.EndsWith(ViewBag.MainPageUrl)).First().Description;
            }
            string name = Resources.SYS.Menu.ResourceManager.GetString(ViewBag.MainPageName);
            if (name != null)
            {
                ViewBag.MainPageName = name;
            }
            _CreateLog((string)ViewBag.MainPageUrl, (string)ViewBag.MainPageName);


            return View();
        }

        public ActionResult Top()
        {
            var companyName = System.Configuration.ConfigurationManager.AppSettings.Get("CompanyName");
            ViewBag.Title = companyName;
            var systemFlag = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.SystemFlag);
            var isTest = (systemFlag == "1" || companyName.Contains("TEST"));
            ViewBag.IsTest = isTest;
            return View();
        }

        public ActionResult Nav()
        {
            ViewBag.UserCode = this.CurrentUser.CodeDescription;
            var menuModelList = base.GetAuthrizedMenuTree();
            return PartialView(menuModelList);
        }

        public ActionResult Switch()
        {
            return View();
        }

        [HttpPost]
        public void _CreateLog(string pageUrl, string pageName)
        {
            AccessLog accessLog = new AccessLog();
            accessLog.CreateDate = DateTime.Now;
            accessLog.CsBrowser = Request.Browser.Type;
            accessLog.UserAgent = Request.UserAgent;
            accessLog.CsIP = Request.UserHostAddress;
            accessLog.PageUrl = pageUrl;
            accessLog.PageName = pageName;
            accessLog.UserCode = this.CurrentUser.Code;
            accessLog.UserName = this.CurrentUser.FullName;
            this.genericMgr.Create(accessLog);
        }
        public string _GetTransName(string name)
        {
           string name1 = Resources.SYS.Menu.ResourceManager.GetString(name);
           if (name1 != null)
            {
                name = name1;
            }
            return name;
        }
    }
}