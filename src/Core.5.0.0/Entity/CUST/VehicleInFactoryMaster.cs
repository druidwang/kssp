using System;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.CUST
{
    public partial class VehicleInFactoryMaster
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.VehicleInFactoryStatus, ValueField = "Status")]
        [Display(Name = "VehicleInFactory_Status", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public string VehicleInFactoryStatusDescription { get; set; }

        public IList<VehicleInFactoryDetail> VehicleInFactoryDetails { get; set; }

        #endregion
    }
}