using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Web.Models;
using System.Web.Routing;
using com.Sconit.Entity.SYS;
using com.Sconit.Service;
using com.Sconit.Web.Models.SearchModels.MD;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.MD;
using com.Sconit.Web.Models.SearchModels.SYS;


namespace com.Sconit.Web.Controllers.SYS
{
    public class CodeMasterController : WebAppBaseController
    {


        private static string selectCountStatement = "select count(*) from CodeMaster as r";

        private static string selectStatement = "select r from CodeMaster as r";

        //public IGenericMgr genericMgr { get; set; }

        //
        // GET: /CodeMaster/
        [SconitAuthorize(Permissions = "Url_Region_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        public ActionResult Update(int id, string value, int Sequence, bool isDefault)
        {
           
            CodeDetail CodeDetail = this.genericMgr.FindById<CodeDetail>((int)id);
            CodeDetail.Value = value;
            CodeDetail.Sequence = Sequence;
            CodeDetail.IsDefault = isDefault;
            if (isDefault)
            {
                this.genericMgr.Update("update CodeDetail as d set d.IsDefault=false where d.Code = ? ", CodeDetail.Code);
            }

            this.genericMgr.Update(CodeDetail);

            IList<CodeDetail> codeDetailList = genericMgr.FindAll<CodeDetail>("select d from CodeDetail as d where d.Code = ?", CodeDetail.Code);
            foreach (CodeDetail codeDetail in codeDetailList)
            {
                string description = Resources.SYS.CodeDetail.ResourceManager.GetString(codeDetail.Description);
                codeDetail.Description = description != null ? description : codeDetail.Description;
            }
            return View(new GridModel(codeDetailList));
        }

         /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CodeMaster_View")]
        public ActionResult List(GridCommand command, CodeMasterModel searchModel)
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
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, CodeMasterModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CodeMaster_View")]
        public ActionResult _AjaxList(GridCommand command, CodeMasterModel searchModel)
        {
            string replaceFrom = "_AjaxList";
            string replaceTo = "List";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);

            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<com.Sconit.Entity.SYS.CodeMaster>(searchStatementModel, command));
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CodeMaster_Edit")]
        public ActionResult Edit(string Id)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                com.Sconit.Entity.SYS.CodeMaster codeMaster = this.genericMgr.FindById<com.Sconit.Entity.SYS.CodeMaster>(Id);
                return View(codeMaster);
            }


        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CodeMaster_Edit")]
        public ActionResult _List(string Id, int type)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.type = type;

                IList<CodeDetail> codeDetailList = genericMgr.FindAll<CodeDetail>("select d from CodeDetail as d where d.Code = ?", Id);
                foreach (CodeDetail codeDetail in codeDetailList)
                {
                    string description = Resources.SYS.CodeDetail.ResourceManager.GetString(codeDetail.Description);
                    codeDetail.Description = description != null ? description : codeDetail.Description;
                }
                return PartialView(codeDetailList);

            }

        }

    }
}
