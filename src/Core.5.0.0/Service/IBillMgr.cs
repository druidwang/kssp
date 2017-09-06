using System;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.ORD;
using System.Collections.Generic;
using com.Sconit.Entity.VIEW;

namespace com.Sconit.Service
{
    public interface IBillMgr
    {
        PlanBill CreatePlanBill(ReceiptDetail receiptDetail, ReceiptDetailInput receiptDetailInput);
        PlanBill CreatePlanBill(ReceiptDetail receiptDetail, ReceiptDetailInput receiptDetailInput, DateTime effectiveDate);
        SettleBillTransaction SettleBill(PlanBill planBill);
        SettleBillTransaction SettleBill(PlanBill planBill, DateTime effectiveDate);
        List<SettleBillTransaction> SettleBillList(IList<PlanBill> planBillList);
        SettleBillTransaction VoidSettleBill(ActingBill actingBill, PlanBill planBill, bool IsVoidPlanBill);
        void VoidPlanBill(PlanBill planBill);
        PlanBill CreatePlanBill(MiscOrderMaster miscOrderMaster, MiscOrderDetail miscOrderDetail, MiscOrderLocationDetail miscOrderLocationDetail, DateTime effectiveDate);

        /// <summary>
        /// 增加账单明细
        /// </summary>
        /// <param name="billNo">账单号</param>
        /// <param name="actingBillList">待开票明细</param>
        void AddBillDetail(string billNo, IList<ActingBill> actingBillList);

        /// <summary>
        /// 取消/冲销账单
        /// </summary>
        /// <param name="billNo">账单号</param>
        /// <param name="effectiveDate">生效日期</param>
        void CancelBill(string billNo, DateTime effectiveDate);

        /// <summary>
        /// 关闭账单
        /// </summary>
        /// <param name="billNo">账单号</param>
        void CloseBill(string billNo);

        /// <summary>
        /// 创建账单
        /// </summary>
        /// <param name="actingBillList">待开票明细</param>
        /// <param name="effectiveDate">生效日期</param>
        /// <param name="externalBillNo">外部账单号</param>
        /// <returns>账单对象列表(带明细)</returns>
        IList<BillMaster> CreateBill(IList<ActingBill> actingBillList);

        /// <summary>
        /// 删除账单
        /// </summary>
        /// <param name="billNo"></param>
        void DeleteBill(string billNo);

        /// <summary>
        /// 删除账单明细
        /// </summary>
        /// <param name="billDetailList">账单明细</param>
        void DeleteBillDetail(IList<BillDetail> billDetailList);

        /// <summary>
        /// 释放账单
        /// </summary>
        /// <param name="billNo">账单号</param>
        /// <param name="effectiveDate">生效时间</param>
        void ReleaseBill(string billNo, DateTime effectiveDate);

        /// <summary>
        /// 更新账单
        /// </summary>
        /// <param name="bill">账单对象,需带明细</param>
        void UpdateBill(BillMaster bill);

        /// <summary>
        /// 重新计价
        /// </summary>
        /// <param name="actingBillList"></param>
        /// <param name="efftiveDate"></param>
        void RecalculatePrice(IList<ActingBill> actingBillList);

        void GroupActingBillByItem(ref IList<ActingBill> actingBills);

        void GroupBillDetailByItem(ref IList<BillDetail> billDetails);

        //void GroupActingBill(IList<ActingBill> actingBillList);

        void GroupBillDetail(ref IList<BillDetail> billDetails);

        void SaveBill(string billNo, string externalBillNo, string referenceBillNo, string invoiceBillNo, DateTime invoiceDate);

        IList<ActingBill> GetRecalculatePrice(CodeMaster.BillType billType, string party, string flow,
            string receiptNo, string externalReceiptNo, string item, string currency, DateTime startDate, DateTime endDate, bool includeNoEstPrice);

        IList<BillIOB> GetBillIOB(CodeMaster.BillType billType, string party, string location, string item,
            DateTime startDate, DateTime endDate);

        void MergePlanBill(IList<HuStatus> huStatusList);
    }
}
