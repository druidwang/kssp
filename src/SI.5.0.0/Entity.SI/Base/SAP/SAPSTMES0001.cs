using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPSTMES0001 : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 10)]
        [Display(Name = "SAPSTMES_ZMESKO", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string ZMESKO { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 20)]
        [Display(Name = "SAPSTMES_ZMESKOSEQ", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string ZMESKOSEQ { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 30)]
        [Display(Name = "SAPSTMES_BWARTWA", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string BWARTWA { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 40)]
        [Display(Name = "SAPSTMES_WERKS", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string WERKS { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 50)]
        [Display(Name = "SAPSTMES_BLDAT", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public DateTime BLDAT { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 60)]
        [Display(Name = "SAPSTMES_BUDAT", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public DateTime BUDAT { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 70)]
        [Display(Name = "SAPSTMES_LGORT", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string LGORT { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 80)]
        [Display(Name = "SAPSTMES_KOSTL", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string KOSTL { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 90)]
        [Display(Name = "SAPSTMES_LIFNR", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string LIFNR { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 100)]
        [Display(Name = "SAPSTMES_MATNR1", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string MATNR1 { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 110)]
        [Display(Name = "SAPSTMES_EPFMG", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string EPFMG { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 120)]
        [Display(Name = "SAPSTMES_ERFME", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string ERFME { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 130)]
        [Display(Name = "SAPSTMES_MATNR_TH", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string MATNR_TH { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 140)]
        [Display(Name = "SAPSTMES_UMLGO", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string UMLGO { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 150)]
        [Display(Name = "SAPSTMES_ZMESGUID", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string ZMESGUID { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 160)]
        [Display(Name = "SAPSTMES_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public DateTime ZCSRQSJ { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 170)]
        [Display(Name = "SAPSTMES_Status", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public Int32 Status { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 180)]
        [Display(Name = "SAPSTMES_BatchNo", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public string BatchNo { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 190)]
        [Display(Name = "SAPMMMES_UniqueCode", ResourceType = typeof(Resources.SI.SAPMMMES))]
		public string UniqueCode { get; set; }
        [Export(ExportName = "SAPSTMES0001", ExportSeq = 200)]
        [Display(Name = "SAPSTMES_DataType", ResourceType = typeof(Resources.SI.SAPSTMES))]
		public Int32 DataType { get; set; }
        public string SPART { get; set; }
        
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
            SAPSTMES0001 another = obj as SAPSTMES0001;

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
