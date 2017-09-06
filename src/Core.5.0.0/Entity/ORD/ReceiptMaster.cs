using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.INV;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class ReceiptMaster
    {
        #region Non O/R Mapping Properties

        [DataMember]
        public IList<ReceiptDetail> ReceiptDetails { get; set; }
        [Export(ExportName = "ProductionReceiptOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "SupplierReceiptOrderMaster", ExportSeq = 70)]
        [Export(ExportName = "SupplierReturnReceiptOrderMaster", ExportSeq = 60)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 60)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ReceiptStatus, ValueField = "Status")]
        [Display(Name = "ReceiptMaster_StatusDescription", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ReceiptMasterStatusDescription { get; set; }
        [Export(ExportName = "ProductionReceiptOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "ReceiptOrderMaster", ExportSeq = 60)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 70)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderSubType, ValueField = "OrderSubType")]
        [Display(Name = "ReceiptMaster_OrderSubTypeDescription", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string OrderSubTypeDescription { get; set; }

        public FlowMaster CurrentFlowMaster { get; set; }

        public string HuTemplate { get; set; }
        public string Shift { get; set; }

        public DateTime StartTime { get; set; }
        #endregion

        #region methods
        public void AddReceiptDetail(ReceiptDetail receiptDetail)
        {
            if (ReceiptDetails == null)
            {
                ReceiptDetails = new List<ReceiptDetail>();
            }
            ReceiptDetails.Add(receiptDetail);
        }
        #endregion

        public List<Hu> HuList { get; set; }
    }
}