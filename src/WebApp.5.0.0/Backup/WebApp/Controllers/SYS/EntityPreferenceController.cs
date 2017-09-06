/// <summary>
/// Summary description for AddressController
/// </summary>
namespace com.Sconit.Web.Controllers.SYS
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.SYS;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;

    /// <summary>
    /// This controller response to control the EntityPreference.
    /// </summary>
    public class EntityPreferenceController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the EntityPreference security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        /// <summary>
        /// Gets or sets the this.SystemMgr which main consider the EntityPreference security 
        /// </summary>
        //public ISystemMgr systemMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the Item 
        /// </summary>
        private static string entityPrefSelectCountStatement = "select count(*) from EntityPreference as e";

        /// <summary>
        /// hql to get all of the Item
        /// </summary>
        private static string entityPrefSelectStatement = "select e from EntityPreference as e";

        /// <summary>
        /// EntityPreference Index action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">EntityPreference Search Model</param>
        /// <returns>Index view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_EntityPreference_View")]
        public ActionResult Index(GridCommand command, EntityPreferenceSearchModel searchModel)
        {
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxIndex(GridCommand command, EntityPreferenceSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<EntityPreference> list = GetAjaxPageData<EntityPreference>(searchStatementModel, command);
            foreach (EntityPreference entityPreference in list.Data)
            {
                entityPreference.EntityPreferenceDesc = this.systemMgr.TranslateEntityPreferenceDescription(entityPreference.Description.ToString());
            }
            return PartialView(list);

        }

        /// <summary>
        /// EntityPreference Index action
        /// </summary>
        /// <param name="id">EntityPreference id</param>
        /// <param name="value">EntityPreference value</param>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">EntityPreference Search Model</param>
        /// <returns>Index view</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_EntityPreference_View")]
        public ActionResult update(int? id, string value, GridCommand command, EntityPreferenceSearchModel searchModel)
        {
            if (id.HasValue)
            {
                EntityPreference entityPreference = this.genericMgr.FindById<EntityPreference>((int)id);
                entityPreference.Value = value;
                this.genericMgr.Update(entityPreference);
                systemMgr.ResetCache();
                //SaveSuccessMessage(Resources.SYS.EntityPreference.EntityPreference_Updated);
            }
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<EntityPreference> gridmodel = GetAjaxPageData<EntityPreference>(searchStatementModel, command);
            foreach (var item in gridmodel.Data)
            {
                item.EntityPreferenceDesc = this.systemMgr.TranslateEntityPreferenceDescription(item.Description.ToString());
            }
            return PartialView(gridmodel);
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Item Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, EntityPreferenceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "EntityPreferenceDesc")
                    command.SortDescriptors[0].Member = "Description";
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = entityPrefSelectCountStatement;
            searchStatementModel.SelectStatement = entityPrefSelectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            if (command.SortDescriptors.Count == 0)
            {
                searchStatementModel.SortingStatement = " order by Sequence";
            }
            return searchStatementModel;
        }
    }
}
