using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpExShiftPlan : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        public int ItemId { get; set; }
        public DateTime ReleaseVersion { get; set; }
        public DateTime PlanVersion { get; set; }
        [Display(Name = "MrpExShiftPlan_PlanNo", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string PlanNo { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 10)]
        [Display(Name = "MrpExShiftPlan_Sequence", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public Int32 Sequence { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 50)]
        [Display(Name = "MrpExShiftPlan_ProductLine", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string ProductLine { get; set; }
        [Display(Name = "MrpExShiftPlan_DateIndex", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string DateIndex { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 20)]
        [Display(Name = "MrpExShiftPlan_Item", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string Item { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 140)]
        [Display(Name = "MrpExShiftPlan_Qty", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public Double Qty { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 110)]
        [Display(Name = "MrpExShiftPlan_UnitCount", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public Double UnitCount { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 80)]
        [Display(Name = "MrpExShiftPlan_Uom", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string Uom { get; set; }
        [Display(Name = "MrpExShiftPlan_RateQty", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public double RateQty { get; set; }


        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 60)]
        [Display(Name = "MrpExShiftPlan_StartTime", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public DateTime StartTime { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 70)]
        [Display(Name = "MrpExShiftPlan_WindowTime", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public DateTime WindowTime { get; set; }
        //[Display(Name = "MrpExShiftPlan_ProductType", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        //public CodeMaster.ProductType ProductType { get; set; }
        [Display(Name = "MrpExShiftPlan_Section", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string Section { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 130)]
        [Display(Name = "MrpExShiftPlan_IsCorrection", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public bool IsCorrection { get; set; }
        //public bool IsClose { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 120)]
        [Display(Name = "MrpExShiftPlan_IsNew", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public bool IsNew { get; set; }//²åµ¥
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 90)]
        [Display(Name = "MrpExShiftPlan_Remark", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string Remark { get; set; }

        [Display(Name = "MrpExShiftPlan_Bom", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string Bom { get; set; }
        [Display(Name = "MrpExShiftPlan_LocationFrom", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string LocationFrom { get; set; }
        [Display(Name = "MrpExShiftPlan_LocationTo", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string LocationTo { get; set; }

        //public Double CorrectionQty { get; set; }
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 150)]
        [Display(Name = "MrpExShiftPlan_ReceivedQty", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public Double ReceivedQty { get; set; }

        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 40)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExShiftPlan_Shift", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string Shift { get; set; }

        public DateTime PlanStartTime { get; set; }
        public DateTime PlanWindowTime { get; set; }
        public Double Speed { get; set; }
        public Double SwitchTime { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 30)]
        [Display(Name = "MrpExShiftPlan_ItemDescription", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string ItemDescription { get; set; }

        [Display(Name = "MrpExShiftPlan_ShiftQty", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public Double ShiftQty { get; set; }
        public int ShiftType { get; set; }
        public DateTime PlanDate { get; set; }
        public string OrderNo { get; set; }
        public int TurnQty { get; set; }

        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 100)]
        [Display(Name = "MrpExShiftPlan_IsFreeze", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public bool IsFreeze { get; set; }
        [Display(Name = "ProdLineEx_ProductType", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string ProductType { get; set; }
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
            MrpExShiftPlan another = obj as MrpExShiftPlan;

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

