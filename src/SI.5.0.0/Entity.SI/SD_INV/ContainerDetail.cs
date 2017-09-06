using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SI.SD_INV
{
    [Serializable]
    public class ContainerDetail
    {
        public string ContainerId { get; set; }
        public string Container { get; set; }
        public string Location { get; set; }
        public Boolean IsEmpty { get; set; }
        public string ContainerDescription { get; set; }
        public com.Sconit.CodeMaster.InventoryType ContainerType { get; set; }
        public Decimal ContainerQty { get; set; }
        public DateTime ActiveDate { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }
    }
	
}
