/// <summary>
/// Summary description 
/// </summary>
namespace com.Sconit.Web.Controllers.ORD
{
    #region reference
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Utility.Report;
    using AutoMapper;
    using com.Sconit.PrintModel.ORD;
    using System;
    #endregion

    /// <summary>
    /// This controller response to control the ProcurementOrderIssue.
    /// </summary>
    public class ProcurementIpGapController : WebAppBaseController
    {
        #region Properties
        //public IGenericMgr genericMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        public IIpMgr ipMgr { get; set; }
        //public IReportGen reportGen { get; set; }
        #endregion

        private static string selectCountStatement = "select count(*) from IpMaster as i";

        private static string selectStatement = "select i from IpMaster as i";

        private static string selectIpDetailStatement = "select i from IpDetail as i where i.IpNo = ? and i.Type = ?";

        private static string selectAdjustIpDetailStatement = "select i from IpDetail as i where i.IsClose = ? and i.Type = ? and i.IpNo = ?";

        private static string selectAdjustHuIpDetailStatement = "select i from IpLocationDetail as i where i.IsClose = ?  and i.IpNo = ?";

        #region public actions

        #region view
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_View")]
        public ActionResult List(GridCommand command, IpMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_View")]
        public ActionResult _AjaxList(GridCommand command, IpMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            string whereStatement = string.Empty;
            if (searchModel.IpOrderType.HasValue && searchModel.IpOrderType.Value > 0)
            {
                whereStatement = "where i.OrderType =" + searchModel.IpOrderType.Value
                     + " and exists (select 1 from IpDetail as d where i.IpNo = d.IpNo and d.Type = "
                     + searchModel.IpDetailType + ")";
            }
            else
            {
                whereStatement = "where i.OrderType in (" + (int)com.Sconit.CodeMaster.OrderType.Procurement + ","
                    + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContract + ","
                    + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ")"
                    + " and exists (select 1 from IpDetail as d where i.IpNo = d.IpNo and d.Type = "
                    + searchModel.IpDetailType + ")";
            }
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageData<IpMaster>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_View,Url_ProcurementIpMaster_Cancel")]
        public ActionResult _IpDetailList(IpMaster ipMaster)
        {
            IList<IpDetail> ipDetailList = ipMaster.IpDetails;
            FillCodeDetailDescription<IpDetail>(ipDetailList);
            return PartialView(ipDetailList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_View")]
        public ActionResult Edit(string ipNo)
        {
            if (string.IsNullOrEmpty(ipNo))
            {
                return HttpNotFound();
            }
            IpMaster ipMaster = genericMgr.FindById<IpMaster>(ipNo);
            ipMaster.IpDetails = genericMgr.FindAll<IpDetail>("select i from IpDetail as i where i.IpNo =? ", ipNo);
            return View(ipMaster);
        }

        #endregion

        #region adjust
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public ActionResult Adjust()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public ActionResult AdjustList(GridCommand command, IpMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public ActionResult _AdjustAjaxList(GridCommand command, IpMasterSearchModel searchModel)
        {
            string replaceFrom = "_AdjustAjaxList";
            string replaceTo = "AdjustList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            string whereStatement = string.Empty;
            if (searchModel.IpOrderType.HasValue && searchModel.IpOrderType.Value > 0)
            {
                whereStatement = "where  i.OrderType =" + searchModel.IpOrderType.Value
                        + " and i.Status in (" + (int)com.Sconit.CodeMaster.IpStatus.Submit + ","
                        + (int)com.Sconit.CodeMaster.IpStatus.InProcess + ")"
                        + " and exists (select 1 from IpDetail as d where d.IsClose = 0  and d.IpNo = i.IpNo and d.Type = "
                        + searchModel.IpDetailType + ") ";
            }
            else
            {
                whereStatement = "where i.OrderType in ("
                        + (int)com.Sconit.CodeMaster.OrderType.Procurement + ","
                        + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + ","
                        + (int)com.Sconit.CodeMaster.OrderType.SubContract + ","
                        + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                        + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ","
                        + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ")"
                        + " and i.Status in (" + (int)com.Sconit.CodeMaster.IpStatus.Submit + ","
                        + (int)com.Sconit.CodeMaster.IpStatus.InProcess + ")"
                        + " and exists (select 1 from IpDetail as d where d.IsClose = 0  and d.IpNo = i.IpNo and d.Type = "
                        + searchModel.IpDetailType + ") ";
            }

            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageData<IpMaster>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public ActionResult _AdjustIpDetailList(string ipNo)
        {
            IList<IpDetail> ipDetailList = genericMgr.FindAll<IpDetail>(selectAdjustIpDetailStatement, new object[] { false, (int)com.Sconit.CodeMaster.IpDetailType.Gap, ipNo });
            return PartialView(ipDetailList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public ActionResult _AdjustHuIpDetailList(string ipNo)
        {
            IList<IpLocationDetail> ipLocationDetailList = genericMgr.FindAll<IpLocationDetail>(selectAdjustHuIpDetailStatement, new object[] { false, ipNo });
            if (ipLocationDetailList != null && ipLocationDetailList.Count > 0)
            {
                foreach (IpLocationDetail ipLocDetail in ipLocationDetailList)
                {
                    ipLocDetail.IpDetail = genericMgr.FindById<IpDetail>(ipLocDetail.IpDetailId);
                }
            }
            return PartialView(ipLocationDetailList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public ActionResult AdjustEdit(string ipNo)
        {
            if (string.IsNullOrEmpty(ipNo))
            {
                return HttpNotFound();
            }
            #region 条码还是数量
            IList<long> huCount = genericMgr.FindAll<long>("select count(*) from IpLocationDetail as i where i.IsClose = ?  and i.IpNo = ? and i.HuId is not null", new object[] { false, ipNo });
            ViewBag.IsContainHu = huCount[0] == 0 ? false : true;
            #endregion

            IpMaster ipMaster = genericMgr.FindById<IpMaster>(ipNo);
            return View(ipMaster);
        }

        #region adjust qty
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public JsonResult AdjustIpGapGI(string idStr, string qtyStr)
        {
            return AdjustIpGap(idStr, qtyStr, com.Sconit.CodeMaster.IpGapAdjustOption.GI);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public JsonResult AdjustIpGapGR(string idStr, string qtyStr)
        {
            return AdjustIpGap(idStr, qtyStr, com.Sconit.CodeMaster.IpGapAdjustOption.GR);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public JsonResult AdjustIpGap(string idStr, string qtyStr, com.Sconit.CodeMaster.IpGapAdjustOption gapAdjustOption)
        {
            try
            {
                IList<IpDetail> ipDetailList = new List<IpDetail>();
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] idArray = idStr.Split(',');
                    string[] qtyArray = qtyStr.Split(',');

                    for (int i = 0; i < idArray.Count(); i++)
                    {
                        if (Convert.ToDecimal(qtyArray[i]) > 0)
                        {
                            IpDetail ipDetail = genericMgr.FindById<IpDetail>(Convert.ToInt32(idArray[i]));
                            IpDetailInput input = new IpDetailInput();
                            input.ReceiveQty = Convert.ToDecimal(qtyArray[i]);
                            ipDetail.AddIpDetailInput(input);
                            ipDetailList.Add(ipDetail);
                        }
                    }
                }
                if (ipDetailList.Count() == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_AdjustDetailCanNotBeEmpty);
                }

                orderMgr.AdjustIpGap(ipDetailList, gapAdjustOption);
                object obj = new { SuccessMessage = string.Format(Resources.ORD.IpMaster.IpMaster_Adjusted, ipDetailList[0].IpNo), SuccessData = ipDetailList[0].IpNo };
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
        #endregion

        #region adjust hu
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public JsonResult AdjustHuIpGapGI(string idStr)
        {
            return AdjustHuIpGap(idStr, com.Sconit.CodeMaster.IpGapAdjustOption.GI);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public JsonResult AdjustHuIpGapGR(string idStr)
        {
            return AdjustHuIpGap(idStr, com.Sconit.CodeMaster.IpGapAdjustOption.GR);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_Adjust")]
        public JsonResult AdjustHuIpGap(string idStr, com.Sconit.CodeMaster.IpGapAdjustOption gapAdjustOption)
        {
            try
            {
                IList<IpDetail> ipDetailList = new List<IpDetail>();
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] idArray = idStr.Split(',');

                    for (int i = 0; i < idArray.Count(); i++)
                    {
                        IpLocationDetail ipLocationDetail = genericMgr.FindById<IpLocationDetail>(Convert.ToInt32(idArray[i]));
                        var existIpDetail = ipDetailList.Where(d => d.Id == ipLocationDetail.IpDetailId).ToList();
                        if (existIpDetail != null && existIpDetail.Count > 0)
                        {
                            IpDetail ipDetail = existIpDetail[0];
                            IpDetailInput input = new IpDetailInput();
                            input.ReceiveQty = ipLocationDetail.Qty / existIpDetail[0].UnitQty; //转为订单单位
                            input.HuId = ipLocationDetail.HuId;
                            input.LotNo = ipLocationDetail.LotNo;
                            existIpDetail[0].AddIpDetailInput(input);
                        }
                        else
                        {
                            IpDetail ipDetail = genericMgr.FindById<IpDetail>(ipLocationDetail.IpDetailId);
                            IpDetailInput input = new IpDetailInput();
                            input.ReceiveQty = ipLocationDetail.Qty / ipDetail.UnitQty; //转为订单单位
                            input.HuId = ipLocationDetail.HuId;
                            input.LotNo = ipLocationDetail.LotNo;
                            ipDetail.AddIpDetailInput(input);
                            ipDetailList.Add(ipDetail);
                        }
                    }
                }
                if (ipDetailList.Count() == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_AdjustDetailCanNotBeEmpty);
                }

                orderMgr.AdjustIpGap(ipDetailList, gapAdjustOption);
                object obj = new { SuccessMessage = string.Format(Resources.ORD.IpMaster.IpMaster_Adjusted, ipDetailList[0].IpNo), SuccessData = ipDetailList[0].IpNo };
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
        #endregion
        #endregion

        #endregion

        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, IpMasterSearchModel searchModel, string whereStatement)
        {

            IList<object> param = new List<object>();

            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "i", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, false);
            //SecurityHelper.AddPartyToPermissionStatement(ref whereStatement, "i", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "i", "OrderType", "i", "PartyFrom", "i", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, false);

            HqlStatementHelper.AddLikeStatement("IpNo", searchModel.IpNo, HqlStatementHelper.LikeMatchMode.Anywhere, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("CreateUserName", searchModel.CreateUserName, "i", ref whereStatement, param);
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "i", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "i", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "i", ref whereStatement, param);
            }

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "IpMasterStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by i.CreateDate desc";
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
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_ProcurementIpGap_View")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstr(IpMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            string whereStatement = string.Empty;
            if (searchModel.IpOrderType.HasValue && searchModel.IpOrderType.Value > 0)
            {
                whereStatement = "where i.OrderType =" + searchModel.IpOrderType.Value
                     + " and exists (select 1 from IpDetail as d where i.IpNo = d.IpNo and d.Type = "
                     + searchModel.IpDetailType + ")";
            }
            else
            {
                whereStatement = "where i.OrderType in (" + (int)com.Sconit.CodeMaster.OrderType.Procurement + ","
                    + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContract + ","
                    + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ")"
                    + " and exists (select 1 from IpDetail as d where i.IpNo = d.IpNo and d.Type = "
                    + searchModel.IpDetailType + ")";
            }
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel, whereStatement);
            ExportToXLS<IpMaster>("ProcurementIpGapMaster.xls", GetAjaxPageData<IpMaster>(searchStatementModel, command).Data.ToList());
        }
        #endregion
    }
}
