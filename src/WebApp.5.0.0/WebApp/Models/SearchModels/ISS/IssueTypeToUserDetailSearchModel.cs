using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ISS
{
    public class IssueTypeToUserDetailSearchModel : SearchModelBase
    {
        public string UserCode { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public Boolean HasEmail { get; set; }
        public Boolean HasMobilePhone { get; set; }
    }
}