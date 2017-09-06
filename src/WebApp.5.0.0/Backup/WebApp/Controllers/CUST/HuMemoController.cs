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
    public class HuMemoController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from HuMemo as h";

        private static string selectStatement = "from HuMemo as h";

        //public IGenericMgr genericMgr { get; set; }
        //
        // GET: /FailCode/
        #region  public 
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_HuMemo_View")]
        [GridAction]
        public ActionResult List(GridCommand command, HuMemoSearchModel searchModel)
        {
            this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            ViewBag.Code = searchModel.Code;
            ViewBag.Description = searchModel.Description;
            ViewBag.ResourceGroup = searchModel.ResourceGroup;
            return View();
        }

        [SconitAuthorize(Permissions = "Url_HuMemo_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, HuMemoSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<HuMemo>(searchStatementModel, command));
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_HuMemo_View")]
        public ActionResult _Insert(GridCommand command,HuMemo huMemo, string CodeTo, string DescriptionTo)
        {
            HuMemoSearchModel searchModel = new HuMemoSearchModel();
            searchModel.Code = CodeTo;
            searchModel.Description = DescriptionTo;
            if (ModelState.IsValid)
            {
                IList<HuMemo> huMemoList = genericMgr.FindAll<HuMemo>("from HuMemo as h where h.Code=?", huMemo.Code);
                if (huMemoList.Count > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CodeAlreadyExists);
                    return PartialView();
                   
                }
                else
                {
                    huMemo.ResourceGroup = (com.Sconit.CodeMaster.ResourceGroup)Enum.Parse(typeof(com.Sconit.CodeMaster.ResourceGroup), huMemo.ResourceGroupDescription, true);
                    genericMgr.CreateWithTrim(huMemo);
                }
            }
      
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<HuMemo>(searchStatementModel, command));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_HuMemo_View")]
        public ActionResult _Delete(GridCommand command,string Id,string CodeTo, string DescriptionTo)
        {
            HuMemoSearchModel searchModel = new HuMemoSearchModel();
            searchModel.Code = CodeTo;
            searchModel.Description = DescriptionTo;
            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<HuMemo>(Id);
            }
              
                SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
                return PartialView(GetAjaxPageData<HuMemo>(searchStatementModel, command));
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_HuMemo_View")]
        public ActionResult _Update(GridCommand command,HuMemo huMemo, string id, string CodeTo, string DescriptionTo)
        {
            HuMemoSearchModel searchModel = new HuMemoSearchModel();
            searchModel.Code = CodeTo;
            searchModel.Description = DescriptionTo;
            ModelState.Remove("Code");
            HuMemo newHuMemo = genericMgr.FindById<HuMemo>(id);
            newHuMemo.Code = id;
            newHuMemo.Description = huMemo.Description;
            newHuMemo.ResourceGroup = (com.Sconit.CodeMaster.ResourceGroup)Enum.Parse(typeof(com.Sconit.CodeMaster.ResourceGroup), huMemo.ResourceGroupDescription, true);
            genericMgr.UpdateWithTrim(newHuMemo);
  
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<HuMemo>(searchStatementModel, command));
        }


        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, HuMemoSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "h", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "h", ref whereStatement, param);
           
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
