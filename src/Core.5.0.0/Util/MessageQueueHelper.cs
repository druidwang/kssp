using System.Collections.Generic;
using System.Messaging;

namespace com.Sconit.Utility
{
    public class MessageQueueHelper
    {
        /**/
        /// <summary>
        /// 通过Create方法创建使用指定路径的新消息队列
        /// </summary>
        /// <param name="queuePath"></param>
        public static void CreateQueue(string queuePath)
        {
            if (!MessageQueue.Exists(queuePath))
            {
                MessageQueue.Create(queuePath);
            }
        }

        /**/
        /// <summary>
        /// 删除指定队列
        /// </summary>
        public static void DeleteQueue(string queuePath)
        {
            if (MessageQueue.Exists(queuePath))
            {
                MessageQueue.Delete(queuePath);
            }
        }

        /**/
        /// <summary>
        /// 连接消息队列并发送消息到队列
        /// </summary>
        public static void SendMessage<T>(string queuePath, T Message)
        {
            //连接到本地的队列
            MessageQueue queue = new MessageQueue(queuePath);

            Message message = new Message();
            message.Body = Message;
            message.Formatter = new BinaryMessageFormatter();
            //发送消息到队列中
            queue.Send(message);
        }

        /**/
        /// <summary>
        /// 连接消息队列并从队列中接收消息
        /// </summary>
        public static T ReceiveMessage<T>(string queuePath)
        {
            //连接到本地队列
            MessageQueue queue = new MessageQueue(queuePath);
            queue.Formatter = new BinaryMessageFormatter();

            //从队列中接收消息
            Message message = queue.Receive();
            if (message != null)
            {
                T context = (T)message.Body; //获取消息的内容
                return context;
            }

            return default(T);
        }

        /**/
        /// <summary>
        /// 清空指定队列的消息
        /// </summary>
        public static void ClearMessage(string queuePath)
        {
            MessageQueue myQueue = new MessageQueue(queuePath);
            myQueue.Purge();
        }

        /**/
        /// <summary>
        /// 连接队列并获取队列的全部消息
        /// </summary>
        public static IList<T> GetAllMessage<T>(string queuePath)
        {
            //连接到本地队列
            MessageQueue queue = new MessageQueue(queuePath);
            Message[] messages = queue.GetAllMessages();
            BinaryMessageFormatter formatter = new BinaryMessageFormatter();

            if (messages != null && messages.Length > 0)
            {
                IList<T> contents = new List<T>();
                foreach (Message message in messages)
                {
                    message.Formatter = formatter;
                    contents.Add((T)message.Body);
                }

                return contents;
            }

            return default(IList<T>);
        }       
    }
}
