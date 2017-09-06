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
    public class PalletController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from Pallet as p";
        private static string selectStatement = "select p from Pallet as p";


        public IGenericMgr genericMgr { get; set; }

        public IHuMgr huMgr { get; set; }

        public INumberControlMgr numberControlMgr { get; set; }


        #region public method
        [SconitAuthorize(Permissions = "Url_Inventory_Pallet_View")]
        public ActionResult Index()
        {
            ViewBag.CreateUserName = this.CurrentUser.FullName;
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_Pallet_New")]
        public ActionResult List(GridCommand command, PalletSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_Inventory_Pallet_New")]
        public ActionResult _AjaxList(GridCommand command, PalletSearchModel searchModel)
        {
            TempData["PalletSearchModel"] = searchModel;
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            var list = GetAjaxPageData<Pallet>(searchStatementModel, command);
            //foreach (var data in list.Data)
            //{
            //    data.HuStatus = huMgr.GetHuStatus(data.HuId);
            //    data.ItemDescription = string.IsNullOrWhiteSpace(data.ReferenceItemCode) ? data.ItemDescription : data.ItemDescription + "[" + data.ReferenceItemCode + "]";
            //}

            return PartialView(list);
        }

        [SconitAuthorize(Permissions = "Url_Inventory_Pallet_View")]
        public ActionResult Edit(string id)
        {
            Pallet pallet = genericMgr.FindById<Pallet>(id);

            return View("Edit", string.Empty, pallet);
        }


        [SconitAuthorize(Permissions = "Url_Inventory_Pallet_View")]
        public ActionResult _PalletHuList(string palletCode)
        {
            string hql = "select p from PalletHu as p where p.PalletCode = ?";
            IList<PalletHu> palletHuList = genericMgr.FindAll<PalletHu>(hql, palletCode);
            //foreach (var receiptDetail in receiptDetailList)
            //{
            //    receiptDetail.ReceiptLocationDetails = this.genericMgr.FindAll<ReceiptLocationDetail>
            //        ("from ReceiptLocationDetail where ReceiptDetailId =? ", receiptDetail.Id);
            //}
            return PartialView(palletHuList);
        }


        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Inventory_Pallet_New")]
        public ActionResult New()
        {
            return View();
        }


        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Inventory_Pallet_New")]
        public ActionResult New(Pallet pallet)
        {
            if (ModelState.IsValid)
            {
                pallet.Code = numberControlMgr.GetPalletCode();

                genericMgr.CreateWithTrim(pallet);
                SaveSuccessMessage(Resources.INV.Pallet.Pallet_Added);
                return RedirectToAction("Edit/" + pallet.Code);
            }

            return View(pallet);
        }


        #region 打印导出
        [HttpPost]
        public void SaveToClient(string code)
        {
            string huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
            string[] checkedOrderArray = code.Split(',');
            string selectStatement = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedOrderArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = "from Hu where HuId in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                selectPartyPara.Add(para);
            }
            selectStatement += ")";

            IList<Hu> huList = genericMgr.FindAll<Hu>(selectStatement, selectPartyPara.ToArray());
            foreach (var hu in huList)
            {
                if (!string.IsNullOrEmpty(hu.ManufactureParty))
                {
                    hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                }
                if (!string.IsNullOrWhiteSpace(hu.HuTemplate))
                {
                    huTemplate = hu.HuTemplate;
                }

                if (!string.IsNullOrWhiteSpace(hu.Direction))
                {
                    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                }
            }
            IList<PrintHu> printHu = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
            IList<object> data = new List<object>();
            data.Add(printHu);
            data.Add(CurrentUser.FullName);
            reportGen.WriteToClient(huTemplate, data, huTemplate);
        }

        [HttpPost]
        public void SaveToClientTo(string checkedOrders)
        {
            string defaultHuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);

            string[] checkedOrderArray = checkedOrders.Split(',');
            string selectStatement = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedOrderArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = "from Hu where HuId in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                selectPartyPara.Add(para);
            }
            selectStatement += ")";

            IList<Hu> huList = genericMgr.FindAll<Hu>(selectStatement, selectPartyPara.ToArray());
            foreach (var hu in huList)
            {
                if (!string.IsNullOrEmpty(hu.ManufactureParty))
                {
                    hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                }
                if (string.IsNullOrWhiteSpace(hu.HuTemplate))
                {
                    hu.HuTemplate = defaultHuTemplate;
                }
                if (!string.IsNullOrWhiteSpace(hu.Direction))
                {
                    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                }
            }
            var huGroupList = huList.GroupBy(p => p.HuTemplate, (k, g) => new { k, g });
            foreach (var huGroup in huGroupList)
            {
                IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huGroup.g.ToList());
                IList<object> data = new List<object>();
                data.Add(printHuList);
                data.Add(CurrentUser.FullName);
                reportGen.WriteToClient(huGroup.k, data, huGroup.k);
            }
        }

        public string Print(string code)
        {
            Pallet pallet = queryMgr.FindById<Pallet>(code);
            IList<Hu> palletHuList = genericMgr.FindAll<Hu>("from Hu where PalletCode = ?", code);

            if (palletHuList == null || palletHuList.Count == 0)
            {
                throw new BusinessException("托盘明细为空");
            }

            Hu palletHu = palletHuList.First();
            IList<PrintHu> huList = new List<PrintHu>();
            PrintHu printHu = Mapper.Map<Hu, PrintHu>(palletHu);


            PrintHu hu = new PrintHu();
            hu.CreateDate = pallet.CreateDate;
            hu.CreateUserId = pallet.CreateUserId;
            hu.CreateUserName = pallet.CreateUserName;
            hu.Item = printHu.Item;
            hu.ItemDescription = printHu.ItemDescription;
            hu.HuId = pallet.Code;
            hu.PalletCode = pallet.Code;
            hu.ManufactureParty = printHu.ManufactureParty;
            hu.ManufacturePartyDescription = genericMgr.FindById<Party>(printHu.ManufactureParty).Name;
            hu.ManufactureDate = printHu.ManufactureDate;
            hu.LotNo = printHu.LotNo;
            hu.OrderNo = printHu.OrderNo;

            hu.Qty = palletHuList.Sum(p => p.Qty);
            hu.Uom = palletHu.Uom + "(" + palletHuList.Count() + "箱)";

            hu.ExternalOrderNo = printHu.ExternalOrderNo;


            huList.Add(hu);

            IList<object> data = new List<object>();
            data.Add(huList);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile("Pallet.xls", data);



        }
        [HttpPost]
        public JsonResult CheckExportTemplate(string checkedOrders)
        {
            string[] checkedOrderArray = checkedOrders.Split(',');
            string selectStatement = string.Empty;
            List<object> huIds = new List<object>();
            foreach (var checkedOrder in checkedOrderArray)
            {
                huIds.Add(checkedOrder);
            }
            var templateCount = genericMgr.FindAllIn<Hu>("from Hu as i where i.HuId in (?", huIds).Select(o => o.HuTemplate).Distinct().Count();
            var message = "OK";
            if (templateCount > 1)
            {
                message = string.Format(Resources.EXT.ControllerLan.Con_SelectedBarcodePrintedTemplatesAreInconsistent);
            }
            return Json(new { Message = message });
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_Pallet_New")]
        public JsonResult _PrintHuList(string checkedOrders)
        {
            string[] checkedOrderArray = checkedOrders.Split(',');
            string selectStatement = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedOrderArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = "from Hu where HuId in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                selectPartyPara.Add(para);
            }
            selectStatement += ")";

            IList<Hu> huList = genericMgr.FindAll<Hu>(selectStatement, selectPartyPara.ToArray());
            var defaultHuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
            foreach (var hu in huList)
            {
                if (!string.IsNullOrEmpty(hu.ManufactureParty))
                {
                    hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                }

                if (string.IsNullOrWhiteSpace(hu.HuTemplate))
                {
                    hu.HuTemplate = defaultHuTemplate;
                }
                //if (!string.IsNullOrWhiteSpace(hu.Direction))
                //{
                //    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                //}
            }
            var huGroupList = huList.GroupBy(p => p.HuTemplate, (k, g) => new { k, g });
            List<string> printUrls = new List<string>();
            foreach (var huGroup in huGroupList)
            {
                string reportFileUrl = PrintHuList(huGroup.g.ToList(), huGroup.k);
                printUrls.Add(reportFileUrl);
            }
            object obj = new { SuccessMessage = Resources.EXT.ControllerLan.Con_BarcodePrintedSuccessfully, PrintUrl = printUrls };
            return Json(obj);
        }

        public string PrintHuList(IList<Hu> huList, string huTemplate)
        {
            foreach (var hu in huList)
            {
                if (!string.IsNullOrWhiteSpace(hu.Direction))
                {
                    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                }
            }
            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);

            IList<object> data = new List<object>();
            data.Add(printHuList);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile(huTemplate, data);
        }



        #endregion

        #endregion

        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PalletSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Code", searchModel.Code, "p", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Anywhere, "p", ref whereStatement, param);



            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "p", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "p", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "p", ref whereStatement, param);
            }


            if (!string.IsNullOrEmpty(searchModel.HuId))
            {
                if (string.IsNullOrEmpty(whereStatement))
                {
                    whereStatement += "  where exists (select 1 from PalletHu as h  where h.PalletCode = p.Code and h.HuId = '" + searchModel.HuId + "') ";
                }
                else
                {
                    whereStatement += " and exists (select 1 from PalletHu as h  where h.PalletCode = p.Code and h.HuId = '" + searchModel.HuId + "') ";
                }
            }
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
