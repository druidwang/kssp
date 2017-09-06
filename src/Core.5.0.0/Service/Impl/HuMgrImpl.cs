using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.VIEW;
using com.Sconit.PrintModel.INV;
using com.Sconit.Utility;
using com.Sconit.Entity.Exception;
using NHibernate;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class HuMgrImpl : BaseMgr, IHuMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IItemMgr itemMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public IList<Hu> CreateHu(ReceiptMaster receiptMaster, ReceiptDetail receiptDetail, DateTime effectiveDate)
        {
            IList<Hu> huList = new List<Hu>();
            decimal remainReceivedQty = receiptDetail.ReceivedQty;
            receiptDetail.LotNo = LotNoHelper.GenerateLotNo(effectiveDate); //取生效日期为批号生成日期
            IDictionary<string, decimal> huIdDic = numberControlMgr.GetHuId(receiptDetail);

            if (huIdDic != null && huIdDic.Count > 0)
            {
                foreach (string huId in huIdDic.Keys)
                {
                    Hu hu = new Hu();
                    hu.HuId = huId;
                    hu.LotNo = receiptDetail.LotNo;
                    hu.Item = receiptDetail.Item;
                    hu.ItemDescription = receiptDetail.ItemDescription;
                    hu.BaseUom = receiptDetail.BaseUom;
                    hu.Qty = huIdDic[huId];
                    hu.ManufactureParty = receiptMaster.PartyFrom;   //取区域代码为制造商代码
                    hu.ManufactureDate = LotNoHelper.ResolveLotNo(receiptDetail.LotNo);
                    hu.PrintCount = 0;
                    hu.ConcessionCount = 0;
                    hu.ReferenceItemCode = receiptDetail.ReferenceItemCode;
                    hu.UnitCount = receiptDetail.UnitCount;
                    hu.UnitQty = itemMgr.ConvertItemUomQty(receiptDetail.Item, receiptDetail.Uom, 1, receiptDetail.BaseUom);
                    hu.Uom = receiptDetail.Uom;
                    hu.IsOdd = hu.Qty < receiptDetail.MinUc;
                    hu.OrderNo = receiptDetail.OrderNo;
                    hu.ReceiptNo = receiptMaster.ReceiptNo;
                    hu.Flow = receiptMaster.Flow;
                    hu.IpNo = receiptMaster.IpNo;
                    hu.IsChangeUnitCount = false;
                    hu.Remark = receiptDetail.Remark;
                    hu.Direction = receiptDetail.Direction;
                    hu.Flow = receiptDetail.Flow;
                    hu.Shift = receiptMaster.Shift;
                    //hu.UnitCountDescription = receiptDetail.UnitCountDescription;
                    var item = this.itemMgr.GetCacheItem(receiptDetail.Item);
                    hu.MaterialsGroup = this.GetMaterialsGroupDescrption(item.MaterialsGroup);
                    hu.HuTemplate = receiptMaster.HuTemplate;
                    hu.HuOption = GetHuOption(item);
                    if (item.Warranty > 0)
                    {
                        hu.ExpireDate = hu.ManufactureDate.AddDays(item.Warranty);
                    }
                    if (item.WarnLeadTime > 0)
                    {
                        hu.RemindExpireDate = hu.ManufactureDate.AddDays(item.WarnLeadTime);
                    }
                    this.genericMgr.Create(hu);
                    //this.AsyncSendPrintData(hu);
                    huList.Add(hu);
                }
            }
            return huList;
        }

        [Transaction(TransactionMode.Requires)]
        public IList<Hu> CreateHu(FlowMaster flowMaster, IList<FlowDetail> flowDetailList)
        {
            return CreateHu(flowMaster, flowDetailList, string.Empty);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<Hu> CreateHu(FlowMaster flowMaster, IList<FlowDetail> flowDetailList, string externalOrderNo, bool isPrintPallet = false)
        {
            IList<Hu> huList = new List<Hu>();
            string palletCode = string.Empty;
            int currentFlowDetailId = 0;
            foreach (FlowDetail flowDetail in flowDetailList)
            {
                ///每条做一个托盘
                if (isPrintPallet)
                {
                    if (currentFlowDetailId != flowDetail.Id)
                    {
                        palletCode = numberControlMgr.GetPalletCode();
                        currentFlowDetailId = flowDetail.Id;
                    }
                }


                IDictionary<string, decimal> huIdDic = numberControlMgr.GetHuId(flowDetail);
                if (huIdDic != null && huIdDic.Count > 0)
                {
                    foreach (string huId in huIdDic.Keys)
                    {

                        var item = this.itemMgr.GetCacheItem(flowDetail.Item);
                        if (string.IsNullOrWhiteSpace(flowDetail.ReferenceItemCode))
                        {
                            flowDetail.ReferenceItemCode = item.ReferenceCode;
                        }
                        Hu hu = new Hu();
                        hu.HuId = huId;
                        hu.LotNo = flowDetail.LotNo;
                        hu.Item = flowDetail.Item;
                        hu.ItemDescription = item.Description;
                        hu.BaseUom = flowDetail.BaseUom;
                        hu.Qty = huIdDic[huId];

                        if (flowMaster.Type == CodeMaster.OrderType.Distribution)
                        {
                            //销售的用主机厂记在制造商上面
                            hu.ManufactureParty = flowMaster.PartyTo;
                        }
                        else
                        {
                            hu.ManufactureParty = flowDetail.ManufactureParty;

                        }

                        hu.ManufactureDate = flowDetail.ManufactureDate;
                        hu.PrintCount = 0;
                        hu.ConcessionCount = 0;
                        hu.ReferenceItemCode = flowDetail.ReferenceItemCode;
                        hu.UnitCount = flowDetail.UnitCount;
                        hu.UnitQty = itemMgr.ConvertItemUomQty(flowDetail.Item, flowDetail.Uom, 1, flowDetail.BaseUom);
                        hu.Uom = flowDetail.Uom;
                        hu.IsOdd = flowDetail.HuQty < flowDetail.MinUc;
                        hu.SupplierLotNo = flowDetail.SupplierLotNo;
                        hu.IsChangeUnitCount = flowDetail.IsChangeUnitCount;
                        hu.UnitCountDescription = flowDetail.UnitCountDescription;
                        hu.ContainerDesc = flowDetail.ContainerDescription;
                        hu.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowMaster.LocationTo : flowDetail.LocationTo;
                        hu.Flow = flowDetail.Flow;
                        hu.MaterialsGroup = GetMaterialsGroupDescrption(item.MaterialsGroup);
                        hu.HuTemplate = flowMaster.HuTemplate;
                        hu.HuOption = GetHuOption(item);
                        hu.Remark = flowDetail.Remark;

                        hu.PalletCode = palletCode;
                        hu.ExternalOrderNo = externalOrderNo;

                        if (item.Warranty > 0)
                        {
                            hu.ExpireDate = hu.ManufactureDate.AddDays(item.Warranty);
                        }
                        if (item.WarnLeadTime > 0)
                        {
                            hu.RemindExpireDate = hu.ManufactureDate.AddDays(item.WarnLeadTime);
                        }
                        this.genericMgr.Create(hu);
                        //this.AsyncSendPrintData(hu);
                        huList.Add(hu);
                    }
                }
            }

            #region 托盘
            if (isPrintPallet)
            {
                var itemList = huList.Select(p => p.Item).Distinct();
                foreach (string itemCode in itemList)
                {

                    var palletHuList = huList.Where(p => p.Item == itemCode);
                    string itemPalletCode = palletHuList.First().PalletCode;
                    Pallet pallet = new Pallet();
                    pallet.Code = itemPalletCode;
                    pallet.Description = palletHuList.First().Item + "|" + palletHuList.First().ItemDescription + "|" + palletHuList.Count();
                    this.genericMgr.Create(pallet);

                    foreach (Hu hu in palletHuList)
                    {
                        PalletHu palletHu = new PalletHu();
                        palletHu.HuId = hu.HuId;
                        palletHu.PalletCode = itemPalletCode;
                        genericMgr.Create(palletHu);
                    }
                }
            }
            #endregion

            return huList;
        }

        [Transaction(TransactionMode.Requires)]
        public IList<Hu> CreateHu(OrderMaster orderMaster, IList<OrderDetail> orderDetailList, bool isScrapHu = false, bool isPrintPallet = false)
        {
            IList<Hu> huList = new List<Hu>();
            string palletCode = string.Empty;
            int orderDetailId = 0;
            foreach (OrderDetail orderDetail in orderDetailList)
            {

                ///每条做一个托盘
                if (isPrintPallet)
                {
                    if (orderDetailId != orderDetail.Id)
                    {
                        palletCode = numberControlMgr.GetPalletCode();
                        orderDetailId = orderDetail.Id;
                    }
                }


                IDictionary<string, decimal> huIdDic = numberControlMgr.GetHuId(orderDetail);
                if (huIdDic != null && huIdDic.Count > 0)
                {
                    foreach (string huId in huIdDic.Keys)
                    {
                        var item = orderDetail.CurrentItem;
                        if (item == null)
                        {
                            item = this.itemMgr.GetCacheItem(orderDetail.Item);
                        }
                        Hu hu = new Hu();
                        hu.HuId = huId;
                        hu.LotNo = orderDetail.LotNo;
                        hu.Qty = huIdDic[huId];
                        if (isScrapHu)
                        {
                            hu.Item = item.Code;
                            hu.ItemDescription = item.Description;
                            hu.BaseUom = item.Uom;
                            hu.Uom = item.Uom;
                            hu.UnitCount = item.UnitCount;
                            hu.UnitQty = 1;
                            hu.Qty = 0;
                            hu.HuTemplate = "BarCodeEXScrap.xls";
                        }
                        else
                        {
                            hu.Item = orderDetail.Item;
                            hu.ItemDescription = orderDetail.ItemDescription;
                            hu.BaseUom = orderDetail.BaseUom;
                            hu.Uom = orderDetail.Uom;
                            hu.UnitCount = orderDetail.UnitCount;
                            hu.UnitQty = orderDetail.UnitQty;
                            hu.HuTemplate = orderMaster.HuTemplate;
                        }

                        hu.ManufactureParty = orderDetail.ManufactureParty;
                        hu.ManufactureDate = orderDetail.ManufactureDate;
                        hu.PrintCount = 0;
                        hu.ConcessionCount = 0;
                        hu.ReferenceItemCode = orderDetail.ReferenceItemCode;
                        //hu.IsOdd = hu.Qty < hu.UnitCount;
                        hu.IsChangeUnitCount = orderDetail.IsChangeUnitCount;
                        hu.UnitCountDescription = orderDetail.UnitCountDescription;
                        hu.SupplierLotNo = orderDetail.SupplierLotNo;
                        hu.ContainerDesc = orderDetail.ContainerDescription;
                        hu.LocationTo = string.IsNullOrWhiteSpace(orderDetail.LocationTo) ? orderMaster.LocationTo : orderDetail.LocationTo;
                        hu.OrderNo = orderDetail.OrderNo;
                        hu.Shift = orderMaster.Shift;
                        hu.Flow = orderMaster.Flow;

                        hu.MaterialsGroup = GetMaterialsGroupDescrption(item.MaterialsGroup);
                        hu.HuOption = GetHuOption(item);
                        hu.Remark = orderDetail.Remark;
                        hu.PalletCode = palletCode;
                        hu.ExternalOrderNo = orderMaster.ExternalOrderNo;

                        if (item.Warranty > 0)
                        {
                            hu.ExpireDate = hu.ManufactureDate.AddDays(item.Warranty);
                        }
                        if (item.WarnLeadTime > 0)
                        {
                            hu.RemindExpireDate = hu.ManufactureDate.AddDays(item.WarnLeadTime);
                        }
                        this.genericMgr.Create(hu);
                        //this.AsyncSendPrintData(hu);
                        huList.Add(hu);
                    }
                }

            }

            #region 托盘
            if (isPrintPallet)
            {
                var itemList = huList.Select(p => p.Item).Distinct();
                foreach (string itemCode in itemList)
                {
                    var palletHuList = huList.Where(p => p.Item == itemCode);
                    string itemPalletCode = palletHuList.First().PalletCode;

                    Pallet pallet = new Pallet();
                    pallet.Code = itemPalletCode;
                    pallet.Description = palletHuList.First().Item + "|" + palletHuList.First().ItemDescription + "|" + palletHuList.Count();
                    this.genericMgr.Create(pallet);

                    foreach (Hu hu in palletHuList)
                    {
                        PalletHu palletHu = new PalletHu();
                        palletHu.HuId = hu.HuId;
                        palletHu.PalletCode = itemPalletCode;
                        genericMgr.Create(palletHu);
                    }
                }
            }
            #endregion

            return huList;
        }

        [Transaction(TransactionMode.Requires)]
        public IList<Hu> CreateHu(IpMaster ipMaster, IList<IpDetail> ipDetailList)
        {
            IList<Hu> huList = new List<Hu>();
            foreach (IpDetail ipDetail in ipDetailList)
            {
                IDictionary<string, decimal> huIdDic = numberControlMgr.GetHuId(ipDetail);
                if (huIdDic != null && huIdDic.Count > 0)
                {
                    foreach (string huId in huIdDic.Keys)
                    {
                        Hu hu = new Hu();
                        hu.HuId = huId;
                        hu.LotNo = ipDetail.LotNo;
                        hu.Item = ipDetail.Item;
                        hu.ItemDescription = ipDetail.ItemDescription;
                        hu.BaseUom = ipDetail.BaseUom;
                        hu.Qty = huIdDic[huId];
                        hu.ManufactureParty = ipDetail.ManufactureParty;
                        hu.ManufactureDate = ipDetail.ManufactureDate;
                        hu.PrintCount = 0;
                        hu.ConcessionCount = 0;
                        hu.ReferenceItemCode = ipDetail.ReferenceItemCode;
                        hu.UnitCount = ipDetail.UnitCount;
                        hu.UnitQty = ipDetail.UnitQty;
                        hu.Uom = ipDetail.Uom;
                        hu.IsOdd = ipDetail.HuQty < ipDetail.MinUc;
                        hu.IsChangeUnitCount = ipDetail.IsChangeUnitCount;
                        hu.UnitCountDescription = ipDetail.UnitCountDescription;
                        hu.SupplierLotNo = ipDetail.SupplierLotNo;
                        hu.ContainerDesc = ipDetail.ContainerDescription;
                        hu.LocationTo = ipDetail.LocationTo;
                        hu.OrderNo = ipDetail.OrderNo;
                        hu.IpNo = ipDetail.IpNo;
                        hu.Flow = ipDetail.Flow;
                        var item = this.itemMgr.GetCacheItem(ipDetail.Item);
                        hu.MaterialsGroup = this.GetMaterialsGroupDescrption(item.MaterialsGroup);
                        hu.HuOption = GetHuOption(item);
                        hu.HuTemplate = ipMaster.HuTemplate;
                        hu.Remark = ipDetail.Remark;
                        if (item.Warranty > 0)
                        {
                            hu.ExpireDate = hu.ManufactureDate.AddDays(item.Warranty);
                        }
                        if (item.WarnLeadTime > 0)
                        {
                            hu.RemindExpireDate = hu.ManufactureDate.AddDays(item.WarnLeadTime);
                        }
                        this.genericMgr.Create(hu);
                        //this.AsyncSendPrintData(hu);
                        huList.Add(hu);
                    }
                }
            }
            return huList;
        }

        [Transaction(TransactionMode.Requires)]
        public IList<Hu> CreateHu(Item item)
        {
            IList<Hu> huList = new List<Hu>();
            IDictionary<string, decimal> huIdDic = numberControlMgr.GetHuId(item);
            string palletCode = string.Empty;
            if (huIdDic != null && huIdDic.Count > 0)
            {
                if (item.IsPrintPallet)
                {
                    palletCode = numberControlMgr.GetPalletCode();
                }

                foreach (string huId in huIdDic.Keys)
                {
                    Hu hu = new Hu();
                    hu.HuId = huId;
                    hu.LotNo = item.LotNo;
                    hu.Item = item.Code;
                    hu.ItemDescription = item.Description;
                    hu.BaseUom = item.Uom;
                    hu.Qty = huIdDic[huId];
                    hu.ManufactureParty = item.ManufactureParty;
                    hu.ManufactureDate = item.ManufactureDate;
                    hu.PrintCount = 0;
                    hu.ConcessionCount = 0;
                    hu.ReferenceItemCode = item.ReferenceCode;
                    hu.UnitCount = item.HuUnitCount;
                    hu.UnitQty = itemMgr.ConvertItemUomQty(item.Code, item.HuUom, 1, item.Uom);
                    hu.Uom = item.HuUom;
                    hu.IsOdd = hu.Qty < hu.UnitCount;
                    hu.SupplierLotNo = item.SupplierLotNo;
                    hu.IsChangeUnitCount = true;
                    hu.ContainerDesc = item.Container;
                    hu.MaterialsGroup = this.GetMaterialsGroupDescrption(item.MaterialsGroup);
                    hu.Direction = item.Deriction;
                    hu.Remark = item.Remark;
                    hu.HuOption = item.HuOption;
                    hu.HuTemplate = item.HuTemplate;
                    hu.PalletCode = palletCode;
                    if (item.Warranty > 0)
                    {
                        hu.ExpireDate = hu.ManufactureDate.AddDays(item.Warranty);
                    }
                    if (item.WarnLeadTime > 0)
                    {
                        hu.RemindExpireDate = hu.ManufactureDate.AddDays(item.WarnLeadTime);
                    }
                    this.genericMgr.Create(hu);
                    //this.AsyncSendPrintData(hu);
                    huList.Add(hu);
                }


                #region 托盘
                if (item.IsPrintPallet)
                {

                    Pallet pallet = new Pallet();
                    pallet.Code = palletCode;
                    pallet.Description = item.Code + "|" + item.Description + "|" + huList.Count();
                    this.genericMgr.Create(pallet);

                    foreach (Hu hu in huList)
                    {
                        PalletHu palletHu = new PalletHu();
                        palletHu.HuId = hu.HuId;
                        palletHu.PalletCode = palletCode;
                        genericMgr.Create(palletHu);
                    }

                }
                #endregion

            }
            return huList;
        }


        [Transaction(TransactionMode.Requires)]
        public Hu CreateHu(string customerCode, string customerName, string lotNo, string itemCode, string itemDesc, string manufactureDate, string manufacturer, string orderNo, string uom, decimal uc, decimal qty, string createUser, string createDate, string printer, string huId)
        {
            IList<Hu> huList = new List<Hu>();
            var item = this.genericMgr.FindById<Item>(itemCode);
            Hu hu = new Hu();
            var flowMstr = this.genericMgr.FindEntityWithNativeSql<FlowMaster>("select fm.* from SCM_FlowMstr fm inner join SCM_FlowDet fd on fm.Code=fd.Flow where fm.IsActive = 1 and fm.PartyTo=? and fd.Item=?", new object[] { customerCode, itemCode });
            if (flowMstr == null || flowMstr.Count == 0)
            {
                hu.HuTemplate = item.HuTemplate;
            }
            else
            {
                hu.HuTemplate = flowMstr.FirstOrDefault().HuTemplate;
            }
            //  var tobeHuId = huId;
            IDictionary<string, decimal> huidDic = new Dictionary<string, decimal>();

            huidDic = numberControlMgr.GetHuId(lotNo, itemCode, manufacturer, qty, uc);
            //   tobeHuId = huidDic.FirstOrDefault().Key;


            hu.HuId = huidDic.FirstOrDefault().Key;
            hu.ExternalHuId = huId;
            hu.IsExternal = string.IsNullOrEmpty(huId) ? false : true;

            hu.SupplierLotNo = lotNo;
            hu.Item = item.Code;
            hu.ItemDescription = item.Description;
            hu.BaseUom = uom;
            hu.Qty = qty;
            hu.ManufactureParty = string.IsNullOrEmpty(customerCode) ? flowMstr.FirstOrDefault().PartyTo : customerCode;
            //      hu.ManufactureDate = Convert.ToDateTime(manufactureDate);
            hu.ExternalOrderNo = orderNo;
            var manufactureDt = DateTime.Now;
            DateTime.TryParseExact(manufactureDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out manufactureDt);
            hu.ManufactureDate = manufactureDt;

            hu.LotNo = Utility.LotNoHelper.GenerateLotNo(hu.ManufactureDate);
            hu.PrintCount = 0;
            hu.ConcessionCount = 0;
            hu.ReferenceItemCode = item.ReferenceCode;
            hu.UnitCount = uc;
            hu.UnitQty = 1;
            hu.Uom = uom;
            hu.IsOdd = hu.Qty < hu.UnitCount;

            hu.IsChangeUnitCount = true;
            hu.ContainerDesc = item.Container;
            hu.MaterialsGroup = this.GetMaterialsGroupDescrption(item.MaterialsGroup);
            hu.Direction = item.Deriction;
            // hu.Remark = item.Remark;
            hu.HuOption = item.HuOption;
            //hu.HuTemplate = item.HuTemplate;
            hu.Remark = createUser;
            //  hu. = createDate;
            if (item.Warranty > 0)
            {
                hu.ExpireDate = hu.ManufactureDate.AddDays(item.Warranty);
            }
            if (item.WarnLeadTime > 0)
            {
                hu.RemindExpireDate = hu.ManufactureDate.AddDays(item.WarnLeadTime);
            }
            this.genericMgr.Create(hu);
            if (string.IsNullOrEmpty(huId))
            {
                //暂时不用写
                //this.AsyncSendPrintData(hu);
            }
            //this.AsyncSendPrintData(hu);
            return hu;

        }

        [Transaction(TransactionMode.Requires)]
        public Hu CreateHu(Item item, string huId)
        {
            Hu hu = new Hu();
            hu.HuId = huId;
            hu.LotNo = item.LotNo;
            hu.Item = item.Code;
            hu.ItemDescription = item.Description;
            hu.BaseUom = item.Uom;
            hu.Qty = item.HuQty;
            hu.ManufactureParty = item.ManufactureParty;
            hu.ManufactureDate = LotNoHelper.ResolveLotNo(item.LotNo);
            hu.PrintCount = 0;
            hu.ConcessionCount = 0;
            hu.ReferenceItemCode = item.ReferenceCode;
            hu.UnitCount = item.HuUnitCount;
            hu.UnitQty = itemMgr.ConvertItemUomQty(item.Code, item.HuUom, 1, item.Uom);
            hu.Uom = item.HuUom;
            hu.IsOdd = hu.Qty < hu.UnitCount;
            hu.SupplierLotNo = item.SupplierLotNo;
            hu.IsChangeUnitCount = true;
            hu.MaterialsGroup = this.GetMaterialsGroupDescrption(item.MaterialsGroup);
            hu.HuOption = GetHuOption(item);
            hu.HuTemplate = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
            if (item.Warranty > 0)
            {
                hu.ExpireDate = hu.ManufactureDate.AddDays(item.Warranty);
            }
            if (item.WarnLeadTime > 0)
            {
                hu.RemindExpireDate = hu.ManufactureDate.AddDays(item.WarnLeadTime);
            }
            this.genericMgr.Create(hu);
            return hu;
        }



        [Transaction(TransactionMode.Requires)]
        public IList<Hu> GetHuListByPallet(string palletCode)
        {
            string hql = "select h from Hu as h inner join PalletHu as p on h.HuId = p.HuId where p.PalletCode = ?";
            IList<Hu> huList = this.genericMgr.FindAll<Hu>(hql, palletCode);
            return huList;
        }

        [Transaction(TransactionMode.Requires)]
        public Hu CloneHu(Hu oldHu, decimal qty)
        {
            if (qty <= 0)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.HuQtyMustMoreThenZore);
            }
            IDictionary<string, decimal> huIdDic = numberControlMgr.GetHuId(oldHu.LotNo, oldHu.Item, oldHu.ManufactureParty, oldHu.UnitCount, oldHu.UnitCount);
            if (huIdDic != null && huIdDic.Count > 0)
            {
                var huId = huIdDic.Keys.First();
                Hu hu = Mapper.Map<Hu, Hu>(oldHu); ;
                hu.HuId = huId;
                hu.Qty = qty;
                hu.PrintCount = 0;
                hu.ConcessionCount = 0;
                hu.IsOdd = hu.Qty < hu.UnitCount && oldHu.IsOdd;
                hu.RefHu = oldHu.HuId;

                this.genericMgr.Create(hu);
                this.AsyncSendPrintData(hu);
                return hu;
            }
            return null;
        }

        public IList<Hu> LoadHus(string[] huIdList)
        {
            //IList<object> para = new List<object>();
            //foreach (string huId in huIdList)
            //{
            //    para.Add(huId);
            //}
            return this.genericMgr.FindAllIn<Hu>("from Hu where HuId in (?", huIdList);
        }

        public IList<Hu> LoadHus(IList<string> huIdList)
        {
            return LoadHus(huIdList.ToArray());
        }

        public HuStatus GetHuStatus(string huId)
        {
            Hu hu = this.genericMgr.FindById<Hu>(huId);

            HuStatus huStatus = Mapper.Map<Hu, HuStatus>(hu);
            huStatus.Status = CodeMaster.HuStatus.NA;

            string hql = "from LocationLotDetail where HuId = ?";

            IList<LocationLotDetail> locationLotDetailList = this.genericMgr.FindAll<LocationLotDetail>(hql, huId);

            if (locationLotDetailList != null && locationLotDetailList.Count > 0)
            {
                LocationLotDetail locationLotDetail = locationLotDetailList[0];
                WrapHuStatus(huStatus, locationLotDetail);
            }

            if (huStatus.Status == CodeMaster.HuStatus.NA)
            {

                hql = "from IpLocationDetail where HuId = ? and IsClose = ?";

                IList<IpLocationDetail> ipLocationDetailList = this.genericMgr.FindAll<IpLocationDetail>(hql, new object[] { huId, false });

                if (ipLocationDetailList != null && ipLocationDetailList.Count > 0)
                {
                    IpLocationDetail ipLocationDetail = ipLocationDetailList[0];
                    WrapHuStatus(huStatus, ipLocationDetail);
                }
            }

            return huStatus;
        }

        public IList<HuStatus> GetHuStatus(IList<string> huIdList)
        {
            if (huIdList != null && huIdList.Count > 0)
            {
                IList<Hu> huList = LoadHus(huIdList);

                IList<object> paras = new List<object>();
                foreach (string huId in huIdList)
                {
                    paras.Add(huId);
                }

                IList<LocationLotDetail> locationLotDetailList = this.genericMgr.FindAllIn<LocationLotDetail>
                    ("from LocationLotDetail where HuId in (?", paras);
                IList<IpLocationDetail> ipLocationDetailList = this.genericMgr.FindAllIn<IpLocationDetail>
                    ("from IpLocationDetail where IsClose = false and HuId in (?", paras);

                IList<HuStatus> huStatusList = new List<HuStatus>();
                foreach (Hu hu in huList)
                {
                    HuStatus huStatus = Mapper.Map<Hu, HuStatus>(hu);
                    huStatus.Status = CodeMaster.HuStatus.NA;

                    if (locationLotDetailList != null && locationLotDetailList.Count > 0)
                    {
                        LocationLotDetail locationLotDetail = locationLotDetailList.Where(locDet => locDet.HuId == hu.HuId).SingleOrDefault();
                        WrapHuStatus(huStatus, locationLotDetail);
                    }

                    if (huStatus.Status == CodeMaster.HuStatus.NA)
                    {
                        IpLocationDetail ipLocationDetail = ipLocationDetailList.Where(locDet => locDet.HuId == hu.HuId).SingleOrDefault();
                        WrapHuStatus(huStatus, ipLocationDetail);
                    }
                    huStatusList.Add(huStatus);
                }

                return huStatusList;
            }

            return null;
        }


        [Transaction(TransactionMode.Requires)]
        public string CreatePallet(List<string> boxNos, string boxCount, string printer, string createUser, string createDate, string palletId)
        {

            User user = genericMgr.FindAll<User>("select u from User u where u.Code = ?", "Monitor").FirstOrDefault();

            string[] huidArray = boxNos.ToArray();

            var huPallet =  new Hu();


            IList<Hu> huList = new List<Hu>();
            foreach (string huid in huidArray)
            {
                Hu hu = genericMgr.FindAll<Hu>("select h from Hu h where h.HuId = ? and h.IsExternal = 0", huid).FirstOrDefault();
                if (hu == null)
                {
                    hu = genericMgr.FindAll<Hu>("select h from Hu h where h.ExternalHuId = ? and h.IsExternal = 1", huid).FirstOrDefault();
                    if (hu == null)
                    {
                        throw new BusinessException("箱条码" + huid + "不存在" );
                    }
                }
                if (!string.IsNullOrEmpty(hu.PalletCode))
                {
                    throw new BusinessException("箱条码{0}已在托盘中", huid);
                }
                huList.Add(hu);
            }

            var itemList = huList.Select(p => p.Item).Distinct();
            if (itemList.Count() > 1)
            {
                throw new BusinessException("箱条码对应的零件号为多个，不能打印在同一托盘");
            }

            #region 校验数量
            if (!string.IsNullOrEmpty(palletId) && palletId.StartsWith("HU"))
            {
                decimal husQty = huList.Sum(p => p.Qty);

                 huPallet = genericMgr.FindById<Hu>(palletId);
                if (huPallet.Qty != husQty)
                {
                    throw new BusinessException("箱条码总数与托条码数不匹配");
                }
            }
            #endregion

            #region 托盘

            Pallet pallet = new Pallet();
            string palletCode = string.Empty;
            if (string.IsNullOrEmpty(palletId))
            {
                palletCode = numberControlMgr.GetPalletCode();
            }
            else
            {
                palletCode = palletId;
            }
            pallet.Code = palletCode;
            pallet.Description = huList.First().Item + "|" + huList.First().ItemDescription + "|" + huList.Count();
            this.genericMgr.Create(pallet);

            foreach (Hu hu in huList)
            {
                PalletHu palletHu = new PalletHu();
                palletHu.HuId = hu.HuId;
                palletHu.PalletCode = palletCode;
                palletHu.CreateDate = DateTime.Now;
                palletHu.CreateUserId = user.Id;
                palletHu.CreateUserName = createUser;
                genericMgr.Create(palletHu);
            }
            #endregion

            #region 更新条码
            foreach (Hu h in huList)
            {
                h.PalletCode = palletCode;
                if (!string.IsNullOrEmpty(palletId) && palletId.StartsWith("HU"))
                {
                    h.ManufactureParty = huPallet.ManufactureParty;
                }
                h.LastModifyDate = DateTime.Now;
                h.LastModifyUserId = user.Id;
                h.LastModifyUserName = createUser;
                genericMgr.Update(h);
            }

            #endregion


            #region 更新托盘
            if (!string.IsNullOrEmpty(palletId))
            {
                huPallet.IsPallet = true;
                genericMgr.Update(huPallet);
            }
            #endregion

            return palletCode;
        }



        #region private methods
        private void WrapHuStatus(HuStatus huStatus, LocationLotDetail locationLotDetail)
        {
            if (locationLotDetail != null && locationLotDetail.Qty > 0)
            {
                Location location = this.genericMgr.FindById<Location>(locationLotDetail.Location);

                huStatus.Status = CodeMaster.HuStatus.Location;
                huStatus.Region = location.Region;
                huStatus.Location = locationLotDetail.Location;
                huStatus.Bin = locationLotDetail.Bin;
                huStatus.IsConsignment = locationLotDetail.IsConsignment;
                huStatus.PlanBill = locationLotDetail.PlanBill;
                huStatus.QualityType = locationLotDetail.QualityType;
                huStatus.IsFreeze = locationLotDetail.IsFreeze;
                huStatus.IsATP = locationLotDetail.IsATP;
                huStatus.OccupyType = locationLotDetail.OccupyType;
                huStatus.OccupyReferenceNo = locationLotDetail.OccupyReferenceNo;
            }
        }

        private void WrapHuStatus(HuStatus huStatus, IpLocationDetail ipLocationDetail)
        {
            if (ipLocationDetail != null && !ipLocationDetail.IsClose)
            {
                IpDetail ipDetail = this.genericMgr.FindById<IpDetail>(ipLocationDetail.IpDetailId);

                huStatus.IpNo = ipLocationDetail.IpNo;
                huStatus.LocationFrom = ipDetail.LocationFrom;
                huStatus.LocationTo = ipDetail.LocationTo;
                huStatus.Status = CodeMaster.HuStatus.Ip;
                huStatus.IsConsignment = ipLocationDetail.IsConsignment;
                huStatus.PlanBill = ipLocationDetail.PlanBill;
                huStatus.QualityType = ipLocationDetail.QualityType;
                huStatus.IsFreeze = ipLocationDetail.IsFreeze;
                huStatus.IsATP = ipLocationDetail.IsATP;
                huStatus.OccupyType = ipLocationDetail.OccupyType;
                huStatus.OccupyReferenceNo = ipLocationDetail.OccupyReferenceNo;
            }
        }

        #endregion

        #region 异步打印

        public void AsyncSendPrintData(Hu hu)
        {
            //AsyncSend asyncSend = new AsyncSend(this.SendPrintData);
            //asyncSend.BeginInvoke(hu, null, null);
            try
            {
                var subPrintOrderList = this.genericMgr.FindAll<SubPrintOrder>();
                var pubPrintOrders = subPrintOrderList.Where(p => (p.Flow == hu.Flow || string.IsNullOrWhiteSpace(p.Flow))
                            && (p.UserId == hu.CreateUserId || p.UserId == 0)
                            && (p.Region == hu.ManufactureParty || string.IsNullOrWhiteSpace(p.Region))
                            && (p.Location == hu.LocationTo || string.IsNullOrWhiteSpace(p.Location))
                            && p.ExcelTemplate == hu.HuTemplate)
                            .Select(p => new PubPrintOrder
                            {
                                Client = p.Client,
                                ExcelTemplate = p.ExcelTemplate,
                                Code = hu.HuId,
                                Printer = p.Printer
                            });
                foreach (var pubPrintOrder in pubPrintOrders)
                {
                    this.genericMgr.Create(pubPrintOrder);
                }
            }
            catch (Exception ex)
            {
                pubSubLog.Error("Send data to print sevrer error:", ex);
            }
        }

        public delegate void AsyncSend(Hu hu);
        #endregion


        private string GetMaterialsGroupDescrption(string materialsGroupCode)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(materialsGroupCode))
                {
                    var itemCategorys = this.genericMgr.FindAll<string>
                        ("select Description from ItemCategory where Code = ? and SubCategory = ? ",
                        new object[] { materialsGroupCode, (int)CodeMaster.SubCategory.MaterialsGroup },
                        new NHibernate.Type.IType[] { NHibernateUtil.String, NHibernateUtil.Int32 });
                    if (itemCategorys != null && itemCategorys.Count > 0)
                    {
                        return itemCategorys[0];
                    }
                }
            }
            catch (Exception)
            { }
            return materialsGroupCode;
        }

        #region 客户化代码
        [Transaction(TransactionMode.Requires)]
        public IList<Hu> CreateHu(OrderDetail orderDetail, Boolean isRepack, string manufactureParty, string lotNo, decimal totalQty,
            decimal unitQty, decimal huQty, string oldHus, string binTo, Boolean isRepackForOrder)
        {
            IList<Hu> huList = new List<Hu>();

            IDictionary<string, decimal> huIdDic = numberControlMgr.GetHuId(lotNo, orderDetail.Item, manufactureParty, totalQty, unitQty);
            if (huIdDic != null && huIdDic.Count > 0)
            {
                Hu hu = new Hu();
                hu.HuId = huIdDic.SingleOrDefault().Key;
                hu.LotNo = lotNo;
                hu.Item = orderDetail.Item;
                hu.ItemDescription = orderDetail.ItemDescription;
                hu.BaseUom = orderDetail.BaseUom;
                hu.Qty = huQty;
                hu.ManufactureParty = manufactureParty;
                hu.ManufactureDate = LotNoHelper.ResolveLotNo(lotNo);
                hu.PrintCount = 0;
                hu.ConcessionCount = 0;
                hu.ReferenceItemCode = orderDetail.ReferenceItemCode;
                hu.UnitCount = orderDetail.UnitCount;
                hu.UnitQty = orderDetail.UnitQty;
                hu.Uom = orderDetail.Uom;
                hu.IsOdd = huQty != hu.UnitCount;
                hu.IsChangeUnitCount = orderDetail.IsChangeUnitCount;
                hu.UnitCountDescription = orderDetail.UnitCountDescription;
                hu.BinTo = binTo;
                hu.LocationTo = orderDetail.LocationTo;
                hu.OldHus = oldHus;
                var item = this.itemMgr.GetCacheItem(orderDetail.Item);
                hu.MaterialsGroup = this.GetMaterialsGroupDescrption(item.MaterialsGroup);
                hu.Direction = orderDetail.Direction;
                hu.RefId = orderDetail.Id;
                hu.HuOption = GetHuOption(item);
                if (item.Warranty > 0)
                {
                    hu.ExpireDate = hu.ManufactureDate.AddDays(item.Warranty);
                }
                if (item.WarnLeadTime > 0)
                {
                    hu.RemindExpireDate = hu.ManufactureDate.AddDays(item.WarnLeadTime);
                }
                this.genericMgr.Create(hu);

                huList.Add(hu);
            }
            return huList;
        }
        #endregion

        public CodeMaster.HuOption GetHuOption(Item item)
        {
            if (item.ItemOption == CodeMaster.ItemOption.NeedAging)
            {
                return CodeMaster.HuOption.UnAging;
            }
            else if (item.ItemOption == CodeMaster.ItemOption.NeedFilter)
            {
                return CodeMaster.HuOption.UnFilter;
            }
            else
            {
                return CodeMaster.HuOption.NoNeed;
            }
        }
    }
}
