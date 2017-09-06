using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class Machine
    {
        #region Non O/R Mapping Properties
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ShiftType, ValueField = "ShiftType")]
        [Display(Name = "Machine_ShiftType", ResourceType = typeof(Resources.MRP.Machine))]
        public string ShiftTypeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.MachineType, ValueField = "MachineType")]
        [Display(Name = "Machine_MachineType", ResourceType = typeof(Resources.MRP.Machine))]
        public string MachineTypeDescription { get; set; }
        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Description))
                {
                    return this.Code;
                }
                else
                {
                    return this.Code + " [" + this.Description + "]";
                }
            }
        }

        public double IslandQty { get; set; }

        public string IslandDescription { get; set; }
  

        #endregion
    }
}