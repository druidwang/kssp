using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.SI.MES;
using com.Sconit.Utility;

namespace com.Sconit.Service.SI.MES
{
    public interface IMESServicesMgr
    {
        MESCreateResponse CreateHu(string CustomerCode, string CustomerName, string LotNo, string Item, string ItemDesc, string ManufactureDate, string Manufacturer, string OrderNo, string Uom, decimal UC, decimal Qty, string CreateUser, string CreateDate, string Printer, string HuId);

        MESCreateResponse CreatePallet(List<string> BoxNos, string BoxCount, string Printer, string CreateUser, string CreateDate, string PalletId);

        InventoryResponse GetInventory(InventoryRequest request);

        List<ErrorMessage> GenBusinessOrderData(DateTime curDate);
        List<ErrorMessage> TransBusinessOrderData();
    }
}	