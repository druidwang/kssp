using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using com.Sconit.Service.Impl;
using com.Sconit.Service;

namespace com.Sconit.Web.Installer
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromAssemblyNamed("com.Sconit.Service")
                .Pick().If(t => t.Name.EndsWith("MgrImpl")
                    && !t.Name.Equals("GenericMgrImpl")
                    && !t.Name.Equals("QueryImpl")
                    && !t.Name.Equals("EmailMgrImpl")
                    && !t.Name.Equals("PubSubMgrImpl")
                    && !t.Name.Equals("SecurityMgrImpl"))
                .Configure(c => c.LifeStyle.Singleton)
                .WithService.DefaultInterface()
                );

            //container.Register(AllTypes.FromAssemblyNamed("com.Sconit.Service.MRP")
            //    .Pick().If(t => t.Name.EndsWith("MgrImpl"))
            //    .Configure(c => c.LifeStyle.Singleton)
            //    .WithService.DefaultInterface()
            //    );

            container.Register(AllTypes.FromAssemblyNamed("com.Sconit.Service.SI")
              .Pick().If(t => t.Name.EndsWith("MgrImpl"))
              .Configure(c => c.LifeStyle.Singleton)
              .WithService.DefaultInterface()
              );
        }
    }
}