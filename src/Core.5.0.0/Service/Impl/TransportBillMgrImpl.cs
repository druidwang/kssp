using System;
using System.Linq;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.MD;
using System.Collections.Generic;
using NHibernate.Criterion;
using System.Text;
using com.Sconit.Entity.TMS;
using com.Sconit.CodeMaster;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class TransportBillMgrImpl : BaseMgr, ITransportBillMgr
    {
        #region 变量
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        #endregion



        #region TransportBillMaster Methods
        [Transaction(TransactionMode.Requires)]
        public IList<TransportBillMaster> CreateBill(IList<TransportActingBill> actingBillList)
        {
            if (actingBillList == null || actingBillList.Count == 0)
            {
                throw new BusinessException("Bill.Error.EmptyBillDetails");
            }

            DateTime dateTimeNow = DateTime.Now;
            IList<TransportBillMaster> billList = new List<TransportBillMaster>();

            foreach (TransportActingBill actingBill in actingBillList)
            {
                TransportBillMaster bill = null;

                #region 查找和待开明细的transactionType、billAddress、currency一致的BillMstr
                foreach (TransportBillMaster thisBill in billList)
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
                    bill = new TransportBillMaster();
                    bill.BillDetails = new List<TransportBillDetail>();
                    bill.BillAddress = actingBill.BillAddress;
                    bill.BillAddressDescription = actingBill.BillAddressDescription;
                    bill.BillNo = numberControlMgr.GetTransportBillNo(bill);
                    bill.Currency = actingBill.Currency;
                    bill.Status = CodeMaster.BillStatus.Create;
                    bill.Type = actingBill.Type;
                    bill.SubType = CodeMaster.BillSubType.Normal;
                    bill.Carrier = actingBill.Carrier;
                    bill.CarrierDescription = actingBill.CarrierDescription;

                    this.genericMgr.Create(bill);
                    billList.Add(bill);
                    #endregion
                }
                #endregion

                var q1_billDetails = bill.BillDetails.Where(b => b.ActBill == actingBill.Id);
                if (q1_billDetails != null && q1_billDetails.Count() > 0)
                {
                    TransportBillDetail billDetail = q1_billDetails.First();
                    billDetail.BillQty += actingBill.CurrentBillQty;
                    billDetail.BillAmount += actingBill.CurrentBillAmount;
                    this.genericMgr.Update(billDetail);
                    //扣减TransportActingBill数量和金额
                    this.UpdateTransportActingBill(billDetail);
                }
                else
                {
                    TransportBillDetail billDetail = this.TransportActingBill2TransportBillDetail(actingBill);
                    billDetail.BillNo = bill.BillNo;
                    bill.AddBillDetail(billDetail);
                    this.genericMgr.Create(billDetail);
                    //扣减TransportActingBill数量和金额
                    this.UpdateTransportActingBill(billDetail);
                }
            }
            foreach (var bill in billList)
            {
                foreach (var billDetail in bill.BillDetails)
                {
                    bill.BillAmount += billDetail.BillAmount;
                }
            }
            return billList;
        }

        [Transaction(TransactionMode.Requires)]
        public void AddBillDetail(string billNo, IList<TransportActingBill> actingBillList)
        {
            TransportBillMaster bill = this.genericMgr.FindById<TransportBillMaster>(billNo);

            #region 检查状态
            if (bill.Status != CodeMaster.BillStatus.Create)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenAddDetail", bill.Status, bill.BillNo);
            }
            #endregion

            if (actingBillList != null && actingBillList.Count > 0)
            {
                foreach (TransportActingBill actingBill in actingBillList)
                {
                    actingBill.CurrentBillQty = actingBill.CurrentBillQty;
                    actingBill.CurrentBillAmount = actingBill.CurrentBillAmount;
                    TransportBillDetail billDetail = this.TransportActingBill2TransportBillDetail(actingBill);
                    billDetail.BillNo = bill.BillNo;
                    bill.AddBillDetail(billDetail);

                    this.genericMgr.Create(billDetail);
                    //扣减TransportActingBill数量和金额
                    this.UpdateTransportActingBill(billDetail);
                }
                foreach (var billDetail in bill.BillDetails)
                {
                    bill.BillAmount += billDetail.BillAmount;
                }
                this.genericMgr.Update(bill);
            }

        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteBillDetail(IList<TransportBillDetail> billDetailList)
        {
            if (billDetailList != null && billDetailList.Count > 0)
            {
                IDictionary<string, TransportBillMaster> cachedBillDic = new Dictionary<string, TransportBillMaster>();

                foreach (TransportBillDetail billDetail in billDetailList)
                {
                    TransportBillMaster bill = this.genericMgr.FindById<TransportBillMaster>(billDetail.BillNo);

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

                    //扣减TransportActingBill数量和金额
                    this.ReverseTransportActingBill(billDetail);

                    this.genericMgr.Delete(billDetail);
                }

                #region 更新Bill
                DateTime dateTimeNow = DateTime.Now;
                foreach (TransportBillMaster bill in cachedBillDic.Values)
                {
                    foreach (var billDetail in bill.BillDetails)
                    {
                        bill.BillAmount += billDetail.BillAmount;
                    }
                    this.genericMgr.Update(bill);
                }
                #endregion
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteBill(string billNo)
        {
            TransportBillMaster bill = this.genericMgr.FindById<TransportBillMaster>(billNo);

            #region 检查状态
            if (bill.Status != CodeMaster.BillStatus.Create)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenDelete", bill.Status, bill.BillNo);
            }
            #endregion

            var billDetailList = GetTransportBillDetail(billNo);
            if (billDetailList != null)
            {
                foreach (TransportBillDetail billDetail in billDetailList)
                {
                    //扣减TransportActingBill数量和金额
                    this.ReverseTransportActingBill(billDetail);

                    this.genericMgr.Delete(billDetail);
                }
            }
            this.genericMgr.Delete(bill);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateBill(TransportBillMaster bill)
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
                foreach (TransportBillDetail billDetail in bill.BillDetails)
                {
                    //反向更新TransportActingBill，会重新计算开票金额
                    if (billDetail.CurrentBillQty != billDetail.BillQty)
                    {
                        this.ReverseTransportActingBill(billDetail);
                    }

                    billDetail.BillQty = billDetail.CurrentBillQty;
                    billDetail.BillAmount = billDetail.CurrentBillAmount;
                    billDetail.Discount = billDetail.CurrentDiscount;
                    this.genericMgr.Update(billDetail);
                }
            }
            #endregion
            bill.BillAmount = bill.BillDetails.Sum(p => p.BillAmount);
            this.genericMgr.Update(bill);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseBill(string billNo, DateTime effectiveDate)
        {
            TransportBillMaster oldBill = this.genericMgr.FindById<TransportBillMaster>(billNo);
            oldBill.BillDetails = this.GetTransportBillDetail(billNo);

            #region 检查状态
            if (oldBill.Status != CodeMaster.BillStatus.Create)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenRelease", oldBill.Status, oldBill.BillNo);
            }
            #endregion

            #region 检查明细不能为空
            if (oldBill.BillDetails == null || oldBill.BillDetails.Count == 0)
            {
                throw new BusinessException("Bill.Error.EmptyTransportBillDetail", oldBill.BillNo);
            }
            #endregion

            #region 记录开票事务
            foreach (TransportBillDetail billDetail in oldBill.BillDetails)
            {
                //  this.RecordBillTransaction(billDetail, effectiveDate, false);

                var actingBill = this.genericMgr.FindById<TransportActingBill>(billDetail.ActBill);
                actingBill.BilledQty += billDetail.BillQty;
                actingBill.BilledAmount += billDetail.BillAmount;
                this.genericMgr.Update(actingBill);

            }
            #endregion

            oldBill.Status = CodeMaster.BillStatus.Submit;
            oldBill.SubmitDate = DateTime.Now;
            oldBill.SubmitUser = SecurityContextHolder.Get().Id;
            oldBill.SubmitUserName = SecurityContextHolder.Get().Code;

            //this.genericMgr.Update(oldBill);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelBill(string billNo, DateTime effectiveDate)
        {
            TransportBillMaster bill = this.genericMgr.FindById<TransportBillMaster>(billNo);
            bill.BillDetails = this.GetTransportBillDetail(billNo);

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
                foreach (TransportBillDetail billDetail in bill.BillDetails)
                {
                    //反向更新TransportActingBill
                    this.ReverseTransportActingBill(billDetail);

                    #region 记录开票事务
                    // this.RecordBillTransaction(billDetail, effectiveDate, true);
                    #endregion

                    var actingBill = this.genericMgr.FindById<TransportActingBill>(billDetail.ActBill);
                    actingBill.BilledQty -= billDetail.BillQty;
                    actingBill.BilledAmount -= billDetail.BillAmount;
                    this.genericMgr.Update(actingBill);
                }
            }
            bill.CancelDate = DateTime.Now;
            bill.CancelUser = SecurityContextHolder.Get().Id;
            bill.CancelUserName = SecurityContextHolder.Get().Code;
            this.genericMgr.Update(bill);
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseBill(string billNo)
        {
            TransportBillMaster oldBill = this.genericMgr.FindById<TransportBillMaster>(billNo);

            #region 检查状态
            if (oldBill.Status != CodeMaster.BillStatus.Submit)
            {
                throw new BusinessException("Bill.Error.StatusErrorWhenClose", oldBill.Status, oldBill.BillNo);
            }
            #endregion

            oldBill.Status = CodeMaster.BillStatus.Close;
            oldBill.CloseDate = DateTime.Now;
            oldBill.CloseUser = SecurityContextHolder.Get().Id;
            oldBill.CloseUserName = SecurityContextHolder.Get().Code;

            this.genericMgr.Update(oldBill);
        }

        [Transaction(TransactionMode.Requires)]
        public void SaveBill(string billNo, string externalBillNo, string referenceBillNo, string invoiceBillNo, DateTime invoiceDate)
        {
            TransportBillMaster oldBill = this.genericMgr.FindById<TransportBillMaster>(billNo);
            oldBill.ExternalBillNo = externalBillNo;

            //oldBill..InvoiceNo = invoiceBillNo;
            //oldBill.InvoiceDate = invoiceDate;
            this.genericMgr.Update(oldBill);
        }

        #endregion

        #region TransportBillDetail Methods
        [Transaction(TransactionMode.Unspecified)]
        private IList<TransportBillDetail> GetTransportBillDetail(string billNo)
        {
            DetachedCriteria criteria = DetachedCriteria.For<TransportBillDetail>();
            criteria.Add(Expression.Eq("BillNo", billNo));

            return this.genericMgr.FindAll<TransportBillDetail>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        private TransportBillDetail TransportActingBill2TransportBillDetail(TransportActingBill actingBill)
        {
            TransportBillDetail billDetail = Mapper.Map<TransportActingBill, TransportBillDetail>(actingBill);

            billDetail.BillAmount = actingBill.CurrentBillAmount;
            billDetail.BillQty = actingBill.CurrentBillQty;
            //  billDetail.PriceList = actingBill.PriceList;

            //本次开票数量大于剩余数量
            if (actingBill.CurrentBillQty > (actingBill.BillQty - actingBill.BillingQty))
            {
                throw new BusinessException("TransportActingBill.Error.CurrentBillQtyGeRemainQty");
            }

            //本次开票金额大于剩余金额
            if (actingBill.CurrentBillAmount > (actingBill.BillAmount - actingBill.BillingAmount))
            {
                throw new BusinessException("TransportActingBill.Error.CurrentBillAmountGeRemainAmount");
            }

            return billDetail;
        }


        #endregion Customized Methods

        #region TransportActingBill
        [Transaction(TransactionMode.Requires)]
        public TransportActingBill CreateTransportActingBill(string orderNo)
        {
            TransportOrderMaster order = genericMgr.FindById<TransportOrderMaster>(orderNo);
            TransportActingBill actBill = new TransportActingBill();
            IList<TransportActingBillDetail> transportActingBillDetailList = new List<TransportActingBillDetail>();
            if (!string.IsNullOrEmpty(order.Expense))
            {
                #region 费用单
                //actBill.BillAmount = order.Expense.Amount;
                //actBill.UnitPrice = order.Expense.Amount;
                //actBill.BillQty = 1;
                //actBill.Currency = order.Expense.Currency;
                //actBill.IsIncludeTax = order.Expense.IsIncludeTax;
                //actBill.Currency.Code = order.Expense.Currency.Code;
                //actBill.IsProvisionalEstimate = false;
                #endregion

            }
            else
            {
                TransportFlowMaster tFlow = genericMgr.FindById<TransportFlowMaster>(order.Flow);
                TransportPriceList tPriceList = genericMgr.FindById<TransportPriceList>(order.PriceList);
                string currency = tPriceList.Currency;
                IList<TransportPriceListDetail> tPriceListDetailList = genericMgr.FindAll<TransportPriceListDetail>("from TransportPriceListDetail t where t.PriceList = ?", order.PriceList);



                #region 包车
                if (order.PricingMethod == CodeMaster.TransportPricingMethod.Chartered)
                {
                    TransportPriceListDetail tPriceListDetail = tPriceListDetailList.Where(p => p.ShipFrom == order.ShipFrom && p.ShipTo == order.ShipTo && p.PricingMethod == order.PricingMethod && p.Tonnage == order.Tonnage && p.StartDate < order.StartDate && (p.EndDate == null || p.EndDate > order.StartDate)).FirstOrDefault();

                    if (tPriceListDetail == null)
                    {
                        throw new BusinessException("没有找到对应的价格单明细");
                    }

                    actBill.BillQty = 1;

                    actBill.UnitPrice = tPriceListDetail.UnitPrice;
                    actBill.BillAmount = actBill.UnitPrice * actBill.BillQty;

                }
                #endregion

                #region 体积
                if (order.PricingMethod == CodeMaster.TransportPricingMethod.Volume)
                {
                    decimal totalVolume = 0;
                    TransportPriceListDetail tPriceListDetail = null;
                    foreach (TransportOrderDetail d in order.TransportOrderDetailList)
                    {
                        TransportActingBillDetail transportActingBillDetail = new TransportActingBillDetail();
                        tPriceListDetail = tPriceListDetailList.Where(p => p.ShipFrom == d.ShipFrom && p.ShipTo == d.ShipTo && p.Tonnage == order.Tonnage && p.PricingMethod == order.PricingMethod && p.StartDate < order.StartDate && (p.EndDate == null || p.EndDate > order.StartDate)).FirstOrDefault();

                        if (tPriceListDetail == null)
                        {
                            throw new BusinessException("没有找到对应的价格单明细");
                        }

                        #region 最小起运量
                        if (d.Volume.Value < tPriceListDetail.MinVolume)
                        {
                            totalVolume += tPriceListDetail.MinVolume;
                        }
                        else
                        {
                            totalVolume += d.Volume.Value;
                        }
                        #endregion

                        transportActingBillDetail.BillQty = totalVolume;
                        transportActingBillDetail.BillAmount = totalVolume * tPriceListDetail.UnitPrice;
                        transportActingBillDetail.Currency = tPriceListDetail.Currency;
                        transportActingBillDetail.IpNo = d.IpNo;
                        transportActingBillDetail.IsIncludeTax = tPriceList.IsIncludeTax;
                        transportActingBillDetail.PriceListDetail = tPriceListDetail.Id;
                        transportActingBillDetail.ShipFrom = d.ShipFrom;
                        transportActingBillDetail.ShipFromAddress = d.ShipFromAddress;
                        transportActingBillDetail.ShipTo = d.ShipTo;
                        transportActingBillDetail.ShipToAddress = d.ShipToAddress;
                        transportActingBillDetail.TaxCode = tPriceList.Tax;
                        transportActingBillDetail.UnitPrice = tPriceListDetail.UnitPrice;
                        transportActingBillDetailList.Add(transportActingBillDetail);
                    }

                    actBill.BillQty = totalVolume;
                    if (tPriceListDetail != null && actBill.UnitPrice == 0)
                    {
                        actBill.UnitPrice = tPriceListDetail.UnitPrice;
                    }
                    actBill.BillAmount = actBill.UnitPrice * actBill.BillQty;

                }
                #endregion

                #region 阶梯体积
                if (order.PricingMethod == CodeMaster.TransportPricingMethod.LadderVolume)
                {

                    decimal totalVolume = 0;
                    decimal totalAmount = 0;
                    TransportPriceListDetail tPriceListDetail = null;
                    foreach (TransportOrderDetail d in order.TransportOrderDetailList)
                    {
                        TransportActingBillDetail transportActingBillDetail = new TransportActingBillDetail();
                        decimal detailVolume = d.Volume.Value;
                        #region 最小起运量
                        if (detailVolume < tPriceListDetail.MinVolume)
                        {
                            detailVolume = tPriceListDetail.MinVolume;
                        }
                        #endregion

                        tPriceListDetail = tPriceListDetailList.Where(p => p.ShipFrom == d.ShipFrom && p.ShipTo == d.ShipTo
                           && p.PricingMethod == order.PricingMethod && p.StartDate < order.StartDate && (p.EndDate == null || p.EndDate > order.StartDate)
                           && p.StartQty < detailVolume && (!p.EndQty.HasValue || p.EndQty.Value > detailVolume)).FirstOrDefault();
                        if (tPriceListDetail == null)
                        {
                            throw new BusinessException("没有找到对应的价格单明细");
                        }


                        #region 账单明细
                        decimal minPrice = tPriceListDetail.MinPrice.HasValue ? tPriceListDetail.MinPrice.Value : 0;
                        transportActingBillDetail.BillQty = detailVolume;
                        transportActingBillDetail.BillAmount = minPrice + detailVolume * tPriceListDetail.UnitPrice;
                        transportActingBillDetail.Currency = tPriceListDetail.Currency;
                        transportActingBillDetail.IpNo = d.IpNo;
                        transportActingBillDetail.IsIncludeTax = tPriceList.IsIncludeTax;
                        transportActingBillDetail.PriceListDetail = tPriceListDetail.Id;
                        transportActingBillDetail.ShipFrom = d.ShipFrom;
                        transportActingBillDetail.ShipFromAddress = d.ShipFromAddress;
                        transportActingBillDetail.ShipTo = d.ShipTo;
                        transportActingBillDetail.ShipToAddress = d.ShipToAddress;
                        transportActingBillDetail.TaxCode = tPriceList.Tax;
                        transportActingBillDetail.UnitPrice = tPriceListDetail.UnitPrice;
                        transportActingBillDetailList.Add(transportActingBillDetail);
                        #endregion

                        totalVolume += detailVolume;
                        totalAmount += transportActingBillDetail.BillAmount;
                    }

                    #region 头
                    actBill.BillQty = totalVolume;
                    actBill.BillAmount = totalAmount;
                    #endregion
                }
                #endregion

                #region 重量
                if (order.PricingMethod == CodeMaster.TransportPricingMethod.Weight)
                {
                    decimal totalWeight = 0;
                    TransportPriceListDetail tPriceListDetail = null;
                    foreach (TransportOrderDetail d in order.TransportOrderDetailList)
                    {
                        TransportActingBillDetail transportActingBillDetail = new TransportActingBillDetail();
                        tPriceListDetail = tPriceListDetailList.Where(p => p.ShipFrom == d.ShipFrom && p.ShipTo == d.ShipTo && p.PricingMethod == order.PricingMethod && p.Tonnage == order.Tonnage && p.StartDate < order.StartDate && (p.EndDate == null || p.EndDate > order.StartDate)).FirstOrDefault();

                        if (tPriceListDetail == null)
                        {
                            throw new BusinessException("没有找到对应的价格单明细");
                        }

                        #region 最小起运量
                        if (d.Weight.Value < tPriceListDetail.MinWeight)
                        {
                            totalWeight += tPriceListDetail.MinWeight;
                        }
                        else
                        {
                            totalWeight += d.Weight.Value;
                        }
                        #endregion

                        transportActingBillDetail.BillQty = totalWeight;
                        transportActingBillDetail.BillAmount = totalWeight * tPriceListDetail.UnitPrice;
                        transportActingBillDetail.Currency = tPriceListDetail.Currency;
                        transportActingBillDetail.IpNo = d.IpNo;
                        transportActingBillDetail.IsIncludeTax = tPriceList.IsIncludeTax;
                        transportActingBillDetail.PriceListDetail = tPriceListDetail.Id;
                        transportActingBillDetail.ShipFrom = d.ShipFrom;
                        transportActingBillDetail.ShipFromAddress = d.ShipFromAddress;
                        transportActingBillDetail.ShipTo = d.ShipTo;
                        transportActingBillDetail.ShipToAddress = d.ShipToAddress;
                        transportActingBillDetail.TaxCode = tPriceList.Tax;
                        transportActingBillDetail.UnitPrice = tPriceListDetail.UnitPrice;
                        transportActingBillDetailList.Add(transportActingBillDetail);
                    }

                    actBill.BillQty = totalWeight;
                    if (tPriceListDetail != null && actBill.UnitPrice == 0)
                    {
                        actBill.UnitPrice = tPriceListDetail.UnitPrice;
                    }
                    actBill.BillAmount = actBill.UnitPrice * actBill.BillQty;


                }
                #endregion

                #region 距离
                if (order.PricingMethod == CodeMaster.TransportPricingMethod.Distance)
                {
                    TransportPriceListDetail tPriceListDetail = tPriceListDetailList.Where(p => p.ShipFrom == order.ShipFrom && p.ShipTo == order.ShipTo && p.PricingMethod == order.PricingMethod && p.Tonnage == order.Tonnage && p.StartDate < order.StartDate && (p.EndDate == null || p.EndDate > order.StartDate)).FirstOrDefault();
                    if (tPriceListDetail == null)
                    {
                        throw new BusinessException("没有找到对应的价格单明细");
                    }
                    IList<TransportOrderRoute> tRouteList = genericMgr.FindAll<TransportOrderRoute>("from TransportOrderRoute t where t.OrderNo = ?", order.OrderNo);

                    decimal totalDistance = tRouteList.Sum(p => p.Distance.HasValue ? p.Distance.Value : 0);

                    actBill.BillQty = totalDistance;
                    actBill.UnitPrice = tPriceListDetail.UnitPrice;
                    actBill.BillAmount = actBill.UnitPrice * actBill.BillQty;
                }
                #endregion
            }

            actBill.OrderNo = order.OrderNo;
            actBill.BillAddress = order.BillAddress;
            actBill.EffectiveDate = DateTime.Parse(order.CreateDate.ToString("yyyy-MM-dd"));
            actBill.Type = CodeMaster.BillType.Transport;

            genericMgr.Create(actBill);

            #region 保存头
            if (transportActingBillDetailList != null && transportActingBillDetailList.Count > 0)
            {
                foreach (TransportActingBillDetail d in transportActingBillDetailList)
                {
                    d.ActBill = actBill.Id;
                    genericMgr.Create(d);
                }
            }
            #endregion
            return actBill;
        }

        [Transaction(TransactionMode.Requires)]
        private void UpdateTransportActingBill(TransportBillDetail billDetail)
        {
            #region 增加新TransportBillDetail的数量和金额
            if (billDetail != null)
            {
                TransportActingBill actingBill = this.genericMgr.FindById<TransportActingBill>(billDetail.ActBill);

                actingBill.BillingQty += billDetail.BillQty;
                actingBill.BillingAmount += billDetail.BillAmount;
                if ((actingBill.BillQty > 0 && actingBill.BillQty < actingBill.BillingQty)
                    || (actingBill.BillQty < 0 && actingBill.BillQty > actingBill.BillingQty))
                {
                    throw new BusinessException("TransportActingBill.Error.CurrentBillQtyGeRemainQty");
                }

                if ((actingBill.BillAmount > 0 && actingBill.BillAmount < actingBill.BillingAmount)
                   || (actingBill.BillAmount < 0 && actingBill.BillAmount > actingBill.BillingAmount))
                {
                    throw new BusinessException("TransportActingBill.Error.CurrentBillAmountGeRemainAmount");
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
        private void ReverseTransportActingBill(TransportBillDetail billDetail)
        {
            #region 扣减旧TransportBillDetail的数量和金额
            TransportActingBill actingBill = this.genericMgr.FindById<TransportActingBill>(billDetail.ActBill);
            actingBill.BillingQty -= billDetail.BillQty;
            actingBill.BillingAmount -= billDetail.BillAmount;
            actingBill.BillingQty += billDetail.CurrentBillQty;
            actingBill.BillingAmount += billDetail.CurrentBillAmount;

            if ((actingBill.BillQty > 0 && actingBill.BillQty < actingBill.BillingQty)
                || (actingBill.BillQty < 0 && actingBill.BillQty > actingBill.BillingQty))
            {
                throw new BusinessException("TransportActingBill.Error.CurrentBillQtyGeRemainQty");
            }

            if ((actingBill.BillAmount > 0 && actingBill.BillAmount < actingBill.BillingAmount)
                || (actingBill.BillAmount < 0 && actingBill.BillAmount > actingBill.BillingAmount))
            {
                throw new BusinessException("TransportActingBill.Error.CurrentBillAmountGeRemainAmount");
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
        public void RecalculatePrice(IList<TransportActingBill> actingBillList)
        {
            if (actingBillList != null && actingBillList.Count > 0)
            {
                foreach (TransportActingBill actingBill in actingBillList)
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
        public IList<TransportActingBill> GetRecalculatePrice(CodeMaster.BillType billType, string party, string flow,
            string receiptNo, string externalReceiptNo, string item, string currency, DateTime startDate, DateTime endDate, bool includeNoEstPrice)
        {
            string hql = @"select a from TransportActingBill as a where Type = ? and EffectiveDate >= ? and EffectiveDate <= ? and IsClose=? ";
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

            var actingBillList = this.genericMgr.FindAll<TransportActingBill>(hql, para.ToArray());
            var newTransportActingBillList = new List<TransportActingBill>();
            if (actingBillList != null && actingBillList.Count > 0)
            {
                foreach (TransportActingBill actingBill in actingBillList)
                {
                    TransportPriceListDetail priceListDetail = new TransportPriceListDetail();

                    if (priceListDetail != null &&//正式价不能更新成暂估价
                        !priceListDetail.IsProvEst)
                    {
                        actingBill.IsIncludeTax = false; //待开票明细都是不含税金额

                        if (actingBill.IsIncludeTax)   //如果价格单含税，待开票金额要转为不含税金额
                        {
                            Tax tax = this.genericMgr.FindById<Tax>(actingBill.TaxCode);
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
                        newTransportActingBillList.Add(actingBill);
                    }
                }
            }
            return newTransportActingBillList;
        }



        #endregion TransportActingBill



    }
}
