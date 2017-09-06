using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class PalletHu : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public int Id { get; set; }

        [Display(Name = "Pallet_PalletCode", ResourceType = typeof(Resources.INV.PalletHu))]
        public string PalletCode { get; set; }

        [Display(Name = "Pallet_HuId", ResourceType = typeof(Resources.INV.PalletHu))]
        public string HuId { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "Pallet_CreateUserName", ResourceType = typeof(Resources.INV.PalletHu))]
        public string CreateUserName { get; set; }
        [Display(Name = "Pallet_CreateDate", ResourceType = typeof(Resources.INV.PalletHu))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "Pallet_LastModifyUserName", ResourceType = typeof(Resources.INV.PalletHu))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "Pallet_LastModifyDate", ResourceType = typeof(Resources.INV.PalletHu))]
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
            PalletHu another = obj as PalletHu;

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
