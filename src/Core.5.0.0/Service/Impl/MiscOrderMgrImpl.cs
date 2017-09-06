using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.VIEW;
using System.IO;
using NHibernate.Type;
using NHibernate;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using com.Sconit.Utility;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.INP;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class MiscOrderMgrImpl : BaseMgr, IMiscOrderMgr
    {
        #region 变量
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        #endregion

        private static string selectInspectResult = @"select r from InspectResult r where r.InspectNo = ? and r.RejectHandleResult=? 
            and r.IsHandle=? and r.JudgeQty > r.HandleQty";

        private static string selectRejectDetail = @"select r from RejectDetail as r where r.RejectNo=? and r.HandleQty > r.HandledQty
            and exists (select 1 from RejectMaster as m where r.RejectNo = m.RejectNo and m.HandleResult =? )";


        [Transaction(TransactionMode.Requires)]
        public void CreateMiscOrder(MiscOrderMaster miscOrderMaster)
        {
            miscOrderMaster.MiscOrderNo = this.numberControlMgr.GetMiscOrderNo(miscOrderMaster);
            if (string.IsNullOrWhiteSpace(miscOrderMaster.DeliverRegion))
            {
                //非跨工厂移库时，会从Region字段找到对应的Plant填到该字段上面
                miscOrderMaster.DeliverRegion = this.genericMgr.FindById<Region>(miscOrderMaster.Region).Plant;
            }
            this.genericMgr.Create(miscOrderMaster);

            if (miscOrderMaster.MiscOrderDetails != null && miscOrderMaster.MiscOrderDetails.Count > 0)
            {
                BatchUpdateMiscOrderDetails(miscOrderMaster, miscOrderMaster.MiscOrderDetails, null, null);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void QuickCreateMiscOrder(MiscOrderMaster miscOrderMaster, DateTime effectiveDate)
        {
            miscOrderMaster.MiscOrderNo = this.numberControlMgr.GetMiscOrderNo(miscOrderMaster);
            this.genericMgr.Create(miscOrderMaster);



            #region 更新引用的明细状态
            if (miscOrderMaster.ReferenceDocumentsType > 0)
            {
                if (miscOrderMaster.QualityType != CodeMaster.QualityType.Reject)
                {
                    throw new BusinessException("不合格品报废请选择质量类型为不合格");
                }
                if (miscOrderMaster.MoveType != "201")
                {
                    throw new BusinessException("不合格品报废的移动类型应该为201");
                }

                List<string> huList = new List<string>();
                if (miscOrderMaster.ReferenceDocumentsType == (int)CodeMaster.DocumentsType.INS)
                {
                    var inspectResultList = genericMgr.FindAll<InspectResult>(selectInspectResult,
                    new object[] { miscOrderMaster.ReferenceNo, com.Sconit.CodeMaster.HandleResult.Scrap, false });
                    foreach (var inspectResult in inspectResultList)
                    {
                        inspectResult.HandleQty = inspectResult.JudgeQty;
                        inspectResult.IsHandle = true;
                        genericMgr.Update(inspectResult);
                    }
                    huList = inspectResultList.Select(p => p.HuId).Distinct().ToList();
                }
                else if (miscOrderMaster.ReferenceDocumentsType == (int)CodeMaster.DocumentsType.REJ)
                {
                    var rejectDetailList = genericMgr.FindAll<RejectDetail>(selectRejectDetail,
                        new object[] { miscOrderMaster.ReferenceNo, com.Sconit.CodeMaster.HandleResult.Scrap });
                    foreach (var rejectDetail in rejectDetailList)
                    {
                        rejectDetail.HandledQty = rejectDetail.HandleQty;
                        genericMgr.Update(rejectDetail);
                    }
                    RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(miscOrderMaster.ReferenceNo);
                    rejectMaster.Status = CodeMaster.RejectStatus.Close;
                    genericMgr.Update(rejectMaster);

                    huList = rejectDetailList.Select(p => p.HuId).Distinct().ToList();
                }
                if (miscOrderMaster.IsScanHu)
                {
                    miscOrderMaster.MiscOrderDetails = new List<MiscOrderDetail>();
                    this.BatchUpdateMiscOrderDetails(miscOrderMaster, huList, null);
                }
            }
            #endregion
            this.CreateMiscOrderDetail(miscOrderMaster.MiscOrderDetails, miscOrderMaster.MiscOrderNo);
            this.CloseMiscOrder(miscOrderMaster, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateMiscOrder(MiscOrderMaster miscOrderMaster)
        {
            if (miscOrderMaster.Status != CodeMaster.MiscOrderStatus.Create)
            {
                throw new BusinessException("计划外出入库单{0}的状态为{1}不能修改。",
                      miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
            }
            this.genericMgr.Update(miscOrderMaster);
        }


        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateMiscLocationOrderDetails(string miscOrderNo,
            IList<MiscOrderDetail> addMiscOrderDetailList, IList<MiscOrderDetail> updateMiscOrderDetailList, IList<MiscOrderLocationDetail> deleteMiscOrderLocationDetailList)
        {
            BatchUpdateMiscLocationOrderDetails(this.genericMgr.FindById<MiscOrderMaster>(miscOrderNo), addMiscOrderDetailList, updateMiscOrderDetailList, deleteMiscOrderLocationDetailList);
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateMiscLocationOrderDetails(MiscOrderMaster miscOrderMaster,
            IList<MiscOrderDetail> addMiscOrderDetailList, IList<MiscOrderDetail> updateMiscOrderDetailList, IList<MiscOrderLocationDetail> deleteMiscOrderLocationDetailList)
        {
            if (miscOrderMaster.Status != CodeMaster.MiscOrderStatus.Create)
            {
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    throw new BusinessException("计划外出库单{0}的状态为{1}不能修改明细。",
                          miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
                else
                {
                    throw new BusinessException("计划外入库单{0}的状态为{1}不能修改明细。",
                        miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
            }

            #region 新增计划外出入库明细
            if (addMiscOrderDetailList != null && addMiscOrderDetailList.Count > 0)
            {
                #region 获取最大订单明细序号
                string hql = "select max(Sequence) as seq from MiscOrderDetail where MiscOrderNo = ?";
                IList<object> maxSeqList = genericMgr.FindAll<object>(hql, miscOrderMaster.MiscOrderNo);
                int maxSeq = maxSeqList[0] != null ? (int)(maxSeqList[0]) : 0;
                #endregion

                #region 数量处理
                foreach (MiscOrderDetail miscOrderDetail in addMiscOrderDetailList)
                {
                    //Item item = this.genericMgr.FindById<Item>(miscOrderDetail.Item);

                    miscOrderDetail.MiscOrderNo = miscOrderMaster.MiscOrderNo;
                    miscOrderDetail.Sequence = ++maxSeq;
                    //miscOrderDetail.Item = miscOrderDetail.Item;
                    //miscOrderDetail.ItemDescription = item.Description;
                    //miscOrderDetail.ReferenceItemCode = item.ReferenceCode;
                    //miscOrderDetail.Uom = miscOrderDetail.Uom;
                    //miscOrderDetail.BaseUom = item.Uom;
                    //miscOrderDetail.UnitCount = miscOrderDetail.UnitCount;
                    if (miscOrderDetail.Uom != miscOrderDetail.BaseUom)
                    {
                        miscOrderDetail.UnitQty = this.itemMgr.ConvertItemUomQty(miscOrderDetail.Item, miscOrderDetail.BaseUom, 1, miscOrderDetail.Uom);
                    }
                    else
                    {
                        miscOrderDetail.UnitQty = 1;
                    }
                    //miscOrderDetail.ReserveNo = miscOrderDetail.ReserveNo;
                    //miscOrderDetail.ReserveLine = miscOrderDetail.ReserveLine;
                    //miscOrderDetail.Qty = miscOrderDetail.Qty;

                    this.genericMgr.Create(miscOrderDetail);

                    if (miscOrderMaster.MiscOrderDetails == null)
                    {
                        miscOrderMaster.MiscOrderDetails = new List<MiscOrderDetail>();
                    }
                    //miscOrderMaster.MiscOrderDetails.Add(miscOrderDetail);
                }
                #endregion
            }
            #endregion

            #region 修改计划外出入库明细
            if (updateMiscOrderDetailList != null && updateMiscOrderDetailList.Count > 0)
            {
                foreach (MiscOrderDetail miscOrderDetail in updateMiscOrderDetailList)
                {
                    if (miscOrderDetail.Uom != miscOrderDetail.BaseUom)
                    {
                        miscOrderDetail.UnitQty = this.itemMgr.ConvertItemUomQty(miscOrderDetail.Item, miscOrderDetail.BaseUom, 1, miscOrderDetail.Uom);
                    }
                    else
                    {
                        miscOrderDetail.UnitQty = 1;
                    }
                    this.genericMgr.Update(miscOrderDetail);
                }
            }
            #endregion

            #region 删除计划外出入库明细
            //删除OrderDet
            IList<string> para = new List<string>();
            para.Add(deleteMiscOrderLocationDetailList.FirstOrDefault().MiscOrderNo);
            //para.Add(miscOrderLocationDetail.Item);
            IList<MiscOrderLocationDetail> miscOrderLocationDetails = genericMgr.FindAll<MiscOrderLocationDetail>(@"
                                                                             from MiscOrderLocationDetail as m where m.MiscOrderNo = ? ", para);
            var deleteItemCount = from p in deleteMiscOrderLocationDetailList
                                  group p by p.Item into result
                                  select new
                                  {
                                      Item = result.Key,
                                      OrderdetId = result.FirstOrDefault().MiscOrderDetailId,
                                      Count = result.Count()
                                  };
            var miscOrderDetailCount = from p in miscOrderLocationDetails
                                       group p by p.Item into result
                                       select new
                                       {
                                           Item = result.Key,
                                           OrderdetId = result.FirstOrDefault().MiscOrderDetailId,
                                           Count = result.Count()
                                       };
            var needToDeleteItems = from cc in deleteItemCount
                                    from cd in miscOrderDetailCount
                                    where cc.Item == cd.Item && cc.Count == cd.Count
                                    select new
                                    {
                                        cc.Item,
                                        cc.OrderdetId
                                    };


            if (deleteMiscOrderLocationDetailList != null && deleteMiscOrderLocationDetailList.Count > 0)
            {
                #region 数量处理
                foreach (MiscOrderLocationDetail miscOrderLocationDetail in deleteMiscOrderLocationDetailList)
                {
                    //删除locationdet
                    this.genericMgr.DeleteById<MiscOrderLocationDetail>(miscOrderLocationDetail.Id);

                    //this.genericMgr.Delete("from MiscOrderLocationDetail as l where l.MiscOrderDetailId=" + miscOrderDetail.Id);
                }
                #endregion

            }
            if ((needToDeleteItems != null && needToDeleteItems.ToList().Count() > 0))
            {
                foreach (var needToDeleteItem in needToDeleteItems.Select(p => new { p.Item, p.OrderdetId }).Distinct().ToList())
                {
                    this.genericMgr.DeleteById<MiscOrderDetail>(needToDeleteItem.OrderdetId);
                }
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateMiscOrderDetails(string miscOrderNo,
            IList<MiscOrderDetail> addMiscOrderDetailList, IList<MiscOrderDetail> updateMiscOrderDetailList, IList<MiscOrderDetail> deleteMiscOrderDetailList)
        {
            BatchUpdateMiscOrderDetails(this.genericMgr.FindById<MiscOrderMaster>(miscOrderNo), addMiscOrderDetailList, updateMiscOrderDetailList, deleteMiscOrderDetailList);
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateMiscOrderDetails(MiscOrderMaster miscOrderMaster,
            IList<MiscOrderDetail> addMiscOrderDetailList, IList<MiscOrderDetail> updateMiscOrderDetailList, IList<MiscOrderDetail> deleteMiscOrderDetailList)
        {
            if (miscOrderMaster.Status != CodeMaster.MiscOrderStatus.Create)
            {
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    throw new BusinessException("计划外出库单{0}的状态为{1}不能修改明细。",
                          miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
                else
                {
                    throw new BusinessException("计划外入库单{0}的状态为{1}不能修改明细。",
                        miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
            }

            #region 新增计划外出入库明细
            if (addMiscOrderDetailList != null && addMiscOrderDetailList.Count > 0)
            {
                #region 获取最大订单明细序号
                string hql = "select max(Sequence) as seq from MiscOrderDetail where MiscOrderNo = ?";
                IList<object> maxSeqList = genericMgr.FindAll<object>(hql, miscOrderMaster.MiscOrderNo);
                int maxSeq = maxSeqList[0] != null ? (int)(maxSeqList[0]) : 0;
                #endregion

                #region 数量处理
                foreach (MiscOrderDetail miscOrderDetail in addMiscOrderDetailList)
                {
                    //Item item = this.genericMgr.FindById<Item>(miscOrderDetail.Item);

                    miscOrderDetail.MiscOrderNo = miscOrderMaster.MiscOrderNo.ToUpper();
                    miscOrderDetail.Sequence = ++maxSeq;
                    //miscOrderDetail.Item = miscOrderDetail.Item;
                    //miscOrderDetail.ItemDescription = item.Description;
                    //miscOrderDetail.ReferenceItemCode = item.ReferenceCode;
                    //miscOrderDetail.Uom = miscOrderDetail.Uom;
                    //miscOrderDetail.BaseUom = item.Uom;
                    //miscOrderDetail.UnitCount = miscOrderDetail.UnitCount;
                    if (miscOrderDetail.Uom != miscOrderDetail.BaseUom)
                    {
                        miscOrderDetail.UnitQty = this.itemMgr.ConvertItemUomQty(miscOrderDetail.Item, miscOrderDetail.BaseUom, 1, miscOrderDetail.Uom);
                    }
                    else
                    {
                        miscOrderDetail.UnitQty = 1;
                    }
                    //miscOrderDetail.ReserveNo = miscOrderDetail.ReserveNo;
                    //miscOrderDetail.ReserveLine = miscOrderDetail.ReserveLine;
                    //miscOrderDetail.Qty = miscOrderDetail.Qty;

                    this.genericMgr.Create(miscOrderDetail);

                    if (miscOrderMaster.MiscOrderDetails == null)
                    {
                        miscOrderMaster.MiscOrderDetails = new List<MiscOrderDetail>();
                    }
                    //miscOrderMaster.MiscOrderDetails.Add(miscOrderDetail);
                }
                #endregion
            }
            #endregion

            #region 修改计划外出入库明细
            if (updateMiscOrderDetailList != null && updateMiscOrderDetailList.Count > 0)
            {
                foreach (MiscOrderDetail miscOrderDetail in updateMiscOrderDetailList)
                {
                    if (miscOrderDetail.Uom != miscOrderDetail.BaseUom)
                    {
                        miscOrderDetail.UnitQty = this.itemMgr.ConvertItemUomQty(miscOrderDetail.Item, miscOrderDetail.BaseUom, 1, miscOrderDetail.Uom);
                    }
                    else
                    {
                        miscOrderDetail.UnitQty = 1;
                    }
                    this.genericMgr.Update(miscOrderDetail);
                }
            }
            #endregion

            #region 删除计划外出入库明细
            if (deleteMiscOrderDetailList != null && deleteMiscOrderDetailList.Count > 0)
            {
                #region 数量处理
                foreach (MiscOrderDetail miscOrderDetail in deleteMiscOrderDetailList)
                {
                    //删除locationdet
                    this.genericMgr.Delete("from MiscOrderLocationDetail as l where l.MiscOrderDetailId=" + miscOrderDetail.Id);
                    this.genericMgr.Delete(miscOrderDetail);
                }
                #endregion
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateMiscOrderDetails(string miscOrderNo, IList<string> addHuIdList, IList<string> deleteHuIdList)
        {
            BatchUpdateMiscOrderDetails(this.genericMgr.FindById<MiscOrderMaster>(miscOrderNo), addHuIdList, deleteHuIdList);
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateMiscOrderDetails(MiscOrderMaster miscOrderMaster, IList<string> addHuIdList, IList<string> deleteHuIdList)
        {
            if (miscOrderMaster.Status != CodeMaster.MiscOrderStatus.Create)
            {
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    throw new BusinessException("计划外出库单{0}的状态为{1}不能修改明细。",
                          miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
                else
                {
                    throw new BusinessException("计划外入库单{0}的状态为{1}不能修改明细。",
                        miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
            }

            TryLoadMiscOrderDetails(miscOrderMaster);
            IList<MiscOrderLocationDetail> miscOrderLocationDetailList = TryLoadMiscOrderLocationDetails(miscOrderMaster);

            #region 新增计划外出入库明细
            if (addHuIdList != null && addHuIdList.Count > 0)
            {
                #region 获取最大订单明细序号
                string hql = "select max(Sequence) as seq from MiscOrderDetail where MiscOrderNo = ?";
                IList maxSeqList = genericMgr.FindAll(hql, miscOrderMaster.MiscOrderNo);
                int maxSeq = maxSeqList != null && maxSeqList.Count > 0 && maxSeqList[0] != null ? (int)maxSeqList[0] : 0;
                #endregion

                #region 条码处理
                #region 明细重复输入校验
                #region 合并新增的HuId和原有的HuId
                IList<string> huIdList = new List<string>();
                ((List<string>)huIdList).AddRange(addHuIdList);
                if (miscOrderLocationDetailList != null && miscOrderLocationDetailList.Count > 0)
                {
                    ((List<string>)huIdList).AddRange(miscOrderLocationDetailList.Select(det => det.HuId).ToList());
                }
                #endregion

                #region 检查是否重复
                BusinessException businessException = new BusinessException();
                var groupedHuIds = from huId in huIdList
                                   group huId by huId into result
                                   select new
                                   {
                                       HuId = result.Key,
                                       Count = result.Count()
                                   };

                foreach (var groupedHuId in groupedHuIds.Where(g => g.Count > 1))
                {
                    businessException = new BusinessException(string.Format("重复扫描条码{0}。", groupedHuId.HuId));
                }

                if (businessException.HasMessage)
                {
                    throw businessException;
                }
                #endregion
                #endregion

                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    #region 计划外出库
                    #region 库存占用
                    IList<InventoryOccupy> inventoryOccupyList = (from huId in addHuIdList
                                                                  select new InventoryOccupy
                                                                  {
                                                                      HuId = huId,
                                                                      //Location = miscOrderMaster.Location,  不指定库位
                                                                      QualityType = miscOrderMaster.QualityType,
                                                                      OccupyType = CodeMaster.OccupyType.MiscOrder,
                                                                      OccupyReferenceNo = miscOrderMaster.MiscOrderNo
                                                                  }).ToList();

                    IList<LocationLotDetail> locationLotDetailList = this.locationDetailMgr.InventoryOccupy(inventoryOccupyList);
                    #endregion

                    #region 新增明细
                    foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
                    {
                        MiscOrderDetail matchedMiscOrderDetail = null;

                        #region 明细处理
                        if (miscOrderMaster.MiscOrderDetails != null && miscOrderMaster.MiscOrderDetails.Count > 0)
                        {
                            //查找匹配的明细行
                            matchedMiscOrderDetail = miscOrderMaster.MiscOrderDetails.Where(det => det.Item == locationLotDetail.Item
                                                                                                && det.Uom == locationLotDetail.HuUom
                                                                                                && det.UnitCount == locationLotDetail.UnitCount
                                                                                                && det.Location == locationLotDetail.Location).SingleOrDefault();
                        }

                        if (matchedMiscOrderDetail == null)
                        {
                            //没有找到明细行，新增明细
                            Item item = this.genericMgr.FindById<Item>(locationLotDetail.Item);                                //没有找到匹配的明细行，新增一行
                            matchedMiscOrderDetail = new MiscOrderDetail();

                            matchedMiscOrderDetail.MiscOrderNo = miscOrderMaster.MiscOrderNo;
                            matchedMiscOrderDetail.Sequence = ++maxSeq;
                            matchedMiscOrderDetail.Item = locationLotDetail.Item;
                            matchedMiscOrderDetail.ItemDescription = item.Description;
                            matchedMiscOrderDetail.ReferenceItemCode = item.ReferenceCode;
                            matchedMiscOrderDetail.Uom = locationLotDetail.HuUom;
                            matchedMiscOrderDetail.BaseUom = locationLotDetail.BaseUom;
                            matchedMiscOrderDetail.UnitCount = locationLotDetail.UnitCount;
                            matchedMiscOrderDetail.UnitQty = locationLotDetail.UnitQty;
                            matchedMiscOrderDetail.Location = locationLotDetail.Location;
                            //matchedMiscOrderDetail.ReserveNo = addMiscOrderDetail.ReserveNo;
                            //matchedMiscOrderDetail.ReserveLine = addMiscOrderDetail.ReserveLine;
                            matchedMiscOrderDetail.Qty = locationLotDetail.Qty;

                            this.genericMgr.Create(matchedMiscOrderDetail);

                            miscOrderMaster.MiscOrderDetails.Add(matchedMiscOrderDetail);
                        }
                        else
                        {
                            //找到明细行，更新数量
                            matchedMiscOrderDetail.Qty += locationLotDetail.Qty;
                            this.genericMgr.Update(matchedMiscOrderDetail);
                        }
                        #endregion

                        #region 库存明细新增
                        MiscOrderLocationDetail miscOrderLocationDetail = new MiscOrderLocationDetail();

                        miscOrderLocationDetail.MiscOrderNo = miscOrderMaster.MiscOrderNo;
                        miscOrderLocationDetail.MiscOrderDetailId = matchedMiscOrderDetail.Id;
                        miscOrderLocationDetail.MiscOrderDetailSequence = matchedMiscOrderDetail.Sequence;
                        miscOrderLocationDetail.Item = locationLotDetail.Item;
                        miscOrderLocationDetail.Uom = locationLotDetail.HuUom;
                        miscOrderLocationDetail.HuId = locationLotDetail.HuId;
                        miscOrderLocationDetail.LotNo = locationLotDetail.LotNo;
                        miscOrderLocationDetail.IsCreatePlanBill = false;
                        miscOrderLocationDetail.IsConsignment = locationLotDetail.IsConsignment;
                        miscOrderLocationDetail.PlanBill = locationLotDetail.PlanBill;
                        #region 查找寄售供应商
                        if (locationLotDetail.IsConsignment && locationLotDetail.PlanBill.HasValue)
                        {
                            miscOrderLocationDetail.ConsignmentSupplier = this.genericMgr.FindAll<string>("select Party from PlanBill where Id = ?", locationLotDetail.PlanBill.Value)[0];
                        }
                        #endregion
                        miscOrderLocationDetail.ActingBill = null;
                        miscOrderLocationDetail.QualityType = locationLotDetail.QualityType;
                        miscOrderLocationDetail.IsFreeze = locationLotDetail.IsFreeze;
                        miscOrderLocationDetail.IsATP = locationLotDetail.IsATP;
                        miscOrderLocationDetail.OccupyType = locationLotDetail.OccupyType;
                        miscOrderLocationDetail.OccupyReferenceNo = locationLotDetail.OccupyReferenceNo;
                        miscOrderLocationDetail.Qty = locationLotDetail.Qty;
                        if (addHuIdList.Contains(miscOrderLocationDetail.HuId))
                        {
                            this.genericMgr.Create(miscOrderLocationDetail);
                        }
                        #endregion
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    #region 计划外入库
                    #region 检查条码状态
                    IList<HuStatus> huStatusList = this.huMgr.GetHuStatus(addHuIdList);

                    foreach (string huId in addHuIdList)
                    {
                        HuStatus huStatus = huStatusList.Where(h => h.HuId == huId).SingleOrDefault();
                        if (huStatus == null)
                        {
                            businessException = new BusinessException(string.Format("条码{0}不存在。", huId));
                        }
                        else if (huStatus.Status == CodeMaster.HuStatus.Location)
                        {
                            businessException = new BusinessException(string.Format("条码{0}在库位{1}中，不能计划外入库。", huStatus.HuId, huStatus.Location));
                        }
                        else if (huStatus.Status == CodeMaster.HuStatus.Ip)
                        {
                            businessException = new BusinessException(string.Format("条码{0}为库位{1}至库位{2}的在途库存，不能计划外入库。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo));
                        }
                    }

                    if (businessException.HasMessage)
                    {
                        throw businessException;
                    }
                    #endregion

                    #region 新增明细
                    foreach (HuStatus huStatus in huStatusList)
                    {
                        MiscOrderDetail matchedMiscOrderDetail = null;

                        #region 明细处理
                        if (miscOrderMaster.MiscOrderDetails != null && miscOrderMaster.MiscOrderDetails.Count > 0)
                        {
                            //查找匹配的明细行
                            matchedMiscOrderDetail = miscOrderMaster.MiscOrderDetails
                                .Where(det => det.Item == huStatus.Item && det.Uom == huStatus.Uom && det.UnitCount == huStatus.UnitCount)
                                .SingleOrDefault();
                        }

                        if (matchedMiscOrderDetail == null)
                        {
                            //没有找到明细行，新增明细//没有找到匹配的明细行，新增一行
                            Item item = this.genericMgr.FindById<Item>(huStatus.Item);
                            matchedMiscOrderDetail = new MiscOrderDetail();

                            matchedMiscOrderDetail.MiscOrderNo = miscOrderMaster.MiscOrderNo;
                            matchedMiscOrderDetail.Sequence = ++maxSeq;
                            matchedMiscOrderDetail.Item = huStatus.Item;
                            matchedMiscOrderDetail.ItemDescription = item.Description;
                            matchedMiscOrderDetail.ReferenceItemCode = item.ReferenceCode;
                            matchedMiscOrderDetail.Uom = huStatus.Uom;
                            matchedMiscOrderDetail.BaseUom = huStatus.BaseUom;
                            matchedMiscOrderDetail.UnitCount = huStatus.UnitCount;
                            matchedMiscOrderDetail.UnitQty = huStatus.UnitQty;
                            matchedMiscOrderDetail.ManufactureParty = huStatus.ManufactureParty;
                            //matchedMiscOrderDetail.Location = 
                            //matchedMiscOrderDetail.ReserveNo = addMiscOrderDetail.ReserveNo;
                            //matchedMiscOrderDetail.ReserveLine = addMiscOrderDetail.ReserveLine;
                            matchedMiscOrderDetail.Qty = huStatus.Qty;

                            this.genericMgr.Create(matchedMiscOrderDetail);

                            miscOrderMaster.MiscOrderDetails.Add(matchedMiscOrderDetail);
                        }
                        else
                        {
                            //找到明细行，更新数量
                            matchedMiscOrderDetail.Qty += huStatus.Qty;
                            this.genericMgr.Update(matchedMiscOrderDetail);
                        }
                        #endregion

                        #region 库存明细新增
                        MiscOrderLocationDetail miscOrderLocationDetail = new MiscOrderLocationDetail();

                        miscOrderLocationDetail.MiscOrderNo = miscOrderMaster.MiscOrderNo;
                        miscOrderLocationDetail.MiscOrderDetailId = matchedMiscOrderDetail.Id;
                        miscOrderLocationDetail.MiscOrderDetailSequence = matchedMiscOrderDetail.Sequence;
                        miscOrderLocationDetail.Item = huStatus.Item;
                        miscOrderLocationDetail.Uom = huStatus.Uom;
                        miscOrderLocationDetail.HuId = huStatus.HuId;
                        miscOrderLocationDetail.LotNo = huStatus.LotNo;
                        miscOrderLocationDetail.IsCreatePlanBill = false;
                        miscOrderLocationDetail.IsConsignment = false;
                        miscOrderLocationDetail.PlanBill = null;
                        miscOrderLocationDetail.ConsignmentSupplier = null;
                        miscOrderLocationDetail.ActingBill = null;
                        miscOrderLocationDetail.QualityType = huStatus.QualityType;
                        miscOrderLocationDetail.IsFreeze = false;
                        miscOrderLocationDetail.IsATP = true;
                        miscOrderLocationDetail.OccupyType = CodeMaster.OccupyType.None;
                        miscOrderLocationDetail.OccupyReferenceNo = null;
                        miscOrderLocationDetail.Qty = huStatus.Qty * huStatus.UnitQty;

                        this.genericMgr.Create(miscOrderLocationDetail);
                        #endregion
                    }
                    #endregion
                    #endregion
                }
                #endregion
            }
            #endregion

            #region 删除计划外出入库明细

            if (deleteHuIdList != null && deleteHuIdList.Count > 0)
            {
                #region 条码处理
                #region 条码是否在计划外出入库单中存在检查
                BusinessException businessException = new BusinessException();
                foreach (string huId in deleteHuIdList)
                {
                    if (miscOrderLocationDetailList == null || miscOrderLocationDetailList.Where(m => m.HuId == huId).Count() == 0)
                    {
                        if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                        {
                            businessException.AddMessage("条码{0}在计划外出库单{1}中不存在。", huId, miscOrderMaster.MiscOrderNo);
                        }
                        else
                        {
                            businessException.AddMessage("条码{0}在计划外入库单{1}中不存在。", huId, miscOrderMaster.MiscOrderNo);
                        }
                    }
                }

                if (businessException.HasMessage)
                {
                    throw businessException;
                }
                #endregion

                #region 循环删除
                #region 取消占用
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    this.locationDetailMgr.CancelInventoryOccupy(CodeMaster.OccupyType.MiscOrder, miscOrderMaster.MiscOrderNo, deleteHuIdList);
                }
                #endregion

                foreach (string huId in deleteHuIdList)
                {
                    #region 扣减明细数量，删除库存明细
                    MiscOrderLocationDetail miscOrderLocationDetail = miscOrderLocationDetailList.Where(det => det.HuId == huId).Single();
                    MiscOrderDetail miscOrderDetail = miscOrderMaster.MiscOrderDetails.Where(det => det.Id == miscOrderLocationDetail.MiscOrderDetailId).Single();
                    miscOrderDetail.Qty -= miscOrderLocationDetail.Qty / miscOrderDetail.UnitQty;

                    this.genericMgr.Update(miscOrderDetail);
                    this.genericMgr.Delete(miscOrderLocationDetail);
                    #endregion
                }
                #endregion
                #endregion
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteMiscOrder(string miscOrderNo)
        {
            this.DeleteMiscOrder(this.genericMgr.FindById<MiscOrderMaster>(miscOrderNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteMiscOrder(MiscOrderMaster miscOrderMaster)
        {
            if (miscOrderMaster.Status != CodeMaster.MiscOrderStatus.Create)
            {
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    throw new BusinessException("计划外出库单{0}的状态为{1}不能删除。",
                          miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
                else
                {
                    throw new BusinessException("计划外入库单{0}的状态为{1}不能删除。",
                         miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
            }

            if (miscOrderMaster.IsScanHu)
            {
                IList<MiscOrderLocationDetail> miscOrderLocationDetailList = TryLoadMiscOrderLocationDetails(miscOrderMaster);
                if (miscOrderLocationDetailList != null && miscOrderLocationDetailList.Count > 0)
                {
                    this.genericMgr.Delete<MiscOrderLocationDetail>(miscOrderLocationDetailList);
                }
            }

            IList<MiscOrderDetail> miscOrderDetailList = TryLoadMiscOrderDetails(miscOrderMaster);
            if (miscOrderDetailList != null && miscOrderDetailList.Count > 0)
            {
                this.genericMgr.Delete<MiscOrderDetail>(miscOrderDetailList);
            }

            this.genericMgr.Delete(miscOrderMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseMiscOrder(string miscOrderNo)
        {
            this.CloseMiscOrder(this.genericMgr.FindById<MiscOrderMaster>(miscOrderNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseMiscOrder(string miscOrderNo, DateTime effectiveDate)
        {
            this.CloseMiscOrder(this.genericMgr.FindById<MiscOrderMaster>(miscOrderNo), effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseMiscOrder(MiscOrderMaster miscOrderMaster)
        {
            this.CloseMiscOrder(miscOrderMaster, miscOrderMaster.EffectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseMiscOrder(MiscOrderMaster miscOrderMaster, DateTime effectiveDate)
        {
            #region 检查
            BusinessException businessException = new BusinessException();
            if (miscOrderMaster.Status != CodeMaster.MiscOrderStatus.Create)
            {
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    businessException.AddMessage("计划外出库单{0}的状态为{1}不能确认。",
                          miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
                else
                {
                    businessException.AddMessage("计划外入库单{0}的状态为{1}不能确认。",
                         miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
            }

            IList<MiscOrderDetail> miscOrderDetailList = TryLoadMiscOrderDetails(miscOrderMaster);
            if (miscOrderDetailList == null || miscOrderDetailList.Count() == 0)
            {
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    businessException.AddMessage("计划外出库单{0}明细为空。", miscOrderMaster.MiscOrderNo);
                }
                else
                {
                    businessException.AddMessage("计划外入库单{0}明细为空。", miscOrderMaster.MiscOrderNo);
                }
            }
            else
            {
                foreach (MiscOrderDetail miscOrderDetail in miscOrderDetailList)
                {
                    if (miscOrderDetail.Qty <= 0)
                    {
                        businessException.AddMessage("计划外入库单{0}明细行{1}的数量不能小于0。", miscOrderMaster.MiscOrderNo, miscOrderDetail.Sequence.ToString());
                    }
                }
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            User user = SecurityContextHolder.Get();
            miscOrderMaster.CloseDate = DateTime.Now;
            miscOrderMaster.CloseUserId = user.Id;
            miscOrderMaster.CloseUserName = user.FullName;
            miscOrderMaster.Status = com.Sconit.CodeMaster.MiscOrderStatus.Close;
            this.genericMgr.Update(miscOrderMaster);
            //后加工废品报工不影响库存
            if (miscOrderMaster.SubType == CodeMaster.MiscOrderSubType.MES27)
            {
                return;
            }
            //
            IList<MiscOrderLocationDetail> miscOrderLocationDetailList = TryLoadMiscOrderLocationDetails(miscOrderMaster);

            foreach (MiscOrderDetail miscOrderDetail in miscOrderDetailList.OrderByDescending(det => det.ManufactureParty))
            {
                miscOrderDetail.ManufactureParty = miscOrderMaster.IsCs ? miscOrderDetail.ManufactureParty : null;
                IList<InventoryTransaction> inventoryTransactionList = this.locationDetailMgr.InventoryOtherInOut(miscOrderMaster, miscOrderDetail, effectiveDate);

                #region 新增、更新订单库存明细
                foreach (InventoryTransaction inventoryTransaction in inventoryTransactionList)
                {
                    if (miscOrderMaster.IsScanHu)
                    {
                        #region 条码
                        MiscOrderLocationDetail miscOrderLocationDetail = miscOrderLocationDetailList.Where(m => m.HuId == inventoryTransaction.HuId).Single();
                        if (inventoryTransaction.ActingBill.HasValue)
                        {
                            miscOrderLocationDetail.IsConsignment = false;
                            miscOrderLocationDetail.PlanBill = null;
                            miscOrderLocationDetail.ActingBill = inventoryTransaction.ActingBill;
                        }

                        this.genericMgr.Update(miscOrderLocationDetail);
                        #endregion
                    }
                    else
                    {
                        #region 数量
                        MiscOrderLocationDetail miscOrderLocationDetail = new MiscOrderLocationDetail();

                        miscOrderLocationDetail.MiscOrderNo = miscOrderMaster.MiscOrderNo;
                        miscOrderLocationDetail.MiscOrderDetailId = miscOrderDetail.Id;
                        miscOrderLocationDetail.MiscOrderDetailSequence = miscOrderDetail.Sequence;
                        miscOrderLocationDetail.Item = inventoryTransaction.Item;
                        miscOrderLocationDetail.Uom = miscOrderDetail.Uom;
                        //miscOrderLocationDetail.HuId = locationLotDetail.HuId;
                        //miscOrderLocationDetail.LotNo = locationLotDetail.LotNo;
                        miscOrderLocationDetail.IsCreatePlanBill = inventoryTransaction.IsCreatePlanBill;
                        miscOrderLocationDetail.IsConsignment = inventoryTransaction.IsConsignment;
                        miscOrderLocationDetail.PlanBill = inventoryTransaction.PlanBill;
                        #region 查找寄售供应商
                        if (inventoryTransaction.IsConsignment && inventoryTransaction.PlanBill.HasValue)
                        {
                            miscOrderLocationDetail.ConsignmentSupplier = this.genericMgr.FindAll<string>("select Party from PlanBill where Id = ?", inventoryTransaction.PlanBill.Value).Single();
                        }
                        #endregion
                        miscOrderLocationDetail.ActingBill = null;
                        miscOrderLocationDetail.QualityType = inventoryTransaction.QualityType;
                        miscOrderLocationDetail.IsFreeze = inventoryTransaction.IsFreeze;
                        miscOrderLocationDetail.IsATP = inventoryTransaction.IsATP;
                        miscOrderLocationDetail.OccupyType = inventoryTransaction.OccupyType;
                        miscOrderLocationDetail.OccupyReferenceNo = inventoryTransaction.OccupyReferenceNo;
                        miscOrderLocationDetail.Qty = inventoryTransaction.Qty;

                        this.genericMgr.Create(miscOrderLocationDetail);
                        #endregion
                    }
                }
                #endregion
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelMiscOrder(string miscOrderNo)
        {
            CancelMiscOrder(miscOrderNo, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelMiscOrder(string miscOrderNo, DateTime effectiveDate)
        {
            this.CancelMiscOrder(this.genericMgr.FindById<MiscOrderMaster>(miscOrderNo), effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelMiscOrder(MiscOrderMaster miscOrderMaster)
        {
            this.CancelMiscOrder(miscOrderMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelMiscOrder(MiscOrderMaster miscOrderMaster, DateTime effectiveDate)
        {
            effectiveDate = miscOrderMaster.EffectiveDate;
            if (miscOrderMaster.Status != CodeMaster.MiscOrderStatus.Close)
            {
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI)
                {
                    throw new BusinessException("计划外出库单{0}的状态为{1}不能冲销。",
                          miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
                else
                {
                    throw new BusinessException("计划外入库单{0}的状态为{1}不能冲销。",
                         miscOrderMaster.MiscOrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ((int)miscOrderMaster.Status).ToString()));
                }
            }

            TryLoadMiscOrderLocationDetails(miscOrderMaster);
            User user = SecurityContextHolder.Get();
            miscOrderMaster.CancelDate = DateTime.Now;
            miscOrderMaster.CancelUserId = user.Id;
            miscOrderMaster.CancelUserName = user.FullName;
            miscOrderMaster.Status = com.Sconit.CodeMaster.MiscOrderStatus.Cancel;

            this.genericMgr.Update(miscOrderMaster);
            //后加工废品报工不影响库存
            if (miscOrderMaster.SubType == CodeMaster.MiscOrderSubType.MES27)
            {
                return;
            }
            //
            this.locationDetailMgr.CancelInventoryOtherInOut(miscOrderMaster, effectiveDate);
        }
        [Transaction(TransactionMode.Requires)]
        public void Import201202MiscOrder(Stream inputStream, string wMSNo, string moveTypeSet, string cancelMoveTypeSet, string miscType)
        {
            #region 导入数据
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 10);

            #region 列定义
            int colMoveType = 1;//移动类型
            //int colEffectiveDate = 2;//生效日期
            //int colRegion = 3;//区域
            int colLocation = 2;//库位
            //int colReferenceNo = 5;//Sap订单号
            int colItem = 3;//物料编号
            int colQty = 4;//数量
            int colCostCenter = 5;//成本中心
            DateTime? prevEffeDate = null;
            string prevRegion = string.Empty;
            #endregion

            BusinessException businessException = new BusinessException();
            int rowCount = 10;
            IList<MiscOrderDetail> activeDetailList = new List<MiscOrderDetail>();
            IList<MiscOrderMaster> activeMasterList = new List<MiscOrderMaster>();
            IList<Region> regionList = this.genericMgr.FindAll<Region>();
            IList<Item> itemList = this.genericMgr.FindAll<Item>();
            IList<Location> locationList = this.genericMgr.FindAll<Location>();
            IList<Location> adjustocationList = this.genericMgr.FindAll<Location>();
            IList<CostCenter> costCenterList = this.genericMgr.FindAll<CostCenter>();
            //调整单Check库位权限
            User user = SecurityContextHolder.Get();

            string sql = @"select * from MD_Location as l where Code in(select distinct(LocFrom) from SCM_FlowMstr where type=4) 
                          and l.IsActive = ?";
            IList<object> paramList = new List<object>();

            paramList.Add(true);
            sql += " order by LEN(Code),Code ";
            adjustocationList = genericMgr.FindEntityWithNativeSql<Location>(sql, paramList.ToArray())
                .Where(p => user.RegionPermissions.Contains(p.Region)).ToList();
            //
            while (rows.MoveNext())
            {
                rowCount++;
                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 1, 9))
                {
                    break;//边界
                }
                string moveType = string.Empty;
                DateTime effectiveDate = System.DateTime.Now;
                string regionCode = string.Empty;

                string locationCode = string.Empty;
                string costCenterCode = string.Empty;
                string referenceNo = string.Empty;
                string itemCode = string.Empty;
                decimal qty = 0;
                Item item = new Item();

                #region 读取数据
                #region 移动类型
                moveType = ImportHelper.GetCellStringValue(row.GetCell(colMoveType));
                if (string.IsNullOrWhiteSpace(moveType))
                {
                    businessException.AddMessage(string.Format("第{0}行:移动类型不能为空。", rowCount));
                }
                else
                {
                    if (moveType != moveTypeSet && moveType != cancelMoveTypeSet)
                    {
                        businessException.AddMessage(string.Format("第{0}行:移动类型{1}填写有误，只能填{2}、{3}。", rowCount, moveType, moveTypeSet, cancelMoveTypeSet));
                    }
                }
                #endregion

                //#region 生效日期
                //string readEffectiveDate = ImportHelper.GetCellStringValue(row.GetCell(colEffectiveDate));
                //if (string.IsNullOrWhiteSpace(readEffectiveDate))
                //{
                //    businessException.AddMessage(string.Format("第{0}行:生效日期不能为空。", rowCount));
                //}
                //else
                //{
                //    if (!DateTime.TryParse(readEffectiveDate, out effectiveDate))
                //    {
                //        businessException.AddMessage(string.Format("第{0}行:生效日期{1}填写有误.", rowCount, moveType));
                //        continue;
                //    }
                //    if (prevEffeDate != null)
                //    {
                //        if (prevEffeDate.Value != effectiveDate)
                //        {
                //            businessException.AddMessage(string.Format("第{0}行:生效日期{1}与前一行生效日期{2}不同。", rowCount, effectiveDate, prevEffeDate.Value));
                //            continue;
                //        }
                //    }
                //    prevEffeDate = effectiveDate;

                //}
                //#endregion

                //#region 区域
                //regionCode = ImportHelper.GetCellStringValue(row.GetCell(colRegion));
                //if (string.IsNullOrWhiteSpace(regionCode))
                //{
                //    businessException.AddMessage(string.Format("第{0}行:区域不能为空。", rowCount));
                //}
                //else
                //{
                //    if (string.IsNullOrWhiteSpace(prevRegion))
                //    {
                //        var regions = regionList.Where(l => l.Code == regionCode).ToList();
                //        if (regions == null || regions.Count == 0)
                //        {
                //            businessException.AddMessage(string.Format("第{0}行:区域{1}填写有误.", rowCount, regionCode));
                //        }
                //    }
                //    else
                //    {
                //        if (regionCode != prevRegion)
                //        {
                //            businessException.AddMessage(string.Format("第{0}行:区域{1}与前一行区域{2}不同。", rowCount, regionCode, prevRegion));
                //            continue;
                //        }
                //    }
                //    prevRegion = regionCode;
                //}
                //#endregion

                #region 读取库位
                locationCode = ImportHelper.GetCellStringValue(row.GetCell(colLocation));
                if (!string.IsNullOrEmpty(locationCode))
                {
                    var locations = locationList.Where(l => l.Code == locationCode).ToList();
                    if (locations == null || locations.Count == 0)
                    {
                        businessException.AddMessage(string.Format("第{0}行:库位{1}不存在。", rowCount, locationCode));
                    }
                    //else if (locations.First().Region != regionCode)
                    //{
                    //    businessException.AddMessage(string.Format("第{0}行:区域{1}不存在库位{2}。", rowCount, regionCode, locationCode));
                    //}
                    else
                    {
                        regionCode = locations[0].Region;
                    }
                    if (miscType == "AdjustOrder")
                    {
                        var adjustlocations = adjustocationList.Where(l => l.Code == locationCode).ToList();
                        if (adjustlocations == null || adjustlocations.Count == 0)
                        {
                            businessException.AddMessage(string.Format("第{0}行:用户没有调整库位{1}的权限。", rowCount, locationCode));
                        }
                    }
                }
                else
                {
                    //businessException.AddMessage(string.Format("第{0}行:区域不能为空。", rowCount));
                }

                #endregion
                #region 成本中心
                costCenterCode = ImportHelper.GetCellStringValue(row.GetCell(colCostCenter));
                if (!string.IsNullOrEmpty(costCenterCode))
                {
                    var costCenters = costCenterList.Where(l => l.Code == costCenterCode).ToList();
                    if (costCenters == null || costCenters.Count == 0)
                    {
                        businessException.AddMessage(string.Format("第{0}行:成本中心{1}不存在。", rowCount, costCenterCode));
                    }
                    else
                    {
                        costCenterCode = costCenters.ToList().FirstOrDefault().Code;
                    }
                    //else if (locations.First().Region != regionCode)
                    //{
                    //    businessException.AddMessage(string.Format("第{0}行:区域{1}不存在库位{2}。", rowCount, regionCode, locationCode));
                    //}
                }
                else if (moveTypeSet == "201")
                {
                    businessException.AddMessage(string.Format("第{0}行:成本中心不能为空。", rowCount));
                }
                #endregion
                //#region Sap订单号
                //referenceNo = ImportHelper.GetCellStringValue(row.GetCell(colReferenceNo));
                //if (string.IsNullOrEmpty(referenceNo))
                //{
                //    businessException.AddMessage(string.Format("第{0}行:Sap订单号不能为空。", rowCount));
                //}
                //else
                //{
                //    //if (this.genericMgr.FindAllWithNativeSql<int>("select count(*) from SAP_ProdBomDet where AUFNR=? ", referenceNo.PadLeft(12, '0'))[0] == 0)
                //    //{
                //    //    businessException.AddMessage(string.Format("第{0}行:Sap订单号不存在ORD_OrderMstr_4表中。", rowCount));
                //    //}
                //}
                //#endregion

                #region 物料编号
                itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItem));
                if (string.IsNullOrWhiteSpace(itemCode))
                {
                    businessException.AddMessage(string.Format("第{0}行:物料编号不能为空。", rowCount));
                }
                else
                {
                    var items = itemList.Where(l => l.Code == itemCode).ToList();
                    if (items == null || items.Count == 0)
                    {
                        businessException.AddMessage(string.Format("第{0}行:物料编号{1}不存在.", rowCount, itemCode));
                    }
                    else
                    {
                        item = items.First();
                    }
                }
                #endregion

                #region 数量
                string readQty = ImportHelper.GetCellStringValue(row.GetCell(colQty));
                if (string.IsNullOrEmpty(readQty))
                {
                    businessException.AddMessage(string.Format("第{0}行:数量不能为空。", rowCount));
                }
                else
                {
                    decimal.TryParse(readQty, out qty);
                    if (qty <= 0)
                    {
                        businessException.AddMessage(string.Format("第{0}行:数量{1}只能为大于等于0的数字。", rowCount, readQty));
                    }
                }
                #endregion

                #endregion

                #region 填充数据
                if (!businessException.HasMessage)
                {
                    MiscOrderDetail miscOrderDetail = new MiscOrderDetail();
                    miscOrderDetail.MoveType = moveType;
                    miscOrderDetail.EffectiveDate = effectiveDate;
                    miscOrderDetail.Location = locationCode;
                    miscOrderDetail.Region = regionCode;
                    miscOrderDetail.Item = item.Code;
                    miscOrderDetail.ItemDescription = item.Description;
                    miscOrderDetail.ReferenceItemCode = item.ReferenceCode;
                    miscOrderDetail.Uom = item.Uom;
                    miscOrderDetail.BaseUom = item.Uom;
                    miscOrderDetail.UnitCount = item.UnitCount;
                    miscOrderDetail.Qty = qty;
                    miscOrderDetail.CostCenter = costCenterCode;
                    activeDetailList.Add(miscOrderDetail);
                }
                #endregion
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            if (activeDetailList.Count == 0)
            {
                throw new BusinessException("导入的有效数据为0，请确实。");
            }
            //Merge details

            //201一张单
            var allLocation = activeDetailList.Select(p => p.Location).Distinct();
            foreach (var location in allLocation)
            {
                var outDetail = activeDetailList.Where(a => a.MoveType == moveTypeSet && a.Location == (string)location).ToList();

                if (outDetail != null && outDetail.Count > 0)
                {
                    MiscOrderDetail fisrDet = outDetail.First();
                    MiscOrderMoveType miscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType =? ", moveTypeSet)[0];
                    MiscOrderMaster miscMaster = new MiscOrderMaster();
                    miscMaster.Type = miscOrderMoveType.IOType;
                    miscMaster.MoveType = miscOrderMoveType.MoveType;
                    miscMaster.CancelMoveType = miscOrderMoveType.CancelMoveType;
                    miscMaster.Location = fisrDet.Location;
                    miscMaster.Region = fisrDet.Region;
                    miscMaster.EffectiveDate = fisrDet.EffectiveDate;
                    miscMaster.IsScanHu = false;
                    miscMaster.ReferenceNo = null;
                    miscMaster.MiscOrderDetails = outDetail;
                    miscMaster.WMSNo = wMSNo;     //备注
                    miscMaster.CostCenter = fisrDet.CostCenter;
                    miscMaster.SubType = miscType == "AdjustOrder" ? CodeMaster.MiscOrderSubType.SY05 : CodeMaster.MiscOrderSubType.COST;
                    activeMasterList.Add(miscMaster);
                }

                //202 一张单
                var inDetail = activeDetailList.Where(a => a.MoveType == cancelMoveTypeSet && a.Location == (string)location).ToList();
                if (inDetail != null && inDetail.Count > 0)
                {
                    MiscOrderDetail fisrInDet = inDetail.First();
                    MiscOrderMoveType miscOrderInMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType =? ", cancelMoveTypeSet)[0];
                    var inMiscOrder = new MiscOrderMaster();
                    inMiscOrder.Type = miscOrderInMoveType.IOType;
                    inMiscOrder.MoveType = miscOrderInMoveType.MoveType;
                    inMiscOrder.CancelMoveType = miscOrderInMoveType.CancelMoveType;
                    inMiscOrder.Location = fisrInDet.Location;
                    inMiscOrder.Region = fisrInDet.Region;
                    inMiscOrder.EffectiveDate = fisrInDet.EffectiveDate;
                    inMiscOrder.IsScanHu = false;
                    inMiscOrder.ReferenceNo = null;
                    inMiscOrder.MiscOrderDetails = inDetail;
                    inMiscOrder.WMSNo = wMSNo;  //备注
                    inMiscOrder.CostCenter = fisrInDet.CostCenter;
                    inMiscOrder.SubType = miscType == "AdjustOrder" ? CodeMaster.MiscOrderSubType.SY05 : CodeMaster.MiscOrderSubType.COST;
                    activeMasterList.Add(inMiscOrder);
                }
            }
            if (businessException.HasMessage)
            {
                throw businessException;
            }

            string message = "生成单号";
            foreach (var master in activeMasterList)
            {
                master.QualityType = com.Sconit.CodeMaster.QualityType.Qualified;
                activeDetailList = (from p in master.MiscOrderDetails
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
                master.MiscOrderDetails = new List<MiscOrderDetail>();
                this.CreateMiscOrder(master);
                BatchUpdateMiscOrderDetails(master, activeDetailList, null, null);
                this.genericMgr.FlushSession();
                master.MiscOrderDetails = null;
                //CloseMiscOrder(master, master.EffectiveDate);
                message += " " + master.MiscOrderNo + ";";
            }
            MessageHolder.AddMessage(new Message(CodeMaster.MessageType.Info, message));
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateMiscOrderDetailFromXls(Stream inputStream, string miscOrderNo)
        {
            //导入的话以导入的为准，之前的全部干掉
            #region 清空明细
            string hql = @"from MiscOrderDetail as m where m.MiscOrderNo = ?";
            genericMgr.Delete(hql, new object[] { miscOrderNo }, new IType[] { NHibernateUtil.String });
            #endregion


            MiscOrderMaster miscOrder = genericMgr.FindById<MiscOrderMaster>(miscOrderNo);

            #region 导入数据
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 11);

            #region 列定义
            int colItem = 1;//物料代码
            //int colUom = 3;//单位
            int colLocation = 2;//库位
            int colQty = 3;//数量
            int colReverseLine = 4;//预留行
            int colReverseNo = 5;//预留号
            #endregion

            DateTime dateTimeNow = DateTime.Now;

            IList<MiscOrderDetail> miscOrderDetailList = new List<MiscOrderDetail>();
            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 1, 9))
                {
                    break;//边界
                }
                string itemCode = string.Empty;
                decimal qty = 0;
                string uomCode = string.Empty;
                string locationCode = string.Empty;
                string reverseLine = string.Empty;
                string reverseNo = string.Empty;

                #region 读取数据
                #region 读取物料代码
                itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItem));
                if (itemCode == null || itemCode.Trim() == string.Empty)
                {
                    ImportHelper.ThrowCommonError(row.RowNum, colItem, row.GetCell(colItem));
                }

                #endregion
                //#region 读取单位
                //uomCode = row.GetCell(colUom) != null ? row.GetCell(colUom).StringCellValue : string.Empty;
                //if (uomCode == null || uomCode.Trim() == string.Empty)
                //{
                //    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colUom.ToString());
                //}
                //#endregion

                #endregion

                #region 读取库位
                locationCode = row.GetCell(colLocation) != null ? row.GetCell(colLocation).StringCellValue : string.Empty;
                if (string.IsNullOrEmpty(locationCode))
                {
                    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), locationCode.ToString());
                }

                Location location = genericMgr.FindById<Location>(locationCode);
                //if (location.Region != miscOrder.Region)
                //{
                //    throw new BusinessException("指定区域不存在此库位" + location, (row.RowNum + 1).ToString(), colLocation.ToString());
                //}
                #endregion


                #region 读取数量
                try
                {
                    qty = decimal.Parse(ImportHelper.GetCellStringValue(row.GetCell(colQty)));
                }
                catch
                {
                    ImportHelper.ThrowCommonError(row.RowNum, colQty, row.GetCell(colQty));
                }
                #endregion

                #region
                MiscOrderMoveType miscOrderMoveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { miscOrder.MoveType, miscOrder.Type })[0];
                if (miscOrderMoveType.CheckReserveLine)
                {
                    reverseLine = row.GetCell(colReverseLine) != null ? row.GetCell(colReverseLine).StringCellValue : string.Empty;
                    //if (reverseLine == null || reverseLine.Trim() == string.Empty)
                    //{
                    //    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colUom.ToString());
                    //}
                }
                if (miscOrderMoveType.CheckReserveNo)
                {
                    reverseNo = row.GetCell(colReverseNo) != null ? row.GetCell(colReverseNo).StringCellValue : string.Empty;
                    //if (reverseNo == null || reverseNo.Trim() == string.Empty)
                    //{
                    //    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colUom.ToString());
                    //}
                }
                #endregion

                #region 填充数据
                MiscOrderDetail mod = new MiscOrderDetail();
                Item item = genericMgr.FindById<Item>(itemCode);
                mod.Location = locationCode;
                mod.Item = itemCode;
                mod.Uom = item.Uom;
                mod.ItemDescription = item.Description;
                mod.BaseUom = item.Uom;
                mod.Qty = qty;
                mod.ReserveLine = reverseLine;
                mod.ReserveNo = reverseNo;

                miscOrderDetailList.Add(mod);
                #endregion
            }

            #endregion

            #region 新增明细
            CreateMiscOrderDetail(miscOrderDetailList, miscOrder.MiscOrderNo);
            #endregion
        }


        [Transaction(TransactionMode.Requires)]
        public void QuickCreateMiscOrder(IList<string> addHuIdList, string locationCode, string binCode, int type)
        {
            MiscOrderMaster miscOrderMaster = new MiscOrderMaster();
            miscOrderMaster.MiscOrderNo = this.numberControlMgr.GetMiscOrderNo(miscOrderMaster);
            miscOrderMaster.EffectiveDate = DateTime.Now;
            miscOrderMaster.QualityType = CodeMaster.QualityType.Qualified;
            miscOrderMaster.IsScanHu = true;
            miscOrderMaster.CreateDate = DateTime.Now;
            User user = SecurityContextHolder.Get();
            miscOrderMaster.CreateUserId = user.Id;
            miscOrderMaster.CreateUserName = user.Name;

            if (type == 1)
            {
                MiscOrderMoveType moveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { "101", type })[0];

                miscOrderMaster.Type = CodeMaster.MiscOrderType.GR;
                miscOrderMaster.MoveType = moveType.MoveType;
                miscOrderMaster.CancelMoveType = moveType.CancelMoveType;

                if (!string.IsNullOrEmpty(binCode))
                {
                    LocationBin bin = genericMgr.FindById<LocationBin>(binCode);
                    Location location = genericMgr.FindById<Location>(bin.Location);
                    miscOrderMaster.Location = bin.Location;
                    miscOrderMaster.Region = location.Region;
                }
                else if (!string.IsNullOrEmpty(locationCode))
                {
                    Location location = genericMgr.FindById<Location>(locationCode);
                    miscOrderMaster.Location = location.Code;
                    miscOrderMaster.Region = location.Region;
                }
            }
            else
            {
                MiscOrderMoveType moveType = genericMgr.FindAll<MiscOrderMoveType>("from MiscOrderMoveType as m where m.MoveType=? and m.IOType=?", new object[] { "102", type })[0];
                miscOrderMaster.Type = CodeMaster.MiscOrderType.GI;
                miscOrderMaster.MoveType = moveType.MoveType;
                miscOrderMaster.CancelMoveType = moveType.CancelMoveType;

                Location location = genericMgr.FindById<Location>(locationCode);
                miscOrderMaster.Location = location.Code;
                miscOrderMaster.Region = location.Region;
            }

            this.CreateMiscOrder(miscOrderMaster);

            BatchUpdateMiscOrderDetails(miscOrderMaster, addHuIdList, null);
            this.genericMgr.FlushSession();
            CloseMiscOrder(miscOrderMaster.MiscOrderNo);

            #region 上架
            if (type == 1 && !string.IsNullOrEmpty(binCode))
            {
                var inventoryPutList = new List<Entity.INV.InventoryPut>();
                foreach (var huId in addHuIdList)
                {
                    var inventoryPut = new Entity.INV.InventoryPut();

                    inventoryPut.Bin = binCode;
                    inventoryPut.HuId = huId;
                    inventoryPutList.Add(inventoryPut);
                }
                this.locationDetailMgr.InventoryPut(inventoryPutList);
            }
            #endregion
        }

        private IList<MiscOrderDetail> TryLoadMiscOrderDetails(MiscOrderMaster miscOrderMaster)
        {
            if (!string.IsNullOrWhiteSpace(miscOrderMaster.MiscOrderNo))
            {
                if (miscOrderMaster.MiscOrderDetails == null)
                {
                    string hql = "from MiscOrderDetail where MiscOrderNo = ? order by Sequence";

                    miscOrderMaster.MiscOrderDetails = this.genericMgr.FindAll<MiscOrderDetail>(hql, miscOrderMaster.MiscOrderNo);
                }

                return miscOrderMaster.MiscOrderDetails;
            }
            else
            {
                return null;
            }
        }

        private IList<MiscOrderLocationDetail> TryLoadMiscOrderLocationDetails(MiscOrderMaster miscOrderMaster)
        {
            if (miscOrderMaster.MiscOrderNo != null)
            {
                TryLoadMiscOrderDetails(miscOrderMaster);

                IList<MiscOrderLocationDetail> miscOrderLocationDetailList = new List<MiscOrderLocationDetail>();

                string hql = string.Empty;
                IList<object> para = new List<object>();
                foreach (MiscOrderDetail miscOrderDetail in miscOrderMaster.MiscOrderDetails)
                {
                    if (miscOrderDetail.MiscOrderLocationDetails != null && miscOrderDetail.MiscOrderLocationDetails.Count > 0)
                    {
                        ((List<MiscOrderLocationDetail>)miscOrderLocationDetailList).AddRange(miscOrderDetail.MiscOrderLocationDetails);
                    }
                    else
                    {
                        if (hql == string.Empty)
                        {
                            hql = "from MiscOrderLocationDetail where MiscOrderDetailId in (?";
                        }
                        else
                        {
                            hql += ",?";
                        }
                        para.Add(miscOrderDetail.Id);
                    }
                }

                if (hql != string.Empty)
                {
                    hql += ") order by MiscOrderDetailId";

                    ((List<MiscOrderLocationDetail>)miscOrderLocationDetailList).AddRange(this.genericMgr.FindAll<MiscOrderLocationDetail>(hql, para.ToArray()));
                }

                foreach (MiscOrderDetail miscOrderDetail in miscOrderMaster.MiscOrderDetails)
                {
                    if (miscOrderDetail.MiscOrderLocationDetails == null || miscOrderDetail.MiscOrderLocationDetails.Count == 0)
                    {
                        miscOrderDetail.MiscOrderLocationDetails = miscOrderLocationDetailList.Where(o => o.MiscOrderDetailId == miscOrderDetail.Id).ToList();
                    }
                }

                return miscOrderLocationDetailList;
            }
            else
            {
                return null;
            }
        }

        private void CreateMiscOrderDetail(IList<MiscOrderDetail> miscOrderDetailList, string miscOrderNo)
        {
            int maxSeq = 0;
            foreach (MiscOrderDetail miscOrderDetail in miscOrderDetailList)
            {

                miscOrderDetail.MiscOrderNo = miscOrderNo;
                miscOrderDetail.Sequence = ++maxSeq;
                if (miscOrderDetail.Uom != miscOrderDetail.BaseUom)
                {
                    miscOrderDetail.UnitQty = this.itemMgr.ConvertItemUomQty(miscOrderDetail.Item, miscOrderDetail.BaseUom, 1, miscOrderDetail.Uom);
                }
                else
                {
                    miscOrderDetail.UnitQty = 1;
                }
                this.genericMgr.Create(miscOrderDetail);
            }
        }
    }
}
