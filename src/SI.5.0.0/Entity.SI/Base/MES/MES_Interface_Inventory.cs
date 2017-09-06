using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.MES
{
    public class MES_Interface_Inventory
    {

        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        public string RequestId { get; set; }
		public string MaterialCode { get; set; }
		public string WarehouseCode { get; set; }
		public string FactoryCode { get; set; }
        public string Uom { get; set; }
		public decimal Quantity { get; set; }
		public string BarCode { get; set; }
		public string BatchNo { get; set; }
        public string Reservoir { get; set; }
		public int Type { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 Status { get; set; }
        
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
            MES_Interface_Inventory another = obj as MES_Interface_Inventory;

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
