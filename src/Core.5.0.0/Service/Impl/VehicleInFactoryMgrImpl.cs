using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Services.Transaction;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class VehicleInFactoryMgrImpl : BaseMgr, IVehicleInFactoryMgrImpl
    {
        #region 变量
        public INumberControlMgr numberControlMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        #endregion


        public void AddVehicleInFactory(string ipNo, IList<VehicleInFactoryDetail> vehicleInFactoryDetailList)
        {
            if (vehicleInFactoryDetailList == null)
            {
                vehicleInFactoryDetailList = new List<VehicleInFactoryDetail>();
            }

            IList<IpMaster> ipMasterList = genericMgr.FindAll<IpMaster>("from IpMaster as i where i.IpNo = ? and i.Status in (?,?)", new object[] { ipNo, (int)com.Sconit.CodeMaster.IpStatus.Submit, (int)com.Sconit.CodeMaster.IpStatus.InProcess });
            if (ipMasterList == null || ipMasterList.Count() == 0)
            {
                throw new BusinessException("没有找到对应的送货单号{0}", ipNo);
            }
            IpMaster ipMaster = ipMasterList[0];



            VehicleInFactoryDetail vehicleInFactoryDetail = new VehicleInFactoryDetail();
            vehicleInFactoryDetail.IpNo = ipNo;
            vehicleInFactoryDetailList.Add(vehicleInFactoryDetail);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateVehicleInFactory(VehicleInFactoryMaster vehicleInFactoryMaster)
        {
            string hql = "select distinct(r.Plant) from IpMaster as i,Region as r where i.PartyTo = r.Code and i.IpNo in (?";
            IList<object> param = new List<object>();
            param.Add(vehicleInFactoryMaster.VehicleInFactoryDetails[0].IpNo);
            for (int i = 0; i < vehicleInFactoryMaster.VehicleInFactoryDetails.Count(); i++)
            {
                hql += ",?";
                param.Add(vehicleInFactoryMaster.VehicleInFactoryDetails[i].IpNo);
            }
            hql += ")";
            IList<string> plantList = genericMgr.FindAll<string>(hql, param.ToArray());
            if (plantList != null && plantList.Count > 1)
            {
                throw new BusinessException("送货单对应多个工厂");
            }
            vehicleInFactoryMaster.Plant = plantList[0];
            vehicleInFactoryMaster.Status = CodeMaster.VehicleInFactoryStatus.Submit;
            vehicleInFactoryMaster.OrderNo = numberControlMgr.GetVehicleInFactoryNo(vehicleInFactoryMaster);
            genericMgr.Create(vehicleInFactoryMaster);
            foreach (VehicleInFactoryDetail vehicleInFactoryDetail in vehicleInFactoryMaster.VehicleInFactoryDetails)
            {
                vehicleInFactoryDetail.OrderNo = vehicleInFactoryMaster.OrderNo;
                genericMgr.Create(vehicleInFactoryDetail);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseVehicleInFactoryDetail(int id)
        {
            CloseVehicleInFactoryDetail(genericMgr.FindById<VehicleInFactoryDetail>(id));
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseVehicleInFactoryDetail(VehicleInFactoryDetail vehicleInFactoryDetail)
        {
            vehicleInFactoryDetail.CloseDate = DateTime.Now;
            vehicleInFactoryDetail.CloseUserId = SecurityContextHolder.Get().Id;
            vehicleInFactoryDetail.CloseUserName = SecurityContextHolder.Get().FullName;
            vehicleInFactoryDetail.IsClose = true;

            genericMgr.Update(vehicleInFactoryDetail);

            TryCloseVehicleInFactory(vehicleInFactoryDetail.OrderNo);
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCloseVehicleInFactory(string orderNo)
        {
            TryCloseVehicleInFactory(genericMgr.FindById<VehicleInFactoryMaster>(orderNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCloseVehicleInFactory(VehicleInFactoryMaster vehicleInFactoryMaster)
        {
            if (vehicleInFactoryMaster.Status != com.Sconit.CodeMaster.VehicleInFactoryStatus.Close)
            {
                this.genericMgr.FlushSession();

                string hql = "select count(*) as counter from VehicleInFactoryDetail where OrderNo = ? and IsClose = ?";
                long counter = genericMgr.FindAll<long>(hql, new object[] { vehicleInFactoryMaster.OrderNo, false })[0];
                if (counter == 0)
                {
                    vehicleInFactoryMaster.Status = com.Sconit.CodeMaster.VehicleInFactoryStatus.Close;
                    vehicleInFactoryMaster.CloseDate = DateTime.Now;
                    vehicleInFactoryMaster.CloseUserId = SecurityContextHolder.Get().Id;
                    vehicleInFactoryMaster.CloseUserName = SecurityContextHolder.Get().FullName;

                    genericMgr.Update(vehicleInFactoryMaster);
                }
                else if(vehicleInFactoryMaster.Status == com.Sconit.CodeMaster.VehicleInFactoryStatus.Submit)
                {
                    vehicleInFactoryMaster.Status = com.Sconit.CodeMaster.VehicleInFactoryStatus.InProcess;
                    genericMgr.Update(vehicleInFactoryMaster);
                }
            }
        }

        private IList<VehicleInFactoryDetail> TryLoadVehicleInFactoryDetailList(VehicleInFactoryMaster vehicleInFactoryMaster)
        {

            if (!string.IsNullOrWhiteSpace(vehicleInFactoryMaster.OrderNo))
            {
                if (vehicleInFactoryMaster.VehicleInFactoryDetails == null)
                {
                    string hql = "from VehicleInFactoryDetail where OrderNo = ?";

                    vehicleInFactoryMaster.VehicleInFactoryDetails = this.genericMgr.FindAll<VehicleInFactoryDetail>(hql, vehicleInFactoryMaster.OrderNo);
                }

                return vehicleInFactoryMaster.VehicleInFactoryDetails;
            }
            else
            {
                return null;
            }

        }
    }
}
