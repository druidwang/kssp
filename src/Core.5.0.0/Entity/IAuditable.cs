using System;

namespace com.Sconit.Entity
{
    public interface IAuditable
    {
        Int32 CreateUserId { get; set; }
        string CreateUserName { get; set; }
        DateTime CreateDate { get; set; }
        Int32 LastModifyUserId { get; set; }
        string LastModifyUserName { get; set; }
        DateTime LastModifyDate { get; set; }
    }
}
