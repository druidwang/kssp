using System;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity.INP;
using Castle.Services.Transaction;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.MD;
using AutoMapper;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.CUST;
using com.Sconit.Utility;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class InspectMgrImpl : BaseMgr, IInspectMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }

        #region 创建报验单
        [Transaction(TransactionMode.Requires)]
        public void CreateInspectMaster(InspectMaster inspectMaster)
        {
            this.CreateInspectMaster(inspectMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateInspectMaster(InspectMaster inspectMaster, DateTime effectiveDate)
        {
            #region 检查
            if (inspectMaster.InspectDetails == null || inspectMaster.InspectDetails.Where(i => i.InspectQty > 0 || !string.IsNullOrWhiteSpace(i.HuId)).Count() == 0)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.InspectDetailShouldNotEmpty);
            }

            inspectMaster.InspectDetails = inspectMaster.InspectDetails.Where(i => i.InspectQty > 0 || !string.IsNullOrWhiteSpace(i.HuId)).OrderBy(det => det.Sequence).ToList();
            if (inspectMaster.Type == CodeMaster.InspectType.Barcode)
            {
                BusinessException businessException = new BusinessException();
                IList<HuStatus> huStatusList = huMgr.GetHuStatus(inspectMaster.InspectDetails.Select(i => i.HuId).ToList());
                #region 查找零件
                string hql = string.Empty;
                IList<object> paras = new List<object>();
                foreach (string itemCode in huStatusList.Select(i => i.Item).Distinct())
                {
                    if (hql == string.Empty)
                    {
                        hql = "from Item where Code in (?";
                    }
                    else
                    {
                        hql += ", ?";
                    }
                    paras.Add(itemCode);
                }
                hql += ")";
                IList<Item> itemList = this.genericMgr.FindAll<Item>(hql, paras.ToArray());
                #endregion

                int seq = 1; //新的序号
                foreach (InspectDetail inspectDetail in inspectMaster.InspectDetails)
                {
                    HuStatus huStatus = huStatusList.Where(h => h.HuId == inspectDetail.HuId).SingleOrDefault();
                    if (huStatus == null)
                    {
                        businessException.AddMessage(Resources.EXT.ServiceLan.HuNotExist, inspectDetail.HuId);
                    }
                    else if (huStatus.Status == CodeMaster.HuStatus.NA)
                    {
                        businessException.AddMessage(Resources.EXT.ServiceLan.HuNotInInvCanNotInspect, huStatus.HuId);
                    }
                    else if (huStatus.Status == CodeMaster.HuStatus.Ip)
                    {
                        businessException.AddMessage(Resources.EXT.ServiceLan.HuIsInProcessCanNotInspect, huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                    }
                    else if (huStatus.OccupyType != CodeMaster.OccupyType.None)
                    {
                        businessException.AddMessage(Resources.EXT.ServiceLan.HuIsOccupiedCanNotInspect, huStatus.HuId);
                    }
                    else
                    {
                        inspectDetail.Sequence = seq++;
                        inspectDetail.Item = huStatus.Item;
                        inspectDetail.ItemDescription = itemList.Where(i => i.Code == huStatus.Item).Single().Description;
                        inspectDetail.ReferenceItemCode = huStatus.ReferenceItemCode;
                        inspectDetail.UnitCount = huStatus.UnitCount;
                        inspectDetail.Uom = huStatus.Uom;
                        inspectDetail.BaseUom = huStatus.BaseUom;
                        inspectDetail.UnitQty = huStatus.UnitQty;
                        inspectDetail.LotNo = huStatus.LotNo;
                        inspectDetail.LocationFrom = huStatus.Location;
                        inspectDetail.CurrentLocation = huStatus.Location;
                        inspectDetail.InspectQty = huStatus.Qty;
                    }
                }

                #region 检查报验零件是否在同一个区域中
                IList<string> regionList = huStatusList.Select(l => l.Region).Distinct().ToList();
                if (regionList != null && regionList.Count > 1)
                {
                    throw new BusinessException(Resources.EXT.ServiceLan.TheHusNotInSameLoc);
                }

                inspectMaster.Region = regionList.Single();
                #endregion

                if (businessException.HasMessage)
                {
                    throw businessException;
                }
            }
            else
            {
                int seq = 1;
                foreach (var inspectDetail in inspectMaster.InspectDetails)
                {
                    inspectDetail.Sequence = seq;
                    seq++;
                }
            }
            #endregion

            #region 创建报验单头
            inspectMaster.InspectNo = this.numberControlMgr.GetInspectNo(inspectMaster);
            this.genericMgr.Create(inspectMaster);
            #endregion

            #region 创建报验单明细
            foreach (var inspectDetail in inspectMaster.InspectDetails)
            {
                inspectDetail.InspectNo = inspectMaster.InspectNo;
                inspectDetail.ManufactureParty = inspectMaster.ManufactureParty;
                this.genericMgr.Create(inspectDetail);
            }
            #endregion

            #region 库存操作
            locationDetailMgr.InventoryInspect(inspectMaster, effectiveDate);
            #endregion
        }
        #endregion

        #region 报验单扫描条码
        public IList<InspectDetail> AddInspectDetail(string HuId, IList<InspectDetail> InspectDetailDetailList)
        {
            if (InspectDetailDetailList == null)
            {
                InspectDetailDetailList = new List<InspectDetail>();
            }
            else
            {
                IList<InspectDetail> q = InspectDetailDetailList.Where(v => v.HuId == HuId).ToList();
                if (q.Count > 0)
                {
                    throw new BusinessException(Resources.EXT.ServiceLan.HuIsOccupiedCanNotInspect, HuId);
                }
            }

            IList<LocationLotDetail> itemList = genericMgr.FindAll<LocationLotDetail>("select l from LocationLotDetail as l where HuId='" + HuId + "'");
            if (itemList == null || itemList.Count() == 0)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.NotFoundTheMatchHu, HuId);
            }
            //IList<InspectDetail> inspectDetailByHuId = this.genericMgr.FindAll<InspectDetail>("select i from InspectDetail as i where i.HuId='" + HuId + "'");
            //if (inspectDetailByHuId.Count() > 0)
            //{
            //    throw new BusinessException("条码{0}已经被占用", HuId);
            //}
            LocationLotDetail locationLotDetail = itemList[0];



            InspectDetail inspectDetail = new InspectDetail();
            inspectDetail.Item = locationLotDetail.Item;
            inspectDetail.ItemDescription = locationLotDetail.ItemDescription;
            inspectDetail.ReferenceItemCode = locationLotDetail.ReferenceItemCode;
            inspectDetail.HuId = HuId;
            inspectDetail.Uom = locationLotDetail.HuUom;
            inspectDetail.UnitCount = locationLotDetail.UnitCount;
            inspectDetail.InspectQty = locationLotDetail.Qty;
            inspectDetail.LocationFrom = locationLotDetail.Location;
            inspectDetail.CurrentLocation = locationLotDetail.Location;
            InspectDetailDetailList.Add(inspectDetail);
            return InspectDetailDetailList;
        }
        #endregion

        #region 报验立即判定不合格
        [Transaction(TransactionMode.Requires)]
        public void CreateAndReject(InspectMaster inspectMaster)
        {
            CreateAndReject(inspectMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateAndReject(InspectMaster inspectMaster, DateTime effectiveDate)
        {
            CreateInspectMaster(inspectMaster, effectiveDate);
            this.genericMgr.FlushSession();
            var inspectDetails = new List<InspectDetail>();
            foreach (InspectDetail inspectDetail in inspectMaster.InspectDetails)
            {
                //inspectDetail.CurrentRejectQty = inspectDetail.InspectQty;
                if (!string.IsNullOrWhiteSpace(inspectDetail.FailCode))
                {
                    inspectDetail.JudgeFailCode = inspectDetail.FailCode;
                    inspectDetail.CurrentQty = inspectDetail.InspectQty;
                    inspectDetails.Add(inspectDetail);
                }
            }

            JudgeInspectDetail(inspectDetails, effectiveDate);
        }
        #endregion

        #region 报验判定
        [Transaction(TransactionMode.Requires)]
        public void JudgeInspectDetail(IList<InspectDetail> inspectDetailList)
        {
            JudgeInspectDetail(inspectDetailList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void JudgeInspectDetail(IList<InspectDetail> inspectDetailList, DateTime effectiveDate)
        {
            #region 检查
            var failCodeDic = this.genericMgr.FindAll<FailCode>().ToDictionary(d => d.Code, d => d);
            if (inspectDetailList == null)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.JudgeResultShouldNotEmpty);
            }
            IList<InspectDetail> noneZeroInspectDetailList = inspectDetailList.Where(i => i.CurrentQty > 0).ToList();
            if (noneZeroInspectDetailList == null || noneZeroInspectDetailList.Count == 0)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.JudgeResultShouldNotEmpty);
            }
            BusinessException businessException = new BusinessException();
            foreach (InspectDetail inspectDetail in noneZeroInspectDetailList)
            {
                if (inspectDetail.InspectQty < (inspectDetail.QualifyQty + inspectDetail.RejectQty + inspectDetail.CurrentQty))
                {
                    businessException.AddMessage(Resources.EXT.ServiceLan.TheJudgeQtyMoreThenInspectQty, inspectDetail.InspectNo, inspectDetail.Sequence.ToString());
                }
                if (!string.IsNullOrWhiteSpace(inspectDetail.JudgeFailCode))
                {
                    inspectDetail.JudgeFailCode = inspectDetail.JudgeFailCode.Trim();
                    var failCode = failCodeDic.ValueOrDefault(inspectDetail.JudgeFailCode);
                    if (failCode == null)
                    {
                        businessException.AddMessage(Resources.EXT.ServiceLan.TheJudgeFailCodeIsInvaild, inspectDetail.InspectNo, inspectDetail.Sequence.ToString());
                    }
                    else
                    {
                        inspectDetail.HandleResult = failCode.HandleResult;
                    }
                    if (string.IsNullOrWhiteSpace(inspectDetail.FailCode))
                    {
                        inspectDetail.FailCode = inspectDetail.JudgeFailCode;
                    }
                }
                else
                {
                    businessException.AddMessage(Resources.EXT.ServiceLan.TheJudgeFailCodeIsInvaild, inspectDetail.InspectNo, inspectDetail.Sequence.ToString());
                }
            }
            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            #region 查找
            var inspectMasterList = this.genericMgr.FindAllIn<InspectMaster>
                ("from InspectMaster where InspectNo in (?", noneZeroInspectDetailList.Select(i => i.InspectNo).Distinct());
            var inspectMasterDic = inspectMasterList.ToDictionary(d => d.InspectNo, d => d);
            #endregion

            #region 生成报验结果
            var inspectResultList = noneZeroInspectDetailList.Select(p => new InspectResult
            {
                InspectNo = p.InspectNo,
                InspectDetailId = p.Id,
                InspectDetailSequence = p.Sequence,
                Item = p.Item,
                ItemDescription = p.ItemDescription,
                ReferenceItemCode = p.ReferenceItemCode,
                UnitCount = p.UnitCount,
                Uom = p.Uom,
                BaseUom = p.BaseUom,
                UnitQty = p.UnitQty,
                HuId = p.HuId,
                LotNo = p.LotNo,
                LocationFrom = p.LocationFrom,
                CurrentLocation = p.CurrentLocation,
                JudgeResult = p.HandleResult == CodeMaster.HandleResult.Qualify ? CodeMaster.JudgeResult.Qualified : CodeMaster.JudgeResult.Rejected,
                JudgeQty = p.CurrentQty,
                HandleQty = 0,
                IsHandle = false,
                ManufactureParty = p.ManufactureParty,
                CurrentHandleQty = p.CurrentQty,
                RejectHandleResult = p.HandleResult,
                IpNo = inspectMasterDic[p.InspectNo].IpNo,
                IpDetailSequence = p.IpDetailSequence,
                FailCode = p.JudgeFailCode,
                Defect = p.Defect,
                WMSNo = p.WMSResNo,
                WMSSeq = p.WMSResSeq,
                Note = p.CurrentInspectResultNote,
                ReceiptNo = inspectMasterDic[p.InspectNo].ReceiptNo,
                ReceiptDetailSequence = p.ReceiptDetailSequence,
            }).ToList();
            #endregion

            #region 保存报验头
            foreach (InspectMaster inspectMaster in inspectMasterList)
            {
                if (inspectMaster.Status == CodeMaster.InspectStatus.Submit)
                {
                    inspectMaster.Status = CodeMaster.InspectStatus.InProcess;
                    this.genericMgr.Update(inspectMaster);
                }
            }
            #endregion

            #region 保存报验明细
            foreach (InspectDetail inspectDetail in noneZeroInspectDetailList)
            {
                if (failCodeDic[inspectDetail.JudgeFailCode].HandleResult == CodeMaster.HandleResult.Qualify)
                {
                    inspectDetail.QualifyQty += inspectDetail.CurrentQty;
                }
                else
                {
                    inspectDetail.RejectQty += inspectDetail.CurrentQty;
                }
                if (inspectDetail.InspectQty == inspectDetail.QualifyQty + inspectDetail.RejectQty)
                {
                    inspectDetail.IsJudge = true;
                }

                this.genericMgr.UpdateWithTrim(inspectDetail);
            }
            #endregion

            #region 保存报验结果
            foreach (InspectResult inspectResult in inspectResultList)
            {
                this.genericMgr.Create(inspectResult);
            }
            #endregion

            #region 关闭报验单
            foreach (InspectMaster inspectMaster in inspectMasterList)
            {
                this.genericMgr.FlushSession();
                TryCloseInspectMaster(inspectMaster);
            }
            #endregion

            #region 库存操作
            foreach (InspectMaster inspectMaster in inspectMasterList)
            {
                this.locationDetailMgr.InspectJudge(inspectMaster, inspectResultList.Where(i => i.InspectNo == inspectMaster.InspectNo).ToList());
            }
            #endregion

            #region 不合格品自动处理,(只有让步使用的是自动处理)
            var autoCreateRejectList = from rst in inspectResultList
                                       where rst.RejectHandleResult == CodeMaster.HandleResult.Concession
                                       group rst by rst.RejectHandleResult into g
                                       select new
                                       {
                                           HandleResult = g.Key,
                                           InspectResultList = g.ToList()
                                       };

            if (autoCreateRejectList != null && autoCreateRejectList.Count() > 0)
            {
                this.genericMgr.FlushSession();
                foreach (var autoCreateReject in autoCreateRejectList)
                {
                    RejectMaster rejectMaster = CreateRejectMaster(autoCreateReject.HandleResult, autoCreateReject.InspectResultList, effectiveDate);
                    ReleaseRejectMaster(rejectMaster);
                }
            }
            #endregion
        }
        #endregion

        #region 创建不合格品处理单
        [Transaction(TransactionMode.Requires)]
        public RejectMaster CreateRejectMaster(CodeMaster.HandleResult rejectHandleResult, IList<InspectResult> inspectResultList)
        {
            return CreateRejectMaster(rejectHandleResult, inspectResultList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public RejectMaster CreateRejectMaster(CodeMaster.HandleResult rejectHandleResult, IList<InspectResult> inspectResultList, DateTime effectiveDate)
        {
            #region 检查
            if (inspectResultList == null)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.RejectProcessNotEmpty);
            }

            IList<InspectResult> noneZeroInspectResultList = inspectResultList.Where(i => i.CurrentHandleQty > 0).ToList();

            if (noneZeroInspectResultList == null || noneZeroInspectResultList.Count == 0)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.RejectProcessNotEmpty);
            }

            foreach (InspectResult inspectResult in noneZeroInspectResultList)
            {
                if (inspectResult.JudgeQty < (inspectResult.HandleQty + inspectResult.CurrentHandleQty))
                {
                    throw new BusinessException("不合格品的处理数超过了判定数。");
                }
            }

            #region 检查不合格品处理是否在同一个区域中
            #region 查询Location
            string hql = string.Empty;
            IList<object> paras = new List<object>();
            foreach (string locationCode in noneZeroInspectResultList.Select(i => i.CurrentLocation).Distinct())
            {
                if (hql == string.Empty)
                {
                    hql = "from Location where Code in (?";
                }
                else
                {
                    hql += ", ?";
                }
                paras.Add(locationCode);
            }
            hql += ")";
            IList<Location> locationList = this.genericMgr.FindAll<Location>(hql, paras.ToArray());
            #endregion

            IList<string> regionList = locationList.Select(l => l.Region).Distinct().ToList();
            if (regionList != null && regionList.Count > 1)
            {
                throw new BusinessException("不合格品的库位属于不同区域不能合并处理。");
            }

            string region = regionList.Single();
            #endregion
            #endregion

            #region 生成不合格品处理单
            #region 生成不合格品处理单头
            RejectMaster rejectMaster = new RejectMaster();
            rejectMaster.Status = CodeMaster.RejectStatus.Create;
            rejectMaster.Region = region;
            rejectMaster.HandleResult = rejectHandleResult;
            rejectMaster.RejectNo = this.numberControlMgr.GetRejectNo(rejectMaster);
            //條碼還是數量
            rejectMaster.InspectType = noneZeroInspectResultList.Where(r => r.HuId != null).Count() > 0 ? com.Sconit.CodeMaster.InspectType.Barcode : com.Sconit.CodeMaster.InspectType.Quantity;

            this.genericMgr.Create(rejectMaster);
            #endregion

            #region 生成不合格品处理单明细
            int seq = 1;
            foreach (InspectResult inspectResult in noneZeroInspectResultList)
            {
                RejectDetail rejectDetail = Mapper.Map<InspectResult, RejectDetail>(inspectResult);
                rejectDetail.Sequence = seq++;
                rejectDetail.HandleQty = inspectResult.CurrentHandleQty;
                rejectDetail.HandledQty = 0;
                rejectDetail.FailCode = inspectResult.FailCode;
                rejectDetail.RejectNo = rejectMaster.RejectNo;
                rejectDetail.ManufactureParty = inspectResult.ManufactureParty;

                rejectMaster.AddRejectDetail(rejectDetail);
                this.genericMgr.Create(rejectDetail);
            }
            #endregion
            #endregion

            #region 更新判定结果
            foreach (InspectResult inspectResult in noneZeroInspectResultList)
            {
                inspectResult.HandleQty += inspectResult.CurrentHandleQty;
                if (inspectResult.HandleQty == inspectResult.JudgeQty)
                {
                    inspectResult.IsHandle = true;
                }
                this.genericMgr.Update(inspectResult);
            }
            #endregion
            return rejectMaster;
        }
        #endregion

        #region 添加不合格品处理单明细
        [Transaction(TransactionMode.Requires)]
        public IList<RejectDetail> AddRejectDetails(string rejectNo, IList<InspectResult> inspectResultList)
        {
            #region 检查
            if (string.IsNullOrWhiteSpace(rejectNo))
            {
                throw new ArgumentNullException("RejectNo not specified.");
            }

            RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(rejectNo);

            if (inspectResultList == null)
            {
                throw new BusinessException("不合格品处理结果不能为空。");
            }

            IList<InspectResult> noneZeroInspectResultList = inspectResultList.Where(i => i.CurrentHandleQty > 0).ToList();

            if (noneZeroInspectResultList == null || noneZeroInspectResultList.Count == 0)
            {
                throw new BusinessException("不合格品处理结果不能为空。");
            }

            foreach (InspectResult inspectResult in noneZeroInspectResultList)
            {
                if (inspectResult.JudgeQty < (inspectResult.HandleQty + inspectResult.CurrentHandleQty))
                {
                    throw new BusinessException("不合格品的处理数超过了判定数。");
                }
            }

            #region 检查不合格品处理是否在不合格品处理单的区域中
            #region 查询Location
            string hql = string.Empty;
            IList<object> paras = new List<object>();
            foreach (string locationCode in noneZeroInspectResultList.Select(i => i.CurrentLocation).Distinct())
            {
                if (hql == string.Empty)
                {
                    hql = "from Location where Code in (?";
                }
                else
                {
                    hql += ", ?";
                }
                paras.Add(locationCode);
            }
            hql += ")";
            IList<Location> locationList = this.genericMgr.FindAll<Location>(hql, paras.ToArray());
            #endregion

            if (locationList.Where(l => l.Region != rejectMaster.Region).Count() > 0)
            {
                throw new BusinessException("添加的不合格品的库位不属于区域{0}。", rejectMaster.Region);
            }
            #endregion
            #endregion

            IList<RejectDetail> resultRejectDetailList = new List<RejectDetail>();
            #region 获取最大订单明细序号
            IList<object> maxSeqList = genericMgr.FindAll<object>("select max(Sequence) as seq from RejectDetail where RejectNo = ?", rejectMaster.RejectNo);
            int maxSeq = (maxSeqList.Count > 0 && maxSeqList[0] != null) ? (int)maxSeqList[0] : 0;
            #endregion

            #region 生成不合格品处理单明细
            foreach (InspectResult inspectResult in noneZeroInspectResultList)
            {
                RejectDetail rejectDetail = Mapper.Map<InspectResult, RejectDetail>(inspectResult);
                rejectDetail.Sequence = ++maxSeq;
                rejectDetail.HandleQty = inspectResult.CurrentHandleQty;
                rejectDetail.HandledQty = 0;
                rejectDetail.FailCode = inspectResult.CurrentFailCode;
                rejectDetail.RejectNo = rejectNo;
                rejectDetail.ManufactureParty = inspectResult.ManufactureParty;

                rejectMaster.AddRejectDetail(rejectDetail);
                this.genericMgr.Create(rejectDetail);
                resultRejectDetailList.Add(rejectDetail);
            }
            #endregion

            #region 更新判定结果
            foreach (InspectResult inspectResult in noneZeroInspectResultList)
            {
                inspectResult.HandleQty += inspectResult.CurrentHandleQty;
                if (inspectResult.HandleQty == inspectResult.JudgeQty)
                {
                    inspectResult.IsHandle = true;
                }
                this.genericMgr.Update(inspectResult);
            }
            #endregion

            return resultRejectDetailList;
        }
        #endregion

        #region 更新不合格品处理单明细
        [Transaction(TransactionMode.Requires)]
        public IList<RejectDetail> BatchUpdateRejectDetails(string rejectNo, IList<RejectDetail> updateRejectDetailList, IList<RejectDetail> deleteRejectDetailList)
        {
            #region 检查
            if (string.IsNullOrWhiteSpace(rejectNo))
            {
                throw new ArgumentNullException("RejectNo not specified.");
            }

            RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(rejectNo);

            if (rejectMaster.Status != com.Sconit.CodeMaster.RejectStatus.Create)
            {
                throw new BusinessException("状态为{1}的不合格品处理单{0}不能修改明细。", rejectMaster.RejectNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.RejectStatus, (int)rejectMaster.Status));
            }
            #endregion

            #region 查询报验单结果
            IList<RejectDetail> rejectDetailList = new List<RejectDetail>();
            if (updateRejectDetailList != null && updateRejectDetailList.Count > 0)
            {
                ((List<RejectDetail>)rejectDetailList).AddRange(updateRejectDetailList);
            }
            if (deleteRejectDetailList != null && deleteRejectDetailList.Count > 0)
            {
                ((List<RejectDetail>)rejectDetailList).AddRange(deleteRejectDetailList);
            }

            IList<InspectResult> inspectResultList = null;
            if (rejectDetailList != null && rejectDetailList.Count > 0)
            {
                string selectInspectResultStatement = string.Empty;
                IList<object> selectInspectResultParams = new List<object>();
                foreach (RejectDetail rejectDetail in rejectDetailList)
                {
                    if (selectInspectResultStatement == string.Empty)
                    {
                        selectInspectResultStatement = "from InspectResult where Id in (?";
                    }
                    else
                    {
                        selectInspectResultStatement += ", ?";
                    }
                    selectInspectResultParams.Add(rejectDetail.InspectResultId);
                }
                selectInspectResultStatement += ")";

                inspectResultList = this.genericMgr.FindAll<InspectResult>(selectInspectResultStatement, selectInspectResultParams.ToArray());
            }
            #endregion

            IList<RejectDetail> resultRejectDetailList = new List<RejectDetail>();
            #region 更新不合格品明细
            if (updateRejectDetailList != null && updateRejectDetailList.Count > 0)
            {
                foreach (RejectDetail updateRejectDetail in updateRejectDetailList)
                {
                    #region 报验结果
                    InspectResult inspectResult = inspectResultList.Where(i => i.Id == updateRejectDetail.InspectResultId).Single();
                    inspectResult.HandleQty += updateRejectDetail.CurrentHandleQty - updateRejectDetail.HandleQty;
                    if (inspectResult.HandleQty > inspectResult.JudgeQty)
                    {
                        throw new BusinessException("不合格品的处理数超过了判定数。");
                    }

                    if (inspectResult.HandleQty < 0)
                    {
                        throw new TechnicalException("InspectResult's handle qty can't be negtive.");
                    }

                    if (inspectResult.HandleQty == inspectResult.JudgeQty)
                    {
                        inspectResult.IsHandle = true;
                    }
                    else
                    {
                        inspectResult.IsHandle = false;
                    }

                    genericMgr.Update(inspectResult);
                    #endregion

                    #region 更新不合格品明细
                    updateRejectDetail.HandleQty = updateRejectDetail.CurrentHandleQty;
                    genericMgr.Update(updateRejectDetail);
                    resultRejectDetailList.Add(updateRejectDetail);
                    #endregion
                }
            }
            #endregion

            #region 删除不合格品明细
            if (deleteRejectDetailList != null && deleteRejectDetailList.Count > 0)
            {
                foreach (RejectDetail deleteRejectDetail in deleteRejectDetailList)
                {
                    InspectResult inspectResult = inspectResultList.Where(i => i.Id == deleteRejectDetail.InspectResultId).Single();
                    inspectResult.HandleQty -= deleteRejectDetail.HandleQty;
                    if (inspectResult.HandleQty < 0)
                    {
                        throw new TechnicalException("InspectResult's handle qty can't be negtive.");
                    }
                    inspectResult.IsHandle = false;
                    genericMgr.Update(inspectResult);

                    genericMgr.Delete(deleteRejectDetail);
                }
            }
            #endregion

            return resultRejectDetailList;
        }
        #endregion

        #region 释放不合格品处理单
        [Transaction(TransactionMode.Requires)]
        public void ReleaseRejectMaster(string rejectNo)
        {
            ReleaseRejectMaster(genericMgr.FindById<RejectMaster>(rejectNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseRejectMaster(RejectMaster rejectMaster)
        {
            if (rejectMaster.Status != com.Sconit.CodeMaster.RejectStatus.Create)
            {
                throw new BusinessException("状态为{1}的不合格品处理单{0}不能释放。", rejectMaster.RejectNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.RejectStatus, (int)rejectMaster.Status));
            }

            rejectMaster.Status = CodeMaster.RejectStatus.Submit;
            this.genericMgr.Update(rejectMaster);

            #region 让步使用
            if (rejectMaster.HandleResult == CodeMaster.HandleResult.Concession)
            {
                ConcessionMaster concessionMaster = new ConcessionMaster();
                concessionMaster.RejectNo = rejectMaster.RejectNo;
                CreateConcessionMaster(concessionMaster);
                ReleaseConcessionMaster(concessionMaster);
                CloseConcessionMaster(concessionMaster, DateTime.Now);
            }
            #endregion
        }
        #endregion

        #region 关闭不合格品处理单
        [Transaction(TransactionMode.Requires)]
        public void CloseRejectMaster(RejectMaster rejectMaster)
        {
            if (rejectMaster.Status == com.Sconit.CodeMaster.RejectStatus.Create)
            {
                throw new BusinessException(string.Format("状态为{0}的不合格品单{1}不能关闭。", rejectMaster.Status, rejectMaster.RejectNo));
            }
            rejectMaster.Status = com.Sconit.CodeMaster.RejectStatus.Close;
            genericMgr.Update(rejectMaster);
        }


        #endregion

        #region 创建让步使用单
        [Transaction(TransactionMode.Requires)]
        public ConcessionMaster CreateConcessionMaster(IList<InspectResult> inspectResultList, string location = null)
        {
            var concessionMaster = new ConcessionMaster();
            if (inspectResultList == null || inspectResultList.Count() == 0)
            {
                throw new BusinessException("让步使用单明细不能为空。");
            }

            concessionMaster.ConcessionDetails =
                inspectResultList.Select(p => new ConcessionDetail
                {
                    BaseUom = p.BaseUom,
                    HuId = p.HuId,
                    Item = p.Item,
                    ItemDescription = p.ItemDescription,
                    LocationFrom = string.IsNullOrWhiteSpace(location) ? p.LocationFrom : location,
                    LocationTo = string.IsNullOrWhiteSpace(location) ? p.LocationFrom : location,
                    LotNo = p.LotNo,
                    Qty = p.CurrentHandleQty,
                    ReferenceItemCode = p.ReferenceItemCode,
                    UnitCount = p.UnitCount,
                    UnitQty = p.UnitQty,
                    Uom = p.Uom
                }).ToList();


            #region 创建让步使用单头
            InspectMaster inspectMaster = genericMgr.FindById<InspectMaster>(inspectResultList.First().InspectNo);
            concessionMaster.ConcessionNo = this.numberControlMgr.GetConcessionNo(concessionMaster);
            concessionMaster.Region = inspectMaster.Region;
            concessionMaster.Status = CodeMaster.ConcessionStatus.Create;
            //concessionMaster.RejectNo = inspectMaster.InspectNo;
            concessionMaster.ReferenceNo = inspectMaster.InspectNo;
            this.genericMgr.Create(concessionMaster);
            #endregion

            #region 创建让步使用单明细
            int seq = 1;
            foreach (var concessionDetail in concessionMaster.ConcessionDetails)
            {
                concessionDetail.Sequence = seq++;
                concessionDetail.ConcessionNo = concessionMaster.ConcessionNo;
                concessionDetail.LocationTo = string.IsNullOrWhiteSpace(location) ? concessionDetail.LocationFrom : location;
                this.genericMgr.Create(concessionDetail);
            }
            #endregion

            #region 更新明细状态
            foreach (var inspectResult in inspectResultList)
            {
                inspectResult.HandleQty += inspectResult.CurrentHandleQty;
                if (inspectResult.HandleQty == inspectResult.JudgeQty)
                {
                    inspectResult.IsHandle = true;
                }
                genericMgr.Update(inspectResult);
            }
            #endregion

            ReleaseConcessionMaster(concessionMaster);
            CloseConcessionMaster(concessionMaster, DateTime.Now);
            return concessionMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public ConcessionMaster CreateConcessionMaster(IList<RejectDetail> rejectDetailList, string location = null)
        {
            var concessionMaster = new ConcessionMaster();

            if (rejectDetailList == null || rejectDetailList.Count() == 0)
            {
                throw new BusinessException("让步使用单明细不能为空。");
            }

            IList<ConcessionDetail> concessionDetailList = new List<ConcessionDetail>();
            Mapper.Map(rejectDetailList, concessionDetailList);
            concessionMaster.ConcessionDetails = concessionDetailList;

            #region 创建让步使用单头
            RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(rejectDetailList.First().RejectNo);
            concessionMaster.ConcessionNo = this.numberControlMgr.GetConcessionNo(concessionMaster);
            concessionMaster.Region = rejectMaster.Region;
            concessionMaster.Status = CodeMaster.ConcessionStatus.Create;
            this.genericMgr.Create(concessionMaster);
            #endregion

            #region 创建让步使用单明细
            int seq = 1;
            foreach (var concessionDetail in concessionDetailList)
            {
                concessionDetail.Sequence = seq++;
                concessionDetail.ConcessionNo = concessionMaster.ConcessionNo;
                concessionDetail.LocationFrom = string.IsNullOrWhiteSpace(location) ? concessionDetail.LocationFrom : location;
                concessionDetail.LocationTo = string.IsNullOrWhiteSpace(location) ? concessionDetail.LocationFrom : location;
                this.genericMgr.Create(concessionDetail);
            }
            #endregion

            #region 更新明细的已处理不合格品处理单状态
            foreach (RejectDetail rejectDetail in rejectDetailList)
            {
                rejectDetail.HandledQty += rejectDetail.CurrentHandleQty;
                genericMgr.Update(rejectDetail);
            }
            string hql = "from RejectDetail as r where r.RejectNo = ?";
            IList<RejectDetail> remainRejectDetailList = genericMgr.FindAll<RejectDetail>(hql, rejectDetailList[0].RejectNo);
            var m = remainRejectDetailList.Where(r => (r.HandledQty < r.HandleQty)).ToList();
            if (m == null || m.Count == 0)
            {
                rejectMaster.Status = CodeMaster.RejectStatus.Close;
                genericMgr.Update(rejectMaster);
            }
            #endregion
            ReleaseConcessionMaster(concessionMaster);
            CloseConcessionMaster(concessionMaster, DateTime.Now);
            return concessionMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public ConcessionMaster CreateConcessionMaster(ConcessionMaster concessionMaster)
        {
            #region 检查
            string hql = "select r from RejectDetail r where r.RejectNo = ? ";
            IList<RejectDetail> rejectDetailList = this.genericMgr.FindAll<RejectDetail>(hql, concessionMaster.RejectNo);
            if (rejectDetailList == null || rejectDetailList.Count() == 0)
            {
                throw new BusinessException("让步使用单明细不能为空。");
            }

            foreach (var rejectDetail in rejectDetailList)
            {
                rejectDetail.CurrentHandleQty = rejectDetail.HandleQty;
            }

            IList<ConcessionDetail> concessionDetailList = new List<ConcessionDetail>();
            Mapper.Map(rejectDetailList, concessionDetailList);
            concessionMaster.ConcessionDetails = concessionDetailList;
            #endregion

            #region 创建让步使用单头
            RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(concessionMaster.RejectNo);
            concessionMaster.ConcessionNo = this.numberControlMgr.GetConcessionNo(concessionMaster);
            concessionMaster.Region = rejectMaster.Region;
            concessionMaster.Status = CodeMaster.ConcessionStatus.Create;
            this.genericMgr.Create(concessionMaster);
            #endregion

            #region 创建让步使用单明细
            int seq = 1;
            foreach (var concessionDetail in concessionDetailList)
            {
                concessionDetail.Sequence = seq++;
                concessionDetail.ConcessionNo = concessionMaster.ConcessionNo;
                concessionDetail.LocationTo = concessionDetail.LocationFrom;
                this.genericMgr.Create(concessionDetail);
            }
            #endregion

            return concessionMaster;
        }
        #endregion

        #region 释放让步使用单
        [Transaction(TransactionMode.Requires)]
        public void ReleaseConcessionMaster(string concessionNo)
        {
            ReleaseConcessionMaster(genericMgr.FindById<ConcessionMaster>(concessionNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseConcessionMaster(ConcessionMaster concessionMaster)
        {
            if (concessionMaster.Status != com.Sconit.CodeMaster.ConcessionStatus.Create)
            {
                throw new BusinessException("状态为{1}的让步使用单{0}不能释放。", concessionMaster.ConcessionNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.ConcessionStatus, ((int)concessionMaster.Status).ToString()));
            }

            #region 关闭不合格品处理单
            if (!string.IsNullOrWhiteSpace(concessionMaster.RejectNo))
            {
                RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(concessionMaster.RejectNo);
                //if (rejectMaster.Status != com.Sconit.CodeMaster.RejectStatus.Submit)
                //{
                //    throw new BusinessException("不合格品处理单{0}的状态为{1},不能释放。", rejectMaster.RejectNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.RejectStatus, ((int)rejectMaster.Status).ToString()));
                //}
                rejectMaster.Status = CodeMaster.RejectStatus.Close;
                this.genericMgr.Update(rejectMaster);
            }
            #endregion

            concessionMaster.Status = CodeMaster.ConcessionStatus.Submit;
            this.genericMgr.Update(concessionMaster);
        }
        #endregion

        #region 删除让步使用单
        [Transaction(TransactionMode.Requires)]
        public void DeleteConcessionMaster(string concessionNo)
        {
            ConcessionMaster concessionMaster = genericMgr.FindById<ConcessionMaster>(concessionNo);

            if (concessionMaster.Status != com.Sconit.CodeMaster.ConcessionStatus.Create)
            {
                throw new BusinessException("状态为{1}的让步使用单{0}不能删除。", concessionMaster.ConcessionNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.RejectStatus, ((int)concessionMaster.Status).ToString()));
            }

            IList<ConcessionDetail> concessionDetailList = TryLoadConcessionDetails(concessionMaster);
            if (concessionDetailList != null && concessionDetailList.Count > 0)
            {
                this.genericMgr.Delete<ConcessionDetail>(concessionDetailList);
            }

            this.genericMgr.Delete(concessionMaster);
        }

        #endregion

        #region 让步使用单关闭
        [Transaction(TransactionMode.Requires)]
        public void CloseConcessionMaster(string concessionNo)
        {
            CloseConcessionMaster(concessionNo, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseConcessionMaster(string concessionNo, DateTime effectiveDate)
        {
            ConcessionMaster concessionMaster = genericMgr.FindById<ConcessionMaster>(concessionNo);
            CloseConcessionMaster(concessionMaster, effectiveDate);
        }

        public void CloseConcessionMaster(ConcessionMaster concessionMaster, DateTime effectiveDate)
        {
            if (concessionMaster.Status != com.Sconit.CodeMaster.ConcessionStatus.Submit)
            {
                throw new BusinessException("状态为{1}的让步使用单{0}不能关闭。", concessionMaster.ConcessionNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.ConcessionStatus, ((int)concessionMaster.Status).ToString()));
            }

            concessionMaster.Status = CodeMaster.ConcessionStatus.Close;
            this.genericMgr.Update(concessionMaster);
            if (concessionMaster.ConcessionDetails == null || concessionMaster.ConcessionDetails.Count() == 0)
            {
                string hql = "from ConcessionDetail where ConcessionNo = ?";
                concessionMaster.ConcessionDetails = this.genericMgr.FindAll<ConcessionDetail>(hql, concessionMaster.ConcessionNo);
            }

            var invTrans = this.locationDetailMgr.ConcessionToUse(concessionMaster, effectiveDate);
            var huIds = invTrans.Where(p => !string.IsNullOrWhiteSpace(p.HuId)).Select(p => p.HuId).Distinct();
            if (huIds != null && huIds.Count() > 0)
            {
                foreach (var huId in huIds)
                {
                    var hu = this.genericMgr.FindById<Hu>(huId);
                    hu.ConcessionCount++;
                    this.genericMgr.Update(hu);
                }
            }
        }
        #endregion

        #region 工废
        [Transaction(TransactionMode.Requires)]
        public void CreateWorkersWaste(InspectMaster inspectMaster)
        {
            CreateWorkersWaste(inspectMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateWorkersWaste(InspectMaster inspectMaster, DateTime effectiveDate)
        {
            #region 创建报验单
            CreateInspectMaster(inspectMaster, effectiveDate);
            #endregion

            #region 判定不合格
            foreach (InspectDetail inspectDetail in inspectMaster.InspectDetails)
            {
                inspectDetail.CurrentQty = inspectDetail.InspectQty;
                inspectDetail.FailCode = "6";
            }
            JudgeInspectDetail(inspectMaster.InspectDetails, effectiveDate);
            #endregion

            #region 创建不合格品处理单
            IList<InspectResult> inspectResultList = genericMgr.FindAll<InspectResult>("from InspectResult as r where r.InspectNo = ?", inspectMaster.InspectNo);
            foreach (InspectResult inspectResult in inspectResultList)
            {
                inspectResult.CurrentHandleQty = inspectResult.JudgeQty;
                inspectResult.RejectHandleResult = com.Sconit.CodeMaster.HandleResult.WorkersWaste;
            }
            RejectMaster rejectMaster = CreateRejectMaster(com.Sconit.CodeMaster.HandleResult.WorkersWaste, inspectResultList);
            #endregion
            #region 释放不合格品处理单
            rejectMaster.Status = com.Sconit.CodeMaster.RejectStatus.Submit;
            genericMgr.Update(rejectMaster);
            #endregion
        }
        #endregion

        public InspectMaster TransferReceipt2Inspect(ReceiptMaster receiptMaster)
        {
            List<ReceiptLocationDetail> inspectReceiptLocationDetailList = new List<ReceiptLocationDetail>();
            foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
            {
                if (receiptDetail.IsInspect)
                {
                    inspectReceiptLocationDetailList.AddRange(receiptDetail.ReceiptLocationDetails);
                }
            }

            if (inspectReceiptLocationDetailList != null && inspectReceiptLocationDetailList.Count > 0)
            {
                #region 报验单头
                InspectMaster inspectMaster = new InspectMaster();

                inspectMaster.IpNo = receiptMaster.IpNo;
                inspectMaster.ReceiptNo = receiptMaster.ReceiptNo;
                inspectMaster.Region = receiptMaster.PartyTo;
                inspectMaster.Status = com.Sconit.CodeMaster.InspectStatus.Submit;
                //inspectMaster.Type = receiptMaster.CreateHuOption == CodeMaster.CreateHuOption.Receive || receiptMaster.IsReceiveScanHu ? com.Sconit.CodeMaster.InspectType.Barcode : com.Sconit.CodeMaster.InspectType.Quantity;
                inspectMaster.Type = inspectReceiptLocationDetailList.Where(locDet => !string.IsNullOrWhiteSpace(locDet.HuId)).Count() > 0 ? com.Sconit.CodeMaster.InspectType.Barcode : com.Sconit.CodeMaster.InspectType.Quantity;
                inspectMaster.IsATP = true;
                inspectMaster.WMSNo = receiptMaster.WMSNo;
                inspectMaster.PartyFrom = receiptMaster.PartyFrom;
                inspectMaster.PartyFromName = receiptMaster.PartyFromName;
                #endregion

                #region 根据收货明细+条码+WMS行号汇总
                var groupedInspectReceiptLocationDetailList = from locDet in inspectReceiptLocationDetailList
                                                              group locDet by new
                                                              {
                                                                  ReceiptDetailId = locDet.ReceiptDetailId,
                                                                  HuId = locDet.HuId,
                                                                  LotNo = locDet.LotNo,
                                                                  WMSSeq = locDet.WMSSeq,
                                                                  IsConsignment = locDet.IsConsignment,
                                                                  PlanBill = locDet.PlanBill,
                                                              } into gj
                                                              select new
                                                              {
                                                                  ReceiptDetailId = gj.Key.ReceiptDetailId,
                                                                  HuId = gj.Key.HuId,
                                                                  LotNo = gj.Key.LotNo,
                                                                  WMSSeq = gj.Key.WMSSeq,
                                                                  IsConsignment = gj.Key.IsConsignment,
                                                                  PlanBill = gj.Key.PlanBill,
                                                                  ReceiveQty = gj.Sum(locDet => locDet.Qty),   //基本单位
                                                              };
                #endregion

                #region 报验单明细
                foreach (var groupedInspectReceiptLocationDetail in groupedInspectReceiptLocationDetailList)
                {
                    ReceiptDetail receiptDetail = receiptMaster.ReceiptDetails.Where(det => det.Id == groupedInspectReceiptLocationDetail.ReceiptDetailId).Single();


                    InspectDetail inspectDetail = new InspectDetail();
                    inspectDetail.Item = receiptDetail.Item;
                    inspectDetail.ItemDescription = receiptDetail.ItemDescription;
                    inspectDetail.ReferenceItemCode = receiptDetail.ReferenceItemCode;
                    inspectDetail.BaseUom = receiptDetail.BaseUom;
                    inspectDetail.HuId = groupedInspectReceiptLocationDetail.HuId;
                    inspectDetail.LotNo = groupedInspectReceiptLocationDetail.LotNo;
                    inspectDetail.Uom = receiptDetail.Uom;
                    inspectDetail.UnitCount = receiptDetail.UnitCount;
                    inspectDetail.UnitQty = receiptDetail.UnitQty;
                    inspectDetail.LocationFrom = receiptDetail.LocationTo;
                    inspectDetail.CurrentLocation = receiptDetail.LocationTo;
                    inspectDetail.InspectQty = groupedInspectReceiptLocationDetail.ReceiveQty / inspectDetail.UnitQty;
                    inspectDetail.IsJudge = false;
                    inspectDetail.IpDetailSequence = receiptDetail.IpDetailSequence;
                    inspectDetail.ReceiptDetailSequence = receiptDetail.Sequence;
                    inspectDetail.WMSSeq = groupedInspectReceiptLocationDetail.WMSSeq;
                    inspectDetail.IsConsignment = groupedInspectReceiptLocationDetail.IsConsignment;
                    inspectDetail.PlanBill = groupedInspectReceiptLocationDetail.PlanBill;

                    inspectMaster.AddInspectDetail(inspectDetail);
                }
                #endregion

                return inspectMaster;
            }
            else
            {
                return null;
            }
        }

        private void TryCloseInspectMaster(InspectMaster inspectMaster)
        {
            string hql = "select count(*) as counter from InspectDetail where InspectNo = ? and IsJudge = ?";
            if (this.genericMgr.FindAll<long>(hql, new object[] { inspectMaster.InspectNo, false })[0] == 0)
            {
                inspectMaster.Status = CodeMaster.InspectStatus.Close;
                this.genericMgr.Update(inspectMaster);
            }
        }

        private IList<ConcessionDetail> TryLoadConcessionDetails(ConcessionMaster concessionMaster)
        {
            if (!string.IsNullOrWhiteSpace(concessionMaster.ConcessionNo))
            {
                if (concessionMaster.ConcessionDetails == null)
                {
                    string hql = "from ConcessionDetail where ConcessionNo = ?";

                    concessionMaster.ConcessionDetails = this.genericMgr.FindAll<ConcessionDetail>(hql, concessionMaster.ConcessionNo);
                }

                return concessionMaster.ConcessionDetails;
            }
            else
            {
                return null;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void SaveInspectResult(IList<InspectResult> inspectResultList)
        {
            if (inspectResultList == null || inspectResultList.Count == 0)
            {
                throw new BusinessException("没有需要更改的明细");
            }
            var failCodeDic = this.genericMgr.FindAll<FailCode>().ToDictionary(d => d.Code, d => d);
            foreach (var inspectResult in inspectResultList)
            {
                var oldHandleResult = failCodeDic[inspectResult.FailCode].HandleResult;
                var currentHandleResult = failCodeDic[inspectResult.CurrentFailCode].HandleResult;
                if (oldHandleResult == CodeMaster.HandleResult.Qualify)
                {
                    continue;
                }
                if (currentHandleResult == CodeMaster.HandleResult.Qualify)
                {
                    throw new BusinessException("已经判定为不合格的判定结果明细不能变更为合格");
                }
                inspectResult.FailCode = inspectResult.CurrentFailCode;
                inspectResult.RejectHandleResult = currentHandleResult;

                if (inspectResult.CurrentIsHandle != inspectResult.IsHandle)
                {
                    inspectResult.HandleQty = inspectResult.CurrentIsHandle ? inspectResult.JudgeQty : 0;
                    inspectResult.IsHandle = inspectResult.CurrentIsHandle;
                }

                this.genericMgr.UpdateWithTrim(inspectResult);
            }
        }
    }
}
