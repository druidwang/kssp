using System;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class MiscOrderMaster
    {
        #region Non O/R Mapping Properties

        public IList<MiscOrderDetail> MiscOrderDetails;

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.QualityType, ValueField = "QualityType")]
        [Display(Name = "MiscOrderMstr_QualityTypeDescription", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string QualityTypeDescription { get; set; }

        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 60)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 50)]
        [Export(ExportName = "Master", ExportSeq = 50)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.MiscOrderStatus, ValueField = "Status")]
        [Display(Name = "MiscOrderMstr_StatusDescription", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string StatusDescription { get; set; }

        public int ReferenceDocumentsType { get; set; }

        public List<int> ReferenceIdList { get; set; }

        public decimal? WorkHour { get; set; }
        #endregion

        public void AddMiscOrderDetail(MiscOrderDetail miscOrderDetail)
        {
            if (this.MiscOrderDetails == null)
            {
                this.MiscOrderDetails = new List<MiscOrderDetail>();
            }
            this.MiscOrderDetails.Add(miscOrderDetail);
        }
    }
}