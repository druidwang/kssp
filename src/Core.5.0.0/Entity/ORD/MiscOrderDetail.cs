using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class MiscOrderDetail
    {
        #region Non O/R Mapping Properties
        public IList<MiscOrderLocationDetail> MiscOrderLocationDetails { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 60)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 60)]
        [Export(ExportName = "Detail", ExportSeq = 60)]
        [Display(Name = "MiscOrderMstr_Note", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Note { get; set; }
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 60)]
        [Display(Name = "MiscOrderMstr_WBS", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string WBS { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 20)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 20)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 20)]
        [Export(ExportName = "Detail", ExportSeq = 20)]
        [Display(Name = "MiscOrderMstr_EffectiveDate", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public DateTime EffectiveDate { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 40)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 40)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 40)]
        [Export(ExportName = "Detail", ExportSeq = 40)]
        [Display(Name = "MiscOrderMstr_MoveType", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string MoveType { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 50)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 50)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 50)]
        [Display(Name = "MiscOrderMstr_Flow", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Flow { get; set; }
        [Export(ExportName = "Detail", ExportSeq = 50)]
        [Display(Name = "MiscOrderMstr_CostCenter", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string CostCenter { get; set; }

        public string Region { get; set; }
        #endregion

        public void AddMiscOrderLocationDetail(MiscOrderLocationDetail miscOrderLocationDetail)
        {
            if (this.MiscOrderLocationDetails == null)
            {
                this.MiscOrderLocationDetails = new List<MiscOrderLocationDetail>();
            }
            this.MiscOrderLocationDetails.Add(miscOrderLocationDetail);
        }
    }
}