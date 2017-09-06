using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.CUST;
using com.Sconit.Service;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Web.Controllers.CUST
{
    public class DeferredFeedCounterController : WebAppBaseController
    {
        private static string selectCountStatement = "select count(*) from OrderMaster as o";

        private static string selectStatement = "from OrderMaster as o";

        //public IOrderMgr orderMgr { get; set; }

        //public IGenericMgr genericMgr { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_FailCode_View")]
        [GridAction]
        public ActionResult List(GridCommand command, string Flow)
        {
            ViewBag.Flow = Flow;
            this.CheckFlow(Flow, true);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        private bool CheckFlow(string Flow, bool IsPartyList)
        {
            if (!string.IsNullOrEmpty(Flow))
            {
                IList<ProductLineMap> productLineMapList = genericMgr.FindAll<ProductLineMap>("from ProductLineMap as p where (p.ProductLine = ? or p.CabFlow = ? or p.ChassisFlow = ?) and (p.ProductLine is not null and p.CabFlow is not null and p.ChassisFlow is not null)", new object[] { Flow, Flow, Flow });
                if (productLineMapList.Count == 0)
                {
                    if (IsPartyList)
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_ProductionLineIsWrong);
                    }
                    return false;
                }
                else
                {
                    ProductLineMap productLineMap = productLineMapList[0]; ;
                    if (productLineMap.CabFlow == null || productLineMap.ChassisFlow == null)
                    {
                        if (IsPartyList)
                        {
                            SaveWarningMessage(Resources.EXT.ControllerLan.Con_ProductionLineIsWrong);
                        }
                        return false;
                    }
                    else
                    {
                        TempData["_AjaxMessage"] = "";
                        ViewBag.StartEmptyVanOrder = true;
                        return true;
                    }
                }
            }
            else
            {
                if (IsPartyList)
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseInputProductionLineToSearch);
                }
                return false;
            }
        }

        [SconitAuthorize(Permissions = "Url_FailCode_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, string Flow)
        {
            if (!this.CheckFlow(Flow, false))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, Flow);
            return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
        }

        public string StartEmptyVanOrder(string Flow)
        {
            try
            {
                orderMgr.StartEmptyVanOrder(Flow);
                return Resources.EXT.ControllerLan.Con_ProductionLine + Flow + Resources.EXT.ControllerLan.Con_EmptyCarGoLiveSuccessfully;
            }
            catch (BusinessException ex)
            {
                return ex.GetMessages()[0].GetMessageString();
            }
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, string Flow)
        {
            string whereStatement = "where o.Status = ?";
            IList<object> param = new List<object>();
            param.Add(CodeMaster.OrderStatus.InProcess);

            HqlStatementHelper.AddEqStatement("Flow", Flow, "o", ref whereStatement, param);


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

    }
}
