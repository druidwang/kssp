using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.Serialization;
using com.Sconit.Entity.SCM;
//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class IpMaster
    {
        #region Non O/R Mapping Properties
        [Export(ExportName = "ProcurementIpMaster", ExportSeq = 60)]
        [Export(ExportName = "ProcurementIpGapMaster", ExportSeq = 60)]
        [Export(ExportName = "DistributionIpGapMaster", ExportSeq = 70)]
        [Export(ExportName = "DistributionIpMaster", ExportSeq = 80)]
        [Export(ExportName = "SupplierIpMaster", ExportSeq = 30)]
        [Export(ExportName = "DistributionReturnOrderMaster", ExportSeq = 80)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.IpStatus, ValueField = "Status")]
        [Display(Name = "IpMaster_Status", ResourceType = typeof(Resources.ORD.IpMaster))]
        public string IpMasterStatusDescription { get; set; }
        [Export(ExportName = "ProcurementIpMaster", ExportSeq = 30)]
        [Export(ExportName = "ProcurementIpGapMaster", ExportSeq = 30)]
        [Export(ExportName = "DistributionIpGapMaster", ExportSeq = 40)]
        [Export(ExportName = "DistributionIpMaster", ExportSeq = 40)]
        [Export(ExportName = "DistributionReturnOrderMaster", ExportSeq = 40)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "IpMaster_OrderType", ResourceType = typeof(Resources.ORD.IpMaster))]
        public string IpMasterTypeDescription { get; set; }
        [Export(ExportName = "ProcurementIpMaster", ExportSeq = 40)]
        [Export(ExportName = "DistributionIpMaster", ExportSeq = 50)]
        [Export(ExportName = "DistributionReturnOrderMaster", ExportSeq = 50)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderSubType, ValueField = "OrderSubType")]
        [Display(Name = "IpMaster_Type", ResourceType = typeof(Resources.ORD.IpMaster))]
        public string OrderSubTypeDescription { get; set; }
        public string FlowDescription { get; set; }
        public string CheckIpNo { get; set; }
        public string ShipmentNo { get; set; }

        public FlowMaster CurrentFlowMaster { get; set; }
        [DataMember]
        public IList<IpDetail> IpDetails { get; set; }

        #endregion

        #region methods
        public void AddIpDetail(IpDetail ipDetail)
        {
            if (IpDetails == null)
            {
                IpDetails = new List<IpDetail>();
            }
            IpDetails.Add(ipDetail);
        }
        #endregion
    }
}