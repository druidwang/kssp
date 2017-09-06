using System.Data;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.SI;
using System.Data.SqlClient;
using System;

namespace com.Sconit.Web.Controllers.SI
{
    public class SIEntityPreferenceController : WebAppBaseController
    {
        public SIEntityPreferenceController()
        {

        }

        [SconitAuthorize(Permissions = "Url_SI_SIEntityPreference")]
        public ActionResult Index(SearchModel searchModel)
        {
            SqlParameter[] sqlParam = new SqlParameter[1];
            string sql = @"select e.* from SI_EntityPreference e ";

            DataSet entity = genericMgr.GetDatasetBySql(sql, sqlParam);

            ViewModel model = new ViewModel();
            model.Data = entity.Tables[0];
            model.Columns = IListHelper.GetColumns(entity.Tables[0]);
            return View(model);
        }
    }
}
