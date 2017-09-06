using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models;
using com.Sconit.Entity.INP;
using com.Sconit.Service;
using com.Sconit.Web.Models.SearchModels.INP;
using com.Sconit.Entity;
using System.IO;

namespace com.Sconit.Web.Controllers.INP
{
    public class InspectResultExtController : WebAppBaseController
    {

        //public IGenericMgr genericMgr { get; set; }
        //
        // GET: /InspectResultExt/
        #region  public
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_InspectResultExt_View")]
        public ActionResult List(GridCommand command, InspectResultSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (!string.IsNullOrEmpty(((InspectResultSearchModel)(searchCacheModel.SearchObject)).IpNo))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_NeedInputShipOrderToSearch);
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_InspectResultExt_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, string IpNo)
        {
            if (string.IsNullOrEmpty(IpNo))
            {
                return PartialView(new GridModel(new List<InspectResult>()));
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, IpNo);
            return PartialView(GetAjaxPageData<InspectResult>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_InspectResultExt_View")]
        public ActionResult Edit(string id)
        {
            InspectResultExt InspectResultExt = new InspectResultExt();
            InspectResultExt.InspectResultId = int.Parse(id);

            IList<InspectResultExt> InspectResultExtList = genericMgr.FindAll<InspectResultExt>("select i from InspectResultExt as i where i.InspectResultId=?", id);
            if (InspectResultExtList.Count > 0)
            {
                InspectResultExt = InspectResultExtList[0];
            }
            if (string.IsNullOrEmpty(InspectResultExt.Checker))
            {
                com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                InspectResultExt.Checker = user.FullName;
            }
            return View(InspectResultExt);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_InspectResultExt_View")]
        public ActionResult Edit(InspectResultExt InspectResultExt)
        {
            try
            {
                if (string.IsNullOrEmpty(InspectResultExt.Checker))
                {
                    com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                    InspectResultExt.Checker = user.FullName;
                }
                if (!string.IsNullOrEmpty(InspectResultExt.File))
                {
                    FileStream fs = new FileStream(InspectResultExt.File, FileMode.Open, FileAccess.Read);
                    byte[] picBytes = new byte[fs.Length];
                    fs.Read(picBytes, 0, Convert.ToInt32(fs.Length));
                    InspectResultExt.Picture = picBytes;
                    fs.Close();
                }
                else
                {
                    InspectResultExt.Picture = this.genericMgr.FindById<InspectResultExt>(InspectResultExt.Id).Picture;
                }
                IList<InspectResultExt> InspectResultExtList = genericMgr.FindAll<InspectResultExt>("select i from InspectResultExt as i where i.InspectResultId=?", InspectResultExt.InspectResultId);
                if (InspectResultExtList.Count > 0)
                {
                    this.genericMgr.UpdateWithTrim(InspectResultExt);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(InspectResultExt);
                }
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_EntranceLogisticsInformationUpdatedSuccessfully);

            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message.ToString());
            }
            return View(InspectResultExt);
        }


        public ActionResult ProductShow(string id)
        {
            try
            {
                byte[] imageData = new byte[] { };
                if (!string.IsNullOrEmpty(id) && int.Parse(id) != 0)
                {
                    imageData = genericMgr.FindById<InspectResultExt>(int.Parse(id)).Picture;//从数据读取图片;  
                }
                return new FileContentResult(imageData, "image/jpeg");
                
            }
            catch (Exception)
            {
                
                throw;
            }
            
            
            
        }



        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, string IpNo)
        {
            IList<object> param = new List<object>();
            string whereStatement = string.Empty;
            HqlStatementHelper.AddLikeStatement("IpNo", IpNo, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by i.CreateDate desc";
            }



            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from InspectResult as i";
            searchStatementModel.SelectStatement = "select i from InspectResult as i";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

    }
}
