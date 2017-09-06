using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.INV
{
    public partial class LocationLotDetail
    {
        #region Non O/R Mapping Properties
     
        #endregion
        [Export(ExportName = "LocationLotDetail", ExportSeq = 43)]
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }

        [Export(ExportName = "LocationLotDetail", ExportSeq = 46)]
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }

        [Export(ExportName = "LocationLotDetail", ExportSeq = 40)]
        [Display(Name = "LocationLotDetail_ItemDescription", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string ItemDescription { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 50)]
        [Display(Name = "LocationLotDetail_ReferenceItemCode", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string ReferenceItemCode { get; set; }

        [Display(Name = "LocationLotDetail_ItemDescription", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string ItemFullDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.ReferenceItemCode))
                {
                    return this.ItemDescription;
                }
                else
                {
                    return this.ItemDescription + " [" + this.ReferenceItemCode + "]";
                }
            }
        }

        public Boolean IsPick { get; set; }

        public string CheckOrderNo { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 130)]
        [Display(Name = "LocationLotDetail_Direction", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string DirectionDescription { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 75)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OccupyType, ValueField = "OccupyType")]
        [Display(Name = "LocationLotDetail_OccupyType", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string HuStatusOccupyTypeDescription { get; set; }


        [Export(ExportName = "LocationLotDetail", ExportSeq = 85, ExportTitleResourceType = typeof(Resources.INV.Hu), ExportTitle = "QualityType")]
        [com.Sconit.Entity.SYS.CodeDetailDescription(CodeMaster = com.Sconit.CodeMaster.CodeMaster.QualityType, ValueField = "QualityType")]
        [Display(Name = "LocationLotDetail_QualityType", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string QualityTypeDescription { get; set; }

        [Export(ExportName = "LocationLotDetail", ExportSeq = 80, ExportTitleResourceType = typeof(Resources.INV.Hu), ExportTitle = "Hu_HuOption")]
        [Display(Name = "LocationLotDetail_HuOption", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string HuOptionDesc { get; set; }

        [Export(ExportName = "LocationLotDetail", ExportSeq = 80, ExportTitleResourceType = typeof(Resources.INV.LocationLotDetail), ExportTitle = "LocationLotDetail_ItemVersion")]
        [Display(Name = "LocationLotDetail_ItemVersion", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string ItemVersion { get; set; }

        public string Reason { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 83, ExportTitleResourceType = typeof(Resources.INV.LocationLotDetail), ExportTitle = "LocationLotDetail_ItemVersion")]
        [Display(Name = "Hu_Remark", ResourceType = typeof(Resources.INV.Hu))]
        public string Remark { get; set; }
    }

    public class InventoryIO
    {
        public string Location { get; set; }
        public string Bin { get; set; }
        public string Item { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        //public string ManufactureParty { get; set; }
        public Decimal Qty { get; set; }   //都是库存单位的数量
        public Boolean IsCreatePlanBill { get; set; }  //为了区别是当前创建的PlanBill还是从库存中取得的PlanBill，供冲销使用
        public Boolean IsConsignment { get; set; }
        public Int32? PlanBill { get; set; }
        public Int32? ActingBill { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public bool IsFreeze { get; set; }
        public bool IsATP { get; set; }
        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }
        public string OccupyReferenceNo { get; set; }
        public com.Sconit.CodeMaster.TransactionType TransactionType { get; set; }
        public bool IsVoid { get; set; }
        public string ManufactureParty { get; set; }    //指定供应商物料出库
        public Int32? LocationLotDetailId { get; set; }    //指定供应商物料出库

        public DateTime EffectiveDate { get; set; }

        public Item CurrentItem { get; set; }
        public Location CurrentLocation { get; set; }
        public PlanBill CurrentPlanBill { get; set; }
        public ActingBill CurrentActingBill { get; set; }
        public Hu CurrentHu { get; set; }

        public string SupplierLotNo { get; set; }
    }

    public class InventoryTransaction
    {
        public int LocationLotDetailId { get; set; }
        public string Location { get; set; }
        public string Bin { get; set; }
        public string Item { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public decimal Qty { get; set; }
        public Boolean IsCreatePlanBill { get; set; }
        public Boolean IsConsignment { get; set; }
        public Int32? PlanBill { get; set; }
        public decimal PlanBillQty { get; set; }
        public Int32? ActingBill { get; set; }
        public decimal ActingBillQty { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public bool IsFreeze { get; set; }
        public bool IsATP { get; set; }
        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }
        public string OccupyReferenceNo { get; set; }
        public Int32 BillTransactionId { get; set; }
        public Int32 PlanBackflushId { get; set; }

        #region 辅助字段
        public decimal RemainQty { get; set; }
        public decimal RemainActingBillQty { get; set; }
        public int? Operation { get; set; }
        public string OpReference { get; set; }
        public string OrgLocation { get; set; }  //为了记录生产线投料回冲原库位代码
        public string WMSIpSeq { get; set; }  //为了记录WMS发货行号
        public string WMSRecSeq { get; set; }  //为了记录WMS收货行号

        public string ReserveNo { get; set; }    //预留号
        public string ReserveLine { get; set; }  //预留行号
        public string AUFNR { get; set; }        //SAP生产单号
        public string BWART { get; set; }        //移动类型
        public string ICHARG { get; set; }       //SAP批次
        public bool NotReport { get; set; }      //不导给SAP
        #endregion
    }

    public class InventoryPack
    {
        public string Location { get; set; }
        public string HuId { get; set; }
        public CodeMaster.OccupyType OccupyType { get; set; }
        public string OccupyReferenceNo { get; set; }
        public IList<Int32> LocationLotDetailIdList { get; set; }

        public HuStatus CurrentHu { get; set; }
        public Location CurrentLocation { get; set; }
    }

    public class InventoryUnPack
    {
        public string HuId { get; set; }

        public HuStatus CurrentHu { get; set; }
    }

    public class InventoryRePack
    {
        public CodeMaster.RePackType Type { get; set; }
        public string HuId { get; set; }

        public HuStatus CurrentHu { get; set; }
        public string Location { get; set; }
        public Location CurrentLocation { get; set; }
    }

    public class InventoryPick
    {
        public string HuId { get; set; }
    }

    public class InventoryPut
    {
        public string HuId { get; set; }
        public string Bin { get; set; }

        public HuStatus CurrentHu { get; set; }
        public LocationBin CurrentBin { get; set; }
    }

    public class InventoryOccupy
    {
        public string HuId { get; set; }
        public string Location { get; set; }
        public CodeMaster.QualityType QualityType { get; set; }
        public CodeMaster.OccupyType OccupyType { get; set; }
        public string OccupyReferenceNo { get; set; }
    }

    public class HuHistoryInventory
    {
        public string Location { get; set; }
        public string Item { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public Decimal Qty { get; set; }
        public CodeMaster.QualityType QualityType { get; set; }
    }
    //报表辅助类
    public class HistoryInventory
    {
        [Export(ExportName = "HistoryInventory", ExportSeq = 20)]
        [Display(Name = "HistoryInventoryView_Location", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public string Location { get; set; }
        [Export(ExportName = "HistoryInventory", ExportSeq = 10)]
        [Display(Name = "HistoryInventoryView_ItemFrom", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public string Item { get; set; }
        [Display(Name = "HistoryInventoryView_Qty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public Decimal Qty { get; set; }
        [Display(Name = "HistoryInventoryView_ConsignmentQty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public Decimal ConsignmentQty { get; set; }
        [Export(ExportName = "HistoryInventory", ExportSeq = 50)]
        [Display(Name = "HistoryInventoryView_QualifyQty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public Decimal QualifyQty { get; set; }
        [Export(ExportName = "HistoryInventory", ExportSeq =60)]
        [Display(Name = "HistoryInventoryView_InspectQty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public Decimal InspectQty { get; set; }
        [Export(ExportName = "HistoryInventory", ExportSeq = 70)]
        [Display(Name = "HistoryInventoryView_RejectQty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public Decimal RejectQty { get; set; }
        [Display(Name = "HistoryInventoryView_ATPQty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public Decimal ATPQty { get; set; }
        [Display(Name = "HistoryInventoryView_FreezeQty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public Decimal FreezeQty { get; set; }
        [Export(ExportName = "HistoryInventory", ExportSeq = 30)]
        [Display(Name = "HistoryInventoryView_LotNo", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public string LotNo { get; set; }

        [Display(Name = "HistoryInventoryView_ManufactureParty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public string ManufactureParty { get; set; }
        [Export(ExportName = "HistoryInventory", ExportSeq = 40)]
        [Display(Name = "HistoryInventoryView_CsQty", ResourceType = typeof(Resources.Report.HistoryInventoryView))]
        public Decimal CsQty { get; set; }
        public Decimal TobeQualifyQty { get; set; }
        
        public Decimal TobeInspectQty { get; set; }
      
        public Decimal TobeRejectQty { get; set; }
    }
    //报表辅助类
    public class InventoryAge
    {
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 30, ExportTitle = "InventoryAge_LocationRegion", ExportTitleResourceType = typeof(Resources.Report.InventoryAge))]
        [Display(Name = "InventoryAge_locationFrom", ResourceType = typeof(Resources.Report.InventoryAge))]
        public string Location { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 10)] 
        [Display(Name = "InventoryAge_itemFrom", ResourceType = typeof(Resources.Report.InventoryAge))]
        public string Item { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 20)] 
        [Display(Name = "InventoryAge_itemFromDesc", ResourceType = typeof(Resources.Report.InventoryAge))]
        public string ItemFromDesc { get; set; }

        [Export(ExportName = "libraryAgeStatements", ExportSeq = 23)]
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 26)]
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }

        [Export(ExportName = "libraryAgeStatements", ExportSeq = 40)] 
        [Display(Name = "InventoryAge_Range0", ResourceType = typeof(Resources.Report.InventoryAge))]
        public string Range0 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 50)] 
         [Display(Name = "InventoryAge_Range1", ResourceType = typeof(Resources.Report.InventoryAge))]
        public string Range1 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 60)] 
         [Display(Name = "InventoryAge_Range2", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range2 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 70)] 
         [Display(Name = "InventoryAge_Range3", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range3 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 80)] 
         [Display(Name = "InventoryAge_Range4", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range4 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 90)] 
         [Display(Name = "InventoryAge_Range5", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range5 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 100)] 
         [Display(Name = "InventoryAge_Range6", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range6 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 110)] 
         [Display(Name = "InventoryAge_Range7", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range7 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 120)] 
         [Display(Name = "InventoryAge_Range8", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range8 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 130)] 
         [Display(Name = "InventoryAge_Range9", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range9 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 140)] 
         [Display(Name = "InventoryAge_Range10", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range10 { get; set; }
        [Export(ExportName = "libraryAgeStatements", ExportSeq = 150)] 
         [Display(Name = "InventoryAge_Range11", ResourceType = typeof(Resources.Report.InventoryAge))]
         public string Range11 { get; set; }
    }
    //报表辅助类收发存
    public class Transceivers
    {
       [Display(Name = "Transceivers_itemFrom", ResourceType = typeof(Resources.Report.Transceivers))]
        public string Item { get; set; }
        [Display(Name = "Transceivers_locationFrom", ResourceType = typeof(Resources.Report.Transceivers))]
        public string Location { get; set; }
         [Display(Name = "Transceivers_SAPLocation", ResourceType = typeof(Resources.Report.Transceivers))]
        public string SAPLocation { get; set; }
         [Display(Name = "Transceivers_InputQty", ResourceType = typeof(Resources.Report.Transceivers))]
        public Decimal InputQty { get; set; }
         [Display(Name = "Transceivers_OutputQty", ResourceType = typeof(Resources.Report.Transceivers))]
        public Decimal OutputQty { get; set; }
         [Display(Name = "Transceivers_EOPQty", ResourceType = typeof(Resources.Report.Transceivers))]
        public Decimal EOPQty { get; set; }
         [Display(Name = "Transceivers_BOPQty", ResourceType = typeof(Resources.Report.Transceivers))]
        public Decimal BOPQty { get; set; }
    }
}