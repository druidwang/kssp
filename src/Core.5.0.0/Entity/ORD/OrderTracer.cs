using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class OrderTracer
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [Display(Name = "ItemDescription", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public string ItemDescription { get; set; }

        #endregion
    }
}