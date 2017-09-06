using System;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SCM;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.PRD
{
    public partial class ProductLineLocationDetail
    {
        #region Non O/R Mapping Properties

        [Display(Name = "ProductLineLocationDetail_RemainBackFlushQty", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public decimal RemainBackFlushQty
        {
            get
            {
                return this.Qty - this.BackFlushQty - this.VoidQty;
            }

        }

        [Display(Name = "ProductLineLocationDetail_RemainBackFlushQty", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public decimal RemainQty { get; set; }

        [Display(Name = "ProductLineLocationDetail_CurrentRemainQty", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public decimal CurrentRemainQty { get; set; }

        [Display(Name = "BomDetail_UnitCount", ResourceType = typeof(Resources.PRD.Bom))]
        public Decimal UnitCount { get; set; }

        [Display(Name = "OrderBomDetail_Uom", ResourceType = typeof(Resources.ORD.OrderBomDetail))]
        public string Uom { get; set; }

        [Display(Name = "ProductLineLocationDetail_CurrentReturnQty", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public decimal CurrentReturnQty { get; set; }


        //public decimal CurrentVoidQty { get; set; }
        #endregion
    }

    public class FeedInput
    {

        #region ������Ͷ�ϱ����ֶ�
        public string Item { get; set; }
        public string Uom { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public Decimal Qty { get; set; }
        public string LocationFrom { get; set; }
        #endregion

        #region ������Ͷ�ϱ����ֶ�
        public string HuId { get; set; }
        #endregion

        #region �����ֶ�
        public string BaseUom { get; set; }
        public string OrderNo { get; set; }
        //׷����
        public string TraceCode { get; set; }
        public Int32? OrderDetailId { get; set; }
        public Int32? Operation { get; set; }
        public string OpReference { get; set; }
        public string ProductLine { get; set; }
        public string ProductLineFacility { get; set; }
        public string LotNo { get; set; }
        public Boolean IsConsignment { get; set; }
        public Int32? PlanBill { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }
        public Decimal UnitQty { get; set; }
        public Location CurrentLocationFrom { get; set; }
        public FlowMaster CurrentProductLine { get; set; }
        public OrderMaster CurrentOrderMaster { get; set; }
        public Item CurrentItem { get; set; }
        public string ReserveNo { get; set; }    //Ԥ����
        public string ReserveLine { get; set; }  //Ԥ���к�
        public string AUFNR { get; set; }        //SAP��������
        public string ICHARG { get; set; }       //SAP���κ�
        public string BWART { get; set; }        //�ƶ�����
        public bool NotReport { get; set; }       //������SAP
        #endregion
    }

    public class ReturnInput
    {
        //ָ������/��������������
        public int? ProductLineLocationDetailId { get; set; } 

        #region ���������ϱ����ֶ�
        public string Item { get; set; }
        public string Uom { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public Decimal Qty { get; set; }
        public string LocationTo { get; set; }   //�Ǳ���
        #endregion

        #region ���������ϱ����ֶ�
        public string HuId { get; set; }
        #endregion

        #region �����ֶ�
        public string BaseUom { get; set; }
        public string ProductLine { get; set; }
        public string ProductLineFacility { get; set; }
        public string OrderNo { get; set; }
        //׷����
        public string TraceCode { get; set; }
        public Int32? Operation { get; set; }
        public string OpReference { get; set; }
        public string LotNo { get; set; }
        //public Boolean IsConsignment { get; set; }
        //public Int32? PlanBill { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }
        public Decimal UnitQty { get; set; }
        public Location CurrentLocationTo { get; set; }
        public FlowMaster CurrentProductLine { get; set; }
        public OrderMaster CurrentOrderMaster { get; set; }
        #endregion
    }

    public class WeightAverageBackflushInput
    {
        public string Item { get; set; }
        public string Uom { get; set; }
        public Decimal Qty { get; set; }

        #region �����ֶ�
        public string BaseUom { get; set; }
        public string ProductLine { get; set; }
        public string ProductLineFacility { get; set; }
        //public string OrderNo { get; set; }
        //public Int32? Operation { get; set; }
        //public string OpReference { get; set; }
        //public string LotNo { get; set; }
        //public Boolean IsConsignment { get; set; }
        //public Int32? PlanBill { get; set; }
        //public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        //public com.Sconit.CodeMaster.OrderType OrderSubType { get; set; }
        public Decimal UnitQty { get; set; }
        //public Location CurrentLocationTo { get; set; }
        public FlowMaster CurrentProductLine { get; set; }
        //public OrderMaster CurrentOrderMaster { get; set; }
        #endregion
    }

    public class WeightAverageBackflushResult
    {
        public Decimal Qty { get; set; }
        public Decimal BaseQty { get; set; }
        public Decimal ActingQty { get; set; }
        public Decimal BaseActingQty { get; set; }

        public PlanBackflush PlanBackflush { get; set; }
        public ProductLineLocationDetail ProductLineLocationDetail { get; set; }
        public InventoryTransaction InventoryTransaction { get; set; }
        public FlowMaster CurrentProductLine { get; set; }
        public string LocationFrom { get; set; }
        public int? Operation { get; set; }
        public string OpReference { get; set; }
        public string ProductLine { get; set; }
        public string ProductLineFacility { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
    }

    public class BackflushInput
    {
        public string OrderNo { get; set; }
        public CodeMaster.OrderType OrderType { get; set; }
        public CodeMaster.OrderSubType OrderSubType { get; set; }
        public int OrderDetailSequence { get; set; }
        public int OrderDetailId { get; set; }
        public string ReceiptNo { get; set; }
        public int ReceiptDetailId { get; set; }
        public int ReceiptDetailSequence { get; set; }
        public OrderBomDetail OrderBomDetail { get; set; }
        public string FGItem { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ReferenceItemCode { get; set; }
        public string Uom { get; set; }
        public Decimal Qty { get; set; }
        public string TraceCode { get; set; }
        public string Location { get; set; }
        public com.Sconit.CodeMaster.QualityType FGQualityType { get; set; }
        //public com.Sconit.CodeMaster.QualityType QualityType { get; set; }  �س������һ���Ǻϸ�Ʒ�����ܻس岻�ϸ�Ʒ����
        public IList<ProductLineLocationDetail> ProductLineLocationDetailList { get; set; }
        public string BaseUom { get; set; }
        public string ProductLine { get; set; }
        public string ProductLineFacility { get; set; }
        public Int32? Operation { get; set; }
        public string OpReference { get; set; }
        //public Boolean IsConsignment { get; set; }
        //public Int32? PlanBill { get; set; }
        public Decimal UnitQty { get; set; }
        //public Location CurrentLocationTo { get; set; }
        public FlowMaster CurrentProductLine { get; set; }
        //public OrderMaster CurrentOrderMaster { get; set; }
        public string ReserveNo { get; set; }
        public string ReserveLine { get; set; }
        public string AUFNR { get; set; }
        public string BWART { get; set; }
        public string ICHARG { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public decimal ReceivedQty { get; set; }
        public string IpNo { get; set; }
        public IList<InventoryTransaction> InventoryTransactionList { get; set; }
    }
}