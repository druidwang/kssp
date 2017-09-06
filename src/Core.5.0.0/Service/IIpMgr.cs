using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Service
{
    public interface IIpMgr
    {
        IpMaster TransferOrder2Ip(IList<OrderMaster> orderMasterList);
        IpMaster MergeOrderMaster2IpMaster(IList<OrderMaster> orderMasterList);
        IpMaster TransferPickList2Ip(PickListMaster pickListMaster);
        IpMaster TransferSequenceMaster2Ip(SequenceMaster sequenceMaster);
        void CreateIp(IpMaster ipMaster);
        void CreateIp(IpMaster ipMaster, DateTime effectiveDate);
        void ManualCloseIp(string IpNo);
        void ManualCloseIp(IpMaster ipMaster);
        void TryCloseIp(IpMaster ipMaster);
        void CancelIp(string IpNo);
        void CancelIp(string IpNo, DateTime effectiveDate);
        void CancelIp(IpMaster ipMaster);
        void CancelIp(IpMaster ipMaster, DateTime effectiveDate);
    }
}
