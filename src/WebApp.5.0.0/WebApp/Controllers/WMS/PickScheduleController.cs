
namespace com.Sconit.Web.Controllers.WMS
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using Telerik.Web.Mvc;
    using System.Web.Routing;
    using com.Sconit.Web.Models;
    using com.Sconit.Utility;
    using com.Sconit.Web.Models.SearchModels.WMS;
    using com.Sconit.Entity.WMS;

    public class PickScheduleController : WebAppBaseController
    {
        #region 拣货策略
      
        private static string selectCountStatement = "select count(*) from PickSchedule as p";

        private static string selectStatement = "select p from PickSchedule as p";

        private static string selectCountPickScheduleStatement = "select count(*) from PickSchedule as s where s.PickScheduleNo = ?";


        [SconitAuthorize(Permissions = "Url_PickSchedule_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_PickSchedule_View")]
        public ActionResult List(GridCommand command, PickScheduleSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickSchedule_View")]
        public ActionResult _AjaxList(GridCommand command, PickScheduleSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<PickSchedule>(searchStatementModel, command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_PickSchedule_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_PickSchedule_Edit")]
        public ActionResult New(PickSchedule pickSchedule)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                if (base.genericMgr.FindAll<long>(selectCountPickScheduleStatement, new object[] { pickSchedule.PickScheduleNo })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.WMS.PickSchedule.PickSchedule_Errors_Existing_PickScheduleNo, pickSchedule.PickScheduleNo);
                }
                genericMgr.Create(pickSchedule);
                SaveSuccessMessage(Resources.WMS.PickSchedule.PickSchedule_Added);
                return RedirectToAction("Edit/" + pickSchedule.PickScheduleNo);
            }
            return View(pickSchedule);
        }

     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_PickSchedule_Edit")]
        public ActionResult Edit(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                PickSchedule pickSchedule = base.genericMgr.FindById<PickSchedule>(id);
                return View(pickSchedule);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_PickSchedule_Edit")]
        public ActionResult Edit(PickSchedule pickSchedule)
        {

            if (ModelState.IsValid)
            {
                base.genericMgr.Update(pickSchedule);
                SaveSuccessMessage(Resources.WMS.PickSchedule.PickSchedule_Updated);
            }

            return View(pickSchedule);
        }

        [SconitAuthorize(Permissions = "Url_PickSchedule_Delete")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<PickSchedule>(id);
                SaveSuccessMessage(Resources.WMS.PickSchedule.PickSchedule_Deleted);
                return RedirectToAction("List");
            }
        }
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PickScheduleSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("PickScheduleNo", searchModel.PickScheduleNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
         
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        #endregion

      
    }
}
