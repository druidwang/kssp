using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpExOrder : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 SectionId { get; set; }
        [Display(Name = "MrpExOrder_PlanNo", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public string PlanNo { get; set; }
        [Display(Name = "MrpExOrder_ProductLine", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public string ProductLine { get; set; }
        [Display(Name = "MrpExOrder_DateIndex", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public string DateIndex { get; set; }
        [Display(Name = "MrpExOrder_Section", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public string Section { get; set; }
        [Display(Name = "MrpExOrder_PlanVersion", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public DateTime PlanVersion { get; set; }
        [Display(Name = "MrpExOrder_Sequence", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public Int32 Sequence { get; set; }
        [Display(Name = "MrpExOrder_Status", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public CodeMaster.OrderStatus Status { get; set; }
        [Display(Name = "MrpExOrder_StartTime", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public DateTime StartTime { get; set; }
        [Display(Name = "MrpExOrder_WindowTime", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public DateTime WindowTime { get; set; }
        [Display(Name = "MrpExOrder_StartDate", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public DateTime? StartDate { get; set; }
        [Display(Name = "MrpExOrder_CloseDate", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public DateTime? CloseDate { get; set; }

        public double ScrapQty { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        #endregion

    }

}
