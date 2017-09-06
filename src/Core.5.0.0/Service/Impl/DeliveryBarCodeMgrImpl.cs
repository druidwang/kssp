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
using com.Sconit.Entity.WMS;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class DeliveryBarCodeMgrImpl : BaseMgr, IDeliveryBarCodeMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public IList<DeliveryBarCode> CreateDeliveryBarCode(IList<ShipPlan> shipPlanList)
        {
            IList<DeliveryBarCode> deliveryBarCodeList = new List<DeliveryBarCode>();
            foreach (ShipPlan shipPlan in shipPlanList)
            {
                IDictionary<string, decimal> deliveryBarCodeDic = numberControlMgr.GetDeliveryBarCode(shipPlan);
                if (deliveryBarCodeDic != null && deliveryBarCodeDic.Count > 0)
                {
                    foreach (string barCode in deliveryBarCodeDic.Keys)
                    {
                        DeliveryBarCode dc = new DeliveryBarCode();
                        dc.BarCode = barCode;
                        dc.Flow = shipPlan.Flow;
                        dc.Dock = shipPlan.Dock;
                        dc.IsPickHu = shipPlan.IsShipScanHu;

                        dc.ShipPlanId = shipPlan.Id;
                        dc.Item = shipPlan.Item;
                        dc.ItemDescription = shipPlan.ItemDescription;
                        dc.Qty = deliveryBarCodeDic[barCode];
                        dc.ReferenceItemCode = shipPlan.ReferenceItemCode;
                        dc.UnitCount = shipPlan.UnitCount;
                        dc.Uom = shipPlan.Uom;
                        dc.UnitCountDescription = shipPlan.UnitCountDescription;
                        dc.LocationFrom = shipPlan.LocationFrom;
                        dc.LocationFromName = shipPlan.LocationFromName;
                        dc.LocationTo = shipPlan.LocationTo;
                        dc.LocationToName = shipPlan.LocationToName;
                        dc.ShipFrom = shipPlan.ShipFrom;
                        dc.ShipFromAddress = shipPlan.ShipFromAddress;
                        dc.ShipFromCell = shipPlan.ShipFromCell;
                        dc.ShipFromContact = shipPlan.ShipFromContact;
                        dc.ShipFromFax = shipPlan.ShipFromFax;
                        dc.ShipFromTel = shipPlan.ShipFromTel;
                        dc.ShipTo = shipPlan.ShipTo;
                        dc.ShipToAddress = shipPlan.ShipToAddress;
                        dc.ShipToCell = shipPlan.ShipToCell;
                        dc.ShipToContact = shipPlan.ShipToContact;
                        dc.ShipToFax = shipPlan.ShipToFax;
                        dc.ShipToTel = shipPlan.ShipToTel;
                        dc.StartTime = shipPlan.StartTime;
                        dc.Station = shipPlan.Station;
                        dc.OrderNo = shipPlan.OrderNo;
                        dc.OrderSequence = shipPlan.OrderSequence;
                        dc.PartyFrom = shipPlan.PartyFrom;
                        dc.PartyFromName = shipPlan.PartyFromName;
                        dc.PartyTo = shipPlan.PartyTo;
                        dc.PartyToName = shipPlan.PartyToName;
                        dc.IsActive = false;
                        this.genericMgr.Create(dc);
                        deliveryBarCodeList.Add(dc);
                    }
                }
            }
            return deliveryBarCodeList;
        }


    }
}
