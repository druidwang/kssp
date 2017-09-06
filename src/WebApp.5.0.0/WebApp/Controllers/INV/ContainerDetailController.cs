using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Web.Models;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.SCM;
using com.Sconit.Service;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.PrintModel.INV;
using AutoMapper;
using com.Sconit.Utility.Report;
using com.Sconit.Web.Models.SearchModels.BIL;
using com.Sconit.Entity.BIL;
using com.Sconit.Web.Models.SearchModels.SCM;
using com.Sconit.Web.Models.SearchModels.ORD;
using com.Sconit.Entity;
using com.Sconit.Entity.MRP.MD;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Entity.SYS;
using NHibernate;

namespace com.Sconit.Web.Controllers.INV
{
    public class ContainerDetailController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from ContainerDetail as c";
        private static string selectStatement = "select c from ContainerDetail as c";



        public IContainerMgr containerMgr { get; set; }


        #region public method
        public ActionResult Index()
        {
            return View();
        }


        [SconitAuthorize(Permissions = "Url_ContainerDetail_View")]
        public ActionResult New()
        {

            return View();
        }


        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ContainerDetail_View")]
        public ActionResult New(ContainerDetail containerDetail)
        {
            try
            {

                containerMgr.CreateContainer(containerDetail.Container, containerDetail.CreateQty);
                SaveSuccessMessage("容器新增成功");
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
                return View(containerDetail);
            }
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_ContainerDetail_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ContainerDetail containerDetail = this.genericMgr.FindById<ContainerDetail>(id);
                return View(containerDetail);
            }
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ContainerDetail_View")]
        public ActionResult List(GridCommand command, ContainerDetailSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ContainerDetail_View")]
        public ActionResult _AjaxList(GridCommand command, ContainerDetailSearchModel searchModel)
        {
            TempData["ContainerDetailSearchModel"] = searchModel;
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            var list = GetAjaxPageData<ContainerDetail>(searchStatementModel, command);
            return PartialView(list);
        }



        #endregion

        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ContainerDetailSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();


            HqlStatementHelper.AddEqStatement("ContainerId", searchModel.ContainerId, "c", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Container", searchModel.Container, "c", ref whereStatement, param);




            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by CreateDate desc";
            }
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
