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

namespace com.Sconit.Web.Controllers.SP
{
    public class SupplierBillController : WebAppBaseController
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
        [SconitAuthorize(Permissions = "Url_SupplierBill_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SupplierBill_View")]
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
        [SconitAuthorize(Permissions = "Url_SupplierBill_View")]
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
        #region 导出
        [SconitAuthorize(Permissions = "Url_SupplierBill_View")]
        public void ExportXLSBillNotice(BillMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<BillMaster> gridModel = GetAjaxPageData<BillMaster>(searchStatementModel, command);
            List<BillMaster> BillMasterLists = new List<BillMaster>();
            if (this.CheckSearchModelIsNull(searchModel))
            {
                foreach (BillMaster BillMasterList in gridModel.Data)
                {
                    BillMasterLists.Add(BillMasterList);
                }
            }
            ExportToXLS<BillMaster>("BillNotice.xls", BillMasterLists);
        }

        #endregion
        [GridAction]
        [SconitAuthorize(Permissions = "Url_SupplierBill_View")]
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
        [SconitAuthorize(Permissions = "Url_SupplierBill_View")]
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
        [SconitAuthorize(Permissions = "Url_SupplierBill_View")]
        public ActionResult Edit(string BillNo, string groupOrDetail)
        {
            ViewBag.groupOrDetail = groupOrDetail == string.Empty ? "0" : groupOrDetail;
            BillMaster Billmaster = genericMgr.FindById<BillMaster>(BillNo);
            return View(Billmaster);
        }

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
                reportGen.WriteToClient("BIL_PurchaseBill.xls", data, printBillMaster.BillNo+ ".xls");
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
            string reportFileUrl = reportGen.WriteToFile("BIL_PurchaseBill.xls", data);
            return reportFileUrl;
        }

        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, BillMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            SecurityHelper.AddBillPermissionStatement(ref whereStatement, "b", "Party", com.Sconit.CodeMaster.BillType.Procurement);

            HqlStatementHelper.AddLikeStatement("ExternalBillNo", searchModel.ExternalBillNo, HqlStatementHelper.LikeMatchMode.Start, "b", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("BillNo", searchModel.BillNo, HqlStatementHelper.LikeMatchMode.Start, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", (int)Sconit.CodeMaster.BillType.Procurement, "b", ref whereStatement, param);


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


        #endregion


        [SconitAuthorize(Permissions = "Url_SupplierBill_ActingBill")]
        public ActionResult ActingBillGroup()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_SupplierBill_ActingBill")]
        public ActionResult _ActingBillGroupList(string Party, string Item)
        {
            var hql = " from ActingBill p where p.BillQty> p.BilledQty and p.Type =? ";
            SecurityHelper.AddBillPermissionStatement(ref hql, "p", "Party", com.Sconit.CodeMaster.BillType.Procurement);
            var paramList = new List<object> { 0 };
            if (!string.IsNullOrWhiteSpace(Party))
            {
                paramList.Add(Party);
                hql += " and p.Party = ? ";
            }
            if (!string.IsNullOrWhiteSpace(Item))
            {
                paramList.Add(Item);
                hql += " and p.Item = ? ";
            }

            var actingBillList = genericMgr.FindAll<ActingBill>
                (hql, paramList.ToArray()).GroupBy(p => new
                {
                    p.Party,
                    p.Item,
                    p.Uom,
                }, (k, g) => new ActingBill
                {
                    Party = k.Party,
                    Item = k.Item,
                    ItemDescription = g.First().ItemDescription,
                    Uom = k.Uom,
                    CurrentBillQty = g.Sum(q => q.BillQty) - g.Sum(q => q.BilledQty),
                });
            return PartialView(actingBillList);
        }
        #region
        [SconitAuthorize(Permissions = "Url_SupplierBill_ActingBill")]
        public void ExportXLS(string Party ,string Item)
                {
 
                    var hql = " from ActingBill where BillQty> BilledQty and Type =? ";
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

                    var actingBillList = genericMgr.FindAll<ActingBill>
                        (hql, paramList.ToArray()).GroupBy(p => new
                        {
                            p.Party,
                            p.Item,
                            p.Uom,
                        }, (k, g) => new ActingBill
                        {
                            Party = k.Party,
                            Item = k.Item,
                            ItemDescription = g.First().ItemDescription,
                            Uom = k.Uom,
                            CurrentBillQty = g.Sum(q => q.BillQty) - g.Sum(q => q.BilledQty),
                        });
                    IList<ActingBill> actingBillLists =  actingBillList.ToList();
                    ExportToXLS<ActingBill>("ProcurementNoInvoice.xls", actingBillLists);
                }
                #endregion

    }
}
