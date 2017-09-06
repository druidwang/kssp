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
using com.Sconit.Entity.INV;
using System.Text;
using com.Sconit.Entity.SCM;
using System;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Web.Controllers.ORD
{
    public class ProductionReworkMiscOrderController : WebAppBaseController
    {
        private static string selectCountStatement = "select count(*) from MiscOrderMaster as m";

        private static string selectStatement = "select m from MiscOrderMaster as m";

        //public IGenericMgr genericMgr { get; set; }
        public IMiscOrderMgr miscOrderMgr { get; set; }

        #region public method

        #region view
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_View")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrderDet_View")]
        public ActionResult DetailIndex()
        {
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_View")]
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
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(GridCommand.PageSize);
            return View();

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_View")]
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
            ViewBag.PageSize = base.ProcessPageSize(GridCommand.PageSize);
            return View();

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_View")]
        public ActionResult _AjaxList(GridCommand command, MiscOrderSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_New")]
        public ActionResult New()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_New")]
        public ActionResult CreateMiscOrder(MiscOrderMaster miscOrderMaster, string[] sequences, string[] items, string[] qtys, string[] locations)
        {
            try
            {
                if (items == null || items.Length == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_DetailBeEmptyCanNotCreate);
                }
                MiscOrderMoveType miscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>(
                    "from MiscOrderMoveType as m where m.MoveType=? and m.SubType=? ",
                    new object[] { miscOrderMaster.MoveType, com.Sconit.CodeMaster.MiscOrderSubType.SY04 })[0];

                miscOrderMaster.Type = miscOrderMoveType.IOType;
                miscOrderMaster.SubType = miscOrderMoveType.SubType;
                miscOrderMaster.MoveType = miscOrderMoveType.MoveType;
                miscOrderMaster.CancelMoveType = miscOrderMoveType.CancelMoveType;
                miscOrderMaster.Status = CodeMaster.MiscOrderStatus.Create;
                miscOrderMaster.QualityType = miscOrderMaster.QualityType;
                var flow = this.genericMgr.FindById<FlowMaster>(miscOrderMaster.Flow);
                miscOrderMaster.Region = flow.PartyTo;
                miscOrderMaster.Location = flow.LocationTo;

                //if (miscOrderMaster.MoveType == "261" || miscOrderMaster.MoveType == "262")
                //{
                //    miscOrderMaster.Region = flow.PartyFrom;
                //    miscOrderMaster.Location = flow.LocationFrom;
                //}
                //else
                //{
                //    miscOrderMaster.Region = flow.PartyTo;
                //    miscOrderMaster.Location = flow.LocationTo;
                //}

                IList<MiscOrderDetail> miscOrderDetails = new List<MiscOrderDetail>();
                for (int i = 0; i < items.Length; i++)
                {
                    var item = this.genericMgr.FindById<Item>(items[i]);
                    MiscOrderDetail od = new MiscOrderDetail();
                    od.Sequence = Convert.ToInt32(sequences[i]);
                    od.Item = item.Code;
                    od.ItemDescription = item.Description;
                    od.ReferenceItemCode = item.ReferenceCode;
                    od.Uom = item.Uom;
                    od.BaseUom = item.Uom;
                    od.UnitCount = item.UnitCount;
                    od.UnitQty = 1;
                    od.Qty = Convert.ToDecimal(qtys[i]);
                    if (!string.IsNullOrWhiteSpace(locations[i]))
                    {
                        od.Location = locations[i];
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
                                                    }).ToList();

                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CreateSuccessfully);
                miscOrderMgr.CreateMiscOrder(miscOrderMaster);
                ViewBag.miscOrderNo = miscOrderMaster.MiscOrderNo;
                ViewBag.Flow = miscOrderMaster.Flow;
                return View("Edit", miscOrderMaster);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        #endregion

        #region MiscOrderDetail
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_New")]
        public ActionResult _MiscOrderDetail(string miscOrderNo, string flow)
        {
            if (!string.IsNullOrEmpty(miscOrderNo))
            {
                MiscOrderMaster miscOrder = genericMgr.FindById<MiscOrderMaster>(miscOrderNo);
                ViewBag.Status = miscOrder.Status;
                ViewBag.IsCreate = miscOrder.Status == com.Sconit.CodeMaster.MiscOrderStatus.Create ? true : false;
            }
            else
            {
                ViewBag.IsCreate = true;
            }
            ViewBag.miscOrderNo = miscOrderNo;
            ViewBag.Flow = flow;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_New")]
        public ActionResult _SelectMiscOrderDetail(string miscOrderNo, string flow)
        {
            IList<MiscOrderDetail> miscOrderDetailList = new List<MiscOrderDetail>();
            var multiRows = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.GridDefaultMultiRowsCount);
            int v;
            int.TryParse(multiRows, out v);

            if (!string.IsNullOrEmpty(miscOrderNo))
            {
                v = -1;
                miscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>("from MiscOrderDetail as m where m.MiscOrderNo=? ", miscOrderNo);
            }
            int seq = miscOrderDetailList.Count > 0 ? miscOrderDetailList.Max(p => p.Sequence) : 10;
            if (v > 0)
            {
                for (int i = 0; i < v; i++)
                {
                    var order = new MiscOrderDetail();
                    order.Sequence = seq + i * 10;
                    miscOrderDetailList.Add(order);
                }
            }
            ViewBag.Flow = flow;
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

        #region  Edit
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_Edit")]
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
                return View(miscOrderMaster);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_Edit")]
        public ActionResult EditMiscOrder(MiscOrderMaster miscOrderMaster, string[] sequences, string[] items, string[] qtys, string[] locations)
        {
            try
            {
                #region master 只能改备注字段
                MiscOrderMaster oldMiscOrderMaster = this.genericMgr.FindById<MiscOrderMaster>(miscOrderMaster.MiscOrderNo);
                oldMiscOrderMaster.Note = miscOrderMaster.Note;
                oldMiscOrderMaster.QualityType = miscOrderMaster.QualityType;
                #endregion

                #region Detail
                IList<MiscOrderDetail> oldMiscOrderDetailList = this.genericMgr.FindAll<MiscOrderDetail>
                    ("select d from MiscOrderDetail as d where d.MiscOrderNo=? ", miscOrderMaster.MiscOrderNo);
                IList<MiscOrderDetail> newMiscOrderDetailList = new List<MiscOrderDetail>();
                IList<MiscOrderDetail> updateMiscOrderDetailList = new List<MiscOrderDetail>();
                IList<MiscOrderDetail> deletedMiscOrderDetails = new List<MiscOrderDetail>();

                IList<MiscOrderDetail> miscOrderDetails = new List<MiscOrderDetail>();
                for (int i = 0; i < items.Length; i++)
                {
                    var item = this.genericMgr.FindById<Item>(items[i]);
                    MiscOrderDetail od = new MiscOrderDetail();
                    od.Sequence = Convert.ToInt32(sequences[i]);
                    od.Item = item.Code;
                    od.ItemDescription = item.Description;
                    od.ReferenceItemCode = item.ReferenceCode;
                    od.Uom = item.Uom;
                    od.BaseUom = item.Uom;
                    od.UnitCount = item.UnitCount;
                    od.UnitQty = 1;
                    od.Qty = Convert.ToDecimal(qtys[i]);
                    if (!string.IsNullOrWhiteSpace(locations[i]))
                    {
                        od.Location = locations[i];
                    }
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
                    else if (oldDetail.Qty != detail.Qty)
                    {
                        oldDetail.Qty = detail.Qty;
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

                ViewBag.Flow = oldMiscOrderMaster.Flow;
                return View("Edit", oldMiscOrderMaster);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_Edit")]
        public ActionResult btnClose(string id)
        {
            try
            {
                MiscOrderMaster miscOrderMaster = genericMgr.FindById<MiscOrderMaster>(id);
                IList<MiscOrderDetail> miscOrderDetailList = genericMgr.FindAll<MiscOrderDetail>
                    ("from MiscOrderDetail as m where m.MiscOrderNo=? ", miscOrderMaster.MiscOrderNo);
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
            return RedirectToAction("Edit/" + id);
        }


        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_Edit")]
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

        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_Edit")]
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

        #endregion
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
            string whereStatement = string.Format("where m.SubType={0} ", (int)CodeMaster.MiscOrderSubType.SY04);
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("MiscOrderNo", searchModel.MiscOrderNo, HqlStatementHelper.LikeMatchMode.Anywhere, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "m", ref  whereStatement, param);
            HqlStatementHelper.AddEqStatement("MoveType", searchModel.MoveType, "m", ref  whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "m", ref  whereStatement, param);

            HqlStatementHelper.AddLikeStatement("Note", searchModel.Note, HqlStatementHelper.LikeMatchMode.Anywhere, "m", ref whereStatement, param);
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

        public string _GetMiscOrderLocation(string moveType, string flow)
        {
            try
            {
                FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(flow);
                ViewBag.Flow = flow;
                if (false)// moveType == "261" || moveType == "262")
                {
                    var location = this.genericMgr.FindById<Location>(flowMaster.LocationFrom);
                    return location.CodeName;
                }
                else
                {
                    var location = this.genericMgr.FindById<Location>(flowMaster.LocationTo);
                    return location.CodeName;
                }
            }
            catch (Exception ex)
            {
                //SaveErrorMessage("");
                return string.Empty;
            }
        }

        #endregion
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrder_View")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstr(MiscOrderSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            ExportToXLS<MiscOrderMaster>("ProductionReworkOrderMaster.xls", GetAjaxPageData<MiscOrderMaster>(searchStatementModel, command).Data.ToList());
        }
        #endregion
        #region Detail search
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrderDet_View")]
        public ActionResult _AjaxDetailList(GridCommand command, MiscOrderSearchModel searchModel)
        {
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
                                   i.Desc1 As ItemDescription,d.LastModifyDate,d.LastModifyUser,d.LastModifyUserNm,m.Location,d.ManufactureParty,d.MiscOrderNo,d.Qty
                                   ,i.RefCode As ReferenceItemCode,d.Remark,d.ReserveLine,d.ReserveNo,d.Seq,d.UC As UnitCount,d.UnitQty,d.Uom,d.WMSSeq,d.WorkHour 
                                   ,m.Note,m.effDate,m.MoveType,m.Flow,m.WBS from ORD_MiscOrderDet d with(nolock) 
                                   inner join ORD_MiscOrderMstr m with(nolock) on d.MiscOrderNo=m.MiscOrderNo
                                   inner join MD_Item i On d.Item =i.Code  where 1=1 ";
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

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                whereStatement += string.Format(" and d.Item = '{0}'", searchModel.Item);
            }
            if (!string.IsNullOrEmpty(searchModel.Flow))
            {
                whereStatement += string.Format(" and m.Flow = '{0}'", searchModel.Flow);
            }
            if (!string.IsNullOrEmpty(searchModel.Note))
            {
                whereStatement += string.Format(" and m.Note = '{0}'", searchModel.Note);
            }
            if (searchModel.Location != null)
            {
                whereStatement += string.Format(" and m.Location ={0}", searchModel.Location);
            }
            if (searchModel.MoveType != null)
            {
                whereStatement += string.Format(" and m.MoveType ={0}", searchModel.MoveType);
            }
            if (searchModel.GeneralLedger != null)
            {
                whereStatement += string.Format(" and m.GeneralLedger ={0}", searchModel.GeneralLedger);
            }
            if (searchModel.Status != null)
            {
                whereStatement += string.Format(" and m.Status ={0}", searchModel.Status);
            }

            whereStatement += string.Format(" and m.SubType ={0}",(int)CodeMaster.MiscOrderSubType.SY04);
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
        [SconitAuthorize(Permissions = "Url_ProductionReworkMiscOrderDet_View")]
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
                                       }).ToList();
                #endregion
            }
            GridModel<MiscOrderDetail> gridModel = new GridModel<MiscOrderDetail>();
            gridModel.Total = total;
            gridModel.Data = MiscOrderDetailList;
            ExportToXLS<MiscOrderDetail>("ProductionReworkOrderDet.xls", gridModel.Data.ToList());
        }
        #endregion
    }
}

