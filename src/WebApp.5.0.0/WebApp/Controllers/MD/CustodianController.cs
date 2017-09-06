using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.MD;
using com.Sconit.Web.Models;
using com.Sconit.Service;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using System.Web.Routing;

namespace com.Sconit.Web.Controllers.MD
{
    public class CustodianController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from Custodian as c";

        private static string selectStatement = "select c from Custodian as c";
        //
        // GET: /Custodian/
        //public IGenericMgr genericMgr { get; set; }

        //public IItemMgr itemMgr { get; set; }

        #region view
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Custodian_View")]
        public ActionResult List(GridCommand command, CustodianSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Custodian_View")]
        public ActionResult _AjaxList(GridCommand command, CustodianSearchModel searchModel)
        {

            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<Custodian>()));
            }
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Custodian>(searchStatementModel, command));
        }
        #endregion
        #region Edit
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Custodian_View")]
        public ActionResult _DeleteCustodian(string Id)
        {
            string UserCode = this.genericMgr.FindById<Custodian>(int.Parse(Id)).UserCode;
            this.genericMgr.DeleteById<Custodian>(int.Parse(Id));
            //return new RedirectToRouteResult(new RouteValueDictionary { { "action", "List" }, { "controller", "Custodian" }, { "UserCode", UserCode } });
            return Json(new { UserCode = UserCode });
        }

        [SconitAuthorize(Permissions = "Url_Custodian_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Custodian_View")]
        public ActionResult New(Custodian custodian)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    itemMgr.CreateCustodian(custodian);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_KeeperCreateSuccessfully);
                   // return RedirectToAction("List/" +custodian.UserCode );
                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "List" }, { "controller", "Custodian" }, { "UserCode", custodian.UserCode }, { "isFromList",true } });
                }
                catch (BusinessException ex)
                {
                    string messagesStr = "";
                    IList<Message> messageList = ex.GetMessages();
                    foreach (Message message in messageList)
                    {
                        messagesStr += message.GetMessageString()+",";
                    }
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_Item + messagesStr.Substring(0, messagesStr.Length - 1));
                }
                
            }
            return View(custodian);
        }
        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, CustodianSearchModel searchModel)
        {

           IList<object> param = new List<object>();
           string whereStatement = string.Empty;

           HqlStatementHelper.AddEqStatement("UserCode", searchModel.UserCode,  "c", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by c.CreateDate desc";
            }

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
