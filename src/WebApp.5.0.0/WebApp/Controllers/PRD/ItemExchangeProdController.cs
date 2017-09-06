using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Web.Models;
using com.Sconit.Service;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using AutoMapper;
using com.Sconit.PrintModel.INV;
using com.Sconit.Utility.Report;
using com.Sconit.Entity.MRP.MD;
using System.Data.SqlClient;
using System.Data;

namespace com.Sconit.Web.Controllers.PRD
{
    public class ItemExchangeProdController : ReportBaseController
    {

        //
        // GET: /ItemExchange/

        private static string selectCountStatement = "select count(*) from ItemExchange as i";
        private static string selectStatement = "select i from ItemExchange as i";

        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the address security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //public IItemMgr itemMgr { get; set; }

        public ICustomizationMgr customizationMgr { get; set; }

        //public IReportGen reportGen { get; set; }
        #endregion

        #region public

        #region Aging

        [SconitAuthorize(Permissions = "Url_ItemExchange_Ageing")]
        public ActionResult AgeingIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ItemExchange_Ageing")]
        public JsonResult Ageing(string oldHu, DateTime? effectiveDate)
        {
            try
            {
                if (string.IsNullOrEmpty(oldHu))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BarcodeCanNotBeEmpty);
                }
                Hu hu = this.genericMgr.FindById<Hu>(oldHu);
                effectiveDate = effectiveDate.HasValue ? effectiveDate : DateTime.Now;
                Hu newHu = customizationMgr.AgedHu(hu, effectiveDate.Value);

                string url = Print(newHu);
                object obj = new { url = url };
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

        [GridAction]
        public ActionResult _AgeingList(GridCommand command)
        {
            //  IList<ItemExchange> itenExchangeList = this.genericMgr.FindAll<ItemExchange>("from ItemExchange as i where i.ItemExchangeType=?", com.Sconit.CodeMaster.ItemExchangeType.Aging);

            //return PartialView(itenExchangeList);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxAging(GridCommand command)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, (int)com.Sconit.CodeMaster.ItemExchangeType.Aging);
            return PartialView(GetAjaxPageData<ItemExchange>(searchStatementModel, command));
        }

        public string CancelAging(string id)
        {
            try
            {
                customizationMgr.CancelItemExchangeHu(int.Parse(id), DateTime.Now);
                return id;

            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return string.Empty;
            }
        }

        #endregion

        #region Fliter

        [SconitAuthorize(Permissions = "Url_ItemExchange_Filter")]
        public ActionResult FilterIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ItemExchange_Filter")]
        public JsonResult Filter(string oldHu, int qty, DateTime? effectiveDate)
        {
            try
            {
                if (string.IsNullOrEmpty(oldHu))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BarcodeCanNotBeEmpty);
                }
                Hu hu = this.genericMgr.FindById<Hu>(oldHu);
                effectiveDate = effectiveDate.HasValue ? effectiveDate : DateTime.Now;
                Hu newHu = customizationMgr.FilterHu(hu, qty, effectiveDate.Value);

                string url = Print(newHu);
                object obj = new { url = url };
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

        [GridAction]
        public ActionResult _FilterList(GridCommand command)
        {
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxFilter(GridCommand command, string OldHu, DateTime? EffectiveDate,int? Id)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, Id,(int)com.Sconit.CodeMaster.ItemExchangeType.Filter, OldHu, EffectiveDate);
            return PartialView(GetAjaxPageData<ItemExchange>(searchStatementModel, command));
        }

        #endregion



        #endregion

        #region Print
        public string Print(Hu hu)
        {
            string HuTemplate = hu.HuTemplate;

            IList<PrintHu> huList = new List<PrintHu>();

            PrintHu printHu = Mapper.Map<Hu, PrintHu>(hu);
            if (!string.IsNullOrWhiteSpace(hu.ManufactureParty))
            {
                printHu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
            }
            if (!string.IsNullOrWhiteSpace(hu.Direction))
            {
                printHu.Direction = this.genericMgr.FindById<HuTo>(printHu.Direction).CodeDescription;
            }
            if (string.IsNullOrWhiteSpace(hu.HuTemplate))
            {
                HuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
            }
            huList.Add(printHu);
            IList<object> data = new List<object>();
            data.Add(huList);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile(HuTemplate, data);
        }

        public string PrintTo(string huId)
        {
            Hu hu = this.genericMgr.FindById<Hu>(huId);
            return Print(hu);
        }
        #endregion

        #region private

