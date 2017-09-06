using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models.SearchModels.ORD;
using com.Sconit.Web.Models;
using com.Sconit.Entity.ORD;
using com.Sconit.Service;
using com.Sconit.Utility.Report;
using com.Sconit.PrintModel.ORD;
using AutoMapper;
using System.Text;
using com.Sconit.Entity.SYS;


namespace com.Sconit.Web.Controllers.ORD
{
    public class ReturnReceiptController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the ProcurementGoodsReceipt security 
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

        #region public actions
        public ActionResult Index()
        {
            return View();
        }

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
        [SconitAuthorize(Permissions = "Url_ProcurementReceipt_View")]
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
            return View();
        }


        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> ReceiptMaster Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementReceipt_View")]
        public ActionResult _AjaxList(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<ReceiptMaster>()));
            }
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ReceiptMaster>(searchStatementModel, command));
        }


        [SconitAuthorize(Permissions = "Url_ProcurementReceipt_View")]
        public ActionResult Edit(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.ReceiptNo))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.ReceiptNo = searchModel.ReceiptNo;
                ReceiptMaster rm = this.genericMgr.FindById<ReceiptMaster>(searchModel.ReceiptNo);
                return View(rm);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ReturnReceipt_View")]
        public ActionResult _ReceiptDetail(string receiptNo)
        {
            ViewBag.receiptNo = receiptNo;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ReturnReceipt_View")]
        public ActionResult _ReceiptDetailHierarchyAjax(string receiptNo)
        {
            string hql = "select r from ReceiptDetail as r where r.ReceiptNo = ?";
            IList<ReceiptDetail> receiptDetail = genericMgr.FindAll<ReceiptDetail>(hql, receiptNo);
            return View(new GridModel(receiptDetail));
        }

        [SconitAuthorize(Permissions = "Url_ReturnReceipt_Cancel")]
        public void Cancel(string id)
        {
            ReceiptMaster ReceiptMaster = this.genericMgr.FindById<ReceiptMaster>(id);
            receiptMgr.CancelReceipt(ReceiptMaster);
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
            IList<ReceiptDetail> receiptDetail = queryMgr.FindAll<ReceiptDetail>("select rd from ReceiptDetail as rd where rd.Receipt=?", receiptNo);
            receiptMaster.ReceiptDetails = receiptDetail;
            PrintReceiptMaster printReceiptMaster = Mapper.Map<ReceiptMaster, PrintReceiptMaster>(receiptMaster);
            IList<object> data = new List<object>();
            data.Add(printReceiptMaster);
            data.Add(printReceiptMaster.ReceiptDetails);
            reportGen.WriteToClient(printReceiptMaster.ReceiptTemplate, data, printReceiptMaster.ReceiptNo+ ".xls");

        }

        public string Print(string receiptNo)
        {
            IpMaster ipMaster = queryMgr.FindById<IpMaster>(receiptNo);
            IList<IpDetail> ipDetails = queryMgr.FindAll<IpDetail>("select id from IpDetail as id where id.IpNo=?", receiptNo);
            ipMaster.IpDetails = ipDetails;
            PrintIpMaster printIpMaster = Mapper.Map<IpMaster, PrintIpMaster>(ipMaster);
            IList<object> data = new List<object>();
            data.Add(printIpMaster);
            data.Add(printIpMaster.IpDetails);
            return reportGen.WriteToFile(ipMaster.AsnTemplate, data);
        }
        #endregion

        #endregion

        #region
        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ReceiptMaster Search Model</param>
        /// <returns>return ReceiptMaster search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            string whereStatement = " where r.OrderSubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Return;

            IList<object> param = new List<object>();

            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "r", "PartyTo", com.Sconit.CodeMaster.OrderType.Distribution, false);
            //SecurityHelper.AddPartyToPermissionStatement(ref whereStatement, "r", "PartyFrom", com.Sconit.CodeMaster.OrderType.Distribution);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "r", "OrderType", "r", "PartyFrom", "r", "PartyTo", com.Sconit.CodeMaster.OrderType.Distribution, false);
            HqlStatementHelper.AddLikeStatement("WMSNo", searchModel.ExternalReceiptNo, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ReceiptNo", searchModel.ReceiptNo, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("IpNo", searchModel.IpNo, HqlStatementHelper.LikeMatchMode.Start, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("OrderType", searchModel.GoodsReceiptOrderType, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "r", ref whereStatement, param);

            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "r", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "r", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "r", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "ReceiptMasterStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "ReceiptMasterStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by r.CreateDate desc";
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
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ReturnReceipt_Detail_View")]
        public ActionResult DetailList(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            TempData["ReceiptMasterSearchModel"] = searchModel;
            if (this.CheckSearchModelIsNull(searchModel))
            {
                TempData["_AjaxMessage"] = "";

                IList<ReceiptDetail> list = genericMgr.FindAll<ReceiptDetail>(PrepareSearchDetailStatement(command, searchModel)); //GetPageData<OrderDetail>(searchStatementModel, command);

                int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
                if (list.Count > value)
                {
                    SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_DataExceedRow, value));
                }
                return View(list.Take(value));
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
                return View(new List<ReceiptDetail>());
            }
        }


     
        private string PrepareSearchDetailStatement(GridCommand command, ReceiptMasterSearchModel searchModel)
        {
            StringBuilder Sb = new StringBuilder();
            string whereStatement = " select  d from ReceiptDetail as d  where exists (select 1 from ReceiptMaster  as o where o.OrderSubType in (" + (int)com.Sconit.CodeMaster.OrderSubType.Return + ")"
                                     + "and  o.OrderType in (" + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + ")"
                                    + " and o.ReceiptNo=d.ReceiptNo ";

            Sb.Append(whereStatement);
            if (searchModel.Status != null)
            {
                Sb.Append(string.Format(" and o.Status = '{0}'", searchModel.Status));
            }

            if (!string.IsNullOrEmpty(searchModel.ReceiptNo))
            {
                Sb.Append(string.Format(" and o.ReceiptNo like '{0}%'", searchModel.ReceiptNo));
            }
            if (!string.IsNullOrEmpty(searchModel.PartyFrom))
            {
                Sb.Append(string.Format(" and o.PartyFrom = '{0}'", searchModel.PartyFrom));
            }
            if (!string.IsNullOrEmpty(searchModel.PartyTo))
            {
                Sb.Append(string.Format(" and o.PartyTo = '{0}'", searchModel.PartyTo));

            }
            string str = Sb.ToString();
            //SecurityHelper.AddPartyFromPermissionStatement(ref str, "o", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, true);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref str, "o", "Type", "o", "PartyFrom", "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, true);
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                Sb.Append(string.Format(" and o.CreateDate between '{0}' and '{1}'", searchModel.StartDate, searchModel.EndDate));
                // HqlStatementHelper.AddBetweenStatement("StartTime", searchModel.DateFrom, searchModel.DateTo, "o", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                Sb.Append(string.Format(" and o.CreateDate >= '{0}'", searchModel.StartDate));

            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                Sb.Append(string.Format(" and o.CreateDate <= '{0}'", searchModel.EndDate));

            }
            if (!string.IsNullOrEmpty(searchModel.ExternalReceiptNo))
            {
                Sb.Append(string.Format(" and  o.WMSNo like '%{0}%'", searchModel.ExternalReceiptNo));

            }

            Sb.Append(" )");

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                Sb.Append(string.Format(" and  d.Item like '{0}%'", searchModel.Item));

            }

            return Sb.ToString();
        }
    }
}
