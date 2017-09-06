using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class MrpFlowDetail : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Display(Name = "MrpFlowDetail_Flow", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string Flow { get; set; }
         [Display(Name = "MrpFlowDetail_DetailId", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public Int32 DetailId { get; set; }
        public com.Sconit.CodeMaster.OrderType Type { get; set; }
         [Display(Name = "MrpFlowDetail_Item", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string Item { get; set; }
        [Display(Name = "MrpFlowDetail_LocationFrom", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string LocationFrom { get; set; }
        [Display(Name = "MrpFlowDetail_LocationTo", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string LocationTo { get; set; }
        [Display(Name = "MrpFlowDetail_PartyFrom", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string PartyFrom { get; set; }
        [Display(Name = "MrpFlowDetail_PartyTo", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string PartyTo { get; set; }
       
        public double LeadTime { get; set; }
        public Int32 MrpPriority { get; set; }
        public double MrpWeight { get; set; }
        public string Bom { get; set; }
        public double MaxStock { get; set; }
        public double SafeStock { get; set; }
         [Display(Name = "MrpFlowDetail_SnapTime", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public DateTime SnapTime { get; set; }
         [Display(Name = "MrpFlowDetail_Machine", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string Machine { get; set; }
        //public Double ShiftQuota { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public double MinLotSize { get; set; }

        public int Sequence { get; set; }

        public CodeMaster.ResourceGroup ResourceGroup { get; set; }
        [Display(Name = "MrpFlowDetail_UnitCount", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public double UnitCount { get; set; }
        [Display(Name = "MrpFlowDetail_Uom", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string Uom { get; set; }
        //public string Uom1 { get; set; }
        public double BatchSize { get; set; }
        [Display(Name = "MrpFlowDetail_ExtraLocationTo", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string ExtraLocationTo { get; set; }
        [Display(Name = "MrpFlowDetail_ExtraLocationFrom", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string ExtraLocationFrom { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MrpFlowDetail another = obj as MrpFlowDetail;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }

}
