
namespace com.Sconit.Web.Controllers.ORD
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.SCM;
    using System;
    using AutoMapper;
    using com.Sconit.Entity.MD;
    using NHibernate.Criterion;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Web.Models.ORD;
    using Telerik.Web.Mvc.UI;
    using System.Collections;
    using com.Sconit.Entity;


    public class ScheduleLineController : WebAppBaseController
    {

        //public IGenericMgr genericMgr { get; set; }
        public IIpMgr ipMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }

        public ScheduleLineController()
        {
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_Search")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_Search")]
        public ActionResult List(GridCommand command, OrderMasterSearchModel searchModel)
        {
            TempData["OrderMasterSearchModel"] = searchModel;

            if (string.IsNullOrWhiteSpace(searchModel.Flow))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseChooseOneFlow);
            }

            DateTime dateTimeNow = DateTime.Now;
            ScheduleView scheduleView = PrepareScheduleView(searchModel.Flow, searchModel.Item, searchModel.ScheduleType, searchModel.DateFrom, searchModel.DateTo, dateTimeNow);

            #region  grid column
            var columns = new List<GridColumnSettings>();
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.OrderNoHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_ScheduleLineNo,
                Sortable = false
            });
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.SequenceHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_ScheduleLineSeq,
                Sortable = false
            });
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.ItemHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_Item,
                Sortable = false
            });
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.ItemDescriptionHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_ItemDescription,
                Sortable = false
            });
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.ReferenceItemCodeHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_ReferenceItemCode,
                Sortable = false
            });
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.UomHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_Uom,
                Sortable = false
            });
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.UnitCountHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_UnitCount,
                Sortable = false
            });
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.LocationToHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_LocationTo,
                Sortable = false
            });
            columns.Add(new GridColumnSettings
            {
                Member = scheduleView.ScheduleHead.CurrentShipQtyToHead,
                Title = Resources.ORD.OrderDetail.OrderDetail_CurrentShipQty,
                Sortable = false
            });
            #endregion

            #region
            if (scheduleView.ScheduleHead.ColumnCellList != null && scheduleView.ScheduleHead.ColumnCellList.Count > 0)
            {
                for (int i = 0; i < scheduleView.ScheduleHead.ColumnCellList.Count; i++)
                {
                    string ScheduleType = scheduleView.ScheduleHead.ColumnCellList[i].ScheduleType.ToString() == "Firm" ? Resources.EXT.ControllerLan.Con_Settled : Resources.EXT.ControllerLan.Con_Forcast;
                    columns.Add(new GridColumnSettings
                    {
                        Member = "RowCellList[" + i + "].DisplayQty",
                        MemberType = typeof(string),
                        Title = (scheduleView.ScheduleHead.ColumnCellList[i].EndDate.Value < dateTimeNow) ? Resources.EXT.ControllerLan.Con_BackOrder : (scheduleView.ScheduleHead.ColumnCellList[i].EndDate.Value.ToString() + "(" + ScheduleType + ")"),
                        Sortable = false
                    });
                }
            }
            #endregion

            ViewData["columns"] = columns.ToArray();

            IList<ScheduleBody> scheduleBodyList = scheduleView.ScheduleBodyList != null && scheduleView.ScheduleBodyList.Count > 0 ? scheduleView.ScheduleBodyList.OrderBy(s => s.Item).ThenBy(s => s.OrderNo).ToList() : new List<ScheduleBody>();
            return View(scheduleBodyList);
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_Search")]
        public ActionResult Refresh(string flow)
        {
            try
            {
                if (string.IsNullOrEmpty(flow))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_FlowCodeCanNotBeEmpty);
                }
                //FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flow);
                //Region region = genericMgr.FindById<Region>(flowMaster.PartyTo);
                //SAPService.SAPService sapService = new SAPService.SAPService();
                //com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                //sapService.GetProcOrders(user.Code, string.Empty, flowMaster.PartyFrom, region.Plant, DateTime.Now);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_PlanProtocolRefreshSuccessfully);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }

            TempData["OrderMasterSearchModel"] = new OrderMasterSearchModel { Flow = flow };
            return View("Index");
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_Ship")]
        public JsonResult ShipOrderByQty(string OrderNoStr, string SequenceStr, string CurrentShipQtyStr)
        {
            try
            {
                if (!string.IsNullOrEmpty(OrderNoStr))
                {
                    string[] orderNoArray = OrderNoStr.Split(',');
                    string[] sequenceArray = SequenceStr.Split(',');
                    string[] currentShipQtyArray = CurrentShipQtyStr.Split(',');
                    IList<ScheduleLineInput> scheduleLineInputList = new List<ScheduleLineInput>();
                    int i = 0;
                    foreach (string orderNo in orderNoArray)
                    {

                        ScheduleLineInput scheduleLineInput = new ScheduleLineInput();
                        scheduleLineInput.EBELN = orderNoArray[i];
                        scheduleLineInput.EBELP = sequenceArray[i];
                        scheduleLineInput.ShipQty = int.Parse(currentShipQtyArray[i]);
                        scheduleLineInputList.Add(scheduleLineInput);
                        i++;
                    }
                    IpMaster ipMaster = this.orderMgr.ShipScheduleLine(scheduleLineInputList);
                    object obj = new { SuccessMessage = string.Format(Resources.ORD.OrderMaster.ScheduleLine_Shipped), IpNo = ipMaster.IpNo };
                    return Json(obj);
                }
                else
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ShippingDetailCanNotBeEmpty);
                }
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        private RowCell GetRowCell(ScheduleBody scheduleBody, ColumnCell columnCell, IList<RowCell> rowCellList, DateTime dateTimeNow)
        {
            RowCell rowCell = new RowCell();
            var q = rowCellList.Where(r => r.OrderNo == scheduleBody.OrderNo && r.Sequence == scheduleBody.Sequence
                                            && r.ScheduleType == columnCell.ScheduleType
                                            && (columnCell.EndDate < dateTimeNow ? r.EndDate < dateTimeNow : r.EndDate == columnCell.EndDate));

            if (q != null && q.Count() > 0)
            {
                rowCell = q.First();
                rowCell.OrderQty = q.Sum(oq => oq.OrderQty);
                rowCell.ShippedQty = q.Sum(oq => oq.ShippedQty);
                if (rowCell.EndDate < dateTimeNow && rowCell.OrderQty == rowCell.ShippedQty)
                {
                    rowCell.OrderQty = 0;
                    rowCell.ShippedQty = 0;
                }
            }
            else
            {
                rowCell.OrderNo = scheduleBody.OrderNo;
                rowCell.Sequence = scheduleBody.Sequence;
                rowCell.ScheduleType = columnCell.ScheduleType;
                rowCell.EndDate = columnCell.EndDate;
                rowCell.OrderQty = 0;
                rowCell.ShippedQty = 0;
            }
            return rowCell;
        }

        private ScheduleView PrepareScheduleView(string flow, string item, com.Sconit.CodeMaster.ScheduleType? schedeleType, DateTime? startDate, DateTime? endDate, DateTime dateTimeNow)
        {
            ScheduleView scheduleView = new ScheduleView();
            ScheduleHead scheduleHead = new ScheduleHead();
            if (!string.IsNullOrWhiteSpace(flow))
            {
                if (startDate == null)
                {
                    #region 取能发货的最小日期
                    string dateSql = "select min(d.EndDate) as EndDate from Ord_OrderDet_8 as d  where d.OrderType = ? and d.RecQty < d.OrderQty and exists (select 1 from Ord_OrderMstr_8 as o where o.Flow = ? and d.OrderNo = o.OrderNo) ";
                    IList<object> dateParam = new List<object>();
                    dateParam.Add((int)com.Sconit.CodeMaster.OrderType.ScheduleLine);
                    dateParam.Add(flow);

                    if (!string.IsNullOrEmpty(item))
                    {
                        dateSql += " and d.Item = ?";
                        dateParam.Add(item);
                    }
                    if (schedeleType != null)
                    {
                        dateSql += "  and d.ScheduleType = ?";
                        dateParam.Add(schedeleType.Value);
                    }
                    IList<DateTime?> dateList = genericMgr.FindAllWithNativeSql<DateTime?>(dateSql, dateParam.ToArray());
                    if (dateList[0] != null)
                    {
                        scheduleView.MinDate = dateList[0];
                        if (startDate == null)
                        {
                            startDate = dateList[0];
                        }
                    }
                    #endregion
                }
                if (startDate != null)
                {


                    string hql = "select d.EndDate,d.ScheduleType,d.OrderNo,d.ExtNo,left(extseq,charindex('-',extseq)-1),d.Item,d.Uom,d.UC,d.OrderQty,d.ShipQty from Ord_OrderDet_8 as d  where d.EndDate >= ? and d.OrderType = ? and exists (select 1 from Ord_OrderMstr_8 as o where o.Flow = ? and d.OrderNo=o.OrderNo)";

                    IList<object> param = new List<object>();

                    param.Add(startDate.Value);
                    param.Add((int)com.Sconit.CodeMaster.OrderType.ScheduleLine);
                    param.Add(flow);
                    if (!string.IsNullOrEmpty(item))
                    {
                        hql += " and d.Item = ?";
                        param.Add(item);
                    }
                    if (schedeleType != null)
                    {
                        hql += "  and d.ScheduleType = ?";
                        param.Add(schedeleType.Value);
                    }
                    if (endDate != null)
                    {
                        hql += " and d.EndDate <= ?";
                        param.Add(endDate.Value);
                    }
                    IList<object[]> orderDetailList = genericMgr.FindAllWithNativeSql<object[]>(hql, param.ToArray());

                    #region head


                    var c = from p in orderDetailList
                            group p by new
                            {
                                EndDate = (DateTime)p[0] < dateTimeNow ? dateTimeNow.AddDays(-1) : (DateTime)p[0],
                                ScheduleType = (byte)p[1]
                            } into g
                            select new ColumnCell
                            {
                                EndDate = g.Key.EndDate,
                                ScheduleType = (com.Sconit.CodeMaster.ScheduleType)g.Key.ScheduleType
                            };

                    scheduleHead.ColumnCellList = c.OrderBy(p => p.EndDate).ThenBy(p => p.ScheduleType).ToList();
                    #endregion

                    #region body
                    var s = from p in orderDetailList
                            group p by new
                            {
                                OrderNo = (string)p[2],
                                ExternalOrderNo = (string)p[3],
                                ScheduleLineSeq = (string)p[4],
                                Item = (string)p[5],
                                // ItemDescription = p.ItemDescription,
                                // ReferenceItemCode = !string.IsNullOrWhiteSpace(p.ReferenceItemCode) ? p.ReferenceItemCode :string.Empty, 
                                Uom = (string)p[6],
                                UnitCount = (decimal)p[7]
                            } into g
                            where g.Count(det => (decimal)det[8] != (decimal)det[9]) > 0
                            select new ScheduleBody
                            {
                                OrderNo = g.Key.ExternalOrderNo,
                                Sequence = g.Key.ScheduleLineSeq,
                                Item = g.Key.Item,
                                ItemDescription = genericMgr.FindById<Item>(g.Key.Item).Description,
                                ReferenceItemCode = genericMgr.FindById<Item>(g.Key.Item).ReferenceCode,
                                Uom = g.Key.Uom,
                                UnitCount = g.Key.UnitCount,
                                LocationTo = genericMgr.FindById<OrderMaster>(g.Key.OrderNo).LocationTo
                            };


                    var r = from p in orderDetailList
                            group p by new
                            {
                                OrderNo = (string)p[3],
                                ScheduleLineSeq = (string)p[4],
                                EndDate = (DateTime)p[0],
                                ScheduleType = (byte)p[1]
                            } into g
                            select new RowCell
                            {
                                OrderNo = g.Key.OrderNo,
                                Sequence = g.Key.ScheduleLineSeq,
                                EndDate = g.Key.EndDate,
                                ScheduleType = (com.Sconit.CodeMaster.ScheduleType)g.Key.ScheduleType,
                                OrderQty = g.Sum(p => (decimal)p[8]),
                                ShippedQty = g.Sum(p => (decimal)p[9])
                            };

                    IList<ScheduleBody> scheduleBodyList = s.ToList();
                    IList<RowCell> allRowCellList = r.ToList();
                    if (scheduleBodyList != null && scheduleBodyList.Count > 0)
                    {
                        foreach (ScheduleBody scheduleBody in scheduleBodyList)
                        {
                            if (scheduleHead.ColumnCellList != null && scheduleHead.ColumnCellList.Count > 0)
                            {
                                List<RowCell> rowCellList = new List<RowCell>();
                                foreach (ColumnCell columnCell in scheduleHead.ColumnCellList)
                                {
                                    RowCell rowCell = GetRowCell(scheduleBody, columnCell, allRowCellList, dateTimeNow);
                                    rowCellList.Add(rowCell);
                                }
                                scheduleBody.RowCellList = rowCellList;
                            }
                        }
                    }

                    scheduleView.ScheduleBodyList = scheduleBodyList;
                    #endregion

                }
            }

            scheduleView.ScheduleHead = scheduleHead;
            return scheduleView;
        }
    }
}
