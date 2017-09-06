using System;
using System.Collections.Generic;
using com.Sconit.Entity.ORD;
using System.IO;

namespace com.Sconit.Service
{
    public interface IShipmentMgr
    {
        #region 运单创建
        void CreateBillofLadingMaster(ShipmentMaster shipmentMaster);

        void DeleteBillofLadingMaster(ShipmentMaster shipmentMaster);
        #endregion
    }
}
