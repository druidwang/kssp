using System.Web.Mvc;
using com.Sconit.Web.Util;

/// <summary>
///MainController 的摘要说明
/// </summary>
namespace com.Sconit.Web.Controllers
{
    [SconitAuthorize]
    public class MainController : WebAppBaseController
    {
        public MainController()
        {

        }

        public ActionResult Default()
        {
            return View();
        }

        public ActionResult Top()
        {
            ViewBag.IsShowImage = true;
            return View();
        }

        public ActionResult Nav()
        {
            ViewBag.UserCode = this.CurrentUser.CodeDescription;
            return PartialView(base.GetAuthrizedMenuTree());
        }

        public ActionResult Switch()
        {
            return View();
        }
    }
}