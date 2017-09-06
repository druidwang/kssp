using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPTransferLog : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Display(Name = "SAPTransferLog_BatchNo", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public string BatchNo { get; set; }
        [Display(Name = "SAPTransferLog_SysCode", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public string SysCode { get; set; }
        [Display(Name = "SAPTransferLog_Interface", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public string Interface { get; set; }
        [Display(Name = "SAPTransferLog_Status", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public Int32? Status { get; set; }
        [Display(Name = "SAPTransferLog_ErrorMsg", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public string ErrorMsg { get; set; }
        [Display(Name = "SAPTransferLog_RowCount", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public int RowCounts { get; set; }
        [Display(Name = "SAPTransferLog_TransDate", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public DateTime? TransDate { get; set; }
        [Display(Name = "SAPTransferLog_TransStartDate", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public DateTime? TransStartDate { get; set; }
        [Display(Name = "SAPTransferLog_DataFromDate", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public DateTime? DataFromDate { get; set; }
        [Display(Name = "SAPTransferLog_DataToDate", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public DateTime? DataToDate { get; set; }
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
            SAPTransferLog another = obj as SAPTransferLog;

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
