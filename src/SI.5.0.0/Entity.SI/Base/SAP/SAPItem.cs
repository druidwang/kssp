using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPItem : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 10)]
        [Display(Name = "SAPItem_MATNR", ResourceType = typeof(Resources.SI.SAPItem))]
        public string MATNR { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 20)]
        [Display(Name = "SAPItem_BISMT", ResourceType = typeof(Resources.SI.SAPItem))]
        public string BISMT { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 30)]
        [Display(Name = "SAPItem_MAKTX", ResourceType = typeof(Resources.SI.SAPItem))]
        public string MAKTX { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 40)]
        [Display(Name = "SAPItem_MEINS", ResourceType = typeof(Resources.SI.SAPItem))]
        public string MEINS { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 50)]
        [Display(Name = "SAPItem_WERKS", ResourceType = typeof(Resources.SI.SAPItem))]
        public string WERKS { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 60)]
        [Display(Name = "SAPItem_MTART", ResourceType = typeof(Resources.SI.SAPItem))]
        public string MTART { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 70)]
        [Display(Name = "SAPItem_MTBEZ", ResourceType = typeof(Resources.SI.SAPItem))]
        public string MTBEZ { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 80)]
        [Display(Name = "SAPItem_MATKL", ResourceType = typeof(Resources.SI.SAPItem))]
        public string MATKL { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 90)]
        [Display(Name = "SAPItem_WGBEZ", ResourceType = typeof(Resources.SI.SAPItem))]
        public string WGBEZ { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 100)]
        [Display(Name = "SAPItem_SPART", ResourceType = typeof(Resources.SI.SAPItem))]
        public string SPART { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 110)]
        [Display(Name = "SAPItem_LVORM", ResourceType = typeof(Resources.SI.SAPItem))]
        public string LVORM { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 120)]
        [Display(Name = "SAPItem_SOBSL", ResourceType = typeof(Resources.SI.SAPItem))]
        public string SOBSL { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 130)]
        [Display(Name = "SAPItem_BatchNo", ResourceType = typeof(Resources.SI.SAPItem))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 140)]
        [Display(Name = "SAPItem_Status", ResourceType = typeof(Resources.SI.SAPItem))]
        public int Status { get; set; }
        [Export(ExportName = "SAPItem", ExportSeq = 150)]
        [Display(Name = "SAPItem_CreateDate", ResourceType = typeof(Resources.SI.SAPItem))]
        public DateTime CreateDate { get; set; }
        
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
            SAPItem another = obj as SAPItem;

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
