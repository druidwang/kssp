using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.CUST
{
    public partial class ItemProperty
    {
        #region Non O/R Mapping Properties
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.Viscosity, ValueField = "Viscosity")]
        [Display(Name = "ItemProperty_Viscosity", ResourceType = typeof(Resources.CUST.ItemProperty))]
        public string ViscosityDescription { get; set; }


        #endregion
    }
}