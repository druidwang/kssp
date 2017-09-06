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
    using System.Text;
    using com.Sconit.Entity.SCM;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This controller response to control the ProcurementOrderIssue.
    /// </summary>
    public class ProcurementIpMasterController : WebAppBaseController
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


        private static string selectIpDetailCountStatement = "select count(*) from IpDetail as i";
        private static string selectIpDetailStatement = "select i from IpDetail as i";
        /// <summary>
        /// hql 
        /// </summary>
        private static string selectStatement = "select i from IpMaster as i";



        private static string selectReceiveIpDetailStatement = "select i from IpDetail as i where i.IsClose = ? and i.Type = ? and i.IpNo = ?";
        //public IGenericMgr genericMgr { get; set; }

        #region public actions

        #region view
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DetailUnitPriceIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Detail")]
        public ActionResult DetailIndex()
        {
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
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
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
        public ActionResult _AjaxList(GridCommand command, IpMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<IpMaster>()));
            }
            string whereStatement = " and 1=1 ";//" and i.Type=" + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command));
        }



        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
        public ActionResult DetailList(GridCommand command, IpMasterSearchModel searchModel, string ipNo)
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
        public ActionResult _AjaxIpDetailList(GridCommand command, IpMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<IpDetail>()));
            }
            string whereStatement = "and exists(select 1 from IpMaster  as i where i.IpNo=d.IpNo)";
            //string whereStatement = " where  i.Type = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal + "";
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
            string whereStatement = "and exists(select 1 from IpMaster  as i where i.IpNo=d.IpNo)";
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            var list = GetAjaxPageDataProcedure<IpDetail>(procedureSearchStatementModel, command);

            ExportToXLS<IpDetail>("ProcumentIpDetail.xls", list.Data.ToList());
        }
        #endregion
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
        public ActionResult _IpDetail(GridCommand command, IpDetailSearchModel searchModel, string ipNo)
        {
            searchModel.IpNo = ipNo;
            TempData["IpDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
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
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
        public ActionResult _AjaxIpLocDetList(string Id)
        {
            IList<IpLocationDetail> IpLocDet =
                genericMgr.FindAll<IpLocationDetail>("from IpLocationDetail as o where o.IpDetailId = ?", Id);
            return View(new GridModel(IpLocDet));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
        public ActionResult Edit(string ipNo)
        {
            if (string.IsNullOrEmpty(ipNo))
            {
                return HttpNotFound();
            }

            IpMaster ipMaster = genericMgr.FindById<IpMaster>(ipNo);
            return View(ipMaster);
        }

        #endregion

        #region receive
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult ReceiveIndex()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult ReceiveList(GridCommand command, IpMasterSearchModel searchModel)
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
        public ActionResult _ReceiveAjaxList(GridCommand command, IpMasterSearchModel searchModel)
        {
            string replaceFrom = "_ReceiveAjaxList";
            string replaceTo = "ReceiveList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<IpMaster>()));
            }

            string whereStatement = " and i.IsRecScanHu = 0"
                                  + " and i.Status in (" + (int)com.Sconit.CodeMaster.IpStatus.Submit + "," + (int)com.Sconit.CodeMaster.IpStatus.InProcess + ")"
                                  + " and exists (select 1 from IpDetail as d where d.IsClose = 0 and d.Type = 0 and d.IpNo = i.IpNo) ";

            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);

            return PartialView(GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult _ReceiveIpDetailList(string ipNo)
        {
            //IList<IpDetail> ipDetailList = genericMgr.FindAll<IpDetail>(selectReceiveIpDetailStatement, new object[] { false, (int)com.Sconit.CodeMaster.IpDetailType.Normal, ipNo });
            ViewBag.ipNo = ipNo;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult _AjaxReceiveIpDetailList(string ipNo)
        {
            IList<IpDetail> ipDetailList = genericMgr.FindAll<IpDetail>(selectReceiveIpDetailStatement, new object[] { false, (int)com.Sconit.CodeMaster.IpDetailType.Normal, ipNo });
            ViewBag.ipNo = ipNo;
            return PartialView(new GridModel(ipDetailList));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public ActionResult ReceiveEdit(string ipNo)
        {
            if (string.IsNullOrEmpty(ipNo))
            {
                return HttpNotFound();
            }

            IpMaster ipMaster = genericMgr.FindById<IpMaster>(ipNo);
            return View(ipMaster);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Receive")]
        public JsonResult ReceiveIpMaster(string idStr, string qtyStr)
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
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReceiveDetailCanNotBeEmpty);
                }

                ReceiptMaster receiptMaster = orderMgr.ReceiveIp(ipDetailList, false, DateTime.Now);
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
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Cancel")]
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

        #region Close
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_Close")]
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
                var ipDetailGapData = ipDetails.Where(o => o.GapIpDetailId == IpDetail.Id).FirstOrDefault() ?? new IpDetail();
                IpDetail.OrderQty = genericMgr.FindById<OrderDetail>(IpDetail.OrderDetailId).OrderedQty;
                IpDetail.GapQty = ipDetailGapData.Qty;
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
            reportGen.WriteToClient(ipMaster.AsnTemplate, data, ipMaster.IpNo + ".xls");

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
                var IpLocData = genericMgr.FindAll<IpLocationDetail>("from IpLocationDetail as o where o.IpDetailId = ?", IpDetail.Id).Where(o => o.HuId != null && o.HuId != "");
                if (IpLocData == null)
                {
                    if (!IpDetail.IsChangeUnitCount)
                    {
                        IpDetail.BoxQty = (int)Math.Ceiling(IpDetail.Qty / IpDetail.UnitCount);
                    }
                }
                else
                {
                    IpDetail.BoxQty = IpLocData.Count();
                }
            }
            ipDetails = list.ToList();
            ipMaster.IpDetails = ipDetails;
            //ipMaster.IpDetails = ipDetails.Where(i => string.IsNullOrEmpty(i.GapReceiptNo)).ToList();
            PrintIpMaster printIpMaster = Mapper.Map<IpMaster, PrintIpMaster>(ipMaster);
            IList<object> data = new List<object>();
            data.Add(printIpMaster);
            data.Add(printIpMaster.IpDetails);
            return reportGen.WriteToFile(ipMaster.AsnTemplate, data);
        }
        #endregion
        #endregion

        #region private method

        private ProcedureSearchStatementModel PrepareProcedureSearchStatement(GridCommand command, IpMasterSearchModel searchModel, string whereStatement)
        {
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            if (searchModel.IpOrderType.HasValue && searchModel.IpOrderType > 0)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.IpOrderType, Type = NHibernate.NHibernateUtil.String });
            }
            else
            {
                paraList.Add(new ProcedureParameter
                {
                    Parameter = (int)com.Sconit.CodeMaster.OrderType.Procurement + ","
                                + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + ","
                                + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ","
                                + (int)com.Sconit.CodeMaster.OrderType.SubContract + ","
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

        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, IpMasterSearchModel searchModel, string whereStatement)
        {

            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            if (searchModel.IpOrderType.HasValue && searchModel.IpOrderType > 0)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.IpOrderType, Type = NHibernate.NHibernateUtil.String });
            }
            else
            {
                paraList.Add(new ProcedureParameter
                {
                    Parameter = (int)com.Sconit.CodeMaster.OrderType.Procurement + ","
                                + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + ","
                                + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ","
                                + (int)com.Sconit.CodeMaster.OrderType.SubContract + ","
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


        [GridAction]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_UpdateDetailUnitPrice")]
        public ActionResult DetailUnitPriceList(GridCommand command, IpDetailSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (string.IsNullOrEmpty(searchModel.IpNo))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_ShipOrderCanNotBeEmpty);
                return View();
            }
            IList<IpMaster> ipMasterList = genericMgr.FindAll<IpMaster>(" from IpMaster i where i.IpNo=?", searchModel.IpNo);
            if (ipMasterList.Count < 1)
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_ShipOrderNotExits);
                return View();
            }
            else
            {
                if (ipMasterList[0].Status == com.Sconit.CodeMaster.IpStatus.Cancel || ipMasterList[0].Status == com.Sconit.CodeMaster.IpStatus.Close)
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_TheShipOrderAlreadyCanNotModificate);
                    return View();
                }
                if (ipMasterList[0].OrderType != Sconit.CodeMaster.OrderType.Procurement
                    && ipMasterList[0].OrderType != Sconit.CodeMaster.OrderType.SubContract)
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_IsNotPurchaseConsignmentShipOrderCanNotModifyPrice);
                    return View();
                }
                if (!Utility.SecurityHelper.HasPermission(ipMasterList[0]))
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_LackModificateTheShipOrderPermission);
                    return View();
                }
            }
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }

            ViewBag.Item = searchModel.Item;
            ViewBag.IpNo = searchModel.IpNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_UpdateDetailUnitPrice")]
        public ActionResult _DetailUnitPriceAjaxList(GridCommand command, IpDetailSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.IpNo))
            {
                return PartialView(new GridModel(new List<IpDetail>()));
            }
            IList<IpMaster> ipMasterList = genericMgr.FindAll<IpMaster>(" from IpMaster i where i.IpNo=?", searchModel.IpNo);
            if (ipMasterList.Count < 1)
            {
                return PartialView(new GridModel(new List<IpDetail>()));
            }
            else
            {
                if (ipMasterList[0].Status == com.Sconit.CodeMaster.IpStatus.Cancel || ipMasterList[0].Status == com.Sconit.CodeMaster.IpStatus.Close)
                {
                    return PartialView(new GridModel(new List<IpDetail>()));
                }
                if (ipMasterList[0].OrderType != Sconit.CodeMaster.OrderType.Procurement
                    && ipMasterList[0].OrderType != Sconit.CodeMaster.OrderType.SubContract)
                {
                    return PartialView(new GridModel(new List<IpDetail>()));
                }
                if (!Utility.SecurityHelper.HasPermission(ipMasterList[0]))
                {
                    return PartialView(new GridModel(new List<IpDetail>()));
                }
            }

            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<IpDetail>()));
            }

            string whereStatement = " where  i.OrderType in ("
                            + (int)com.Sconit.CodeMaster.OrderType.Procurement + ","
                            + (int)com.Sconit.CodeMaster.OrderType.SubContract + ")";
            SearchStatementModel searchStatementModel = this.PrepareSearchDetailUnitPriceStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageData<IpDetail>(searchStatementModel, command));
        }
        private SearchStatementModel PrepareSearchDetailUnitPriceStatement(GridCommand command, IpDetailSearchModel searchModel, string whereStatement)
        {
            IList<object> param = new List<object>();
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "i", "OrderType", "i", "PartyFrom", "i", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, false);
            HqlStatementHelper.AddLikeStatement("IpNo", searchModel.IpNo, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "i", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by i.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectIpDetailCountStatement;
            searchStatementModel.SelectStatement = selectIpDetailStatement;
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


        [GridAction]
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_UpdateDetailUnitPrice")]
        public ActionResult _DetailUnitPriceUpdate(IpDetail updateIpDetail, string Item, string IpNo)
        {

            IpDetail ipDetail = genericMgr.FindById<IpDetail>(updateIpDetail.Id);
            try
            {
                ipDetail.UnitPrice = updateIpDetail.UnitPrice;
                ipDetail.IsProvisionalEstimate = updateIpDetail.IsProvisionalEstimate;
                genericMgr.Update(ipDetail);


                IList<object> param = new List<object>();
                string hql = " from IpDetail i where 1=1";
                if (!string.IsNullOrEmpty(IpNo))
                {
                    hql += " and i.IpNo=?";
                    param.Add(IpNo);
                }
                if (!string.IsNullOrEmpty(Item))
                {
                    hql += " and i.Item=?";
                    param.Add(Item);
                }

                IList<IpDetail> IpDetailList = genericMgr.FindAll<IpDetail>(hql, param.ToArray());
                return PartialView(new GridModel(IpDetailList));
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_ProcurementIpMaster_View")]
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
            string whereStatement = " and 1=1 ";//" and i.Type=" + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);
            ExportToXLS<IpMaster>("ProcurementIpMaster.xls", GetAjaxPageDataProcedure<IpMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
    }
}
