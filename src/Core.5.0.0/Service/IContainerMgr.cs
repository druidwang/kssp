using System;
using System.Collections.Generic;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Service
{
    public interface IContainerMgr
    {
        IList<ContainerDetail> CreateContainer(string containerCode, int qty);

     //   Hu CloneContainer(ContainerDetail oldContainerDetail, int qty);
    }
}
