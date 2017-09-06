using System.Web.Mvc;
using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using com.Sconit.Entity.SYS;
using com.Sconit.Web.Models;

/// <summary>
/// Summary description for AutoMapperInstaller
/// </summary>
namespace com.Sconit.Web.Installer
{
    public class AutoMapperInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            Mapper.CreateMap<Menu, MenuModel>();

            Mapper.CreateMap<CodeDetail, SelectListItem>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Value))
                .ForMember(d => d.Text, opt => opt.MapFrom(s => s.Description));


        }
    }
}