using System;
using System.Web.Routing;

namespace com.Sconit.Web.Pluming
{
    public class RequestContextHost
    {
        private RequestContext context;
        public void SetContext(RequestContext requestContext)
        {
            if (context != null && context.Equals(requestContext))
            {
                return;
            }
            context = requestContext;
        }
        public RequestContext GetContext()
        {
            if (context == null)
            {
                throw new InvalidOperationException("Context was not set!");
            }
            return context;
        }
    }
}