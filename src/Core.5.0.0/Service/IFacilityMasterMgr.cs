using System;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;
using System.IO;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityMasterMgr : IFacilityMasterBaseMgr
    {
        #region Customized Methods
        void UpdateFacilityMasterAndCreateFacilityTrans(FacilityMaster facilityMaster, FacilityTrans facilityTrans, string status, string userCode);
        //void GenerateMaintainInfo();
        void UpdateFacilityMasterMaintain(FacilityMaster facilityMaster);

        FacilityTrans LoadFacilityMaintain(string fcId);
        FacilityTrans LoadFacilityFix(string fcId);
        FacilityTrans LoadFacilityInspect(string fcId);
        FacilityTrans LoadFacilityEnvelop(string fcId);

        IList<FacilityMaster> GetFacilityChargeSite();
        
        void GenerateISITasks();

        void GenerateMouldISITasks();

        IList<FacilityMaster> GetFacilityMasterList(string fcId);

        IList<FacilityMaster> GetFacilityChargeOrganization();
        IList<FacilityMaster> GetFacilityChargePerson();

        IList<FacilityMaster> GetMaintainGroupList();

        IList<FacilityMaster> GetMaintainTypeList();

        IList<FacilityMaintainPlan> ReadFacilityMaintainPlanFromxls(Stream inputStream, User user);


        IList<AttachmentDetail> GetFacilityTransAttachment(string key);

        IList<AttachmentDetail> GetFacilityCategoryAttachment(string key);

        IList<AttachmentDetail> GetMaintainPlanAttachment(string key);

        IList<AttachmentDetail> GetFacilityMasterAttachment(string key);

        IList<AttachmentDetail> GetFacilityDistributionAttachment(string key);

        int GetFacilityTransAttachmentCount(string key);

        int GetFacilityCategoryAttachmentCount(string key);

        int GetMaintainPlanAttachmentCount(string key);

        int GetFacilityMasterAttachmentCount(string key);

        int GetFacilityDistributionAttachmentCount(string key);

        void BatchMaintainStart(IList<string> fcidList, User user);

        int BatchMaintainFinish(IList<string> fcTransIdList, string startDate, string endDate, string remark, User user);
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityMasterMgrE : com.Sconit.Facility.Service.IFacilityMasterMgr
    {
    }
}

#endregion Extend Interface