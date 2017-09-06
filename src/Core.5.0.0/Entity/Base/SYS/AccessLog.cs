using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SYS
{
    [Serializable]
    public partial class AccessLog
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string PageUrl { get; set; }
        public string PageName { get; set; }
        public string CsIP { get; set; }
        public string CsBrowser { get; set; }
        public string UserAgent { get; set; }

        #endregion
    }

}
