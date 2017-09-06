using System;
using System.Collections.Generic;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IMaintainPlanMgr : IMaintainPlanBaseMgr
    {
        #region Customized Methods

        IList<MaintainPlan> GetMaintainPlanList(string facilityCategory);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IMaintainPlanMgrE : com.Sconit.Facility.Service.IMaintainPlanMgr
    {
    }
}

#endregion Extend Interface