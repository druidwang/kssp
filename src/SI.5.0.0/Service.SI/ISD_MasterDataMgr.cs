using System;
namespace com.Sconit.Service.SI
{
    public interface ISD_MasterDataMgr
    {
        Entity.SI.SD_MD.Bin GetBin(string binCode);

        Entity.SI.SD_MD.Location GetLocation(string locationCode);

        Entity.SI.SD_MD.Item GetItem(string itemCode);

        DateTime GetEffDate(string date);

        string GetEntityPreference(Entity.SYS.EntityPreference.CodeEnum entityEnum);

        Entity.SI.SD_MD.Pallet GetPallet(string palletCode);
    }
}
