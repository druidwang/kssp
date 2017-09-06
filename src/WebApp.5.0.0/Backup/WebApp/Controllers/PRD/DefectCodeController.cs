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
    public class DefectCodeController : WebAppBaseController
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
        private static string selectCountStatement = "select count(*) from DefectCode  r";

        /// <summary>
        /// hql to get all of the RoutingMaster
        /// </summary>
        private static string selectStatement = "select r from DefectCode as r";

        private static string CodeDuiplicateVerifyStatement = @"select count(*) from DefectCode as c where c.Code = ?";
      
        /// <summary>
        /// Index action for Routing controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Assembly_DefectCode")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Assembly_DefectCode")]
        public ActionResult DefectCodeSeachAdd()
        {
            return View();
        }
         [HttpPost]
         [SconitAuthorize(Permissions = "Url_Assembly_DefectCode")]
        public ActionResult DefectCodeSeachAdd(DefectCode search)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                try
                {
                    if (this.genericMgr.FindAll<long>(CodeDuiplicateVerifyStatement, new object[] { search.Code })[0] > 0)
                    {
                        base.SaveErrorMessage(Resources.CUST.DefectCode.DefectCode_Added_Existing_Code, search.Code);
                    }
                    else
                    {
                        genericMgr.CreateWithTrim(search);
                        SaveSuccessMessage(Resources.CUST.DefectCode.DefectCode_Added);
                        return RedirectToAction("Edit", new { code = search.Code });
                    }
                }
                catch (System.Exception ex)
                {
                      SaveErrorMessage(ex.Message);
                }
            }
            return View(search);

        }


          [SconitAuthorize(Permissions = "Url_Assembly_DefectCode")]
         public ActionResult Edit(string Code)
         {
             if (string.IsNullOrEmpty(Code))
             {
                 return HttpNotFound();
             }
             else
             {
                 DefectCode code = this.genericMgr.FindById<DefectCode>(Code);
                 return View(code);
             }
         }


          [SconitAuthorize(Permissions = "Url_Assembly_DefectCode")]
          public ActionResult SaveEdit(GridCommand command, DefectCode search)
          {
              try
              {
                  genericMgr.UpdateWithTrim(search);
                  SaveSuccessMessage(Resources.CUST.DefectCode.DefectCode_Added);
                  return RedirectToAction("Edit", new { code = search.Code });
              }
              catch (System.Exception ex)
              {
                  
                   SaveErrorMessage(ex.Message);
              }
              return View(search);
          }


        [SconitAuthorize(Permissions = "Url_Assembly_DefectCode")]
          public ActionResult DefectCodeDeleteId(string Id)
          {
              try
              {
                  genericMgr.DeleteById<DefectCode>(Id);
                  SaveSuccessMessage(Resources.CUST.DefectCode.DefectCode_Deletedsuccessful);
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
        [SconitAuthorize(Permissions = "Url_Assembly_DefectCode")]
        public ActionResult List(GridCommand command, DefectCodeSearchModel searchModel)
        {
            this.ProcessSearchModel(command, searchModel);
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
        [SconitAuthorize(Permissions = "Url_Assembly_DefectCode")]
        public ActionResult _AjaxList(GridCommand command, DefectCodeSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<DefectCode>(searchStatementModel, command));
        }


        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">RoutingMaster Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, DefectCodeSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ProductCode", searchModel.ProductCode, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);

            if (searchModel.CreateDateStart != null & searchModel.CreateDateEnd != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.CreateDateStart, searchModel.CreateDateEnd, "r", ref whereStatement, param);
            }
            else if (searchModel.CreateDateStart != null & searchModel.CreateDateEnd == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.CreateDateStart, "r", ref whereStatement, param);
            }
            else if (searchModel.CreateDateStart == null & searchModel.CreateDateEnd != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.CreateDateEnd, "r", ref whereStatement, param);
            }

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

