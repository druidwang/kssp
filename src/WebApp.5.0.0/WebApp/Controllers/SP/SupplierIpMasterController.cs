/// <summary>
/// Summary description 
/// </summary>
namespace com.Sconit.Web.Controllers.SP
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
    using System.Text;
    using System;
    using com.Sconit.PrintModel.ORD;
    using AutoMapper;
    using com.Sconit.Utility.Report;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This controller response to control the ProcurementOrderIssue.
    /// </summary>
    public class SupplierIpMasterController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the ProcurementOrderIssue security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //public IOrderMgr orderMgr { get; set; }

        public IIpMgr ipMgr { get; set; }

        //public IReportGen reportGen { get; set; }
        #endregion

        /// <summary>
        /// hql 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IpMaster as i";

        /// <summary>
        /// hql 
        /// </summary>
        /// 
        private static string selectStatement = "select i from IpMaster as i";

        private static string selectIpDetailCountStatement = "select count(*) from IpDetail as i";

        private static string selectIpDetailStatement = "select i from IpDetail as i";

        private static string selectReceiveIpDetailStatement = "select i from IpDetail as i where i.IsClose = ? and i.Type = ? and i.IpNo = ?";

        //public IGenericMgr genericMgr { get; set; }
        #region public actions
        /// <summary>
        /// Index action for ProcurementOrderIssue controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Query")]
        public ActionResult Index()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Detail_Query")]
        public ActionResult DetailIndex()
        {
            return View();
        }


        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult ReceiveIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Cancel")]
        public ActionResult CancelIndex()
        {
            return View();
        }




        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> IpMaster Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Query")]
        public ActionResult List(GridCommand command, IpMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult CancelList(GridCommand command, IpMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareCancelSearchStatement(command, (IpMasterSearchModel)searchCacheModel.SearchObject);
            return View(GetPageData<IpMaster>(searchStatementModel, command));
        }


        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> IpMaster Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Query")]
        public ActionResult _AjaxList(GridCommand command, IpMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<IpMaster>()));
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, string.Empty);
            return PartialView(GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command));
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Detail_Query")]
        public ActionResult DetailList(GridCommand command, IpMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxIpDetList(GridCommand command, IpMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<IpDetail>()));
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, string.Empty);
            return PartialView(GetAjaxPageDataProcedure<IpDetail>(procedureSearchStatementModel, command));
        }




        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Cancel")]
        public ActionResult _CancelAjaxList(GridCommand command, IpMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareCancelSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IpMaster>(searchStatementModel, command));
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult _ReceiveIpDetailList(string id)
        {
            IList<IpDetail> ipDetailList = genericMgr.FindAll<IpDetail>(selectReceiveIpDetailStatement, new object[] { false, (int)com.Sconit.CodeMaster.IpDetailType.Normal, id });
            return PartialView(ipDetailList);
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Query")]
        public ActionResult _IpDetailList(GridCommand command, IpDetailSearchModel searchModel, string ipNo)
        {
            searchModel.IpNo = ipNo;
            TempData["IpDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Query")]
        public ActionResult _AjaxIpDetailList(GridCommand command, IpDetailSearchModel searchModel, string ipNo)
        {
            SearchStatementModel searchStatementModel = this.IpDetailPrepareSearchStatement(command, searchModel, ipNo);
            GridModel<IpDetail> ipDetailData = GetAjaxPageData<IpDetail>(searchStatementModel, command);

            var list = ipDetailData.Data.Where(o => o.Type == Sconit.CodeMaster.IpDetailType.Normal);
            foreach (var IpDetail in list)
            {
                var IpDetailGapData = ipDetailData.Data.Where(o => o.GapIpDetailId == IpDetail.Id).FirstOrDefault() ?? new IpDetail();
                IpDetail.GapQty = IpDetailGapData.Qty;
            }
            ipDetailData.Data = list;
            return PartialView(ipDetailData);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult ReceiveEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            IpMaster ip = genericMgr.FindById<IpMaster>(id);
            return View(ip);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Cancel")]
        public ActionResult CancelEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            IpMaster ip = genericMgr.FindById<IpMaster>(id);
            return View(ip);
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult ReceiveIpMaster(int[] id, decimal[] currentReceiveQty)
        {
            IList<IpDetail> ipDetailList = new List<IpDetail>();
            for (int i = 0; i < currentReceiveQty.Count(); i++)
            {
                if (currentReceiveQty[i] > 0)
                {
                    IpDetail ipDet = genericMgr.FindById<IpDetail>(id[i]);
                    IpDetailInput input = new IpDetailInput();
                    input.ReceiveQty = currentReceiveQty[i];

                    ipDet.AddIpDetailInput(input);
                    //校验还没发
                    ipDetailList.Add(ipDet);
                }
            }

            if (ipDetailList.Count() == 0)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    orderMgr.ReceiveIp(ipDetailList);
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Received);
                    return RedirectToAction("ReceiveIndex");
                }
                catch (BusinessException ex)
                {
                    SaveBusinessExceptionMessage(ex);
                    return View();
                }
            }
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Cancel")]
        public ActionResult CancelIpMaster(string ipNo)
        {
            try
            {
                ipMgr.CancelIp(ipNo);
                SaveSuccessMessage(Resources.ORD.IpMaster.IpMaster_Cancel);
                return RedirectToAction("CancelIndex");
            }
            catch (BusinessException ex)
            {
                SaveBusinessExceptionMessage(ex);
                return View();
            }

        }
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Query")]
        public ActionResult _Edit(string IpNo)
        {
            if (string.IsNullOrEmpty(IpNo))
            {
                return HttpNotFound();
            }
            IpMaster ip = genericMgr.FindById<IpMaster>(IpNo);
            return View(ip);
        }

        #region cancel
        [SconitAuthorize(Permissions = "Url_SupplierIpMaster_Cancel")]
        public ActionResult Cancel(string id)
        {
            try
            {
                ipMgr.CancelIp(id);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ShipOrderCancelledSuccessfully);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return RedirectToAction("_Edit", new { IpNo = id });
        }
        #endregion

        #region 打印导出
        public void SaveToClient(string ipNo)
        {
            IpMaster ipMaster = queryMgr.FindById<IpMaster>(ipNo);
            IList<IpDetail> ipDetails = queryMgr.FindAll<IpDetail>("select id from IpDetail as id where id.IpNo=?", ipNo);
            var list = ipDetails.Where(o => o.Type == Sconit.CodeMaster.IpDetailType.Normal);
            foreach (var IpDetail in list)
            {
                var IpDetailGapData = ipDetails.Where(o => o.GapIpDetailId == IpDetail.Id).FirstOrDefault() ?? new IpDetail();
                IpDetail.OrderQty = genericMgr.FindById<OrderDetail>(IpDetail.OrderDetailId).OrderedQty;
                IpDetail.GapQty = IpDetailGapData.Qty;
                var iplocationCount = genericMgr.FindAll<IpLocationDetail>
                    ("from IpLocationDetail as o where o.IpDetailId = ?", IpDetail.Id)
                    .Where(o => !string.IsNullOrWhiteSpace(o.HuId))
                    .Count();
                if (iplocationCount == 0)
                {
                    if (!IpDetail.IsChangeUnitCount)
                    {
                        IpDetail.BoxQty = (int)Math.Ceiling(IpDetail.Qty / IpDetail.UnitCount);
                    }
                }
                else
                {
                    IpDetail.BoxQty = iplocationCount;
                }
            }
            ipDetails = list.ToList();
            ipMaster.IpDetails = ipDetails;
            //orderMaster.OrderDetails = orderDetails
            PrintIpMaster printIpMaster = Mapper.Map<IpMaster, PrintIpMaster>(ipMaster);
            IList<object> data = new List<object>();
            data.Add(printIpMaster);
            data.Add(printIpMaster.IpDetails);
            //string reportFileUrl = reportGen.WriteToFile(orderMaster.OrderTemplate, data);
            reportGen.WriteToClient(ipMaster.AsnTemplate, data, ipMaster.IpNo + ".xls");

            //return reportFileUrl;
            //reportGen.WriteToFile(orderMaster.OrderTemplate, data);
        }

        public string Print(string ipNo)
        {
            IpMaster ipMaster = queryMgr.FindById<IpMaster>(ipNo);
            IList<IpDetail> ipDetails = queryMgr.FindAll<IpDetail>("select id from IpDetail as id where id.IpNo=?", ipNo);
            var list = ipDetails.Where(o => o.Type == Sconit.CodeMaster.IpDetailType.Normal);
            foreach (var IpDetail in list)
            {
                var IpDetailGapData = ipDetails.Where(o => o.GapIpDetailId == IpDetail.Id).FirstOrDefault() ?? new IpDetail();
                IpDetail.OrderQty = genericMgr.FindById<OrderDetail>(IpDetail.OrderDetailId).OrderedQty;
                IpDetail.GapQty = IpDetailGapData.Qty;
                var iplocationCount = genericMgr.FindAll<IpLocationDetail>
                    ("from IpLocationDetail as o where o.IpDetailId = ?", IpDetail.Id)
                    .Where(o => !string.IsNullOrWhiteSpace(o.HuId))
                    .Count();
                if (iplocationCount == 0)
                {
                    if (!IpDetail.IsChangeUnitCount)
                    {
                        IpDetail.BoxQty = (int)Math.Ceiling(IpDetail.Qty / IpDetail.UnitCount);
                    }
                }
                else
                {
                    IpDetail.BoxQty = iplocationCount;
                }
            }
            ipDetails = list.ToList();
            ipMaster.IpDetails = ipDetails;
            //orderMaster.OrderDetails = orderDetails
            PrintIpMaster printIpMaster = Mapper.Map<IpMaster, PrintIpMaster>(ipMaster);
            IList<object> data = new List<object>();
            data.Add(printIpMaster);
            data.Add(printIpMaster.IpDetails);
            string reportFileUrl = reportGen.WriteToFile(ipMaster.AsnTemplate, data);
            //reportGen.WriteToClient(orderMaster.OrderTemplate, data, orderMaster.OrderTemplate);

            return reportFileUrl;
            //reportGen.WriteToFile(orderMaster.OrderTemplate, data);
        }

        #endregion

        #endregion

        private ProcedureSearchStatementModel PrepareProcedureSearchStatement(GridCommand command, IpMasterSearchModel searchModel, string whereStatement)
        {
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.Procurement + "," + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + ","
                            + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine,
                Type = NHibernate.NHibernateUtil.String
            });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderSubType, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.StartDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.EndDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Dock, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Item, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalIpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ManufactureParty, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.CreateUserName, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "IpMasterStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }

            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : null, Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? (command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc") : "asc", Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.PageSize, Type = NHibernate.NHibernateUtil.Int32 });
            pageParaList.Add(new ProcedureParameter { Parameter = command.Page, Type = NHibernate.NHibernateUtil.Int32 });

            var procedureSearchStatementModel = new ProcedureSearchStatementModel();
            procedureSearchStatementModel.Parameters = paraList;
            procedureSearchStatementModel.PageParameters = pageParaList;
            procedureSearchStatementModel.CountProcedure = "USP_Search_IpMstrCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_IpMstr";

            return procedureSearchStatementModel;
        }

        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, IpMasterSearchModel searchModel, string whereStatement)
        {
            whereStatement = "and exists(select 1 from IpMaster  as i where i.OrderSubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal + " and i.IpNo=d.IpNo)";
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)(int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine,
                Type = NHibernate.NHibernateUtil.String
            });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderSubType, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.StartDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.EndDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Dock, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Item, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalIpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ManufactureParty, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Int64 });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.CreateUserName, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
                }
                else if (command.SortDescriptors[0].Member == "OrderStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : null, Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? (command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc") : "asc", Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.PageSize, Type = NHibernate.NHibernateUtil.Int32 });
            pageParaList.Add(new ProcedureParameter { Parameter = command.Page, Type = NHibernate.NHibernateUtil.Int32 });

            var procedureSearchStatementModel = new ProcedureSearchStatementModel();
            procedureSearchStatementModel.Parameters = paraList;
            procedureSearchStatementModel.PageParameters = pageParaList;
            procedureSearchStatementModel.CountProcedure = "USP_Search_IpDetCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_IpDet";

            return procedureSearchStatementModel;
        }

        private SearchStatementModel PrepareCancelSearchStatement(GridCommand command, IpMasterSearchModel searchModel)
        {
            string whereStatement = "where  i.OrderType in ("
                                    + (int)com.Sconit.CodeMaster.OrderType.Procurement + "," + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ","
                                    + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + "," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ")"
                                    + " and i.Status = " + (int)com.Sconit.CodeMaster.IpStatus.Submit;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("IpNo", searchModel.IpNo, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("OrderType", searchModel.IpOrderType, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "i", ref whereStatement, param);

            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "i", "OrderType", "i", "PartyFrom", "i", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, true);
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

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);


            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel IpDetailPrepareSearchStatement(GridCommand command, IpDetailSearchModel searchModel, string ipNo)
        {
            string whereStatement = " where i.IpNo='" + ipNo + "'";

            IList<object> param = new List<object>();

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectIpDetailCountStatement;
            searchStatementModel.SelectStatement = selectIpDetailStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        #region  Export master search
        [SconitAuthorize(Permissions = "Url_Supplier_Deliveryorder_Query")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstr(IpMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, string.Empty);
            ExportToXLS<IpMaster>("SupplierIpMaster.xls", GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
    }
}
