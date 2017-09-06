using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace SP.WebServiceTests.Installers
{
    public class SDServiceInstaller : IWindsorInstaller
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