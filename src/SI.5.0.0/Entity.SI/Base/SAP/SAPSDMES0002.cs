using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPSDMES0002 : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Export(ExportName = "SAPSDCancel", ExportSeq = 10)]
        [Display(Name = "SAPSDNormal_ZMESGUID", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string ZMESGUID { get; set; }
        [Export(ExportName = "SAPSDCancel", ExportSeq = 20)]
        [Display(Name = "SAPSDNormal_ZMESSO", ResourceType = typeof(Resources.SI.SAPSDNormal))]
		public string ZMESSO { get; set; }
        [Export(ExportName = "SAPSDCancel", ExportSeq = 190)]
        [Display(Name = "SAPSDNormal_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPSDNormal))]
        public DateTime ZCSRQSJ { get; set; }
        [Export(ExportName = "SAPSDCancel", ExportSeq = 200)]
        [Display(Name = "SAPSDNormal_Status", ResourceType = typeof(Resources.SI.SAPSDNormal))]
        public Int32 Status { get; set; }
        [Export(ExportName = "SAPSDCancel", ExportSeq = 210)]
        [Display(Name = "SAPSDNormal_BatchNo", ResourceType = typeof(Resources.SI.SAPSDNormal))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPSDCancel", ExportSeq = 220)]
        [Display(Name = "SAPSDNormal_UniqueCode", ResourceType = typeof(Resources.SI.SAPSDNormal))]
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
            SAPSDMES0002 another = obj as SAPSDMES0002;

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
