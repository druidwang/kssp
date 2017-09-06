using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPBom : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 10)]
        [Display(Name = "SAPBom_MATNR", ResourceType = typeof(Resources.SI.SAPBom))]
        public string MATNR { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 20)]
        [Display(Name = "SAPBom_MAKTX", ResourceType = typeof(Resources.SI.SAPBom))]
        public string MAKTX { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 30)]
        [Display(Name = "SAPBom_BMENG", ResourceType = typeof(Resources.SI.SAPBom))]
        public string BMENG { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 40)]
        [Display(Name = "SAPBom_BMEIN", ResourceType = typeof(Resources.SI.SAPBom))]
        public string BMEIN { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 50)]
        [Display(Name = "SAPBom_IDNRK", ResourceType = typeof(Resources.SI.SAPBom))]
        public string IDNRK { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 60)]
        [Display(Name = "SAPBom_MEINS", ResourceType = typeof(Resources.SI.SAPBom))]
        public string MEINS { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 70)]
        [Display(Name = "SAPBom_MENGE", ResourceType = typeof(Resources.SI.SAPBom))]
        public string MENGE { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 80)]
        [Display(Name = "SAPBom_AUSCH", ResourceType = typeof(Resources.SI.SAPBom))]
        public string AUSCH { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 90)]
        [Display(Name = "SAPBom_BatchNo", ResourceType = typeof(Resources.SI.SAPBom))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 100)]
        [Display(Name = "SAPBom_Status", ResourceType = typeof(Resources.SI.SAPBom))]
        public Int32? Status { get; set; }
        [Export(ExportName = "SAPBom", ExportSeq = 110)]
        [Display(Name = "SAPBom_CreateDate", ResourceType = typeof(Resources.SI.SAPBom))]
        public DateTime CreateDate { get; set; }

        public string DATUV { get; set; }

        public string VORNR { get; set; }
        
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
            SAPBom another = obj as SAPBom;

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
