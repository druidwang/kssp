using System;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityTransMgr : IFacilityTransBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityTransMgrE : com.Sconit.Facility.Service.IFacilityTransMgr
    {
    }
}

#endregion Extend Interface