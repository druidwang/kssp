using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Utility;
using com.Sconit.Web.Models;
using com.Sconit.Entity.ORD;
using com.Sconit.Service;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.Exception;
using com.Sconit.Web.Models.ORD;
using com.Sconit.Entity.INV;
using com.Sconit.PrintModel.ORD;
using AutoMapper;

namespace com.Sconit.Web.Controllers.ORD
{
    public class OutMiscOrderController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from MiscOrderMaster as m";

        private static string selectStatement = "select m from MiscOrderMaster as m";

        //public IGenericMgr genericMgr { get; set; }
        public IMiscOrderMgr miscOrderMgr { get; set; }



        #region public method

        #region view
        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_View")]
        public ActionResult List(GridCommand GridCommand, OutMiscOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(GridCommand, searchModel);
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
            ViewBag.PageSize = base.ProcessPageSize(GridCommand.PageSize);
            return View();

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_View")]
        public ActionResult _AjaxList(GridCommand command, OutMiscOrderSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<MiscOrderMaster>()));
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MiscOrderMaster>(searchStatementModel, command));
        }
        #endregion

        #region new

        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_New")]
        public ActionResult New(string MoveType)
        {

            if (!string.IsNullOrWhiteSpace(MoveType))
            {
                MiscOrderMoveType miscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { MoveType, com.Sconit.CodeMaster.MiscOrderType.GI })[0];
                TempData["MiscOrderMoveType"] = miscOrderMoveType;
            }
            MiscOrderMaster miscOrderMaster = new MiscOrderMaster();
            miscOrderMaster.EffectiveDate = System.DateTime.Now;
            miscOrderMaster.MoveType = MoveType;
            return View(miscOrderMaster);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_New")]
        public ActionResult New(MiscOrderMaster MiscOrderMaster)
        {
            MiscOrderMoveType MiscOrderMoveType = null;
            if (ModelState.IsValid)
            {
                MiscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { MiscOrderMaster.MoveType, com.Sconit.CodeMaster.MiscOrderType.GI })[0];
                // TempData["MiscOrderMoveType"] = MiscOrderMoveType;

                if (MiscOrderMoveType.CheckAsn && MiscOrderMaster.Asn == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ShipOrderCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckRecLoc && MiscOrderMaster.ReceiveLocation == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ReceiveLocationCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckNote && MiscOrderMaster.Note == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_NoteCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckCostCenter && MiscOrderMaster.CostCenter == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CostCenterCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckRefNo && MiscOrderMaster.ReferenceNo == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_refertoCountCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckDeliverRegion && MiscOrderMaster.DeliverRegion == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ReceiveAreaCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckWBS && MiscOrderMaster.WBS == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_WBSCanNotBeEmpty);
                }
                else
                {
                    try
                    {
                        MiscOrderMaster.MoveType = MiscOrderMoveType.MoveType;
                        MiscOrderMaster.CancelMoveType = MiscOrderMoveType.CancelMoveType;
                        miscOrderMgr.CreateMiscOrder(MiscOrderMaster);
                        SaveSuccessMessage(Resources.EXT.ControllerLan.Con_AddedSuccessfully);
                        return RedirectToAction("Edit/" + MiscOrderMaster.MiscOrderNo);

                    }
                    catch (BusinessException ex)
                    {
                        SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                    }

                }
            }
            else
            {
                if (MiscOrderMaster.MoveType != null)
                {
                    MiscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { MiscOrderMaster.MoveType, com.Sconit.CodeMaster.MiscOrderType.GI })[0];
                }
            }

            // MiscOrderMoveType miscOrderMoveType = genericMgr.FindById<MiscOrderMoveType>(int.Parse(MiscOrderMaster.MoveType));
            TempData["MiscOrderMoveType"] = MiscOrderMoveType;
            return View(MiscOrderMaster);
        }
        #endregion

        #region  Edit
        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_New")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                MiscOrderMaster miscOrderMaster = this.genericMgr.FindById<MiscOrderMaster>(id);
                miscOrderMaster.StatusDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString());
                miscOrderMaster.QualityTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.QualityType, ((int)miscOrderMaster.QualityType).ToString());
                MiscOrderMoveType miscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { miscOrderMaster.MoveType, com.Sconit.CodeMaster.MiscOrderType.GI })[0];
                ViewBag.editorTemplate = miscOrderMaster.Status == com.Sconit.CodeMaster.MiscOrderStatus.Create ? "" : "ReadonlyTextBox";
                TempData["MiscOrderMoveType"] = miscOrderMoveType;
                return View(miscOrderMaster);
            }
        }


        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_New")]
        public ActionResult Edit(MiscOrderMaster miscOrderMaster)
        {

            if (ModelState.IsValid)
            {
                MiscOrderMoveType MiscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { miscOrderMaster.MoveType, com.Sconit.CodeMaster.MiscOrderType.GI })[0];
                if (MiscOrderMoveType.CheckAsn && miscOrderMaster.Asn == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ShipOrderCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckRecLoc && miscOrderMaster.ReceiveLocation == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ReceiveLocationCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckNote && miscOrderMaster.Note == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_NoteCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckCostCenter && miscOrderMaster.CostCenter == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CostCenterCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckRefNo && miscOrderMaster.ReferenceNo == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_refertoCountCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckDeliverRegion && miscOrderMaster.DeliverRegion == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ShippingAreaCanNotBeEmpty);
                }
                else if (MiscOrderMoveType.CheckWBS && miscOrderMaster.WBS == null)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_WBSCanNotBeEmpty);
                }
                else
                {
                    try
                    {
                        miscOrderMgr.UpdateMiscOrder(miscOrderMaster);
                        SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ModificateSuccessfully);
                    }
                    catch (BusinessException ex)
                    {

                        SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                    }
                }
            }
            return RedirectToAction("Edit/" + miscOrderMaster.MiscOrderNo);

        }

        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_New")]
        public ActionResult btnDelete(string id)
        {
            try
            {
                MiscOrderMaster MiscOrderMaster = genericMgr.FindById<MiscOrderMaster>(id);
                miscOrderMgr.DeleteMiscOrder(MiscOrderMaster);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_DeletedSuccessfully);


            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());

            }
            return RedirectToAction("List");

        }

        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_New")]
        public ActionResult btnClose(string id)
        {
            try
            {
                MiscOrderMaster MiscOrderMaster = genericMgr.FindById<MiscOrderMaster>(id);
                IList<MiscOrderDetail> miscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>("from MiscOrderDetail as m where m.MiscOrderNo=?", MiscOrderMaster.MiscOrderNo);
                if (miscOrderDetailList.Count < 1)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_DetailBeEmptyCanNotExecuteConfirm);
                }
                else
                {

                    foreach (var miscOrderDetail in miscOrderDetailList)
                    {
                        CheckMiscOrderDetail(miscOrderDetail, MiscOrderMaster.MoveType, (int)com.Sconit.CodeMaster.MiscOrderType.GI);

                    }
                    miscOrderMgr.CloseMiscOrder(MiscOrderMaster);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ConfirmSuccessfully);
                }
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());

            }
            return RedirectToAction("Edit/" + id);
        }


        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_New")]
        public ActionResult btnCancel(string id)
        {
            try
            {
                MiscOrderMaster MiscOrderMaster = genericMgr.FindById<MiscOrderMaster>(id);
                miscOrderMgr.CancelMiscOrder(MiscOrderMaster);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CancelledSuccessfully);

            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());

            }
            return RedirectToAction("Edit/" + id);
        }

        #region MiscOrderDetail
        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_New")]
        public ActionResult _MiscOrderDetailNoScanHu(string MiscOrderNo, string MoveType, string Status)
        {
            MiscOrderMoveType MiscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { MoveType, com.Sconit.CodeMaster.MiscOrderType.GI })[0];
            MiscOrderMaster miscOrder = genericMgr.FindById<MiscOrderMaster>(MiscOrderNo);
            ViewBag.editorTemplate = miscOrder.Status == com.Sconit.CodeMaster.MiscOrderStatus.Create ? "" : "ReadonlyTextBox";
            ViewBag.MoveType = MoveType;
            ViewBag.MiscOrderNo = MiscOrderNo;
            ViewBag.Status = Status;
            ViewBag.ReserveLine = MiscOrderMoveType.CheckReserveLine;
            ViewBag.ReserveNo = MiscOrderMoveType.CheckReserveNo;
            ViewBag.EBELN = MiscOrderMoveType.CheckEBELN;
            ViewBag.EBELP = MiscOrderMoveType.CheckEBELP;
            return PartialView();

        }

        public ActionResult _MiscOrderDetailIsScanHu(string MiscOrderNo, string MoveType, string Status)
        {
            MiscOrderMoveType MiscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { MoveType, com.Sconit.CodeMaster.MiscOrderType.GI })[0];

            ViewBag.MoveType = MoveType;
            ViewBag.MiscOrderNo = MiscOrderNo;
            ViewBag.Status = Status;
            ViewBag.ReserveLine = MiscOrderMoveType.CheckReserveLine;
            ViewBag.ReserveNo = MiscOrderMoveType.CheckReserveNo;
            ViewBag.EBELN = MiscOrderMoveType.CheckEBELN;
            ViewBag.EBELP = MiscOrderMoveType.CheckEBELP;
            return PartialView();

        }

        [GridAction]
        public ActionResult _SelectMiscOrderDetail(string MiscOrderNo, string MoveType)
        {
            ViewBag.MiscOrderNo = MiscOrderNo;
            IList<MiscOrderDetail> MiscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>("from MiscOrderDetail as m where m.MiscOrderNo=?", MiscOrderNo);
            return View(new GridModel(MiscOrderDetailList));
        }

        [GridAction]
        public ActionResult _SelectMiscOrderLocationDetail(string MiscOrderNo, string MoveType)
        {
            ViewBag.MiscOrderNo = MiscOrderNo;
            IList<MiscOrderDetail> MiscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>("from MiscOrderDetail as m where m.MiscOrderNo=?", MiscOrderNo);
            IList<MiscOrderLocationDetail> miscOrderLocationDetailList = genericMgr.FindAll<MiscOrderLocationDetail>("from MiscOrderLocationDetail as m where m.MiscOrderNo=?", MiscOrderNo);
            foreach (MiscOrderLocationDetail miscOrderLocationDetail in miscOrderLocationDetailList)
            {
                MiscOrderDetail miscOrderDetail = MiscOrderDetailList.Where(m => m.Id == miscOrderLocationDetail.MiscOrderDetailId).ToList().First();
                miscOrderLocationDetail.Id = miscOrderDetail.Id;
                miscOrderLocationDetail.ReferenceItemCode = miscOrderDetail.ReferenceItemCode;
                miscOrderLocationDetail.ItemDescription = miscOrderDetail.ItemDescription;
                miscOrderLocationDetail.UnitCount = miscOrderDetail.UnitCount;
                miscOrderLocationDetail.Location = miscOrderDetail.Location;
                miscOrderLocationDetail.ReserveNo = miscOrderDetail.ReserveNo;
                miscOrderLocationDetail.ReserveLine = miscOrderDetail.ReserveLine;
                miscOrderLocationDetail.EBELN = miscOrderDetail.EBELN;
                miscOrderLocationDetail.EBELP = miscOrderDetail.EBELP;
                miscOrderLocationDetail.ManufactureParty = genericMgr.FindById<Hu>(miscOrderLocationDetail.HuId).ManufactureParty;
            }
            return View(new GridModel(miscOrderLocationDetailList));
        }

        public ActionResult _WebOrderDetail(string Code)
        {
            if (!string.IsNullOrEmpty(Code))
            {

                WebOrderDetail webOrderDetail = new WebOrderDetail();
                Item item = genericMgr.FindById<Item>(Code);
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

        [GridAction]
        [SconitAuthorize(Permissions = "Url_StockTake_Edit")]
        public bool CheckMiscOrderDetail(MiscOrderDetail miscOrderDetail, string MoveType, int IOType)
        {
            bool isValid = true;
            if (!string.IsNullOrEmpty(MoveType))
            {

                MiscOrderMoveType miscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { MoveType, IOType })[0];
                if (miscOrderMoveType.CheckEBELN && miscOrderDetail.EBELN == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_PurchaseOrderNumberCanNotBeEmpty);
                }
                else if (miscOrderMoveType.CheckEBELP && miscOrderDetail.EBELP == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_PurchaseOrderRowNumberCanNotBeEmpty);
                }
                else if (miscOrderMoveType.CheckReserveLine && miscOrderDetail.ReserveLine == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReserveRowCanNotBeEmpty);
                }
                else if (miscOrderMoveType.CheckReserveNo && miscOrderDetail.ReserveNo == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReserveNumberCanNotBeEmpty);
                }
                else if (miscOrderDetail.Qty == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_QuantityCanNotBeEmpty);
                }
                else if (miscOrderDetail.Item == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ItemCanNotBeEmpty);
                }
                else
                {
                    //IList<MiscOrderDetail> MiscOrderDetailist = genericMgr.FindAll<MiscOrderDetail>("from MiscOrderDetail as m where m.Item=? and m.MiscOrderNo=?", new object[] { miscOrderDetail.Item, miscOrderDetail.MiscOrderNo });
                    //if (MiscOrderDetailist.Count > 0 )
                    //{
                    //    throw new BusinessException("物料已经存在");
                    //}
                }
            }
            return isValid;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Edit")]
        public string _SaveMiscOrderDetail(
            [Bind(Prefix = "updated")]IEnumerable<MiscOrderDetail> updatedMiscOrderDetails,
            [Bind(Prefix = "deleted")]IEnumerable<MiscOrderDetail> deletedMiscOrderDetails, string MiscOrderNo, string moveType)
        {
            try
            {
                IList<MiscOrderDetail> newMiscOrderDetailList = new List<MiscOrderDetail>();
                IList<MiscOrderDetail> updateMiscOrderDetailList = new List<MiscOrderDetail>();
                if (updatedMiscOrderDetails != null)
                {
                    foreach (var miscOrderDetail in updatedMiscOrderDetails)
                    {
                        if (CheckMiscOrderDetail(miscOrderDetail, moveType, (int)com.Sconit.CodeMaster.MiscOrderType.GI))
                        {
                            updateMiscOrderDetailList.Add(miscOrderDetail);
                        }
                    }
                }


                miscOrderMgr.BatchUpdateMiscOrderDetails(MiscOrderNo, newMiscOrderDetailList, updateMiscOrderDetailList, (IList<MiscOrderDetail>)deletedMiscOrderDetails);
                return MiscOrderNo;

            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return string.Empty;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Edit")]
        public string _SaveBatchEditing([Bind(Prefix =
            "inserted")]IEnumerable<MiscOrderDetail> insertedMiscOrderDetails,
            [Bind(Prefix = "updated")]IEnumerable<MiscOrderDetail> updatedMiscOrderDetails,
            [Bind(Prefix = "deleted")]IEnumerable<MiscOrderDetail> deletedMiscOrderDetails,
            string MiscOrderNo, string moveType)
        {
            try
            {
                IList<MiscOrderDetail> newMiscOrderDetailList = new List<MiscOrderDetail>();
                IList<MiscOrderDetail> updateMiscOrderDetailList = new List<MiscOrderDetail>();
                if (insertedMiscOrderDetails != null)
                {
                    foreach (var miscOrderDetail in insertedMiscOrderDetails)
                    {
                        if (CheckMiscOrderDetail(miscOrderDetail, moveType, (int)com.Sconit.CodeMaster.MiscOrderType.GI))
                        {
                            Item item = genericMgr.FindById<Item>(miscOrderDetail.Item);
                            miscOrderDetail.ItemDescription = item.Description;
                            miscOrderDetail.UnitCount = item.UnitCount;
                            miscOrderDetail.Uom = item.Uom;
                            miscOrderDetail.BaseUom = item.Uom;
                            miscOrderDetail.MiscOrderNo = MiscOrderNo;
                            newMiscOrderDetailList.Add(miscOrderDetail);
                        }
                    }
                }
                if (updatedMiscOrderDetails != null)
                {
                    foreach (var miscOrderDetail in updatedMiscOrderDetails)
                    {
                        if (CheckMiscOrderDetail(miscOrderDetail, moveType, (int)com.Sconit.CodeMaster.MiscOrderType.GI))
                        {
                            updateMiscOrderDetailList.Add(miscOrderDetail);
                        }
                    }
                }
                miscOrderMgr.BatchUpdateMiscOrderDetails(MiscOrderNo, newMiscOrderDetailList, updateMiscOrderDetailList, (IList<MiscOrderDetail>)deletedMiscOrderDetails);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_UpdatedSuccessfully);
                return MiscOrderNo;
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



        #region print
        public string Print(string miscOrderNo)
        {
            MiscOrderMaster miscOrderMaster = queryMgr.FindById<MiscOrderMaster>(miscOrderNo);
            IList<MiscOrderDetail> miscOrderDetails = queryMgr.FindAll<MiscOrderDetail>("select od from MiscOrderDetail as od where od.MiscOrderNo=?", miscOrderNo);
            miscOrderMaster.MiscOrderDetails = miscOrderDetails;
            PrintMiscOrderMaster printPickListMaster = Mapper.Map<MiscOrderMaster, PrintMiscOrderMaster>(miscOrderMaster);
            IList<object> data = new List<object>();
            data.Add(printPickListMaster);
            data.Add(printPickListMaster.MiscOrderDetails);
            return reportGen.WriteToFile("MiscOrder.xls", data);
        }
        #endregion

        #endregion

        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, OutMiscOrderSearchModel searchModel)
        {
            string whereStatement = "where m.Type = " + (int)com.Sconit.CodeMaster.MiscOrderType.GI;
            IList<object> param = new List<object>();
            SecurityHelper.AddRegionPermissionStatement(ref whereStatement, "m", "Region");

            HqlStatementHelper.AddLikeStatement("MiscOrderNo", searchModel.MiscOrderNo, HqlStatementHelper.LikeMatchMode.Start, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "m", ref  whereStatement, param);
            HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "m", ref  whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "m", ref  whereStatement, param);

            HqlStatementHelper.AddEqStatement("MoveType", searchModel.MoveType, "m", ref  whereStatement, param);


            HqlStatementHelper.AddEqStatement("CostCenter", searchModel.CostCenter, "m", ref  whereStatement, param);
            HqlStatementHelper.AddEqStatement("CreateUserName", searchModel.CreateUserName, "m", ref  whereStatement, param);
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("EffectiveDate", searchModel.StartDate, searchModel.EndDate, "m", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("EffectiveDate", searchModel.StartDate, "m", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("EffectiveDate", searchModel.EndDate, "m", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }

            string sortingStatement = string.Empty;
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by CreateDate desc";
            }
            else
            {
                sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        [SconitAuthorize(Permissions = "Url_Inventory_OutMiscOrder_View")]
        public ActionResult ImportOutMiscOrderDetail(IEnumerable<HttpPostedFileBase> attachments, string MiscOrderNo)
        {
            try
            {
                foreach (var file in attachments)
                {
                    miscOrderMgr.CreateMiscOrderDetailFromXls(file.InputStream, MiscOrderNo);
                    object obj = Resources.EXT.ControllerLan.Con_ImportSuccessfully;
                    return Json(new { status = obj }, "text/plain");
                }
            }
            catch (BusinessException ex)
            {
                Response.Write(ex.GetMessages()[0].GetMessageString());
            }
            return null;
        }
        #endregion
    }
}
