using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.Serialization;
//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class ShipmentMaster
    {
        #region Non O/R Mapping Properties

        public IList<ShipmentDetail> ShipmentDetails { get; set; }
        public string FlowDescription { get; set; }
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.BillMasterStatus, ValueField = "Status")]
        [Display(Name = "ShipmentMaster_Status", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public string StatusDescription { get; set; }

        public IList<IpMaster> ipMasters { get; set; }

        #endregion
    }
}