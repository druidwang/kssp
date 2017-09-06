using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.VIEW
{
    public partial class HuStatus
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.HuStatus, ValueField = "Status")]
        [Display(Name = "Hu_HuStatusDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string HuStatusDescription { get; set; }
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.QualityType, ValueField = "QualityType")]
        [Display(Name = "Hu_HuQualityTypeDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string HuQualityTypeDescription { get; set; }
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OccupyType, ValueField = "OccupyType")]
        [Display(Name = "Hu_HuStatusOccupyTypeDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string HuStatusOccupyTypeDescription { get; set; }

        #endregion
    }
}