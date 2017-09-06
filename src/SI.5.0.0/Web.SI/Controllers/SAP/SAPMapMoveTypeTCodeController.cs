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
    public class SAPMapMoveTypeTCodeController : WebAppBaseController
    {
        public SAPMapMoveTypeTCodeController()
        {

        }

        //[SconitAuthorize(Permissions = "Url_SI_SAP_Item_View")]
        public ActionResult Index(SearchModel searchModel)
        {
            TempData["SearchModel"] = searchModel;
            SqlParameter[] sqlParam = new SqlParameter[1];
            string sql = @"select e.* from SI_SAP_MapMoveTypeTCode e ";

            if (!string.IsNullOrWhiteSpace(searchModel.Code))
            {
                sql += " where e.BWART = @p0 ";
                sqlParam[0] = new SqlParameter("@p0", searchModel.Code);
            }

            DataSet entity = siMgr.GetDatasetBySql(sql, sqlParam);

            ViewModel model = new ViewModel();
            model.Data = entity.Tables[0];
            model.Columns = IListHelper.GetColumns(entity.Tables[0]);
            return View(model);
        }
    }
}