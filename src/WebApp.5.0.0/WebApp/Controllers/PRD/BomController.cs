/// <summary>
/// Summary description for BomController
/// </summary>
namespace com.Sconit.Web.Controllers.PRD
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using com.Sconit.Entity.PRD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.PRD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using System.Web.Routing;
    using System;
    using System.Web;

    public class BomController : WebAppBaseController
    {
        private static string selectBomMasterCountStatement = "select count(*) from BomMaster as bm";
        private static string selectBomMasterStatement = "select bm from BomMaster as bm";
        private static string DuiplicateBomMasterVerifyStatement = @"select count(*) from BomMaster as bm where bm.Code = ?";

        private static string selectBomDetailCountStatement = "select count(*) from BomDetail as bd";
        private static string selectBomDetailStatement = "select bd from BomDetail as bd";
        private static string DuiplicateBomDetailVerifyStatement = @"select count(*) from BomDetail as bd where bd.Bom = ? and bd.Item = ?";

        //public IGenericMgr genericMgr { get; set; }
        public IBomMgr bomMgr { get; set; }

        [SconitAuthorize(Permissions = "Url_Bom_View")]
        public ActionResult Index()
        {
            return View();
        }

        #region BomMaster
        [SconitAuthorize(Permissions = "Url_Bom_View")]
        public ActionResult _Search_Master()
        {
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Bom_View")]
        public ActionResult List_Master(GridCommand command, BomMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Bom_View")]
        public ActionResult _AjaxList_Master(GridCommand command, BomMasterSearchModel searchModel)
        {
            string replaceFrom = "_AjaxList_Master";
            string replaceTo = "List_Master/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = PrepareBomMasterSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<BomMaster>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult _New_Master()
        {
            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult _New_Master(BomMaster bomMaster)
        {
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                if (this.genericMgr.FindAll<long>(DuiplicateBomMasterVerifyStatement, new object[] { bomMaster.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, bomMaster.Code);
                }
                else
                {
                    genericMgr.CreateWithTrim(bomMaster);
                    SaveSuccessMessage(Resources.PRD.Bom.BomMaster_Added);
                    return RedirectToAction("_Edit_Master/" + bomMaster.Code);
                }
            }
            return PartialView(bomMaster);
        }
        [HttpPost]
        //  [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public JsonResult _SearchResult_Master(string id)
        {
            if (id == null)
            {
                return Json(null);
            }
            else
            {
                BomMaster bomMaster = genericMgr.FindById<BomMaster>(id);
                return Json(bomMaster);
            }
        }
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult _Edit_Master(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                BomMaster bomMaster = genericMgr.FindById<BomMaster>(id);
                return PartialView(bomMaster);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult _Edit_Master(BomMaster bomMaster)
        {
            if (ModelState.IsValid)
            {
                genericMgr.UpdateWithTrim(bomMaster);
                bomMgr.ResetBomCache();
                SaveSuccessMessage(Resources.PRD.Bom.BomMaster_Updated);
            }

            return PartialView(bomMaster);
        }

        [SconitAuthorize(Permissions = "Url_Bom_Delete")]
        public ActionResult Delete_Master(string id)
        {
            try
            {
                if (id == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    genericMgr.DeleteById<BomMaster>(id);
                    SaveSuccessMessage(Resources.PRD.Bom.BomMaster_Deleted);
                    return RedirectToAction("List_Master");
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return null;
        }
        #endregion

        #region BomDetail
        [SconitAuthorize(Permissions = "Url_Bom_View")]
        public ActionResult _Search_Detail()
        {
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Bom_View")]
        public ActionResult List_Detail(GridCommand command, BomDetailSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Bom_View")]
        public ActionResult _AjaxList_Detail(GridCommand command, BomDetailSearchModel searchModel)
        {
            string replaceFrom = "_AjaxList_Detail";
            string replaceTo = "List_Detail/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = PrepareBomDetailSearchStatement(command, searchModel);
            var result = GetAjaxPageData<BomDetail>(searchStatementModel, command);
            if (result.Data != null && result.Data.Count() > 0)
            {
                var allMasterBom = this.genericMgr.FindAll<BomMaster>(string.Format("select b from BomMaster as b where b.Code in ('{0}')", string.Join("','", result.Data.Select(d => d.Bom).ToArray())));
                foreach (var bomDetail in result.Data)
                {
                    bomDetail.CurrentBom = allMasterBom.Where(a => a.Code == bomDetail.Bom).FirstOrDefault();
                    var item = this.itemMgr.GetCacheItem(bomDetail.Item);
                    bomDetail.ItemDescription = item.Description;
                    bomDetail.ReferenceItemCode = item.ReferenceCode;
                    bomDetail.Description = bomDetail.CurrentBom.Description;
                    bomDetail.Qty = bomDetail.CurrentBom.Qty;
                    bomDetail.MstrUom = bomDetail.CurrentBom.Uom;
                    bomDetail.IsActive = bomDetail.CurrentBom.IsActive;
                }
            }
            return PartialView(result);
        }

        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult _New_Detail()
        {
            BomDetail bomDetail = new BomDetail();
            bomDetail.ScrapPercentage = 0;
            return PartialView(bomDetail);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult _New_Detail(BomDetail bomDetail)
        {
            ModelState.Remove("Item.Description");
            if (ModelState.IsValid)
            {
                //判断描述不能重复
                if (this.genericMgr.FindAll<long>(DuiplicateBomDetailVerifyStatement, new object[] { bomDetail.Bom, bomDetail.Item })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, bomDetail.Id.ToString());
                }
                else
                {
                    genericMgr.CreateWithTrim(bomDetail);
                    bomMgr.ResetBomCache();
                    SaveSuccessMessage(Resources.PRD.Bom.BomDetail_Added);
                    return RedirectToAction("_Edit_Detail/" + bomDetail.Id);
                }
            }
            return PartialView(bomDetail);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult _Edit_Detail(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                BomDetail bomDetail = genericMgr.FindById<BomDetail>(int.Parse(id));
                var item = this.itemMgr.GetCacheItem(bomDetail.Item);
                bomDetail.ItemDescription = item.Description;
                bomDetail.ReferenceItemCode = item.ReferenceCode;
                bomDetail.CurrentBom = genericMgr.FindById<BomMaster>(bomDetail.Bom);
                return PartialView(bomDetail);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult _Edit_Detail(BomDetail bomDetail)
        {
            ModelState.Remove("Item.Description");
            if (ModelState.IsValid)
            {
                genericMgr.UpdateWithTrim(bomDetail);
                bomMgr.ResetBomCache();
                SaveSuccessMessage(Resources.PRD.Bom.BomDetail_Updated);
            }

            return PartialView(bomDetail);
        }

        [SconitAuthorize(Permissions = "Url_Bom_Delete")]
        public ActionResult Delete_Detail(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<BomDetail>(int.Parse(id));
                bomMgr.ResetBomCache();
                SaveSuccessMessage(Resources.PRD.Bom.BomDetail_Deleted);
                return RedirectToAction("List_Detail");
            }
        }
        #endregion

        private SearchStatementModel PrepareBomMasterSearchStatement(GridCommand command, BomMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.BomMaster_Code, HqlStatementHelper.LikeMatchMode.Start, "bm", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectBomMasterCountStatement;
            searchStatementModel.SelectStatement = selectBomMasterStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel PrepareBomDetailSearchStatement(GridCommand command, BomDetailSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Bom", searchModel.BomDetail_Bom, HqlStatementHelper.LikeMatchMode.Start, "bd", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Item", searchModel.BomDetail_Item, HqlStatementHelper.LikeMatchMode.Start, "bd", ref whereStatement, param);

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StructureTypeDescription")
                {
                    command.SortDescriptors[0].Member = "StructureType";
                }
                if (command.SortDescriptors[0].Member == "BackFlushMethodDescription")
                {
                    command.SortDescriptors[0].Member = "BackFlushMethod";
                }
                if (command.SortDescriptors[0].Member == "FeedMethodDescription")
                {
                    command.SortDescriptors[0].Member = "FeedMethod";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectBomDetailCountStatement;
            searchStatementModel.SelectStatement = selectBomDetailStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        #region Import
        [SconitAuthorize(Permissions = "Url_Bom_Edit")]
        public ActionResult Import(IEnumerable<HttpPostedFileBase> bomattachments)
        {
            try
            {
                foreach (var file in bomattachments)
                {
                    bomMgr.ImportBom(file.InputStream);
                    //bomMgr.ProcessSectionBom();
                }
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_BomSuccessfullyImport);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Content(string.Empty);
        }

        #endregion
        #region  Export detail search
        [SconitAuthorize(Permissions = "Url_Bom_View")]
        [GridAction(EnableCustomBinding = true)]
        public void Export(BomDetailSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            SearchStatementModel searchStatementModel = PrepareBomDetailSearchStatement(command, searchModel);
            var result = GetAjaxPageData<BomDetail>(searchStatementModel, command);
            if (result.Data != null && result.Data.Count() > 0)
            {
                var allMasterBom = this.genericMgr.FindAll<BomMaster>(string.Format("select b from BomMaster as b where b.Code in ('{0}')", string.Join("','", result.Data.Select(d => d.Bom).ToArray())));
                foreach (var bomDetail in result.Data)
                {
                    bomDetail.CurrentBom = allMasterBom.Where(a => a.Code == bomDetail.Bom).FirstOrDefault();
                    var item = this.itemMgr.GetCacheItem(bomDetail.Item);
                    bomDetail.ItemDescription = item.Description;
                    bomDetail.ReferenceItemCode = item.ReferenceCode;
                    bomDetail.Description = bomDetail.CurrentBom.Description;
                    bomDetail.Qty = bomDetail.CurrentBom.Qty;
                    bomDetail.MstrUom = bomDetail.CurrentBom.Uom;
                    bomDetail.IsActive = bomDetail.CurrentBom.IsActive;
                }
            }
            ExportToXLS<BomDetail>("BomDet.xls", result.Data.ToList());
        }
        #endregion
    }

}
