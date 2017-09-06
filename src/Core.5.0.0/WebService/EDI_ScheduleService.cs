namespace com.Sconit.WebService
{
    using System.Collections.Generic;
    using System.Web.Services;
    using System.Web.Services.Protocols;
    using com.Sconit.Entity;
    using System;
    using System.Xml.Serialization;
    using System.Collections;
    using com.Sconit.Service.SI;

    [WebService(Namespace = "http://com.Sconit.WebService.EDI.ScheduleService/")]
    public class EDI_ScheduleService : BaseWebService
    {
        #region public properties
        private IEDI_ScheduleMgr scheduleMgr { get { return GetService<IEDI_ScheduleMgr>(); } }

        #endregion

        [WebMethod]
        public void LoadEDI()
        {
            scheduleMgr.LoadEDI();
        }

        [WebMethod]
        public void EDI2Plan(string userCode)
        {
            SecurityContextHolder.Set(securityMgr.GetUser(userCode));
            scheduleMgr.EDI2Plan();
        }
    }
}
