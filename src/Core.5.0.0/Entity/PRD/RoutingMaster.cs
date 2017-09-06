using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
using System.Collections.Generic;
//TODO: Add other using statements here

namespace com.Sconit.Entity.PRD
{
    public partial class RoutingMaster
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.RoutingTimeUnit, ValueField = "TaktTimeUnit")]
        [Display(Name = "RoutingMaster_TaktTimeUnit", ResourceType = typeof(Resources.PRD.Routing))]
        public string RoutingTimeUnitDescription { get; set; }

        public IList<RoutingDetail> RoutingDetails { get; set; }
        #endregion

        public void AddRoutingDetail(RoutingDetail routingDetail)
        {
            if (RoutingDetails ==null)
            {
                RoutingDetails = new List<RoutingDetail>();
            }

            RoutingDetails.Add(routingDetail);
        }
        public RoutingMaster()
        {
        }

        public RoutingMaster(string code)
        {
            this.Code = code;
        }
        #region Non O/R Mapping Properties

        public string CodeName
        {
            get
            {
                return this.Code + " [" + this.Name + "]";
            }
        }

        #endregion

    }
}