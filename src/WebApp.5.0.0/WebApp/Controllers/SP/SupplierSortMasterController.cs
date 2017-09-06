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
using System.Text;
using com.Sconit.Service;
using com.Sconit.Entity.SYS;
using com.Sconit.Service.Impl;
using com.Sconit.Entity.Exception;


namespace com.Sconit.Web.Controllers.SP
{
    public class SupplierSortMasterController : WebAppBaseController
    {
        //
        // GET: /SequenceOrder/

        private static string selectCountStatement = "select count(*) from SequenceMaster as s";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select s from SequenceMaster as s";
       
      
      //  public ISystemMgr systemMgr { get; set; }
        //public IGenericMgr genericMgr { get; set; }
        //public IOrderMgr orderMrg { get; set; }
        #region public
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DetailIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Sort_List_Query")]
        public ActionResult List(GridCommand command, SequenceMasterSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_Sort_List_Query")]
        public ActionResult _AjaxList(GridCommand command, SequenceMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<SequenceMaster>()));
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<SequenceMaster>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Sort_List_Query")]
        public ActionResult Edit(string SequenceNo)
        {
            ViewBag.SequenceNo = SequenceNo;
            SequenceMaster sequenceMaster = genericMgr.FindById<SequenceMaster>(SequenceNo);
            sequenceMaster.SequenceMasterStatusDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.SequenceStatus, ((int)sequenceMaster.Status).ToString());
            sequenceMaster.OrderTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.OrderType, ((int)sequenceMaster.OrderType).ToString());
            return View(sequenceMaster);
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Sort_List_Query")]
        public ActionResult _SequenceDetailList(string SequenceNo)
        {
            IList<SequenceDetail> sequenceList = genericMgr.FindAll<SequenceDetail>("from SequenceDetail as s where s.SequenceNo=? order by Sequence ", SequenceNo);
            return PartialView(sequenceList);
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Invoice_Detail_Query")]
        public ActionResult DetailList(GridCommand command, SequenceMasterSearchModel searchModel)
        {
            TempData["SequenceMasterSearchModel"] = searchModel;
            if (this.CheckSearchModelIsNull(searchModel))
            {
                TempData["_AjaxMessage"] = "";
            
            IList<SequenceDetail> list = genericMgr.FindAll<SequenceDetail>(PrepareSearchDetailStatement(command, searchModel)); //GetPageData<OrderDetail>(searchStatementModel, command);

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
                return View(new List<SequenceDetail>());
            }
        }
        #endregion

        #region Ship
        [GridAction(EnableCustomBinding = true)]
         [SconitAuthorize(Permissions = "Url_Sort_List_Query")]
        public JsonResult ShipSequenceOrderBySupplier(string SequenceNo)
        {
            try
            {
             IpMaster ipMaster = orderMgr.ShipSequenceOrderBySupplier(SequenceNo);

             SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SortedOrder + SequenceNo + Resources.EXT.ControllerLan.Con_ShipSuccessfully);
             object obj = new { SuccessMessage = string.Format(Resources.ORD.SequenceMaster.SequenceMaster_Shipped, SequenceNo), IpNo = ipMaster.IpNo };
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


        #region private
        private string PrepareSearchDetailStatement(GridCommand command, SequenceMasterSearchModel searchModel)
        {
            StringBuilder Sb = new StringBuilder();
            string whereStatement = " select  d from SequenceDetail as d  where exists (select 1 from SequenceMaster  as o where o.OrderType in (" + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + ")"
                                    + " and o.SequenceNo=d.SequenceNo ";

            Sb.Append(whereStatement);
            if (searchModel.Status != null)
            {
                Sb.Append(string.Format(" and o.Status = '{0}'", searchModel.Status));
            }
            if (!string.IsNullOrEmpty(searchModel.SequenceNo))
            {
                Sb.Append(string.Format(" and o.SequenceNo like '{0}%'", searchModel.SequenceNo));
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

            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                Sb.Append(string.Format(" and o.CreateDate between '{0}' and '{1}'", searchModel.StartTime, searchModel.EndTime));
                // HqlStatementHelper.AddBetweenStatement("StartTime", searchModel.DateFrom, searchModel.DateTo, "o", ref whereStatement, param);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                Sb.Append(string.Format(" and o.CreateDate >= '{0}'", searchModel.StartTime));

            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                Sb.Append(string.Format(" and o.CreateDate <= '{0}'", searchModel.EndTime));

            }

            Sb.Append(" )");

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                Sb.Append(string.Format(" and  d.Item like '{0}%'", searchModel.Item));

            }

            return Sb.ToString();
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, SequenceMasterSearchModel searchModel)
        {
            string whereStatement = " where s.OrderType in (" + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract +"," + (int)com.Sconit.CodeMaster.OrderType.Transfer+"," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ")";
            IList<object> param = new List<object>();

            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "s", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, true);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "s", "OrderType", "s", "PartyFrom", "s", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, true);

            HqlStatementHelper.AddLikeStatement("SequenceNo", searchModel.SequenceNo, HqlStatementHelper.LikeMatchMode.Start, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "s", ref whereStatement, param);
            //HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "s", ref whereStatement, param);
            //HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "s", ref whereStatement, param);
           

            if (searchModel.StartTime!= null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddBetweenStatement("StartTime", searchModel.StartTime, searchModel.EndTime, "s", ref whereStatement, param);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                HqlStatementHelper.AddGeStatement("StartTime", searchModel.StartTime, "s", ref whereStatement, param);
            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddLeStatement("StartTime", searchModel.EndTime, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "SequenceMasterStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
                else if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "OrderType";
                }
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by s.CreateDate desc";
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
