using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPPriceList : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 10)]
        [Display(Name = "SAPPriceList_LIFNR", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string LIFNR { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 20)]
        [Display(Name = "SAPPriceList_WAERS", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string WAERS { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 30)]
        [Display(Name = "SAPPriceList_MATNR", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string MATNR { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 40)]
        [Display(Name = "SAPPriceList_BPRME", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string BPRME { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 50)]
        [Display(Name = "SAPPriceList_NETPR", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string NETPR { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 60)]
        [Display(Name = "SAPPriceList_MWSKZ", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string MWSKZ { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 70)]
        [Display(Name = "SAPPriceList_ERDAT", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string ERDAT { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 80)]
        [Display(Name = "SAPPriceList_PRDAT", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string PRDAT { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 90)]
        [Display(Name = "SAPPriceList_NORMB", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string NORMB { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 100)]
        [Display(Name = "SAPPriceList_BatchNo", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 110)]
        [Display(Name = "SAPPriceList_Status", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public Int32? Status { get; set; }
        [Export(ExportName = "SAPPriceList", ExportSeq = 120)]
        [Display(Name = "SAPPriceList_CreateDate", ResourceType = typeof(Resources.SI.SAPPriceList))]
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
            SAPPriceList another = obj as SAPPriceList;

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
