using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SI.SD_INV
{
    [Serializable]
    public class ContainerHu
    {
        public Int32 Id { get; set; }
        public string ContainerId { get; set; }
        public string HuId { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LotNo { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string RefItemCode { get; set; }
        public string Uom { get; set; }
        public string BaseUom { get; set; }
        public Decimal UnitCount { get; set; }
        public Decimal Qty { get; set; }
        public Decimal UnitQty { get; set; }
        public DateTime ManufactureDate { get; set; }
        public string ManufactureParty { get; set; }
        public Boolean IsOdd { get; set; }
        public string SupplierLotNo { get; set; }
        public string ContainerDesc { get; set; }
        public Int16 ContainerType { get; set; }
        public Decimal ContainerQty { get; set; }
        public string Container { get; set; }
        
    }
	
}
