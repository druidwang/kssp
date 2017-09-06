namespace com.Sconit.Web.Controllers.MRP
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.MRP.TRANS;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MRP;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Service.MRP;
    using System;
    using com.Sconit.Entity.MRP.VIEW;
    using System.Data.SqlClient;
    using System.Data;
    using System.Text;

    public class MrpPlanTraceController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        //public ISystemMgr systemMgr { get; set; }
        public IPlanMgr planMgr { get; set; }

        #region view
        [SconitAuthorize(Permissions = "Url_Mrp_PlanSimulation_Trace")]
        public ActionResult Trace()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_PlanSimulation_Trace")]
        public ActionResult TraceList(GridCommand command, MrpPlanTraceSearchModel searchModel)
        {
            if (searchModel.ResourceGroup == null)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_ResourceGroupCanNotBeEmpty);
                return View(new List<MrpPlanTraceView>());
            }
            else
            {
                SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
                var mrpPlanTraceViewList = planMgr.GetPlanTraceViewList
                    ((CodeMaster.ResourceGroup)searchModel.ResourceGroup, searchModel.ProductLine, searchModel.Item);
                foreach (var planSimulationView in mrpPlanTraceViewList)
                {
                    this.FillCodeDetailDescription(planSimulationView.MrpPlanTraceDetailViewList);
                }
                return View(mrpPlanTraceViewList);
            }
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlan_Simulation")]
        public ActionResult Simulation()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlan_Simulation")]
        public string _SimulationList(DateTime planVersion, string flow)
        {
            return planMgr.GetPlanSimulation(planVersion, flow).ToString();
        }
        #region Export Simulation
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlan_Simulation")]
        public ActionResult Export(DateTime planVersion, string flow)
        {
            var table = _SimulationList(planVersion, flow );
            return new DownloadFileActionResult(table, "Simulation.xls");
        }
        #endregion
        public string _GetResourceGroup(DateTime planVersion)
        {
            return ((int)this.genericMgr.FindById<MrpPlanMaster>(planVersion).ResourceGroup).ToString();
        }
        #endregion
        #region get simulation
        public string _GetSimulationList(DateTime planVersion, string flow)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@PlanVersion", planVersion);
            sqlParams[1] = new SqlParameter("@Flow", flow);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Busi_MRP_GetPlanSimulation_Fi", sqlParams);
            string str = (string)ds.Tables[0].Rows[0][0];
            return str.Replace("\\","");
        }
        #endregion
        #region Export simulation
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlan_Simulation")]
        public ActionResult ExportSimulation(DateTime planVersion, string flow)
        {
            var table = _GetSimulationList(planVersion, flow);
            return new DownloadFileActionResult(table, "FiSimulationList.xls");
        }
        #endregion
    }
}
