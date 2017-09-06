using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class KanBanCard : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "KanBanCard_CardNo", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string Code { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "KanBanCard_Flow", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string Flow { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "KanBanCard_LocationTo", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string LocationTo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "KanBanCard_Item", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string Item { get; set; }
        [Display(Name = "KanBanCard_ItemDescription", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string ItemDescription { get; set; }
        [Display(Name = "KanBanCard_ItemCategory", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string ItemCategory { get; set; }
        [Display(Name = "KanBanCard_Uom", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string Uom { get; set; }
        [Display(Name = "KanBanCard_ManufactureParty", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string ManufactureParty { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "KanBanCard_UnitCount", ResourceType = typeof(Resources.INV.KanBanCard))]
        public Decimal UnitCount { get; set; }

        [Display(Name = "KanBanCard_Qty", ResourceType = typeof(Resources.INV.KanBanCard))]
        public int? Qty { get; set; }

        [Display(Name = "KanBanCard_StationUseQty", ResourceType = typeof(Resources.INV.KanBanCard))]
        public Int32 StationUseQty { get; set; }
        
        [Display(Name = "KanBanCard_PackType", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string PackType { get; set; }
       
        [Display(Name = "KanBanCard_MultiStation", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string MultiStation { get; set; }
        [Display(Name = "KanBanCard_Note", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string Note { get; set; }
        [Display(Name = "KanBanCard_Sequence", ResourceType = typeof(Resources.INV.KanBanCard))]
        public Int32 Sequence { get; set; }
        [Display(Name = "KanBanCard_ThumbNo", ResourceType = typeof(Resources.INV.KanBanCard))]
        public Int32? ThumbNo { get; set; }
        public Int32 CreateUserId { get; set; }
        [Display(Name = "KanBanCard_CreateUserName", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string CreateUserName { get; set; }
        [Display(Name = "KanBanCard_CreateDate", ResourceType = typeof(Resources.INV.KanBanCard))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "KanBanCard_LastModifyUserName", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "KanBanCard_LastModifyDate", ResourceType = typeof(Resources.INV.KanBanCard))]
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "KanBanCard_ReferenceItemCode", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string ReferenceItemCode { get; set; }

        [Display(Name = "KanBanCard_Routing", ResourceType = typeof(Resources.INV.KanBanCard))]
        public string Routing { get; set; }

         [Display(Name = "KanBanCard_IsEleKanBan", ResourceType = typeof(Resources.INV.KanBanCard))]
        public Boolean IsEleKanBan { get; set; }

        public string CheckOrderNo { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            KanBanCard another = obj as KanBanCard;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Code == another.Code);
            }
        }
    }

}
