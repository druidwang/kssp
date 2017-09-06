using System;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.BIL;
using System.Runtime.Serialization;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.INV;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class OrderMaster
    {
        public string IpNo { get; set; }

        #region Non O/R Mapping Properties
        [Export(ExportName = "ProcurementReturnOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "ProcurementOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "DistributionOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "SupplierOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "DistributionReturnOrderMaster", ExportSeq = 40)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "Type")]
        [Display(Name = "OrderMaster_Type", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string OrderTypeDescription { get; set; }
        [Export(ExportName = "ProductionOrderMaster", ExportSeq = 30)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderSubType, ValueField = "SubType")]
        [Display(Name = "OrderMaster_SubType", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string OrderSubTypeDescription { get; set; }
        [Export(ExportName = "ProcurementReturnOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "ProcurementOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "DistributionOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "SupplierOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "DistributionReturnOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "SupplierReturnOrderMaster", ExportSeq = 30)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderPriority, ValueField = "Priority")]
        [Display(Name = "OrderMaster_Priority", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string OrderPriorityDescription { get; set; }
        [Export(ExportName = "ProcurementReturnOrderMaster", ExportSeq = 90)]
        [Export(ExportName = "ProcurementOrderMaster", ExportSeq = 90)]
        [Export(ExportName = "ProductionOrderMaster", ExportSeq = 90)]
        [Export(ExportName = "DistributionOrderMaster", ExportSeq = 100)]
        [Export(ExportName = "SupplierOrderMaster", ExportSeq = 80)]
        [Export(ExportName = "DistributionReturnOrderMaster", ExportSeq = 100)]
        [Export(ExportName = "SupplierReturnOrderMaster", ExportSeq = 60)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderStatus, ValueField = "Status")]
        [Display(Name = "OrderMaster_Status", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string OrderStatusDescription { get; set; }

        public bool IsCheck { get; set; }
        [Export(ExportName = "ProcurementOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "DistributionOrderMaster", ExportSeq = 30)]
        [Display(Name = "OrderMaster_RefExtOrderNo", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string RefExtOrderNo
        {
            get
            {
                //string _refExtOrderNo = string.Empty;
                //if (_refExtOrderNo == string.Empty)
                //{
                //    if (!string.IsNullOrWhiteSpace(this.ReferenceOrderNo))
                //    {
                //        _refExtOrderNo = this.ReferenceOrderNo;
                //    }
                //}
                //if (!string.IsNullOrWhiteSpace(this.ExternalOrderNo))
                //{
                //    if (_refExtOrderNo == string.Empty)
                //    {
                //        _refExtOrderNo = this.ExternalOrderNo;
                //    }
                //    else
                //    {
                //        _refExtOrderNo = string.Format("{0}/{1}", _refExtOrderNo, this.ExternalOrderNo);
                //    }
                //}

                return string.Format("{0}/{1}", this.ReferenceOrderNo, this.ExternalOrderNo);
            }
        }

        //用来做checkbox的头
        public string CheckOrderNo { get; set; }
        //public string BackUrl { get; set; }

        [DataMember]
        public IList<OrderDetail> OrderDetails { get; set; }
        //[DataMember]
        public IList<OrderBinding> OrderBindings { get; set; }
        [Display(Name = "OrderMaster_Checker", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string Checker { get; set; }

        public string ColorStyle
        {
            get
            {
                if (this.Status == com.Sconit.CodeMaster.OrderStatus.Create || this.Status == com.Sconit.CodeMaster.OrderStatus.Submit || this.Status == com.Sconit.CodeMaster.OrderStatus.InProcess)
                {
                    if (this.WindowTime < System.DateTime.Now)
                    {
                        return "Color:red";
                    }
                    else if (this.WindowTime >= System.DateTime.Now && this.StartTime <= System.DateTime.Now)
                    {
                        return "Color:orange";
                    }
                    else if (this.StartTime > System.DateTime.Now)
                    {
                        return "Color:green";
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        public string WindowTimeFromat { get { return this.WindowTime.ToString(); } }
        #endregion

        #region 辅助字段
        public PriceListMaster CurrentPriceListMaster { get; set; }
        public string DummyLocation { get; set; }  //虚拟库位，从ProductMap表获取，往下传递给分装生产单（KIT单）
        //public DateTime PlanDate { get; set; }
        //public DateTime PlanVersion { get; set; }
        public FlowMaster CurrentFlowMaster { get; set; }

        public IList<Hu> CurrentHuList { get; set; }
        #endregion

        #region methods
        public void AddOrderDetail(OrderDetail orderDetail)
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
            OrderDetails.Add(orderDetail);
        }
        //
        public void AddOrderBinding(OrderBinding orderBinding)
        {
            if (OrderBindings == null)
            {
                OrderBindings = new List<OrderBinding>();
            }
            OrderBindings.Add(orderBinding);
        }
        #endregion
    }
}