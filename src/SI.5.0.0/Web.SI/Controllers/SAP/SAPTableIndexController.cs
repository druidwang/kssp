using System.Data;
using System.Web.Mvc;
using com.Sconit.Web.Util;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.SI;
using System.Data.SqlClient;
using System;

/// <summary>
///MainController 的摘要说明
/// </summary>
namespace com.Sconit.Web.Controllers.SI.SAP
{
    [SconitAuthorize]
    // [SconitAuthorize(Permissions = "Url_SI_SAP_Supplier_View")]
    public class SAPTableIndexController : WebAppBaseController
    {
        public SAPTableIndexController()
        {

        }

        [SconitAuthorize(Permissions = "Url_SI_SAP_Supplier_View")]
        public ActionResult Index(SearchModel searchModel)
        {
            SqlParameter[] sqlParam = new SqlParameter[1];
            string sql = @"select e.* from SI_SAP_TableIndex e ";

            DataSet entity = siMgr.GetDatasetBySql(sql, sqlParam);

            ViewModel model = new ViewModel();
            model.Data = entity.Tables[0];
            model.Columns = IListHelper.GetColumns(entity.Tables[0]);
            return View(model);
        }

    }
}