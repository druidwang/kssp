using System.Net.Mail;
using System.Web.Services;
using com.Sconit.Service;

namespace com.Sconit.WebService
{
    [WebService(Namespace = "http://com.Sconit.WebService.EmailService/")]
    public class EmailService : BaseWebService
    {
        private IEmailMgr emailManager
        {
            get
            {
                return GetService<IEmailMgr>();
            }
        }
     
        [WebMethod]
        public void Send(string subject, string body, string mailTo, MailPriority mailPriority)
        {
            emailManager.SendEmail(subject, body, mailTo, mailPriority);
        }

        [WebMethod]
        public void AsyncSend(string subject, string body, string mailTo, MailPriority mailPriority)
        {
            emailManager.AsyncSendEmail(subject, body, mailTo, mailPriority);
        }
    }
}
