using System;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;
using System.IO;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityMaintainPlanMgr : IFacilityMaintainPlanBaseMgr
    {
        #region Customized Methods

        void UpdateFacilityMaintainPlan(FacilityMaintainPlan facilityMaintainPlan, bool isCalc);

        void CreateFacilityMaintainPlanList(IList<FacilityMaintainPlan> facilityMaintainPlanList);



        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityMaintainPlanMgrE : com.Sconit.Facility.Service.IFacilityMaintainPlanMgr
    {
    }
}

#endregion Extend Interface