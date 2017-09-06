namespace com.Sconit.Web.Controllers.PRD
{
    using System.Web.Mvc;
    using com.Sconit.Entity.VIEW;
    using com.Sconit.Utility;

    public class ExReportController : WebAppBaseController
    {
        //
        // GET: /Chart/
        [SconitAuthorize(Permissions = "Url_ExReport_KBIndex")]
        public ActionResult KBIndex()
        {
            string sql = @"USP_Report_EXPROD_Monitor";
            var exReportDataSet = genericMgr.GetDatasetByStoredProcedure(sql, null);
            var exReportList = Utility.IListHelper.DataTableToList<ExReport>(exReportDataSet.Tables[0]);
            return View(exReportList);
        }

    }
    //GetCurrentTheme  should be a stand alone extension (static class)
    public static class ThemeExtender
    {
        public static string GetCurrentTheme(this HtmlHelper html)
        {
            return html.ViewContext.HttpContext.Request.QueryString["theme"] ?? "vista";
        }
    }


}
