using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;


namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPCustomer : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 10)]
        [Display(Name = "SAPCustomer_KUNNR", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string KUNNR { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 20)]
        [Display(Name = "SAPCustomer_NAME1", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string NAME1 { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq =30)]
        [Display(Name = "SAPCustomer_BUKRS", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string BUKRS { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 40)]
        [Display(Name = "SAPCustomer_COUNTRY", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string COUNTRY { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 50)]
        [Display(Name = "SAPCustomer_CITY", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string CITY { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 60)]
        [Display(Name = "SAPCustomer_REMARK", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string REMARK { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 70)]
        [Display(Name = "SAPCustomer_TELF1", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string TELF1 { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 80)]
        [Display(Name = "SAPCustomer_TELFX", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string TELFX { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 90)]
        [Display(Name = "SAPCustomer_PARNR", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string PARNR { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 100)]
        [Display(Name = "SAPCustomer_PSTLZ", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string PSTLZ { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 110)]
        [Display(Name = "SAPCustomer_TELBX", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string TELBX { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 120)]
        [Display(Name = "SAPCustomer_TELF2", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string TELF2 { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 130)]
        [Display(Name = "SAPCustomer_LOEVM", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string LOEVM { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 140)]
        [Display(Name = "SAPCustomer_BatchNo", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 150)]
        [Display(Name = "SAPCustomer_Status", ResourceType = typeof(Resources.SI.SAPCustomer))]
        public Int32? Status { get; set; }
        [Export(ExportName = "SAPCustomer", ExportSeq = 160)]
        [Display(Name = "SAPCustomer_CreateDate", ResourceType = typeof(Resources.SI.SAPCustomer))]
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
            SAPCustomer another = obj as SAPCustomer;

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
