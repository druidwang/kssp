using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class MachineInstance
    {
        #region Non O/R Mapping Properties
        [Export(ExportName = "FICalendar", ExportSeq = 50)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ShiftType, ValueField = "ShiftType")]
        [Display(Name = "MachineInstance_ShiftType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string ShiftTypeDescription { get; set; }

        [Export(ExportName = "FICalendar", ExportSeq = 40)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TimeUnit, ValueField = "DateType")]
        [Display(Name = "MachineInstance_DateType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string DateTypeDescription { get; set; }

        [Export(ExportName = "FICalendar", ExportSeq = 100)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.MachineType, ValueField = "MachineType")]
        [Display(Name = "MachineInstance_MachineType", ResourceType = typeof(Resources.MRP.MachineInstance))]
        public string MachineTypeDescription { get; set; }

        public DateTime? DateIndexDate { get; set; }

        #endregion
    }
}