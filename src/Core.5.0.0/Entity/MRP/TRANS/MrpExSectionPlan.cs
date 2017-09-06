using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpExSectionPlan
    {
        [Export(ExportName = "EXPlanSection", ExportSeq = 120)]
        [Display(Name = "MrpExSectionPlan_TotalQty", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double TotalQty
        {
            get
            {
                return this.Qty + this.AdjustQty + this.CorrectionQty;
            }
        }

        //°à²úÁ¿
        public double ShiftQuota
        {
            get
            {
                int _shift = (int)this.ShiftType;
                _shift = _shift == 0 ? 3 : _shift;
                return this.Speed * this.SpeedTimes * 60 * (24 / _shift);
            }
        }

        public string Color { get; set; }
        public int NewSequence { get; set; }

        public List<MrpExItemPlan> MrpExItemPlanList { get; set; }

        public void AddMrpExPlanItem(MrpExItemPlan mrpExPlanItem)
        {
            if (MrpExItemPlanList == null)
            {
                MrpExItemPlanList = new List<MrpExItemPlan>();
            }
            MrpExItemPlanList.Add(mrpExPlanItem);
        }

        public string Shift { get; set; }
        public DateTime ShiftDateFrom { get; set; }
        public DateTime ShiftDateTo { get; set; }
        public double LeftShiftQty { get; set; }
        public double LeftSwitchTime { get; set; }

        public double CurrentQty { get; set; }

        public double LeftQty { get; set; }
        public double LeftAdjustQty { get; set; }
        public double LeftCorrectionQty { get; set; }

        public int NewId { get; set; }

        public string PrevSection { get; set; }

        public string PrevProductType { get; set; }

        public double PrevShiftQty { get; set; }


    }
}
