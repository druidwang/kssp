using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Web.Models.ORD
{
    public class ScheduleView
    {
        public ScheduleHead ScheduleHead { get; set; }

        public IList<ScheduleBody> ScheduleBodyList { get; set; }

        public DateTime? MinDate { get; set; }
    }


    public class ScheduleHead
    {
        //计划协议号
        public string OrderNoHead = "OrderNo";

        //序号
        public string SequenceHead = "Sequence";

        //零件号
        public string ItemHead = "Item";

        //描述
        public string ItemDescriptionHead = "ItemDescription";

        //旧图号
        public string ReferenceItemCodeHead = "ReferenceItemCode";

        //单位
        public string UomHead = "Uom";

        //包装数
        public string UnitCountHead = "UnitCount";

        //包装数
        public string LocationToHead = "LocationTo";

        public IList<ColumnCell> ColumnCellList { get; set; }

    }


    public class ScheduleBody
    {

        //计划协议号
        [Display(Name = "OrderDetail_ScheduleLineNo", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string OrderNo { get; set; }

        //序号
        [Display(Name = "OrderDetail_Sequence", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Sequence { get; set; }

        //零件号
        [Display(Name = "OrderDetail_Item", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Item { get; set; }

        //描述
        [Display(Name = "OrderDetail_ItemDescription", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ItemDescription { get; set; }

        //旧图号
        [Display(Name = "OrderDetail_ReferenceItemCode", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ReferenceItemCode { get; set; }

        //单位
        [Display(Name = "OrderDetail_Uom", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Uom { get; set; }

        //包装数
        [Display(Name = "OrderDetail_UnitCount", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal UnitCount { get; set; }

        //包装数
        [Display(Name = "OrderDetail_LocationTo", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string LocationTo { get; set; }

        //订单明细
        public List<RowCell> RowCellList { get; set; }

    }

    public class RowCell
    {
        public string OrderNo { get; set; }

        public string Sequence { get; set; }

        public string ScheduleType { get; set; }

        public string EndDate { get; set; }

        public Decimal OrderQty { get; set; }

        public Decimal ShippedQty { get; set; }

        public string Qty { get; set; }

        public string DisplayQty
        {
            get
            {
                return this.OrderQty.ToString("F2") + "/" + this.ShippedQty.ToString("F2");
            }
        }
    }

    public class ColumnCell
    {
        public string ScheduleType { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
