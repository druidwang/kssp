using System.Data;
using System.Web.Mvc;
using com.Sconit.Web.Util;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.SI;
using System.Data.SqlClient;
using System;

namespace com.Sconit.Web.Controllers.SI.WMS
{
    public class WMSMessageQueueLogController : WebAppBaseController
    {
        public WMSMessageQueueLogController()
        {

        }

        [SconitAuthorize(Permissions = "Url_SI_SAP_Trans_View")]
        public ActionResult Index(SearchModel searchModel)
        {
            TempData["SearchModel"] = searchModel;
            SqlParameter[] sqlParam = new SqlParameter[4];
            string sql = @"select top " + MaxRowSize +
                @" e.* from SI_WMS_MessageQueue e where e.Status = @p0 and e.CreateTime > @p1 and e.CreateTime < @p2  ";

            sqlParam[0] = new SqlParameter("@p0", searchModel.Status.HasValue ? searchModel.Status.Value : 2);

            if (searchModel.StartDate.HasValue)
            {
                sqlParam[1] = new SqlParameter("@p1", searchModel.StartDate);
            }
            else
            {
                sqlParam[1] = new SqlParameter("@p1", DateTime.Now.AddDays(-1));
            }
            if (searchModel.EndDate.HasValue)
            {
                sqlParam[2] = new SqlParameter("@p2", searchModel.EndDate);
            }
            else
            {
                sqlParam[2] = new SqlParameter("@p2", DateTime.Now);
            }

            if (searchModel.Id.HasValue)
            {
                sql += " and e.Id = @p3 ";
                sqlParam[3] = new SqlParameter("@p3", searchModel.Id);
            }

            sql += " order by e.Id desc ";

            DataSet entity = siMgr.GetDatasetBySql(sql, sqlParam);

            ViewModel model = new ViewModel();
            model.Data = entity.Tables[0];
            model.Columns = IListHelper.GetColumns(entity.Tables[0]);
            return View(model);
        }
    }
}
