namespace com.Sconit.Service.SI
{
    public interface ISD_FlowMgr
    {
        Entity.SI.SD_SCM.FlowMaster GetFlowMaster(string flowCode, bool includeDetail);

        Entity.SI.SD_SCM.FlowMaster GetFlowMasterByFacility(string facilityCode, bool includeDetail);
    }
}
