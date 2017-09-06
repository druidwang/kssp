using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.MD;
using System;
using com.Sconit.Entity.SYS;
using System.Xml.Serialization;
//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class OrderDetail
    {

        [Display(Name = "OrderDetail_InvQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal InvQty { get; set; }

        [Display(Name = "OrderDetail_InTransQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal InTransQty { get; set; }

        [Display(Name = "OrderDetail_MaxStock", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal MaxStock { get; set; }

        public string IsProvisionalEstimateDesc { get; set; }

        public IList<OrderTracer> OrderTracerList { get; set; }

        #region Non O/R Mapping Properties
        public decimal ShipQtyInput
        {
            get
            {
                if (OrderDetailInputs != null
                    && OrderDetailInputs.Count > 0)
                {
                    return OrderDetailInputs.Sum(i => i.ShipQty);
                }
                return 0;
            }
        }

        public decimal ReceiveQtyInput
        {
            get
            {
                if (OrderDetailInputs != null
                    && OrderDetailInputs.Count > 0)
                {
                    return OrderDetailInputs.Sum(i => i.ReceiveQty);
                }
                return 0;
            }
        }

        public decimal PickQtyInput
        {
            get
            {
                if (OrderDetailInputs != null
                    && OrderDetailInputs.Count > 0)
                {
                    return OrderDetailInputs.Sum(i => i.PickQty);
                }
                return 0;
            }
        }

        public decimal ScrapQtyInput
        {
            get
            {
                if (OrderDetailInputs != null
                    && OrderDetailInputs.Count > 0)
                {
                    return OrderDetailInputs.Sum(i => i.ScrapQty);
                }
                return 0;
            }
        }

        [Display(Name = "OrderDetail_CurrentShipQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal CurrentShipQty { get; set; }

        [Display(Name = "OrderDetail_CurrentReceiveQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal CurrentReceiveQty { get; set; }

        [Display(Name = "OrderDetail_Direction", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string DirectionDescription { get; set; }

        [Display(Name = "OrderDetail_CurrentScrapQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal CurrentScrapQty { get; set; }

        [Display(Name = "OrderMaster_Status", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public CodeMaster.OrderStatus Status { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 130)]
        //[Export(ExportName = "ProcumentOrderDetail", ExportSeq = 130)]
        //[Export(ExportName = "DistributionOrderDetail", ExportSeq = 130)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 130)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderStatus, ValueField = "Status")]
        [Display(Name = "OrderMaster_Status", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string OrderStatusDescription { get; set; }

        [Display(Name = "OrderDetail_CurrentPickQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal CurrentPickQty { get; set; }

        public CodeMaster.HuOption OldOption { get; set; }

        public IList<OrderBomDetail> OrderBomDetails { get; set; }
        public IList<OrderOperation> OrderOperations { get; set; }
        //public IList<OrderDetailTrace> OrderDetailTraces { get; set; }
        public IList<OrderDetailInput> OrderDetailInputs { get; set; }
        public Item CurrentItem { get; set; }

        #endregion

        #region 辅助字段
        public decimal RemainShippedQty
        {
            get
            {
                decimal remainShippedQty = this.OrderedQty > 0 ?
                    (this.OrderedQty > this.ShippedQty ? this.OrderedQty - this.ShippedQty : 0)
                    : (this.OrderedQty < this.ShippedQty ? this.OrderedQty - this.ShippedQty : 0);
                return remainShippedQty;
            }
        }

        [Display(Name = "OrderDetail_CurrentReceiveQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal RemainReceivedQty
        {
            get
            {
                decimal remainReceivedQty = this.OrderedQty > 0 ?
                    (this.OrderedQty > this.ReceivedQty ? this.OrderedQty - this.ReceivedQty : 0)
                    : (this.OrderedQty < this.ReceivedQty ? this.OrderedQty - this.ReceivedQty : 0);
                return remainReceivedQty;
            }
        }

        public PriceListMaster CurrentPriceListMaster { get; set; }

        public BindDemand BindDemand { get; set; }

        [Display(Name = "OrderDetail_LocationQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal LocationQty { get; set; }
        /// <summary>
        /// 取Master 里面的窗口时间
        /// </summary>
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 90)]
        [Display(Name = "OrderMaster_WindowTime", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public DateTime? WindowTime { get; set; }

        [Display(Name = "OrderMaster_StartTime", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public DateTime StartTime { get; set; }

        [Display(Name = "OrderMaster_Shift", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string ShiftName { get; set; }

        [Display(Name = "FlowDetail_ManufactureDate", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public DateTime ManufactureDate { get; set; }

        [Display(Name = "FlowDetail_ManufactureDate", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string ManufactureDateStrFormat { get; set; }

        [Display(Name = "OrderMaster_PartyFromName", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string PartyFromName { get; set; }

        [Display(Name = "OrderMaster_PartyToName", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string PartyToName { get; set; }

        [Display(Name = "OrderDetail_CurrentPickListQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal CurrentPickListQty
        {
            get
            {
                return this.OrderedQty - this.ShippedQty - this.PickedQty;
            }
        }

        [Display(Name = "OrderDetail_HuQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal HuQty { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 100)]
        //[Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 100)]
        [Display(Name = "OrderDetail_LotNo", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string LotNo { get; set; }

        public int CheQty
        {
            get
            {
                if (this.UnitCount == 0)
                {
                    return (int)this.OrderedQty;
                }
                else
                {
                    return (int)(this.OrderedQty / this.UnitCount);
                }
            }
        }

        public string ScheduleLineSeq
        {
            get
            {
                string scheduleLineSeq = string.Empty;
                if (!string.IsNullOrEmpty(this.ExternalSequence))
                {
                    string[] scheduleLineSeqArray = this.ExternalSequence.Split('-');
                    scheduleLineSeq = scheduleLineSeqArray[0];
                }
                return scheduleLineSeq;
            }
        }

        [Display(Name = "OrderDetail_SupplierLotNo", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public String SupplierLotNo { get; set; }
        public String EBELN { get; set; }   //计划协议号
        public String EBELP { get; set; }   //计划协议行号
        //[Export(ExportName = "OrderDetail", ExportSeq = 150)]
        //[Export(ExportName = "ProcumentOrderDetail", ExportSeq = 150)]
        //[Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 140)]
        [Display(Name = "OrderDetail_Flow", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Flow { get; set; }

        public CodeMaster.RoundUpOption RoundUpOption { get; set; }
        public decimal MinLotSize { get; set; }
        #endregion

        #region methods
        public void AddOrderBomDetail(OrderBomDetail orderBomDetail)
        {
            if (OrderBomDetails == null)
            {
                OrderBomDetails = new List<OrderBomDetail>();
            }
            OrderBomDetails.Add(orderBomDetail);
        }

        public void AddOrderDetailInput(OrderDetailInput orderDetailInput)
        {
            if (OrderDetailInputs == null)
            {
                OrderDetailInputs = new List<OrderDetailInput>();
            }
            OrderDetailInputs.Add(orderDetailInput);
        }
        //public void AddOrderOperation(OrderOperation orderOperation)
        //{
        //    if (OrderOperations == null)
        //    {
        //        OrderOperations = new List<OrderOperation>();
        //    }
        //    OrderOperations.Add(orderOperation);
        //}        
        #endregion
    }

    public class OrderDetailInput
    {
        [Display(Name = "OrderDetail_CurrentShipQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal ShipQty { get; set; }

        [Display(Name = "OrderDetail_CurrentReceiveQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public decimal ReceiveQty { get; set; }

        public decimal ScrapQty { get; set; }

        public decimal PickQty { get; set; }

        public decimal HuQty { get; set; }

        //给生产收货用
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        //public decimal RejectQty { get; set; }

        public string HuId { get; set; }

        public string LotNo { get; set; }

        //收货时指定了追溯码，回冲在制品时，只回冲指定追溯码的在制品
        public string TraceCode { get; set; }

        public IList<ReceiptDetail> ReceiptDetails { get; set; }

        public string Bin { get; set; }

        public string WMSIpNo { get; set; }

        public string WMSIpSeq { get; set; }

        public string ManufactureParty { get; set; }

        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }

        public string OccupyReferenceNo { get; set; }

        //委外Ip收货收货单无法记录IpNo和IpDetId

        public string IpNo { get; set; }

        public int IpDetId { get; set; }

    }

    public class ScheduleLineInput
    {
        public string EBELN { get; set; }
        public string EBELP { get; set; }
        public decimal ShipQty { get; set; }
    }
}