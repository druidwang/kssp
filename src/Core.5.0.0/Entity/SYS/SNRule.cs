using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SYS
{
    public partial class SNRule
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.DocumentsType, ValueField = "Code")]
        [Display(Name = "SNRule_Description", ResourceType = typeof(Resources.SYS.SNRule))]
        public string DocumentsTypeDescription { get; set; }

        #endregion
    }
}
