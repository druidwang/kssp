using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class ProductType
    {
        #region Non O/R Mapping Properties
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ScheduleType, ValueField = "SubType")]
        [Display(Name = "OrderMaster_SubType", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string OrderSubTypeDescription { get; set; }
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