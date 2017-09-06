using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPPPMES0001 : EntityBase
    {
        #region O/R Mapping Properties

		public Int32 Id { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 10)]
        [Display(Name = "SAPPPMES_ZMESSC", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESSC { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 20)]
        [Display(Name = "SAPPPMES_ZMESLN", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESLN { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 30)]
        [Display(Name = "SAPPPMES_ZPTYPE", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZPTYPE { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 40)]
        [Display(Name = "SAPPPMES_AUFART", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string AUFART { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 50)]
        [Display(Name = "SAPPPMES_WERKS", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string WERKS { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 60)]
        [Display(Name = "SAPPPMES_MATNR_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string MATNR_H { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 160)]
        [Display(Name = "SAPPPMES_GAMNG_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string GAMNG_H { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 70)]
        [Display(Name = "SAPPPMES_GMEIN_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string GMEIN_H { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 80)]
        [Display(Name = "SAPPPMES_GLTRP", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public DateTime GLTRP { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 90)]
        [Display(Name = "SAPPPMES_GSTRP", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime GSTRP { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 100)]
        [Display(Name = "SAPPPMES_BLDAT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime BLDAT { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 110)]
        [Display(Name = "SAPPPMES_BUDAT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime BUDAT { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 120)]
        [Display(Name = "SAPPPMES_LFSNR", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LFSNR { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 130)]
        [Display(Name = "SAPPPMES_BWART_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string BWART_H { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 140)]
        [Display(Name = "SAPPPMES_LGORT_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LGORT_H { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 150)]
        [Display(Name = "SAPPPMES_ERFMG_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ERFMG_H { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 160)]
        [Display(Name = "SAPPPMES_ZComnum", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZComnum { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 170)]
        [Display(Name = "SAPPPMES_LMNGA_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string LMNGA_H { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 180)]
        [Display(Name = "SAPPPMES_ISM", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ISM { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 190)]
        [Display(Name = "SAPPPMES_BWART_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string BWART_I { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 200)]
        [Display(Name = "SAPPPMES_MATNR_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string MATNR_I { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 210)]
        [Display(Name = "SAPPPMES_ERFMG_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ERFMG_I { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 220)]
        [Display(Name = "SAPPPMES_GMEIN_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string GMEIN_I { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 230)]
        [Display(Name = "SAPPPMES_LGORT_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string LGORT_I { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 240)]
        [Display(Name = "SAPPPMES_ZMESGUID", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESGUID { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 250)]
        [Display(Name = "SAPPPMES_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime ZCSRQSJ { get; set; }
        [Display(Name = "SAPPPMES_Status", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public Int32 Status { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 260)]
        [Display(Name = "SAPPPMES_BatchNo", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string BatchNo { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 270)]
		public string UniqueCode { get; set; }

        public string MTSNR { get; set; }
        public string OrderType { get; set; }
        [Export(ExportName = "SAPPPMES0001", ExportSeq = 270)]
        [Display(Name = "SAPPPMES_TailQty", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string TailQty { get; set; }
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
            SAPPPMES0001 another = obj as SAPPPMES0001;

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
