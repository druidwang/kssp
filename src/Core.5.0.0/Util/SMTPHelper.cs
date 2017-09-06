

namespace com.Sconit.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.Mail;

    public static class SMTPHelper
    {
        public static string SendSMTPEMail(string Subject, string Body, string MailFrom, string MailTo, string SmtpServer, string MailFromPasswd, string ReplyTo)
        {
            return SendSMTPEMail(Subject, Body, MailFrom, MailTo, SmtpServer, MailFromPasswd, ReplyTo, MailPriority.Normal);
        }
        public static string SendSMTPEMail(string Subject, string Body, string MailFrom, string MailTo, string SmtpServer, string MailFromPasswd, string ReplyTo, MailPriority Priority)
        {
            MailMessage message = null;
            try
            {
                message = new MailMessage();
                SmtpClient client = new SmtpClient(SmtpServer);
                foreach (string mailTo in MailTo.Split(';'))
                {
                    foreach (string mailto in mailTo.Split(','))
                    {
                        if (ControlHelper.IsValidEmail(mailto))
                        {
                            message.To.Add(new MailAddress(mailto));
                        }
                    }
                }
                
                message.Priority = Priority;//优先级
                message.Subject = Subject;
                message.Body = Body;

                string address = string.Empty;
                string displayName = string.Empty;
                string[] mailfrom = ReplyTo.Split(new char[] { ',', '，' });
                displayName = mailfrom[0];
                if (mailfrom.Length > 1)
                {
                    address = mailfrom[1];
                }
                else
                {
                    address = mailfrom[0];
                }

                message.From = new MailAddress(address, displayName, Encoding.UTF8);//address, displayName, Encoding.GetEncoding("GB2312")
                //message.ReplyTo = new MailAddress(address, displayName, Encoding.UTF8);
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(MailFrom, MailFromPasswd);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment("D:\\logs\\" + filePath);
                // message.Attachments.Add(attachment);

                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = System.Text.Encoding.UTF8;//正文编码
                message.IsBodyHtml = true;//设置为HTML格式     
                client.Send(message);

                // message.Dispose();
                // logger.Error(Subject);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (message != null)
                {
                    message.Dispose();
                }
            }
        }
    }
}
