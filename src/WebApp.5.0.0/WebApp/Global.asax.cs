using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Facilities.AutoTx;
using Castle.Windsor;
using Castle.Windsor.Installer;
using com.Sconit.Service;
using com.Sconit.Service.SI;
using com.Sconit.Web.Filters;
using com.Sconit.Web.Pluming;
using com.Sconit.Utility;

namespace com.Sconit.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class Global : HttpApplication, IContainerAccessor
    {
        private static IWindsorContainer container;

        private Timer timer = new Timer();

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
            //com.Sconit.Web.Installer.AutoMapperInstaller.Install();
            //com.Sconit.Service.AutoMapperInstaller.Install();
            ViewEngines.Engines.Add(new SconitRazorViewEngine());
            ViewEngines.Engines.Add(new SconitWebFormViewEngine());

            timer.Enabled = true;
            timer.Interval = 1 * 30 * 1000;//执行间隔时间,单位为毫秒  
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(BatchJob_Elapsed);
        }

        protected void Application_End()
        {
            container.Dispose();
        }

        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionFilter());
        }

        private static void RegisterRoutes(RouteCollection routes)
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

        private static void BootstrapContainer()
        {
            container = new WindsorContainer();
            container.AddFacility("transactionmanagement", new TransactionFacility());
            container.Install(Configuration.FromAppConfig());
            container.Install(FromAssembly.Named("com.Sconit.Persistence"));
            container.Install(FromAssembly.Named("com.Sconit.Service"));
            //container.Install(FromAssembly.Named("com.Sconit.Service.MRP"));
            container.Install(FromAssembly.Named("com.Sconit.Service.SI"));
            container.Install(FromAssembly.This());
            var controllerFactory = new WindsorControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        private void BatchJob_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("BatchJobEnabled")))
            {
                IJobRunMgr jobRunMgr = Container.Resolve<IJobRunMgr>();
                jobRunMgr.RunBatchJobs();
            }
        }
    }
}