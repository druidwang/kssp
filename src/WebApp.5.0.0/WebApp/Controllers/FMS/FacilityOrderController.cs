namespace com.Sconit.Web.Controllers.FMS
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.INP;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.INP;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.SYS;
    using System;
    using AutoMapper;
    using com.Sconit.Service.Impl;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.CUST;
    using System.Text;
    using com.Sconit.Entity;
    using com.Sconit.PrintModel.INP;
    using com.Sconit.Utility.Report;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Web.Models.SearchModels.FMS;
    using com.Sconit.Entity.FMS;


    public class FacilityOrderController : WebAppBaseController
    {


        public IFacilityMgr facilityMgr { get; set; }
        private static string selectStatement = "select f from FacilityOrderMaster as f";

        private static string selectCountStatement = "select count(*) from FacilityOrderMaster as f";

        private static string selectFacilityOrderDetailCountStatement = "select count(*) from FacilityOrderDetail as fd";

    
        private static string selectFacilityOrderDetailStatement = "select fd from FacilityOrderDetail as fd where fd.FacilityOrderNo=?";

    

        #region view
        [SconitAuthorize(Permissions = "Url_FacilityOrder_View")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_FacilityOrder_View")]
        [GridAction]
        public ActionResult List(GridCommand command, FacilityOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
          
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_FacilityOrder_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, FacilityOrderSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<FacilityOrderMaster>()));
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel, string.Empty);
            return PartialView(GetAjaxPageData<FacilityOrderMaster>(searchStatementModel, command));
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_FacilityOrder_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                FacilityOrderMaster facilityOrderMaster = this.genericMgr.FindById<FacilityOrderMaster>(id);

                return View(facilityOrderMaster);
            }
        }


        [SconitAuthorize(Permissions = "Url_FacilityOrder_View")]
        public ActionResult Start(string id)
        {
            try
            {
                facilityMgr.StartFacilityOrder(id);
                SaveSuccessMessage("保养单开始执行", id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { id = id });
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityOrder_View")]
        public JsonResult FinishFacilityOrder(string facilityOrderNo, string idStr, string qtyStr, string noteStr)
        {
            try
            {
                FacilityOrderMaster facilityOrderMaster = genericMgr.FindById<FacilityOrderMaster>(facilityOrderNo);
                IList<FacilityOrderDetail> facilityOrderDetailList = genericMgr.FindAll<FacilityOrderDetail>("from FacilityOrderDetail where FacilityOrderNo=?", facilityOrderNo);
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] idArray = idStr.Split(',');
                    string[] qtyArray = qtyStr.Split(',');
                    string[] noteArray = noteStr.Split(',');
                    for (int i = 0; i < idArray.Count(); i++)
                    {
                        if (Convert.ToInt32(idArray[i]) > 0)
                        {
                            FacilityOrderDetail facilityOrderDetail = facilityOrderDetailList.Where(p => p.Id == Convert.ToInt32(idArray[i])).FirstOrDefault();

                            facilityOrderDetail.ActualQty = Convert.ToDecimal(qtyArray[i]);
                            facilityOrderDetail.Note = noteArray[i];

                        }
                    }
                }
                facilityOrderMaster.FacilityOrderDetails = facilityOrderDetailList.ToList();
                facilityMgr.FinishFacilityOrder(facilityOrderMaster);

                object obj = new { SuccessMessage = "保养完成成功" };
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
    

        [SconitAuthorize(Permissions = "Url_FacilityOrder_View")]
        public ActionResult _FacilityOrderDetailList(string facilityOrderNo)
        {

            FacilityOrderMaster order = genericMgr.FindById<FacilityOrderMaster>(facilityOrderNo);
            ViewBag.IsEndable = order.Status == Sconit.CodeMaster.FacilityOrderStatus.InProcess;

            IList<FacilityOrderDetail> facilityOrderDetailList = genericMgr.FindAll<FacilityOrderDetail>(selectFacilityOrderDetailStatement, facilityOrderNo);

            return PartialView(facilityOrderDetailList);
        }

    

       
        #endregion

       

    

        #region private method
     
       
        private SearchStatementModel PrepareSearchStatement(GridCommand command, FacilityOrderSearchModel searchModel, string whereStatement)
        {
            IList<object> param = new List<object>();
        
            HqlStatementHelper.AddLikeStatement("FacilityOrderNo", searchModel.FacilityOrderNo, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
         
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "f", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "f", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "f", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                //if (command.SortDescriptors[0].Member == "InspectTypeDescription")
                //{
                //    command.SortDescriptors[0].Member = "Type";
                //}
                //else if (command.SortDescriptors[0].Member == "InspectStatusDescription")
                //{
                //    command.SortDescriptors[0].Member = "Status";
                //}
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by f.CreateDate desc";
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

      
    }
}
