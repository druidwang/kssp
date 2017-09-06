namespace com.Sconit.Service.SI
{
    using System.Collections.Generic;

    public interface ISD_SecurityMgr
    {
        Entity.SI.SD_ACC.User GetUser(string userCode, string hashedPassword, string ipAddress);

        Entity.ACC.User GetBaseUser(string userCode, bool withPermissions = false);

        void CreateAccessLog(Entity.SYS.AccessLog accesslog);
    }
}
