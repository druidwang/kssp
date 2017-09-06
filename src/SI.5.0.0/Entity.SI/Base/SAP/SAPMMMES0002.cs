using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPMMMES0002 : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Display(Name = "SAPMMMES_ZMESPO", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0002", ExportSeq = 10)]
		public string ZMESPO { get; set; }
        [Display(Name = "SAPMMMES_ZMESGUID", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0002", ExportSeq = 20)]
        public string ZMESGUID { get; set; }
        [Display(Name = "SAPMMMES_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0002", ExportSeq = 30)]
        public DateTime ZCSRQSJ { get; set; }
        [Display(Name = "SAPMMMES_Status", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0002", ExportSeq = 40)]
        public Int32 Status { get; set; }
        [Display(Name = "SAPMMMES_BatchNo", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0002", ExportSeq = 50)]
        public string BatchNo { get; set; }
        [Display(Name = "SAPMMMES_UniqueCode", ResourceType = typeof(Resources.SI.SAPMMMES))]
        [Export(ExportName = "SAPMMMES0002", ExportSeq = 60)]
		public string UniqueCode { get; set; }
        [Export(ExportName = "SAPMMMES0002", ExportSeq = 90)]
        [Display(Name = "SAPPPMES_CancelDate", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime CancelDate { get; set; }
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
            SAPMMMES0002 another = obj as SAPMMMES0002;

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
