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

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class ContainerMgrImpl : BaseMgr, IContainerMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IHuMgr huMgr { get; set; }



        [Transaction(TransactionMode.Requires)]
        public IList<ContainerDetail> CreateContainer(string containerCode, int qty)
        {
            IList<ContainerDetail> containerDetailList = new List<ContainerDetail>();

            Container container = genericMgr.FindById<Container>(containerCode);
            for (int i = 0; i < qty; i++)
            {
                ContainerDetail containerDetail = new ContainerDetail();
                containerDetail.IsEmpty = true;
                containerDetail.ContainerId = numberControlMgr.GetContainerId("COT"); ;
                containerDetail.ActiveDate = DateTime.Now;
                containerDetail.Container = container.Code;
                containerDetail.ContainerDescription = container.Description;
                containerDetail.ContainerQty = container.Qty;
                containerDetail.ContainerType = container.InventoryType;
                containerDetail.Location = string.Empty;
                this.genericMgr.Create(containerDetail);

                containerDetailList.Add(containerDetail);
            }
            return containerDetailList;
        }
    }
}
