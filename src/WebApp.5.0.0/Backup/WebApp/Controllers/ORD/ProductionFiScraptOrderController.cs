using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SYS;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Service;
using com.Sconit.Utility;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.INV;
using Telerik.Web.Mvc;
using System.Web;
using com.Sconit.Entity.SCM;

namespace com.Sconit.Web.Controllers.ORD
{
    public class ProductionFiScraptOrderController : WebAppBaseController
    {
        private static string selectCountStatement = "select count(*) from MiscOrderMaster as m";

        private static string selectStatement = "select m from MiscOrderMaster as m";

        private static string selectInspectResult = @"select r from InspectResult r where r.InspectNo = ? and r.RejectHandleResult=? 
            and r.IsHandle=? and r.JudgeQty > r.HandleQty";

        private static string selectRejectDetail = @"select r from RejectDetail as r where r.RejectNo=? and r.HandleQty > r.HandledQty
            and exists (select 1 from RejectMaster as m where r.RejectNo = m.RejectNo and m.HandleResult =? )";

        //public IGenericMgr genericMgr { get; set; }
        public IMiscOrderMgr miscOrderMgr { get; set; }

        #region public method

        #region view
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_View")]
        public ActionResult Index()
        {
            ViewBag.SearchType = "CostCenter";
            return View();
        }
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrderDet_View")]
        public ActionResult DetailIndex()
        {
            ViewBag.SearchType = "CostCenter";
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrder_View")]
        public ActionResult ScrapIndex()
        {
            ViewBag.SearchType = "Scrap";
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrderDet_View")]
        public ActionResult ScrapDetailIndex()
        {
            ViewBag.SearchType = "ScrapDetail";
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_View")]
        public ActionResult List(GridCommand GridCommand, MiscOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(GridCommand, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {

            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.SearchType = "CostCenter";
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(GridCommand.PageSize);
            return View();

        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrderDet_View")]
        public ActionResult DetailList(GridCommand GridCommand, MiscOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(GridCommand, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {

            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.SearchType = "CostCenter";
            ViewBag.PageSize = base.ProcessPageSize(GridCommand.PageSize);
            return View();

        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrder_View")]
        public ActionResult ScrapSearchList(GridCommand GridCommand, MiscOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(GridCommand, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {

            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.SearchType = "Scrap";
            ViewBag.PageSize = base.ProcessPageSize(GridCommand.PageSize);
            return View();

        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrderDet_View")]
        public ActionResult ScrapDetailList(GridCommand GridCommand, MiscOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(GridCommand, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {

            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.SearchType = "ScrapDetail";
            ViewBag.PageSize = base.ProcessPageSize(GridCommand.PageSize);
            return View();

        }
        public ActionResult ScrapList(GridCommand GridCommand, MiscOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(GridCommand, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {

            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.SearchType = "Scrap";
            ViewBag.PageSize = base.ProcessPageSize(GridCommand.PageSize);
            return RedirectToAction("ScrapNew");

        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_View,Url_ProductionFiScrapOrder_View")]
        public ActionResult _AjaxList(GridCommand command, MiscOrderSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<MiscOrderMaster>()));
            }
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            var list = GetAjaxPageData<MiscOrderMaster>(searchStatementModel, command);
            foreach (var data in list.Data)
            {
                if (data.Note == "27")
                {
                    data.Note = @Resources.SYS.CodeDetail.CodeDetail_SY27;
                }
                else if (data.Note == "28")
                {
                    data.Note = @Resources.SYS.CodeDetail.CodeDetail_SY28;
                }
                else if (data.Note == "29")
                {
                    data.Note = @Resources.SYS.CodeDetail.CodeDetail_SY29;
                }
            }
            return PartialView(list);
        }
        #endregion

        #region new
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_New")]
        public ActionResult New(string referenceNo)
        {
            ViewBag.ReferenceDocumentsType = 0;
            ViewBag.ReferenceNo = referenceNo;
            var miscOrderMaster = new MiscOrderMaster();
            if (!string.IsNullOrWhiteSpace(referenceNo))
            {
                try
                {
                    miscOrderMaster.ReferenceNo = referenceNo;
                    var snRuleList = this.genericMgr.FindAllIn<SNRule>
                               (" from SNRule where Code in(?", new object[]{
                                    CodeMaster.DocumentsType.REJ ,
                                    CodeMaster.DocumentsType.INS });
                    var matchSnRule = snRuleList.FirstOrDefault(p => miscOrderMaster.ReferenceNo.StartsWith(p.PreFixed));
                    if (matchSnRule != null)
                    {
                        if (matchSnRule.Code == (int)CodeMaster.DocumentsType.INS)
                        {
                            var inspectResult = genericMgr.FindAll<InspectResult>(selectInspectResult,
                        new object[] { miscOrderMaster.ReferenceNo, com.Sconit.CodeMaster.HandleResult.Scrap, false }).First();
                            miscOrderMaster.Location = inspectResult.CurrentLocation;
                            miscOrderMaster.IsScanHu = !string.IsNullOrWhiteSpace(inspectResult.HuId);
                        }
                        else if (matchSnRule.Code == (int)CodeMaster.DocumentsType.REJ)
                        {
                            var rejectDetail = genericMgr.FindAll<RejectDetail>(selectRejectDetail,
                                new object[] { miscOrderMaster.ReferenceNo, com.Sconit.CodeMaster.HandleResult.Scrap }).First();
                            miscOrderMaster.Location = rejectDetail.CurrentLocation;
                            miscOrderMaster.IsScanHu = !string.IsNullOrWhiteSpace(rejectDetail.HuId);
                        }
                        ViewBag.ReferenceDocumentsType = matchSnRule.Code;
                        miscOrderMaster.ReferenceDocumentsType = matchSnRule.Code;
                    }
                }
                catch (Exception ex)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CanNotFindRefDetail + ex.Message);
                }
                miscOrderMaster.QualityType = CodeMaster.QualityType.Reject;
            }
            return View(miscOrderMaster);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_New")]
        public ActionResult CreateMiscOrder(MiscOrderMaster miscOrderMaster, string sequences, string items, string locations, string qtys)
        {
            try
            {
                //CreateNewMiscOrder(miscOrderMaster, sequences, items, locations, qtys);
                return View("Edit", miscOrderMaster);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrder_New")]
        public ActionResult CreateScrapMiscOrder(MiscOrderMaster miscOrderMaster, string sequences, string items, string locations, string qtys, string uoms)
        {
            try
            {
                var errorMessageList = new List<ErrorMessage>();
                uoms = uoms.ToUpper();
                string msg = "";
                string[] ItemsArray = items.Split(',');
                string[] UomsArray = uoms.Split(',');
                var q = from p in ItemsArray
                        group p by p into g
                        select new
                        {
                            g.Key,
                            ItemCount = g.Count()
                        };
                foreach (var qd in q.Where(p=>p.ItemCount>1))
                {
                    msg=msg+qd.Key+",";
                }
                if (msg != "")
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ItemDetailsDuplicate + msg);
                    return Json(null);
                }
                for (int i = 0; i < ItemsArray.Length; i++)
                {
                    var item = this.genericMgr.FindById<Item>(ItemsArray[i]);
                    if (item.ItemCategory != "FERT")
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_ExistNonHalfFinishGoodsType);
                        return Json(null);
                    }
                    if (item.Uom != UomsArray[i])
                    {
                        try
                        {
                            itemMgr.ConvertItemUomQty(item.Code, item.Uom, 1, UomsArray[i]);
                        }
                        catch (Exception ex)
                        {
                            msg += ex.Message;
                        }

                    }
                }
                if (msg != "")
                {
                    SaveErrorMessage(msg);
                    return Json(null);
                }
                CreateNewMiscOrder(miscOrderMaster, sequences, items, locations, qtys, uoms, true);
                return View("ScrapEdit", miscOrderMaster);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        private void CreateNewMiscOrder(MiscOrderMaster miscOrderMaster, string sequences, string items, string locations, string qtys, string uoms, bool isScrap = false)
        {
            if (items == string.Empty && !miscOrderMaster.IsScanHu)
            {
                throw new BusinessException(Resources.EXT.ControllerLan.Con_DetailBeEmptyCanNotCreate);
            }
            MiscOrderMoveType miscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>(
                "from MiscOrderMoveType as m where m.MoveType=? and m.SubType=? ",
                new object[] { miscOrderMaster.MoveType, com.Sconit.CodeMaster.MiscOrderSubType.COST })[0];
            
            //页面不加生效日期
            //miscOrderMaster.EffectiveDate = DateTime.Now;
            miscOrderMaster.CostCenter = genericMgr.FindById<FlowMaster>(miscOrderMaster.Flow).CostCenter;
            miscOrderMaster.Location = genericMgr.FindById<FlowMaster>(miscOrderMaster.Flow).LocationTo;
            miscOrderMaster.Type = miscOrderMoveType.IOType;
            if (!isScrap)
            {
                miscOrderMaster.SubType = miscOrderMoveType.SubType;
            }
            miscOrderMaster.MoveType = miscOrderMoveType.MoveType;
            miscOrderMaster.CancelMoveType = miscOrderMoveType.CancelMoveType;
            miscOrderMaster.Status = CodeMaster.MiscOrderStatus.Create;
            //miscOrderMaster.QualityType = CodeMaster.QualityType.Qualified;
            miscOrderMaster.Region = this.genericMgr.FindById<Location>(miscOrderMaster.Location).Region;
            if (!miscOrderMaster.IsScanHu)
            {
                string[] SequencesArray = sequences.Split(',');
                string[] ItemsArray = items.Split(',');
                string[] OrderedQtysArray = qtys.Split(',');
                string[] LocationsArray = locations.Split(',');
                string[] UomArray = uoms.Split(',');

                IList<MiscOrderDetail> miscOrderDetails = new List<MiscOrderDetail>();
                for (int i = 0; i < ItemsArray.Length; i++)
                {
                    var item = this.genericMgr.FindById<Item>(ItemsArray[i]);
                    MiscOrderDetail od = new MiscOrderDetail();
                    od.Sequence = Convert.ToInt32(SequencesArray[i]);
                    od.Item = item.Code;
                    od.ItemDescription = item.Description;
                    od.ReferenceItemCode = item.ReferenceCode;
                    od.Uom = UomArray[i];
                    od.BaseUom = item.Uom;
                    od.UnitCount = item.UnitCount;
                    od.UnitQty = 1;
                    od.Qty = Convert.ToDecimal(OrderedQtysArray[i]);
                    if (!string.IsNullOrWhiteSpace(LocationsArray[i]))
                    {
                        od.Location = LocationsArray[i];
                    }
                    miscOrderDetails.Add(od);
                }
                miscOrderMaster.MiscOrderDetails = (from p in miscOrderDetails
                                                    group p by new
                                                    {
                                                        p.Item,
                                                        p.ItemDescription,
                                                        p.ReferenceItemCode,
                                                        p.Uom,
                                                        p.BaseUom,
                                                        p.UnitCount,
                                                        p.Location
                                                    } into g
                                                    select new MiscOrderDetail
                                                    {
                                                        Sequence = g.Max(p => p.Sequence),
                                                        Item = g.Key.Item,
                                                        ItemDescription = g.Key.ItemDescription,
                                                        ReferenceItemCode = g.Key.ReferenceItemCode,
                                                        Uom = g.Key.Uom,
                                                        BaseUom = g.Key.BaseUom,
                                                        UnitCount = g.Key.UnitCount,
                                                        UnitQty = 1,
                                                        Location = g.Key.Location,
                                                        Qty = g.Sum(p => p.Qty),
                                                    }).ToList();
            }
            if (miscOrderMaster.ReferenceDocumentsType > 0)
            {
                miscOrderMgr.QuickCreateMiscOrder(miscOrderMaster, miscOrderMaster.EffectiveDate);
            }
            else
            {
                miscOrderMgr.CreateMiscOrder(miscOrderMaster);
            }
            ViewBag.miscOrderNo = miscOrderMaster.MiscOrderNo;
            ViewBag.Location = miscOrderMaster.Location;
            SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CreateSuccessfully);
        }
        #endregion
        #region Save MiscOrderDet for scanned HuIds.
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_New")]
        public string _SaveMiscOrderDetail(
            [Bind(Prefix = "updated")]IEnumerable<MiscOrderDetail> updatedMiscOrderDetails,
            [Bind(Prefix = "deleted")]IEnumerable<MiscOrderLocationDetail> deletedMiscOrderDetails, string MiscOrderNo, string moveType)
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


                miscOrderMgr.BatchUpdateMiscLocationOrderDetails(MiscOrderNo, newMiscOrderDetailList, updateMiscOrderDetailList, (IList<MiscOrderLocationDetail>)deletedMiscOrderDetails);
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
        #region   CheckMiscOrderDetail
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_New")]
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
        #endregion
        #region MisOrderDetailScanHu
        public ActionResult _MiscOrderDetailIsScanHu(string MiscOrderNo, string MoveType, string Status)
        {
            //MiscOrderMoveType MiscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { MoveType, com.Sconit.CodeMaster.MiscOrderType.GI })[0];

            ViewBag.MoveType = MoveType;
            ViewBag.MiscOrderNo = MiscOrderNo;
            ViewBag.Status = Status;
            //ViewBag.ReserveLine = MiscOrderMoveType.CheckReserveLine;
            //ViewBag.ReserveNo = MiscOrderMoveType.CheckReserveNo;
            //ViewBag.EBELN = MiscOrderMoveType.CheckEBELN;
            //ViewBag.EBELP = MiscOrderMoveType.CheckEBELP;
            return PartialView();

        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _SelectMiscOrderLocationDetail(string MiscOrderNo, string MoveType)
        {
            ViewBag.MiscOrderNo = MiscOrderNo;
            IList<MiscOrderDetail> MiscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>("from MiscOrderDetail as m where m.MiscOrderNo=?", MiscOrderNo);
            IList<MiscOrderLocationDetail> miscOrderLocationDetailList = genericMgr.FindAll<MiscOrderLocationDetail>("from MiscOrderLocationDetail as m where m.MiscOrderNo=?", MiscOrderNo);
            foreach (MiscOrderLocationDetail miscOrderLocationDetail in miscOrderLocationDetailList)
            {
                MiscOrderDetail miscOrderDetail = MiscOrderDetailList.Where(m => m.Id == miscOrderLocationDetail.MiscOrderDetailId).ToList().First();
                //miscOrderLocationDetail.Id = miscOrderDetail.Id;
                miscOrderLocationDetail.ReferenceItemCode = miscOrderDetail.ReferenceItemCode;
                miscOrderLocationDetail.ItemDescription = miscOrderDetail.ItemDescription;
                miscOrderLocationDetail.UnitCount = miscOrderDetail.UnitCount;
                miscOrderLocationDetail.Location = miscOrderDetail.Location;
                miscOrderLocationDetail.ReserveNo = miscOrderDetail.ReserveNo;
                miscOrderLocationDetail.ReserveLine = miscOrderDetail.ReserveLine;
                miscOrderLocationDetail.EBELN = miscOrderDetail.EBELN;
                miscOrderLocationDetail.EBELP = miscOrderDetail.EBELP;
            }
            return View(new GridModel(miscOrderLocationDetailList));
        }
        #endregion
        #region MiscOrderDetail

        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_New,Url_ProductionFiScrapOrder_New")]
        public ActionResult _MiscOrderDetail(string miscOrderNo, string location, string referenceNo, int? referenceDocumentsType, string Flow)
        {
            ViewBag.ReferenceDocumentsType = 0;
            ViewBag.IsScanHu = false;
            if (!string.IsNullOrEmpty(miscOrderNo))
            {
                MiscOrderMaster miscOrder = genericMgr.FindById<MiscOrderMaster>(miscOrderNo);
                ViewBag.Status = miscOrder.Status;
                ViewBag.IsCreate = miscOrder.Status == com.Sconit.CodeMaster.MiscOrderStatus.Create ? true : false;
                ViewBag.IsScanHu = miscOrder.IsScanHu;
            }
            else
            {
                ViewBag.IsCreate = true;
                if (!string.IsNullOrEmpty(referenceNo) && referenceDocumentsType.HasValue)
                {
                    ViewBag.ReferenceDocumentsType = referenceDocumentsType;
                }
            }
            ViewBag.ReferenceNo = referenceNo;
            ViewBag.MiscOrderNo = miscOrderNo;
            ViewBag.Location = location;
            ViewBag.Coupled = true;
            ViewBag.Flow = Flow;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_New,Url_ProductionFiScrapOrder_New")]
        public ActionResult _SelectMiscOrderDetail(string miscOrderNo, string location, string referenceNo,string flow)
        {
            IList<MiscOrderDetail> miscOrderDetailList = new List<MiscOrderDetail>();
            try
            {
                var multiRows = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.GridDefaultMultiRowsCount);
                int v;
                int.TryParse(multiRows, out v);
                if (!string.IsNullOrWhiteSpace(flow) && string.IsNullOrWhiteSpace(miscOrderNo))
                {
                    v = -1;
                    var flowdet = flowMgr.GetFlowDetailList(flow);
                    foreach (var flowd in flowdet)
                    {
                        var miscOrderDetail = new MiscOrderDetail();
                        miscOrderDetail.Item = flowd.Item;
                        miscOrderDetail.ItemDescription = itemMgr.GetCacheItem(flowd.Item).Description;
                        miscOrderDetail.ReferenceItemCode = itemMgr.GetCacheItem(flowd.Item).ReferenceCode;
                        miscOrderDetail.Uom = itemMgr.GetCacheItem(flowd.Item).Uom;
                        miscOrderDetail.Qty = 0;
                        miscOrderDetailList.Add(miscOrderDetail);
                    }
                }
                else if (!string.IsNullOrEmpty(miscOrderNo))
                {
                    v = -1;
                    miscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>("from MiscOrderDetail as m where m.MiscOrderNo=? ", miscOrderNo);
                }
                else if (!string.IsNullOrEmpty(referenceNo))
                {
                    v = -1;
                    var snRuleList = this.genericMgr.FindAllIn<SNRule>
                        (" from SNRule where Code in(?", new object[]{
                                    CodeMaster.DocumentsType.REJ ,
                                    CodeMaster.DocumentsType.INS });
                    var matchSnRule = snRuleList.FirstOrDefault(p => referenceNo.StartsWith(p.PreFixed));
                    if (matchSnRule == null)
                    {
                        throw new BusinessException(Resources.EXT.ControllerLan.Con_NotSupportTheTypeOrderReference);
                    }
                    if (matchSnRule.Code == (int)CodeMaster.DocumentsType.INS)
                    {
                        miscOrderDetailList = genericMgr.FindAll<InspectResult>(selectInspectResult,
                        new object[] { referenceNo, com.Sconit.CodeMaster.HandleResult.Scrap, false })
                        .GroupBy(p => new { p.Item, p.Uom, p.CurrentLocation }, (k, g) => new { k, g })
                        .Select(p => new MiscOrderDetail
                        {
                            Item = p.k.Item,
                            ItemDescription = p.g.First().ItemDescription,
                            ReferenceItemCode = p.g.First().ReferenceItemCode,
                            Uom = p.k.Uom,
                            Location = p.k.CurrentLocation,
                            Qty = p.g.Sum(q => q.TobeHandleQty) * p.g.First().UnitQty
                        }).ToList();
                    }
                    else if (matchSnRule.Code == (int)CodeMaster.DocumentsType.REJ)
                    {
                        miscOrderDetailList = genericMgr.FindAll<RejectDetail>(selectRejectDetail,
                            new object[] { referenceNo, com.Sconit.CodeMaster.HandleResult.Scrap })
                           .GroupBy(p => new { p.Item, p.Uom, p.CurrentLocation }, (k, g) => new { k, g })
                           .Select(p => new MiscOrderDetail
                           {
                               Item = p.k.Item,
                               ItemDescription = p.g.First().ItemDescription,
                               ReferenceItemCode = p.g.First().ReferenceItemCode,
                               Uom = p.k.Uom,
                               Location = p.k.CurrentLocation,
                               Qty = p.g.Sum(q => q.TobeHandleQty) * p.g.First().UnitQty
                           }).ToList();
                    }
                    ViewBag.DocumentsType = matchSnRule.Code;
                }
                int seq = 10;
                foreach (var miscOrderDetail in miscOrderDetailList.OrderBy(p=>p.Item))
                {
                    miscOrderDetail.Sequence = seq;
                    seq += 10;
                }
                //int seq = miscOrderDetailList.Count > 0 ? miscOrderDetailList.Max(p => p.Sequence) : 10;
                if (v > 0)
                {
                    for (int i = 0; i < v; i++)
                    {
                        var miscOrderDetail = new MiscOrderDetail();
                        miscOrderDetail.Sequence = seq + i * 10;
                        miscOrderDetailList.Add(miscOrderDetail);
                    }
                }
                ViewBag.Location = location;
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return PartialView(new GridModel(miscOrderDetailList));
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

        #endregion

        #region  Edit&ScrapEdit
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                MiscOrderMaster miscOrderMaster = this.genericMgr.FindById<MiscOrderMaster>(id);
                ViewBag.Flow = miscOrderMaster.Flow;
                ViewBag.Location = miscOrderMaster.Location;
                if (miscOrderMaster.IsScanHu && this.CurrentUser.Code == "su" && miscOrderMaster.Status == CodeMaster.MiscOrderStatus.Create)
                {
                    ViewBag.IsShowImportHu = true;
                }
                return View(miscOrderMaster);
            }
        }

        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrder_New")]
        public ActionResult ScrapEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                MiscOrderMaster miscOrderMaster = this.genericMgr.FindById<MiscOrderMaster>(id);
                ViewBag.Flow = miscOrderMaster.Flow;
                ViewBag.Location = miscOrderMaster.Location;
                return View(miscOrderMaster);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_Edit")]
        public ActionResult EditMiscOrder(MiscOrderMaster miscOrderMaster, string sequences, string items, string qtys, string uoms)
        {
            try
            {
                var oldMiscOrderMaster = EditnewMiscOrder(miscOrderMaster, sequences, items, qtys,uoms);
                return View("Edit", oldMiscOrderMaster);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrder_New")]
        public ActionResult ScrapEditMiscOrder(MiscOrderMaster miscOrderMaster, string sequences, string items, string qtys, string uoms)
        {
            try
            {
                var errorMessageList = new List<ErrorMessage>();
                uoms = uoms.ToUpper();
                string msg = "";
                string[] ItemsArray = items.Split(',');
                string[] UomsArray = uoms.Split(',');
                var q = from p in ItemsArray
                        group p by p into g
                        select new
                        {
                            g.Key,
                            ItemCount = g.Count()
                        };
                foreach (var qd in q.Where(p => p.ItemCount > 1))
                {
                    msg = msg + qd.Key + ",";
                }
                if (msg != "")
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ItemDetailsDuplicate + msg);
                    return Json(null);
                }
                for (int i = 0; i < ItemsArray.Length; i++)
                {
                    var item = this.genericMgr.FindById<Item>(ItemsArray[i]);
                    if (item.ItemCategory != "FERT")
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_ExistNonHalfFinishGoodsType);
                        return Json(null);
                    }
                    if (item.Uom != UomsArray[i])
                    {
                        try
                        {
                            itemMgr.ConvertItemUomQty(item.Code, UomsArray[i], 1, item.Uom);
                        }
                        catch (Exception ex)
                        {
                            msg += ex.Message;
                        }

                    }
                }
                if (msg != "")
                {
                    SaveErrorMessage(msg);
                    return Json(null);
                }
                var oldMiscOrderMaster = EditnewMiscOrder(miscOrderMaster, sequences, items,qtys, uoms);
                return View("ScrapEdit", oldMiscOrderMaster);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        private MiscOrderMaster EditnewMiscOrder(MiscOrderMaster miscOrderMaster, string sequences, string items, string qtys, string uoms)
        {
            #region master 只能改备注字段
            MiscOrderMaster oldMiscOrderMaster = this.genericMgr.FindById<MiscOrderMaster>(miscOrderMaster.MiscOrderNo);
            oldMiscOrderMaster.Note = miscOrderMaster.Note;
            oldMiscOrderMaster.ReferenceNo = miscOrderMaster.ReferenceNo;
            oldMiscOrderMaster.EffectiveDate = miscOrderMaster.EffectiveDate;
            oldMiscOrderMaster.GeneralLedger = miscOrderMaster.GeneralLedger;
            if (oldMiscOrderMaster.IsScanHu)
            {
                genericMgr.Update(oldMiscOrderMaster);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedSuccessfully);
                ViewBag.Location = oldMiscOrderMaster.Location;
                return oldMiscOrderMaster;
            }
            #endregion

            #region Detail
            IList<MiscOrderDetail> oldMiscOrderDetailList = this.genericMgr.FindAll<MiscOrderDetail>
                ("select d from MiscOrderDetail as d where d.MiscOrderNo=? ", miscOrderMaster.MiscOrderNo);
            IList<MiscOrderDetail> newMiscOrderDetailList = new List<MiscOrderDetail>();
            IList<MiscOrderDetail> updateMiscOrderDetailList = new List<MiscOrderDetail>();
            IList<MiscOrderDetail> deletedMiscOrderDetails = new List<MiscOrderDetail>();

            IList<MiscOrderDetail> miscOrderDetails = new List<MiscOrderDetail>();
            string[] SequencesArray = sequences.Split(',');
            string[] ItemsArray = items.Split(',');
            string[] OrderedQtysArray = qtys.Split(',');
            string[] UomArray = uoms.Split(',');
            for (int i = 0; i < ItemsArray.Length; i++)
            {
                var item = this.genericMgr.FindById<Item>(ItemsArray[i]);
                MiscOrderDetail od = new MiscOrderDetail();
                od.Sequence = Convert.ToInt32(SequencesArray[i]);
                od.Item = item.Code;
                od.ItemDescription = item.Description;
                od.ReferenceItemCode = item.ReferenceCode;
                od.Uom = UomArray[i];
                od.BaseUom = item.Uom;
                od.UnitCount = item.UnitCount;
                od.UnitQty = 1;
                od.Qty = Convert.ToDecimal(OrderedQtysArray[i]);
                miscOrderDetails.Add(od);
            }
            var groupMiscOrderDetails = from p in miscOrderDetails
                                        group p by new
                                        {
                                            p.Item,
                                            p.ItemDescription,
                                            p.ReferenceItemCode,
                                            p.Uom,
                                            p.BaseUom,
                                            p.UnitCount,
                                        } into g
                                        select new MiscOrderDetail
                                        {
                                            Sequence = g.Max(p => p.Sequence),
                                            Item = g.Key.Item,
                                            ItemDescription = g.Key.ItemDescription,
                                            ReferenceItemCode = g.Key.ReferenceItemCode,
                                            Uom = g.Key.Uom,
                                            BaseUom = g.Key.BaseUom,
                                            UnitCount = g.Key.UnitCount,
                                            UnitQty = 1,
                                            Qty = g.Sum(p => p.Qty),
                                        };
            foreach (var detail in groupMiscOrderDetails)
            {
                var oldDetail = oldMiscOrderDetailList.FirstOrDefault(p => p.Item == detail.Item);
                if (oldDetail == null)
                {
                    newMiscOrderDetailList.Add(detail);
                }
                else if (oldDetail.Qty != detail.Qty || oldDetail.Uom != detail.Uom)
                {
                    oldDetail.Qty = detail.Qty;
                    oldDetail.Uom = detail.Uom;
                    updateMiscOrderDetailList.Add(oldDetail);
                }
            }
            foreach (var oldDetail in oldMiscOrderDetailList)
            {
                var detail = groupMiscOrderDetails.FirstOrDefault(p => p.Item == oldDetail.Item);
                if (detail == null)
                {
                    deletedMiscOrderDetails.Add(oldDetail);
                }
            }

            #endregion

            genericMgr.Update(oldMiscOrderMaster);
            miscOrderMgr.BatchUpdateMiscOrderDetails(oldMiscOrderMaster, newMiscOrderDetailList, updateMiscOrderDetailList, deletedMiscOrderDetails);
            SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedSuccessfully);
            ViewBag.Location = oldMiscOrderMaster.Location;
            return oldMiscOrderMaster;
        }
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_Confirm")]
        public ActionResult btnClose(string id)
        {
            try
            {
                MiscOrderMaster miscOrderMaster = genericMgr.FindById<MiscOrderMaster>(id);
                IList<MiscOrderDetail> miscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>
                    ("from MiscOrderDetail as m where m.MiscOrderNo=?", miscOrderMaster.MiscOrderNo);
                if (miscOrderDetailList.Count < 1)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_DetailBeEmptyCanNotExecuteConfirm);
                }
                else
                {
                    foreach (var miscOrderDetail in miscOrderDetailList)
                    {
                        CheckMiscOrderDetail(miscOrderMaster, miscOrderDetail);
                    }
                    miscOrderMgr.CloseMiscOrder(miscOrderMaster);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ConfirmSuccessfully);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);

            }
            return RedirectToAction("Edit/" + id);
        }
        [SconitAuthorize(Permissions = "Url_ProductionFiScraptOrder_ScrapConfirm")]
        public ActionResult btnCloseScrap(string id)
        {
            try
            {
                MiscOrderMaster miscOrderMaster = genericMgr.FindById<MiscOrderMaster>(id);
                IList<MiscOrderDetail> miscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>
                    ("from MiscOrderDetail as m where m.MiscOrderNo=?", miscOrderMaster.MiscOrderNo);
                if (miscOrderDetailList.Count < 1)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_DetailBeEmptyCanNotExecuteConfirm);
                }
                else
                {
                    foreach (var miscOrderDetail in miscOrderDetailList)
                    {
                        CheckMiscOrderDetail(miscOrderMaster, miscOrderDetail);
                    }
                    miscOrderMgr.CloseMiscOrder(miscOrderMaster);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ConfirmSuccessfully);
                }
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());

            }
            return RedirectToAction("ScrapEdit/" + id);
        }

        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_Edit")]
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
        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrder_New")]
        public ActionResult btnDeletescrap(string id)
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
            return RedirectToAction("ScrapNew");
        }
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_Confirm")]
        public ActionResult btnCancel(string id)
        {
            try
            {
                MiscOrderMaster MiscOrderMaster = genericMgr.FindById<MiscOrderMaster>(id);
                miscOrderMgr.CancelMiscOrder(MiscOrderMaster);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CancelledSuccessfully);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("Edit/" + id);
        }
        [SconitAuthorize(Permissions = "Url_ProductionFiScraptOrder_ScrapConfirm")]
        public ActionResult btnCancelScrap(string id)
        {
            try
            {
                MiscOrderMaster MiscOrderMaster = genericMgr.FindById<MiscOrderMaster>(id);
                miscOrderMgr.CancelMiscOrder(MiscOrderMaster);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_DeletedSuccessfully);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("ScrapEdit/" + id);
        }

        #endregion

        public void SaveToClient(string miscOrderNo)
        {
            try
            {
                MiscOrderMaster miscOrderMaster = queryMgr.FindById<MiscOrderMaster>(miscOrderNo);
                FillCodeDetailDescription<MiscOrderMaster>(miscOrderMaster);
                IList<MiscOrderDetail> miscOrderDetails = queryMgr.FindAll<MiscOrderDetail>
                    ("from MiscOrderDetail where MiscOrderNo=? ", miscOrderNo);
                PrintMiscOrderMaster printOrderMaster = Mapper.Map<MiscOrderMaster, PrintMiscOrderMaster>(miscOrderMaster);
                printOrderMaster.Title = this.genericMgr.FindAll<MiscOrderMoveType>
                    ("from MiscOrderMoveType where MoveType=? ", miscOrderMaster.MoveType).First().Description;
                printOrderMaster.Title = printOrderMaster.Title;
                IList<PrintMiscOrderDetail> printOrderDetails = Mapper.Map<IList<MiscOrderDetail>, IList<PrintMiscOrderDetail>>(miscOrderDetails);
                IList<object> data = new List<object>();
                printOrderMaster.CostCenter = string.IsNullOrWhiteSpace(printOrderMaster.CostCenter) ? "" : genericMgr.FindById<CostCenter>(printOrderMaster.CostCenter).Description+"("+printOrderMaster.CostCenter+")";
                data.Add(printOrderMaster);
                data.Add(printOrderDetails);
                reportGen.WriteToClient("MiscOrder.xls", data, printOrderMaster.MiscOrderNo + ".xls");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
        }

        public string Print(string miscOrderNo)
        {
            MiscOrderMaster miscOrderMaster = queryMgr.FindById<MiscOrderMaster>(miscOrderNo);
            FillCodeDetailDescription<MiscOrderMaster>(miscOrderMaster);
            IList<MiscOrderDetail> miscOrderDetails = queryMgr.FindAll<MiscOrderDetail>
                ("from MiscOrderDetail where MiscOrderNo=? ", miscOrderNo);
            PrintMiscOrderMaster printOrderMaster = Mapper.Map<MiscOrderMaster, PrintMiscOrderMaster>(miscOrderMaster);
            printOrderMaster.Title = this.genericMgr.FindAll<MiscOrderMoveType>
                ("from MiscOrderMoveType where MoveType=? ", miscOrderMaster.MoveType).First().Description;
            printOrderMaster.Title = printOrderMaster.Title;
            IList<PrintMiscOrderDetail> printOrderDetails = Mapper.Map<IList<MiscOrderDetail>, IList<PrintMiscOrderDetail>>(miscOrderDetails);
            IList<object> data = new List<object>();
            printOrderMaster.CostCenter = string.IsNullOrWhiteSpace(printOrderMaster.CostCenter) ? "" : genericMgr.FindById<CostCenter>(printOrderMaster.CostCenter).Description + "(" + printOrderMaster.CostCenter + ")";
            data.Add(printOrderMaster);
            data.Add(printOrderDetails);
            string reportFileUrl = reportGen.WriteToFile("MiscOrder.xls", data);
            return reportFileUrl;
        }

        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrder_New")]
        public ActionResult ScrapNew()
        {
            return View();
        }

        #endregion

        #region private method
        private bool CheckMiscOrderDetail(MiscOrderMaster miscOrderMaster, MiscOrderDetail miscOrderDetail)
        {
            if (string.IsNullOrEmpty(miscOrderDetail.Item))
            {
                throw new BusinessException(Resources.EXT.ControllerLan.Con_DetailRowItemCanNotBeEmpty);
            }
            if (miscOrderDetail.Qty == 0)
            {
                throw new BusinessException(Resources.EXT.ControllerLan.Con_DetailRowGapQuantityCanNotBeEmpty);
            }
            if (miscOrderDetail.Qty < 0)
            {
                //throw new BusinessException("明细行差异数量不能小于0的数字");
            }

            return true;
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, MiscOrderSearchModel searchModel)
        {
            string whereStatement = string.Format("where m.SubType={0} ", (int)CodeMaster.MiscOrderSubType.MES27);
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("MiscOrderNo", searchModel.MiscOrderNo, HqlStatementHelper.LikeMatchMode.Anywhere, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "m", ref  whereStatement, param);
            HqlStatementHelper.AddEqStatement("MoveType", searchModel.MoveType, "m", ref  whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "m", ref  whereStatement, param);

            HqlStatementHelper.AddLikeStatement("Note", searchModel.Note, HqlStatementHelper.LikeMatchMode.Anywhere, "m", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("Remarks3", searchModel.Remarks3, HqlStatementHelper.LikeMatchMode.Anywhere, "m", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("Remarks4", searchModel.Remarks4, HqlStatementHelper.LikeMatchMode.Anywhere, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("CreateUserName", searchModel.CreateUserName, "m", ref  whereStatement, param);
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "m", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "m", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "m", ref whereStatement, param);
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


        #endregion

        public string Import(string miscOrderNo, string huIds)
        {
            try
            {
                IList<string> huIdList = huIds.Trim()
                    .Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                miscOrderMgr.BatchUpdateMiscOrderDetails(miscOrderNo, huIdList, null);
                return Resources.EXT.ControllerLan.Con_ImportSuccessfully;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrder_Edit")]
        public ActionResult ImportMiscOrderDetail(IEnumerable<HttpPostedFileBase> attachments, string miscOrderNo)
        {
            try
            {
                foreach (var file in attachments)
                {
                    if (miscOrderNo == "NotRelyOnMiscOrdNo")
                    {
                        miscOrderMgr.Import201202MiscOrder(file.InputStream, Resources.EXT.ControllerLan.Con_BatchImportCostCenter, "201", "202", "CostCenter");
                    }
                    else
                    {
                        miscOrderMgr.CreateMiscOrderDetailFromXls(file.InputStream, miscOrderNo);
                    }
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ImportSuccessfully);
                }
            }
            catch (BusinessException ex)
            {
                SaveBusinessExceptionMessage(ex);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_ImportUnsuccessfully+" - " + ex.Message);
            }

            return Content(string.Empty);
        }
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrderDet_View,Url_CostCenterMiscOrderDet_View")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstr(MiscOrderSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            ExportToXLS<MiscOrderMaster>("Master.xls", GetAjaxPageData<MiscOrderMaster>(searchStatementModel, command).Data.ToList());
        }
        #endregion
        #region Detail search
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CostCenterMiscOrderDet_View")]
        public ActionResult _AjaxDetailList(GridCommand command, MiscOrderSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            string sql = PrepareSqlSearchStatement(searchModel);
            int total = this.genericMgr.FindAllWithNativeSql<int>("select count(*) from (" + sql + ") as r1").First();
            string sortingStatement = string.Empty;

            #region
            if (command.SortDescriptors.Count != 0)
            {
                if (command.SortDescriptors[0].Member == "CreateDate")
                {
                    command.SortDescriptors[0].Member = "CreateDate";
                }
                else if (command.SortDescriptors[0].Member == "Item")
                {
                    command.SortDescriptors[0].Member = "Item";
                }
                else if (command.SortDescriptors[0].Member == "MiscOrderNo")
                {
                    command.SortDescriptors[0].Member = "MiscOrderNo";
                }
                else if (command.SortDescriptors[0].Member == "Location")
                {
                    command.SortDescriptors[0].Member = "Location";
                }
                else if (command.SortDescriptors[0].Member == "Qty")
                {
                    command.SortDescriptors[0].Member = "Qty";
                }
                sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
                TempData["sortingStatement"] = sortingStatement;
            }
            #endregion

            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = " order by CreateDate,MiscOrderNo";
            }
            sql = string.Format("select * from (select RowId=ROW_NUMBER()OVER({0}),r1.* from ({1}) as r1 ) as rt where rt.RowId between {2} and {3}", sortingStatement, sql, (command.Page - 1) * command.PageSize + 1, command.PageSize * command.Page);
            IList<object[]> searchResult = this.genericMgr.FindAllWithNativeSql<object[]>(sql);
            IList<MiscOrderDetail> MiscOrderDetailList = new List<MiscOrderDetail>();
            if (searchResult != null && searchResult.Count > 0)
            {
                #region
                MiscOrderDetailList = (from tak in searchResult
                                       select new MiscOrderDetail
                                       {
                                           BaseUom = (string)tak[1],
                                           CreateDate = (DateTime)tak[2],
                                           CreateUserId = (int)tak[3],
                                           CreateUserName = (string)tak[4],
                                           EBELN = (string)tak[5],
                                           EBELP = (string)tak[6],
                                           Item = (string)tak[7],
                                           ItemDescription = (string)tak[8],
                                           LastModifyDate = (DateTime)tak[9],
                                           LastModifyUserId = (int)tak[10],
                                           LastModifyUserName = (string)tak[11],
                                           Location = (string)tak[12],
                                           ManufactureParty = (string)tak[13],
                                           MiscOrderNo = (string)tak[14],
                                           Qty = (decimal)tak[15],
                                           ReferenceItemCode = (string)tak[16],
                                           Remark = (string)tak[17],
                                           ReserveLine = (string)tak[18],
                                           ReserveNo = (string)tak[19],
                                           Sequence = (int)tak[20],
                                           UnitCount = (decimal)tak[21],
                                           UnitQty = (decimal)tak[22],
                                           Uom = (string)tak[23],
                                           WMSSeq = (string)tak[24],
                                           WorkHour = (decimal)tak[25],
                                           Note = (string)tak[26],
                                           EffectiveDate = (DateTime)tak[27],
                                           MoveType = (string)tak[28],
                                           Flow = (string)tak[29],
                                           WBS = (string)tak[30],
                                           CostCenter = (string)tak[31],
                                       }).ToList();
                #endregion
            }
            GridModel<MiscOrderDetail> gridModel = new GridModel<MiscOrderDetail>();
            gridModel.Total = total;
            gridModel.Data = MiscOrderDetailList;
            return PartialView(gridModel);
        }
        #endregion
        #region

        private string PrepareSqlSearchStatement(MiscOrderSearchModel searchModel)
        {
            string whereStatement = @" select d.BaseUom,d.CreateDate,d.CreateUser,d.CreateUserNm,d.EBELN,d.EBELP,d.Item,
                                   i.Desc1 As ItemDescription,d.LastModifyDate,d.LastModifyUser,d.LastModifyUserNm,isnull(d.Location,m.Location) As Location,d.ManufactureParty,d.MiscOrderNo,d.Qty
                                   ,i.RefCode As ReferenceItemCode,d.Remark,d.ReserveLine,d.ReserveNo,d.Seq,d.UC As UnitCount,d.UnitQty,d.Uom,d.WMSSeq,d.WorkHour 
                                   ,m.Note,m.effDate,m.MoveType,m.Flow,m.WBS,e.Desc1 As CostCenter from ORD_MiscOrderDet d with(nolock) 
                                   inner join ORD_MiscOrderMstr m with(nolock) on d.MiscOrderNo=m.MiscOrderNo
                                   inner join MD_Item i On d.Item =i.Code left join CUST_CostCenter e on m.CostCenter=e.Code where 1=1 ";
            if (!string.IsNullOrEmpty(searchModel.MiscOrderNo))
            {
                whereStatement += string.Format(" and m.MiscOrderNo = '{0}'", searchModel.MiscOrderNo);
            }
            if (!string.IsNullOrEmpty(searchModel.CostCenter))
            {
                whereStatement += string.Format(" and m.CostCenter = '{0}'", searchModel.CostCenter);
            }
            if (!string.IsNullOrEmpty(searchModel.CreateUserName))
            {
                whereStatement += string.Format(" and d.CreateUserNm = '{0}'", searchModel.CreateUserName);
            }
            if (!string.IsNullOrEmpty(searchModel.Flow))
            {
                whereStatement += string.Format(" and m.Flow = '{0}'", searchModel.Flow);
            }
            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                whereStatement += string.Format(" and d.Item = '{0}'", searchModel.Item);
            }

            if (!string.IsNullOrEmpty(searchModel.Note))
            {
                whereStatement += string.Format(" and m.Note = '{0}'", searchModel.Note);
            }
            if (searchModel.Location != null)
            {
                whereStatement += string.Format(" and m.Location ={0}", searchModel.Location);
            }
            if (searchModel.GeneralLedger != null)
            {
                whereStatement += string.Format(" and m.GeneralLedger ={0}", searchModel.GeneralLedger);
            }
            if (searchModel.Status != null)
            {
                whereStatement += string.Format(" and m.Status ={0}", searchModel.Status);
            }
            if (searchModel.MoveType != null)
            {
                whereStatement += string.Format(" and m.MoveType ={0}", searchModel.MoveType);
            }
            whereStatement += string.Format(" and m.SubType ={0}", (int)CodeMaster.MiscOrderSubType.MES27);
            if (searchModel.StartDate != null && searchModel.EndDate != null)
            {
                whereStatement += string.Format(" and m.CreateDate between '{0}' and '{1}' ", searchModel.StartDate, searchModel.EndDate);

            }
            else if (searchModel.StartDate != null && searchModel.EndDate == null)
            {
                whereStatement += " and m.CreateDate >='" + searchModel.StartDate + "'";
            }
            else if (searchModel.StartDate == null && searchModel.EndDate != null)
            {
                whereStatement += " and m.CreateDate <= '" + searchModel.EndDate + "'";
            }

            return whereStatement;
        }

        #endregion
        #region  Export Detail
        [SconitAuthorize(Permissions = "Url_ProductionFiScrapOrder_View,Url_CostCenterMiscOrder_View")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportDet(MiscOrderSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            string sql = PrepareSqlSearchStatement(searchModel);
            int total = this.genericMgr.FindAllWithNativeSql<int>("select count(*) from (" + sql + ") as r1").First();
            string sortingStatement = string.Empty;

            #region sort condition
            if (command.SortDescriptors.Count != 0)
            {
                if (command.SortDescriptors[0].Member == "CreateDate")
                {
                    command.SortDescriptors[0].Member = "CreateDate";
                }
                else if (command.SortDescriptors[0].Member == "Item")
                {
                    command.SortDescriptors[0].Member = "Item";
                }
                else if (command.SortDescriptors[0].Member == "MiscOrderNo")
                {
                    command.SortDescriptors[0].Member = "MiscOrderNo";
                }
                else if (command.SortDescriptors[0].Member == "Location")
                {
                    command.SortDescriptors[0].Member = "Location";
                }
                else if (command.SortDescriptors[0].Member == "Qty")
                {
                    command.SortDescriptors[0].Member = "Qty";
                }
                sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
                TempData["sortingStatement"] = sortingStatement;
            }
            #endregion

            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = " order by CreateDate,MiscOrderNo";
            }
            sql = string.Format("select * from (select RowId=ROW_NUMBER()OVER({0}),r1.* from ({1}) as r1 ) as rt where rt.RowId between {2} and {3}", sortingStatement, sql, (command.Page - 1) * command.PageSize + 1, command.PageSize * command.Page);
            IList<object[]> searchResult = this.genericMgr.FindAllWithNativeSql<object[]>(sql);
            IList<MiscOrderDetail> MiscOrderDetailList = new List<MiscOrderDetail>();
            if (searchResult != null && searchResult.Count > 0)
            {
                #region transform
                MiscOrderDetailList = (from tak in searchResult
                                       select new MiscOrderDetail
                                       {
                                           BaseUom = (string)tak[1],
                                           CreateDate = (DateTime)tak[2],
                                           CreateUserId = (int)tak[3],
                                           CreateUserName = (string)tak[4],
                                           EBELN = (string)tak[5],
                                           EBELP = (string)tak[6],
                                           Item = (string)tak[7],
                                           ItemDescription = (string)tak[8],
                                           LastModifyDate = (DateTime)tak[9],
                                           LastModifyUserId = (int)tak[10],
                                           LastModifyUserName = (string)tak[11],
                                           Location = (string)tak[12],
                                           ManufactureParty = (string)tak[13],
                                           MiscOrderNo = (string)tak[14],
                                           Qty = (decimal)tak[15],
                                           ReferenceItemCode = (string)tak[16],
                                           Remark = (string)tak[17],
                                           ReserveLine = (string)tak[18],
                                           ReserveNo = (string)tak[19],
                                           Sequence = (int)tak[20],
                                           UnitCount = (decimal)tak[21],
                                           UnitQty = (decimal)tak[22],
                                           Uom = (string)tak[23],
                                           WMSSeq = (string)tak[24],
                                           WorkHour = (decimal)tak[25],
                                           Note = (string)tak[26],
                                           EffectiveDate = (DateTime)tak[27],
                                           MoveType = (string)tak[28],
                                           Flow = (string)tak[29],
                                           WBS = (string)tak[30],
                                           CostCenter = (string)tak[31],
                                       }).ToList();
                #endregion
            }
            GridModel<MiscOrderDetail> gridModel = new GridModel<MiscOrderDetail>();
            gridModel.Total = total;
            gridModel.Data = MiscOrderDetailList;
            ExportToXLS<MiscOrderDetail>("Detail.xls", gridModel.Data.ToList());
        }
        #endregion
        public string _GetCostCenter(string flow)
        {

            var flowObj = this.genericMgr.FindById<FlowMaster>(flow);
            var costStr = this.genericMgr.FindById<CostCenter>(flowObj.CostCenter);
            return costStr.CodeDescription;
        }
    }
}

