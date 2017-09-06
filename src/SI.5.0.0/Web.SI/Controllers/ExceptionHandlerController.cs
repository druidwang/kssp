using System.Web.Mvc;

namespace com.Sconit.Web.Controllers
{
    public class ExceptionHandlerController : Controller
    {
        //
        // GET: /ExceptionHandler/

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult ObjectNotFound()
        {
            return View();
        }
    }
}
