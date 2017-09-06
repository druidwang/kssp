using System;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class SequenceMaster
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "SequenceMaster_OrderType", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public string OrderTypeDescription { get; set; }


        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.SequenceStatus, ValueField = "Status")]
        [Display(Name = "SequenceMaster_Status", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public string SequenceMasterStatusDescription { get; set; }

        public IList<SequenceDetail> SequenceDetails { get; set; }

        public IList<OrderMaster> OrderMasters { get; set; }

        public IList<OrderDetail> OrderDetails { get; set; }

        public string WMSIpNo { get; set; }
        #endregion

        public void AddSequenceDetail(SequenceDetail sequenceDetail)
        {
            if (SequenceDetails == null)
            {
                SequenceDetails = new List<SequenceDetail>();
            }

            SequenceDetails.Add(sequenceDetail);
        }
    }
}