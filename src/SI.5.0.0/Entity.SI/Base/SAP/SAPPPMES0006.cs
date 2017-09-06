using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPPPMES0006 : EntityBase
    {
        #region O/R Mapping Properties

		public Int32 Id { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 10)]
        [Display(Name = "SAPPPMES_ZMESSC", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESSC { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 20)]
        [Display(Name = "SAPPPMES_ZMESLN", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESLN { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 30)]
        [Display(Name = "SAPPPMES_ZPTYPE", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZPTYPE { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 40)]
        [Display(Name = "SAPPPMES_AUFNR", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string AUFNR { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 45)]
        [Display(Name = "SAPPPMES_WERKS", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string WERKS { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 80)]
        [Display(Name = "SAPPPMES_BWART", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string BWART { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 80)]
        [Display(Name = "SAPPPMES_BWART_F", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string BWART_F { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 80)]
        [Display(Name = "SAPPPMES_BWART_S", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string BWART_S { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 90)]
        [Display(Name = "SAPPPMES_NPLNR", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string NPLNR { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 100)]
        [Display(Name = "SAPPPMES_VORNR", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string VORNR { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 110)]
        [Display(Name = "SAPPPMES_RSNUM", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string RSNUM { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 120)]
        [Display(Name = "SAPPPMES_RSPOS", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string RSPOS { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 130)]
        [Display(Name = "SAPPPMES_MATNR1", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string MATNR1 { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 120)]
        [Display(Name = "SAPPPMES_MTSNR", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string MTSNR { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 120)]
        [Display(Name = "SAPPPMES_LFSNR", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LFSNR { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 110)]
        [Display(Name = "SAPPPMES_BLDAT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime BLDAT { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 110)]
        [Display(Name = "SAPPPMES_BUDAT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime BUDAT { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 140)]
        [Display(Name = "SAPPPMES_EPFMG", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string EPFMG { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 150)]
        [Display(Name = "SAPPPMES_ERFME", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ERFME { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 50)]
        [Display(Name = "SAPPPMES_ZComnum", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZComnum { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 60)]
        [Display(Name = "SAPPPMES_LMNGA_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LMNGA_H { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 60)]
        [Display(Name = "SAPPPMES_LGORT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LGORT { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 70)]
        [Display(Name = "SAPPPMES_ISM", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ISM { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 160)]
        [Display(Name = "SAPPPMES_ZMESGUID", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESGUID { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 170)]
        [Display(Name = "SAPPPMES_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime ZCSRQSJ { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 200)]
        [Display(Name = "SAPPPMES_Status", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public Int32 Status { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 180)]
        [Display(Name = "SAPPPMES_BatchNo", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string BatchNo { get; set; }
        [Export(ExportName = "SAPPPMES0006", ExportSeq = 190)]
        [Display(Name = "SAPPPMES_UniqueCode", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string UniqueCode { get; set; }
        
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
            SAPPPMES0006 another = obj as SAPPPMES0006;

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
