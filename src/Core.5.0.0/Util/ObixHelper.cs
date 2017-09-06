using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.Collections;
using System.Xml;
using System.Web;
using System.Net;
using System.IO;



namespace com.Sconit.Utility
{
    public static class ObixHelper
    {

    
/// <summary>
        /// 通过 WebRequest/WebResponse 类访问远程地址并返回结果，需要Basic认证；
        /// 调用端自己处理异常
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="timeout">访问超时时间，单位毫秒；如果不设置超时时间，传入0</param>
        /// <param name="encoding">如果不知道具体的编码，传入null</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static XmlElement Request_WebRequest(string uriStr)
        {
          
            Encoding encoding = Encoding.UTF8;
            string username = "admin";
            string password = "Wgs19831024";
            string baseUri = "http://localhost/obix/config/Yanke/";
            string uri = baseUri + uriStr;
            WebRequest request = WebRequest.Create(new Uri(uri));

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                request.Credentials = GetCredentialCache(uri, username, password);
                request.Headers.Add("Authorization", GetAuthorization(username, password));
            }

          
                request.Timeout = 3000;

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = encoding == null ? new StreamReader(stream) : new StreamReader(stream, encoding);

       
            XmlDocument doc = new XmlDocument();
            doc.Load(sr);

            XmlElement rootXml =  doc.DocumentElement;
   

            sr.Close();
            stream.Close();
            return rootXml;
    }


        public static void Response_WebRequest(string uriStr, string postData)
        {

            Encoding encoding = Encoding.UTF8;
            string username = "admin";
            string password = "Wgs19831024";
            string baseUri = "http://localhost/obix/config/Yanke/";
            string uri = baseUri + uriStr;
            WebRequest request = WebRequest.Create(new Uri(uri));
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                request.Credentials = GetCredentialCache(uri, username, password);
                request.Headers.Add("Authorization", GetAuthorization(username, password));
            }
            request.Method = "POST";
            
          
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
           
            request.ContentType = "application/x-www-form-urlencoded";
        
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
         
            dataStream.Write(byteArray, 0, byteArray.Length);
        
            dataStream.Close();
          
     
          
        }

        #region # 生成 Http Basic 访问凭证 #

        private static CredentialCache GetCredentialCache(string uri, string username, string password)
        {
            string authorization = string.Format("{0}:{1}", username, password);

            CredentialCache credCache = new CredentialCache();
            credCache.Add(new Uri(uri), "Basic", new NetworkCredential(username, password));

            return credCache;
        }

        private static string GetAuthorization(string username, string password)
        {
            string authorization = string.Format("{0}:{1}", username, password);

            return "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(authorization));
        }

        #endregion



    }
}
