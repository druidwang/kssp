/// <summary>
/// Summary description for ContainerController
/// </summary>
namespace com.Sconit.Web.Controllers.MD
{
    #region reference
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using System;
    using com.Sconit.Service.MRP;
    using System.Text;
    #endregion

    /// <summary>
    /// This controller response to control the Container.
    /// </summary>
    public class ContainerController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the Container security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        public IPlanMgr planMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the Container
        /// </summary>
        private static string selectCountStatement = "select count(*) from Container as c";

        /// <summary>
        /// hql to get all of the Container
        /// </summary>
        private static string selectStatement = "select c from Container as c";

        /// <summary>
        /// hql to get count of the Container by Container's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from Container as c where c.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for Container controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Container_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Container Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Container_View")]
        public ActionResult List(GridCommand command, ContainerSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Container Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Container_View")]
        public ActionResult _AjaxList(GridCommand command, ContainerSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Container>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_Container_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="container">Container Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Container_Edit")]
        public ActionResult New(Container container)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { container.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, container.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(container);
                    SaveSuccessMessage(Resources.MD.Container.Container_Added);
                    return RedirectToAction("Edit/" + container.Code);
                }
            }

            return View(container);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">container id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Container_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                Container container = this.genericMgr.FindById<Container>(id);
                return View(container);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="container">Container Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_Container_Edit")]
        public ActionResult Edit(Container container)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(container);
                SaveSuccessMessage(Resources.MD.Container.Container_Updated);
            }

            return View(container);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">Container id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_Container_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<Container>(id);
                SaveSuccessMessage(Resources.MD.Container.Container_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Container Search Model</param>
        /// <returns>return Container search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ContainerSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        [SconitAuthorize(Permissions = "Url_Mrp_Container_View")]
        public ActionResult ContainerView()
        {

            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_Container_View")]
        public string _GetContainerView(DateTime? planDate)
        {
            if (!planDate.HasValue)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_TimeCanNotBeEmpty);
                return string.Empty;
            }
            planDate = planDate.Value;
            var containerViews = planMgr.GetContainerViewList(planDate.Value);

            if (containerViews.Count() == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><tr>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.EXT.ControllerLan.Con_Item);
            str.Append("</th>");

            str.Append("<th  style=\"text-align:center\" >");
            str.Append(Resources.EXT.ControllerLan.Con_ItemDescription);
            str.Append("</th>");

            var containerGroup = from p in containerViews
                                 group p by new
                                 {
                                     p.Container,
                                     p.ContainerDescription
                                 } into g
                                 select new
                                 {
                                     g.Key.Container,
                                     g.Key.ContainerDescription,
                                     List = g
                                 };

            foreach (var container in containerGroup)
            {
                str.Append("<th  style=\"text-align:center\" >");
                str.Append(container.Container);
                str.Append("[");
                str.Append(container.ContainerDescription);
                str.Append("]");
                str.Append("</th>");
            }
            str.Append("</tr>");

            int l = 0;
            foreach (var containerView in containerViews)
            {
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }

                str.Append("<td>");
                str.Append(containerView.Item);
                str.Append("</td>");
                str.Append("<td>");
                str.Append(containerView.ItemDescription);
                str.Append("</td>");
                #region
                foreach (var container in containerGroup)
                {
                    var rccpExViewFirst = container.List.FirstOrDefault(m => m.Item == containerView.Item);
                    if (rccpExViewFirst != null)
                    {
                        str.Append("<td>");
                        str.Append(rccpExViewFirst.Qty.ToString("0"));
                        str.Append("</td>");
                    }
                    else
                    {
                        str.Append("<td>");
                        str.Append("0");
                        str.Append("</td>");
                    }
                }
                #endregion
                str.Append("</tr>");
            }
            str.Append("<tr class=\"t-alt\">");
            str.Append("<td>");
            str.Append("</td>");
            str.Append("<td>");
            str.Append(Resources.EXT.ControllerLan.Con_RequirementSum);
            str.Append("</td>");
            foreach (var container in containerGroup)
            {
                str.Append("<td>");
                str.Append(container.List.Sum(p => p.Qty).ToString("0"));
                str.Append("</td>");
            }
            str.Append("</tr>");

            str.Append("<tr>");
            str.Append("<td>");
            str.Append("</td>");
            str.Append("<td>");
            str.Append(Resources.EXT.ControllerLan.Con_CurrentQuantity);
            str.Append("</td>");
            foreach (var container in containerGroup)
            {
                str.Append("<td>");
                str.Append(container.List.First().ContainerQty.ToString("0"));
                str.Append("</td>");
            }
            str.Append("</tr>");
            //表尾
            str.Append("</table>");
            return str.ToString();
        }

    }
}
