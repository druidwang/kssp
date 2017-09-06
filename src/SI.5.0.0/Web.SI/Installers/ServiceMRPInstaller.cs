using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace com.Sconit.Web.SI.Installer
{
    public class ServiceMRPInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromAssemblyNamed("com.Sconit.Service.MRP")
              .Pick().If(t => t.Name.EndsWith("MgrImpl"))
              .Configure(c => c.LifeStyle.Singleton)
              .WithService.DefaultInterface()
              );
        }
    }
}