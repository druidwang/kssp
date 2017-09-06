using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.ORD;


namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class ShipmentMgrImpl : BaseMgr, IShipmentMgr
    {
        public IGenericMgr genericMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public void CreateBillofLadingMaster(ShipmentMaster shipmentMaster)
        {
            genericMgr.Create(shipmentMaster);
            foreach (var shipmentDetail in shipmentMaster.ShipmentDetails)
            {
                genericMgr.Create(shipmentDetail); 
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteBillofLadingMaster(ShipmentMaster billofLadingMaster)
        {
            genericMgr.Delete(billofLadingMaster.ShipmentDetails);
            genericMgr.Delete(billofLadingMaster);
            
        }
    }
}
