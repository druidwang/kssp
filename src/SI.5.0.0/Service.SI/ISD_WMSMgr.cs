namespace com.Sconit.Service.SI
{
    using System;
    using System.Collections.Generic;
    using com.Sconit.Entity.SI.SD_WMS;

    public interface ISD_WMSMgr
    {
        List<PickTask> GetPickTaskByUser(int pickUserId, bool isPickByHus);

        void DoPickTask(List<Entity.SI.SD_INV.Hu> huList);

        Entity.SI.SD_INV.Hu GetPickHu(string huId);

        Entity.SI.SD_INV.Hu GetDeliverMatchHu(string huId);

        Entity.SI.SD_WMS.DeliverBarCode GetDeliverBarCode(string barCode);

        void MatchDCToHU(string huId, string barCode);

        void TransferToDock(List<string> huIds, string dock);

        Entity.SI.SD_INV.Hu GetShipHu(string huId, string deliverBarCode);
    }
}
