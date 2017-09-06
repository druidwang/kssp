using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.VIEW
{
    public class MrpPlanTraceView
    {
        //物料号	物料描述	最大	最小	待收	待发	当前库存
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public double MaxStock { get; set; }
        public double SafeStock { get; set; }
        public double StartQty { get; set; }
        public double StartTransQty { get; set; }//期初在途库存
        public double InQty { get; set; }
        public double OutQty { get; set; }
        public double PlanInQty { get; set; }
        public double PlanOutQty { get; set; }
        public double EndQty { get; set; }
        public double EndTransQty { get; set; }//期末在途库存
        public IList<MrpPlanTraceDetailView> MrpPlanTraceDetailViewList { get; set; }
    }

    public class MrpPlanTraceDetailView
    {
        //可能原因（质量，生产，客户） 订单号 时间 库位 单位 数量
        //Item OrderNo Qty QualityType TransType IOType LocFrom LocTo CreateDate
        public string Item { get; set; }
        //public string Reason { get; set; }
        public string OrderNo { get; set; }
        public double Qty { get; set; }
        public CodeMaster.QualityType QualityType { get; set; }
        public CodeMaster.TransactionType TransType { get; set; }
        public CodeMaster.TransactionIOType IOType { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public DateTime CreateDate { get; set; }
        //public string Location { get; set; }
        //public string LocationName { get; set; }
        public string Uom { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TransactionType, ValueField = "TransType")]
        public string TransTypeDescription { get; set; }
    }
}