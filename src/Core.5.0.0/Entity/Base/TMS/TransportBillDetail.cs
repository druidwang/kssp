using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportBillDetail : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id{get;set;}

        public DateTime EffectiveDate { get; set; }
        
        public Int32 Version{get;set;}
       
		public string BillNo{get;set;}
		
		public string OrderNo{get;set;}
		
		public string Flow{get;set;}
		
        public string FlowDescription{get;set;}
       
		public Decimal Discount{get;set;}
		
		public Boolean IsIncludeTax{get;set;}
		
		public string TaxCode{get;set;}
		
        public Decimal BillAmount{get;set;}
        
		public string Currency{get;set;}
		
		public string FreightNo{get;set;}
		
		public string PricingMethod{get;set;}

        public int ActBill { get; set; }

        public Decimal BillQty { get; set; }

        #endregion

		public override int GetHashCode()
        {
			if (Id != null)
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
            TransportBillDetail another = obj as TransportBillDetail;

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
