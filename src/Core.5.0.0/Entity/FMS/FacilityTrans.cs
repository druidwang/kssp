using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    public partial class FacilityTrans
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.FacilityTransType, ValueField = "TransType")]
        [Display(Name = "FacilityTrans_TransType", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string TransTypeDescription { get; set; }

        #endregion
    }

}
