using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using com.Sconit.Web.Pluming;

namespace com.Sconit.Web.Installer
{
    public class MvcComponentsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container
                .Register(
                    Component.For<ControllerContextHost>().LifeStyle.PerWebRequest,
                    Component.For<RequestContextHost>().LifeStyle.PerWebRequest,

                    Component.For<RequestContext>()
                        .UsingFactoryMethod(k => k.Resolve<RequestContextHost>().GetContext()),

                    Component.For<HttpContextBase>()
                        .UsingFactoryMethod(k => new HttpContextWrapper(HttpContext.Current)),

                    Component.For<HttpSessionStateBase>()
                        .UsingFactoryMethod(k => k.Resolve<HttpContextBase>().Session),

                    Component.For<HttpRequestBase>()
                        .UsingFactoryMethod(k => k.Resolve<HttpContextBase>().Request),

                    Component.For<Func<ControllerContext>>()
                        .UsingFactoryMethod<Func<ControllerContext>>(k => k.Resolve<ControllerContextHost>().GetContext),

                    Component.For<ITempDataProvider>()
                        .ImplementedBy<SessionStateTempDataProvider>().
                        LifeStyle.Transient,

                    Component.For<UrlHelper>().LifeStyle.PerWebRequest,
                    Component.For<HtmlHelper>().LifeStyle.PerWebRequest
                );
        }
    }
}