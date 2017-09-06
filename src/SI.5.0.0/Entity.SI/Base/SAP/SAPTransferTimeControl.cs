using System;


namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPTransferTimeControl : EntityBase
    {
        #region O/R Mapping Properties
		
		public string SysCode { get; set; }
		public DateTime? LastTransDate { get; set; }
		public DateTime? CurrTransDate { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (SysCode != null)
            {
                return SysCode.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            SAPTransferTimeControl another = obj as SAPTransferTimeControl;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.SysCode == another.SysCode);
            }
        } 
    }
	
}
