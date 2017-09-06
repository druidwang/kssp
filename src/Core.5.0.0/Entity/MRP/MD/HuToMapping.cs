using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class HuToMapping
    {
        #region Non O/R Mapping Properties
        //HuTo	Party	Flow	Item
        [Display(Name = "HuToMapping_HuToDescription", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string HuToDescription { get; set; }
        [Display(Name = "HuToMapping_PartyName", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string PartyName { get; set; }
        [Display(Name = "HuToMapping_FlowDescription", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string FlowDescription { get; set; }
        [Display(Name = "HuToMapping_ItemDescription", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string ItemDescription { get; set; }
        [Display(Name = "HuToMapping_FgDescription", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string FgDescription { get; set; }
        [Display(Name = "HuToMapping_PartyType", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string PartyType { get; set; }

        public List<string> FgList { get; set; }
        #endregion
    }
}