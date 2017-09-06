namespace com.Sconit.Service.SI.Impl
{
    using System.Collections.Generic;
    using com.Sconit.Entity.SCM;
    using NHibernate.Criterion;
    using com.Sconit.Entity.Exception;
    using Castle.Services.Transaction;
    using System;
    using AutoMapper;
    using System.Linq;

    public class SD_FlowMgrImpl : BaseMgr, com.Sconit.Service.SI.ISD_FlowMgr
    {
        public Entity.SI.SD_SCM.FlowMaster GetFlowMaster(string flowCode,bool includeDetail)
        {
            var flowMaster = this.genericMgr.FindById<Entity.SCM.FlowMaster>(flowCode);
            var flow = Mapper.Map<Entity.SCM.FlowMaster, Entity.SI.SD_SCM.FlowMaster>(flowMaster);
            if (includeDetail)
            {
                var flowDetails = this.flowMgr.GetFlowDetailList(flowCode, false, true);
                flow.FlowDetails = Mapper.Map<IList<Entity.SCM.FlowDetail>, List<Entity.SI.SD_SCM.FlowDetail>>(flowDetails);

                foreach (var flowDetail in flow.FlowDetails)
                {
                    flowDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flow.LocationFrom : flowDetail.LocationFrom;
                    flowDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flow.LocationTo : flowDetail.LocationTo;
                }
            }
            return flow;
        }

        public Entity.SI.SD_SCM.FlowMaster GetFlowMasterByFacility(string facilityCode, bool includeDetail)
        {
            var flowMaster = this.genericMgr.FindAll<Entity.SCM.FlowMaster>("from FlowMaster as fm where fm.Code in(select plf.ProductLine from ProductLineFacility as plf where plf.Code=?)", facilityCode);
            var flow = Mapper.Map<Entity.SCM.FlowMaster, Entity.SI.SD_SCM.FlowMaster>(flowMaster[0]);
            if (includeDetail)
            {
                var flowDetails = this.flowMgr.GetFlowDetailList(flowMaster[0].Code, false, true);
                flow.FlowDetails = Mapper.Map<IList<Entity.SCM.FlowDetail>, List<Entity.SI.SD_SCM.FlowDetail>>(flowDetails);

                foreach (var flowDetail in flow.FlowDetails)
                {
                    flowDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flow.LocationFrom : flowDetail.LocationFrom;
                    flowDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flow.LocationTo : flowDetail.LocationTo;
                }
            }
            return flow;
        }

    }
}
