using System.Web.Services;
using com.Sconit.Service;
using com.Sconit.Entity;
using System.Collections.Generic;

namespace com.Sconit.WebService
{
    [WebService(Namespace = "http://com.Sconit.WebService.OrderService/")]
    public class OrderService : BaseWebService
    {
        private IOrderMgr orderManager
        {
            get
            {
                return GetService<IOrderMgr>();
            }
        }
     
        [WebMethod]
        public void CreatSequenceOrder(string userCode)
        {
            SecurityContextHolder.Set(securityMgr.GetUser(userCode));
            orderManager.CreatSequenceOrder();
        }

        [WebMethod]
        public void DeferredFeed(string flow, string userCode)
        {
            SecurityContextHolder.Set(securityMgr.GetUser(userCode));
            orderManager.DeferredFeed(flow);
        }    

        [WebMethod]
        public void AutoCloseOrder(string userCode)
        {
            SecurityContextHolder.Set(securityMgr.GetUser(userCode));
            orderManager.AutoCloseOrder();
        }
    }
}