        private SearchStatementModel PrepareSearchStatement(GridCommand command, int ItemExchangeType)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("ItemExchangeType", ItemExchangeType, "i", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by i.Id desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        private SearchStatementModel PrepareSearchStatement(GridCommand command,int? Id, int ItemExchangeType, string OldHu,DateTime? EffectiveDate)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Id", Id, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ItemExchangeType", ItemExchangeType, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("OldHu", OldHu, HqlStatementHelper.LikeMatchMode.Anywhere, "i", ref whereStatement,param);
            if (EffectiveDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("EffectiveDate", EffectiveDate, EffectiveDate.Value.AddDays(1), "i", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by i.Id desc";
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
        public ActionResult AgeingSearch()
        {
            LocationLotDetailSearchModel serch = new LocationLotDetailSearchModel();
            TempData["LocationLotDetailSearchModel"] = serch;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ItemExchange_Ageing_Search")]
        public ActionResult AgeingList(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            TempData["LocationLotDetailSearchModel"] = searchModel;
            ViewBag.Page = 1;
            ViewBag.HuOption = searchModel.HuOption;
            ViewBag.PageSize = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(com.Sconit.Entity.SYS.EntityPreference.CodeEnum.MaxRowSizeOnPage)); ;
            if (searchModel.Location == null)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_LocationCodeCanNotBeEmpty);
                return View();
            }

            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ItemExchange_Ageing_Search")]
        public ActionResult _AjaxList(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            command.Page = 1;

            //command.PageSize = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(com.Sconit.Entity.SYS.EntityPreference.CodeEnum.MaxRowSizeOnPage)); ;
            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<Hu> gridModel = GetHuAjaxPageData<Hu>(reportSearchStatementModel);
            FillCodeDetailDescription<Hu>(gridModel.Data.ToList());
            return PartialView(gridModel);
        }

        private ReportSearchStatementModel PrepareSearchStatement(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            searchModel.HuOption = searchModel.HuOption ?? 4;
            searchModel.HuOption = searchModel.HuOption == 0 ? 4 : searchModel.HuOption;

            ReportSearchStatementModel reportSearchStatementModel = new ReportSearchStatementModel();
            reportSearchStatementModel.ProcedureName = "USP_Report_AngingSearch";

            SqlParameter[] parameters = new SqlParameter[6];

            parameters[0] = new SqlParameter("@Location", SqlDbType.VarChar, 8000);
            parameters[0].Value = searchModel.Location;

            parameters[1] = new SqlParameter("@Item", SqlDbType.VarChar, 8000);
            parameters[1].Value = searchModel.Item;

            parameters[2] = new SqlParameter("@LotNo", SqlDbType.VarChar);
            parameters[2].Value = searchModel.LotNo;

            parameters[3] = new SqlParameter("@HuOption", SqlDbType.VarChar);
            parameters[3].Value = searchModel.HuOption;

            parameters[4] = new SqlParameter("@IsIncludeEmptyStock", SqlDbType.Bit);
            parameters[4].Value = searchModel.IsIncludeEmptyStock;

            parameters[5] = new SqlParameter("@IsIncludeNoNeedAging", SqlDbType.Bit);
            parameters[5].Value = searchModel.IsIncludeNoNeedAging;
            //parameters[3] = new SqlParameter("@PageSize", SqlDbType.Int);
            //parameters[3].Value = command.PageSize;

            //parameters[4] = new SqlParameter("@Page", SqlDbType.Int);
            //parameters[4].Value = command.Page;

            reportSearchStatementModel.Parameters = parameters;

            return reportSearchStatementModel;
        }
        #region 导出老化查询
        [SconitAuthorize(Permissions = "Url_ItemExchange_Ageing_Search")]
        public void ExportXLS(LocationLotDetailSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(com.Sconit.Entity.SYS.EntityPreference.CodeEnum.MaxRowSizeOnPage));
            searchModel.Level = ((int)com.Sconit.CodeMaster.LocationLevel.Donotcollect).ToString();
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);

            var fileName = string.Format("Ageing.{0}.xls", searchModel.Location);
            if (searchModel.HuOption == 4)
            {
                fileName = string.Format("AgeingSumByLocation.{0}.xls", searchModel.Location);
            }
            GridModel<Hu> gridModel = GetHuAjaxPageData<Hu>(reportSearchStatementModel);
            FillCodeDetailDescription<Hu>(gridModel.Data.ToList());
            ExportToXLS<Hu>(fileName, gridModel.Data.ToList());
        }

        #endregion

    }
}

