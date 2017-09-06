using System.Data;
using System.Web.Mvc;
using com.Sconit.Web.Util;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.SI.EDI;
using System.Data.SqlClient;
using System;
using com.Sconit.Entity.EDI.Ford;
using System.Collections.Generic;

/// <summary>
///MainController 的摘要说明
/// </summary>
namespace com.Sconit.Web.Controllers.SI.EDI
{
    [SconitAuthorize]
    // [SconitAuthorize(Permissions = "Url_SI_SAP_Supplier_View")]
    public class EDIShippingScheduleController : WebAppBaseController
    {
        public EDIShippingScheduleController()
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
            string sql = @"select top 20 e.* from SI_EDI_Ford_ShippingScheduleMaster e
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

            DataSet entity = siMgr.GetDatasetBySql(sql, sqlParam);
            var shippingScheduleMasterList = IListHelper.DataTableToList<ShippingScheduleMaster>(entity.Tables[0]);

            return View(new GridModel(shippingScheduleMasterList));
        }

        [GridAction]
        public ActionResult _OrdersDetailHierarchyAjax(string Id)
        {
            IList<ShippingScheduleDetail> shippingScheduleDetailList =
                siMgr.FindAll<ShippingScheduleDetail>("from ShippingScheduleDetail as o where o.ShippingId = ?", Id);

            return View(new GridModel(shippingScheduleDetailList));
        }

    }
}