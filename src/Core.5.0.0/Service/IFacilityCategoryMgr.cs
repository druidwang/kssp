using System;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityCategoryMgr : IFacilityCategoryBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityCategoryMgrE : com.Sconit.Facility.Service.IFacilityCategoryMgr
    {
    }
}

#endregion Extend Interface