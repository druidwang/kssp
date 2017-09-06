/// <summary>
/// Summary description for IpMasterController
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
    using com.Sconit.PrintModel.ORD;
    using AutoMapper;
    using com.Sconit.Utility.Report;
    using System;
    using System.Text;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This controller response to control the IpMaster.
    /// </summary>
    public class DistributionIpMasterController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the IpMaster security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //public IOrderMgr orderMgr { get; set; }

        public IIpMgr ipMgr { get; set; }

        //public IReportGen reportGen { get; set; }
        #endregion

        #region private
        /// <summary>
        /// hql to get count of the IpMaster
        /// </summary>
        private static string selectCountStatement = "select count(*) from IpMaster as i";

        /// <summary>
        /// hql to get all of the IpMaster
        /// </summary>
        private static string selectStatement = "select i from IpMaster as i";

        /// <summary>
        /// hql to get count of the IpMaster by IpMaster's code
        /// </summary>
        //private static string duiplicateVerifyStatement = @"select count(*) from IpMaster as i where i.Code = ?";


        private static string selectIpDetailCountStatement = "select count(*) from IpDetail as i";

        private static string selectIpDetailStatement = "select i from IpDetail as i";

        private static string selectReceiveIpDetailStatement = "select i from IpDetail as i where i.IsClose = ? and i.Type = ? and i.IpNo = ?";

        private static string selectReceiveIpLocationDetailStatement = "select i from IpLocationDetail as i where i.IsClose = ? and i.IpNo = ?";
        #endregion

        #region public actions

        #region view
        /// <summary>
        /// Index action for IpMaster controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_View")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Distribution_IpDetail")]
        public ActionResult DetailIndex()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">IpMaster Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_View")]
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

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">IpMaster Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_View")]
        public ActionResult _AjaxList(GridCommand command, IpMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<IpMaster>()));
            }
            //string whereStatement = " where i.Type=0";
            //SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel, whereStatement);
            //return PartialView(GetAjaxPageData<IpMaster>(searchStatementModel, command));
            string whereStatement = " and i.Type=" + (int)com.Sconit.CodeMaster.IpDetailType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command));
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Distribution_IpDetail")]
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
        public ActionResult _AjaxIpDetLists(GridCommand command, IpMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<IpDetail>()));
            }
            string whereStatement = " and exists (select 1 from IpMaster  as i where  i.Type = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal + " and i.IpNo=d.IpNo) ";
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageDataProcedure<IpDetail>(procedureSearchStatementModel, command));
        }
        #region  Export detail search
        [GridAction(EnableCustomBinding = true)]
        public void Export(IpMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            string whereStatement = " and exists (select 1 from IpMaster  as i where  i.Type = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal + " and i.IpNo=d.IpNo) ";
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            var list = GetAjaxPageDataProcedure<IpDetail>(procedureSearchStatementModel, command);

            ExportToXLS<IpDetail>("DistributionIpDetail.xls", list.Data.ToList());
        }
        #endregion
        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">IpMaster id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_View")]
        public ActionResult Edit(string IpNo)
        {
            if (string.IsNullOrEmpty(IpNo))
            {
                return HttpNotFound();
            }
            else
            {
                IpMaster ipMaster = this.genericMgr.FindById<IpMaster>(IpNo);
                return View(ipMaster);
            }
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_View")]
        public ActionResult IpDetail(GridCommand command, IpDetailSearchModel searchModel, string ipNo)
        {
            searchModel.IpNo = ipNo;
            TempData["IpDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_View")]
        public ActionResult _AjaxIpDetList(GridCommand command, IpDetailSearchModel searchModel, string ipNo)
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
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_View")]
        public ActionResult _AjaxIpLocDetList(string Id)
        {
            IList<IpLocationDetail> IpLocDet =
                genericMgr.FindAll<IpLocationDetail>("from IpLocationDetail as o where o.IpDetailId = ?", Id);
            return View(new GridModel(IpLocDet));
        }

        [SconitAuthorize(Permissions = "Url_IpMaster_New")]
        public ActionResult New()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IpMaster_New")]
        public ActionResult _AjaxOrderMasterList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.OrderMasterPrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
        }
        #endregion

        #region receive
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        [HttpGet]
        public ActionResult ReceiveIndex()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        public ActionResult ReceiveList(GridCommand command, IpMasterSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        public ActionResult _ReceiveAjaxList(GridCommand command, IpMasterSearchModel searchModel)
        {

            string replaceFrom = "_ReceiveAjaxList";
            string replaceTo = "ReceiveList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            string whereStatement = " and i.Status in (" + (int)com.Sconit.CodeMaster.IpStatus.Submit + "," + (int)com.Sconit.CodeMaster.IpStatus.InProcess + ")"
                                 + " and exists (select 1 from IpDetail as d where d.IsClose = 0 and d.Type = 0 and d.IpNo = i.IpNo) ";

            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareReceiveProcedureSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        public ActionResult _ReceiveIpDetailList(string ipNo)
        {
            IList<IpDetail> ipDetailList = genericMgr.FindAll<IpDetail>(selectReceiveIpDetailStatement, new object[] { false, (int)com.Sconit.CodeMaster.IpDetailType.Normal, ipNo });
            return PartialView(ipDetailList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        public ActionResult _ReceiveHuIpDetailList(string ipNo)
        {
            IList<IpLocationDetail> ipDetailList = genericMgr.FindAll<IpLocationDetail>(selectReceiveIpLocationDetailStatement, new object[] { false, ipNo });
            return PartialView(ipDetailList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        public ActionResult ReceiveEdit(string ipNo)
        {
            if (string.IsNullOrEmpty(ipNo))
            {
                return HttpNotFound();
            }

            IpMaster ipMaster = genericMgr.FindById<IpMaster>(ipNo);
            ipMaster.IpMasterStatusDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.IpStatus, ((int)ipMaster.Status).ToString());
            ipMaster.IpMasterTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.IpDetailType, ((int)ipMaster.QualityType).ToString());
            return View(ipMaster);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        public JsonResult ReceiveIpMaster(string idStr, string qtyStr, string externalIpNo, string ipNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(externalIpNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ExternalOrderNumberCanNotBeEmpty);
                }
                if (!string.IsNullOrEmpty(ipNo))
                {
                    IpMaster ipMaster = genericMgr.FindById<IpMaster>(ipNo);
                    ipMaster.ExternalIpNo = externalIpNo;
                    genericMgr.Update(ipMaster);
                }

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
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReceiveDetailCanNotBeEmpty);
                }

                ReceiptMaster receiptMaster = orderMgr.ReceiveIp(ipDetailList);
                object obj = new { SuccessMessage = string.Format(Resources.ORD.IpMaster.IpMaster_Received, ipDetailList[0].IpNo, receiptMaster.ReceiptNo), SuccessData = ipDetailList[0].IpNo };
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
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        public JsonResult ReceiveHuIpMaster(string idStr, string externalIpNo, string ipNo)
        {
            IList<IpDetail> ipDetailList = new List<IpDetail>();
            try
            {
                if (!string.IsNullOrEmpty(ipNo))
                {
                    IpMaster ipMaster = genericMgr.FindById<IpMaster>(ipNo);
                    ipMaster.ExternalIpNo = externalIpNo;
                    genericMgr.Update(ipMaster);
                }
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] idArr = idStr.Split(',');
                    foreach (string id in idArr)
                    {
                        IpLocationDetail ipLocationDetail = genericMgr.FindById<IpLocationDetail>(int.Parse(id));
                        IpDetail ipDetail = genericMgr.FindById<IpDetail>(ipLocationDetail.IpDetailId);
                        IpDetailInput input = new IpDetailInput();
                        input.ReceiveQty = ipLocationDetail.Qty / ipDetail.UnitQty;  //转为订单单位
                        ipDetail.AddIpDetailInput(input);
                        ipDetailList.Add(ipDetail);
                    }

                }
                if (ipDetailList.Count() == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReceiveDetailCanNotBeEmpty);
                }

                ReceiptMaster receiptMaster = orderMgr.ReceiveIp(ipDetailList);
                object obj = new { SuccessMessage = string.Format(Resources.ORD.IpMaster.IpMaster_Received, ipDetailList[0].IpNo, receiptMaster.ReceiptNo), SuccessData = ipDetailList[0].IpNo };
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

        #region cancel
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Cancel")]
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
            return RedirectToAction("Edit", new { ipNo = id });
        }
        #endregion

        #region cancel
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Close")]
        public ActionResult Close(string id)
        {
            try
            {
                ipMgr.ManualCloseIp(id);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ShipOrderClosedSuccessfully);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return RedirectToAction("Edit", new { ipNo = id });
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
            PrintIpMaster printIpMaster = Mapper.Map<IpMaster, PrintIpMaster>(ipMaster);
            IList<object> data = new List<object>();
            data.Add(printIpMaster);
            data.Add(printIpMaster.IpDetails);
            reportGen.WriteToClient(ipMaster.AsnTemplate, data, ipMaster.IpNo+ ".xls");

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
            PrintIpMaster printIpMaster = Mapper.Map<IpMaster, PrintIpMaster>(ipMaster);
            IList<object> data = new List<object>();
            data.Add(printIpMaster);
            data.Add(printIpMaster.IpDetails);
            return reportGen.WriteToFile(ipMaster.AsnTemplate, data);
        }
        #endregion

        #endregion

        #region private action


        private ProcedureSearchStatementModel PrepareProcedureSearchStatement(GridCommand command, IpMasterSearchModel searchModel, string whereStatement)
        {
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            if (searchModel.IpOrderType.HasValue)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.IpOrderType, Type = NHibernate.NHibernateUtil.String });
            }
            else
            {
                paraList.Add(new ProcedureParameter
                {
                    Parameter = (int)com.Sconit.CodeMaster.OrderType.Distribution + ","
                    + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                    Type = NHibernate.NHibernateUtil.String
                });
            }
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
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.CreateUserName, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "IpMasterStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
                if (command.SortDescriptors[0].Member == "IpMasterTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
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

        private ProcedureSearchStatementModel PrepareReceiveProcedureSearchStatement(GridCommand command, IpMasterSearchModel searchModel, string whereStatement)
        {
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            if (searchModel.IpOrderType.HasValue)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.IpOrderType, Type = NHibernate.NHibernateUtil.String });
            }
            else
            {
                paraList.Add(new ProcedureParameter
                {
                    Parameter = (int)com.Sconit.CodeMaster.OrderType.Distribution + ","
                    + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                    Type = NHibernate.NHibernateUtil.String
                });
            }
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
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.CreateUserName, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "IpMasterStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
                if (command.SortDescriptors[0].Member == "IpMasterTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
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

        private SearchStatementModel OrderMasterPrepareSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.OrderNo != null && searchModel.OrderNo != string.Empty)
            {
                HqlStatementHelper.AddEqStatement("OrderNo", searchModel.OrderNo, "i", ref whereStatement, param);
            }
            else
            {
                if (searchModel.Flow != null && searchModel.Flow != string.Empty)
                {
                    HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "i", ref whereStatement, param);
                }
                else
                {
                    if (searchModel.PartyFrom != null && searchModel.PartyFrom != string.Empty)
                    {
                        HqlStatementHelper.AddEqStatement("Flow", searchModel.PartyFrom, "i", ref whereStatement, param);
                    }
                    if (searchModel.PartyTo != null && searchModel.PartyTo != string.Empty)
                    {
                        HqlStatementHelper.AddEqStatement("Flow", searchModel.PartyTo, "i", ref whereStatement, param);
                    }
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectIpDetailCountStatement;
            searchStatementModel.SelectStatement = selectIpDetailStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, IpMasterSearchModel searchModel, string whereStatement)
        {

            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            if (searchModel.IpOrderType.HasValue)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.IpOrderType, Type = NHibernate.NHibernateUtil.String });
            }
            else
            {
                paraList.Add(new ProcedureParameter
                {
                    Parameter = (int)com.Sconit.CodeMaster.OrderType.Distribution + ","
                    + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                    Type = NHibernate.NHibernateUtil.String
                });
            }
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
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Int64 });
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


        #endregion
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_View")]
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

            string whereStatement = " and i.Type=" + (int)com.Sconit.CodeMaster.IpDetailType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);
            ExportToXLS<IpMaster>("DistributionIpMaster.xls", GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_DistributionIpMaster_Receive")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstrRec(IpMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            string replaceFrom = "_ReceiveAjaxList";
            string replaceTo = "ReceiveList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            string whereStatement = " and i.Status in (" + (int)com.Sconit.CodeMaster.IpStatus.Submit + "," + (int)com.Sconit.CodeMaster.IpStatus.InProcess + ")"
                                 + " and exists (select 1 from IpDetail as d where d.IsClose = 0 and d.Type = 0 and d.IpNo = i.IpNo) ";
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareReceiveProcedureSearchStatement(command, searchModel, whereStatement);
            ExportToXLS<IpMaster>("DistributionIpGapMaster.xls", GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command).Data.ToList());

        }
        #endregion
    }
}
