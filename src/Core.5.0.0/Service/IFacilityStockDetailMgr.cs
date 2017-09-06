using System;
using com.Sconit.Facility.Service;
using System.Collections.Generic;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityStockDetailMgr : IFacilityStockDetailBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.
        IList<FacilityStockDetail> LoadFacilityStockDetails(string stNo);
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityStockDetailMgrE : com.Sconit.Facility.Service.IFacilityStockDetailMgr
    {
    }
}

#endregion Extend Interface