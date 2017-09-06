using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.WMS;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using Castle.Services.Transaction;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class ShipPlanMgrImpl : IShipPlanMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public IOrderMgr orderMgr { get; set; }

        public void CreateShipPlan(string orderNo)
        {
            User user = SecurityContextHolder.Get();
            genericMgr.ExecuteUpdateWithNativeQuery("exec USP_WMS_CreateShipPlan ?, ?, ?", new object[] { orderNo, user.Id, user.FullName });
        }

        public void CancelShipPlan(string orderNo)
        {
            throw new NotImplementedException();
        }

        public void AssignShipPlan(IList<ShipPlan> shipPlanList, string assignUser)
        {
            if (shipPlanList != null && shipPlanList.Count > 0)
            {
                User lastModifyUser = SecurityContextHolder.Get();
                User user = genericMgr.FindById<User>(Convert.ToInt32(assignUser));
                foreach (ShipPlan p in shipPlanList)
                {
                    p.ShipUserId = user.Id;
                    p.ShipUserName = user.FullName;
                    p.LastModifyDate = DateTime.Now;
                    p.LastModifyUserId = lastModifyUser.Id;
                    p.LastModifyUserName = lastModifyUser.FullName;
                    genericMgr.Update(p);
                }

            }
        }
    }
}