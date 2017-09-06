using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.MES
{
    public class MES_Interface_CreateHu
    {

        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string LotNo { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string ManufactureDate { get; set; }
        public string Manufacturer { get; set; }
        public string OrderNo { get; set; }
        public string Uom { get; set; }
        public decimal UC { get; set; }
        public decimal Qty { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string Printer { get; set; }
        public string HuId { get; set; }
        public Int32 Status { get; set; }
        public string Message { get; set; }
		
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
            MES_Interface_CreateHu another = obj as MES_Interface_CreateHu;

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
