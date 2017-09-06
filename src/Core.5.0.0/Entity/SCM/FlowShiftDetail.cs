using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SCM
{
    public partial class FlowShiftDetail
    {
        #region Non O/R Mapping Properties
        [Display(Name = "FlowShiftDetail_Shift", ResourceType = typeof(Resources.SCM.FlowShiftDetail))]
        public string Shift_value
        {
            get { return this.Shift; }
            set { this.Shift = value; }
        }

        public Boolean IsActive { get; set; }
        #endregion
    }
}