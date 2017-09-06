using System.Net.Mail;

namespace com.Sconit.Service
{
    public interface IEmailMgr
    {
        void SendEmail(string subject, string body, string mailTo);

        void SendEmail(string subject, string body, string mailTo, string replayTo);

        void SendEmail(string subject, string body, string mailTo, MailPriority mailPriority);

        void SendEmail(string subject, string body, string mailTo, string replayTo, MailPriority mailPriority);

        void AsyncSendEmail(string subject, string body, string mailTo);

        void AsyncSendEmail(string subject, string body, string mailTo, string replayTo);

        void AsyncSendEmail(string subject, string body, string mailTo, MailPriority mailPriority);

        void AsyncSendEmail(string subject, string body, string mailTo, string replayTo, MailPriority mailPriority);
    }
}
