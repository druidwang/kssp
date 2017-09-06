using System;
using System.Collections.Generic;
using System.IO;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.MRP.VIEW;
using System.Text;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.PRD;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP
{
    public interface IPlanMgr
    {
        #region Customized Methods

        #region Import
        /// <summary>
        /// 导入MRP计划
        /// </summary>
        void ReadDailyMrpPlanFromXls(Stream inputStream, DateTime? startDate, DateTime? endDate, string flowCode, bool isItemRef);

        void ReadWeeklyMrpPlanFromXls(Stream inputStream, string startWeek, string endWeek, string flowCode, bool isItemRef);

        /// <summary>
        /// 导入RCCP计划
        /// </summary>
        void ReadRccpPlanFromXls(Stream inputStream, string startDateIndex, string endDateIndex, bool isItemRef, com.Sconit.CodeMaster.TimeUnit periodType);
        void ReadRccpPlanFromXls(Stream inputStream, DateTime startDate, DateTime endDate, bool isItemRef, com.Sconit.CodeMaster.TimeUnit periodType);

        /// <summary>
        /// 导入班产计划
        /// </summary>
        List<MrpShiftPlan> ReadShiftPlanFromXls(Stream inputStream, DateTime startDate, DateTime endDate, string flow);

        void CreateMrpPlan(string flowCode, List<MrpPlanLog> mrpPlanLogList);
        void CreateRccpPlan(CodeMaster.TimeUnit dateType, List<RccpPlanLog> rccpPlanLogList);
        #endregion

        #region

        /// <summary>
        /// 后加工模具负荷
        /// </summary>
        IList<RccpFiView> GetMachineRccpView(IList<RccpFiPlan> rccpFiPlanList);

        /// <summary>
        /// 后加工岛区负荷
        /// </summary>
        IList<RccpFiView> GetIslandRccpView(IList<RccpFiPlan> rccpFiPlanList);

        void UpdateMrpPlan(MrpPlan mrpPlan);

        IEnumerable<RccpExGroupByProdLineView> GetExRccpViewGroupByProdLineLoad(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByProdLineView> GetExRccpViewGroupByProdLineQty(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByProdLineView> GetExRccpViewGroupByProdLineSpeed(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByProdLineView> GetExRccpViewGroupByProdLineScrapPercentage(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByItemView> GetExRccpViewGroupByItemTime(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByItemView> GetExRccpViewGroupByItemLoad(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByItemView> GetExRccpViewGroupByItemQty(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByClassifyView> GetExRccpViewGroupByClassifyLoad(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByClassifyView> GetExRccpViewGroupByClassifySpeed(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByClassifyView> GetExRccpViewGroupByClassifyScrapPercentage(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExGroupByClassifyView> GetExRccpViewGroupByClassifyQty(IEnumerable<RccpTransGroup> rccpTransList);

        IEnumerable<RccpExView> GetExRccpView(IEnumerable<RccpTransGroup> rccpTransList);
        #endregion

        StringBuilder GetPlanSimulation(DateTime planVersion, string flow);

        StringBuilder GetFiShiftPlanView(DateTime planVersion, string flow);

        StringBuilder GetMiDailyPlanView(DateTime planVersion, string flow);

        /// <summary>
        /// 计划跟踪
        /// </summary>
        IList<MrpPlanTraceView> GetPlanTraceViewList(CodeMaster.ResourceGroup resourceGroup, string flow, string item, bool onlyShowUrgent = true);

        IList<ContainerView> GetContainerViewList(DateTime dateTime);

        string GetMrpInvIn(DateTime planVersion, CodeMaster.ResourceGroup resourceGroup, string flow, bool isShowDetail);

        string GetStringRccpPlanView(IList<RccpPlan> rccpPlanList, int planVersion, string timeType);

        string GetStringMrpPlanView(IList<MrpPlan> mrpPlanList, DateTime startDate, int planVersion, string reqUrl);
        #endregion Customized Methods
    }
}
