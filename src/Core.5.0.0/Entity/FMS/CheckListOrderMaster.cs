using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.FMS
{
  
    public partial class CheckListOrderMaster 
    {
        #region Non O/R Mapping Properties
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.CheckListOrderStatus, ValueField = "Status")]
      
        public string CheckListOrderStatusDescription { get; set; }

        #endregion
    }

}
