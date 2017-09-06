using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace com.Sconit.Web.SI.Installer
{
    public class ServiceSDInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromAssemblyNamed("com.Sconit.Service.SD")
              .Pick().If(t => t.Name.EndsWith("MgrImpl"))
              .Configure(c => c.LifeStyle.Singleton)
              .WithService.DefaultInterface()
              );
        }
    }
}