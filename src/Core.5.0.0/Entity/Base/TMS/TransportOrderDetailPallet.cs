using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportOrderDetailPallet : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        public string IpNo { get; set; }
        public Int32 TransportOrderDetailId { get; set; }
		public string PalletCode { get; set; }
		public Int32 PalletQty { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public Int32 Version { get; set; }
        
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
            TransportOrderDetailPallet another = obj as TransportOrderDetailPallet;

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
