namespace com.Sconit.Web.Controllers.INP
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.INP;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.INP;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.SYS;
    using System;
    using AutoMapper;
    using com.Sconit.Service.Impl;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.CUST;
    using System.Text;
    using com.Sconit.Entity;
    using com.Sconit.PrintModel.INP;
    using com.Sconit.Utility.Report;
    using com.Sconit.Entity.ORD;

    public class InspectionOrderController : WebAppBaseController
    {
        //
        // GET: /InspectionOrder/
        //public IGenericMgr genericMgr { get; set; }
        public IInspectMgr inspectMgr { get; set; }
        //public ISystemMgr systemMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        //public IReportGen reportGen { get; set; }


        private static string selectStatement = "select i from InspectMaster as i";

        private static string selectCountStatement = "select count(*) from InspectMaster as i";

        private static string selectInspectDetailCountStatement = "select count(*) from InspectDetail as id";

        private static string selectInspectDetailSearchModelStatement = "select id from InspectDetail as id ";

        private static string selectInspectDetailStatement = "select i from InspectDetail as i where i.InspectNo=?";

        private static string selectJudgeInspectDetailStatement = "select i from InspectDetail as i where i.InspectNo=? and i.IsJudge=False";

        private static string InspectResultSelectStatement = "select i from InspectResult as i";

        private static string InspectResultSelectCountStatement = "select count(*) from InspectResult as i";

        #region view
        [SconitAuthorize(Permissions = "Url_InspectionOrder_View")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_View")]
        [GridAction]
        public ActionResult List(GridCommand command, InspectMasterSearchModel searchModel)
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

        [SconitAuthorize(Permissions = "Url_InspectionOrder_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, InspectMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<InspectMaster>()));
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel, string.Empty);
            return PartialView(GetAjaxPageData<InspectMaster>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {

                return View("Edit", string.Empty, id);
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_View")]
        public ActionResult _Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                InspectMaster inspectMaster = this.genericMgr.FindById<InspectMaster>(id);

                return PartialView(inspectMaster);
            }
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_View")]
        public ActionResult InspectionOrderDetailEdit(string inspectNo)
        {
            InspectMaster inspectMaster = this.genericMgr.FindById<InspectMaster>(inspectNo);
            if (inspectMaster.Type == com.Sconit.CodeMaster.InspectType.Barcode)
            {
                ViewBag.HideHuId = false;
            }
            else
            {
                ViewBag.HideHuId = true;
            }
            ViewBag.inspectNo = inspectNo;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_View")]
        public ActionResult _InspectionOrderDetailList(string inspectNo)
        {
            IList<InspectDetail> inspectDetailList = genericMgr.FindAll<InspectDetail>(selectInspectDetailStatement, inspectNo);

            IList<FailCode> failCodeList = genericMgr.FindAll<FailCode>();

            foreach (InspectDetail inspectDetail in inspectDetailList)
            {
                foreach (FailCode failCode in failCodeList)
                {
                    if (inspectDetail.FailCode == failCode.Code)
                    {
                        inspectDetail.FailCode = failCode.CodeDescription;
                    }
                }
                if (!string.IsNullOrWhiteSpace(inspectDetail.HuId))
                {
                    Hu huData = genericMgr.FindById<Hu>(inspectDetail.HuId);
                    inspectDetail.ExpireDate = huData.ExpireDate;
                    inspectDetail.SupplierLotNo = huData.SupplierLotNo;
                }
            }

            return View(new GridModel(inspectDetailList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_View,Url_InspectionOrder_Judge")]
        public ActionResult InspectionResult(GridCommand command, InspectResultSearchModel searchModel)
        {
            ViewBag.Item = searchModel.Item;
            TempData["InspectResultSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_View,Url_InspectionOrder_Judge")]
        public ActionResult _AjaxInspectionResultList(GridCommand command, InspectResultSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.InspectResultPrepareSearchStatement(command, searchModel);
            GridModel<InspectResult> list = GetAjaxPageData<InspectResult>(searchStatementModel, command);
            IList<FailCode> failCodeList = genericMgr.FindAll<FailCode>();

            foreach (InspectResult inspectResult in list.Data)
            {
                foreach (FailCode failCode in failCodeList)
                {
                    if (inspectResult.FailCode == failCode.Code)
                    {
                        inspectResult.FailCodeDescription = failCode.CodeDescription;
                        inspectResult.CurrentFailCode = inspectResult.FailCode;
                        inspectResult.CurrentIsHandle = inspectResult.IsHandle;
                    }
                }
                if (!string.IsNullOrWhiteSpace(inspectResult.HuId))
                {
                    Hu huData = genericMgr.FindById<Hu>(inspectResult.HuId);
                    inspectResult.ExpireDate = huData.ExpireDate;
                    inspectResult.SupplierLotNo = huData.SupplierLotNo;
                }
            }
            return PartialView(list);
        }
        #endregion

        #region new
        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public ActionResult New()
        {
            if (TempData["InspectDetailList"] != null)
            {
                TempData["InspectDetailList"] = null;
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public JsonResult New(string region, string locationFrom, [Bind(Prefix = "inserted")]IEnumerable<InspectDetail> insertedInspectDetails)
        {
            BusinessException businessException = new BusinessException();
            try
            {
                ViewBag.Region = region;
                ViewBag.LocationFrom = locationFrom;

                #region orderDetailList
                if (string.IsNullOrEmpty(locationFrom))
                {
                    throw new BusinessException(Resources.INP.InspectDetail.Errors_InspectDetail_LocationRequired);
                }

                IList<InspectDetail> inspectDetailList = new List<InspectDetail>();
                if (insertedInspectDetails != null && insertedInspectDetails.Count() > 0)
                {
                    int i = 1;
                    foreach (InspectDetail inspectDetail in insertedInspectDetails)
                    {
                        if (string.IsNullOrEmpty(inspectDetail.Item))
                        {
                            businessException.AddMessage(Resources.EXT.ControllerLan.Con_ItemCanNotBeEmpty + (i++) + Resources.EXT.ControllerLan.Con_RowItemCanNotBeEmpty);
                            continue;
                        }
                        if (inspectDetail.InspectQty <= 0)
                        {
                            businessException.AddMessage(Resources.EXT.ControllerLan.Con_ItemCanNotBeEmpty + (i++) + Resources.EXT.ControllerLan.Con_InspectionQuantityMustGreaterThenZero);
                            continue;
                        }
                        //if (string.IsNullOrEmpty(inspectDetail.FailCode))
                        //{
                        //    businessException.AddMessage("第" + (i++) + "失效代码为必填。");
                        //    continue;
                        //}

                        Item item = this.genericMgr.FindById<Item>(inspectDetail.Item);
                        inspectDetail.ItemDescription = item.Description;
                        inspectDetail.UnitCount = item.UnitCount;
                        inspectDetail.ReferenceItemCode = item.ReferenceCode;
                        inspectDetail.Uom = item.Uom;
                        inspectDetail.LocationFrom = locationFrom;
                        inspectDetail.CurrentLocation = locationFrom;
                        inspectDetail.BaseUom = item.Uom;
                        inspectDetail.UnitQty = 1;

                        inspectDetailList.Add(inspectDetail);

                    }
                }
                #endregion
                if (businessException.HasMessage)
                {
                    throw businessException;
                }
                if (inspectDetailList != null && inspectDetailList.Count == 0)
                {
                    throw new BusinessException(Resources.INP.InspectDetail.Errors_InspectDetail_Required);
                }

                InspectMaster inspectMaster = new InspectMaster();
                inspectMaster.Region = region;
                inspectMaster.InspectDetails = inspectDetailList;

                inspectMaster.Type = com.Sconit.CodeMaster.InspectType.Quantity;
                inspectMaster.IsATP = false;

                inspectMgr.CreateAndReject(inspectMaster);
                SaveSuccessMessage(Resources.INP.InspectMaster.InspectMaster_Added);

                return Json(new object[] { inspectMaster.InspectNo });
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public ActionResult _InspectionOrderDetail()
        {
            //IList<Item> items = genericMgr.FindAll<Item>(selecItemStatement, true);
            //ViewData.Add("items", items);

            IList<Uom> uoms = genericMgr.FindAll<Uom>();
            ViewData.Add("uoms", uoms);
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public ActionResult _SelectBatchEditing()
        {
            IList<InspectDetail> inspectDetailList = new List<InspectDetail>();
            return View(new GridModel(inspectDetailList));
        }


        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public ActionResult _WebInspectDetail(string itemCode)
        {
            if (!string.IsNullOrEmpty(itemCode))
            {
                IList<WebOrderDetail> webOrderDetailList = new List<WebOrderDetail>();
                WebOrderDetail webOrderDetail = new WebOrderDetail();

                Item item = genericMgr.FindById<Item>(itemCode);
                if (item != null)
                {
                    webOrderDetail.Item = item.Code;
                    webOrderDetail.ItemDescription = item.Description;
                    webOrderDetail.UnitCount = item.UnitCount;
                    webOrderDetail.Uom = item.Uom;
                    webOrderDetail.ReferenceItemCode = item.ReferenceCode;
                }
                return this.Json(webOrderDetail);
            }
            return null;
        }

        #region New ByScanHu
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public void ItemHuIdScan(string HuId)
        {
            IList<InspectDetail> InspectDetailList = (IList<InspectDetail>)TempData["InspectDetailList"];
            try
            {
                InspectDetailList = inspectMgr.AddInspectDetail(HuId, InspectDetailList);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
            }
            TempData["InspectDetailList"] = InspectDetailList;
        }

        public ActionResult _InspectionOrderDetailScanHu()
        {
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public ActionResult _SelectInspectionOrderDetail()
        {
            IList<InspectDetail> InspectionDetailList = new List<InspectDetail>();
            if (TempData["InspectDetailList"] != null)
            {
                InspectionDetailList = (IList<InspectDetail>)TempData["InspectDetailList"];
            }
            TempData["InspectDetailList"] = InspectionDetailList;
            return PartialView(new GridModel(InspectionDetailList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public ActionResult _DeleteInspectionDetail(string HuId)
        {
            IList<InspectDetail> InspectDetailList = (IList<InspectDetail>)TempData["InspectDetailList"];
            IList<InspectDetail> q = InspectDetailList.Where(v => v.HuId != HuId).ToList();
            TempData["InspectDetailList"] = q;
            return PartialView(new GridModel(q));
        }

        public void _CleanInspectionDetail()
        {
            if (TempData["InspectDetailList"] != null)
            {
                TempData["InspectDetailList"] = null;
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public string CreateInspectionDetail(string ItemStr, string HuIdStr, string LocationStr, string InspectQtyStr, string FailCodeStr, string NoteStr)
        {
            try
            {
                if (!string.IsNullOrEmpty(ItemStr))
                {
                    string[] itemArray = ItemStr.Split(',');
                    string[] huidArray = HuIdStr.Split(',');
                    string[] locationArray = LocationStr.Split(',');
                    string[] inspectqtyArray = InspectQtyStr.Split(',');
                    string[] falicodeArray = FailCodeStr.Split(',');
                    string[] noteArray = NoteStr.Split(',');
                    IList<InspectDetail> inspectDetailList = new List<InspectDetail>();
                    int i = 0;
                    foreach (string itemcode in itemArray)
                    {
                        InspectDetail inspectDetail = new InspectDetail();
                        Item item = this.genericMgr.FindById<Item>(itemcode);
                        inspectDetail.ItemDescription = item.Description;
                        inspectDetail.UnitCount = item.UnitCount;
                        inspectDetail.ReferenceItemCode = item.ReferenceCode;
                        inspectDetail.Uom = item.Uom;

                        inspectDetail.BaseUom = item.Uom;
                        inspectDetail.UnitQty = 1;
                        inspectDetail.LocationFrom = locationArray[i];
                        inspectDetail.CurrentLocation = locationArray[i];
                        inspectDetail.FailCode = falicodeArray[i];
                        inspectDetail.Note = noteArray[i];
                        inspectDetail.HuId = huidArray[i];
                        inspectDetail.InspectQty = Convert.ToDecimal(inspectqtyArray[i]);
                        i++;
                        inspectDetailList.Add(inspectDetail);
                    }

                    if (inspectDetailList != null && inspectDetailList.Count == 0)
                    {
                        throw new BusinessException(Resources.INP.InspectDetail.Errors_InspectDetail_Required);
                    }

                    InspectMaster inspectMaster = new InspectMaster();

                    inspectMaster.InspectDetails = inspectDetailList;

                    inspectMaster.Type = com.Sconit.CodeMaster.InspectType.Barcode;
                    inspectMaster.IsATP = false;

                    inspectMgr.CreateInspectMaster(inspectMaster);
                    SaveSuccessMessage(Resources.INP.InspectMaster.InspectMaster_Added);
                    this._CleanInspectionDetail();
                    return inspectMaster.InspectNo;
                }
                else
                {
                    throw new BusinessException(Resources.INP.InspectDetail.Errors_InspectDetail_Required);
                }

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
        #endregion

        #region Judge

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public ActionResult JudgeIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        [GridAction]
        public ActionResult JudgeList(GridCommand command, InspectMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxJudgeList(GridCommand command, InspectMasterSearchModel searchModel)
        {
            string replaceFrom = "_AjaxJudgeList";
            string replaceTo = "JudgeList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            string whereStatement = " where i.Status in ( " + (int)com.Sconit.CodeMaster.InspectStatus.Submit + "," + (int)com.Sconit.CodeMaster.InspectStatus.InProcess + ")";
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageData<InspectMaster>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public ActionResult JudgeEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                return View("JudgeEdit", string.Empty, id);
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public ActionResult _JudgeEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                InspectMaster inspectMaster = this.genericMgr.FindById<InspectMaster>(id);
                inspectMaster.InspectStatusDescription = systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.InspectStatus, ((int)inspectMaster.Status).ToString());
                return PartialView(inspectMaster);
            }
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public ActionResult InspectionOrderDetailJudge(string inspectNo, com.Sconit.CodeMaster.InspectType inspectType)
        {
            ViewBag.inspectNo = inspectNo;
            ViewBag.inspectType = inspectType;
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public ActionResult InspectionOrderDetailJudgeWithHu(string inspectNo)
        {
            ViewBag.inspectNo = inspectNo;
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public ActionResult _SelectJudgeBatchEditing(string inspectNo, com.Sconit.CodeMaster.InspectType inspectType)
        {
            IList<InspectDetail> inspectDetailList = genericMgr.FindAll<InspectDetail>(selectJudgeInspectDetailStatement, inspectNo);
            if (inspectType == Sconit.CodeMaster.InspectType.Barcode)
            {
                inspectDetailList = inspectDetailList.GroupBy(p => p.Item, (k, g) => new { k, g }).Select(p =>
                    {
                        var inspectDetail = p.g.First();
                        inspectDetail.InspectQty = p.g.Sum(q => q.InspectQty);
                        inspectDetail.QualifyQty = p.g.Sum(q => q.QualifyQty);
                        inspectDetail.RejectQty = p.g.Sum(q => q.RejectQty);
                        return inspectDetail;
                    }).ToList();
            }

            foreach (var inspectDetail in inspectDetailList)
            {
                inspectDetail.CurrentQty = inspectDetail.InspectQty - inspectDetail.QualifyQty - inspectDetail.RejectQty;
                if (!string.IsNullOrWhiteSpace(inspectDetail.HuId))
                {
                    Hu huData = genericMgr.FindById<Hu>(inspectDetail.HuId);
                    inspectDetail.ExpireDate = huData.ExpireDate;
                    inspectDetail.SupplierLotNo = huData.SupplierLotNo;
                }
            }
            return View(new GridModel(inspectDetailList));
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_SaveInspectResult")]
        public string SaveInspectResult([Bind(Prefix = "updated")]IEnumerable<InspectResult> updatedInspectResults)
        {
            try
            {
                inspectMgr.SaveInspectResult(updatedInspectResults.ToList());
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_InspectionResultSavedSuccessfully);
                return string.Empty;
            }
            catch (Exception ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.Message);
                return string.Empty;
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public JsonResult Judge(string inspectNo, com.Sconit.CodeMaster.InspectType inspectType,
            [Bind(Prefix = "updated")]IEnumerable<InspectDetail> updatedInspectDetails)
        {
            try
            {
                updatedInspectDetails = updatedInspectDetails.Where(d => d.CurrentQty > 0 && !string.IsNullOrWhiteSpace(d.JudgeFailCode));
                if (updatedInspectDetails == null || updatedInspectDetails.Count() == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LackJudgeDetail);
                }
                List<InspectDetail> inspectDetailList = new List<InspectDetail>();
                if (inspectType == Sconit.CodeMaster.InspectType.Barcode)
                {
                    var huInspectDetailList = genericMgr.FindAll<InspectDetail>(selectJudgeInspectDetailStatement, inspectNo)
                        .GroupBy(p => p.Item, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);
                    foreach (var updatedInspectDetail in updatedInspectDetails)
                    {
                        inspectDetailList.AddRange(huInspectDetailList.ValueOrDefault(updatedInspectDetail.Item)
                            .Select(p =>
                            {
                                p.CurrentQty = p.CurrentInspectQty;
                                p.JudgeFailCode = updatedInspectDetail.JudgeFailCode;
                                p.CurrentInspectResultNote = updatedInspectDetail.CurrentInspectResultNote;
                                return p;
                            }));
                    }
                }
                else
                {
                    inspectDetailList = updatedInspectDetails.ToList();
                }
                inspectMgr.JudgeInspectDetail(inspectDetailList);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_InspectionOrderJudgeSuccessfully, inspectNo);
                return Json(inspectNo);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        //[HttpPost]
        //[SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        //public string JudgeQualify(string inspectNo, string idStr)
        //{
        //    string failCodeStr = string.Empty;
        //    return BatchJudge(inspectNo, idStr, true, failCodeStr, string.Empty);
        //}

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public string JudgeReject(string inspectNo, string idStr, string failCodeStr, string notes)
        {
            return BatchJudge(inspectNo, idStr, failCodeStr, notes);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_Judge")]
        public JsonResult JudgeHu(string inspectNo, string idStr, string failCodeStr, string notes)
        {
            try
            {
                string[] idArr = idStr.Split(',');
                string[] noteArr = null;
                if (!string.IsNullOrEmpty(notes))
                {
                    noteArr = notes.Split(',');
                }
                string[] failCodeArr = null;
                if (!string.IsNullOrEmpty(failCodeStr))
                {
                    failCodeArr = failCodeStr.Split(',');
                }
                IList<InspectDetail> inspectDetailList = new List<InspectDetail>();
                for (int i = 0; i < idArr.Length; i++)
                {
                    InspectDetail inspectDetail = genericMgr.FindById<InspectDetail>(Convert.ToInt32(idArr[i]));
                    inspectDetail.CurrentQty = inspectDetail.InspectQty;
                    if (failCodeArr != null)
                    {
                        inspectDetail.JudgeFailCode = failCodeArr[i];
                    }
                    if (noteArr != null)
                    {
                        inspectDetail.CurrentInspectResultNote = noteArr[i];
                    }
                    inspectDetailList.Add(inspectDetail);
                }
                inspectMgr.JudgeInspectDetail(inspectDetailList);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_InspectionOrderJudgeSuccessfully, inspectNo);
                return Json(inspectNo);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }
        #endregion

        #region transfer
        [GridAction]
        [SconitAuthorize(Permissions = "Url_InspectionOrder_Transfer")]
        public ActionResult Transfer()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Transfer")]
        public ActionResult _TransferDetailList(string inspectNo)
        {
            IList<InspectDetail> inspectDetailList = genericMgr.FindAll<InspectDetail>(selectJudgeInspectDetailStatement, inspectNo);
            return PartialView(inspectDetailList);
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Transfer")]
        public JsonResult CreateInspectTransfer(string location, string idStr, string qtyStr)
        {
            try
            {
                #region 检查库位
                if (string.IsNullOrEmpty(location))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LocationCanNotBeEmpty);
                }
                Location locTo = genericMgr.FindById<Location>(location);
                #endregion

                if (string.IsNullOrEmpty(idStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_InspectionDetailCanNotBeEmpty);
                }
                string[] idArr = idStr.Split(',');

                IList<InspectDetail> inspectDetailList = new List<InspectDetail>();
                foreach (string id in idArr)
                {
                    InspectDetail inspetDetail = genericMgr.FindById<InspectDetail>(Convert.ToInt32(id));
                    inspectDetailList.Add(inspetDetail);
                }

                orderMgr.CreateInspectTransfer(locTo, inspectDetailList);
                object obj = new { SuccessMessage = string.Format(Resources.INP.InspectMaster.InspectMaster_Transfered) };
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

        #region detail
        [SconitAuthorize(Permissions = "Url_InspectionOrder_Detail")]
        public ActionResult DetailIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_InspectionOrder_Detail")]
        public ActionResult DetailList(GridCommand command, InspectMasterSearchModel searchModel)
        {
            if (this.CheckSearchModelIsNull(searchModel))
            {
                SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
                IList<InspectDetail> InspectDetailList = new List<InspectDetail>();
                InspectDetailList = GetDetailList(command, searchModel);
                int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
                if (InspectDetailList.Count > value)
                {
                    SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_DataExceedRow, value));
                }
                return View(InspectDetailList.Take(value));
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
                return View(new List<InspectDetail>());
            }

            //return View(list.Take(value));
        }
        #region 明细导出
        private List<InspectDetail> GetDetailList(GridCommand command, InspectMasterSearchModel searchModel)
        {
            TempData["_AjaxMessage"] = "";

            var failCodeDic = this.genericMgr.FindAll<FailCode>().ToDictionary(d => d.Code, d => d);
            string hql = PrepareDetailSearchStatement(command, searchModel);
            IList<InspectDetail> InspectDetailList = new List<InspectDetail>();
            IList<object[]> objectList = this.queryMgr.FindAllWithNativeSql<object[]>(hql);
            InspectDetailList = (from tak in objectList
                                 select new InspectDetail
                                 {
                                     Id = (int)tak[0],
                                     InspectNo = (string)tak[1],
                                     Sequence = (int)tak[2],
                                     Item = (string)tak[3],
                                     ItemDescription = (string)tak[4],
                                     ReferenceItemCode = (string)tak[5],
                                     UnitCount = (decimal)tak[6],
                                     Uom = (string)tak[7],
                                     BaseUom = (string)tak[8],
                                     UnitQty = (decimal)tak[9],
                                     HuId = (string)tak[10],
                                     LotNo = (string)tak[11],
                                     LocationFrom = (string)tak[12],
                                     CurrentLocation = (string)tak[13],
                                     InspectQty = (decimal)tak[14],
                                     QualifyQty = (decimal)tak[15],
                                     RejectQty = (decimal)tak[16],
                                     IsJudge = (bool)tak[17],
                                     CreateUserId = (int)tak[18],
                                     CreateUserName = (string)tak[19],
                                     CreateDate = (DateTime)tak[20],
                                     LastModifyUserId = (int)tak[21],
                                     LastModifyUserName = (string)tak[22],
                                     LastModifyDate = (DateTime)tak[23],
                                     Version = (int)tak[24],
                                     ManufactureParty = (string)tak[25],
                                     WMSSeq = (string)tak[26],
                                     IpDetailSequence = (int)tak[27],
                                     ReceiptDetailSequence = (int)tak[28],
                                     Note = (string)tak[29],
                                     FailCode = (string)tak[30],
                                     FailCodeDescription = string.IsNullOrWhiteSpace((string)tak[30]) ? null : failCodeDic.ValueOrDefault((string)tak[30]).CodeDescription,
                                     HandledQty = (decimal)tak[31],
                                 }).ToList();
            return (List<InspectDetail>)InspectDetailList;
        }
        [SconitAuthorize(Permissions = "Url_InspectionOrder_Detail")]
        public void ExportXLS(InspectMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            IList<InspectDetail> InspectDetailList = new List<InspectDetail>();
            InspectDetailList = GetDetailList(command, searchModel);
            if (InspectDetailList.Count > value)
            {
                SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_DataExceedRow, value));
            }
            ExportToXLS<InspectDetail>("InspectOrderDetail.xls", InspectDetailList);
        }
        #endregion
        #region 打印导出
        public void SaveToClient(string inspectNo)
        {
            InspectMaster inspectMaster = queryMgr.FindById<InspectMaster>(inspectNo);
            IList<InspectDetail> inspectDetails = queryMgr.FindAll<InspectDetail>("select id from InspectDetail as id where id.InspectNo=?", inspectNo);
            inspectMaster.InspectDetails = inspectDetails;
            PrintInspectMaster printInspectMaster = Mapper.Map<InspectMaster, PrintInspectMaster>(inspectMaster);
            var failCodeDic = this.genericMgr.FindAll<FailCode>().ToDictionary(d => d.Code, d => d);
            var inspectResults = this.genericMgr.FindAll<InspectResult>(" from InspectResult as i where i.InspectNo=? ", inspectNo).ToDictionary(d=>d.InspectDetailId,d=>d);
            foreach (var inspectDetail in printInspectMaster.InspectDetails)
            {
                inspectDetail.FailCode = inspectResults.ValueOrDefault(inspectDetail.Id) == null ? inspectDetail.FailCode : inspectResults.ValueOrDefault(inspectDetail.Id).FailCode;
                if (!string.IsNullOrWhiteSpace(inspectDetail.FailCode))
                {
                    inspectDetail.FailCode = failCodeDic[inspectDetail.FailCode].CodeDescription;
                }
            }
            IList<object> data = new List<object>();
            data.Add(printInspectMaster);
            data.Add(printInspectMaster.InspectDetails);
            //string reportFileUrl = reportGen.WriteToFile(orderMaster.OrderTemplate, data);
            reportGen.WriteToClient("InspectOrder.xls", data, inspectMaster.InspectNo + ".xls");

            //return reportFileUrl;
            //reportGen.WriteToFile(orderMaster.OrderTemplate, data);
        }

        public string Print(string inspectNo)
        {
            InspectMaster inspectMaster = queryMgr.FindById<InspectMaster>(inspectNo);
            IList<InspectDetail> inspectDetails = queryMgr.FindAll<InspectDetail>("select id from InspectDetail as id where id.InspectNo=?", inspectNo);
            inspectMaster.InspectDetails = inspectDetails;
            PrintInspectMaster printInspectMaster = Mapper.Map<InspectMaster, PrintInspectMaster>(inspectMaster);
            IList<object> data = new List<object>();
            var failCodeDic = this.genericMgr.FindAll<FailCode>().ToDictionary(d => d.Code, d => d);
            foreach (var inspectDetail in printInspectMaster.InspectDetails)
            {
                //Exception occurs when "FailCode" is null ,here eliminate exception since null value is rational
                inspectDetail.FailCode = this.genericMgr.FindAll<InspectResult>(" from InspectResult as i where i.InspectDetailId=? ", inspectDetail.Id).FirstOrDefault().FailCode ?? inspectDetail.FailCode;
                if (!string.IsNullOrWhiteSpace(inspectDetail.FailCode))
                {
                    inspectDetail.FailCode = failCodeDic[inspectDetail.FailCode].CodeDescription;
                }
            }

            data.Add(printInspectMaster);
            data.Add(printInspectMaster.InspectDetails);
            string reportFileUrl = reportGen.WriteToFile("InspectOrder.xls", data);
            //reportGen.WriteToClient(orderMaster.OrderTemplate, data, orderMaster.OrderTemplate);

            return reportFileUrl;
            //reportGen.WriteToFile(orderMaster.OrderTemplate, data);
        }

        #endregion

        #endregion

        #region private method
        private SearchStatementModel InspectResultPrepareSearchStatement(GridCommand command, InspectResultSearchModel searchModel)
        {
            string whereStatement = "where i.InspectNo = '" + searchModel.InspectNo + "'";
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("Item", searchModel.Item, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "JudgeResultDescription")
                {
                    command.SortDescriptors[0].Member = "JudgeResult";
                }
                else if (command.SortDescriptors[0].Member == "DefectDescription")
                {
                    command.SortDescriptors[0].Member = "Defect";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = InspectResultSelectCountStatement;
            searchStatementModel.SelectStatement = InspectResultSelectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        private string PrepareDetailSearchStatement(GridCommand command, InspectMasterSearchModel searchModel)
        {
            StringBuilder Sb = new StringBuilder();
            string whereStatement = "select detailResult.*,isnull(rest.handleQty,0) from ( select * from INP_InspectDet as id where 1=1";

            if (searchModel.IpNo != null || searchModel.ReceiptNo != null || searchModel.WMSNo != null)
            {
                whereStatement += " and exists (select 1 from INP_InspectMstr as i where  i.InpNo= id.InpNo ";
            }

            if (searchModel.IpNo != null && searchModel.IpNo != "")
            {
                whereStatement += " and i.IpNo='" + searchModel.IpNo + "'";
            }
            if (searchModel.WMSNo != null && searchModel.WMSNo != "")
            {
                whereStatement += " and i.WMSNo='" + searchModel.WMSNo + "'";
            }
            if (searchModel.ReceiptNo != null && searchModel.ReceiptNo != "")
            {
                whereStatement += " and i.RecNo='" + searchModel.ReceiptNo + "'";
            }
            if (searchModel.IpNo != null || searchModel.ReceiptNo != null || searchModel.WMSNo != null)
            {
                whereStatement += ")";
            }
            Sb.Append(whereStatement);
            if (searchModel.InspectNo != null)
            {
                Sb.Append(string.Format(" and id.InpNo = '{0}'", searchModel.InspectNo));
            }

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                Sb.Append(string.Format(" and id.Item ='{0}'", searchModel.Item));
            }
            if (searchModel.CreateUserName != null)
            {
                Sb.Append(string.Format(" and id.CreateUserNm = '{0}'", searchModel.CreateUserName));
            }

            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                Sb.Append(string.Format(" and id.CreateDate  between '{0}' and '{1}'", new object[] { searchModel.StartDate, searchModel.EndDate }));
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                Sb.Append(string.Format(" and id.CreateDate >= '{0}'", searchModel.StartDate));
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                Sb.Append(string.Format(" and id.CreateDate <= '{0}'", searchModel.EndDate));
            }

            Sb.Append(@" ) as detailResult left join  
                (select ir.inpdetid,sum(ir.HandleQty) 
                as handleQty from INP_InspectResult as ir group by inpdetid) as rest
                on detailResult.Id=rest.inpdetid  order by detailResult.CreateDate desc");
            return Sb.ToString();
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, InspectMasterSearchModel searchModel, string whereStatement)
        {
            IList<object> param = new List<object>();
            SecurityHelper.AddRegionPermissionStatement(ref whereStatement, "i", "Region");
            HqlStatementHelper.AddLikeStatement("InspectNo", searchModel.InspectNo, HqlStatementHelper.LikeMatchMode.End, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CreateUserName", searchModel.CreateUserName, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("IpNo", searchModel.IpNo, HqlStatementHelper.LikeMatchMode.End, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("WMSNo", searchModel.WMSNo, HqlStatementHelper.LikeMatchMode.End, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ReceiptNo", searchModel.ReceiptNo, HqlStatementHelper.LikeMatchMode.End, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "i", ref whereStatement, param);

            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "i", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "i", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "i", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "InspectTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
                }
                else if (command.SortDescriptors[0].Member == "InspectStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by i.CreateDate desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private string BatchJudge(string inspectNo, string idStr, string failCodeStr, string notes)
        {
            try
            {
                string[] idArr = idStr.Split(',');
                string[] noteArr = null;
                if (!string.IsNullOrEmpty(notes))
                {
                    noteArr = notes.Split(',');
                }
                string[] failCodeArr = null;
                if (!string.IsNullOrEmpty(failCodeStr))
                {
                    failCodeArr = failCodeStr.Split(',');
                }
                IList<InspectDetail> inspectDetailList = new List<InspectDetail>();
                for (int i = 0; i < idArr.Length; i++)
                {
                    InspectDetail inspectDetail = genericMgr.FindById<InspectDetail>(Convert.ToInt32(idArr[i]));
                    inspectDetail.CurrentQty = inspectDetail.InspectQty;
                    if (failCodeArr != null)
                    {
                        inspectDetail.JudgeFailCode = failCodeArr[i];
                    }
                    if (noteArr != null)
                    {
                        inspectDetail.CurrentInspectResultNote = noteArr[i];
                    }
                    inspectDetailList.Add(inspectDetail);
                }
                inspectMgr.JudgeInspectDetail(inspectDetailList);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_JudgeSuccessfully);
                return inspectNo;
            }
            catch (Exception ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.Message);
                return string.Empty;
            }
        }

        #endregion

        [SconitAuthorize(Permissions = "Url_InspectionOrder_New")]
        public string CreateRefReceiptNoInspectMaster(string ReceiptNo)
        {
            try
            {
                IList<ReceiptMaster> receiptList = genericMgr.FindAll<ReceiptMaster>(string.Format("from  ReceiptMaster r where r.ReceiptNo='{0}' ", ReceiptNo));

                if (receiptList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReceiveOrderNumberNotExits);
                }
                ReceiptMaster receiptEntity = receiptList[0];

                receiptEntity.ReceiptDetails = genericMgr.FindAll<ReceiptDetail>(string.Format(" from ReceiptDetail r where r.ReceiptNo='{0}'", ReceiptNo));
                foreach (ReceiptDetail ReceiptDetailEntity in receiptEntity.ReceiptDetails)
                {
                    ReceiptDetailEntity.IsInspect = true;
                    ReceiptDetailEntity.ReceiptLocationDetails = genericMgr.FindAll<ReceiptLocationDetail>(string.Format(" from ReceiptLocationDetail r where r.ReceiptNo='{0}'", ReceiptNo));
                }

                InspectMaster inspectMaster = inspectMgr.TransferReceipt2Inspect(receiptEntity);
                inspectMgr.CreateInspectMaster(inspectMaster);
                return string.Format(Resources.EXT.ControllerLan.Con_InspectionOrderNumberGeneratedSuccessfully, inspectMaster.InspectNo);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                string messages = "";
                for (int i = 0; i < ex.GetMessages().Count; i++)
                {
                    messages += ex.GetMessages()[i].GetMessageString();
                }
                Response.Write(messages);
                return string.Empty;
            }
        }
    }
}
