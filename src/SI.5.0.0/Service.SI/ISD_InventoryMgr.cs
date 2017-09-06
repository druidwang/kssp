namespace com.Sconit.Service.SI
{
    using System;
    using System.Collections.Generic;
    using com.Sconit.Entity.SI.SD_INV;

    public interface ISD_InventoryMgr
    {
        Hu GetHu(string huId);

        Hu CloneHu(string huId, decimal qty);
        
        Entity.SI.SD_INV.StockTakeMaster GetStockTake(string stNo);

        void DoStockTake(string stNo, string[][] stockTakeDetails);

        void DoPutAway(string huId, string binCode);

        void DoPickUp(string huId);

        void DoPack(List<string> huIdList, string location, DateTime? effDate);

        void DoUnPack(List<string> huIdList, DateTime? effDate);

        void DoRePack(List<string> oldHuList, List<string> newHuList, DateTime? effDate);

        void InventoryUnFreeze(IList<string> huIdList);

        void InventoryFreeze(IList<string> huIdList);

        Hu GetDistHu(string huId);

        Hu ResolveHu(string extHuId);

        com.Sconit.Entity.SI.SD_INV.ContainerDetail GetContainerDetail(string containerId);

        List<com.Sconit.Entity.SI.SD_INV.Hu> GetContainerHu(string containerId);

        bool ContainerBind(string containerId, string huId);

        bool ContainerUnBind(string containerId, string huId);

        bool OnBin(string binCode, List<string> huIds);

        bool OffBin(List<string> huIds);

        bool IsHuInContainer(string huId);

        List<com.Sconit.Entity.SI.SD_INV.Hu> GetPalletHu(string palletCode);

        bool IsHuInPallet(string huId);


        bool PalletBind(string palletCode, string huId);

        bool PalletUnBind(string palletCode, string huId);
    }
}
