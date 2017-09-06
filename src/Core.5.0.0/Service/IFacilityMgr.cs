using System;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.ORD;
using System.Collections.Generic;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.FMS;

namespace com.Sconit.Service
{
    public interface IFacilityMgr
    {
        void CreateFacilityMaster(FacilityMaster facilityMaster);

        void GetFacilityControlPoint(string facilityName, string orderNo);

        void CreateFacilityOrder(string facilityName);

        void GenerateFacilityMaintainPlan();

        bool CheckProductLine(string productline);

        void GetFacilityParamater(string facilityName, string paramaterName, string name, string traceCode);

        void CreateCheckListOrder(CheckListOrderMaster checkListOrderMaster);

        void ReleaseCheckListOrder(CheckListOrderMaster checkListOrderMaster);

        void StartFacilityOrder(string facilityOrderNo);

        void FinishFacilityOrder(FacilityOrderMaster facilityOrderMaster);
    }
}
