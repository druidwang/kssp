using System.Collections.Generic;
using com.Sconit.Entity.ORD;
using System;
using com.Sconit.Entity.WMS;

namespace com.Sconit.Service
{
    public interface IPackingListMgr
    {
        PackingList CreatePackingList(string flow,IList<string> huIdList);

        void Ship(IList<string> packingListCode);
    }
}
