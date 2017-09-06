using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.BIL;
using com.Sconit.Web.Models;
using com.Sconit.Entity.BIL;
using com.Sconit.Service;
using com.Sconit.Entity.Exception;
using com.Sconit.PrintModel.BILL;
using AutoMapper;
using com.Sconit.Utility.Report;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Web.Controllers.BIL
{
    public class DistributionBillController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public IBillMgr billMgr { get; set; }
        //public IReportGen reportGen { get; set; }

        #region hql
        //采购账单BillMaster

        private static string selectCountStatement = "select count(*) from BillMaster as b";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select b from BillMaster as b";

        //采购账单ActingBill

        private static string selectCountActingBillStatement = "select count(*) from ActingBill as a";

        /// <summary>
        /// 
        /// </summary>
        private static string selectActingBillStatement = "select a from ActingBill as a";

        #endregion

        #region View
        [SconitAuthorize(Permissions = "Url_DistributionBill_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_DistributionBill_View")]
        public ActionResult List(GridCommand command, BillMasterSearchModel searchModel)
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
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionBill_View")]
        public ActionResult _AjaxList(GridCommand command, BillMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<BillMaster>()));
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<BillMaster>(searchStatementModel, command));

        }
        #region Export 账单查询
        [SconitAuthorize(Permissions = "Url_DistributionBill_View")]
        public void ExportXLSSearch(BillMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            IList<BillMaster> actingBillLists = GetAjaxPageData<BillMaster>(searchStatementModel, command).Data.ToList();
            ExportToXLS<BillMaster>("SaleBillSearch.xls", actingBillLists);
        }
        #endregion
        [GridAction]
        [SconitAuthorize(Permissions = "Url_DistributionBill_View")]
        public ActionResult _BillGroupDetailList(string BillNo)
        {
            BillMaster billMaster = genericMgr.FindById<BillMaster>(BillNo);
            ViewBag.BilledAmount = billMaster.Amount;
            ViewBag.Status = billMaster.Status;
            IList<object> ArrayPara = new List<object>();
            ArrayPara.Add(BillNo);
            IList<BillDetail> billDetails = genericMgr.FindAll<BillDetail>(" from  BillDetail b where  b.BillNo=? ", ArrayPara.ToArray());
            foreach (BillDetail billDetail in billDetails)
            {
                ActingBill actBill = genericMgr.FindById<ActingBill>(billDetail.ActingBillId);
                billDetail.BillQty = actBill.BillQty;
                billDetail.BilledQty = actBill.BillingQty - billDetail.Qty;
                billDetail.CurrentBillQty = billDetail.Qty;
                billDetail.CurrentBillAmount = billDetail.Amount;
            }
            this.billMgr.GroupBillDetailByItem(ref billDetails);
            return PartialView(billDetails);
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_DistributionBill_View")]
        public ActionResult _BillDetailList(string BillNo)
        {
            BillMaster Billmaster = genericMgr.FindById<BillMaster>(BillNo);
            ViewBag.BilledAmount = Billmaster.Amount;
            ViewBag.Status = Billmaster.Status;

            IList<object> arrayPara = new List<object>();
            arrayPara.Add(BillNo);
            IList<BillDetail> billdetail = null;

            billdetail = genericMgr.FindAll<BillDetail>(" from  BillDetail b where  b.BillNo=? ", arrayPara.ToArray());
            foreach (BillDetail billDetail in billdetail)
            {
                ActingBill actBill = genericMgr.FindById<ActingBill>(billDetail.ActingBillId);
                billDetail.BillQty = actBill.BillQty;
                billDetail.BilledQty = actBill.BillingQty - billDetail.Qty;
                billDetail.CurrentBillQty = billDetail.Qty;
                billDetail.CurrentBillAmount = billDetail.Amount;
            }
            return PartialView(billdetail);
        }
        #endregion

        #region Edit


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_DistributionBill_Edit")]
        public ActionResult Edit(string BillNo, string groupOrDetail)
        {
            ViewBag.groupOrDetail = groupOrDetail == string.Empty ? "0" : groupOrDetail;
            BillMaster Billmaster = genericMgr.FindById<BillMaster>(BillNo);
            return View(Billmaster);
        }

        /// <summary>
        /// 释放后保存头
        /// </summary>
        /// <param name="billNo"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceDate"></param>
        /// <returns></returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_DistributionBill_Edit")]
        public JsonResult UpdateBillMaster(string billNo, string invoiceNo, string externalBillNo, DateTime? invoiceDate)
        {
            try
            {
                if (string.IsNullOrEmpty(billNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BillAccountCanNotBeEmpty);
                }
                else
                {
                    BillMaster billmaster = genericMgr.FindById<BillMaster>(billNo);
                    billmaster.InvoiceNo = invoiceNo;
                    billmaster.ExternalBillNo = externalBillNo;
                    billmaster.InvoiceDate = invoiceDate;
                    this.genericMgr.Update(billmaster);
                    // SaveSuccessMessage(Resources.MD.Address.Address_Updated);
                    object obj = new { SuccessMessage = string.Format(Resources.MD.Address.Address_Updated) };
                    return Json(obj);

                }
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionBill_Edit")]
        public JsonResult UpdateBill(string idStr, string CurrentBillQtyStr, string CurrentBillAmountStr, string externalBillNo, string InvoiceNo, DateTime? InvoiceDate, string BillNo, string CurrentDiscountStr)
        {
            try
            {
                List<BillDetail> billDetailList = new List<BillDetail>();
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] CurrentDiscountArr = CurrentDiscountStr.Split(',');
                    string[] idArray = idStr.Split(',');
                    string[] currentBillQtyStrArray = CurrentBillQtyStr.Split(',');
                    string[] currentBillAmountArray = CurrentBillAmountStr.Split(',');

                    for (int i = 0; i < currentBillQtyStrArray.Count(); i++)
                    {
                        BillDetail bd = genericMgr.FindById<BillDetail>(Convert.ToInt32(idArray[i]));
                        bd.CurrentBillQty = Decimal.Parse(currentBillQtyStrArray[i]);
                        bd.CurrentBillAmount = Decimal.Parse(currentBillAmountArray[i]);
                        billDetailList.Add(bd);
                    }
                }
                BillMaster billMaster = this.genericMgr.FindById<BillMaster>(BillNo);
                billMaster.InvoiceDate = InvoiceDate;
                billMaster.InvoiceNo = InvoiceNo;
                billMaster.ExternalBillNo = externalBillNo;
                billMaster.BillDetails = billDetailList;
                billMgr.UpdateBill(billMaster);
                object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_BillAccountUpdatedSuccessfully, BillNo), billNo = BillNo };
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

        [SconitAuthorize(Permissions = "Url_DistributionBill_Submit")]
        public ActionResult Submit(string id, string groupOrDetail)
        {
            try
            {
                billMgr.ReleaseBill(id, DateTime.Now);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_BillAccountReleasedSuccessfully, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());

            }
            return RedirectToAction("Edit", new { BillNo = id, groupOrDetail = groupOrDetail });
        }

        [SconitAuthorize(Permissions = "Url_DistributionBill_Delete")]
        public ActionResult Delete(string id, string groupOrDetail)
        {
            try
            {
                billMgr.DeleteBill(id);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_BillAccountDeletedSuccessfully, id);
                return RedirectToAction("List");
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return RedirectToAction("Edit", new { BillNo = id, groupOrDetail = groupOrDetail });
            }

        }

        [SconitAuthorize(Permissions = "Url_DistributionBill_Cancel")]
        public ActionResult Cancel(string id, string groupOrDetail)
        {
            try
            {
                billMgr.CancelBill(id, DateTime.Now);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_BillAccountCancelledSuccessfully, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { BillNo = id, groupOrDetail = groupOrDetail });
        }

        [SconitAuthorize(Permissions = "Url_DistributionBill_Close")]
        public ActionResult Close(string id, string groupOrDetail)
        {
            try
            {
                billMgr.CloseBill(id);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_BillAccountClosedSuccessfully, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { BillNo = id, groupOrDetail = groupOrDetail });
        }

        #endregion

        #region New


        [SconitAuthorize(Permissions = "Url_DistributionBill_New")]
        public ActionResult New()
        {
            TempData["ActingBillSearchModel"] = null;
            return View();
        }

        [SconitAuthorize(Permissions = "Url_DistributionBill_New")]
        public ActionResult BillMaster()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_DistributionBill_New")]
        public ActionResult BatchBillMaster()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_DistributionBill_ToCalculateMaster")]
        public ActionResult ReCalculateMaster()
        {
            return View();
        }

        #region 采购账单新建
        [GridAction]
        [SconitAuthorize(Permissions = "Url_DistributionBill_New")]
        public ActionResult _ActingBillList(GridCommand command, ActingBillSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (!string.IsNullOrEmpty(searchModel.Party))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_SupplierCanNotBeEmpty);
            }
            ViewBag.Party = searchModel.Party;
            ViewBag.ReceiptNo = searchModel.ReceiptNo;
            ViewBag.ExternalReceiptNo = searchModel.ExtReceiptNo;
            ViewBag.Flow = searchModel.Flow;
            ViewBag.Item = searchModel.Item;
            ViewBag.StartTime = searchModel.StartTime;
            ViewBag.EndTime = searchModel.EndTime;
            ViewBag.EndTime = searchModel.Currency;

            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionBill_New")]
        public ActionResult _AjaxActBillList(GridCommand command, ActingBillSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.Party))
            {
                return PartialView(new GridModel(new List<ActingBill>()));
            }
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("ReceiptNo", searchModel.ReceiptNo, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ExternalReceiptNo", searchModel.ExtReceiptNo, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Currency", searchModel.Currency, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsClose", false, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsProvisionalEstimate", false, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", (int)Sconit.CodeMaster.BillType.Distribution, "a", ref whereStatement, param);
            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartTime, searchModel.EndTime, "a", ref whereStatement, param);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartTime, "a", ref whereStatement, param);
            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndTime, "a", ref whereStatement, param);
            }

            IList<ActingBill> actingBillList = genericMgr.FindAll<ActingBill>
                (string.Format(" from ActingBill a {0}", whereStatement), param.ToArray());

            GridModel<ActingBill> gdList = new GridModel<ActingBill>();
            foreach (var actingBill in actingBillList)
            {
                actingBill.CurrentBillAmount = actingBill.UnitPrice * (actingBill.BillQty - actingBill.BillingQty);
            }
            gdList.Data = actingBillList.OrderBy(p=>p.ExternalReceiptNo);
            return PartialView(gdList);
        }

        #region 导出
        public void ExportXLS(ActingBillSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.Party))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseChooseCustomer);
                return;
            }

            IList<object> param = new List<object>();
            string hql = "from ActingBill where IsClose=? and  Type=? ";
            param.Add(false);
            param.Add((int)Sconit.CodeMaster.BillType.Distribution);

            if (!string.IsNullOrEmpty(searchModel.ReceiptNo))
            {
                hql += " and ReceiptNo=?";
                param.Add(searchModel.ReceiptNo);
            }
            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                hql += " and Item=?";
                param.Add(searchModel.Item);
            }
            if (!string.IsNullOrEmpty(searchModel.Flow))
            {
                hql += " and Flow=?";
                param.Add(searchModel.Flow);
            }
            if (!string.IsNullOrEmpty(searchModel.Party))
            {
                hql += " and Party=?";
                param.Add(searchModel.Party);
            }
            if (!string.IsNullOrEmpty(searchModel.Currency))
            {
                hql += " and Currency=?";
                param.Add(searchModel.Currency);
            }
            if (!string.IsNullOrEmpty(searchModel.ExtReceiptNo))
            {
                hql += " and ExternalReceiptNo=?";
                param.Add(searchModel.ExtReceiptNo);
            }
            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                hql += " and CreateDate Between ? and ? ";
                param.Add(searchModel.StartTime);
                param.Add(searchModel.EndTime);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                hql += " and CreateDate>=?";
                param.Add(searchModel.StartTime);
            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                hql += " and CreateDate<=?";
                param.Add(searchModel.EndTime);
            }

            IList<ActingBill> actingBillList = this.genericMgr.FindAll<ActingBill>(hql, param.ToArray());
            ExportToXLS<ActingBill>("SalesActBill.xls", actingBillList);
        }
        #endregion


        [SconitAuthorize(Permissions = "Url_DistributionBill_New")]
        public JsonResult CreateActBill(string idStr, string amountStr, string qtyStr, string discontStr)
        {
            try
            {
                string[] discontArray = discontStr.Split(',');
                string[] idStrArray = idStr.Split(',');
                string[] amountStrArray = amountStr.Split(',');
                string[] qtyStrArray = qtyStr.Split(',');
                string billNo = string.Empty;
                IList<ActingBill> actingBillList = new List<ActingBill>();

                for (int i = 0; i < idStrArray.Length; i++)
                {
                    decimal currentBillQty = Convert.ToDecimal(qtyStrArray[i]);
                    decimal CurrentBillAmount = Convert.ToDecimal(amountStrArray[i]);
                    ActingBill actingBill = queryMgr.FindById<ActingBill>(Convert.ToInt32(idStrArray[i]));
                    actingBill.Type = com.Sconit.CodeMaster.BillType.Distribution;

                    actingBill.CurrentBillQty = currentBillQty;
                    actingBill.CurrentBillAmount = CurrentBillAmount;
                    actingBill.CurrentDiscount = Convert.ToDecimal(discontArray[i]);
                    actingBillList.Add(actingBill);
                }
                IList<BillMaster> ListMaster = billMgr.CreateBill(actingBillList);
                billNo = ListMaster.First().BillNo;
                object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_SalesBillAccountCreatedSuccessfully), BillNo = billNo };
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
        #endregion

        //[SconitAuthorize(Permissions = "Url_DistributionBill_New")]
        //public ActionResult BatchNew(GridCommand command, ActingBillBatchSearchModel searchModel)
        //{
        //    SaveSuccessMessage("创建成功！");
        //    return View();
        //}

        #region 重新计价
        [GridAction]
        [SconitAuthorize(Permissions = "Url_DistributionBill_ToCalculateMaster")]
        public ActionResult _ReCalculateDetailList(GridCommand command, ActingBillSearchModel searchModel)
        {
            TempData["ActingBillSearchModel"] = searchModel;
            ViewBag.Party = searchModel.Party;
            ViewBag.ReceiptNo = searchModel.ReceiptNo;
            ViewBag.Currency = searchModel.Currency;
            ViewBag.Item = searchModel.Item;
            ViewBag.StartTime = searchModel.StartTime;
            ViewBag.EndTime = searchModel.EndTime;
            ViewBag.IncludeNoEstPrice = searchModel.IncludeNoEstPrice;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionBill_ToCalculateMaster")]
        public ActionResult _AjaxCalculateDetailList(GridCommand command, ActingBillSearchModel searchModel)
        {

            var actingBillList = this.billMgr.GetRecalculatePrice(CodeMaster.BillType.Distribution, searchModel.Party,
                searchModel.Flow, searchModel.ReceiptNo, searchModel.ExtReceiptNo, searchModel.Item, searchModel.Currency,
                searchModel.StartTime.HasValue ? searchModel.StartTime.Value : DateTime.Now,
                searchModel.EndTime.HasValue ? searchModel.EndTime.Value : DateTime.Now, searchModel.IncludeNoEstPrice);

            return PartialView(new GridModel(actingBillList));
        }

        [SconitAuthorize(Permissions = "Url_DistributionBill_ToCalculateMaster")]
        public JsonResult _CreateTocalcuLate(string idStr, string currentRecalculatePriceStr)
        {
            if (idStr == string.Empty)
            {
                object obj1 = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_PleaseChooseValuationItem) };
                return Json(obj1);
            }
            string[] checkedidActingBillArray = idStr.Split(',');
            string[] currentRecalculatePriceStrArray = currentRecalculatePriceStr.Split(',');
            List<object> ids = new List<object>();
            foreach (var idstr in checkedidActingBillArray)
            {
                ids.Add(int.Parse(idstr));
            }
            IList<ActingBill> actingBillList = genericMgr.FindAllIn<ActingBill>("from ActingBill where Id in (?", ids);
            for (int i = 0; i < actingBillList.Count; i++)
            {
                actingBillList[i].CurrentRecalculatePrice = decimal.Parse(currentRecalculatePriceStrArray[i]);
            }

            billMgr.RecalculatePrice(actingBillList);//重新计价
            object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_RevaluatedSuccessfully) };
            return Json(obj);
        }
        #endregion


        #endregion

        #region 打印导出
        public void SaveToClient(string billNo)
        {
            try
            {
                BillMaster billMaster = queryMgr.FindById<BillMaster>(billNo);
                IList<BillDetail> billDetails = queryMgr.FindAll<BillDetail>("select bd from BillDetail as bd where bd.BillNo=?", billNo);
                billMaster.BillDetails = billDetails;
                PrintBillMaster printBillMaster = Mapper.Map<BillMaster, PrintBillMaster>(billMaster);
                IList<object> data = new List<object>();
                data.Add(printBillMaster);
                data.Add(printBillMaster.BillDetails);
                reportGen.WriteToClient("BIL_SaleBill.xls", data, billMaster.BillNo + ".xls");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
        }

        public string Print(string billNo)
        {
            BillMaster billMaster = queryMgr.FindById<BillMaster>(billNo);
            IList<BillDetail> billDetails = queryMgr.FindAll<BillDetail>("select bd from BillDetail as bd where bd.BillNo=?", billNo);
            billMaster.BillDetails = billDetails;
            PrintBillMaster printBillMaster = Mapper.Map<BillMaster, PrintBillMaster>(billMaster);
            IList<object> data = new List<object>();
            data.Add(printBillMaster);
            data.Add(printBillMaster.BillDetails);
            string reportFileUrl = reportGen.WriteToFile("BIL_SaleBill.xls", data);
            return reportFileUrl;
        }

        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, BillMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            SecurityHelper.AddBillPermissionStatement(ref whereStatement, "b", "Party", com.Sconit.CodeMaster.BillType.Distribution);

            HqlStatementHelper.AddLikeStatement("ExternalBillNo", searchModel.ExternalBillNo, HqlStatementHelper.LikeMatchMode.Start, "b", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("BillNo", searchModel.BillNo, HqlStatementHelper.LikeMatchMode.Start, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", (int)Sconit.CodeMaster.BillType.Distribution, "b", ref whereStatement, param);

            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartTime, searchModel.EndTime, "b", ref whereStatement, param);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartTime, "b", ref whereStatement, param);
            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndTime, "b", ref whereStatement, param);
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by b.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        private SearchStatementModel PrepareActingBillToListSearchStatement(GridCommand command, ActingBillSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("ReceiptNo", searchModel.ReceiptNo, HqlStatementHelper.LikeMatchMode.Start, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "a", ref whereStatement, param);

            HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Currency", searchModel.Currency, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsProvisionalEstimate", false, "a", ref whereStatement, param);
            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartTime, searchModel.EndTime, "a", ref whereStatement, param);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartTime, "a", ref whereStatement, param);
            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndTime, "a", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountActingBillStatement;
            searchStatementModel.SelectStatement = selectActingBillStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion


        #region DistributionBill
        [SconitAuthorize(Permissions = "Url_DistributionBill_ActingBillGroup")]
        public ActionResult ActingBillGroup()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_DistributionBill_ActingBillGroup")]
        public ActionResult _ActingBillGroupList(string Party, string Item)
        {
            var hql = " from ActingBill where BillQty> BilledQty and Type =? ";
            var paramList = new List<object> { 1 };
            if (!string.IsNullOrWhiteSpace(Party))
            {
                paramList.Add(Party);
                hql += " and Party = ? ";
            }
            if (!string.IsNullOrWhiteSpace(Item))
            {
                paramList.Add(Item);
                hql += " and Item = ? ";
            }

            var actingBillList = genericMgr.FindAll<ActingBill>
                (hql, paramList.ToArray()).GroupBy(p => new
                {
                    p.Party,
                    p.Item,
                    p.Uom,
                    p.Currency,
                }, (k, g) => new ActingBill
                {
                    Party = k.Party,
                    Item = k.Item,
                    ItemDescription = g.First().ItemDescription,
                    Uom = k.Uom,
                    CurrentBillQty = g.Sum(q => q.BillQty) - g.Sum(q => q.BilledQty),
                    Currency = k.Currency,
                    CurrentBillAmount = (g.Sum(q => q.BillAmount) - g.Sum(q => q.BilledAmount)) 

                });
            return PartialView(actingBillList);
        }
        #endregion
        #region Export distribution that no invoice
        [SconitAuthorize(Permissions = "Url_DistributionBill_ActingBillGroup")]
        public void ExportXLSSaleNoInvoice(string Party, string Item)
        {
            var hql = " from ActingBill where BillQty> BilledQty and Type =? ";
            var paramList = new List<object> { 1 };
            if (!string.IsNullOrWhiteSpace(Party))
            {
                paramList.Add(Party);
                hql += " and Party = ? ";
            }
            if (!string.IsNullOrWhiteSpace(Item))
            {
                paramList.Add(Item);
                hql += " and Item = ? ";
            }

            var actingBillList = genericMgr.FindAll<ActingBill>
                (hql, paramList.ToArray()).GroupBy(p => new
                {
                    p.Party,
                    p.Item,
                    p.Uom,
                    p.Currency,
                }, (k, g) => new ActingBill
                {
                    Party = k.Party,
                    Item = k.Item,
                    ItemDescription = g.First().ItemDescription,
                    Uom = k.Uom,
                    CurrentBillQty = g.Sum(q => q.BillQty) - g.Sum(q => q.BilledQty),
                    Currency = k.Currency,
                    CurrentBillAmount = (g.Sum(q => q.BillAmount) - g.Sum(q => q.BilledAmount)) 
                });
            IList<ActingBill> actingBillLists = actingBillList.ToList();
            ExportToXLS<ActingBill>("SalesNoInvoiceBill.xls", actingBillLists);
        }
        #endregion
        #region 手工寄售结算
        [SconitAuthorize(Permissions = "Url_DistributionBill_SettlePlanBill")]
        public ActionResult PlanBill()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_DistributionBill_SettlePlanBill")]
        public ActionResult _PlanBillList(GridCommand command, PlanBillSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (!string.IsNullOrEmpty(searchModel.Party))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_SupplierCanNotBeEmpty);
            }
            ViewBag.Party = searchModel.Party;
            ViewBag.Flow = searchModel.Flow;
            ViewBag.Item = searchModel.Item;
            ViewBag.StartTime = searchModel.StartTime;
            ViewBag.EndTime = searchModel.EndTime;
            ViewBag.EndTime = searchModel.Currency;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionBill_SettlePlanBill")]
        public ActionResult _AjaxPlanBillList(GridCommand command, PlanBillSearchModel searchModel)
        {
            var gridModel = new GridModel<PlanBill>(new List<PlanBill>());
            if (!string.IsNullOrEmpty(searchModel.Party))
            {
                gridModel = GetPlanBillGridModel(command, searchModel);
                gridModel.Data = gridModel.Data.GroupBy(p => new
                {
                    p.Item,
                    p.Currency,
                    p.Uom,
                }, (k, g) => new PlanBill
                {
                    Item = k.Item,
                    ItemDescription = g.First().ItemDescription,
                    ReferenceItemCode = g.First().ReferenceItemCode,
                    Uom = k.Uom,
                    Currency = k.Currency,
                    PlanQty = g.Sum(q => q.PlanQty),
                    ActingQty = g.Sum(q => q.ActingQty),
                    CurrentActingQty = g.Sum(q => q.PlanQty) - g.Sum(q => q.ActingQty)
                });
            }
            return PartialView(gridModel);
        }
        #region 导出
        [SconitAuthorize(Permissions = "Url_DistributionBill_SettlePlanBill")]
        public void ExportXLPlanBill(PlanBillSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !string.IsNullOrEmpty(searchModel.Party) ? 0 : value;
            var gridModel = new GridModel<PlanBill>(new List<PlanBill>());
            if (!string.IsNullOrEmpty(searchModel.Party))
            {
                gridModel = GetPlanBillGridModel(command, searchModel);
                gridModel.Data = gridModel.Data.GroupBy(p => new
                {
                    p.Item,
                    p.Currency,
                    p.Uom,
                }, (k, g) => new PlanBill
                {
                    Item = k.Item,
                    ItemDescription = g.First().ItemDescription,
                    ReferenceItemCode = g.First().ReferenceItemCode,
                    Uom = k.Uom,
                    Currency = k.Currency,
                    PlanQty = g.Sum(q => q.PlanQty),
                    ActingQty = g.Sum(q => q.ActingQty),
                    CurrentActingQty = g.Sum(q => q.PlanQty) - g.Sum(q => q.ActingQty)
                });
            }

            IList<PlanBill> PlanBillList = gridModel.Data.ToList();
            ExportToXLS<PlanBill>("PlanBill.xls", PlanBillList);
        }
        #endregion
        [SconitAuthorize(Permissions = "Url_DistributionBill_SettlePlanBill")]
        public JsonResult SettlePlanBill(PlanBillSearchModel searchModel, string itemStr, string uomStr, string currencyStr, string qtyStr)
        {
            try
            {
                var planBillList = GetPlanBillGridModel(new GridCommand(), searchModel).Data.OrderBy(p => p.CreateDate);
                string[] itemArray = itemStr.Split(',');
                string[] uomArray = uomStr.Split(',');
                string[] currencyArray = currencyStr.Split(',');
                string[] qtyArray = qtyStr.Split(',');

                for (int i = 0; i < itemArray.Length; i++)
                {
                    decimal currentSettleQty = Convert.ToDecimal(qtyArray[i]);
                    string item = itemArray[i];
                    string uom = uomArray[i];
                    string currency = currencyArray[i];
                    var planBills = planBillList.Where(p => p.Item == itemArray[i]
                        && p.Uom == uomArray[i] && p.Currency == currencyArray[i]);
                    foreach (var planBill in planBills)
                    {
                        //负数应该会自动结算,不考虑负数 todo 测试
                        var currentQty = planBill.PlanQty - planBill.ActingQty;
                        if (currentQty >= currentSettleQty)
                        {
                            planBill.CurrentActingQty = currentSettleQty;
                            currentSettleQty = 0;
                            break;
                        }
                        else
                        {
                            planBill.CurrentActingQty = currentQty;
                            currentSettleQty -= currentQty;
                        }
                    }
                }
                billMgr.SettleBillList(planBillList.Where(p => p.CurrentActingQty != 0).ToList());
                return Json(new object());
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        private GridModel<PlanBill> GetPlanBillGridModel(GridCommand command, PlanBillSearchModel searchModel)
        {
            command.PageSize = int.MaxValue;
            command.Page = 1;
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            SecurityHelper.AddBillPermissionStatement(ref whereStatement, "b", "Party", com.Sconit.CodeMaster.BillType.Distribution);
            HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Currency", searchModel.Currency, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", com.Sconit.CodeMaster.BillType.Distribution, "b", ref whereStatement, param);
            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartTime, searchModel.EndTime, "b", ref whereStatement, param);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartTime, "b", ref whereStatement, param);
            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndTime, "b", ref whereStatement, param);
            }
            if (whereStatement == string.Empty)
            {
                whereStatement += " where b.PlanQty > b.ActingQty";
            }
            else
            {
                whereStatement += " and b.PlanQty > b.ActingQty";
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from PlanBill as b";
            searchStatementModel.SelectStatement = "from PlanBill as b";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return GetAjaxPageData<PlanBill>(searchStatementModel, command);
        }
        #endregion
    }
}
