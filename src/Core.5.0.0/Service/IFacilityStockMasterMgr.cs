using System;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityStockMasterMgr : IFacilityStockMasterBaseMgr
    {
        #region Customized Methods

        IList<FacilityStockDetail> GetUnConfirmedStockDetailList(string stNo);

        void ConfirmStockTakeDetail(IList<int> stockTakeDetailList);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityStockMasterMgrE : com.Sconit.Facility.Service.IFacilityStockMasterMgr
    {
    }
}

#endregion Extend Interface