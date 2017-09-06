using System;
using System.Collections.Generic;

namespace com.Sconit.Entity.SI.SD_ACC
{
    [Serializable]
    public class User
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public TypeEnum Type { get; set; }
        public string Email { get; set; }
        public string TelPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Language { get; set; }
        public Boolean IsActive { get; set; }
        //public Boolean AccountLocked { get; set; }
        //public DateTime PasswordExpired { get; set; }
        //public DateTime AccountExpired { get; set; }
        //public bool IsAuthenticated { get; set; }

        //public Int32 CreateUserId { get; set; }
        //public string CreateUserName { get; set; }
        //public DateTime CreateDate { get; set; }
        //public Int32 LastModifyUserId { get; set; }
        //public string LastModifyUserName { get; set; }
        //public DateTime LastModifyDate { get; set; } 
        #endregion

        public List<Permission> Permissions { get; set; }

        public List<BarCodeType> BarCodeTypes { get; set; }

    }

    public class BarCodeType
    {
        public string PreFixed { get; set; }
        public OrdType Type { get; set; }
    }

    public enum OrdType
    {
        ORD,
        ASN,
        PIK,
        STT,
        INS,
        SEQ,
        MIS,
    }

}
