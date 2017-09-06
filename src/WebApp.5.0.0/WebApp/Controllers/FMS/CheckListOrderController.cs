/// <summary>
/// Summary description for CheckListOrderController
/// </summary>
namespace com.Sconit.Web.Controllers.FMS
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
    using com.Sconit.Web.Models.SearchModels.FMS;
    using com.Sconit.Entity.FMS;
    using com.Sconit.Entity.ACC;
    using System.Xml;
    using System;
    using System.Xml.Linq;
    using System.Text;
    using com.Sconit.Entity.Exception;
    #endregion

    /// <summary>
    /// This controller response to control the CheckListOrder.
    /// </summary>
    public class CheckListOrderController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the checkListOrder security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        public IFacilityMgr facilityMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the checkListOrder
        /// </summary>
        private static string selectCountStatement = "select count(*) from CheckListOrderMaster as c";

        /// <summary>
        /// hql to get all of the checkListOrder
        /// </summary>
        private static string selectStatement = "select c from CheckListOrderMaster as c";

        /// <summary>
        /// hql to get count of the checkListOrder by checkListOrder's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from CheckListOrderMaster as c where c.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for CheckListOrder controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_CheckListOrder_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">CheckListOrder Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CheckListOrder_View")]
        public ActionResult List(GridCommand command, CheckListOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">CheckListOrder Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CheckListOrder_View")]
        public ActionResult _AjaxList(GridCommand command, CheckListOrderSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<CheckListOrderMaster>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_CheckListOrder_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="checkListOrder">CheckListOrder Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CheckListOrder_Edit")]
        public ActionResult New(CheckListOrderMaster checkListOrder)
        {
            if (ModelState.IsValid)
            {

                facilityMgr.CreateCheckListOrder(checkListOrder);
                SaveSuccessMessage("巡检单新增成功");
                return RedirectToAction("Edit/" + checkListOrder.Code);

            }

            return View(checkListOrder);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">checkListOrder id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CheckListOrder_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                CheckListOrderMaster checkListOrder = this.genericMgr.FindById<CheckListOrderMaster>(id);
                return View(checkListOrder);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="checkListOrder">CheckListOrder Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_CheckListOrder_Edit")]
        public ActionResult Edit(CheckListOrderMaster checkListOrder)
        {
            if (ModelState.IsValid)
            {

                this.genericMgr.UpdateWithTrim(checkListOrder);
                SaveSuccessMessage("巡检单修改成功");
            }

            return View(checkListOrder);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">checkListOrder id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_CheckListOrder_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<CheckListOrderMaster>(id);
                SaveSuccessMessage("巡检单删除成功");
                return RedirectToAction("List");
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CheckListOrder_Edit")]
        public ActionResult _CheckListOrderDetail(string CheckListOrderCode)
        {
            CheckListOrderMaster order = genericMgr.FindById<CheckListOrderMaster>(CheckListOrderCode);
            ViewBag.IsEndable = order.Status == Sconit.CodeMaster.CheckListOrderStatus.Create;
            IList<CheckListOrderDetail> checkListOrderDetailList = genericMgr.FindAll<CheckListOrderDetail>("from CheckListOrderDetail where OrderNo=?", CheckListOrderCode);
            return PartialView(checkListOrderDetailList);

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CheckListOrder_Edit")]
        public JsonResult ReleaseCheckListOrder(string checkListOrderNo, string idStr, string isNormalStr, string resultStr)
        {
            try
            {
                CheckListOrderMaster checkListOrderMaster = genericMgr.FindById<CheckListOrderMaster>(checkListOrderNo);


                IList<CheckListOrderDetail> checkListOrderDetailList = genericMgr.FindAll<CheckListOrderDetail>("from CheckListOrderDetail where OrderNo=?", checkListOrderNo);
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] idArray = idStr.Split(',');
                    string[] isNormalArray = isNormalStr.Split(',');
                    string[] resultArray = resultStr.Split(',');
                    for (int i = 0; i < idArray.Count(); i++)
                    {
                        if (Convert.ToInt32(idArray[i]) > 0)
                        {
                            CheckListOrderDetail checkListOrderDetail = checkListOrderDetailList.Where(p => p.Id == Convert.ToInt32(idArray[i])).FirstOrDefault();

                            checkListOrderDetail.IsNormal = bool.Parse(isNormalArray[i]);
                            checkListOrderDetail.Remark = resultArray[i];

                        }
                    }
                }
                checkListOrderMaster.CheckListOrderDetailList = checkListOrderDetailList.ToList();
                facilityMgr.ReleaseCheckListOrder(checkListOrderMaster);

                object obj = new { SuccessMessage = "巡检单提交成功" };
                return Json(obj);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">CheckListOrder Search Model</param>
        /// <returns>return checkListOrder search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, CheckListOrderSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("CheckListOrderNo", searchModel.CheckListOrderNo, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CheckListCode", searchModel.CheckListCode, HqlStatementHelper.LikeMatchMode.Anywhere, "c", ref whereStatement, param);

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
