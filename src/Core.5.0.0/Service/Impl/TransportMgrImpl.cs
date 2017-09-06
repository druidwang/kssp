using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Entity.TMS;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using com.Sconit.CodeMaster;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class TransportMgrImpl : BaseMgr, ITransportMgr
    {
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public ITransportBillMgr transportBillMgr { get; set; }

        #region public methods
        public TransportOrderMaster TransferFlow2Order(string flowCode)
        {
            if (string.IsNullOrEmpty(flowCode))
            {
                throw new BusinessException("运输路线代码不能为空");
            }

            TransportFlowMaster transportFlowMaster = this.genericMgr.FindAll<TransportFlowMaster>("from TransportFlowMaster where Code = ?", flowCode).SingleOrDefault();

            TransportOrderMaster transportOrderMaster = new TransportOrderMaster();

            return transportOrderMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public string CreateTransportOrder(TransportOrderMaster transportOrderMaster, IDictionary<int, string> shipAddressDic, IList<string> ipNoList)
        {
            #region 是否生成运输单号校验
            if (!string.IsNullOrWhiteSpace(transportOrderMaster.OrderNo))
            {
                throw new TechnicalException("已经生成运输单号。");
            }
            #endregion

            #region 必输字段校验
            if (string.IsNullOrWhiteSpace(transportOrderMaster.Flow))
            {
                throw new BusinessException("运输路线代码不能为空");
            }

            if (string.IsNullOrWhiteSpace(transportOrderMaster.Vehicle))
            {
                throw new BusinessException("运输工具不能为空。");
            }

            if (string.IsNullOrWhiteSpace(transportOrderMaster.Carrier))
            {
                throw new BusinessException("承运商不能为空。");
            }
            #endregion

            #region 字段合法性校验
            TransportFlowMaster transportFlowMaster = this.genericMgr.FindAll<TransportFlowMaster>("from TransportFlowMaster where Code = ?", transportOrderMaster.Flow).SingleOrDefault();
            if (transportFlowMaster == null)
            {
                throw new BusinessException("运输路线{0}不存在。", transportOrderMaster.Flow);
            }
            else
            {
                transportOrderMaster.FlowDescription = transportFlowMaster.Description;
                transportOrderMaster.MinLoadRate = transportFlowMaster.MinLoadRate;
                transportOrderMaster.IsAutoRelease = transportFlowMaster.IsAutoRelease;
                //transportOrderMaster.IsAutoStart = transportFlowMaster.IsAutoStart;
                transportOrderMaster.MultiSitePick = transportFlowMaster.MultiSitePick;
            }

            //IList<TransportFlowRoute> transportFlowRouteList = this.genericMgr.FindAll<TransportFlowRoute>("from TransportFlowRoute where Flow = ? order by Sequence", transportOrderMaster.Flow);
            //if (transportFlowRouteList == null || transportFlowRouteList.Count < 2)
            //{
            //    throw new BusinessException("运输路线{0}没有维护足够的运输站点。", transportOrderMaster.Flow);
            //}
            //else
            //{
            //    transportOrderMaster.ShipFrom = transportFlowRouteList.First().ShipAddress;
            //    transportOrderMaster.ShipFromAddress = transportFlowRouteList.First().ShipAddressDescription;
            //    transportOrderMaster.ShipTo = transportFlowRouteList.Last().ShipAddress;
            //    transportOrderMaster.ShipToAddress = transportFlowRouteList.Last().ShipAddressDescription;
            //}

            //Carrier carrier = genericMgr.FindAll<Carrier>("from Carrier where Code = ?", transportOrderMaster.Carrier).FirstOrDefault();

            //if (carrier == null)
            //{
            //    throw new BusinessException("承运商代码{0}不存在。", transportOrderMaster.Carrier);
            //}
            //else
            //{
            //    transportOrderMaster.CarrierName = carrier.Name;
            //}

            TransportFlowCarrier transportFlowCarrier = genericMgr.FindAll<TransportFlowCarrier>("from TransportFlowCarrier where Flow = ? and Carrier = ? and TransportMode = ?",
                new object[] { transportFlowMaster.Code, transportOrderMaster.Carrier, transportOrderMaster.TransportMode }).FirstOrDefault();

            if (transportFlowCarrier != null)
            {
                transportFlowCarrier.PriceList = transportFlowCarrier.PriceList;
                transportFlowCarrier.BillAddress = transportFlowCarrier.BillAddress;
            }

            //if (!string.IsNullOrWhiteSpace(transportOrderMaster.Vehicle))
            //{
            //    Vehicle vehicle = genericMgr.FindAll<Vehicle>("from Vehicle where Code = ?", transportOrderMaster.Vehicle).FirstOrDefault();

            //    if (vehicle == null)
            //    {
            //        throw new BusinessException("运输工具{0}不存在。", transportOrderMaster.Vehicle);
            //    }

            //    if (vehicle.Carrier != carrier.Code)
            //    {
            //        throw new BusinessException("运输工具{0}不属于承运商{1}。", transportOrderMaster.Vehicle, transportOrderMaster.Carrier);
            //    }

            //    Tonnage tonnage = genericMgr.FindAll<Tonnage>("from Tonnage where Code = ?", vehicle.Tonnage).FirstOrDefault();

            //    if (tonnage == null)
            //    {
            //        throw new BusinessException("运输工具{0}的吨位代码{1}不存在。", vehicle.Code, transportOrderMaster.Tonnage);
            //    }

            //    transportOrderMaster.Tonnage = tonnage.Code;
            //    transportOrderMaster.LoadVolume = tonnage.LoadVolume;
            //    transportOrderMaster.LoadWeight = tonnage.LoadWeight;
            //    transportOrderMaster.DrivingNo = vehicle.DrivingNo;

            //    if (string.IsNullOrWhiteSpace(transportOrderMaster.Driver) && !string.IsNullOrWhiteSpace(vehicle.Driver))
            //    {
            //        Driver driver = genericMgr.FindAll<Driver>("from Driver where Code = ?", vehicle.Driver).FirstOrDefault();
            //        if (driver != null)
            //        {
            //            transportOrderMaster.Driver = driver.Code;
            //            transportOrderMaster.DriverMobilePhone = driver.MobilePhone;
            //            transportOrderMaster.LicenseNo = driver.LicenseNo;
            //        }
            //    }
            //}

            //if (!string.IsNullOrWhiteSpace(transportOrderMaster.Driver))
            //{
            //    Driver driver = genericMgr.FindAll<Driver>("from Driver where Code = ?", transportOrderMaster.Driver).FirstOrDefault();

            //    if (driver == null)
            //    {
            //        throw new BusinessException("驾驶员{0}不存在。", transportOrderMaster.Driver);
            //    }

            //    transportOrderMaster.Driver = driver.Code;
            //    transportOrderMaster.DriverMobilePhone = driver.MobilePhone;
            //    transportOrderMaster.LicenseNo = driver.LicenseNo;
            //}
            #endregion

            #region 准备运单
            string orderNo = numberControlMgr.GetTransportOrderNo(transportOrderMaster);
            transportOrderMaster.OrderNo = orderNo;
            if (ipNoList != null)
            {
                ipNoList = ipNoList.Distinct().ToArray();
            }

            IList<TransportOrderRoute> transportOrderRouteList = PrepareTransportOrderRoute(orderNo, transportOrderMaster.TransportMode, shipAddressDic);

            IList<TransportOrderDetail> transportOrderDetailList = PrepareTransportOrderDetail(orderNo, transportOrderMaster.TransportMode, transportOrderMaster.MultiSitePick, ipNoList, transportOrderRouteList);

            if (transportOrderRouteList == null || transportOrderRouteList.Count < 2)
            {
                throw new BusinessException("运输站点不能小于2个。");
            }
            else
            {
                transportOrderMaster.ShipFrom = transportOrderRouteList.First().ShipAddress;
                transportOrderMaster.ShipFromAddress = transportOrderRouteList.First().ShipAddressDescription;
                transportOrderMaster.ShipTo = transportOrderRouteList.Last().ShipAddress;
                transportOrderMaster.ShipToAddress = transportOrderRouteList.Last().ShipAddressDescription;
            }
            #endregion

            #region 创建运单
            this.genericMgr.Create(transportOrderMaster);
            foreach (TransportOrderRoute transportOrderRoute in transportOrderRouteList)
            {
                this.genericMgr.Create(transportOrderRoute);
            }
            transportOrderMaster.TransportOrderRouteList = transportOrderRouteList;

            foreach (TransportOrderDetail transportOrderDetail in transportOrderDetailList)
            {
                TransportOrderRoute transportOrderRouteFrom = transportOrderRouteList.Where(r => r.ShipAddress == transportOrderDetail.ShipFrom).OrderBy(r => r.Sequence).First();
                transportOrderDetail.OrderRouteFrom = transportOrderRouteFrom.Id;
                transportOrderDetail.OrderRouteTo = transportOrderRouteList.Where(r => r.ShipAddress == transportOrderDetail.ShipTo && r.Sequence > transportOrderRouteFrom.Sequence).OrderBy(r => r.Sequence).First().Id;
                this.genericMgr.Create(transportOrderDetail);
            }
            transportOrderMaster.TransportOrderDetailList = transportOrderDetailList;
            #endregion

            if (transportOrderMaster.IsAutoRelease)
            {
                ReleaseTransportOrderMaster(transportOrderMaster);
            }

            return orderNo;
        }

        [Transaction(TransactionMode.Requires)]
        public void AddTransportOrderRoute(string orderNo, int seq, string shipAddress)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new TechnicalException("运输单号不能为空。");
            }

            if (seq <= 0)
            {
                throw new TechnicalException("序号不能小于等于0。");
            }

            if (string.IsNullOrWhiteSpace(shipAddress))
            {
                throw new TechnicalException("站点不能为空。");
            }

            TransportOrderMaster transportOrderMaster = this.genericMgr.FindAll<TransportOrderMaster>("from TransportOrderMaster where OrderNo = ?", orderNo).SingleOrDefault();
            if (transportOrderMaster == null)
            {
                throw new BusinessException("运输单号{0}不存在。", orderNo);
            }

            if (transportOrderMaster.Status == TransportStatus.Close)
            {
                throw new BusinessException("运输单号{0}已经关闭，不能添加站点。", orderNo);
            }

            if (transportOrderMaster.Status == TransportStatus.Cancel)
            {
                throw new BusinessException("运输单号{0}已经取消，不能添加站点。", orderNo);
            }

            Address shipAddressInstance = genericMgr.FindAll<Address>("from Address where Code = ?", shipAddress).SingleOrDefault();

            if (shipAddressInstance == null)
            {
                throw new BusinessException("站点{0}不存在。", shipAddress);
            }

            if (!transportOrderMaster.MultiSitePick && seq == 1)
            {
                throw new BusinessException("不能改变运输单{0}的始发站点。", orderNo);
            }

            IList<TransportOrderRoute> transportOrderRouteList = genericMgr.FindAll<TransportOrderRoute>("from TransportOrderRoute where OrderNo = ? ", orderNo);

            var prevTransportOrderRoute = transportOrderRouteList.Where(r => r.Sequence < seq).SingleOrDefault();
            var nextTransportOrderRouteList = transportOrderRouteList.Where(r => r.Sequence >= seq);

            TransportOrderRoute transportOrderRoute = new TransportOrderRoute();
            transportOrderRoute.OrderNo = orderNo;
            transportOrderRoute.Sequence = nextTransportOrderRouteList.Count() > 0 ? seq : prevTransportOrderRoute.Sequence + 1;
            transportOrderRoute.ShipAddress = shipAddress;
            transportOrderRoute.ShipAddressDescription = shipAddressInstance.AddressContent;
            transportOrderRoute.Distance = prevTransportOrderRoute != null ?
                CalculateShipDistance(prevTransportOrderRoute.ShipAddress, transportOrderRoute.ShipAddress, transportOrderMaster.TransportMode)
                : 0;

            this.genericMgr.Create(transportOrderRoute);

            for (int i = 0; i < nextTransportOrderRouteList.Count(); i++)
            {
                TransportOrderRoute nextTransportOrderRoute = nextTransportOrderRouteList.ElementAt(i);
                nextTransportOrderRoute.Sequence++;
                if (i == 0)
                {
                    nextTransportOrderRoute.Distance = CalculateShipDistance(transportOrderRoute.ShipAddress, nextTransportOrderRoute.ShipAddress, transportOrderMaster.TransportMode);
                }

                this.genericMgr.Update(nextTransportOrderRoute);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void AddTransportOrderDetail(string orderNo, IList<string> ipNoList)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new TechnicalException("运输单号不能为空。");
            }

            TransportOrderMaster transportOrderMaster = this.genericMgr.FindAll<TransportOrderMaster>("from TransportOrderMaster where OrderNo = ?", orderNo).SingleOrDefault();
            if (transportOrderMaster == null)
            {
                throw new BusinessException("运输单号{0}不存在。", orderNo);
            }

            if (transportOrderMaster.Status == TransportStatus.Close)
            {
                throw new BusinessException("运输单号{0}已经关闭，不能添加ASN。", orderNo);
            }

            if (transportOrderMaster.Status == TransportStatus.Cancel)
            {
                throw new BusinessException("运输单号{0}已经取消，不能添加ASN。", orderNo);
            }

            if (ipNoList != null)
            {
                ipNoList = ipNoList.Distinct().ToArray();
            }

            IList<TransportOrderRoute> transportOrderRouteList = this.genericMgr.FindAll<TransportOrderRoute>("from TransportOrderRoute where OrderNo = ?", orderNo);
            IList<TransportOrderDetail> transportOrderDetailList = PrepareTransportOrderDetail(orderNo, transportOrderMaster.TransportMode, transportOrderMaster.MultiSitePick, ipNoList, transportOrderRouteList);

            this.CalculateShipLoad(transportOrderDetailList);

            foreach (TransportOrderDetail TransportOrderDetail in transportOrderDetailList)
            {
                this.genericMgr.Create(TransportOrderDetail);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteTransportOrderRoute(string orderNo, int transportOrderRouteId)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new TechnicalException("运输单号不能为空。");
            }

            if (transportOrderRouteId <= 0)
            {
                throw new TechnicalException("站点Id不能小于等于0。");
            }

            TransportOrderMaster transportOrderMaster = this.genericMgr.FindAll<TransportOrderMaster>("from TransportOrderMaster where OrderNo = ?", orderNo).SingleOrDefault();
            if (transportOrderMaster == null)
            {
                throw new BusinessException("运输单号{0}不存在。", orderNo);
            }

            if (transportOrderMaster.Status == TransportStatus.Close)
            {
                throw new BusinessException("运输单号{0}已经关闭，不能删除站点。", orderNo);
            }

            if (transportOrderMaster.Status == TransportStatus.Cancel)
            {
                throw new BusinessException("运输单号{0}已经取消，不能删除站点。", orderNo);
            }

            IList<TransportOrderRoute> transportOrderRouteList = genericMgr.FindAll<TransportOrderRoute>("from TransportOrderRoute where OrderNo = ? ", orderNo);

            TransportOrderRoute transportOrderRoute = transportOrderRouteList.Where(r => r.Id == transportOrderRouteId).SingleOrDefault();

            if (transportOrderRoute == null)
            {
                throw new BusinessException("删除的站点不存在。");
            }

            if (!transportOrderMaster.MultiSitePick && transportOrderRoute.Sequence == 1)
            {
                throw new BusinessException("不能删除运输单{0}的始发站点。", orderNo);
            }

            if (transportOrderRoute.IsArrive)
            {
                throw new BusinessException("已经过站点{0}，不能删除。", transportOrderRoute.ShipAddress);
            }

            IList<TransportOrderDetail> transportOrderDetailList = genericMgr.FindAll<TransportOrderDetail>("from TransportOrderDetail where RouteFromId = ? or RouteToId = ?", new object[] { transportOrderRouteId, transportOrderRouteId });

            if (transportOrderDetailList.Count() > 0)
            {
                throw new BusinessException("站点{0}有需要送货的ASN，不能删除。", transportOrderRoute.ShipAddress);
            }

            var prevTransportOrderRoute = transportOrderRouteList.Where(r => r.Sequence < transportOrderRoute.Sequence).SingleOrDefault();
            var nextTransportOrderRouteList = transportOrderRouteList.Where(r => r.Sequence > transportOrderRoute.Sequence);

            for (int i = 0; i < nextTransportOrderRouteList.Count(); i++)
            {
                TransportOrderRoute nextTransportOrderRoute = nextTransportOrderRouteList.ElementAt(i);
                nextTransportOrderRoute.Sequence--;
                if (i == 0)
                {
                    nextTransportOrderRoute.Distance = CalculateShipDistance(prevTransportOrderRoute.ShipAddress, nextTransportOrderRoute.ShipAddress, transportOrderMaster.TransportMode);
                }

                this.genericMgr.Update(nextTransportOrderRoute);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteTransportOrderDetail(string orderNo, IList<string> ipNoList)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new TechnicalException("运输单号不能为空。");
            }

            if (ipNoList == null || ipNoList.Count() == 0)
            {
                throw new TechnicalException("ASN号不能为空。");
            }

            TransportOrderMaster transportOrderMaster = this.genericMgr.FindAll<TransportOrderMaster>("from TransportOrderMaster where OrderNo = ?", orderNo).SingleOrDefault();
            if (transportOrderMaster == null)
            {
                throw new BusinessException("运输单号{0}不存在。", orderNo);
            }

            if (transportOrderMaster.Status == TransportStatus.Close)
            {
                throw new BusinessException("运输单号{0}已经关闭，不能删除ASN。", orderNo);
            }

            if (transportOrderMaster.Status == TransportStatus.Cancel)
            {
                throw new BusinessException("运输单号{0}已经取消，不能删除ASN。", orderNo);
            }

            if (ipNoList != null)
            {
                ipNoList = ipNoList.Distinct().ToArray();
            }

            StringBuilder selectTransportOrderDetailHql = null;
            StringBuilder selectIpMasterHql = null;
            IList<object> parms = new List<object>();
            foreach (string ipNo in ipNoList)
            {
                if (selectTransportOrderDetailHql == null)
                {
                    selectTransportOrderDetailHql = new StringBuilder("from TransportOrderDetail where OrderNo = ? and IpNo in (?");
                    selectIpMasterHql = new StringBuilder("from IpMaster where IpNo in (?");
                    parms.Add(orderNo);
                }
                else
                {
                    selectTransportOrderDetailHql.Append(",?");
                    selectIpMasterHql.Append(",?");
                }

                parms.Add(ipNo);
            }
            selectTransportOrderDetailHql.Append(")");
            selectIpMasterHql.Append(")");

            IList<TransportOrderDetail> transportOrderDetailList = genericMgr.FindAll<TransportOrderDetail>(selectTransportOrderDetailHql.ToString(), parms.ToArray());
            parms.RemoveAt(0);
            IList<IpMaster> ipMasterList = genericMgr.FindAll<IpMaster>(selectIpMasterHql.ToString(), parms.ToArray());

            foreach (string ipNo in ipNoList)
            {
                IpMaster ipMaster = ipMasterList.Where(i => i.IpNo == ipNo).SingleOrDefault();

                if (ipMaster == null)
                {
                    throw new BusinessException("ASN号{0}不存在。", ipNo);
                }

                if (transportOrderDetailList.Where(d => d.IpNo == ipNo).Count() == 0)
                {
                    throw new BusinessException("ASN{0}不在运输单{1}中，不能删除。", new object[] { ipNo, orderNo });
                }

                if (ipMaster.Status == IpStatus.InProcess || ipMaster.Status == IpStatus.Close)
                {
                    throw new BusinessException("ASN号{0}已经收货不能删除。", ipNo);
                }
            }

            foreach (TransportOrderDetail transportOrderDetail in transportOrderDetailList)
            {
                genericMgr.Delete(transportOrderDetail);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseTransportOrderMaster(string orderNo)
        {
            #region 必输字段校验
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new BusinessException("运单号不能为空。");
            }
            #endregion

            #region 加载运单
            TransportOrderMaster transportOrderMaster = this.genericMgr.FindAll<TransportOrderMaster>("from TransportOrderMaster where OrderNo = ?", orderNo).SingleOrDefault();

            if (transportOrderMaster == null)
            {
                throw new BusinessException("运单号{0}不存在。", orderNo);
            }

            transportOrderMaster.TransportOrderRouteList = this.genericMgr.FindAll<TransportOrderRoute>("from TransportOrderRoute where OrderNo = ? order by Sequence", orderNo);
            transportOrderMaster.TransportOrderDetailList = this.genericMgr.FindAll<TransportOrderDetail>("from TransportOrderDetail where OrderNo = ?", orderNo);
            #endregion

            ReleaseTransportOrderMaster(transportOrderMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void PrepareTransportOrderDetail(TransportOrderDetail transportOrderDetail)
        {
            #region 计算体积和重量
            IList<IpDetail> ipDetailList = genericMgr.FindAll<IpDetail>("from IpDetail where IpNo = ?", transportOrderDetail.IpNo);
            decimal totalPackageVolume = ipDetailList.Sum(p => p.PackageVolume * p.Qty / p.UnitCount);
            decimal totalPackageWeight = ipDetailList.Sum(p => p.PackageWeight * p.Qty / p.UnitCount);
            transportOrderDetail.Volume = totalPackageVolume;
            transportOrderDetail.Weight = totalPackageWeight;
            #endregion
        }

        private void ReleaseTransportOrderMaster(TransportOrderMaster transportOrderMaster)
        {
            #region 运单校验
            if (transportOrderMaster.Status != TransportStatus.Create)
            {
                throw new BusinessException("运单{0}不是创建状态不能释放。", transportOrderMaster.OrderNo);
            }

            if (transportOrderMaster.TransportOrderRouteList.Count < 2)
            {
                throw new BusinessException("运单{0}至少应该包含2个运输站点。", transportOrderMaster.OrderNo);
            }

            if (!transportOrderMaster.MultiSitePick && transportOrderMaster.TransportOrderDetailList.Count == 0)
            {
                throw new BusinessException("运单{0}没有添加ASN。", transportOrderMaster.OrderNo);
            }

            if (string.IsNullOrWhiteSpace(transportOrderMaster.Carrier))
            {
                throw new BusinessException("运单{0}的承运商不能为空。", transportOrderMaster.OrderNo);
            }

            if (transportOrderMaster.TransportMode == TransportMode.Land)
            {
                if (string.IsNullOrWhiteSpace(transportOrderMaster.Vehicle))
                {
                    throw new BusinessException("运单{0}的运输工具不能为空。", transportOrderMaster.OrderNo);
                }

                if (string.IsNullOrWhiteSpace(transportOrderMaster.Driver))
                {
                    throw new BusinessException("运单{0}的驾驶员不能为空。", transportOrderMaster.OrderNo);
                }

                if (!transportOrderMaster.MultiSitePick
                    && transportOrderMaster.MinLoadRate.HasValue
                    && string.IsNullOrWhiteSpace(transportOrderMaster.Tonnage))
                {
                    throw new BusinessException("运输工具{0}的吨位不能为空。", transportOrderMaster.Vehicle);
                }
            }

            Carrier carrier = this.genericMgr.FindAll<Carrier>("from Carrier where Code = ?", transportOrderMaster.Carrier).SingleOrDefault();

            if (carrier == null)
            {
                throw new BusinessException("承运商代码{0}不存在。", transportOrderMaster.Carrier);
            }

            #region 计算装载率
            this.CalculateShipLoad(transportOrderMaster.TransportOrderDetailList);
            TransportOrderRoute firstTransportOrderRoute = transportOrderMaster.TransportOrderRouteList.OrderBy(r => r.Sequence).First();
            firstTransportOrderRoute.WeightRate = transportOrderMaster.TransportOrderDetailList.Sum(d => d.Weight) / transportOrderMaster.LoadWeight;
            firstTransportOrderRoute.LoadRate = transportOrderMaster.TransportOrderDetailList.Sum(d => d.Volume) / transportOrderMaster.LoadWeight;

            if (transportOrderMaster.MinLoadRate.HasValue && transportOrderMaster.MinLoadRate.Value > firstTransportOrderRoute.LoadRate)
            {
                throw new BusinessException("运输单{0}不满足最小装载率{1}。", transportOrderMaster.OrderNo, transportOrderMaster.MinLoadRate);
            }
            #endregion
            #endregion

            #region 确定运输站点
            if (!transportOrderMaster.MultiSitePick)
            {
                IList<TransportOrderRoute> removeTransportOrderRouteList = new List<TransportOrderRoute>();
                IList<TransportOrderRoute> targetTransportOrderRouteList = new List<TransportOrderRoute>();

                int seq = 2;
                foreach (TransportOrderRoute transportOrderRoute in transportOrderMaster.TransportOrderRouteList)
                {
                    if (transportOrderRoute.Sequence == 1)
                    {
                        //不能更改始发站点
                        continue;
                    }

                    if (transportOrderMaster.TransportOrderDetailList.Where(d => d.ShipFrom == transportOrderRoute.ShipAddress
                        || d.ShipTo == transportOrderRoute.ShipAddress).Count() > 0)
                    {
                        transportOrderRoute.Sequence = seq++;
                        targetTransportOrderRouteList.Add(transportOrderRoute);
                    }
                    else
                    {
                        removeTransportOrderRouteList.Add(transportOrderRoute);
                    }
                }

                if (targetTransportOrderRouteList.Count <= 1)
                {
                    throw new BusinessException("运单{0}的站点不行小于2个。", transportOrderMaster.OrderNo);
                }

                transportOrderMaster.TransportOrderRouteList = targetTransportOrderRouteList;

                foreach (TransportOrderRoute transportOrderRoute in transportOrderMaster.TransportOrderRouteList)
                {
                    this.genericMgr.Update(transportOrderRoute);
                }

                foreach (TransportOrderRoute removeTransportOrderRoute in removeTransportOrderRouteList)
                {
                    this.genericMgr.Delete(removeTransportOrderRoute);
                }
            }
            #endregion

            #region 释放运单
            transportOrderMaster.ShipFrom = transportOrderMaster.TransportOrderRouteList.First().ShipAddress;
            transportOrderMaster.ShipFromAddress = transportOrderMaster.TransportOrderRouteList.First().ShipAddressDescription;
            transportOrderMaster.ShipTo = transportOrderMaster.TransportOrderRouteList.Last().ShipAddress;
            transportOrderMaster.ShipToAddress = transportOrderMaster.TransportOrderRouteList.Last().ShipAddressDescription;
            transportOrderMaster.CurrentArriveSiteId = transportOrderMaster.TransportOrderRouteList.First().Id;
            transportOrderMaster.CurrentArriveShipAddress = transportOrderMaster.TransportOrderRouteList.First().ShipAddress;
            transportOrderMaster.CurrentArriveShipAddressDescription = transportOrderMaster.TransportOrderRouteList.First().ShipAddressDescription;
            transportOrderMaster.Status = TransportStatus.Submit;
            transportOrderMaster.SubmitDate = DateTime.Now;
            transportOrderMaster.SubmitUserId = SecurityContextHolder.Get().Id;
            transportOrderMaster.SubmitUserName = SecurityContextHolder.Get().FullName;

            this.genericMgr.Update(transportOrderMaster);
            #endregion

            if (transportOrderMaster.IsAutoStart)
            {
                StartTransportOrderMaster(transportOrderMaster);
            }
        }

        public void StartTransportOrderMaster(String orderNo)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new BusinessException("运单号不能为空。");
            }

            TransportOrderMaster transportOrderMaster = this.genericMgr.FindAll<TransportOrderMaster>("from TransportOrderMaster where OrderNo = ?", orderNo).SingleOrDefault();
            if (transportOrderMaster == null)
            {
                throw new BusinessException("运单号{0}不存在。", orderNo);
            }
        }

        public void StartTransportOrderMaster(TransportOrderMaster transportOrderMaster)
        {
            if (transportOrderMaster == null)
            {
                throw new BusinessException("运单不存在。");
            }

            if (transportOrderMaster.Status != TransportStatus.Submit)
            {
                throw new BusinessException("运单{0}不是释放状态不能放行。", transportOrderMaster.OrderNo);
            }

            transportOrderMaster.Status = TransportStatus.InProcess;
            transportOrderMaster.StartDate = DateTime.Now;
            transportOrderMaster.StartUserId = SecurityContextHolder.Get().Id;
            transportOrderMaster.StartUserName = SecurityContextHolder.Get().FullName;

            this.genericMgr.Update(transportOrderMaster);
        }
        

        [Transaction(TransactionMode.Requires)]
        public void Calculate(string orderNo)
        {
             transportBillMgr.CreateTransportActingBill(orderNo);
        }
        #endregion

        #region private methods
        private IList<TransportOrderRoute> PrepareTransportOrderRoute(string orderNo, TransportMode transportMode, IDictionary<int, string> shipAddressDic)
        {
            IList<TransportOrderRoute> transportOrderRouteList = new List<TransportOrderRoute>();
            TransportOrderRoute prevTransportOrderRoute = null;
            IList<Address> addressList = null;

            if (shipAddressDic != null && shipAddressDic.Count > 0)
            {
                StringBuilder selectHql = null;
                IList<object> parms = new List<object>();
                foreach (var shipAddress in shipAddressDic)
                {
                    if (selectHql == null)
                    {
                        selectHql = new StringBuilder("from Address where Code in (?");
                    }
                    else
                    {
                        selectHql.Append(", ?");
                    }

                    parms.Add(shipAddress.Value);
                }
                selectHql.Append(")");
                addressList = genericMgr.FindAll<Address>(selectHql.ToString(), parms.ToArray());
            }

            foreach (var shipAddress in shipAddressDic.OrderBy(s => s.Key))
            {
                Address address = addressList.Where(a => a.Code == shipAddress.Value).SingleOrDefault();
                if (address == null)
                {
                    throw new BusinessException("站点{0}不存在。", shipAddress.Value);
                }

                TransportOrderRoute transportOrderRoute = new TransportOrderRoute();
                transportOrderRoute.OrderNo = orderNo;
                transportOrderRoute.Sequence = shipAddress.Key;
                transportOrderRoute.ShipAddress = address.Code;
                transportOrderRoute.ShipAddressDescription = address.CodeAddressContent;
                transportOrderRoute.Distance = prevTransportOrderRoute != null ?
                    CalculateShipDistance(prevTransportOrderRoute.ShipAddress, transportOrderRoute.ShipAddress, transportMode)
                    : 0;
                transportOrderRouteList.Add(transportOrderRoute);

                prevTransportOrderRoute = transportOrderRoute;
            }

            return transportOrderRouteList;
        }

        private IList<TransportOrderDetail> PrepareTransportOrderDetail(string orderNo, TransportMode transportMode, bool multiSitePick, IList<string> ipNoList, IList<TransportOrderRoute> transportOrderRouteList)
        {
            IList<TransportOrderDetail> transportOrderDetailList = new List<TransportOrderDetail>();
            if (ipNoList != null && ipNoList.Count > 0)
            {
                StringBuilder selectIpMasterHql = null;
                StringBuilder selectIpDetailHql = null;
                StringBuilder selectItemHql = null;
                StringBuilder verifyIpNoHql = null;
                IList<object> parms = new List<object>();
                foreach (string ipNo in ipNoList)
                {
                    if (selectIpMasterHql == null)
                    {
                        selectIpMasterHql = new StringBuilder("from IpMaster where IpNo in (?");
                        selectIpDetailHql = new StringBuilder("from IpDetail where IpNo in (?");
                        selectItemHql = new StringBuilder("from Item where code in (select Item from IpDetail where IpNo in (?");
                        verifyIpNoHql = new StringBuilder("select IpNo from TransportOrderDetail where IpNo in (?");
                    }
                    else
                    {
                        selectIpMasterHql.Append(", ?");
                        selectIpDetailHql.Append(", ?");
                        selectItemHql.Append(", ?");
                        verifyIpNoHql.Append(", ?");
                    }

                    parms.Add(ipNo);
                }
                selectIpMasterHql.Append(")");
                selectIpDetailHql.Append(")");
                selectItemHql.Append("))");
                verifyIpNoHql.Append(")");

                IList<IpMaster> ipMasterList = genericMgr.FindAll<IpMaster>(selectIpMasterHql.ToString(), parms.ToArray());
                IList<IpDetail> ipDetailList = genericMgr.FindAll<IpDetail>(selectIpDetailHql.ToString(), parms.ToArray());
                IList<Item> itemList = genericMgr.FindAll<Item>(selectItemHql.ToString(), parms.ToArray());
                IList<string> createdIpNoList = genericMgr.FindAll<string>(verifyIpNoHql.ToString(), parms.ToArray());

                if (createdIpNoList.Count > 0)
                {
                    throw new BusinessException("ASN号{0}已经创建了运输单。", String.Join(", ", createdIpNoList.ToArray()));
                }

                int seq = 1;
                foreach (string ipNo in ipNoList)
                {
                    IpMaster ipMaster = ipMasterList.Where(m => m.IpNo == ipNo).SingleOrDefault();
                    if (ipMaster == null)
                    {
                        throw new BusinessException("ASN号{0}不存在。", ipNo);
                    }

                    if (ipMaster.Status != IpStatus.Submit)
                    {
                        throw new BusinessException("ASN号{0}状态不是{1}不能添加至运输单。", ipNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.IpStatus, ((int)IpStatus.Submit)).ToString());
                    }

                    TransportOrderRoute transportOrderRouteFrom =
                        transportOrderRouteList.Where(r => r.ShipAddress == ipMaster.ShipFrom && !r.IsArrive).OrderBy(r => r.Sequence).FirstOrDefault();

                    if (!multiSitePick && transportOrderRouteFrom.Sequence != 1)
                    {
                        throw new BusinessException("ASN号{0}发货地址不是运输单的始发站点。", ipNo);
                    }

                    if (transportOrderRouteFrom == null)
                    {
                        throw new BusinessException("ASN号{0}发货地址不是运输单经过的站点。", ipNo);
                    }

                    TransportOrderRoute transportOrderRouteTo =
                       transportOrderRouteList.Where(r => r.ShipAddress == ipMaster.ShipTo && !r.IsArrive && r.Sequence > transportOrderRouteFrom.Sequence).OrderBy(r => r.Sequence).FirstOrDefault();

                    if (transportOrderRouteTo == null)
                    {
                        throw new BusinessException("ASN号{0}收货地址不是运输单经过的站点。", ipNo);
                    }

                    TransportOrderDetail transportOrderDetail = new TransportOrderDetail();
                    transportOrderDetail.OrderNo = orderNo;
                    transportOrderDetail.Sequence = seq++;
                    transportOrderDetail.IpNo = ipNo;
                    transportOrderDetail.OrderRouteFrom = transportOrderRouteFrom.Id;
                    transportOrderDetail.OrderRouteTo = transportOrderRouteTo.Id;
                    transportOrderDetail.EstPalletQty = Convert.ToInt32(Math.Ceiling(ipDetailList.Sum(
                        d => (d.PalletLotSize > 0 ? (d.Qty / d.PalletLotSize) : ((itemList.Where(i => i.Code == d.Item).Single().PalletLotSize > 0 ? (d.Qty / itemList.Where(i => i.Code == d.Item).Single().PalletLotSize) : 0))))));
                    transportOrderDetail.EstVolume = ipDetailList.Sum(
                        d => (d.UnitCount > 0 ? (d.Qty / d.UnitCount) : ((itemList.Where(i => i.Code == d.Item).Single().UnitCount > 0 ? (d.Qty / itemList.Where(i => i.Code == d.Item).Single().UnitCount) : 0))) * d.PackageVolume);
                    transportOrderDetail.EstVolume = ipDetailList.Sum(
                        d => (d.UnitCount > 0 ? (d.Qty / d.UnitCount) : ((itemList.Where(i => i.Code == d.Item).Single().UnitCount > 0 ? (d.Qty / itemList.Where(i => i.Code == d.Item).Single().UnitCount) : 0))) * d.PackageWeight);
                    transportOrderDetail.EstBoxCount = Convert.ToInt32(ipDetailList.Sum(
                        d => Math.Ceiling((d.UnitCount > 0 ? (d.Qty / d.UnitCount) : ((itemList.Where(i => i.Code == d.Item).Single().UnitCount > 0 ? (d.Qty / itemList.Where(i => i.Code == d.Item).Single().UnitCount) : 0))))));
                    transportOrderDetail.LoadTime = DateTime.Now;
                    transportOrderDetail.PartyFrom = ipMaster.PartyFrom;
                    transportOrderDetail.PartyFromName = ipMaster.PartyFromName;
                    transportOrderDetail.PartyTo = ipMaster.PartyTo;
                    transportOrderDetail.PartyToName = ipMaster.PartyToName;
                    transportOrderDetail.ShipFrom = ipMaster.ShipFrom;
                    transportOrderDetail.ShipFromAddress = ipMaster.ShipFromAddress;
                    transportOrderDetail.ShipFromTel = ipMaster.ShipFromTel;
                    transportOrderDetail.ShipFromCell = ipMaster.ShipFromCell;
                    transportOrderDetail.ShipFromFax = ipMaster.ShipFromFax;
                    transportOrderDetail.ShipFromContact = ipMaster.ShipFromContact;
                    transportOrderDetail.ShipTo = ipMaster.ShipTo;
                    transportOrderDetail.ShipToAddress = ipMaster.ShipToAddress;
                    transportOrderDetail.ShipToTel = ipMaster.ShipToTel;
                    transportOrderDetail.ShipToCell = ipMaster.ShipToCell;
                    transportOrderDetail.ShipToFax = ipMaster.ShipToFax;
                    transportOrderDetail.ShipToContact = ipMaster.ShipToContact;
                    transportOrderDetail.Dock = ipMaster.Dock;
                    transportOrderDetail.Distance =
                        CalculateShipDistance(transportOrderDetail.ShipFrom, transportOrderDetail.ShipTo, transportMode);
                    transportOrderDetail.IsReceived = false;

                    transportOrderDetailList.Add(transportOrderDetail);
                }
            }

            return transportOrderDetailList;
        }

        private decimal? CalculateShipDistance(string shipFrom, string shipTo, TransportMode transportMode)
        {
            Mileage mileage = genericMgr.FindAll<Mileage>("from Mileage where ShipFrom = ? and ShipTo = ? and TransportMode = ? and IsActive = ?",
                new object[] { shipFrom, shipTo, transportMode, true }).FirstOrDefault();

            if (mileage != null)
            {
                return mileage.Distance;
            }
            else
            {
                return null;
            }
        }

        private void CalculateShipLoad(IList<TransportOrderDetail> transportOrderDetailList)
        {
            #region 查找所有ASN明细
            StringBuilder selectIpDetailHql = null;
            IList<object> selectIpDetailParms = new List<object>();
            foreach (TransportOrderDetail transportOrderDetail in transportOrderDetailList)
            {
                if (selectIpDetailHql == null)
                {
                    selectIpDetailHql = new StringBuilder("from IpDet where IpNo in (?");
                }
                else
                {
                    selectIpDetailHql.Append(", ?");
                }

                selectIpDetailParms.Add(transportOrderDetail.IpNo);
            }
            selectIpDetailHql.Append(")");

            IList<IpDetail> ipDetailList = genericMgr.FindAll<IpDetail>(selectIpDetailHql.ToString(), selectIpDetailParms.ToArray());
            #endregion

            #region 查找ASN用的托盘
            StringBuilder selectOrderDetPalletHql = null;
            IList<object> selectOrderDetPalletParms = new List<object>();
            foreach (TransportOrderDetail transportOrderDetail in transportOrderDetailList)
            {
                if (selectOrderDetPalletHql == null)
                {
                    selectOrderDetPalletHql = new StringBuilder("from TransportOrderDetailPallet where TransportOrderDetailId in (?");
                }
                else
                {
                    selectOrderDetPalletHql.Append(", ?");
                }

                selectOrderDetPalletParms.Add(transportOrderDetail.Id);
            }
            selectOrderDetPalletHql.Append(")");

            IList<TransportOrderDetailPallet> transportOrderDetailPalletList = genericMgr.FindAll<TransportOrderDetailPallet>(selectOrderDetPalletHql.ToString(), selectOrderDetPalletParms.ToArray());
            #endregion

            #region 查找托盘类型
            StringBuilder selectPalletHql = null;
            IList<object> selectPalletParms = new List<object>();
            foreach (IpDetail ipDetail in ipDetailList)
            {
                if (string.IsNullOrWhiteSpace(ipDetail.PalletCode)
                    && !selectPalletParms.Contains(ipDetail.PalletCode))
                {
                    if (selectPalletHql == null)
                    {
                        selectPalletHql = new StringBuilder("from Pallet where Code in (?");
                    }
                    else
                    {
                        selectPalletHql.Append(", ?");
                    }

                    selectPalletParms.Add(ipDetail.PalletCode);
                }
            }

            foreach (TransportOrderDetailPallet transportOrderDetailPallet in transportOrderDetailPalletList)
            {
                if (string.IsNullOrWhiteSpace(transportOrderDetailPallet.PalletCode)
                    && !selectPalletParms.Contains(transportOrderDetailPallet.PalletCode))
                {
                    if (selectPalletHql == null)
                    {
                        selectPalletHql = new StringBuilder("from Pallet where Code in (?");
                    }
                    else
                    {
                        selectPalletHql.Append(", ?");
                    }

                    selectPalletParms.Add(transportOrderDetailPallet.PalletCode);
                }
            }

            selectPalletHql.Append(")");

            IList<Pallet> palletList = genericMgr.FindAll<Pallet>(selectPalletHql.ToString(), selectPalletParms.ToArray());
            #endregion

            #region 按ASN汇总计算重量和体积
            var ipDetailLoad = from ipDetail in ipDetailList
                               select new
                               {
                                   IpNo = ipDetail.IpNo,
                                   PalletCode = ipDetail.PalletCode,
                                   EstBoxCount = ipDetail.UnitCount != 0 ? (ipDetail.Qty / ipDetail.UnitCount) : (decimal?)null,
                                   EstPalletQty = ipDetail.PalletLotSize != 0 ? ipDetail.Qty / ipDetail.PalletLotSize : (decimal?)null,
                                   EstVolume = ipDetail.UnitCount != 0 ? ipDetail.PackageVolume * ipDetail.Qty / ipDetail.UnitCount : (decimal?)null,
                                   EstWeight = ipDetail.UnitCount != 0 ? ipDetail.PackageWeight * ipDetail.Qty / ipDetail.UnitCount : (decimal?)null,
                               };


            var groupedIpDetailLoad = from i in ipDetailLoad
                                      group i by new { IpNo = i.IpNo, PalletCode = i.PalletCode } into result
                                      select new
                                      {
                                          IpNo = result.Key.IpNo,
                                          PalletCode = result.Key.PalletCode,
                                          EstBoxCount = result.Sum(i => i.EstBoxCount),
                                          EstPalletQty = result.Sum(i => i.EstPalletQty),
                                          EstVolume = result.Sum(i => i.EstVolume),
                                          EstWeight = result.Sum(i => i.EstWeight),
                                      };

            var orderDetailPalletLoad = from odp in transportOrderDetailPalletList
                                        join p in palletList on odp.PalletCode equals p.Code
                                        select new
                                        {
                                            IpNo = odp.IpNo,
                                            PalletQty = odp.PalletQty,
                                            Volume = odp.PalletQty * p.Volume,
                                            Weight = odp.PalletQty * p.Weight,
                                        };

            var groupedOrderDetailPalletLoad = from o in orderDetailPalletLoad
                                               group o by o.IpNo into g
                                               select new
                                               {
                                                   IpNo = g.Key,
                                                   PalletQty = g.Sum(o => o.PalletQty),
                                                   Volume = g.Sum(o => o.Volume),
                                                   Weight = g.Sum(o => o.Weight),
                                               };


            var groupedIpDetailWithPalletLoad = from i in groupedIpDetailLoad
                                                join p in palletList on i.PalletCode equals p.Code
                                                into q
                                                from g in q.DefaultIfEmpty()
                                                select new
                                                {
                                                    IpNo = i.IpNo,
                                                    PalletCode = i.PalletCode,
                                                    EstBoxCount = i.EstBoxCount,
                                                    EstPalletQty = i.EstPalletQty,
                                                    EstVolume = i.EstVolume + (g.Code != null && g.Volume != 0 ? i.EstPalletQty * g.Volume : 0),
                                                    EstWeight = i.EstWeight + (g.Code != null && g.Weight != 0 ? i.EstPalletQty * g.Weight : 0),
                                                };

            var groupedIpDetailWithActualPalletLoad = from i in groupedIpDetailWithPalletLoad
                                                      join p in groupedOrderDetailPalletLoad on i.IpNo equals p.IpNo
                                                      into q
                                                      from g in q.DefaultIfEmpty()
                                                      select new
                                                      {
                                                          IpNo = i.IpNo,
                                                          PalletCode = i.PalletCode,
                                                          EstBoxCount = i.EstBoxCount,
                                                          EstPalletQty = i.EstPalletQty,
                                                          EstVolume = i.EstVolume,
                                                          EstWeight = i.EstWeight,
                                                          PalletQty = g.PalletQty,
                                                          Volume = g.Volume,
                                                          Weight = g.Weight,
                                                      };
            foreach (TransportOrderDetail transportOrderDetail in transportOrderDetailList)
            {
                var p = groupedIpDetailWithActualPalletLoad.Where(g => g.IpNo == transportOrderDetail.IpNo);
                transportOrderDetail.EstBoxCount = (int)Math.Round(p.Sum(d => d.EstBoxCount).Value);
                transportOrderDetail.EstPalletQty = (int)Math.Round(p.Sum(d => d.EstPalletQty).Value);
                transportOrderDetail.EstVolume = p.Sum(d => d.EstVolume);
                transportOrderDetail.EstWeight = p.Sum(d => d.EstWeight);
                transportOrderDetail.PalletQty = p.Sum(d => d.PalletQty);
                transportOrderDetail.Volume = p.Sum(d => d.Volume);
                transportOrderDetail.Weight = p.Sum(d => d.Weight);
            }
            #endregion

        }
        #endregion
    }
}
