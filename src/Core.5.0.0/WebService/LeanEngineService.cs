namespace com.Sconit.WebService
{
    using System.Web.Services;
    using com.Sconit.Entity;
    using com.Sconit.Service.SI;

    [WebService(Namespace = "http://com.Sconit.WebService.SI.LeanEngineService/")]
    public class LeanEngineService : BaseWebService
    {
        #region public properties
        private ILeanEngineMgr leanEngineMgr { get { return GetService<ILeanEngineMgr>(); } }
        #endregion

        [WebMethod]
        public void RunLeanEngine(string userCode)
        {
            SecurityContextHolder.Set(securityMgr.GetUser(userCode));

            leanEngineMgr.RunLeanEngine();
        }

        [WebMethod]
        public void RunJIT_EX(string userCode)
        {
            SecurityContextHolder.Set(securityMgr.GetUser(userCode));

            leanEngineMgr.RunJIT_EX();
        }


    }
}
