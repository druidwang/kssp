using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace com.Sconit.Entity
{
    public class MessageHolder
    {
        private static ThreadLocal<IList<Message>> messages = new ThreadLocal<IList<Message>>();

        public static IList<Message> GetAll()
        {
            return messages.Value;
        }

        public static void CleanMessage()
        {
            messages.Value = new List<Message>();
        }

        public static void AddMessage(Message message)
        {
            DoAddMessage(message);
        }

        public static void AddErrorMessage(string messageKey)
        {
            Message message = new Message(CodeMaster.MessageType.Error, messageKey, null);
            DoAddMessage(message);
        }

        public static void AddErrorMessage(string messageKey, params string[] messageParams)
        {
            Message message = new Message(CodeMaster.MessageType.Error, messageKey, messageParams);
            DoAddMessage(message);
        }

        public static void AddInfoMessage(string messageKey)
        {
            Message message = new Message(CodeMaster.MessageType.Info, messageKey, null);
            DoAddMessage(message);
        }

        public static void AddInfoMessage(string messageKey, params string[] messageParams)
        {
            Message message = new Message(CodeMaster.MessageType.Info, messageKey, messageParams);
            DoAddMessage(message);
        }

        public static void AddWarningMessage(string messageKey)
        {
            Message message = new Message(CodeMaster.MessageType.Warning, messageKey, null);
            DoAddMessage(message);
        }

        public static void AddWarningMessage(string messageKey, params string[] messageParams)
        {
            Message message = new Message(CodeMaster.MessageType.Warning, messageKey, messageParams);
            DoAddMessage(message);
        }

        public static bool HasInfoMessages()
        {
            if (messages.Value != null)
            {
                return GetInfoMessages().Count > 0;
            }
            else
            {
                return false;
            }
        }

        public static bool HasWarningMessages()
        {
            if (messages.Value != null)
            {
                return GetWarningMessages().Count > 0;
            }
            else
            {
                return false;
            }
        }

        public static bool HasErrorMessages()
        {
            if (messages.Value != null)
            {
                return GetErrorMessages().Count > 0;
            }
            else
            {
                return false;
            }
        }

        public static IList<Message> GetInfoMessages()
        {
            if (messages.Value != null)
            {
                return messages.Value.Where(m => m.MessageType == CodeMaster.MessageType.Info).ToList();
            }
            else
            {
                return null;
            }
        }

        public static IList<Message> GetWarningMessages()
        {
            if (messages.Value != null)
            {
                return messages.Value.Where(m => m.MessageType == CodeMaster.MessageType.Warning).ToList();
            }
            else
            {
                return null;
            }
        }

        public static IList<Message> GetErrorMessages()
        {
            if (messages.Value != null)
            {
                return messages.Value.Where(m => m.MessageType == CodeMaster.MessageType.Error).ToList();
            }
            else
            {
                return null;
            }
        }

        private static void DoAddMessage(Message message)
        {
            if (messages.Value == null)
            {
                messages.Value = new List<Message>();
            }
            messages.Value.Add(message);
        }
    }
}
