using System;

namespace com.Sconit.Entity.SI.SD_ACC
{
    [Serializable]
    public class Permission 
    {
        #region O/R Mapping Properties	
        public Int32 UserId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionCategory { get; set; }
        public com.Sconit.CodeMaster.PermissionCategoryType PermissionCategoryType { get; set; } 
        #endregion
    }


}
