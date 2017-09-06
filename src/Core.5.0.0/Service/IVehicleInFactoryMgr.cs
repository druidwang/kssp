using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.CUST;

namespace com.Sconit.Service
{
    public interface IVehicleInFactoryMgrImpl
    {

        void AddVehicleInFactory(string ipNo, IList<VehicleInFactoryDetail> vehicleInFactoryDetailList);
        
        void CreateVehicleInFactory(VehicleInFactoryMaster vehicleInFactoryMaster);

        void CloseVehicleInFactoryDetail(int id);

        void CloseVehicleInFactoryDetail(VehicleInFactoryDetail vehicleInFactoryDetail);

        void TryCloseVehicleInFactory(string orderNo);

        void TryCloseVehicleInFactory(VehicleInFactoryMaster vehicleInFactoryMaster);
        
    }
}
