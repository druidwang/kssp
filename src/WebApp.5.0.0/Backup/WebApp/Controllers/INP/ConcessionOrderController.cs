namespace com.Sconit.Web.Controllers.INP
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
    using com.Sconit.Entity.CUST;
    using com.Sconit.Entity.Exception;

    public class ConcessionOrderController : WebAppBaseController
    {
        //
        public IInspectMgr inspectMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }

        private static string selectStatement = "select c from ConcessionMaster as c";

        private static string selectCountStatement = "select count(*) from ConcessionMaster as c";

        private static string selectInspectResult = @"select r from InspectResult r where r.InspectNo = ? and r.RejectHandleResult=? 
            and r.IsHandle=? and r.JudgeQty > r.HandleQty";

        private static string selectRejectDetail = @"select r from RejectDetail as r where r.RejectNo=? and r.HandleQty > r.HandledQty
            and exists (select 1 from RejectMaster as m where r.RejectNo = m.RejectNo and m.HandleResult =? )";

        #region view
        [SconitAuthorize(Permissions = "Url_ConcessionOrder_View")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_View")]
        [GridAction]
        public ActionResult List(GridCommand command, ConcessionMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, ConcessionMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = ConcessionMasterPrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ConcessionMaster>(searchStatementModel, command));
        }

        #endregion

        #region new
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ConcessionOrder_New")]
        public ActionResult New()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_New")]
        public ActionResult _RejectDetailList(string rejectNo, string inspectNo)
        {
            IList<RejectDetail> rejectDetailList = new List<RejectDetail>();
            try
            {
                if (!string.IsNullOrWhiteSpace(inspectNo))
                {
                    InspectMaster inspectMaster = genericMgr.FindById<InspectMaster>(inspectNo);
                    ViewBag.InspectType = (int)inspectMaster.Type;
                    //JudgeQty	HandleQty	IsHandle
                    //100.00000000	0.00000000	0
                    rejectDetailList = genericMgr.FindAll<InspectResult>(selectInspectResult,
                        new object[] { inspectNo, Sconit.CodeMaster.HandleResult.Concession, false })
                        .Select(p => new RejectDetail
                        {
                            Id = p.Id,
                            InspectNo = p.InspectNo,
                            Item = p.Item,
                            ItemDescription = p.ItemDescription,
                            ReferenceItemCode = p.ReferenceItemCode,
                            Uom = p.Uom,
                            UnitCount = p.UnitCount,
                            HuId = p.HuId,
                            LotNo = p.LotNo,
                            CurrentLocation = p.CurrentLocation,
                            HandleQty = p.JudgeQty,
                            HandledQty = p.HandleQty,
                            CurrentHandleQty = p.CurrentHandleQty
                        }).ToList();
                }
                else if (!string.IsNullOrWhiteSpace(rejectNo))
                {
                    #region rejectmstr
                    RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(rejectNo);
                    ViewBag.InspectType = (int)rejectMaster.InspectType;
                    #endregion

                    rejectDetailList = genericMgr.FindAll<RejectDetail>
                        (selectRejectDetail, new object[] { rejectNo, Sconit.CodeMaster.HandleResult.Concession });
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            IList<FailCode> failCodeList = genericMgr.FindAll<FailCode>();
            foreach (var rejectDetail in rejectDetailList)
            {
                rejectDetail.CurrentHandleQty = rejectDetail.HandleQty - rejectDetail.HandledQty;
                foreach (FailCode failCode in failCodeList)
                {
                    if (rejectDetail.FailCode == failCode.Code)
                    {
                        rejectDetail.FailCode = failCode.CodeDescription;
                    }
                }
            }
            return PartialView(rejectDetailList);
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_View")]
        public ActionResult _ConcessionDetailList(string concessionNo)
        {
            IList<ConcessionDetail> concessionDetailList = new List<ConcessionDetail>();
            if (!string.IsNullOrEmpty(concessionNo))
            {
                string hql = "from ConcessionDetail where ConcessionNo = ?";
                concessionDetailList = this.genericMgr.FindAll<ConcessionDetail>(hql, concessionNo);
            }
            return PartialView(concessionDetailList);
        }

        #endregion

        #region action

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_New")]
        public string Create(string idStr, string qtyStr, bool isInspect, string location)
        {
            try
            {
                string[] idArr = idStr.Split(',');
                string[] qtyArr = qtyStr.Split(',');
                var concessionMaster = new ConcessionMaster();
                if (!isInspect)
                {
                    IList<RejectDetail> rejectDetailList = new List<RejectDetail>();
                    for (int i = 0; i < idArr.Count(); i++)
                    {
                        RejectDetail rejectDetail = genericMgr.FindById<RejectDetail>(Convert.ToInt32(idArr[i]));
                        rejectDetail.CurrentHandleQty = Convert.ToDecimal(qtyArr[i]);
                        rejectDetailList.Add(rejectDetail);
                    }
                    concessionMaster = inspectMgr.CreateConcessionMaster(rejectDetailList, location);
                }
                else
                {
                    var inspectResultList = genericMgr.FindAllIn<InspectResult>(
                       @"select r from InspectResult r where Id in(?", idArr.Select(p => (object)(int.Parse(p))))
                       .Select(p =>
                       {
                           p.CurrentHandleQty = Convert.ToDecimal(qtyArr[idArr.ToList().IndexOf(p.Id.ToString())]);
                           return p;
                       }).ToList();
                    concessionMaster = inspectMgr.CreateConcessionMaster(inspectResultList, location);
                }
                return concessionMaster.ConcessionNo;
            }
            catch (Exception ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.Message);
                return string.Empty;
            }
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_New")]
        public ActionResult Edit(string id)
        {
            ConcessionMaster ConcessionMaster = this.genericMgr.FindById<ConcessionMaster>(id);
            ConcessionMaster.ConcessionStatusDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.ConcessionStatus, ((int)ConcessionMaster.Status).ToString());
            return View(ConcessionMaster);
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_New")]
        public ActionResult Delete(string id)
        {
            try
            {
                inspectMgr.DeleteConcessionMaster(id);
                SaveSuccessMessage(Resources.INP.ConcessionMaster.ConcessionMaster_Deleted, id);
                return RedirectToAction("List");
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_Submit")]
        public ActionResult Submit(string id)
        {
            try
            {
                inspectMgr.ReleaseConcessionMaster(id);
                SaveSuccessMessage(Resources.INP.ConcessionMaster.ConcessionMaster_Submited, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit/" + id);
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_Commossion")]
        public ActionResult Commossion(string id)
        {
            try
            {
                ConcessionMaster concessionMaster = genericMgr.FindById<ConcessionMaster>(id);
                concessionMaster.ConcessionDetails = this.genericMgr.FindAll<ConcessionDetail>("from ConcessionDetail where ConcessionNo = ?", id);

                locationDetailMgr.ConcessionToUse(concessionMaster);
                SaveSuccessMessage(Resources.INP.ConcessionMaster.ConcessionMaster_ToUsed, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit/" + id);
        }

        [SconitAuthorize(Permissions = "Url_ConcessionOrder_Close")]
        public ActionResult Close(string id)
        {
            try
            {
                inspectMgr.CloseConcessionMaster(id);
                SaveSuccessMessage(Resources.INP.ConcessionMaster.ConcessionMaster_Closed, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit/" + id);
        }

        #endregion

        #region private method
        private SearchStatementModel ConcessionMasterPrepareSearchStatement(GridCommand command, ConcessionMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("ConcessionNo", searchModel.ConcessionNo, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "c", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("CreateUserName", searchModel.CreateUserName, "c", ref whereStatement, param);
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "c", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "c", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "c", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "ConcessionStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by c.CreateDate desc";
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