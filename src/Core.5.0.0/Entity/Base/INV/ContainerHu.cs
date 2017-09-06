using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class ContainerHu : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
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
        public com.Sconit.CodeMaster.InventoryType ContainerType { get; set; }
		public Decimal ContainerQty { get; set; }
        public string Container { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ContainerHu another = obj as ContainerHu;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Id == another.Id);
            }
        } 
    }
	
}
