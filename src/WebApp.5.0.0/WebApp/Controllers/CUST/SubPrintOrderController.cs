using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Services.Transaction;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.SCM;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.CUST;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.SYS;


namespace com.Sconit.Web.Controllers.CUST
{
    public class SubPrintOrderController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public ISecurityMgr securityMgr { get; set; }
        //public ISystemMgr systemMgr { get; set; }

        private static string selectCountStatement = "select count(*) from SubPrintOrder as h";

        private static string selectStatement = "select h from SubPrintOrder as h";

        #region SubPrintOrder Method

        #region View

        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SubPrintOrder_View")]
        public ActionResult List(GridCommand command, SubPrintOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SubPrintOrder_View")]
        public ActionResult _AjaxList(GridCommand command, SubPrintOrderSearchModel searchModel)
        {
            string replaceFrom = "_AjaxList";
            string replaceTo = "List";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);

            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            var list = GetAjaxPageData<SubPrintOrder>(searchStatementModel, command);
            var codeDetails = this.genericMgr.FindAllIn<CodeDetail>
                ("from CodeDetail where Value in(?", list.Data.Select(p => p.ExcelTemplate));

            foreach (var data in list.Data)
            {
                if (data.UserId != 0)
                {
                    data.UserCode = this.genericMgr.FindById<User>(data.UserId).Code;
                }
                var codeDetail = codeDetails.FirstOrDefault(p => p.Value == data.ExcelTemplate);
                if (codeDetail != null)
                {
                    var codeMaster = (com.Sconit.CodeMaster.CodeMaster)Enum.Parse(typeof(com.Sconit.CodeMaster.CodeMaster), codeDetail.Code);
                    data.ExcelTemplateDescription = this.systemMgr.GetCodeDetailDescription(codeMaster, codeDetail.Value);
                }
                else
                {
                    data.ExcelTemplateDescription = data.ExcelTemplate;
                }
            }
            return PartialView(list);
        }
        #endregion

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_SubPrintOrder_View")]
        public ActionResult Edit(int Id)
        {
            if (Id == 0)
            {
                return HttpNotFound();
            }
            else
            {
                SubPrintOrder subPrintOrder = this.genericMgr.FindAll<SubPrintOrder>
                    ("select h from SubPrintOrder as h where h.Id=? ", new object[] { Id })[0];
                User user = new User();
                if (subPrintOrder.UserId != 0)
                {
                    user = this.genericMgr.FindById<User>(subPrintOrder.UserId);
                }
                if (!string.IsNullOrWhiteSpace(user.Code))
                {
                    subPrintOrder.UserCode = user.Code;
                }
                return View(subPrintOrder);
            }
        }

        [SconitAuthorize(Permissions = "Url_SubPrintOrder_View")]
        public ActionResult Edit(SubPrintOrder subPrintOrder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Check(subPrintOrder);
                    if (!string.IsNullOrWhiteSpace(subPrintOrder.UserCode))
                    {
                        var user = securityMgr.GetUser(subPrintOrder.UserCode);
                        subPrintOrder.UserId = user.Id;
                    }
                    this.genericMgr.UpdateWithTrim(subPrintOrder);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_UpdatedSuccessfully);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return View(subPrintOrder);
        }


        [SconitAuthorize(Permissions = "Url_SubPrintOrder_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_SubPrintOrder_View")]
        public ActionResult New(SubPrintOrder subPrintOrder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Check(subPrintOrder);
                    if (!string.IsNullOrWhiteSpace(subPrintOrder.UserCode))
                    {
                        var user = securityMgr.GetUser(subPrintOrder.UserCode);
                        subPrintOrder.UserId = user.Id;
                    }
                    this.genericMgr.CreateWithTrim(subPrintOrder);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CreateSuccessfully);
                    return new RedirectToRouteResult(new RouteValueDictionary { 
                    { "action", "Edit" }, { "controller", "SubPrintOrder" }, { "Id", subPrintOrder.Id } });
                }
            }
            catch (Exception e)
            {
                SaveErrorMessage(e);
            }
            return View(subPrintOrder);
        }

        [SconitAuthorize(Permissions = "Url_SubPrintOrder_View")]
        public ActionResult Delete(int Id)
        {
            SubPrintOrder subPrintOrder = this.genericMgr.FindById<SubPrintOrder>(Id);
            this.genericMgr.Delete(subPrintOrder);
            SaveSuccessMessage(Resources.EXT.ControllerLan.Con_DeletedSuccessfully);
            return RedirectToAction("List");
        }

        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, SubPrintOrderSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Client", searchModel.Client, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Printer", searchModel.Printer, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ExcelTemplate", searchModel.ExcelTemplate, "h", ref whereStatement, param);
            if (!string.IsNullOrWhiteSpace(searchModel.UserCode))
            {
                int userId = 0;
                var user = securityMgr.GetUser(searchModel.UserCode);
                if (user != null)
                {
                    userId = user.Id;
                }
                HqlStatementHelper.AddEqStatement("UserId", userId, "h", ref whereStatement, param);
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by h.Id desc";
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

        private void Check(SubPrintOrder subPrintOrder)
        {
            if (!string.IsNullOrWhiteSpace(subPrintOrder.Location))
            {
                this.genericMgr.FindById<Location>(subPrintOrder.Location);
            }
            if (!string.IsNullOrWhiteSpace(subPrintOrder.Region))
            {
                this.genericMgr.FindById<Region>(subPrintOrder.Region);
            }
            if (!string.IsNullOrWhiteSpace(subPrintOrder.Flow))
            {
                this.genericMgr.FindById<FlowMaster>(subPrintOrder.Flow);
            }
            var excelTemplate = systemMgr.GetCodeDetailDictionary()
                         .SelectMany(p => p.Value.Where(q => q.Value == subPrintOrder.ExcelTemplate));
            if (excelTemplate == null || excelTemplate.Count() == 0)
            {
                throw new Exception(Resources.EXT.ControllerLan.Con_CanNotFindThisExcelTemplate);
            }
        }
    }
}
