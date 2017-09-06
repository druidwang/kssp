using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using com.Sconit.Entity.ACC;
using System;
using System.Reflection;
using System.Linq;

namespace com.Sconit.Utility
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultiButtonAttribute : ActionNameSelectorAttribute
    {
        public string ButtonMethod { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            var keys = controllerContext.HttpContext.Request.Params["ButtonMethod"];
            if (keys != null)
            {
                string key = keys.ToString();
                return key.Equals(ButtonMethod, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return false;
            }
        }

    }
}
