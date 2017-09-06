using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class HuMapping : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

		public Int32 Id { get; set; }
        [Display(Name = "HuMapping_HuId", ResourceType = typeof(Resources.INV.HuMapping))]
		public string HuId { get; set; }
        [Display(Name = "HuMapping_OldHus", ResourceType = typeof(Resources.INV.HuMapping))]
		public string OldHus { get; set; }
        [Display(Name = "HuMapping_Item", ResourceType = typeof(Resources.INV.HuMapping))]
        public string Item { get; set; }
        [Display(Name = "HuMapping_Qty", ResourceType = typeof(Resources.INV.HuMapping))]
        public decimal Qty { get; set; }
        [Display(Name = "HuMapping_IsEffective", ResourceType = typeof(Resources.INV.HuMapping))]
        public Boolean IsEffective { get; set; }
      // [Display(Name = "HuMapping_IsRepack", ResourceType = typeof(Resources.INV.HuMapping))]
        public Boolean IsRepack { get; set; }
        [Display(Name = "HuMapping_OrderNo", ResourceType = typeof(Resources.INV.HuMapping))]
        public string OrderNo { get; set; }
        public int OrderDetId { get; set; }
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
            HuMapping another = obj as HuMapping;

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
