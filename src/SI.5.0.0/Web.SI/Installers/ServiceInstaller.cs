using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using com.Sconit.Service;
using com.Sconit.Service.Impl;
using com.Sconit.Persistence;

namespace com.Sconit.Web.SI.Installer
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
                    && !t.Name.Equals("PubSubMgrImpl"))
                .Configure(c => c.LifeStyle.Singleton)
                .WithService.DefaultInterface()
                );
        }
    }
}