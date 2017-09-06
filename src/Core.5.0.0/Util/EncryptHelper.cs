using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace com.Sconit.Utility
{
    public class EncryptHelper
    {
        public static string Md5(string str) 
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(str);
            bs = x.ComputeHash(bs);
            StringBuilder s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2"));
            }
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return s.ToString();
        }
    }
}
