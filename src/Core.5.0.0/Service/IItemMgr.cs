using System;
using System.Collections.Generic;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.BIL;
using System.IO;

namespace com.Sconit.Service
{
    public interface IItemMgr
    {
        Dictionary<string, Item> GetCacheAllItem();

        Item GetCacheItem(string itemCode);

        void ResetItemCache();

        IList<ItemKit> GetKitItemChildren(string kitItem);

        IList<ItemKit> GetKitItemChildren(string kitItem, bool includeInActive);

        decimal ConvertItemUomQty(string itemCode, string sourceUomCode, decimal sourceQty, string targetUomCode);

        PriceListDetail GetItemPrice(string itemCode, string uom, string priceList, string currency, DateTime? effectiveDate);

        void CreateItem(Item item);

        void UpdateItem(Item item);

        IList<ItemDiscontinue> GetItemDiscontinues(string itemCode, DateTime effectiveDate);

        IList<ItemDiscontinue> GetParentItemDiscontinues(string itemCode, DateTime effectiveDate);

        IList<Item> GetItems(IList<string> itemCodeList);

        void CreateCustodian(Custodian custodian);

        void DeleteItem(string itemCode);

        IList<Uom> GetItemUoms(string itemCode);

        Dictionary<string, Item> GetRefItemCode(string flowCode, List<string> refItemCodeList);

        void ImportItem(Stream inputStream);
    }
}
