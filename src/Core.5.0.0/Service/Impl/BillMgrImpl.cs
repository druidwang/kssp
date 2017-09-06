using System;
using System.Linq;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.MD;
using System.Collections.Generic;
using NHibernate.Criterion;
using System.Text;
using com.Sconit.Entity.VIEW;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class BillMgrImpl : BaseMgr, IBillMgr
    {
        #region 变量
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        #endregion

        #region Public Methods
        [Transaction(TransactionMode.Requires)]
        public PlanBill CreatePlanBill(ReceiptDetail receiptDetail, ReceiptDetailInput receiptDetailInput)
        {
            return CreatePlanBill(receiptDetail, receiptDetailInput, DateTime.Now);
        }

        public PlanBill CreatePlanBill(ReceiptDetail receiptDetail, ReceiptDetailInput receiptDetailInput, DateTime effectiveDate)
        {
            PlanBill planBill = new PlanBill();
            planBill.OrderNo = receiptDetail.OrderNo;
            planBill.IpNo = receiptDetail.IpNo;
            //planBill.ExternalIpNo = receiptDetail.ExternalIpNo;
            planBill.ReceiptNo = receiptDetail.ReceiptNo;
            planBill.ExternalReceiptNo = receiptDetail.CurrentExternalReceiptNo;
            if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement
                || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract
                || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.CustomerGoods
                || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.ScheduleLine)
            {
                planBill.Type = com.Sconit.CodeMaster.BillType.Procurement;
                if (receiptDetail.OrderSubType == CodeMaster.OrderSubType.Normal)
                {
                    planBill.LocationFrom = receiptDetail.LocationTo;
                    planBill.Party = receiptDetail.CurrentPartyFrom;
                    planBill.PartyName = receiptDetail.CurrentPartyFromName;
                }
                else
                {
                    planBill.LocationFrom = receiptDetail.LocationFrom;
                    planBill.Party = receiptDetail.CurrentPartyTo;
                    planBill.PartyName = receiptDetail.CurrentPartyToName;
                }
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Distribution)
            {
                planBill.Type = com.Sconit.CodeMaster.BillType.Distribution;
                if (receiptDetail.OrderSubType == CodeMaster.OrderSubType.Normal)
                {
                    planBill.LocationFrom = receiptDetail.LocationFrom;
                    planBill.Party = receiptDetail.CurrentPartyTo;
                    planBill.PartyName = receiptDetail.CurrentPartyToName;
                }
                else
                {
                    planBill.LocationFrom = receiptDetail.LocationTo;
                    planBill.Party = receiptDetail.CurrentPartyFrom;
                    planBill.PartyName = receiptDetail.CurrentPartyFromName;
                }
            }
            planBill.Item = receiptDetail.Item;
            planBill.ItemDescription = receiptDetail.ItemDescription;

            planBill.Uom = receiptDetail.Uom;
            planBill.UnitCount = receiptDetail.UnitCount;
            planBill.BillTerm = receiptDetail.BillTerm;
            planBill.BillAddress = receiptDetail.BillAddress;
            //planBill.BillAddressDescription = receiptDetail.BillAddressDescription;
            planBill.PriceList = receiptDetail.PriceList;
            planBill.Currency = receiptDetail.Currency;
            planBill.UnitPrice = receiptDetail.UnitPrice.HasValue ? receiptDetail.UnitPrice.Value : 0;
            planBill.IsProvisionalEstimate = receiptDetail.UnitPrice.HasValue ? receiptDetail.IsProvisionalEstimate : false;
            planBill.Tax = receiptDetail.Tax;
            planBill.IsIncludeTax = receiptDetail.IsIncludeTax;
            if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
            {
                //planBill.PlanQty = receiptDetailInput.ReceiveQty / receiptDetail.UnitQty;
                planBill.PlanQty = receiptDetailInput.ReceiveQty;
            }
            else
            {
                //planBill.PlanQty = -receiptDetailInput.ReceiveQty / receiptDetail.UnitQty;
                planBill.PlanQty = -receiptDetailInput.ReceiveQty;
            }
            planBill.PlanAmount = planBill.UnitPrice * planBill.PlanQty;
            planBill.UnitQty = receiptDetail.UnitQty;
            planBill.HuId = receiptDetailInput.HuId;
            planBill.EffectiveDate = effectiveDate;
            planBill.Flow = receiptDetail.Flow;
            planBill.ReferenceItemCode = receiptDetail.ReferenceItemCode;

            this.genericMgr.Create(planBill);

            this.RecordPlanBillTransaction(planBill, effectiveDate, receiptDetail.Id, false);

            return planBill;
        }

        public PlanBill CreatePlanBill(MiscOrderMaster miscOrderMaster, MiscOrderDetail miscOrderDetail, MiscOrderLocationDetail miscOrderLocationDetail, DateTime effectiveDate)
        {
            PlanBill planBill = new PlanBill();
            planBill.OrderNo = miscOrderMaster.MiscOrderNo;
            planBill.ReceiptNo = miscOrderMaster.MiscOrderNo;
            planBill.ExternalReceiptNo = miscOrderMaster.ReferenceNo;
            planBill.Type = com.Sconit.CodeMaster.BillType.Procurement;
            planBill.Party = miscOrderDetail.ManufactureParty;
            planBill.Item = miscOrderDetail.Item;
            planBill.ItemDescription = miscOrderDetail.ItemDescription;
            planBill.Uom = miscOrderDetail.Uom;
            planBill.UnitCount = miscOrderDetail.UnitCount;
            planBill.BillTerm = CodeMaster.OrderBillTerm.OnlineBilling;
            PartyAddress partyAddress = this.genericMgr.FindAll<PartyAddress>(
                "from PartyAddress as pa where pa.Party = ? and pa.Type = ? order by IsPrimary desc,Sequence asc ",
                new object[] { planBill.Party, CodeMaster.AddressType.BillAddress }, 0, 1).FirstOrDefault();
            if (partyAddress == null)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.TheBillAddressNotSpecialSupplier, planBill.Party);
            }
            planBill.BillAddress = partyAddress.Address.Code;
            planBill.PlanAmount = 0;
            int refId = 0;
            if (miscOrderMaster.IsScanHu)
            {
                planBill.PlanQty = miscOrderLocationDetail.Qty;
                planBill.UnitQty = miscOrderDetail.UnitQty;
                planBill.HuId = miscOrderLocationDetail.HuId;
                refId = miscOrderLocationDetail.Id;
            }
            else
            {
                planBill.PlanQty = miscOrderDetail.Qty;
                planBill.UnitQty = miscOrderDetail.UnitQty;
                refId = miscOrderDetail.Id;
            }
            planBill.LocationFrom = string.IsNullOrWhiteSpace(miscOrderDetail.Location) ? miscOrderMaster.Location : miscOrderDetail.Location;
            planBill.EffectiveDate = effectiveDate;
            this.genericMgr.Create(planBill);

            this.RecordPlanBillTransaction(planBill, effectiveDate, refId, false);
            return planBill;
        }

        [Transaction(TransactionMode.Requires)]
        public List<SettleBillTransaction> SettleBillList(IList<PlanBill> planBillList)
        {
            DateTime dateTimeNow = DateTime.Now;
            var settleBillTransactionList = new List<SettleBillTransaction>();
            foreach (var planBill in planBillList)
            {
                settleBillTransactionList.Add(SettleBill(planBill, dateTimeNow));
            }
            return settleBillTransactionList;
        }

        [Transaction(TransactionMode.Requires)]
        public SettleBillTransaction SettleBill(PlanBill planBill)
        {
            return SettleBill(planBill, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public SettleBillTransaction SettleBill(PlanBill planBill, DateTime effectiveDate)
        {
            ////重新计价
            //PriceListDetail priceListDetail =
            //        this.itemMgr.GetItemPrice
            //        (planBill.Item, planBill.Uom, planBill.PriceList, planBill.Currency, planBill.EffectiveDate);

            //if (priceListDetail != null &&//正式价不能更新成暂估价
            //    (!priceListDetail.IsProvisionalEstimate || (priceListDetail.IsProvisionalEstimate && planBill.IsProvisionalEstimate)))
            //{
            //    planBill.IsIncludeTax = priceListDetail.PriceList.IsIncludeTax;
            //    planBill.Tax = priceListDetail.PriceList.Tax;
            //    planBill.IsProvisionalEstimate = priceListDetail.IsProvisionalEstimate;
            //    planBill.UnitPrice = priceListDetail.UnitPrice;
            //}
            //校验，已结算数+本次结算数不能大于总结算数量，可能有负数结算，所以要用绝对值比较
            if (Math.Abs(planBill.ActingQty + planBill.CurrentActingQty) > Math.Abs(planBill.PlanQty))
            {
                throw new BusinessException(Resources.BIL.Bill.Errors_ActingQtyExceedPlanQty, planBill.Item);
            }

            ActingBill actingBill = this.RetriveActingBill(planBill, effectiveDate.Date);

            if (actingBill.Id == 0)
            {
                this.genericMgr.Create(actingBill);
            }
            else
            {
                this.genericMgr.Update(actingBill);
            }
            ////

            #region 更新Planed Bill的已结算数量
            planBill.ActingQty += planBill.CurrentActingQty;
            planBill.ActingAmount += planBill.UnitPrice * planBill.CurrentActingQty;
            planBill.IsClose = (planBill.ActingQty != planBill.PlanQty);
            this.genericMgr.Update(planBill);
            #endregion

            #region 记BillTransaction
            return RecordSettleBillTransaction(planBill, actingBill, effectiveDate, false);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public SettleBillTransaction VoidSettleBill(ActingBill actingBill, PlanBill planBill, bool voidPlanBill)
        {
            #region 更新ActingBill
            if (Math.Abs(actingBill.BillQty) < Math.Abs(actingBill.BillingQty + actingBill.VoidQty + actingBill.CurrentVoidQty))
            {
                //待开票数量不足，不能冲销结算
                throw new BusinessException(Resources.BIL.Bill.Errors_VoidActBillFailActQtyNotEnough, actingBill.Item);
            }

            actingBill.VoidQty += actingBill.CurrentVoidQty;
            actingBill.IsClose = (actingBill.BillQty == (actingBill.BillingQty + actingBill.VoidQty));
            this.genericMgr.Update(actingBill);
            #endregion

            #region 反向更新PlanBill
            if (planBill == null)
            {
                planBill = this.genericMgr.FindById<PlanBill>(actingBill.PlanBill);
            }

            planBill.ActingQty -= actingBill.CurrentVoidQty;

            if (Math.Abs(planBill.ActingQty + planBill.VoidQty) > Math.Abs(planBill.PlanQty))
            {
                //冲销的数量大于待开票数量
                throw new BusinessException(Resources.BIL.Bill.Errors_VoidActingQtyExceedPlanQty, planBill.Item);
            }

            if (voidPlanBill)
            {
                if (Math.Abs(planBill.ActingQty + planBill.VoidQty + actingBill.CurrentVoidQty) > Math.Abs(planBill.PlanQty))
                {
                    //已经结算，不能冲销寄售信息
                    throw new BusinessException(Resources.BIL.Bill.Errors_VoidPlanBillFailPlanQtyNotEnough, planBill.Item);
                }

                planBill.VoidQty += actingBill.CurrentVoidQty;
            }

            if (planBill.PlanQty == (planBill.ActingQty + planBill.VoidQty))
            {
                planBill.IsClose = true;
            }
            else
            {
                planBill.IsClose = false;
            }

            this.genericMgr.Update(planBill);
            #endregion

            #region 记录库存事务
            planBill.CurrentVoidQty = actingBill.CurrentVoidQty;
            return this.RecordSettleBillTransaction(planBill, actingBill, actingBill.EffectiveDate, true);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void VoidPlanBill(PlanBill planBill)
        {
            if (Math.Abs(planBill.ActingQty + planBill.VoidQty + planBill.CurrentVoidQty) > Math.Abs(planBill.PlanQty))
            {
                //已经结算，不能冲销寄售信息
                throw new BusinessException(Resources.BIL.Bill.Errors_VoidPlanBillFailPlanQtyNotEnough, planBill.Item);
            }

            planBill.VoidQty += planBill.CurrentVoidQty;

            if (planBill.PlanQty == (planBill.ActingQty + planBill.VoidQty))
            {
                planBill.IsClose = true;
            }
            else
            {
                planBill.IsClose = false;
            }
            this.genericMgr.Update(planBill);
            this.RecordPlanBillTransaction(planBill, DateTime.Now, planBill.Id, true);
        }

        #endregion

        #region Private Methods

        private SettleBillTransaction RecordSettleBillTransaction(PlanBill planBill, ActingBill actingBill, DateTime effectiveDate, bool isVoid)
        {
            #region 记BillTransaction
            SettleBillTransaction billTransaction = new SettleBillTransaction();

            billTransaction.OrderNo = planBill.OrderNo;
            billTransaction.IpNo = planBill.IpNo;
            billTransaction.ExternalIpNo = planBill.ExternalIpNo;
            billTransaction.ReceiptNo = planBill.ReceiptNo;
            billTransaction.ExternalReceiptNo = planBill.ExternalReceiptNo;
            billTransaction.IsIncludeTax = planBill.IsIncludeTax;
            billTransaction.Item = planBill.Item;
            billTransaction.ItemDescription = planBill.ItemDescription;
            billTransaction.Uom = planBill.Uom;
            billTransaction.UnitCount = planBill.UnitCount;
            billTransaction.HuId = planBill.HuId;
            //billTransaction.TransactionType =
            //    planBill.Type == com.Sconit.CodeMaster.BillType.Procurement ?
            //    (isVoid ? com.Sconit.CodeMaster.BillTransactionType.POSettleVoid : com.Sconit.CodeMaster.BillTransactionType.POSettle) :
            //    (isVoid ? com.Sconit.CodeMaster.BillTransactionType.SOSettleVoid : com.Sconit.CodeMaster.BillTransactionType.SOSettle);
            //billTransaction.BillAddress = planBill.BillAddress;
            //billTransaction.BillAddressDescription = planBill.BillAddressDescription;
            billTransaction.Party = planBill.Party;
            billTransaction.PartyName = planBill.PartyName;
            billTransaction.PriceList = planBill.PriceList;
            billTransaction.Currency = planBill.Currency;
            billTransaction.UnitPrice = planBill.UnitPrice;
            billTransaction.IsProvisionalEstimate = planBill.IsProvisionalEstimate;
            billTransaction.Tax = planBill.Tax;

            #region 记录数量
            decimal qty = isVoid ? planBill.CurrentVoidQty : planBill.CurrentActingQty;
            //billTransaction.BillQty = (isVoid ? -1 : 1)      //冲销为负数
            //        * (planBill.Type == com.Sconit.CodeMaster.BillType.Procurement ? -1 * qty : qty);  //采购付款为负数
            if (planBill.Type == CodeMaster.BillType.Procurement)
            {
                if (isVoid)
                {
                    //采购结算冲销负数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.POSettleVoid;
                    billTransaction.BillQty = -qty;
                    billTransaction.BillAmount = billTransaction.BillQty * billTransaction.UnitPrice;
                }
                else
                {
                    //采购结算正数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.POSettle;
                    billTransaction.BillQty = qty;
                    billTransaction.BillAmount = billTransaction.BillQty * billTransaction.UnitPrice;
                }
            }
            else
            {
                if (isVoid)
                {
                    //销售开票冲销负数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.SOSettleVoid;
                    billTransaction.BillQty = -qty;
                    billTransaction.BillAmount = billTransaction.BillQty * billTransaction.UnitPrice;
                }
                else
                {
                    //销售开票正数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.SOSettle;
                    billTransaction.BillQty = qty;
                    billTransaction.BillAmount = billTransaction.BillQty * billTransaction.UnitPrice;
                }
            }

            #endregion

            billTransaction.UnitQty = planBill.UnitQty;
            //
            billTransaction.LocationFrom = planBill.LocationFrom;
            billTransaction.SettleLocation = planBill.CurrentLocation;

            billTransaction.EffectiveDate = effectiveDate;
            billTransaction.PlanBill = planBill.Id;
            billTransaction.ActingBill = actingBill.Id;

            billTransaction.BillTerm = actingBill.BillTerm;

            User user = SecurityContextHolder.Get();
            billTransaction.CreateUserId = user.Id;
            billTransaction.CreateUserName = user.FullName;
            billTransaction.CreateDate = DateTime.Now;

            this.genericMgr.Create(billTransaction);

            return billTransaction;
            #endregion
        }

        private void RecordBillTransaction(BillDetail billDetail, DateTime effectiveDate, bool isCancel)
        {
            DateTime dateTimeNow = DateTime.Now;

            BillTransaction billTransaction = new BillTransaction();
            billTransaction.ActingBill = billDetail.ActingBillId;
            billTransaction.BillDetail = billDetail.Id;
            billTransaction.Currency = billDetail.Currency;
            billTransaction.EffectiveDate = effectiveDate;
            billTransaction.ExternalIpNo = billDetail.ExternalIpNo;
            billTransaction.ExternalReceiptNo = billDetail.ExternalReceiptNo;
            billTransaction.IpNo = billDetail.IpNo;
            billTransaction.IsIncludeTax = billDetail.IsIncludeTax;
            billTransaction.IsProvisionalEstimate = billDetail.IsProvisionalEstimate;
            billTransaction.Item = billDetail.Item;
            billTransaction.ItemDescription = billDetail.ItemDescription;
            billTransaction.LocationFrom = billDetail.LocationFrom;
            billTransaction.OrderNo = billDetail.OrderNo;
            billTransaction.Party = billDetail.Party;
            billTransaction.PartyName = billDetail.PartyName;
            billTransaction.PriceList = billDetail.PriceList;
            billTransaction.ReceiptNo = billDetail.ReceiptNo;
            billTransaction.UnitCount = billDetail.UnitCount;
            billTransaction.UnitPrice = billDetail.UnitPrice;
            billTransaction.Uom = billDetail.Uom;
            billTransaction.UnitQty = billDetail.UnitQty;

            if (billDetail.Type == CodeMaster.BillType.Procurement)
            {
                if (isCancel)
                {
                    //采购开票冲销负数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.POBilledVoid;
                    billTransaction.BillQty = -billDetail.Qty;
                    billTransaction.BillAmount = -billDetail.Amount;
                }
                else
                {
                    //采购开票正数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.POBilled;
                    billTransaction.BillQty = billDetail.Qty;
                    billTransaction.BillAmount = billDetail.Amount;
                }
            }
            else
            {
                if (isCancel)
                {
                    //销售开票冲销负数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.SOBilledVoid;
                    billTransaction.BillQty = -billDetail.Qty;
                    billTransaction.BillAmount = -billDetail.Amount;
                }
                else
                {
                    //销售开票正数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.SOBilled;
                    billTransaction.BillQty = billDetail.Qty;
                    billTransaction.BillAmount = billDetail.Amount;
                }
            }
            User user = SecurityContextHolder.Get();
            billTransaction.CreateUserId = user.Id;
            billTransaction.CreateUserName = user.FullName;
            billTransaction.CreateDate = DateTime.Now;
            this.genericMgr.Create(billTransaction);
        }

        private void RecordPlanBillTransaction(PlanBill planBill, DateTime effectiveDate, int refId, bool isVoid)
        {
            #region 记BillTransaction
            PlanBillTransaction billTransaction = new PlanBillTransaction();

            billTransaction.OrderNo = planBill.OrderNo;
            billTransaction.IpNo = planBill.IpNo;
            billTransaction.ExternalIpNo = planBill.ExternalIpNo;
            billTransaction.ReceiptNo = planBill.ReceiptNo;
            billTransaction.ExternalReceiptNo = planBill.ExternalReceiptNo;
            billTransaction.IsIncludeTax = planBill.IsIncludeTax;
            billTransaction.Item = planBill.Item;
            billTransaction.ItemDescription = planBill.ItemDescription;
            billTransaction.Uom = planBill.Uom;
            billTransaction.UnitCount = planBill.UnitCount;
            billTransaction.HuId = planBill.HuId;
            billTransaction.Party = planBill.Party;
            billTransaction.PartyName = planBill.PartyName;
            billTransaction.PriceList = planBill.PriceList;
            billTransaction.Currency = planBill.Currency;
            billTransaction.UnitPrice = planBill.UnitPrice;
            billTransaction.IsProvisionalEstimate = planBill.IsProvisionalEstimate;
            billTransaction.Tax = planBill.Tax;

            #region 记录数量
            decimal qty = isVoid ? planBill.CurrentVoidQty : planBill.PlanQty;
            if (planBill.Type == CodeMaster.BillType.Procurement)
            {
                if (isVoid)
                {
                    //采购结算冲销负数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.POPlanBillVoid;
                    billTransaction.BillQty = -qty;
                    billTransaction.BillAmount = billTransaction.BillQty * billTransaction.UnitPrice;
                }
                else
                {
                    //采购结算正数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.POPlanBill;
                    billTransaction.BillQty = qty;
                    billTransaction.BillAmount = billTransaction.BillQty * billTransaction.UnitPrice;
                }
            }
            else
            {
                if (isVoid)
                {
                    //销售开票冲销负数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.SOPlanBillVoid;
                    billTransaction.BillQty = -qty;
                    billTransaction.BillAmount = billTransaction.BillQty * billTransaction.UnitPrice;
                }
                else
                {
                    //销售开票正数
                    billTransaction.TransactionType = CodeMaster.BillTransactionType.SOPlanBill;
                    billTransaction.BillQty = qty;
                    billTransaction.BillAmount = billTransaction.BillQty * billTransaction.UnitPrice;
                }
            }

            #endregion
            billTransaction.UnitQty = planBill.UnitQty;
            billTransaction.LocationFrom = planBill.LocationFrom;
            billTransaction.SettleLocation = planBill.LocationFrom;
            billTransaction.EffectiveDate = effectiveDate;
            billTransaction.PlanBill = planBill.Id;
            billTransaction.RefId = refId;

            billTransaction.BillTerm = planBill.BillTerm;

            User user = SecurityContextHolder.Get();
            billTransaction.CreateUserId = user.Id;
            billTransaction.CreateUserName = user.FullName;
            billTransaction.CreateDate = DateTime.Now;

            this.genericMgr.Create(billTransaction);

            #endregion
        }


        #endregion

        #region BillMaster Methods
        [Transaction(TransactionMode.Requires)]
        public IList<BillMaster> CreateBill(IList<ActingBill> actingBillList)
        {
            if (actingBillList == null || actingBillList.Count == 0)
            {
                throw new BusinessException("Bill.Error.EmptyBillDetails");
            }

            DateTime dateTimeNow = DateTime.Now;
            IList<BillMaster> billList = new List<BillMaster>();

            foreach (ActingBill actingBill in actingBillList)
            {
                BillMaster bill = null;

                #region 查找和待开明细的transactionType、billAddress、currency一致的TransportBillMaster
                foreach (BillMaster thisBill in billList)
                {
                    if (thisBill.Type == actingBill.Type && thisBill.BillAddress == actingBill.BillAddress
                        && thisBill.Currency == actingBill.Currency)
                    {
                        bill = thisBill;
                        break;
                    }
                }
                #endregion

                #region 没有找到匹配的Bill，新建
                if (bill == null)
                {
                    #region 创建Bill
                    bill = new BillMaster();
                    bill.BillDetails = new List<BillDetail>();
                    bill.BillAddress = actingBill.BillAddress;
                    bill.BillAddressDescription = actingBill.BillAddressDescription;
                    bill.BillNo = numberControlMgr.GetBillNo(bill);
                    bill.Currency = actingBill.Currency;
                    bill.Status = CodeMaster.BillStatus.Create;
                    bill.Type = actingBill.Type;
                    bill.SubType = CodeMaster.BillSubType.Normal;
                    bill.Party = actingBill.Party;
                    bill.PartyName = actingBill.PartyName;

                    this.genericMgr.Create(bill);
                    billList.Add(bill);
                    #endregion
                }
                #endregion

                var q1_billDetails = bill.BillDetails.Where(b => b.Item == actingBill.Item
                    && b.UnitCount == actingBill.UnitCount && b.Uom == actingBill.Uom
                    && b.UnitPrice == actingBill.UnitPrice && b.ReceiptNo == actingBill.ReceiptNo
                    && b.ActingBillId == actingBill.Id);
                if (q1_billDetails != null && q1_billDetails.Count() > 0)
                {
                    BillDetail billDetail = q1_billDetails.First();
                    billDetail.Qty += actingBill.CurrentBillQty;
                    billDetail.Amount += actingBill.CurrentBillAmount;
                    this.genericMgr.Update(billDetail);
                    //扣减ActingBill数量和金额
                    this.UpdateActingBill(billDetail);
                }
                else
                {
                    BillDetail billDetail = this.ActingBill2BillDetail(actingBill);
                    billDetail.BillNo = bill.BillNo;
                    bill.AddBillDetail(billDetail);
                    this.genericMgr.Create(billDetail);
                    //扣减ActingBill数量和金额
                    this.UpdateActingBill(billDetail);
                }
            }
            foreach (var bill in billList)
            {
                foreach (var billDetail in bill.BillDetails)
                {
                    bill.Amount += billDetail.Amount;
                }
            }
            return billList;
        }

        [Transaction(TransactionMode.Requires)]
        public void AddBillDetail(string billNo, IList<ActingBill> actingBillList)
        {
            BillMaster bill = this.genericMgr.FindById<BillMaster>(billNo);

            #region 检查状态
            if (bill.Status != CodeMaster.BillStatus.Create)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenAddDetail", bill.Status, bill.BillNo);
            }
            #endregion

            if (actingBillList != null && actingBillList.Count > 0)
            {
                foreach (ActingBill actingBill in actingBillList)
                {
                    actingBill.CurrentBillQty = actingBill.CurrentBillQty;
                    actingBill.CurrentBillAmount = actingBill.CurrentBillAmount;
                    BillDetail billDetail = this.ActingBill2BillDetail(actingBill);
                    billDetail.BillNo = bill.BillNo;
                    bill.AddBillDetail(billDetail);

                    this.genericMgr.Create(billDetail);
                    //扣减ActingBill数量和金额
                    this.UpdateActingBill(billDetail);
                }
                foreach (var billDetail in bill.BillDetails)
                {
                    bill.Amount += billDetail.Amount;
                }
                this.genericMgr.Update(bill);
            }

        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteBillDetail(IList<BillDetail> billDetailList)
        {
            if (billDetailList != null && billDetailList.Count > 0)
            {
                IDictionary<string, BillMaster> cachedBillDic = new Dictionary<string, BillMaster>();

                foreach (BillDetail billDetail in billDetailList)
                {
                    BillMaster bill = this.genericMgr.FindById<BillMaster>(billDetail.BillNo);

                    #region 缓存Bill
                    if (!cachedBillDic.ContainsKey(bill.BillNo))
                    {
                        cachedBillDic.Add(bill.BillNo, bill);

                        #region 检查状态
                        if (bill.Status != CodeMaster.BillStatus.Create)
                        {
                            throw new BusinessException("Bill.Error.StatusErrorWhenDeleteDetail", bill.Status, bill.BillNo);
                        }
                        #endregion
                    }
                    #endregion

                    //扣减ActingBill数量和金额
                    this.ReverseActingBill(billDetail);

                    this.genericMgr.Delete(billDetail);
                }

                #region 更新Bill
                DateTime dateTimeNow = DateTime.Now;
                foreach (BillMaster bill in cachedBillDic.Values)
                {
                    foreach (var billDetail in bill.BillDetails)
                    {
                        bill.Amount += billDetail.Amount;
                    }
                    this.genericMgr.Update(bill);
                }
                #endregion
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteBill(string billNo)
        {
            BillMaster bill = this.genericMgr.FindById<BillMaster>(billNo);

            #region 检查状态
            if (bill.Status != CodeMaster.BillStatus.Create)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenDelete", bill.Status, bill.BillNo);
            }
            #endregion

            var billDetailList = GetBillDetail(billNo);
            if (billDetailList != null)
            {
                foreach (BillDetail billDetail in billDetailList)
                {
                    //扣减ActingBill数量和金额
                    this.ReverseActingBill(billDetail);

                    this.genericMgr.Delete(billDetail);
                }
            }
            this.genericMgr.Delete(bill);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateBill(BillMaster bill)
        {
            #region 检查状态
            if (bill.Status != CodeMaster.BillStatus.Create)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenUpdate", bill.Status, bill.BillNo);
            }
            #endregion

            #region 更新明细
            if (bill.BillDetails != null && bill.BillDetails.Count > 0)
            {
                foreach (BillDetail billDetail in bill.BillDetails)
                {
                    //反向更新ActingBill，会重新计算开票金额
                    if (billDetail.CurrentBillQty != billDetail.Qty)
                    {
                        this.ReverseActingBill(billDetail);
                    }

                    billDetail.Qty = billDetail.CurrentBillQty;
                    billDetail.Amount = billDetail.CurrentBillAmount;
                    billDetail.Discount = billDetail.CurrentDiscount;
                    this.genericMgr.Update(billDetail);
                }
            }
            #endregion
            bill.Amount = bill.BillDetails.Sum(p => p.Amount);
            this.genericMgr.Update(bill);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseBill(string billNo, DateTime effectiveDate)
        {
            BillMaster oldBill = this.genericMgr.FindById<BillMaster>(billNo);
            oldBill.BillDetails = this.GetBillDetail(billNo);

            #region 检查状态
            if (oldBill.Status != CodeMaster.BillStatus.Create)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenRelease", oldBill.Status, oldBill.BillNo);
            }
            #endregion

            #region 检查明细不能为空
            if (oldBill.BillDetails == null || oldBill.BillDetails.Count == 0)
            {
                throw new BusinessException("Bill.Error.EmptyBillDetail", oldBill.BillNo);
            }
            #endregion

            #region 记录开票事务
            foreach (BillDetail billDetail in oldBill.BillDetails)
            {
                this.RecordBillTransaction(billDetail, effectiveDate, false);

                var actingBill = this.genericMgr.FindById<ActingBill>(billDetail.ActingBillId);
                actingBill.BilledQty += billDetail.Qty;
                actingBill.BilledAmount += billDetail.Amount;
                this.genericMgr.Update(actingBill);

            }
            #endregion

            oldBill.Status = CodeMaster.BillStatus.Submit;
            oldBill.ReleaseDate = DateTime.Now;
            oldBill.ReleaseUserId = SecurityContextHolder.Get().Id;
            oldBill.ReleaseUserName = SecurityContextHolder.Get().Code;

            //this.genericMgr.Update(oldBill);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelBill(string billNo, DateTime effectiveDate)
        {
            BillMaster bill = this.genericMgr.FindById<BillMaster>(billNo);
            bill.BillDetails = this.GetBillDetail(billNo);

            #region 检查状态
            if (bill.Status == CodeMaster.BillStatus.Submit)
            {
                bill.Status = CodeMaster.BillStatus.Cancel;
            }
            else if (bill.Status == CodeMaster.BillStatus.Close)
            {
                bill.Status = CodeMaster.BillStatus.Void;
            }
            else
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenCancel", bill.Status, bill.BillNo);
            }
            #endregion

            if (bill.BillDetails != null && bill.BillDetails.Count > 0)
            {
                foreach (BillDetail billDetail in bill.BillDetails)
                {
                    //反向更新ActingBill
                    this.ReverseActingBill(billDetail);

                    #region 记录开票事务
                    this.RecordBillTransaction(billDetail, effectiveDate, true);
                    #endregion

                    var actingBill = this.genericMgr.FindById<ActingBill>(billDetail.ActingBillId);
                    actingBill.BilledQty -= billDetail.BillQty;
                    actingBill.BilledAmount -= billDetail.Amount;
                    this.genericMgr.Update(actingBill);
                }
            }
            bill.CancelDate = DateTime.Now;
            bill.CancelUserId = SecurityContextHolder.Get().Id;
            bill.CancelUserName = SecurityContextHolder.Get().Code;
            this.genericMgr.Update(bill);
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseBill(string billNo)
        {
            BillMaster oldBill = this.genericMgr.FindById<BillMaster>(billNo);

            #region 检查状态
            if (oldBill.Status != CodeMaster.BillStatus.Submit)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenClose", oldBill.Status, oldBill.BillNo);
            }
            #endregion

            oldBill.Status = CodeMaster.BillStatus.Close;
            oldBill.CloseDate = DateTime.Now;
            oldBill.CloseUserId = SecurityContextHolder.Get().Id;
            oldBill.CloseUserName = SecurityContextHolder.Get().Code;

            this.genericMgr.Update(oldBill);
        }

        [Transaction(TransactionMode.Requires)]
        public void SaveBill(string billNo, string externalBillNo, string referenceBillNo, string invoiceBillNo, DateTime invoiceDate)
        {
            BillMaster oldBill = this.genericMgr.FindById<BillMaster>(billNo);
            oldBill.ExternalBillNo = externalBillNo;

            oldBill.InvoiceNo = invoiceBillNo;
            oldBill.InvoiceDate = invoiceDate;
            this.genericMgr.Update(oldBill);
        }

        #endregion

        #region BillDetail Methods
        [Transaction(TransactionMode.Unspecified)]
        private IList<BillDetail> GetBillDetail(string billNo)
        {
            DetachedCriteria criteria = DetachedCriteria.For<BillDetail>();
            criteria.Add(Expression.Eq("BillNo", billNo));

            return this.genericMgr.FindAll<BillDetail>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        private BillDetail ActingBill2BillDetail(ActingBill actingBill)
        {
            BillDetail billDetail = Mapper.Map<ActingBill, BillDetail>(actingBill);

            billDetail.Amount = actingBill.CurrentBillAmount;
            billDetail.Qty = actingBill.CurrentBillQty;
            billDetail.PriceList = actingBill.PriceList;

            //本次开票数量大于剩余数量
            if (actingBill.CurrentBillQty > (actingBill.BillQty - actingBill.BillingQty))
            {
                throw new BusinessException("ActingBill.Error.CurrentBillQtyGeRemainQty");
            }

            //本次开票金额大于剩余金额
            if (actingBill.CurrentBillAmount > (actingBill.BillAmount - actingBill.BillingAmount))
            {
                throw new BusinessException("ActingBill.Error.CurrentBillAmountGeRemainAmount");
            }

            return billDetail;
        }

        [Transaction(TransactionMode.Unspecified)]
        public void GroupActingBillByItem(ref IList<ActingBill> actingBills)
        {
            actingBills = (from b in actingBills
                           group b by new
                           {
                               b.Item,
                               b.Uom,
                               b.UnitPrice,
                               b.Currency
                           } into g
                           select new ActingBill
                           {
                               Item = g.Key.Item,
                               ReferenceItemCode = g.First().ReferenceItemCode,
                               Uom = g.Key.Uom,
                               UnitPrice = g.Key.UnitPrice,
                               Currency = g.Key.Currency,
                               BillAmount = g.Sum(r => r.BillAmount),
                               BillingAmount = g.Sum(r => r.BillingAmount),
                               BillingQty = g.Sum(r => r.BillingQty),
                               BillQty = g.Sum(r => r.BillQty),
                           }).ToList();
        }


        #region

        [Transaction(TransactionMode.Unspecified)]
        public void GroupBillDetailByItem(ref IList<BillDetail> billDetails)
        {
            billDetails = (from b in billDetails
                           group b by new
                           {
                               b.Item,
                               b.Uom,
                               b.UnitPrice,
                               b.Currency
                           } into g
                           select new BillDetail
                           {
                               Item = g.Key.Item,
                               ReferenceItemCode = g.First().ReferenceItemCode,
                               Uom = g.Key.Uom,
                               UnitPrice = g.Key.UnitPrice,
                               Currency = g.Key.Currency,
                               Qty = g.Sum(b => b.Qty),
                               Amount = g.Sum(b => b.Amount),
                               BillQty = g.Sum(b => b.BillQty),
                               BilledQty = g.Sum(b => b.BilledQty),
                               CurrentBillQty = g.Sum(b => b.CurrentBillQty),
                               CurrentBillAmount = g.Sum(b => b.CurrentBillAmount)
                           }).ToList();
        }

        [Transaction(TransactionMode.Unspecified)]
        public void GroupBillDetail(ref IList<BillDetail> billDetails)
        {
            billDetails = (from a in billDetails
                           group a by new
                           {
                               a.Flow,
                               a.UnitPrice,
                               a.Party,
                               a.ReceiptNo,
                               a.Item,
                               a.Uom,
                               a.Currency,
                               a.OrderNo,
                               a.UnitCount,
                               a.Type,
                               a.EffectiveDate,
                               a.ExternalIpNo,
                               a.ExternalReceiptNo,
                               a.IpNo,
                               a.IsIncludeTax,
                               a.IsProvisionalEstimate,
                               a.LocationFrom,
                               a.PartyName,
                               a.ReferenceItemCode,
                               a.Tax,
                           } into result
                           select new BillDetail
                          {
                              ItemDescription = result.First().ItemDescription,
                              Amount = result.Sum(r => r.Amount),
                              Qty = result.Sum(r => r.Qty),
                              Currency = result.Key.Currency,
                              Flow = result.Key.Flow,
                              Item = result.Key.Item,
                              OrderNo = result.Key.OrderNo,
                              Party = result.Key.Party,
                              ReceiptNo = result.Key.ReceiptNo,
                              UnitCount = result.Key.UnitCount,
                              UnitPrice = result.Key.UnitPrice,
                              Uom = result.Key.Uom,
                              Type = result.Key.Type,
                              EffectiveDate = result.Key.EffectiveDate,
                              ExternalIpNo = result.Key.ExternalIpNo,
                              ExternalReceiptNo = result.Key.ExternalReceiptNo,
                              IpNo = result.Key.IpNo,
                              IsIncludeTax = result.Key.IsIncludeTax,
                              IsProvisionalEstimate = result.Key.IsProvisionalEstimate,
                              LocationFrom = result.Key.LocationFrom,
                              PartyName = result.Key.PartyName,
                              ReferenceItemCode = result.Key.ReferenceItemCode,
                              Tax = result.Key.Tax,
                              BillQty = result.Sum(b => b.BillQty),
                              BilledQty = result.Sum(b => b.BilledQty),
                              CurrentBillQty = result.Sum(b => b.CurrentBillQty),
                              CurrentBillAmount = result.Sum(b => b.CurrentBillAmount),
                          }).ToList();
        }
        #endregion

        #endregion Customized Methods

        #region ActingBill

        [Transaction(TransactionMode.Requires)]
        private void UpdateActingBill(BillDetail billDetail)
        {
            #region 增加新BillDetail的数量和金额
            if (billDetail != null)
            {
                ActingBill actingBill = this.genericMgr.FindById<ActingBill>(billDetail.ActingBillId);

                actingBill.BillingQty += billDetail.Qty;
                actingBill.BillingAmount += billDetail.Amount;
                if ((actingBill.BillQty > 0 && actingBill.BillQty < actingBill.BillingQty)
                    || (actingBill.BillQty < 0 && actingBill.BillQty > actingBill.BillingQty))
                {
                    throw new BusinessException("ActingBill.Error.CurrentBillQtyGeRemainQty");
                }

                if ((actingBill.BillAmount > 0 && actingBill.BillAmount < actingBill.BillingAmount)
                   || (actingBill.BillAmount < 0 && actingBill.BillAmount > actingBill.BillingAmount))
                {
                    throw new BusinessException("ActingBill.Error.CurrentBillAmountGeRemainAmount");
                }

                if (actingBill.BillQty == actingBill.BillingQty)
                {
                    actingBill.IsClose = true;
                }
                else
                {
                    actingBill.IsClose = false;
                }
                this.genericMgr.Update(actingBill);
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        private void ReverseActingBill(BillDetail billDetail)
        {
            #region 扣减旧BillDetail的数量和金额
            ActingBill actingBill = this.genericMgr.FindById<ActingBill>(billDetail.ActingBillId);
            actingBill.BillingQty -= billDetail.Qty;
            actingBill.BillingAmount -= billDetail.Amount;
            actingBill.BillingQty += billDetail.CurrentBillQty;
            actingBill.BillingAmount += billDetail.CurrentBillAmount;

            if ((actingBill.BillQty > 0 && actingBill.BillQty < actingBill.BillingQty)
                || (actingBill.BillQty < 0 && actingBill.BillQty > actingBill.BillingQty))
            {
                throw new BusinessException("ActingBill.Error.CurrentBillQtyGeRemainQty");
            }

            if ((actingBill.BillAmount > 0 && actingBill.BillAmount < actingBill.BillingAmount)
                || (actingBill.BillAmount < 0 && actingBill.BillAmount > actingBill.BillingAmount))
            {
                throw new BusinessException("ActingBill.Error.CurrentBillAmountGeRemainAmount");
            }

            if (actingBill.BillQty == actingBill.BillingQty
                && actingBill.BillAmount == actingBill.BillingAmount)
            {
                actingBill.IsClose = true;
            }
            else
            {
                actingBill.IsClose = false;
            }
            this.genericMgr.Update(actingBill);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void RecalculatePrice(IList<ActingBill> actingBillList)
        {
            if (actingBillList != null && actingBillList.Count > 0)
            {
                foreach (ActingBill actingBill in actingBillList)
                {
                    if (actingBill.CurrentRecalculatePrice != actingBill.UnitPrice)
                    {
                        actingBill.BillAmount = actingBill.BillingAmount + (actingBill.BillQty - actingBill.BillingQty) * actingBill.CurrentRecalculatePrice;
                        actingBill.UnitPrice = actingBill.CurrentRecalculatePrice;
                    }
                    actingBill.IsProvisionalEstimate = false;
                    this.genericMgr.Update(actingBill);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public IList<ActingBill> GetRecalculatePrice(CodeMaster.BillType billType, string party, string flow,
            string receiptNo, string externalReceiptNo, string item, string currency, DateTime startDate, DateTime endDate, bool includeNoEstPrice)
        {
            string hql = @"select a from ActingBill as a where Type = ? and EffectiveDate >= ? and EffectiveDate <= ? and IsClose=? ";
            List<object> para = new List<object>();
            para.Add(billType);
            para.Add(startDate.Date);
            para.Add(endDate.Date);
            para.Add(false);
            if (!includeNoEstPrice)
            {
                hql += " and (exists (select 1 from PlanBill as p where p.Id=a.PlanBill and p.IsProvisionalEstimate=?))";
                para.Add(true);
            }

            if (!string.IsNullOrWhiteSpace(party))
            {
                hql += " and Party = ?";
                para.Add(party);
            }
            if (!string.IsNullOrWhiteSpace(flow))
            {
                hql += " and Flow = ?";
                para.Add(flow);
            }
            if (!string.IsNullOrWhiteSpace(receiptNo))
            {
                hql += " and ReceiptNo = ?";
                para.Add(receiptNo);
            }
            if (!string.IsNullOrWhiteSpace(externalReceiptNo))
            {
                hql += " and ExternalReceiptNo = ?";
                para.Add(externalReceiptNo);
            }
            if (!string.IsNullOrWhiteSpace(item))
            {
                hql += " and Item = ?";
                para.Add(item);
            }
            if (!string.IsNullOrWhiteSpace(currency))
            {
                hql += " and Currency = ?";
                para.Add(currency);
            }

            var actingBillList = this.genericMgr.FindAll<ActingBill>(hql, para.ToArray());
            var newActingBillList = new List<ActingBill>();
            if (actingBillList != null && actingBillList.Count > 0)
            {
                foreach (ActingBill actingBill in actingBillList)
                {
                    PriceListDetail priceListDetail =
                        this.itemMgr.GetItemPrice
                        (actingBill.Item, actingBill.Uom, actingBill.PriceList, actingBill.Currency, actingBill.EffectiveDate);

                    if (priceListDetail != null &&//正式价不能更新成暂估价
                        !priceListDetail.IsProvisionalEstimate)
                    {
                        actingBill.IsIncludeTax = false; //待开票明细都是不含税金额

                        if (actingBill.IsIncludeTax)   //如果价格单含税，待开票金额要转为不含税金额
                        {
                            Tax tax = this.genericMgr.FindById<Tax>(actingBill.Tax);
                            actingBill.CurrentRecalculatePrice = priceListDetail.UnitPrice / (1 + tax.Rate);
                        }
                        else
                        {
                            actingBill.CurrentRecalculatePrice = priceListDetail.UnitPrice;
                        }

                        //if (actingBill.IsProvisionalEstimate != priceListDetail.IsProvisionalEstimate)
                        //{
                        //    actingBill.IsProvisionalEstimate = priceListDetail.IsProvisionalEstimate;
                        //    this.genericMgr.Update(actingBill);
                        //}
                        newActingBillList.Add(actingBill);
                    }
                }
            }
            return newActingBillList;
        }

        private ActingBill RetriveActingBill(PlanBill planBill, DateTime effectiveDate)
        {
            DetachedCriteria criteria = DetachedCriteria.For<ActingBill>();
            criteria.Add(Expression.Eq("ReceiptNo", planBill.ReceiptNo));

            IList<ActingBill> actingBillList = this.genericMgr.FindAll<ActingBill>(criteria);

            ActingBill actingBill = new ActingBill();
            if (actingBillList == null || actingBillList.Count == 0)
            {
                actingBill = Mapper.Map<PlanBill, ActingBill>(planBill);
            }
            else
            {
                var actingBills = actingBillList.Where(p => p.OrderNo == planBill.OrderNo &&
                    p.ExternalReceiptNo == planBill.ExternalReceiptNo &&
                    p.Type == planBill.Type &&
                    p.Item == planBill.Item &&
                    p.BillAddress == planBill.BillAddress &&
                    p.Uom == planBill.Uom &&
                    p.UnitCount == planBill.UnitCount &&
                    p.PriceList == planBill.PriceList &&
                    p.RecPrice == planBill.UnitPrice &&
                    p.Currency == planBill.Currency &&
                    p.IsIncludeTax == planBill.IsIncludeTax &&
                    p.Tax == planBill.Tax &&
                    p.LocationFrom == planBill.LocationFrom &&
                    p.IsProvisionalEstimate == planBill.IsProvisionalEstimate &&
                    p.EffectiveDate == effectiveDate);
                if (actingBills == null || actingBills.Count() == 0)
                {
                    actingBill = Mapper.Map<PlanBill, ActingBill>(planBill);
                }
                else if (actingBills.Count() == 1)
                {
                    actingBill = actingBills.First();
                }
                else
                {
                    throw new TechnicalException("Acting bill record consolidate error, find target acting bill number great than 1.");
                }
            }

            actingBill.BillQty += planBill.CurrentActingQty;
            actingBill.EffectiveDate = effectiveDate;
            actingBill.IsClose = (actingBill.BillQty == (actingBill.BillingQty + actingBill.VoidQty));
            actingBill.BillAmount += planBill.UnitPrice * planBill.CurrentActingQty;
            actingBill.RecPrice = planBill.UnitPrice;

            return actingBill;
        }

        #endregion ActingBill

        public IList<BillIOB> GetBillIOB(CodeMaster.BillType billType, string party, string location, string item,
            DateTime startDate, DateTime endDate)
        {
            //PlanBillTransaction 生成寄售 寄售入
            string hqlInStart = @"select p.Party,p.LocationFrom,p.Item,sum(p.BillQty*p.UnitQty) as BillQty ,
                   sum(p.BillAmount*p.UnitQty) as BillAmount                   
                   from PlanBillTransaction p where p.EffectiveDate >= ? and p.EffectiveDate < ? and p.TransactionType in( ? , ? ) 
                   and BillTerm>? ";
            string hqlInEnd = @"select p.Party,p.LocationFrom,p.Item,sum(p.BillQty*p.UnitQty) as BillQty,
                   sum(p.BillAmount*p.UnitQty) as BillAmount
                   from PlanBillTransaction p where (p.EffectiveDate >= ? and p.EffectiveDate>= ?) and p.TransactionType in( ? , ? ) 
                   and BillTerm>? ";

            var paramIn = new List<object>();
            paramIn.Add(startDate.Date);
            paramIn.Add(endDate.Date.AddDays(1));
            if (billType == CodeMaster.BillType.Procurement)
            {
                paramIn.Add(CodeMaster.BillTransactionType.POPlanBill);
                paramIn.Add(CodeMaster.BillTransactionType.POPlanBillVoid);
            }
            else
            {
                paramIn.Add(CodeMaster.BillTransactionType.SOPlanBill);
                paramIn.Add(CodeMaster.BillTransactionType.SOPlanBillVoid);
            }
            paramIn.Add((int)com.Sconit.CodeMaster.OrderBillTerm.AfterInspection);

            //SettleBillTransaction 结算事务 PlanBill->ActBill
            string hqlOutStart = @"select p.Party,p.LocationFrom,p.Item,sum(p.BillQty*p.UnitQty) as BillQty ,
                    sum(p.BillAmount*p.UnitQty) as BillAmount
                   from SettleBillTransaction p where p.EffectiveDate >= ? and p.EffectiveDate< ? and p.TransactionType in( ? , ? )  
                   and BillTerm>? ";
            string hqlOutEnd = @"select p.Party,p.LocationFrom,p.Item,sum(p.BillQty*p.UnitQty) as BillQty,
                    sum(p.BillAmount*p.UnitQty) as BillAmount
                   from SettleBillTransaction p where (p.EffectiveDate >= ? and p.EffectiveDate>= ?) and p.TransactionType in( ? , ? )  
                   and BillTerm>? ";

            var paramOut = new List<object>();
            paramOut.Add(startDate.Date);
            paramOut.Add(endDate.Date.AddDays(1));
            if (billType == CodeMaster.BillType.Procurement)
            {
                paramOut.Add(CodeMaster.BillTransactionType.POSettle);
                paramOut.Add(CodeMaster.BillTransactionType.POSettleVoid);
            }
            else
            {
                paramOut.Add(CodeMaster.BillTransactionType.SOSettle);
                paramOut.Add(CodeMaster.BillTransactionType.SOSettleVoid);
            }
            paramOut.Add((int)com.Sconit.CodeMaster.OrderBillTerm.AfterInspection);

            //当前寄售
            string hqlEnd = @"select p.Party,p.LocationFrom,p.Item,
                    sum((p.PlanQty+p.VoidQty-p.ActingQty)*p.UnitQty) as Qty,
                    sum((p.PlanAmount+p.VoidAmount-p.ActingAmount)*p.UnitQty) as Amount,
                    i.Uom,i.Description,i.ReferenceCode,l.Name,party.Name
                    from PlanBill p,Item i,Location l,Party party 
                    where p.Item = i.Code and p.LocationFrom = l.Code and p.Party = party.Code
                    and p.IsClose = ?  and Type = ? ";
            var paramEnd = new List<object>();
            paramEnd.Add(false);
            paramEnd.Add(billType);

            if (!string.IsNullOrWhiteSpace(party))
            {
                hqlInStart += " and p.Party = ? ";
                hqlInEnd += " and p.Party = ? ";
                hqlOutStart += " and p.Party = ? ";
                hqlOutEnd += " and p.Party = ? ";
                hqlEnd += " and p.Party = ? ";
                paramIn.Add(party);
                paramOut.Add(party);
                paramEnd.Add(party);
            }
            if (!string.IsNullOrWhiteSpace(location))
            {
                hqlInStart += " and p.LocationFrom = ? ";
                hqlInEnd += " and p.LocationFrom = ? ";
                hqlOutStart += " and p.LocationFrom = ? ";
                hqlOutEnd += " and p.LocationFrom = ? ";
                hqlEnd += " and p.LocationFrom = ? ";
                paramIn.Add(location);
                paramOut.Add(location);
                paramEnd.Add(location);
            }
            if (!string.IsNullOrWhiteSpace(item))
            {
                hqlInStart += " and p.Item = ? ";
                hqlInEnd += " and p.Item = ? ";
                hqlOutStart += " and p.Item = ? ";
                hqlOutEnd += " and p.Item = ? ";
                hqlEnd += " and p.Item = ? ";
                paramIn.Add(item);
                paramOut.Add(item);
                paramEnd.Add(item);
            }
            hqlInStart += " group by p.Party,p.LocationFrom,p.Item ";
            hqlInEnd += " group by p.Party,p.LocationFrom,p.Item ";
            hqlOutStart += " group by p.Party,p.LocationFrom,p.Item ";
            hqlOutEnd += " group by p.Party,p.LocationFrom,p.Item ";
            hqlEnd += " group by p.Party,p.LocationFrom,p.Item,i.Uom,i.Description,i.ReferenceCode,l.Name,party.Name ";

            //In
            IList<object[]> billInStartList = this.genericMgr.FindAll<object[]>(hqlInStart, paramIn.ToArray());
            IList<object[]> billInEndList = this.genericMgr.FindAll<object[]>(hqlInEnd, paramIn.ToArray());

            //Out
            IList<object[]> billOutStartList = this.genericMgr.FindAll<object[]>(hqlOutStart, paramOut.ToArray());
            IList<object[]> billOutEndList = this.genericMgr.FindAll<object[]>(hqlOutEnd, paramOut.ToArray());

            //End
            IList<object[]> billEndList = this.genericMgr.FindAll<object[]>(hqlEnd, paramEnd.ToArray());

            IList<BillIOB> billIOBList = new List<BillIOB>();

            //期末寄售
            foreach (object[] billEnd in billEndList)
            {
                BillIOB billIOB = new BillIOB();
                billIOB.Party = (string)billEnd[0];
                billIOB.Location = (string)billEnd[1];
                billIOB.Item = (string)billEnd[2];
                //当前数量
                billIOB.EndQty = (decimal)billEnd[3];
                billIOB.EndAmount = (decimal)billEnd[4];
                billIOB.Uom = (string)billEnd[5];
                billIOB.ItemDescription = (string)billEnd[6] + (string.IsNullOrWhiteSpace((string)billEnd[7]) ? string.Empty : "[" + (string)billEnd[7] + "]");
                billIOB.LocationName = (string)billEnd[8];
                billIOB.PartyName = (string)billEnd[9];

                //寄售入
                //推算出期末数:减去入
                foreach (var billIn in billInEndList)
                {
                    if (billIOB.Party == (string)billIn[0]
                        && billIOB.Location == (string)billIn[1]
                        && billIOB.Item == (string)billIn[2])
                    {
                        billIOB.EndQty -= (decimal)billIn[3];
                        billIOB.EndAmount -= (decimal)billIn[4];
                        break;
                    }
                }
                //记录入数
                foreach (var billIn in billInStartList)
                {
                    if (billIOB.Party == (string)billIn[0]
                        && billIOB.Location == (string)billIn[1]
                        && billIOB.Item == (string)billIn[2])
                    {
                        billIOB.InQty = (decimal)billIn[3];
                        billIOB.InAmount = (decimal)billIn[4];
                        break;
                    }
                }

                //寄售出
                //推算出期末数:加上出
                foreach (var billOut in billOutEndList)
                {
                    if (billIOB.Party == (string)billOut[0]
                        && billIOB.Location == (string)billOut[1]
                        && billIOB.Item == (string)billOut[2])
                    {
                        billIOB.EndQty += (decimal)billOut[3];
                        billIOB.EndAmount += (decimal)billOut[4];
                        break;
                    }
                }
                //记录出数
                foreach (var billOut in billOutStartList)
                {
                    if (billIOB.Party == (string)billOut[0]
                        && billIOB.Location == (string)billOut[1]
                        && billIOB.Item == (string)billOut[2])
                    {
                        billIOB.OutQty = (decimal)billOut[3];
                        billIOB.OutAmount = (decimal)billOut[4];
                        break;
                    }
                }
                //计算出期初数
                billIOB.StartQty = billIOB.EndQty + billIOB.OutQty - billIOB.InQty;
                billIOB.StartAmount = billIOB.EndAmount + billIOB.OutAmount - billIOB.InAmount;

                billIOBList.Add(billIOB);
            }

            //期末无,有出
            foreach (var billOut in billOutStartList)
            {
                var billIOB = billIOBList.FirstOrDefault(p => p.Party == (string)billOut[0]
                     && p.Location == (string)billOut[1] && p.Item == (string)billOut[2]);
                if (billIOB == null)
                {
                    BillIOB newBillIOB = new BillIOB();
                    newBillIOB.Party = (string)billOut[0];
                    newBillIOB.Location = (string)billOut[1];
                    newBillIOB.Item = (string)billOut[2];
                    newBillIOB.InQty = (decimal)billOut[3];
                    newBillIOB.InAmount = (decimal)billOut[4];
                    newBillIOB.StartQty = -newBillIOB.InQty;
                    newBillIOB.StartAmount = -newBillIOB.InAmount;

                    var _item = this.genericMgr.FindById<Item>(newBillIOB.Item);
                    newBillIOB.ItemDescription = _item.FullDescription;
                    newBillIOB.Uom = _item.Uom;
                    newBillIOB.LocationName = this.genericMgr.FindById<Location>(newBillIOB.Location).Name;
                    newBillIOB.PartyName = this.genericMgr.FindById<Party>(newBillIOB.Item).Name;

                    billIOBList.Add(newBillIOB);
                }
            }

            return billIOBList;
        }

        #region 合并Planbill
        public void MergePlanBill(IList<HuStatus> huStatusList)
        {
            BusinessException businessException = new BusinessException();
            if (huStatusList.Where(h => h.PlanBill.HasValue == false).Count() > 0)
            {
                businessException.AddMessage("所有的条码必须为寄售"); 
            }

            IList<PlanBill> planBillList = new List<PlanBill>();
            var lastOrderNo = string.Empty;
            var total = 0M;
            Int32? mergedPlanBill = 0;
            for (int i = 0; i < huStatusList.Count;i++)
            {
                var planBill = this.genericMgr.FindById<PlanBill>(huStatusList[i].PlanBill);
                if (string.IsNullOrEmpty(lastOrderNo))
                {
                    //记录第一个订单号
                    lastOrderNo = planBill.OrderNo;
                }
                else
                {
                    if (lastOrderNo != planBill.OrderNo)
                    {
                        businessException.AddMessage("不允许多个订单合并翻箱");
                        break;
                    }
                }
                if (i < huStatusList.Count - 1)
                {
                    total += huStatusList[i].Qty;
                    planBill.PlanQty -= huStatusList[i].Qty;
                }
                else
                {
                    mergedPlanBill = planBill.Id;
                    planBill.PlanQty += total;
                }
                planBill.HuId = null;
                this.genericMgr.Update(planBill);
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            foreach (var huStatus in huStatusList)
            {
                huStatus.PlanBill = mergedPlanBill.Value;
            }
        }
        #endregion

    }
}
