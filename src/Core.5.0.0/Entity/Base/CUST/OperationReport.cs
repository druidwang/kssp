using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class OperationReport : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public string OrderNo { get; set; }
        public string VAN { get; set; }
        public Int32 Operation { get; set; }
        public string WorkCenter { get; set; }
        public Boolean IsReport { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
        public string ExternalOrderNo { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
            if (OrderNo != null && Operation != 0 && WorkCenter != null)
            {
                return OrderNo.GetHashCode() ^ Operation.GetHashCode() ^ WorkCenter.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            OperationReport another = obj as OperationReport;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.OrderNo == another.OrderNo) && (this.Operation == another.Operation) && (this.WorkCenter == another.WorkCenter);
            }
        } 
    }
	
}
