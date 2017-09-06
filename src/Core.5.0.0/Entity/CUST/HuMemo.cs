using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    public partial class HuMemo
    {
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ResourceGroup, ValueField = "ResourceGroup")]
        [Display(Name = "HuMemo_ResourceGroup_Type", ResourceType = typeof(Resources.CUST.HuMemo))]
        public string ResourceGroupDescription { get; set; }
    }
}
