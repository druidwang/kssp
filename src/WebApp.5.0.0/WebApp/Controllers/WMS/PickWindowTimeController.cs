
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

    public class PickWindowTimeController : WebAppBaseController
    {
        #region 拣货窗口时间

        private static string selectCountStatement = "select count(*) from PickWindowTime as p";

        private static string selectStatement = "select p from PickWindowTime as p";

        private static string selectCountPickWindowTimeStatement = "select count(*) from PickWindowTime as s where s.PickScheduleNo = ? and s.ShiftCode = ?";


        [SconitAuthorize(Permissions = "Url_PickWindowTime_View")]
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
        [SconitAuthorize(Permissions = "Url_PickWindowTime_View")]
        public ActionResult List(GridCommand command, PickWindowTimeSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_PickWindowTime_View")]
        public ActionResult _AjaxList(GridCommand command, PickWindowTimeSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<PickWindowTime>(searchStatementModel, command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_PickWindowTime_Edit")]
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
        [SconitAuthorize(Permissions = "Url_PickWindowTime_Edit")]
        public ActionResult New(PickWindowTime pickWindowTime)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                if (base.genericMgr.FindAll<long>(selectCountPickWindowTimeStatement, new object[] { pickWindowTime.PickScheduleNo,pickWindowTime.ShiftCode})[0] > 0)
                {
                    base.SaveErrorMessage(Resources.WMS.PickWindowTime.PickWindowTime_Errors_Existing_PickScheduleNoAndTime, pickWindowTime.PickScheduleNo,pickWindowTime.ShiftCode);
                }
                genericMgr.Create(pickWindowTime);
                SaveSuccessMessage(Resources.WMS.PickWindowTime.PickWindowTime_Added);
                return RedirectToAction("Edit/" + pickWindowTime.Id);
            }
            return View(pickWindowTime);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_PickWindowTime_Edit")]
        public ActionResult Edit(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                PickWindowTime pickWindowTime = base.genericMgr.FindById<PickWindowTime>(Convert.ToInt32(id));
                return View(pickWindowTime);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_PickWindowTime_Edit")]
        public ActionResult Edit(PickWindowTime pickWindowTime)
        {

            if (ModelState.IsValid)
            {
                base.genericMgr.Update(pickWindowTime);
                SaveSuccessMessage(Resources.WMS.PickWindowTime.PickWindowTime_Updated);
            }

            return View(pickWindowTime);
        }

        [SconitAuthorize(Permissions = "Url_PickWindowTime_Delete")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<PickWindowTime>(id);
                SaveSuccessMessage(Resources.WMS.PickWindowTime.PickWindowTime_Deleted);
                return RedirectToAction("List");
            }
        }
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PickWindowTimeSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("PickScheduleNo", searchModel.PickScheduleNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ShiftCode", searchModel.ShiftCode, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);

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
