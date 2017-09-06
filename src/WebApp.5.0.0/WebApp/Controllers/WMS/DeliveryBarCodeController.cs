using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.SCM;
using com.Sconit.Service;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.PrintModel.INV;
using AutoMapper;
using com.Sconit.Utility.Report;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Entity.SYS;
using NHibernate;
using com.Sconit.Web.Models.SearchModels.WMS;
using com.Sconit.Entity.WMS;

namespace com.Sconit.Web.Controllers.WMS
{
    public class DeliveryBarCodeController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from DeliveryBarCode as h";
        private static string selectStatement = "select h from DeliveryBarCode as h";
        public IDeliveryBarCodeMgr deliveryBarCodeMgr { get; set; }

        #region public method

        #region search
        [SconitAuthorize(Permissions = "Url_DeliveryBarCode_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_DeliveryBarCode_View")]
        public ActionResult List(GridCommand command, DeliveryBarCodeSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DeliveryBarCode_View")]
        public ActionResult _AjaxList(GridCommand command, DeliveryBarCodeSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<DeliveryBarCode>(searchStatementModel, command));
        }
        #endregion

        #region  new
        [SconitAuthorize(Permissions = "Url_DeliveryBarCode_New")]
        public ActionResult New()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_DeliveryBarCode_New")]
        public ActionResult _ShipPlanList(GridCommand command, string flow)
        {
            ViewBag.flow = flow;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_DeliveryBarCode_New")]
        public ActionResult _AjaxShipPlanList(string flow)
        {
            IList<ShipPlan> shipPlanList = new List<ShipPlan>();

            if (!string.IsNullOrEmpty(flow))
            {
                string shipPlanSql = "select s from ShipPlan as s where s.Flow = ? and s.IsActive = ?";
                shipPlanList = genericMgr.FindAll<ShipPlan>(shipPlanSql, new object[] { flow, true });

            }
            return View(new GridModel(shipPlanList));
        }

        [SconitAuthorize(Permissions = "Url_DeliveryBarCode_New")]
        public ActionResult CreateDeliveryBarCode(string idStr, string qtyStr)
        {
            try
            {
                IList<ShipPlan> shipPlanList = new List<ShipPlan>();
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] idArray = idStr.Split(',');
                    string[] qtyArray = qtyStr.Split(',');
                    for (int i = 0; i < idArray.Count(); i++)
                    {
                        ShipPlan sp = genericMgr.FindById<ShipPlan>(Convert.ToInt32(idArray[i]));
                        sp.ToDeliveryBarCodeQty = Convert.ToDecimal(qtyArray[i]);
                        shipPlanList.Add(sp);
                    }
                }

                IList<DeliveryBarCode> deliveryBarCodeList  = deliveryBarCodeMgr.CreateDeliveryBarCode(shipPlanList);


                IList<object> data = new List<object>();
                data.Add(deliveryBarCodeList);
                data.Add(CurrentUser.FullName);

                var barCodeTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultDeliveryBarCodeTemplate);

                string printUrl = reportGen.WriteToFile(barCodeTemplate, data);
                object obj = new { SuccessMessage = Resources.WMS.DeliveryBarCode.DeliveryBarcodePrintedSuccessfully, PrintUrl = printUrl };

                return Json(obj);

            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DeliveryBarCode_New")]
        public JsonResult PrintDeliveryBarCodeList(string checkedbarCodes)
        {
            string[] checkedBarCodeArray = checkedbarCodes.Split(',');
            string dbcSql = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedBarCodeArray)
            {
                if (dbcSql == string.Empty)
                {
                    dbcSql = "from DeliveryBarCode where BarCode in (?";
                }
                else
                {
                    dbcSql += ",?";
                }
                selectPartyPara.Add(para);
            }
            dbcSql += ")";

            IList<DeliveryBarCode> deliveryBarCodeList = genericMgr.FindAll<DeliveryBarCode>(dbcSql, selectPartyPara.ToArray());
            var barCodeTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultDeliveryBarCodeTemplate);


            IList<object> data = new List<object>();
            data.Add(deliveryBarCodeList);
            data.Add(CurrentUser.FullName);

            string reportFileUrl = reportGen.WriteToFile(barCodeTemplate, data);

            object obj = new { SuccessMessage = Resources.WMS.DeliveryBarCode.DeliveryBarcodePrintedSuccessfully, PrintUrl = reportFileUrl };
            return Json(obj);
        }
        #endregion

        #endregion

        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, DeliveryBarCodeSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("BarCode", searchModel.BarCode, HqlStatementHelper.LikeMatchMode.Anywhere, "h", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CreateUserName", searchModel.CreateUserName, HqlStatementHelper.LikeMatchMode.Start, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "h", ref whereStatement, param);

            if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "h", ref whereStatement, param);
            }
            if (searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLtStatement("CreateDate", searchModel.EndDate.Value.AddDays(1), "h", ref whereStatement, param);
            }




            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }



        #endregion

        #region private





        #endregion

    }
}
