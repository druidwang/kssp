using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace com.Sconit.Entity.Exception
{
    public class BusinessException : ApplicationException
    {
        //private IList<Message> Messages { get; set; }
        private ConcurrentBag<Message> Messages { get; set; }

        public Boolean HasMessage
        {
            get
            {
                return Messages != null && Messages.Count > 0;
            }
        }

        public IList<Message> GetMessages()
        {
            if (Messages == null)
            {
                Messages = new ConcurrentBag<Message>();
            }
            return Messages.ToList();
        }

        public void AddMessage(Message message)
        {
            if (Messages == null)
            {
                Messages = new ConcurrentBag<Message>();
            }
            if (Messages.Count(p => p.GetMessageString() == message.GetMessageString()) == 0)
            {
                Messages.Add(message);
            }
        }

        public void AddMessage(string message)
        {
            if (Messages == null)
            {
                Messages = new ConcurrentBag<Message>();
            }
            AddMessage(new Message(CodeMaster.MessageType.Error, message, null));
        }

        public void AddMessage(string message, params string[] messageParams)
        {
            if (Messages == null)
            {
                Messages = new ConcurrentBag<Message>();
            }
            Message newMessage = new Message(CodeMaster.MessageType.Error, message, messageParams);
            AddMessage(newMessage);
        }

        public BusinessException()
            : base()
        {
        }

        public BusinessException(string message)
            : base(message)
        {
            AddMessage(new Message(CodeMaster.MessageType.Error, message, null));
        }

        public BusinessException(string message, params string[] messageParams)
            : base(message)
        {
            AddMessage(new Message(CodeMaster.MessageType.Error, message, messageParams));
        }

        public BusinessException(string message, params object[] messageParams)
            : base(message)
        {
            List<string> strParams = new List<string>();
            if (messageParams != null)
            {
                foreach (var messageParam in messageParams)
                {
                    strParams.Add(messageParam.ToString());
                }
            }
            AddMessage(new Message(CodeMaster.MessageType.Error, message, strParams.ToArray()));
        }
    }
}
