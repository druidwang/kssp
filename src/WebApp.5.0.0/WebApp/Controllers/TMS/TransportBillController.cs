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
using com.Sconit.Entity.TMS;
using com.Sconit.Web.Models.SearchModels.WMS;
using com.Sconit.Web.Models.SearchModels.TMS;

namespace com.Sconit.Web.Controllers.TMS
{
    public class TransportBillController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public ITransportBillMgr billMgr { get; set; }
        //public IReportGen reportGen { get; set; }

        #region hql
        //采购账单TransportBillMaster

        private static string selectCountStatement = "select count(*) from TransportBillMaster as b";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select b from TransportBillMaster as b";

        //采购账单TransportActingBill

        private static string selectCountTransportActingBillStatement = "select count(*) from TransportActingBill as a";

        /// <summary>
        /// 
        /// </summary>
        private static string selectTransportActingBillStatement = "select a from TransportActingBill as a";

        #endregion

        #region View
        [SconitAuthorize(Permissions = "Url_TransportBill_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_TransportBill_View")]
        public ActionResult List(GridCommand command, TransportBillSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_TransportBill_View")]
        public ActionResult _AjaxList(GridCommand command, TransportBillSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<TransportBillMaster>()));
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<TransportBillMaster>(searchStatementModel, command));

        }
        #region Export 账单查询
        [SconitAuthorize(Permissions = "Url_TransportBill_View")]
        public void ExportXLSSearch(TransportBillSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            IList<TransportBillMaster> actingBillLists = GetAjaxPageData<TransportBillMaster>(searchStatementModel, command).Data.ToList();
            ExportToXLS<TransportBillMaster>("TransportBillSearch.xls", actingBillLists);
        }
        #endregion

        [GridAction]
        [SconitAuthorize(Permissions = "Url_TransportBill_View")]
        public ActionResult _TransportBillDetailList(string BillNo)
        {
            TransportBillMaster Billmaster = genericMgr.FindById<TransportBillMaster>(BillNo);
            ViewBag.BilledAmount = Billmaster.BillAmount;
            ViewBag.Status = Billmaster.Status;

            IList<object> arrayPara = new List<object>();
            arrayPara.Add(BillNo);
            IList<TransportBillDetail> billdetail = null;

            billdetail = genericMgr.FindAll<TransportBillDetail>(" from  TransportBillDetail b where  b.BillNo=? ", arrayPara.ToArray());
            foreach (TransportBillDetail billDetail in billdetail)
            {
                TransportActingBill actBill = genericMgr.FindById<TransportActingBill>(billDetail.ActBill);
                billDetail.BillQty = actBill.BillQty;

                billDetail.CurrentBillQty = billDetail.BillQty;
                billDetail.CurrentBillAmount = billDetail.BillAmount;
            }
            return PartialView(billdetail);
        }
        #endregion

        #region Edit


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_TransportBill_Edit")]
        public ActionResult Edit(string BillNo, string groupOrDetail)
        {
            ViewBag.groupOrDetail = groupOrDetail == string.Empty ? "0" : groupOrDetail;
            TransportBillMaster Billmaster = genericMgr.FindById<TransportBillMaster>(BillNo);
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
        [SconitAuthorize(Permissions = "Url_TransportBill_Edit")]
        public JsonResult UpdateTransportBillMaster(string billNo, string invoiceNo, string externalBillNo, DateTime? invoiceDate)
        {
            try
            {
                if (string.IsNullOrEmpty(billNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BillAccountCanNotBeEmpty);
                }
                else
                {
                    TransportBillMaster billmaster = genericMgr.FindById<TransportBillMaster>(billNo);
                    billmaster.InvoiceNo = invoiceNo;
                    billmaster.ExternalBillNo = externalBillNo;
                    billmaster.InvoiceDate = invoiceDate;
                    this.genericMgr.Update(billmaster);
                    // SaveSuccessMessage(Resources.MD.Address.Address_Updated);
                    object obj = new { SuccessMessage = string.Format(billNo + Resources.EXT.ControllerLan.Con_BillAccountNumberSavedSuccessfully) };
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
        [SconitAuthorize(Permissions = "Url_TransportBill_Edit")]
        public JsonResult UpdateBill(string idStr, string CurrentBillQtyStr, string CurrentBillAmountStr, string InvoiceNo, string externalBillNo, DateTime? InvoiceDate, string BillNo, string CurrentDiscountStr)
        {
            try
            {
                List<TransportBillDetail> billDetailList = new List<TransportBillDetail>();
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] CurrentDiscountArr = CurrentDiscountStr.Split(',');
                    string[] idArray = idStr.Split(',');
                    string[] currentBillQtyStrArray = CurrentBillQtyStr.Split(',');
                    string[] currentBillAmountArray = CurrentBillAmountStr.Split(',');

                    for (int i = 0; i < currentBillQtyStrArray.Count(); i++)
                    {
                        TransportBillDetail bd = genericMgr.FindById<TransportBillDetail>(Convert.ToInt32(idArray[i]));
                        bd.CurrentBillQty = Decimal.Parse(currentBillQtyStrArray[i]);
                        bd.CurrentBillAmount = Decimal.Parse(currentBillAmountArray[i]);
                        bd.CurrentDiscount = Decimal.Parse(CurrentDiscountArr[i]);
                        billDetailList.Add(bd);
                    }
                }
                TransportBillMaster billMaster = this.genericMgr.FindById<TransportBillMaster>(BillNo);
                billMaster.InvoiceDate = InvoiceDate;
                billMaster.InvoiceNo = InvoiceNo;
                billMaster.ExternalBillNo = externalBillNo;
                billMaster.BillDetails = billDetailList;
                billMgr.UpdateBill(billMaster);
                object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_PurchaseBillAccountUpdatedSuccessfully, BillNo), billNo = BillNo };
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

        [SconitAuthorize(Permissions = "Url_TransportBill_Submit")]
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

        [SconitAuthorize(Permissions = "Url_TransportBill_Delete")]
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

        [SconitAuthorize(Permissions = "Url_TransportBill_Cancel")]
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

        [SconitAuthorize(Permissions = "Url_TransportBill_Close")]
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
        [SconitAuthorize(Permissions = "Url_TransportBill_New")]
        public ActionResult New()
        {
            TempData["TransportActingBillSearchModel"] = null;
            return View();
        }

        [SconitAuthorize(Permissions = "Url_TransportBill_New")]
        public ActionResult TransportBillMaster()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_TransportBill_New")]
        public ActionResult _TransportActingBillList(GridCommand command, TransportActingBillSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (!string.IsNullOrEmpty(searchModel.Carrier))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_SupplierCanNotBeEmpty);
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            ViewBag.Carrier = searchModel.Carrier;
            ViewBag.Flow = searchModel.Flow;
            ViewBag.StartTime = searchModel.StartTime;
            ViewBag.EndTime = searchModel.EndTime;
          

            return PartialView();
        }
      
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_TransportBill_New")]
        public ActionResult _AjaxTransportActingBillList(GridCommand command, TransportActingBillSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.Carrier))
            {
                return PartialView(new GridModel(new List<TransportActingBill>()));
            }
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Carrier", searchModel.Carrier, "a", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", (int)Sconit.CodeMaster.BillType.Transport, "a", ref whereStatement, param);
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

            IList<TransportActingBill> actingBillList = genericMgr.FindAll<TransportActingBill>(string.Format(" from TransportActingBill a {0}", whereStatement), param.ToArray());

            GridModel<TransportActingBill> gdList = new GridModel<TransportActingBill>();

            foreach (var actingBill in actingBillList)
            {
                actingBill.CurrentBillAmount = actingBill.UnitPrice * (actingBill.BillQty - actingBill.BillingQty);
            }
            gdList.Data = actingBillList;
            return PartialView(gdList);

        }

        [SconitAuthorize(Permissions = "Url_TransportBill_New")]
        public JsonResult CreateActBill(string idStr, string amountStr, string qtyStr, string discontStr)
        {
            try
            {
                string[] discontArray = discontStr.Split(',');
                string[] idStrArray = idStr.Split(',');
                string[] amountStrArray = amountStr.Split(',');
                string[] qtyStrArray = qtyStr.Split(',');
                string billNo = string.Empty;
                IList<TransportActingBill> actingBillList = new List<TransportActingBill>();

                for (int i = 0; i < idStrArray.Length; i++)
                {
                    decimal currentBillQty = Convert.ToDecimal(qtyStrArray[i]);
                    decimal CurrentBillAmount = Convert.ToDecimal(amountStrArray[i]);
                    TransportActingBill actingBill = queryMgr.FindById<TransportActingBill>(Convert.ToInt32(idStrArray[i]));
                    actingBill.Type = com.Sconit.CodeMaster.BillType.Procurement;

                    actingBill.CurrentBillQty = currentBillQty;
                    actingBill.CurrentBillAmount = CurrentBillAmount;
                    actingBill.CurrentDiscount = Convert.ToDecimal(discontArray[i]);
                    actingBillList.Add(actingBill);
                }
                IList<TransportBillMaster> ListMaster = billMgr.CreateBill(actingBillList);
                billNo = ListMaster.First().BillNo;
                object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_PurchaseBillAccountCreatedSuccessfully, billNo), BillNo = billNo };
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
    

        #region 重新计价
        [GridAction]
        [SconitAuthorize(Permissions = "Url_TransportBill_ToCalculateMaster")]
        public ActionResult _ReCalculateDetailList(GridCommand command, TransportActingBillSearchModel searchModel)
        {
            TempData["TransportActingBillSearchModel"] = searchModel;
            ViewBag.Carrier = searchModel.Carrier;
            ViewBag.Flow = searchModel.Flow;
       
            ViewBag.StartTime = searchModel.StartTime;
            ViewBag.EndTime = searchModel.EndTime;
           // ViewBag.IncludeNoEstPrice = searchModel.IncludeNoEstPrice;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_TransportBill_ToCalculateMaster")]
        public ActionResult _AjaxCalculateDetailList(GridCommand command, TransportActingBillSearchModel searchModel)
        {
            var actingBillList = new List<ActingBill>();

            return PartialView(new GridModel(actingBillList));
        }

        [SconitAuthorize(Permissions = "Url_TransportBill_ToCalculateMaster")]
        public JsonResult _CreateReCalcuLate(string idStr, string currentRecalculatePriceStr)
        {
            if (idStr == string.Empty)
            {
                object obj1 = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_PleaseChooseValuationItem) };
                return Json(obj1);
            }
            string[] checkedidTransportActingBillArray = idStr.Split(',');
            string[] currentRecalculatePriceStrArray = currentRecalculatePriceStr.Split(',');
            List<object> ids = new List<object>();
            foreach (var idstr in checkedidTransportActingBillArray)
            {
                ids.Add(int.Parse(idstr));
            }
            IList<TransportActingBill> actingBillList = genericMgr.FindAllIn<TransportActingBill>("from TransportActingBill where Id in (?", ids);
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
                TransportBillMaster billMaster = queryMgr.FindById<TransportBillMaster>(billNo);
                IList<TransportBillDetail> billDetails = queryMgr.FindAll<TransportBillDetail>("select bd from TransportBillDetail as bd where bd.BillNo=?", billNo);
                billMaster.BillDetails = billDetails;
             //   PrintTransportBillMaster printTransportBillMaster = Mapper.Map<TransportBillMaster, PrintTransportBillMaster>(billMaster);
                IList<object> data = new List<object>();
               // data.Add(printTransportBillMaster);
                //data.Add(printTransportBillMaster.TransportBillDetails);
                reportGen.WriteToClient("BIL_PurchaseBill.xls", data, billMaster.BillNo + ".xls");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
        }

        public string Print(string billNo)
        {
            TransportBillMaster billMaster = queryMgr.FindById<TransportBillMaster>(billNo);
            IList<TransportBillDetail> billDetails = queryMgr.FindAll<TransportBillDetail>("select bd from TransportBillDetail as bd where bd.BillNo=?", billNo);
            billMaster.BillDetails = billDetails;
           // PrintTransportBillMaster printTransportBillMaster = Mapper.Map<TransportBillMaster, PrintTransportBillMaster>(billMaster);
            IList<object> data = new List<object>();
           // data.Add(printTransportBillMaster);
           // data.Add(printTransportBillMaster.TransportBillDetails);
            string reportFileUrl = reportGen.WriteToFile("BIL_PurchaseBill.xls", data);
            return reportFileUrl;
        }

        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, TransportBillSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("ExternalBillNo", searchModel.ExternalBillNo, HqlStatementHelper.LikeMatchMode.Start, "b", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("BillNo", searchModel.BillNo, HqlStatementHelper.LikeMatchMode.Start, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Carrier", searchModel.Carrier, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", (int)Sconit.CodeMaster.BillType.Transport, "b", ref whereStatement, param);


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
        private SearchStatementModel PrepareTransportActingBillToListSearchStatement(GridCommand command, TransportActingBillSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "a", ref whereStatement, param);

            HqlStatementHelper.AddEqStatement("Carrier", searchModel.Carrier, "a", ref whereStatement, param);

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
            searchStatementModel.SelectCountStatement = selectCountTransportActingBillStatement;
            searchStatementModel.SelectStatement = selectTransportActingBillStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        #endregion

        #region TransportBill
        [SconitAuthorize(Permissions = "Url_TransportBill_TransportActingBillGroup")]
        public ActionResult TransportActingBillGroup()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_TransportBill_TransportActingBillGroup")]
        public ActionResult _TransportActingBillGroupList(string Party, string Item)
        {
            var hql = " from TransportActingBill where BillQty> BilledQty and Type =? ";
            var paramList = new List<object> { 0 };
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

            var actingBillList = genericMgr.FindAll<TransportActingBill>
                (hql, paramList.ToArray()).GroupBy(p => new
                {
                    p.Carrier,
                
                    p.Currency,
 
                }, (k, g) => new TransportActingBill
                {
                    Carrier = k.Carrier,
                    CurrentBillQty = g.Sum(q => q.BillQty) - g.Sum(q => q.BilledQty),
                    Currency = k.Currency,
                    CurrentBillAmount = (g.Sum(q => q.BillAmount) - g.Sum(q => q.BilledAmount)) 
                });
            return PartialView(actingBillList);
        }
        #endregion

        #region
        [SconitAuthorize(Permissions = "Url_TransportBill_TransportActingBillGroup")]
        public void ExportXLS(string Party, string Item)
        {

            var hql = " from TransportActingBill where BillQty> BilledQty and Type =? ";
            var paramList = new List<object> { 0 };
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

            var actingBillList = genericMgr.FindAll<TransportActingBill>
                (hql, paramList.ToArray()).GroupBy(p => new
                {
                    p.Carrier,
                    p.Currency
                }, (k, g) => new TransportActingBill
                {
                    Carrier = k.Carrier,
               
                    CurrentBillQty = g.Sum(q => q.BillQty) - g.Sum(q => q.BilledQty),
                    Currency = k.Currency,
                    CurrentBillAmount = (g.Sum(q => q.BillAmount) - g.Sum(q => q.BilledAmount)) 
                });
            IList<TransportActingBill> actingBillLists = actingBillList.ToList();
            ExportToXLS<TransportActingBill>("ProcurementNoInvoice.xls", actingBillLists);
        }
        #endregion
    }
}
