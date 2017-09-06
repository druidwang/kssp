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
    using com.Sconit.Service.Impl;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Utility.Report;
    using com.Sconit.PrintModel.ORD;
    using AutoMapper;
    using System.Text;
    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This controller response to control the Address.
    /// </summary>
    /// 


    public class DistributionReceiptController : WebAppBaseController
    {

        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the DistributionGoodsReceipt security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //public IReceiptMgr receiptMgr { get; set; }

        //public IReportGen reportGen { get; set; }
        #endregion


        /// <summary>
        /// hql 
        /// </summary>
        private static string selectCountStatement = "select count(*) from ReceiptMaster as r";

        /// <summary>
        /// hql 
        /// </summary>
        private static string selectStatement = "select r from ReceiptMaster as r";

        //public IGenericMgr genericMgr { get; set; }
        #region public actions
        /// <summary>
        /// Index action for DistributionGoodsReceipt controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_DistributionReceipt_View")]
        public ActionResult Index()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_DistributionReceipt_Detail")]
        public ActionResult DetailIndex()
        {
            return View();
        }



        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> ReceiptMaster Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionReceipt_View")]
        public ActionResult List(GridCommand command, ReceiptMasterSearchModel searchModel)
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
            ViewBag.OrderSubType = searchModel.OrderSubType;
            return View();
        }


        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> ReceiptMaster Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionReceipt_View")]
        public ActionResult _AjaxList(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<ReceiptMaster>()));
            }

            if (searchModel.OrderSubType == (int)com.Sconit.CodeMaster.OrderSubType.Return)
            {
                string centre = searchModel.PartyFrom;
                searchModel.PartyFrom = searchModel.PartyTo;
                searchModel.PartyTo = centre;
            }
            string whereStatement = null;
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageDataProcedure<ReceiptMaster>(procedureSearchStatementModel, command));
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Distribution_IpDetail")]
        public ActionResult DetailList(GridCommand command, ReceiptMasterSearchModel searchModel)
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
        public ActionResult _AjaxRecDetList(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<ReceiptDetail>()));
            }

            string whereStatement = null;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            GridModel<ReceiptDetail> List = GetAjaxPageDataProcedure<ReceiptDetail>(procedureSearchStatementModel, command);
            foreach(var ListData in List.Data)
            {
                ListData.MaterialsGroup = itemMgr.GetCacheItem(ListData.Item).MaterialsGroup;
            }
            return PartialView(List);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxRecLocDetList(string Id)
        {
            IList<ReceiptLocationDetail> RecLocDet =
                genericMgr.FindAll<ReceiptLocationDetail>("from ReceiptLocationDetail as o where o.RecDetId = ?", Id);
            return View(new GridModel(RecLocDet));
        }
        #region
        [GridAction(EnableCustomBinding = true)]
        public void ExportXLS(ReceiptMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            string whereStatement = null;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            var ReceiptDetailLists = GetAjaxPageDataProcedure<ReceiptDetail>(procedureSearchStatementModel, command);
            List<ReceiptDetail> ReceiptDetailList = ReceiptDetailLists.Data.ToList();
            foreach (var ListData in ReceiptDetailList)
            {
                ListData.MaterialsGroup = itemMgr.GetCacheItem(ListData.Item).MaterialsGroup;
            }
            ExportToXLS<ReceiptDetail>("DailsOfDistributionReceiptMaster.xls", ReceiptDetailList);
        }
        #endregion
        [SconitAuthorize(Permissions = "Url_DistributionReceipt_View")]
        public ActionResult Edit(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.ReceiptNo = receiptNo;
                ReceiptMaster rm = this.genericMgr.FindById<ReceiptMaster>(receiptNo);
                return View(rm);
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionReceipt_View")]
        public ActionResult _ReceiptDetailList(string receiptNo)
        {
            string hql = "select r from ReceiptDetail as r where r.ReceiptNo = ?";
            IList<ReceiptDetail> receiptDetailList = genericMgr.FindAll<ReceiptDetail>(hql, receiptNo);

            foreach (var receiptDetail in receiptDetailList)
            {
                receiptDetail.ReceiptLocationDetails = this.genericMgr.FindAll<ReceiptLocationDetail>
                    ("from ReceiptLocationDetail where ReceiptDetailId =? ", receiptDetail.Id);
            }

            return PartialView(receiptDetailList);
        }

        [SconitAuthorize(Permissions = "Url_DistributionReceipt_Cancel")]
        public ActionResult Cancel(string id)
        {
            try
            {
                ReceiptMaster ReceiptMaster = this.genericMgr.FindById<ReceiptMaster>(id);
                receiptMgr.CancelReceipt(ReceiptMaster);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ReceiveOrderCancelledSuccessfully);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { receiptNo = id });
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _ResultHierarchyAjax(string id)
        {
            string hql = "select r from ReceiptLocationDetail as r where r.ReceiptDetailId = ?";
            IList<ReceiptLocationDetail> ReceiptLocationDetail = genericMgr.FindAll<ReceiptLocationDetail>(hql, id);
            return PartialView(new GridModel(ReceiptLocationDetail));
        }

        #region 打印导出
        public void SaveToClient(string receiptNo)
        {
            ReceiptMaster receiptMaster = queryMgr.FindById<ReceiptMaster>(receiptNo);
            IList<ReceiptDetail> receiptDetail = queryMgr.FindAll<ReceiptDetail>("select rd from ReceiptDetail as rd where rd.ReceiptNo=?", receiptNo);
            foreach (var receiptDet in receiptDetail)
            {
                var receiptlocationCount = genericMgr.FindAll<ReceiptLocationDetail>
                    ("from ReceiptLocationDetail as o where o.ReceiptDetailId = ?", receiptDet.Id)
                    .Where(o => !string.IsNullOrWhiteSpace(o.HuId))
                    .Count();

                if (receiptlocationCount == 0)
                {
                    receiptDet.BoxQty = (int)Math.Ceiling(receiptDet.ReceivedQty / (receiptDet.UnitCount > 0 ? receiptDet.UnitCount : 1));
                }
                else
                {
                    receiptDet.BoxQty = receiptlocationCount;
                }
            }
            receiptMaster.ReceiptDetails = receiptDetail;
            PrintReceiptMaster printReceiptMaster = Mapper.Map<ReceiptMaster, PrintReceiptMaster>(receiptMaster);
            IList<object> data = new List<object>();
            data.Add(printReceiptMaster);
            data.Add(printReceiptMaster.ReceiptDetails);
            reportGen.WriteToClient(printReceiptMaster.ReceiptTemplate, data, printReceiptMaster.ReceiptNo + ".xls");

        }
        #region 导出发货汇总确认单
        public void SaveToClientMulti(ReceiptMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = 10000;
            string whereStatement = null;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            IList<ReceiptDetail> receiptDetailLists =  GetAjaxPageDataProcedure<ReceiptDetail>(procedureSearchStatementModel, command).Data.ToList();
            var receiptDetailSumLists = from p in receiptDetailLists
                                    group p by new
                                        {
                                            p.Item,
                                            p.ItemDescription,
                                            p.ReferenceItemCode,
                                            p.Uom,
                                            p.UnitCount,
                                        }
                                        into g
                                        select new ReceiptDetail
                                      {
                                          Item = g.Key.Item,
                                          ItemDescription = g.Key.ItemDescription,
                                          ReferenceItemCode = g.Key.ReferenceItemCode,
                                          Uom = g.Key.Uom,
                                          UnitCount = g.Key.UnitCount,
                                          ReceivedQty=g.Sum(p=>p.ReceivedQty)
                                      };
                                                                        
            IList<object> data = new List<object>();
            data.Add(receiptDetailSumLists.ToList());
            data.Add(CurrentUser.FullName);
            data.Add(searchModel.Flow);
            //2013/11/01 20:12:56 ~ 2013/11/12 12:33:34
            var timeSpan = (searchModel.StartDate == null ? "" : Convert.ToDateTime(searchModel.StartDate).ToString("yyyy/MM/dd HH:mm")) +
                           " ~  " + (searchModel.EndDate == null ? "" : Convert.ToDateTime(searchModel.EndDate).ToString("yyyy/MM/dd HH:mm"));
            data.Add(timeSpan);
            reportGen.WriteToClient("REC_TransferDetail.xls", data, string.Format("{0}{1}{2}","TransferDetailOfFlow", searchModel.Flow ,".xls"));

        }
        #endregion
        public string Print(string receiptNo)
        {
            ReceiptMaster receiptMaster = queryMgr.FindById<ReceiptMaster>(receiptNo);
            IList<ReceiptDetail> receiptDetail = queryMgr.FindAll<ReceiptDetail>("select rd from ReceiptDetail as rd where rd.ReceiptNo=?", receiptNo);
            foreach (var receiptDet in receiptDetail)
            {
                var receiptlocationCount = genericMgr.FindAll<ReceiptLocationDetail>
                    ("from ReceiptLocationDetail as o where o.ReceiptDetailId = ?", receiptDet.Id)
                    .Where(o => !string.IsNullOrWhiteSpace(o.HuId))
                    .Count();

                if (receiptlocationCount == 0)
                {
                    receiptDet.BoxQty = (int)Math.Ceiling(receiptDet.ReceivedQty / (receiptDet.UnitCount > 0 ? receiptDet.UnitCount : 1));
                }
                else
                {
                    receiptDet.BoxQty = receiptlocationCount;
                }
            }
            receiptMaster.ReceiptDetails = receiptDetail;
            PrintReceiptMaster printReceiptMaster = Mapper.Map<ReceiptMaster, PrintReceiptMaster>(receiptMaster);
            IList<object> data = new List<object>();
            data.Add(printReceiptMaster);
            data.Add(printReceiptMaster.ReceiptDetails);
            return reportGen.WriteToFile(printReceiptMaster.ReceiptTemplate, data);
        }
        #endregion

        #endregion


        #region private
        private ProcedureSearchStatementModel PrepareProcedureSearchStatement(GridCommand command, ReceiptMasterSearchModel searchModel, string whereStatement)
        {
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ReceiptNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            if (searchModel.GoodsReceiptOrderType.HasValue && searchModel.GoodsReceiptOrderType.Value > 0)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.GoodsReceiptOrderType, Type = NHibernate.NHibernateUtil.String });
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
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpDetailType, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.StartDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.EndDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Dock, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Item, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalReceiptNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ManufactureParty, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "ReceiptMasterStatusDescription")
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
            procedureSearchStatementModel.CountProcedure = "USP_Search_RecMstrCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_RecMstr";

            return procedureSearchStatementModel;
        }

        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, ReceiptMasterSearchModel searchModel, string whereStatement)
        {
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ReceiptNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            if (searchModel.GoodsReceiptOrderType.HasValue && searchModel.GoodsReceiptOrderType.Value > 0)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.GoodsReceiptOrderType, Type = NHibernate.NHibernateUtil.String });
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
            paraList.Add(new ProcedureParameter { Parameter = searchModel.IpDetailType, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.StartDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.EndDate, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Dock, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Item, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalReceiptNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ManufactureParty, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Int64 });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDescription")
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
            procedureSearchStatementModel.CountProcedure = "USP_Search_RecDetCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_RecDet";

            return procedureSearchStatementModel;
        }

        #endregion
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_DistributionReceipt_View")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstr(ReceiptMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            if (searchModel.OrderSubType == (int)com.Sconit.CodeMaster.OrderSubType.Return)
            {
                string centre = searchModel.PartyFrom;
                searchModel.PartyFrom = searchModel.PartyTo;
                searchModel.PartyTo = centre;
            }
            string whereStatement = null;
            ProcedureSearchStatementModel procedureSearchStatementModel = this.PrepareProcedureSearchStatement(command, searchModel, whereStatement);
            ExportToXLS<ReceiptMaster>("DistributionReceiptMaster.xls", GetAjaxPageDataProcedure<ReceiptMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
    }
}
