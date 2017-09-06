using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace com.Sconit.SmartDevice
{
    public class BusinessException : ApplicationException
    {
        public string[] MessageParams { get; set; }

        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        public BusinessException(string message, params string[] messageParams)
            : this(string.Format(message,messageParams))
        {
            //this.MessageParams = messageParams;   
        }

        public BusinessException(string message, System.Exception inner, params string[] messageParams)
            : this(string.Format(message, messageParams), inner)
        {
            //this.MessageParams = messageParams;
        }
    }
}
