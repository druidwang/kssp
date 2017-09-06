using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models;
using com.Sconit.Entity.CUST;
using com.Sconit.Web.Models.SearchModels.CUST;

namespace com.Sconit.Web.Controllers.CUST
{
    public class FailCodeController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from FailCode as f";

        private static string selectStatement = "from FailCode as f";

        //public IGenericMgr genericMgr { get; set; }
        //
        // GET: /FailCode/
        #region  public 
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_FailCode_View")]
        [GridAction]
        public ActionResult List(GridCommand command, FailCodeSearchModel searchModel)
        {
            TempData["FailCodeSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_FailCode_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, FailCodeSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<FailCode>(searchStatementModel, command));
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_FailCode_View")]
        public ActionResult _Insert(FailCode failCode)
        {
            if (ModelState.IsValid)
            {
                IList<FailCode> failCodeList = genericMgr.FindAll<FailCode>("from FailCode as f where f.Code=?", failCode.Code);
                if (failCodeList.Count > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CodeAlreadyExists);
                   
                }
                else
                {
                    genericMgr.CreateWithTrim(failCode);
                }
            }
            IList<FailCode> FailCodeList = genericMgr.FindAll<FailCode>();
            return PartialView(new GridModel(FailCodeList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_FailCode_View")]
        public ActionResult _Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<FailCode>(Id);
            }
            IList<FailCode> FailCodeList = genericMgr.FindAll<FailCode>();
            return PartialView(new GridModel(FailCodeList));
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_FailCode_View")]
        public ActionResult _Update(FailCode failCode,string id)
        {
            ModelState.Remove("Code");
                FailCode newFailCode = genericMgr.FindById<FailCode>(id);
                newFailCode.Code = id;
                newFailCode.ENGDescription = failCode.ENGDescription;
                newFailCode.CHNDescription = failCode.CHNDescription;
                genericMgr.UpdateWithTrim(newFailCode);
            

            IList<FailCode> FailCodeList = genericMgr.FindAll<FailCode>();
            return PartialView(new GridModel(FailCodeList));
        }


        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, FailCodeSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CHNDescription", searchModel.CHNDescription, HqlStatementHelper.LikeMatchMode.Start, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ENGDescription", searchModel.ENGDescription, HqlStatementHelper.LikeMatchMode.Start, "f", ref whereStatement, param);
           
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
