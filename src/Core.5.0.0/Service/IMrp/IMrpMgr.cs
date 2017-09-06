using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.MRP.MD;

namespace com.Sconit.Service.MRP
{
    public interface IMrpMgr
    {
        void RunMrp(DateTime newPlanVersion, DateTime sourcePlanVersion, CodeMaster.ResourceGroup resourceGroup, string dateIndex, User user);
        
        void RunMrpPurchasePlan(User user);

        void GenMrpSnapShot(DateTime snapTime, User user, bool isRelease, CodeMaster.SnapType snapType);

        void CheckFlow();

        #region ExPlan
        void AdjustMrpExSectionPlanList(IList<MrpExSectionPlan> mrpExSectionPlanList);

        void AdjustMrpExItemPlanList(IList<MrpExItemPlan> mrpExItemPlanList);

        //void AdjustMrpExShiftPlan(IList<MrpExShiftPlan> mrpExShiftPlanList);

        //void AdjustMrpExShiftPlanWorkingCalendar(string dateIndex, string flow);

        void AdjustMrpExShiftPlanWorkingCalendar(DateTime planDate, string flow);

        void AdjustMrpExShiftPlanWorkingCalendar(List<MrpExShiftPlan> mrpExShiftPlanList);

        List<MrpExShiftPlan> GetMrpExShiftPlanList(DateTime planDate, string flow, string shift = null);

        string GetExPlanNo(MrpExSectionPlan exSectionPlan, object sequence);

        void ReleaseExPlan(string flow, DateTime planVersion, DateTime planDate);

        ProdLineExInstance LoadVirtualProdLineExInstance(double qty = 1.0);
        #endregion

        #region MiPlan
        double AdjustMrpMiPlan(IList<MrpMiPlan> mrpMiPlanList);

        double ReleaseMiPlan(IList<MrpMiPlan> mrpMiPlanList);

        double AdjustMrpMiShiftPlan(IList<MrpMiShiftPlan> mrpMiShiftPlanList);
        #endregion
    }
}

