using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class MiscOrderLocationDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public Int32 MiscOrderDetailId { get; set; }
        public Int32 MiscOrderDetailSequence { get; set; }
		public string MiscOrderNo { get; set; }
		public string Item { get; set; }
        public string Uom { get; set; }
		public string HuId { get; set; }
		public string LotNo { get; set; }
		public Boolean IsCreatePlanBill { get; set; }
		public Boolean IsConsignment { get; set; }
        public string ConsignmentSupplier { get; set; }
		public Int32? PlanBill { get; set; }
		public Int32? ActingBill { get; set; }
		public CodeMaster.QualityType QualityType { get; set; }
		public Boolean IsFreeze { get; set; }
		public Boolean IsATP { get; set; }
        public CodeMaster.OccupyType OccupyType { get; set; }
		public string OccupyReferenceNo { get; set; }
		public Decimal Qty { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }

        
        
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
            MiscOrderLocationDetail another = obj as MiscOrderLocationDetail;

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
