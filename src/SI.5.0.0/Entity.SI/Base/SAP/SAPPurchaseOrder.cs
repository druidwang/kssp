using System;
using System.Xml.Serialization;

namespace com.Sconit.Entity.SAP
{
    [Serializable]
    public partial class SAPPurchaseOrder : EntityBase
    {
        #region O/R Mapping Properties
        [XmlIgnore]
		public Int32 Id { get; set; }
		public string PONo { get; set; }
		public string Type { get; set; }
		public string POLineNo { get; set; }
		public string Item { get; set; }
		public string ItemVersion { get; set; }
		public string Qty { get; set; }
		public string ConsumeQty { get; set; }
        public string EffectiveDate { get; set; }
		public string Supplier { get; set; }
		public string SAPLocTo { get; set; }
		public string CKDNo { get; set; }
		public string CKDLineNo { get; set; }
		public string IsActive { get; set; }
		public string IsCompleted { get; set; }
        public string LotNo { get; set; }

        public string IsGenLesOrder { get; set; }

        public string SapCreateUser { get; set; }

        public string PhoneNo { get; set; }
        public string PRNo { get; set; }
        [XmlIgnore]
        public Int16 Status { get; set; }
        [XmlIgnore]
        public string BatchNo { get; set; }
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
            SAPPurchaseOrder another = obj as SAPPurchaseOrder;

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
