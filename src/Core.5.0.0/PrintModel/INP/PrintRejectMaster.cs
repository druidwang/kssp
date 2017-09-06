using System;

namespace com.Sconit.PrintModel.INP
{
    [Serializable]
    public partial class PrintRejectMaster : PrintBase
    {
        #region O/R Mapping Properties

		public string RejectNo { get; set; }
        public string Region { get; set; }
		public string ReferenceNo { get; set; }
		public Boolean IsPrint { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
		public Int32 Version { get; set; }

        #endregion

    }
	
}
