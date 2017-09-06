using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class LocationLotDetailSearchModel : SearchModelBase
    {

        public string itemFrom { get; set; }
        public string itemTo { get; set; }
        public string ManufactureParty { get; set; }
        public Boolean IsFreeze { get; set; }
        public string plantFrom { get; set; }
        public string plantTo { get; set; }
        public string locationFrom { get; set; }
        public string locationTo { get; set; }
        public string regionFrom { get; set; }
        public string regionTo { get; set; }
        public string Level { get; set; }
        public string Location { get; set; }
        public string Item { get; set; }
        public string LotNo { get; set; }
        public string TheFactory { get; set; }
        public string TheFactoryTo { get; set; }
        public bool hideSupper { get; set; }
        public bool hideLotNo { get; set; }
        public string HuId { get; set; }

        public string TypeLocation { get; set; }
        public string SAPLocation { get; set; }

        public string LotNoFrom { get; set; }
        public string LotNoTo { get; set; }
        public bool IsConsignment { get; set; }
        public string Region { get; set; }
        public string Direction { get; set; }
        public bool IsATP { get; set; }
        public bool IsOnlyShowQtyInv { get; set; }
        public bool IsExport { get; set; }
        public bool IsSumByItem { get; set; }
        public bool IsIncludeNoNeedAging { get; set; }
        public Int16? OccupyType { get; set; }
        public string Bin { get; set; }
        public string GetType { get; set; }
        public bool IsIncludeEmptyStock { get; set; }
        public DateTime ManufactureDate { get; set; }
        public int SearchCondition { get; set; }
        public Int16? HuOption { get; set; }
        public Int16? QualityType { get; set; }
        public string HuOptionHuLot { get; set; }
        public Int16? IsATP2 { get; set; }
        public Int16? IsFreeze2 { get; set; }
        public Int16? IsConsignment2 { get; set; }
        public string ProductType { get; set; }
        public string Party { get; set; }
    }
}