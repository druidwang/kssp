using System;
using System.Collections.Generic;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SCM;

namespace com.Sconit.Service
{
    public interface IInspectMgr
    {
        #region 创建报验单
        void CreateInspectMaster(InspectMaster inspectMaster);

        void CreateInspectMaster(InspectMaster inspectMaster, DateTime effectiveDate);
        #endregion

        #region 报验单扫描条码
        IList<InspectDetail> AddInspectDetail(string HuId, IList<InspectDetail> InspectDetailDetailList);
        #endregion

        #region 报验立即判定不合格
        void CreateAndReject(InspectMaster inspectMaster);
        void CreateAndReject(InspectMaster inspectMaster, DateTime effectiveDate);
        #endregion

        #region 报验判定
        void JudgeInspectDetail(IList<InspectDetail> inspectDetailList);

        void JudgeInspectDetail(IList<InspectDetail> inspectDetailList, DateTime effectiveDate);
        #endregion

        #region 创建不合格品处理单
        RejectMaster CreateRejectMaster(CodeMaster.HandleResult rejectHandleResult, IList<InspectResult> inspectResultList);

        RejectMaster CreateRejectMaster(CodeMaster.HandleResult rejectHandleResult, IList<InspectResult> inspectResultList, DateTime effectiveDate);
        #endregion

        #region 添加不合格品处理单明细
        IList<RejectDetail> AddRejectDetails(string rejectNo, IList<InspectResult> inspectResultList);
        #endregion

        #region 更新不合格品处理单明细
        IList<RejectDetail> BatchUpdateRejectDetails(string rejectNo, IList<RejectDetail> updateRejectDetailList, IList<RejectDetail> deleteRejectDetailList);
        #endregion

        #region 释放不合格品处理单
        void ReleaseRejectMaster(string rejectNo);

        void ReleaseRejectMaster(RejectMaster rejectMaster);
        #endregion

        #region 关闭不合格品处理单
        void CloseRejectMaster(RejectMaster rejectMaster);
        #endregion

        #region 收货单转报验单
        InspectMaster TransferReceipt2Inspect(ReceiptMaster receiptMaster);
        #endregion

        #region 让步使用单创建
        ConcessionMaster CreateConcessionMaster(IList<InspectResult> inspectResultList, string location = null);

        ConcessionMaster CreateConcessionMaster(IList<RejectDetail> rejectDetailList, string location = null);

        ConcessionMaster CreateConcessionMaster(ConcessionMaster concessionMaster);
        #endregion

        #region 让步使用单删除
        void DeleteConcessionMaster(string concessionNo);
        #endregion

        #region 让步使用单释放
        void ReleaseConcessionMaster(string concessionNo);

        void ReleaseConcessionMaster(ConcessionMaster concessionMaster);
        #endregion

        #region 让步使用单关闭
        void CloseConcessionMaster(string concessionNo);

        void CloseConcessionMaster(string concessionNo, DateTime effectiveDate);
        #endregion

        #region 工废
        void CreateWorkersWaste(InspectMaster inspectMaster);

        void CreateWorkersWaste(InspectMaster inspectMaster, DateTime effectiveDate);
        #endregion

        #region InspectResult

        void SaveInspectResult(IList<InspectResult> inspectResultList);

        #endregion
    }
}
