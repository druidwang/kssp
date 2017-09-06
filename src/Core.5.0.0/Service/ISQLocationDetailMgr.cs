using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INV;

namespace com.Sconit.Service
{
    public interface ISQLocationDetailMgr
    {
        IList<Hu> MatchNewHuForRepack(IList<OrderDetail> orderDetailList, Boolean isJIT, BusinessException businessException);

        void DistributionLabelCancel(string HuId);
    }
}
