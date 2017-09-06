using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.WMS
{
    public partial class PickTask
    {
        #region Non O/R Mapping Properties

        [Display(Name = "PickTask_DiaplayPickQty", ResourceType = typeof(Resources.WMS.PickTask))]
        public string DiaplayOrderQty
        {
            get
            {
                return this.PickQty.ToString("F0") + "/" + this.OrderQty.ToString("F0");
            }
        }

        #endregion
    }
}