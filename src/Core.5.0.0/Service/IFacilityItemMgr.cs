using System;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityItemMgr : IFacilityItemBaseMgr
    {
        #region Customized Methods
        IList<FacilityItem> GetFacilityItemList(string fcId, string itemCode, string allocateType);
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityItemMgrE : com.Sconit.Facility.Service.IFacilityItemMgr
    {
    }
}

#endregion Extend Interface