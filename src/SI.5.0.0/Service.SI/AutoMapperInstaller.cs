using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LeanEngine.Entity;

namespace com.Sconit.Service.SI
{
    public class AutoMapperInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            Mapper.CreateMap<ItemFlow, ItemFlow>()
                    .ForMember(d => d.DemandSources, o => o.Ignore())
                    .ForMember(d => d.OrderTracers, o => o.Ignore())
                    .ForMember(d => d.OrderQtyList, o => o.Ignore());

            Mapper.CreateMap<Flow, Flow>();

            Mapper.CreateMap<FlowStrategy, FlowStrategy>();

            Mapper.CreateMap<Bom, Bom>();

            Mapper.CreateMap<Item, Item>();

            Mapper.CreateMap<com.Sconit.Entity.ORD.OrderMaster, com.Sconit.Entity.SI.SD_ORD.OrderMaster>();
            Mapper.CreateMap<com.Sconit.Entity.ORD.OrderDetail, com.Sconit.Entity.SI.SD_ORD.OrderDetail>()
                .ForMember(d => d.RemainReceivedQty, o => o.MapFrom(s => s.OrderedQty - s.ReceivedQty))
                 .ForMember(d => d.CurrentQty, o => o.MapFrom(s => s.OrderedQty - s.ReceivedQty));
            Mapper.CreateMap<com.Sconit.Entity.ORD.IpMaster, com.Sconit.Entity.SI.SD_ORD.IpMaster>();
            Mapper.CreateMap<com.Sconit.Entity.ORD.IpDetail, com.Sconit.Entity.SI.SD_ORD.IpDetail>()
                .ForMember(d => d.RemainReceivedQty, o => o.MapFrom(s => s.Qty - s.ReceivedQty))
                .ForMember(d => d.CurrentQty, o => o.MapFrom(s => s.Qty - s.ReceivedQty));

            Mapper.CreateMap<com.Sconit.Entity.ORD.PickListMaster, com.Sconit.Entity.SI.SD_ORD.PickListMaster>();
            Mapper.CreateMap<com.Sconit.Entity.ORD.PickListDetail, com.Sconit.Entity.SI.SD_ORD.PickListDetail>()
                .ForMember(d => d.CurrentQty, o => o.MapFrom(s => s.Qty - s.PickedQty));
            Mapper.CreateMap<com.Sconit.Entity.INP.InspectMaster, com.Sconit.Entity.SI.SD_ORD.InspectMaster>();
            Mapper.CreateMap<com.Sconit.Entity.INP.InspectDetail, com.Sconit.Entity.SI.SD_ORD.InspectDetail>();
            Mapper.CreateMap<com.Sconit.Entity.ORD.SequenceMaster, com.Sconit.Entity.SI.SD_ORD.SequenceMaster>();
            Mapper.CreateMap<com.Sconit.Entity.ORD.SequenceDetail, com.Sconit.Entity.SI.SD_ORD.SequenceDetail>();
            Mapper.CreateMap<com.Sconit.Entity.ORD.MiscOrderMaster, com.Sconit.Entity.SI.SD_ORD.MiscOrderMaster>();
            Mapper.CreateMap<com.Sconit.Entity.ORD.MiscOrderDetail, com.Sconit.Entity.SI.SD_ORD.MiscOrderDetail>();
            //Inv
            Mapper.CreateMap<com.Sconit.Entity.VIEW.HuStatus, com.Sconit.Entity.SI.SD_INV.Hu>();
            Mapper.CreateMap<com.Sconit.Entity.INV.Hu, com.Sconit.Entity.SI.SD_INV.Hu>();
            Mapper.CreateMap<com.Sconit.Entity.INV.StockTakeMaster, com.Sconit.Entity.SI.SD_INV.StockTakeMaster>();
            Mapper.CreateMap<com.Sconit.Entity.INV.KanBanCard, com.Sconit.Entity.SI.SD_ORD.AnDonInput>();
            Mapper.CreateMap<com.Sconit.Entity.INV.ContainerDetail, com.Sconit.Entity.SI.SD_INV.ContainerDetail>();
            Mapper.CreateMap<com.Sconit.Entity.INV.ContainerHu, com.Sconit.Entity.SI.SD_INV.ContainerHu>();

            //ACC
            Mapper.CreateMap<com.Sconit.Entity.ACC.User, com.Sconit.Entity.SI.SD_ACC.User>()
            .ForMember(d => d.BarCodeTypes, s => s.Ignore());

            Mapper.CreateMap<com.Sconit.Entity.VIEW.UserPermissionView, com.Sconit.Entity.SI.SD_ACC.Permission>();

            //MD
            Mapper.CreateMap<com.Sconit.Entity.MD.Location, com.Sconit.Entity.SI.SD_MD.Location>();
            Mapper.CreateMap<com.Sconit.Entity.MD.LocationBin, com.Sconit.Entity.SI.SD_MD.Bin>();
            Mapper.CreateMap<com.Sconit.Entity.MD.Item, com.Sconit.Entity.SI.SD_MD.Item>();
            Mapper.CreateMap<com.Sconit.Entity.MD.Pallet, com.Sconit.Entity.SI.SD_MD.Pallet>();

            //SCM
            Mapper.CreateMap<com.Sconit.Entity.SCM.FlowMaster, com.Sconit.Entity.SI.SD_SCM.FlowMaster>();
            Mapper.CreateMap<com.Sconit.Entity.SCM.FlowDetail, com.Sconit.Entity.SI.SD_SCM.FlowDetail>()
                    .ForMember(d => d.FlowDetailInputs, o => o.Ignore())
                    .ForMember(d => d.CurrentQty, o => o.Ignore());

            //WSM
            Mapper.CreateMap<com.Sconit.Entity.WMS.PickTask, com.Sconit.Entity.SI.SD_WMS.PickTask>()
                .ForMember(d => d.CurrentQty, o => o.MapFrom(s => s.OrderQty - s.PickQty)); ;
            Mapper.CreateMap<com.Sconit.Entity.WMS.DeliveryBarCode, com.Sconit.Entity.SI.SD_WMS.DeliverBarCode>();
            Mapper.CreateMap<com.Sconit.Entity.TMS.TransportOrderMaster, com.Sconit.Entity.SI.SD_TMS.TransportOrderMaster>();
            Mapper.CreateMap<com.Sconit.Entity.TMS.TransportOrderDetail, com.Sconit.Entity.SI.SD_TMS.TransportOrderDetail>();
        }
    }
}
