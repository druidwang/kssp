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
using com.Sconit.Entity.SCM;
using AutoMapper;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Utility.Report;
using com.Sconit.Entity.Exception;
using System.Text;
using com.Sconit.Entity.INV;
using com.Sconit.Entity;
using com.Sconit.PrintModel.INV;
using com.Sconit.Entity.MD;
using NHibernate.Mapping;
using com.Sconit.Entity.MRP.MD;

namespace com.Sconit.Web.Controllers.ORD
{
    public class ProcurementSQOrderController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from OrderDetail as d";

        private static string selectStatement = "select d from OrderDetail as d";

        //
        // GET: /ProcurementSQOrder/
        //public IGenericMgr genericMgr { get; set; }

        public IHuMgr huMgr { get; set; }

        //public IReportGen reportGen { get; set; }

        public ISQLocationDetailMgr sqLocationDetailMgr { get; set; }

        #region public

        #region View
        [SconitAuthorize(Permissions = "Url_SQOrderMstr_Procurement_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SQOrderMstr_Procurement_View")]
        public ActionResult List(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (!string.IsNullOrEmpty(searchModel.Checker))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseInputKeeperToSearch);
            }
            ViewBag.PageSize =100;
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SQOrderMstr_Procurement_View")]
        public ActionResult _AjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (string.IsNullOrEmpty(searchModel.Checker))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }

            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<OrderDetail>(searchStatementModel, command));
        }
        #endregion

        #region Cancel DistributeLabel

        [SconitAuthorize(Permissions = "Url_SQOrderMstr_Procurement_Cancel")]
        public ActionResult CancelIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SQOrderMstr_Procurement_Cancel")]
        public ActionResult CancelList(GridCommand command, string HuId)
        {
            CheckHuIdIsNull(HuId, true);
            ViewBag.HuId = HuId;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        private bool CheckHuIdIsNull(string huId,bool bb) {
            if (!string.IsNullOrEmpty(huId))
            {
                IList<HuMapping> HuMappingListByHuId=this.genericMgr.FindAll<HuMapping>("select h from HuMapping as h where HuId=?",huId);
                if (HuMappingListByHuId.Count == 0)
                {
                    if (bb)
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_BarcodeCanNotFindPleaseReInput);
                    }
                    return false;
                }
                else
                {
                    if (bb)
                    {
                        TempData["_AjaxMessage"] = "";
                    }
                    return true;
                }

            }
            else
            {
                if (bb)
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseInputBarcodeToSearch);
                }
                return false;
            }
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SQOrderMstr_Procurement_Cancel")]
        public ActionResult _AjaxCancelList(GridCommand command, string HuId)
        {
            if (!CheckHuIdIsNull(HuId, false))
            {
                return PartialView(new GridModel(new List<HuMapping>()));
            }
            else
            {
                HuMapping huMapping = this.genericMgr.FindAll<HuMapping>("select h from HuMapping as h where HuId=?",HuId)[0];
                
                string[] oldHuIdArray = huMapping.OldHus.Split(';');
                IList<HuMapping> huMappingList = new List<HuMapping>();
                foreach (string oldHuId in oldHuIdArray)
                {
                    if (!string.IsNullOrEmpty(oldHuId))
                    {
                        HuMapping newHuMapping = new HuMapping() ;
                        newHuMapping.Id = huMapping.Id;
                        newHuMapping.OldHus = oldHuId;
                        newHuMapping.HuId = huMapping.HuId;
                        newHuMapping.Item = huMapping.Item;
                        newHuMapping.Qty = huMapping.Qty;
                        newHuMapping.IsEffective = huMapping.IsEffective;
                        newHuMapping.IsRepack = huMapping.IsRepack;
                        newHuMapping.OrderNo = huMapping.OrderNo;
                        newHuMapping.OrderDetId = huMapping.OrderDetId;
                        newHuMapping.CreateUserId = huMapping.CreateUserId;
                        newHuMapping.CreateUserName = huMapping.CreateUserName;
                        newHuMapping.CreateDate = huMapping.CreateDate;
                        newHuMapping.LastModifyUserId = huMapping.LastModifyUserId;
                        newHuMapping.LastModifyUserName = huMapping.LastModifyUserName;
                        newHuMapping.LastModifyDate = huMapping.LastModifyDate;
                        huMappingList.Add(newHuMapping);
                    }
                }
              
                GridModel<HuMapping> gridList = new GridModel<HuMapping>();
                gridList.Data=huMappingList;
                gridList.Total=oldHuIdArray.Length;
                return PartialView(gridList);
            }

        }

        public JsonResult DistributionLabelCancel(string HuId) {
            try
            {
                this.sqLocationDetailMgr.DistributionLabelCancel(HuId);
                object obj = new { successesulMessages = Resources.EXT.ControllerLan.Con_ShippingLabelCancelledSuccessfully };
                return Json(obj);
            }
            catch (BusinessException be)
            {
                 Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(be.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        #endregion

        #region print 打印导出

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public JsonResult PrintDistributeLabel(string ids, string orders)
        {
            string printUrl = string.Empty;
            try
            {
                BusinessException businessException = new BusinessException();
                List<Hu> huList = new List<Hu>();
                IList<OrderMaster> orderMasterList = GetOrders(ids, orders);
                orderMasterList = orderMasterList.OrderBy(o => o.WindowTime).ToList();
                IList<PrintHu> printHuList = CheckException(orderMasterList, businessException);
                if (printHuList != null && printHuList.Count > 0)
                {
                    IList<object> data = new List<object>();
                    data.Add(printHuList);
                    data.Add(CurrentUser.FullName);
                    printUrl = reportGen.WriteToFile("DistributeLabel.xls", data);
                }
                if (businessException.HasMessage)
                {
                    throw businessException;
                }
                object obj = new { printUrl = printUrl.ToString() };
                return Json(obj);
            }
            catch (BusinessException ex)
            {

                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                string messagesStr = "";
                IList<Message> messageList = ex.GetMessages();
                foreach (Message message in messageList)
                {
                    messagesStr +=  message.GetMessageString() ;
                }

                messagesStr += "*" + printUrl;

                Response.Write(messagesStr);
                return Json(null);
            }

        }
      

       

        /// <summary>
        /// 导出抛出错误信息
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="orders"></param>
        [HttpPost]
        public JsonResult SaveToClient(string ids, string orders)
        {
            string printUrl = string.Empty;
            try
            {
                BusinessException businessException = new BusinessException();
                List<Hu> huList = new List<Hu>();
                IList<OrderMaster> orderMasterList = GetOrders(ids, orders);
                orderMasterList = orderMasterList.OrderBy(o => o.WindowTime).ToList();
                IList<PrintHu> printHuList = CheckException(orderMasterList, businessException);
                if (printHuList!=null && printHuList.Count>0 ) 
                {
                    TempData["printHuList"] = printHuList;
                }
                if (businessException.HasMessage)
                {
                    throw businessException;
                }
            } 
            catch (BusinessException ex)
            {

                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                string messagesStr = "";
                IList<Message> messageList = ex.GetMessages();
                foreach (Message message in messageList)
                {
                    messagesStr +=  message.GetMessageString() ;
                }

                Response.Write(messagesStr);
            }
            return Json(null);
            
        }

        /// <summary>
        /// 导出
        /// </summary>
        public void SaveToClientTo()
        {
            IList<PrintHu> printHuList = (IList<PrintHu>)TempData["printHuList"];
            IList<object> data = new List<object>();
            data.Add(printHuList);
            data.Add(CurrentUser.FullName);
            reportGen.WriteToClient("DistributeLabel.xls", data, "DistributeLabel.xls");
        }

        public string Print(string ids, string orders)
        {

            try
            {
                StringBuilder sb = new StringBuilder();
                IList<OrderMaster> orderMasterList = this.GetOrders(ids, orders);

                foreach (OrderMaster orderMaster in orderMasterList)
                {

                    PrintOrderMaster printOrderMstr = Mapper.Map<OrderMaster, PrintOrderMaster>(orderMaster);
                    IList<object> data = new List<object>();
                    data.Add(printOrderMstr);
                    data.Add(printOrderMstr.OrderDetails);
                    string reportFileUrl = reportGen.WriteToFile(orderMaster.OrderTemplate, data);
                    sb.Append(reportFileUrl + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();

            }
            catch (BusinessException be)
            {

                return be.GetMessages()[0].GetMessageString();
            }

        }

        private IList<OrderMaster> GetOrders(string ids, string orders)
        {

            string[] idArray = ids.Split(',');
            string[] orderArray = orders.Split(',');
            if (idArray.Length == 0)
            {
                throw new BusinessException(Resources.EXT.ControllerLan.Con_PleaseChoosePrintedDetail);
            }
            string orderDetHql = string.Empty;
            string orderMstrHql = string.Empty;
            IList<object> idParm = new List<object>();
            IList<object> orderNoParm = new List<object>();
            int i = 0;
            foreach (string id in idArray)
            {
                if (orderDetHql == string.Empty)
                {
                    orderDetHql = "from OrderDetail as d where d.Id in (?";
                    orderMstrHql = "from OrderMaster as o where o.OrderNo in (?";
                }
                else
                {
                    orderDetHql += ", ?";
                    orderMstrHql += ",?";
                }
                idParm.Add(id);
                orderNoParm.Add(orderArray[i++]);
            }
            IList<OrderDetail> orderDetailList = this.genericMgr.FindAll<OrderDetail>(orderDetHql + ")", idParm.ToArray());
            IList<OrderMaster> orderMasterList = this.genericMgr.FindAll<OrderMaster>(orderMstrHql + ")", orderNoParm.ToArray());

            foreach (OrderMaster orderMaster in orderMasterList)
            {
                IList<OrderDetail> orderDetailByMaster = new List<OrderDetail>();
                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    if (orderMaster.OrderNo == orderDetail.OrderNo)
                    {
                        orderDetailByMaster.Add(orderDetail);
                    }
                    else
                    {
                        continue;
                    }
                }
                orderMaster.OrderDetails = orderDetailByMaster;

            }

            return orderMasterList;

        }

        private IList<PrintHu> CheckException(IList<OrderMaster> orderMasterList, BusinessException businessException)
        {
            List<Hu> huList = new List<Hu>();
            List<OrderDetail> jitOrderDetailList = new List<OrderDetail>();
            List<OrderDetail> nonJITOrderDetailList = new List<OrderDetail>();
            //foreach (var orderMaster in orderMasterList.Where(o => o.OrderStrategy != CodeMaster.FlowStrategy.JIT))
            //{
            //    nonJITOrderDetailList.AddRange(orderMaster.OrderDetails);
            //}
            //foreach (var orderMaster in orderMasterList.Where(o => o.OrderStrategy == CodeMaster.FlowStrategy.JIT))
            //{
            //    jitOrderDetailList.AddRange(orderMaster.OrderDetails);
            //}
            foreach (var orderMaster in orderMasterList.Where(o => o.OrderStrategy != CodeMaster.FlowStrategy.JIT && o.OrderStrategy != CodeMaster.FlowStrategy.Manual))
            {
                nonJITOrderDetailList.AddRange(orderMaster.OrderDetails);
            }
            foreach (var orderMaster in orderMasterList.Where(o => o.OrderStrategy == CodeMaster.FlowStrategy.JIT || o.OrderStrategy == CodeMaster.FlowStrategy.Manual))
            {
                jitOrderDetailList.AddRange(orderMaster.OrderDetails);
            }
            if (jitOrderDetailList.Count > 0)
            {
                huList.AddRange(this.sqLocationDetailMgr.MatchNewHuForRepack(jitOrderDetailList, true, businessException));
            }
            if (nonJITOrderDetailList.Count > 0)
            {
                IList<Hu> hus = this.sqLocationDetailMgr.MatchNewHuForRepack(nonJITOrderDetailList, false, businessException);
                huList.AddRange(hus);
            }
            if (huList.Count > 0)
            {
                foreach (var hu in huList)
                {
                    //hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                }
                foreach (var hu in huList)
                {
                    if (!string.IsNullOrWhiteSpace(hu.Direction))
                    {
                        hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                    }
                }


                IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
                return printHuList;
            }
            return new List<PrintHu>();

        }

     
        #endregion

        #endregion

        #region Private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {

            string whereStatement = "where exists (select 1 from OrderMaster as o where d.OrderNo=o.OrderNo and o.Type =" + (int)com.Sconit.CodeMaster.OrderType.Transfer + ""
                 + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal
                 + " and o.Status  in (" + (int)com.Sconit.CodeMaster.OrderStatus.Submit+","+(int)com.Sconit.CodeMaster.OrderStatus.InProcess+")";
            if (searchModel.Priority != null)
            {
                whereStatement += " and o.Priority=" + searchModel.Priority + "";
            }
            whereStatement += ") and exists( select 1 from Custodian as c where c.Item=d.Item and c.Location=d.LocationFrom and c.UserCode='" + searchModel.Checker + "')";
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "d", ref whereStatement, param);



            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.DateFrom, searchModel.DateTo, "d", ref whereStatement, param);
            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "d", ref whereStatement, param);
            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.DateTo, "d", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
                }
                else if (command.SortDescriptors[0].Member == "OrderPriorityDescription")
                {
                    command.SortDescriptors[0].Member = "Priority";
                }
                else if (command.SortDescriptors[0].Member == "OrderStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by d.CreateDate desc";
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
