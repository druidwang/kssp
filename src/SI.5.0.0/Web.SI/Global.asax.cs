using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Facilities.AutoTx;
using Castle.Windsor;
using Castle.Windsor.Installer;
using com.Sconit.Service;
using com.Sconit.Web.Filters;
using com.Sconit.Web.Pluming;
using com.Sconit.Service.Impl;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class Global : HttpApplication, IContainerAccessor
    {
        private static IWindsorContainer container;

        public IWindsorContainer Container
        {
            get { return container; }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            BootstrapContainer();
            ViewEngines.Engines.Add(new SconitRazorViewEngine());
            ViewEngines.Engines.Add(new SconitWebFormViewEngine());
        }

        protected void Application_End()
        {
            container.Dispose();
        }

        private void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionFilter());
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //   "CustomRoute",
            //   "{controller}/{Action}/{page}/{orderBy}/{filter}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        private void BootstrapContainer()
        {
            container = new WindsorContainer();
            container.AddFacility("transactionmanagement", new TransactionFacility());
            container.Install(Configuration.FromAppConfig());
            container.Install(FromAssembly.Named("com.Sconit.Persistence"));
            container.Install(FromAssembly.Named("com.Sconit.Service"));
            container.Install(FromAssembly.Named("com.Sconit.Service.SAP"));
            container.Install(FromAssembly.Named("com.Sconit.Service.SD"));
            container.Install(FromAssembly.Named("com.Sconit.Service.EDI"));
            container.Install(FromAssembly.Named("com.Sconit.Service.SI"));
            container.Install(FromAssembly.Named("com.Sconit.Service.MRP"));
            container.Install(FromAssembly.This());
            var controllerFactory = new WindsorControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            //OrderMgrImpl orderMgr = (OrderMgrImpl)container.Resolve<IOrderMgr>();

            //#region 先暂时这样，以后可以用Castle.Facilities.EventWiring
            //orderMgr.OrderReleased += new OrderMgrImpl.OrderReleasedHandler(OnOrderReleased);
            //#endregion
        }

        //public void OnOrderReleased(OrderMaster orderMaster)
        //{
        //}
    }
}