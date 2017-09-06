using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using System.Web;

namespace com.Sconit.Utility
{

    /// <summary>
    /// Summary description for ServiceLocator
    /// </summary>
    public class ServiceLocator
    {
        public ServiceLocator()
        {
        }

        //public static Object GetService(string serviceNm)
        //{
        //    return ObtainContainer().Resolve(serviceNm);
        //}

        public static T GetService<T>()
        {
            return ObtainContainer().Resolve<T>();
        }

        public static T GetService<T>(string serviceNm)
        {
            return ObtainContainer().Resolve<T>(serviceNm);
        }

        /// <summary>
        /// get an instance of windsor container from HTTP application
        /// </summary>
        /// <returns>return IWindsorContainer</returns>
        private static IWindsorContainer ObtainContainer()
        {

            IContainerAccessor containerAccessor =
                HttpContext.Current.ApplicationInstance as IContainerAccessor;

            if (containerAccessor == null)
            {
                throw new ApplicationException("HttpApplication doesn't implement the interface IContainerAccessor...");
            }

            IWindsorContainer container = containerAccessor.Container;
            if (container == null)
            {
                throw new ApplicationException("Can not find windsor container from HttpApplication...");
            }

            return container;
        }
    }
}
