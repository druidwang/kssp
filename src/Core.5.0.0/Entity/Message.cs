using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity
{
    public class Message
    {
        public CodeMaster.MessageType MessageType { get; set; }
        private string messageKey { get; set; }
        private string[] messageParams { get; set; }

        public Message(CodeMaster.MessageType messageType, string messageKey, params string[] messageParams)
        {
            this.MessageType = messageType;
            this.messageKey = messageKey;
            this.messageParams = messageParams;
        }

        public string GetMessageString()
        {
            try
            {
                return messageParams == null || messageParams.Count() == 0 ? messageKey : string.Format(messageKey, messageParams);
            }
            catch (System.Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
