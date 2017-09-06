using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;


namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPSupplier : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 10)]
        [Display(Name = "SAPSupplier_LIFNR", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string LIFNR { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 20)]
        [Display(Name = "SAPSupplier_NAME1", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string NAME1 { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 30)]
        [Display(Name = "SAPSupplier_BUKRS", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string BUKRS { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 40)]
        [Display(Name = "SAPSupplier_COUNTRY", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string COUNTRY { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq =50)]
        [Display(Name = "SAPSupplier_CITY", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string CITY { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 60)]
        [Display(Name = "SAPSupplier_REMARK", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string REMARK { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 70)]
        [Display(Name = "SAPSupplier_TELF1", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string TELF1 { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 80)]
        [Display(Name = "SAPSupplier_TELFX", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string TELFX { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 90)]
        [Display(Name = "SAPSupplier_PARNR", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string PARNR { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 100)]
        [Display(Name = "SAPSupplier_PSTLZ", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string PSTLZ { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 110)]
        [Display(Name = "SAPSupplier_TELBX", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string TELBX { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 120)]
        [Display(Name = "SAPSupplier_TELF2", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string TELF2 { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 130)]
        [Display(Name = "SAPSupplier_EKGRP", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string EKGRP { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 140)]
        [Display(Name = "SAPSupplier_LOEVM", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string LOEVM { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 150)]
        [Display(Name = "SAPSupplier_BatchNo", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 160)]
        [Display(Name = "SAPSupplier_Status", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public Int32? Status { get; set; }
        [Export(ExportName = "SAPSupplier", ExportSeq = 170)]
        [Display(Name = "SAPSupplier_CreateDate", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public DateTime? CreateDate { get; set; }
        
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
            SAPSupplier another = obj as SAPSupplier;

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
