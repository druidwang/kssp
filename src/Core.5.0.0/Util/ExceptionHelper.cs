using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Utility
{
    public class ExceptionHelper
    {
        public static string GetExceptionMessage(Exception ex)
        {
            string message = @"LEVEL1:" + ex.Message;
            if (ex.InnerException != null)
            {
                message += @"
LEVEL2:" + ex.InnerException.Message;
                if (ex.InnerException.InnerException != null)
                {
                    message += @"
LEVEL3:" + ex.InnerException.InnerException.Message;
                    if (ex.InnerException.InnerException.InnerException != null)
                    {
                        message += @"
LEVEL4:" + ex.InnerException.InnerException.InnerException.Message;
                    }
                }
            }
            int length = message.Length > 4000 ? 4000 : message.Length;
            message.Substring(0, length);
            return message.Substring(0, length);
        }
    }
}
