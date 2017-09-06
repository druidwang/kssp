using System.Net.Mail;
using System.Web.Services;
using com.Sconit.Service;

namespace com.Sconit.WebService
{
    [WebService(Namespace = "http://com.Sconit.WebService.SMSService/")]
    public class SMSService : BaseWebService
    {
        [WebMethod]
        public void AsyncSend(string mobilePhones, string msg)
        {
            //smsManager.AsyncSendMessage(mobilePhones, msg);
        }

        [WebMethod]
        public void Send(string mobilePhones, string msg)
        {
            //smsManager.SendMessage(mobilePhones, msg);
        }

    }
}
