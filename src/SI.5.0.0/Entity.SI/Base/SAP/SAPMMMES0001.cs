using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPMMMES0001 : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Display(Name = "SAPMMMES_ZMESPO", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 10)]
		public string ZMESPO { get; set; }
        [Display(Name = "SAPMMMES_LIFNR", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 20)]
		public string LIFNR { get; set; }
        [Display(Name = "SAPMMMES_BSART", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 30)]
		public string BSART { get; set; }
        [Display(Name = "SAPMMMES_EKORG", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 40)]
		public string EKORG { get; set; }
        [Display(Name = "SAPMMMES_EKGRP", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 50)]
		public string EKGRP { get; set; }
        [Display(Name = "SAPMMMES_BUKRS", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 60)]
		public string BUKRS { get; set; }
        [Display(Name = "SAPMMMES_BWART_H", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 70)]
		public string BWART_H { get; set; }
        [Display(Name = "SAPMMMES_LFSNR", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 80)]
        public string LFSNR { get; set; }
        [Display(Name = "SAPMMMES_BUDAT", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 90)]
		public DateTime BUDAT { get; set; }
        [Display(Name = "SAPMMMES_BLDAT", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 100)]
		public DateTime BLDAT { get; set; }
        [Display(Name = "SAPMMMES_EBELP_I", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 110)]
		public string EBELP_I { get; set; }
        [Display(Name = "SAPMMMES_EPSTP", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 120)]
		public string EPSTP { get; set; }
        [Display(Name = "SAPMMMES_MATNR", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 130)]
		public string MATNR { get; set; }
        [Display(Name = "SAPMMMES_TARGET_QTY_I", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 140)]
		public string TARGET_QTY_I { get; set; }
        [Display(Name = "SAPMMMES_NETPR", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 150)]
		public string NETPR { get; set; }
        [Display(Name = "SAPMMMES_PEINH", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 160)]
		public string PEINH { get; set; }
        [Display(Name = "SAPMMMES_WAERS", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 170)]
		public string WAERS { get; set; }
        [Display(Name = "SAPMMMES_BPRME_I", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 180)]
		public string BPRME_I { get; set; }
        [Display(Name = "SAPMMMES_EINDT", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 190)]
		public DateTime EINDT { get; set; }
        [Display(Name = "SAPMMMES_WERKS", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 200)]
		public string WERKS { get; set; }
        [Display(Name = "SAPMMMES_LGORT", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 210)]
		public string LGORT { get; set; }
        [Display(Name = "SAPMMMES_RETPO", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 220)]
		public string RETPO { get; set; }
        [Display(Name = "SAPMMMES_BWART_C", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 230)]
		public string BWART_C { get; set; }
        [Display(Name = "SAPMMMES_MATNR_C", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 240)]
		public string MATNR_C { get; set; }
        [Display(Name = "SAPMMMES_TARGET_QTY_C", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 250)]
		public string TARGET_QTY_C { get; set; }
        [Display(Name = "SAPMMMES_BPRME_C", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 260)]
		public string BPRME_C { get; set; }
        [Display(Name = "SAPMMMES_ZMESGUID", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 270)]
		public string ZMESGUID { get; set; }
        [Display(Name = "SAPMMMES_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 280)]
		public DateTime ZCSRQSJ { get; set; }
        [Display(Name = "SAPMMMES_Status", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 290)]
		public Int32 Status { get; set; }
        [Display(Name = "SAPMMMES_BatchNo", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 300)]
		public string BatchNo { get; set; }
        [Display(Name = "SAPMMMES_UniqueCode", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0001", ExportSeq = 310)]
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
            SAPMMMES0001 another = obj as SAPMMMES0001;

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
