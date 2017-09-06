/// <summary>
/// Summary description for LocationController
/// </summary>
namespace com.Sconit.Web.Controllers.PRD
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.PRD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.PRD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Web.Models.SearchModels.CUST;
    using com.Sconit.Entity.CUST;

    /// <summary>
    /// This controller response to control the Routing.
    /// </summary>
    public class ProductLineMapController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the Routing security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

       
        #endregion

        /// <summary>
        /// hql to get count of the RoutingMaster
        /// </summary>
        private static string selectCountStatement = "select count(*) from ProductLineMap  r";

        /// <summary>
        /// hql to get all of the RoutingMaster
        /// </summary>
        private static string selectStatement = "select r from ProductLineMap as r";

        private static string CodeDuiplicateVerifyStatement = @"select count(*) from ProductLineMap as c where c.SAPProductLine = ?";
      
        /// <summary>
        /// Index action for Routing controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_CUST_ProductLineMap_View")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_CUST_ProductLineMap_View")]
        public ActionResult ProductLineMapSeachAdd()
        {
            return View();
        }
         [HttpPost]
         [SconitAuthorize(Permissions = "Url_CUST_ProductLineMap_View")]
        public ActionResult ProductLineMapSeachAdd(ProductLineMap search)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                try
                {
                    if (this.genericMgr.FindAll<long>(CodeDuiplicateVerifyStatement, new object[] { search.SAPProductLine })[0] > 0)
                    {
                        base.SaveErrorMessage(Resources.CUST.ProductLineMap.ProductLineMap_Added_Existing_Code, search.SAPProductLine);
                    }
                    else
                    {
                        genericMgr.CreateWithTrim(search);
                        SaveSuccessMessage(Resources.CUST.ProductLineMap.ProductLineMap_Added);
                        return RedirectToAction("Edit", new { SAPProductLine = search.SAPProductLine });
                    }
                }
                catch (System.Exception ex)
                {
                  SaveErrorMessage(ex.Message);
                }
            }
            return View(search);

        }


          [SconitAuthorize(Permissions = "Url_CUST_ProductLineMap_View")]
         public ActionResult Edit(string SAPProductLine)
         {
             if (string.IsNullOrEmpty(SAPProductLine))
             {
                 return HttpNotFound();
             }
             else
             {
                 ProductLineMap code = this.genericMgr.FindById<ProductLineMap>(SAPProductLine);
                 return View(code);
             }
         }


          [SconitAuthorize(Permissions = "Url_CUST_ProductLineMap_View")]
          public ActionResult SaveEdit(GridCommand command, ProductLineMap search)
          {
              if (ModelState.IsValid)
              {
                  try
                  {
                      genericMgr.UpdateWithTrim(search);
                      SaveSuccessMessage(Resources.CUST.ProductLineMap.ProductLineMap_Added);
                      return RedirectToAction("Edit", new { SAPProductLine = search.SAPProductLine });
                  }
                  catch (System.Exception ex)
                  {
                      
                     SaveErrorMessage(ex.Message);
                  }
              }
              return View(search);
          }


        [SconitAuthorize(Permissions = "Url_CUST_ProductLineMap_View")]
          public ActionResult ProductLineMapDeleteId(string id)
          {
              try
              {
                  genericMgr.DeleteById<ProductLineMap>(id);
                  SaveSuccessMessage(Resources.CUST.ProductLineMap.ProductLineMap_Deletedsuccessful);
              }
              catch (System.Exception ex)
              {
                  SaveErrorMessage(ex.Message);
              }
              return RedirectToAction("List");

          }
       
        /// <summary>
        /// List acion
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">RoutingMaster Search Model</param>
        /// <returns>return to the result action</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CUST_ProductLineMap_View")]
        public ActionResult List(GridCommand command, ProductLineMapSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">RoutingMaster Search Model</param>
        /// <returns>return to the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CUST_ProductLineMap_View")]
        public ActionResult _AjaxList(GridCommand command, ProductLineMapSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ProductLineMap>(searchStatementModel, command));
        }


        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">RoutingMaster Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ProductLineMapSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("SAPProductLine", searchModel.SAPProductLine, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ProductLine", searchModel.ProductLine, "r", ref whereStatement, param);

           
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
    }
}

