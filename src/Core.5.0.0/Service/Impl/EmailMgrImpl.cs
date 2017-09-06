using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using com.Sconit.Entity.SYS;
using com.Sconit.Service.ews;
using com.Sconit.Utility;

namespace com.Sconit.Service.Impl
{
    public class EmailMgrImpl : BaseMgr, IEmailMgr
    {
        public ISystemMgr systemMgr { get; set; }
        public string userName { get; set; }
        public string userPassword { get; set; }
        public string domainName { get; set; }

        public void SendEmail(string subject, string body, string mailTo)
        {
            SendEmail(subject, body, mailTo, string.Empty, MailPriority.Normal);
        }

        public void SendEmail(string subject, string body, string mailTo, string replayTo)
        {
            SendEmail(subject, body, mailTo, replayTo, MailPriority.Normal);
        }

        public void SendEmail(string subject, string body, string mailTo, MailPriority mailPriority)
        {
            SendEmail(subject, body, mailTo, string.Empty, mailPriority);
        }

        public void SendEmail(string subject, string body, string mailTo, string replayTo, MailPriority mailPriority)
        {
            //SendMailExchange(mailTo, subject, body, replayTo);

            string SMTPEmailHost = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.SMTPEmailHost);
            string SMTPEmailPasswd = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.SMTPEmailPasswd);
            string emailFrom = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.SMTPEmailAddr);

            SMTPHelper.SendSMTPEMail(subject, body, emailFrom, mailTo, SMTPEmailHost, SMTPEmailPasswd, (!string.IsNullOrWhiteSpace(replayTo) ? replayTo : emailFrom), mailPriority);

        }

        public void AsyncSendEmail(string subject, string body, string mailTo)
        {
            AsyncSendEmail(subject, body, mailTo, string.Empty, MailPriority.Normal);
        }

        public void AsyncSendEmail(string subject, string body, string mailTo, string replayTo)
        {
            AsyncSendEmail(subject, body, mailTo, replayTo, MailPriority.Normal);
        }

        public void AsyncSendEmail(string subject, string body, string mailTo, MailPriority mailPriority)
        {
            AsyncSendEmail(subject, body, mailTo, string.Empty, mailPriority);
        }

        public void AsyncSendEmail(string subject, string body, string mailTo, string replayTo, MailPriority mailPriority)
        {
            AsyncSend asyncSend = new AsyncSend(this.SendEmail);
            asyncSend.BeginInvoke(subject, body, mailTo, replayTo, mailPriority, null, null);
        }

        public delegate void AsyncSend(string subject, string body, string mailTo, string replayTo, MailPriority mailPriority);


        #region ## Send mail (Exchange)
        public bool SendMailExchange(string recipientEmail, string subjectName, string bodyVal, string replyTo)
        {
            string senderEmail = userName + "@4s.saic.com.cn";
            SetCertificatePolicy();
            ExchangeServiceBinding esb = new ExchangeServiceBinding();
            esb.Credentials = new NetworkCredential(userName, userPassword, domainName);
            esb.Url = "https://10.166.1.32/ews/exchange.asmx";

            CreateItemType createItemRequest = new CreateItemType();

            // Specifiy how the created items are handled
            createItemRequest.MessageDisposition = MessageDispositionType.SendAndSaveCopy;
            createItemRequest.MessageDispositionSpecified = true;

            // Specify the location of sent items. 
            createItemRequest.SavedItemFolderId = new TargetFolderIdType();
            DistinguishedFolderIdType sentitems = new DistinguishedFolderIdType();
            sentitems.Id = DistinguishedFolderIdNameType.sentitems;
            createItemRequest.SavedItemFolderId.Item = sentitems;

            // Create the array of items.
            createItemRequest.Items = new NonEmptyArrayOfAllItemsType();

            // Create a single e-mail message.
            MessageType message = new MessageType();
            message.Subject = subjectName;
            message.Body = new BodyType();
            message.Body.BodyType1 = BodyTypeType.HTML;
            message.Body.Value = bodyVal;
            message.ItemClass = "IPM.Note";
            message.Sender = new SingleRecipientType();
            message.Sender.Item = new EmailAddressType();
            message.Sender.Item.EmailAddress = senderEmail;
            message.ToRecipients = new EmailAddressType[1];
            message.ToRecipients[0] = new EmailAddressType();
            message.ToRecipients[0].EmailAddress = recipientEmail;
            if (!string.IsNullOrWhiteSpace(recipientEmail))
            {
                string[] val = recipientEmail.Split(',');
                int replyCount = val.Length;
                message.ToRecipients = new EmailAddressType[replyCount];
                for (int i = 0; i < replyCount; i++)
                {
                    message.ToRecipients[i] = new EmailAddressType();
                    message.ToRecipients[i].EmailAddress = val[i];
                }
            }

            message.Sensitivity = SensitivityChoicesType.Normal;
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                string[] val = replyTo.Split(',');
                int replyCount = val.Length;
                message.ReplyTo = new EmailAddressType[replyCount];
                for (int i = 0; i < replyCount; i++)
                {
                    message.ReplyTo[i] = new EmailAddressType();
                    message.ReplyTo[i].EmailAddress = val[i];
                }
            }

            // Add the message to the array of items to be created.
            createItemRequest.Items.Items = new ItemType[1];
            createItemRequest.Items.Items[0] = message;

            try
            {
                // Send the request to create and send the e-mail item, and get the response.
                CreateItemResponseType createItemResponse = esb.CreateItem(createItemRequest);

                // Determine whether the request was a success.
                if (createItemResponse.ResponseMessages.Items[0].ResponseClass == ResponseClassType.Error)
                {
                    //throw new Exception(createItemResponse.ResponseMessages.Items[0].MessageText);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion


        private void SetCertificatePolicy()
        {
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        }

        private bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }

    }
}
