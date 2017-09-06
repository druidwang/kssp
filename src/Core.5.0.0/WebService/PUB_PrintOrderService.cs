namespace com.Sconit.WebService
{
    using System.Web.Services;
    using com.Sconit.Entity;
    using com.Sconit.Service;
    using System.Collections.Generic;
    using com.Sconit.Entity.CUST;
    using System.Linq;

    [WebService(Namespace = "http://com.Sconit.WebService.SI.PUB_PrintOrderService/")]
    public class PUB_PrintOrderService : BaseWebService
    {
        #region public properties
        private ICustomizationMgr customizationMgr { get { return GetService<ICustomizationMgr>(); } }
        #endregion

        [WebMethod]
        public List<PubPrintOrder> GetPubPrintOrderList(string clientCode)
        {
            SecurityContextHolder.Set(systemMgr.GetMonitorUser());
            return customizationMgr.GetPubPrintOrderList(clientCode).ToList();
        }
    }
}
