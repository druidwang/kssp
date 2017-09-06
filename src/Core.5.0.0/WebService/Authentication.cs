using System.Web.Services.Protocols;

namespace com.Sconit.WebService
{
    public class Authentication : SoapHeader
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
