using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Web.Models.MRP
{
    public class FlowClassify
    {
        [Display(Name = "FlowClassify_Flow", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public string Flow { get; set; }
        [Display(Name = "FlowClassify_Classify01", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify01 { get; set; }
        [Display(Name = "FlowClassify_Classify02", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify02 { get; set; }
        [Display(Name = "FlowClassify_Classify03", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify03 { get; set; }
        [Display(Name = "FlowClassify_Classify04", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify04 { get; set; }
        [Display(Name = "FlowClassify_Classify05", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify05 { get; set; }
        [Display(Name = "FlowClassify_Classify06", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify06 { get; set; }
        [Display(Name = "FlowClassify_Classify07", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify07 { get; set; }
        [Display(Name = "FlowClassify_Classify08", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify08 { get; set; }
        [Display(Name = "FlowClassify_Classify09", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify09 { get; set; }
        [Display(Name = "FlowClassify_Classify10", ResourceType = typeof(Resources.MRP.FlowClassify))]
        public bool Classify10 { get; set; }
    }
}