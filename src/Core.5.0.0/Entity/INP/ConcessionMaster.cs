using System;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.INP
{
    public partial class ConcessionMaster
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ConcessionStatus, ValueField = "Status")]
        [Display(Name = "ConcessionMaster_Status", ResourceType = typeof(Resources.INP.ConcessionMaster))]
        public string ConcessionStatusDescription { get; set; }

        public IList<ConcessionDetail> ConcessionDetails { get; set; }

        #endregion
    }
}