using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPUomConvertion : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Export(ExportName = "SAPUomConvertion", ExportSeq = 10)]
        [Display(Name = "SAPUomConvertion_MATNR", ResourceType = typeof(Resources.SI.SAPUomConvertion))]
        public string MATNR { get; set; }
        [Export(ExportName = "SAPUomConvertion", ExportSeq = 20)]
        [Display(Name = "SAPUomConvertion_MEINS", ResourceType = typeof(Resources.SI.SAPUomConvertion))]
        public string MEINS { get; set; }
        [Export(ExportName = "SAPUomConvertion", ExportSeq = 30)]
        [Display(Name = "SAPUomConvertion_MEINH", ResourceType = typeof(Resources.SI.SAPUomConvertion))]
        public string MEINH { get; set; }
        [Export(ExportName = "SAPUomConvertion", ExportSeq = 40)]
        [Display(Name = "SAPUomConvertion_UMREZ", ResourceType = typeof(Resources.SI.SAPUomConvertion))]
        public string UMREZ { get; set; }
        [Export(ExportName = "SAPUomConvertion", ExportSeq = 50)]
        [Display(Name = "SAPUomConvertion_UMREN", ResourceType = typeof(Resources.SI.SAPUomConvertion))]
        public string UMREN { get; set; }
        [Export(ExportName = "SAPUomConvertion", ExportSeq = 60)]
        [Display(Name = "SAPUomConvertion_BatchNo", ResourceType = typeof(Resources.SI.SAPUomConvertion))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPUomConvertion", ExportSeq = 70)]
        [Display(Name = "SAPUomConvertion_Status", ResourceType = typeof(Resources.SI.SAPUomConvertion))]
        public Int32? Status { get; set; }
        [Export(ExportName = "SAPUomConvertion", ExportSeq = 80)]
        [Display(Name = "SAPUomConvertion_CreateDate", ResourceType = typeof(Resources.SI.SAPUomConvertion))]
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
            SAPUomConvertion another = obj as SAPUomConvertion;

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
