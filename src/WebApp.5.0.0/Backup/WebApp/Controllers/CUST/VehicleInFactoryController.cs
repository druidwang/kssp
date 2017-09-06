using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.CUST;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.CUST;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Web.Controllers.CUST
{
    public class VehicleInFactoryController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from VehicleInFactoryMaster as v";

        private static string selectStatement = "from VehicleInFactoryMaster as v";

        //public IGenericMgr genericMgr { get; set; }

        public IVehicleInFactoryMgrImpl vehicleInFactoryMgr { get; set; }

        #region  public

        [SconitAuthorize(Permissions = "Url_VehicleInFactory_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_VehicleInFactory_View")]
        public ActionResult List(GridCommand command, VehicleInFactorySearchModel searchModel)
        {
            TempData["VehicleInFactorySearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_VehicleInFactory_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, VehicleInFactorySearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<VehicleInFactoryMaster>(searchStatementModel, command));
        }


        [SconitAuthorize(Permissions = "Url_VehicleInFactory_New")]
        public ActionResult New()
        {
            TempData["vehicleInFactoryDetails"] = new List<VehicleInFactoryDetail>();
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_VehicleInFactory_New")]
        public string CreateVehicleInFactory(VehicleInFactoryMaster vehicleInFactoryMaster)
        {
            try
            {
                vehicleInFactoryMaster.VehicleInFactoryDetails = (IList<VehicleInFactoryDetail>)TempData["vehicleInFactoryDetails"];
                if (vehicleInFactoryMaster.VehicleInFactoryDetails == null || vehicleInFactoryMaster.VehicleInFactoryDetails.Count == 0)
                {
                    throw new BusinessException(@Resources.CUST.VehicleInFactoryMaster.VehicleInFactory_IpNoIsEmpty);
                }
           
                vehicleInFactoryMgr.CreateVehicleInFactory(vehicleInFactoryMaster);
                SaveSuccessMessage(@Resources.CUST.VehicleInFactoryMaster.VehicleInFactory_Created);
                return vehicleInFactoryMaster.OrderNo;
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                TempData["vehicleInFactoryDetails"] = vehicleInFactoryMaster.VehicleInFactoryDetails;
                return string.Empty;
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_VehicleInFactory_New")]
        public void IpNoScan(string ipNo)
        {
            IList<VehicleInFactoryDetail> vehicleInFactoryDetailList = (IList<VehicleInFactoryDetail>)TempData["vehicleInFactoryDetails"];
            try
            {
                vehicleInFactoryMgr.AddVehicleInFactory(ipNo, vehicleInFactoryDetailList);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
            }
            TempData["vehicleInFactoryDetails"] = vehicleInFactoryDetailList;
        }

        public ActionResult _VehicleInFactoryDetailList()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_VehicleInFactory_New")]
        public ActionResult _SelectVehicleInFactoryDetail()
        {
            IList<VehicleInFactoryDetail> vehicleInFactoryDetailList = new List<VehicleInFactoryDetail>();
            if (TempData["vehicleInFactoryDetails"] != null)
            {
                vehicleInFactoryDetailList = (IList<VehicleInFactoryDetail>)TempData["vehicleInFactoryDetails"];
            }
            TempData["vehicleInFactoryDetails"] = vehicleInFactoryDetailList;
            return View(new GridModel(vehicleInFactoryDetailList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_VehicleInFactory_New")]
        public ActionResult _DeleteVehicleInFactoryDetail(string id)
        {
            IList<VehicleInFactoryDetail> vehicleInFactoryDetailList = (IList<VehicleInFactoryDetail>)TempData["vehicleInFactoryDetails"];
            var q = vehicleInFactoryDetailList.Where(v => v.IpNo != id).ToList();
            TempData["vehicleInFactoryDetails"] = q;
            return View(new GridModel(q));
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_VehicleInFactory_View")]
        public ActionResult Edit(string orderNo)
        {
            VehicleInFactoryMaster vehicleInFactoryMaster = genericMgr.FindById<VehicleInFactoryMaster>(orderNo);
            vehicleInFactoryMaster.VehicleInFactoryStatusDescription = systemMgr.GetCodeDetailDescription(CodeMaster.CodeMaster.VehicleInFactoryStatus, ((int)vehicleInFactoryMaster.Status).ToString());
            return View(vehicleInFactoryMaster);
        }


        [SconitAuthorize(Permissions = "Url_VehicleInFactory_View")]
        public ActionResult _ViewVehicleInFactoryDetailList(string orderNo)
        {
            IList<VehicleInFactoryDetail> vehicleInFactoryDetailList = genericMgr.FindAll<VehicleInFactoryDetail>("from VehicleInFactoryDetail where OrderNo = ? ", orderNo);
            return PartialView(vehicleInFactoryDetailList);
        }

        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, VehicleInFactorySearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "v", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("VehicleNo", searchModel.VehicleNo, HqlStatementHelper.LikeMatchMode.Start, "v", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Plant", searchModel.Plant, HqlStatementHelper.LikeMatchMode.Start, "v", ref whereStatement, param);

            if (searchModel.IsInFactory)
            {
                if (whereStatement == string.Empty)
                {
                    whereStatement = " where v.Status in (?,?)";
                }
                else
                {
                    whereStatement += " and v.Status in (?,?)";
                }
                param.Add((int)com.Sconit.CodeMaster.VehicleInFactoryStatus.Submit);
                param.Add((int)com.Sconit.CodeMaster.VehicleInFactoryStatus.InProcess);
            }
            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "v", ref whereStatement, param);
            }
            if (searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.DateTo, "v", ref whereStatement, param);
            }

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "VehicleInFactoryStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        #endregion

    }
}
