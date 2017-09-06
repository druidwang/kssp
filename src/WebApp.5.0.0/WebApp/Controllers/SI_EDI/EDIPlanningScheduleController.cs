using System.Data;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.SI;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using com.Sconit.Entity.SI.EDI_Ford;

/// <summary>
///MainController 的摘要说明
/// </summary>
namespace com.Sconit.Web.Controllers.SI_EDI
{
    [SconitAuthorize]
    // [SconitAuthorize(Permissions = "Url_SI_SAP_Supplier_View")]
    public class EDIPlanningScheduleController : WebAppBaseController
    {
        public EDIPlanningScheduleController()
        {

        }



        //[SconitAuthorize(Permissions = "Url_SI_SAP_Supplier_View")]
        public ActionResult Index(SearchModel searchModel)
        {
            TempData["SearchModel"] = searchModel;
            return View();
        }

        [GridAction]
        public ActionResult _OrderMasterHierarchyAjax()
        {
            SearchModel searchModel = (SearchModel)TempData["SearchModel"];

            searchModel = searchModel == null ? new SearchModel() : searchModel;
            searchModel.StartDate = searchModel.StartDate.HasValue ? searchModel.StartDate.Value : DateTime.Now.AddDays(-7);
            searchModel.EndDate = searchModel.EndDate.HasValue ? searchModel.EndDate.Value : DateTime.Now;

            SqlParameter[] sqlParam = new SqlParameter[4];
            string sql = @"select top 20 e.* from SI_EDI_Ford_PlanningScheduleMaster e
                            where e.CreateDate > @p1 and e.CreateDate < @p2  ";

            //sqlParam[0] = new SqlParameter("@p0", searchModel.Status.HasValue ? searchModel.Status.Value : 2);

            sqlParam[1] = new SqlParameter("@p1", searchModel.StartDate.Value);
            sqlParam[2] = new SqlParameter("@p2", searchModel.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(searchModel.Code))
            {
                sql += " and e.FileType = @p3 ";
                sqlParam[3] = new SqlParameter("@p3", searchModel.Code);
            }

            sql += " order by e.CreateDate desc ";

            DataSet entity = genericMgr.GetDatasetBySql(sql, sqlParam);

            var planningScheduleMasterList = IListHelper.DataTableToList<PlanningScheduleMaster>(entity.Tables[0]);

            return View(new GridModel(planningScheduleMasterList));
        }

        [GridAction]
        public ActionResult _OrdersDetailHierarchyAjax(string Id)
        {
            IList<PlanningScheduleDetail> planningScheduleDetailList =
                genericMgr.FindAll<PlanningScheduleDetail>("from PlanningScheduleDetail as o where o.PlanningId = ?", Id);

            return View(new GridModel(planningScheduleDetailList));
        }
    }
}