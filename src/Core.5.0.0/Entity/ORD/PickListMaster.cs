using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
using System.Collections.Generic;
using System.Runtime.Serialization;
//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class PickListMaster
    {
        #region Non O/R Mapping Properties
        [Export(ExportName = "PickListMaster", ExportSeq = 40)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "PickListMaster_OrderType", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string OrderTypeDescription { get; set; }
        [Export(ExportName = "PickListMaster", ExportSeq = 70)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.PickListStatus, ValueField = "Status")]
        [Display(Name = "PickListMaster_Status", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string OrderStatusDescription { get; set; }

        [DataMember]
        public IList<PickListDetail> PickListDetails { get; set; }

        public IList<PickListResult> PickListResults { get; set; }

        public IList<OrderDetail> OrderDetails { get; set; }
        #endregion

        public void AddPickListDetail(PickListDetail pickListDetail)
        {
            if (PickListDetails == null)
            {
                PickListDetails = new List<PickListDetail>();
            }
            PickListDetails.Add(pickListDetail);
        }

        public void AddPickListResult(PickListResult pickListResult)
        {
            if (PickListResults == null)
            {
                PickListResults = new List<PickListResult>();
            }
            PickListResults.Add(pickListResult);
        }   
    }
}