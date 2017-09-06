namespace com.Sconit.Web.Controllers.EDI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Util;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.SCM;
    using System;
    using AutoMapper;
    using com.Sconit.Entity.MD;
    using NHibernate.Criterion;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Utility;
    using com.Sconit.Web.Models.ORD;
    using Telerik.Web.Mvc.UI;
    using System.Collections;
    using com.Sconit.Entity;
    using com.Sconit.Web.Models.SearchModels.SI.EDI;
    using com.Sconit.Entity.EDI.Ford;

    public class EDIScheduleLineController : WebAppBaseController
    {
        public EDIScheduleLineController()
        {
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_View")]
        public ActionResult List(GridCommand command, SearchModel searchModel)
        {
            TempData["OrderMasterSearchModel"] = searchModel;

            if (!searchModel.StartDate.HasValue)
            {
                SaveWarningMessage("请选择时间");
            }

            ScheduleView scheduleView = PrepareScheduleView(searchModel.StartDate.Value);

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
            #endregion

            #region
            if (scheduleView.ScheduleHead.ColumnCellList != null && scheduleView.ScheduleHead.ColumnCellList.Count > 0)
            {
                for (int i = 0; i < scheduleView.ScheduleHead.ColumnCellList.Count; i++)
                {
                    columns.Add(new GridColumnSettings
                    {
                        Member = "RowCellList[" + i + "].DisplayQty",
                        MemberType = typeof(string),
                        Title = scheduleView.ScheduleHead.ColumnCellList[i].EndDate + "(" + scheduleView.ScheduleHead.ColumnCellList[i].ScheduleType.ToString() + ")",
                        Sortable = false
                    });
                }
            }
            #endregion

            ViewData["columns"] = columns.ToArray();

            IList<ScheduleBody> scheduleBodyList = scheduleView.ScheduleBodyList != null && scheduleView.ScheduleBodyList.Count > 0 ? scheduleView.ScheduleBodyList : new List<ScheduleBody>();
            return View(scheduleBodyList);
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_View")]
        public ActionResult Refresh(string flow)
        {

            return View("Index");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_Ship")]
        public ActionResult ShipEdit(string flow, DateTime startDate)
        {

            return View(new object());

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_Ship")]
        public ActionResult _ShipOrderDetailList(string flow, DateTime startDate)
        {
            return PartialView(null);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_ScheduleLine_Ship")]
        public JsonResult ShipOrder(string idStr, string qtyStr, string flow, DateTime startDate)
        {
            return Json(null);
        }

        private RowCell GetRowCell(ScheduleBody scheduleBody, ColumnCell columnCell, IList<RowCell> rowCellList)
        {
            RowCell rowCell = new RowCell();
            var q = rowCellList.Where(r => r.OrderNo == scheduleBody.OrderNo && r.Sequence == scheduleBody.Sequence
                                            && r.ScheduleType == columnCell.ScheduleType && r.EndDate == columnCell.EndDate);
            if (q.ToList() != null && q.ToList().Count > 0)
            {
                rowCell = q.ToList().First();
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

        private ScheduleView PrepareScheduleView(DateTime publishDate)
        {
            ScheduleView scheduleView = new ScheduleView();
            ScheduleHead scheduleHead = new ScheduleHead();

            var orderDetailList = siMgr.FindAll<Schedule>("from Schedule where Schedule.PublishDate = ?", publishDate);

            #region head
            var c = from p in orderDetailList
                    group p by new { p.EndDate, p.ScheduleType } into g
                    select new ColumnCell
                    {
                        EndDate = g.Key.EndDate,
                        ScheduleType = g.Key.ScheduleType
                    };

            scheduleHead.ColumnCellList = c.OrderBy(p => p.StartDate).ThenBy(p => p.ScheduleType).ToList();
            #endregion

            #region body
            var s = from p in orderDetailList
                    group p by new { p.ScheduleNo, p.Item } into g
                    select new ScheduleBody
                    {
                        OrderNo = g.Key.ScheduleNo,
                        Item = g.Key.Item
                    };

            var r = from p in orderDetailList
                    group p by new { p.ScheduleNo, p.EndDate, p.ScheduleType, p.Qty } into g
                    select new RowCell
                    {
                        OrderNo = g.Key.ScheduleNo,
                        EndDate = g.Key.EndDate,
                        ScheduleType = g.Key.ScheduleType,
                        Qty = g.Key.Qty
                    };

            IList<ScheduleBody> scheduleBodyList = s.ToList();

            if (scheduleBodyList != null && scheduleBodyList.Count > 0)
            {
                foreach (ScheduleBody scheduleBody in scheduleBodyList)
                {
                    if (scheduleHead.ColumnCellList != null && scheduleHead.ColumnCellList.Count > 0)
                    {
                        List<RowCell> rowCellList = new List<RowCell>();
                        foreach (ColumnCell columnCell in scheduleHead.ColumnCellList)
                        {
                            RowCell rowCell = GetRowCell(scheduleBody, columnCell, r.ToList());
                            rowCellList.Add(rowCell);
                        }
                        scheduleBody.RowCellList = rowCellList;
                    }
                }
            }

            scheduleView.ScheduleBodyList = scheduleBodyList;
            #endregion

            scheduleView.ScheduleHead = scheduleHead;
            return scheduleView;
        }
    }
}
