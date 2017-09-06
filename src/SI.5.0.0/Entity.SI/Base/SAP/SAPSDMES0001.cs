using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPSDMES0001 : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        public string LIFNR { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 10)]
        [Display(Name = "SAPSDNormal_ZMESGUID", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string ZMESGUID { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 20)]
        [Display(Name = "SAPSDNormal_ZMESSO", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string ZMESSO { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 30)]
        [Display(Name = "SAPSDNormal_ZMESSOSEQ", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string ZMESSOSEQ { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 40)]
        [Display(Name = "SAPSDNormal_DOCTYPE", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string DOCTYPE { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 50)]
        [Display(Name = "SAPSDNormal_SALESORG", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string SALESORG { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 60)]
        [Display(Name = "SAPSDNormal_DISTRCHAN", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string DISTRCHAN { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 70)]
        [Display(Name = "SAPSDNormal_DIVISION", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string DIVISION { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 80)]
        [Display(Name = "SAPSDNormal_ORDREASON", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string ORDREASON { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 90)]
        [Display(Name = "SAPSDNormal_PRICEDATE", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public DateTime PRICEDATE { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 100)]
        [Display(Name = "SAPSDNormal_DOCDATE", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public DateTime DOCDATE { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 110)]
        [Display(Name = "SAPSDNormal_PARTNNUMB", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string PARTNNUMB { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 120)]
        [Display(Name = "SAPSDNormal_WADATIST", ResourceType = typeof(Resources.SI.SAPSDNormal))]
        public DateTime WADATIST { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 130)]
        [Display(Name = "SAPSDNormal_LIFEX", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string LIFEX { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 140)]
        [Display(Name = "SAPSDNormal_ITMNUMBER", ResourceType = typeof(Resources.SI.SAPSDNormal))]
        public string ITMNUMBER { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 150)]
        [Display(Name = "SAPSDNormal_MATERIAL", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string MATERIAL { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 160)]
        [Display(Name = "SAPSDNormal_TARGETQTY", ResourceType = typeof(Resources.SI.SAPSDNormal))]
        public string TARGETQTY { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 170)]
        [Display(Name = "SAPSDNormal_VRKME", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string VRKME { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 180)]
        [Display(Name = "SAPSDNormal_LGORT", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string LGORT { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 190)]
        [Display(Name = "SAPSDNormal_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public DateTime ZCSRQSJ { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq =200)]
        [Display(Name = "SAPSDNormal_Status", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public Int32 Status { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 210)]
        [Display(Name = "SAPSDNormal_BatchNo", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string BatchNo { get; set; }
        [Export(ExportName = "SAPSDNormal", ExportSeq = 220)]
        [Display(Name = "SAPSDNormal_UniqueCode", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string UniqueCode { get; set; }

        public string SALEORDNO { get; set; } 
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
            SAPSDMES0001 another = obj as SAPSDMES0001;

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
