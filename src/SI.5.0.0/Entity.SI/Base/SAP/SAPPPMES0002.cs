using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPPPMES0002 : EntityBase
    {
        #region O/R Mapping Properties

		public Int32 Id { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 10)]
        [Display(Name = "SAPPPMES_ZMESSC", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESSC { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 20)]
        [Display(Name = "SAPPPMES_ZMESLN", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESLN { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 30)]
        [Display(Name = "SAPPPMES_ZPTYPE", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZPTYPE { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 40)]
        [Display(Name = "SAPPPMES_ZComnum", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZComnum { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 50)]
        [Display(Name = "SAPPPMES_ZMESGUID", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string ZMESGUID { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 60)]
        [Display(Name = "SAPPPMES_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime ZCSRQSJ { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 90)]
        [Display(Name = "SAPPPMES_Status", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public Int32 Status { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 70)]
        [Display(Name = "SAPPPMES_BatchNo", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string BatchNo { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 80)]
        [Display(Name = "SAPPPMES_UniqueCode", ResourceType = typeof(Resources.SI.SAPPPMES))]
		public string UniqueCode { get; set; }
        public string OrderType { get; set; }
        [Export(ExportName = "SAPPPMES0002", ExportSeq = 90)]
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
            SAPPPMES0002 another = obj as SAPPPMES0002;

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
