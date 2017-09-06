using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class ItemExchange : EntityBase,IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
       [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_ItemFrom", ResourceType = typeof(Resources.INV.ItemExchange))]
		public string ItemFrom { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_ItemTo", ResourceType = typeof(Resources.INV.ItemExchange))]
		public string ItemTo { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_RegionFrom", ResourceType = typeof(Resources.INV.ItemExchange))]
		public string RegionFrom { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_RegionTo", ResourceType = typeof(Resources.INV.ItemExchange))]
		public string RegionTo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_LocationFrom", ResourceType = typeof(Resources.INV.ItemExchange))]
		public string LocationFrom { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_LocationTo", ResourceType = typeof(Resources.INV.ItemExchange))]
		public string LocationTo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_Qty", ResourceType = typeof(Resources.INV.ItemExchange))]
		public Decimal Qty { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_Uom", ResourceType = typeof(Resources.INV.ItemExchange))]
		public string Uom { get; set; }
		public string BaseUom { get; set; }
		public Decimal UnitQty { get; set; }
		public Int32 CreateUserId { get; set; }
        [Display(Name = "ItemExchange_CreateUserName", ResourceType = typeof(Resources.INV.ItemExchange))]
		public string CreateUserName { get; set; }
        [Display(Name = "ItemExchange_CreateDate", ResourceType = typeof(Resources.INV.ItemExchange))]
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }

        [Display(Name = "ItemExchange_QualityType", ResourceType = typeof(Resources.INV.ItemExchange))]
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemExchange_EffectiveDate", ResourceType = typeof(Resources.INV.ItemExchange))]
		public DateTime EffectiveDate { get; set; }
        [Display(Name = "ItemExchange_IsVoid", ResourceType = typeof(Resources.INV.ItemExchange))]
		public Boolean IsVoid { get; set; }

        [Display(Name = "ItemExchange_NewQty", ResourceType = typeof(Resources.INV.ItemExchange))]
        public decimal NewQty{get;set;}
        [Display(Name = "ItemExchange_NewUom", ResourceType = typeof(Resources.INV.ItemExchange))]
        public string NewUom{get;set;}
        public string NewBaseUom{get;set;}
        public decimal NewUnitQty{get;set;}
        [Display(Name = "ItemExchange_OldHu", ResourceType = typeof(Resources.INV.ItemExchange))]
        public string OldHu{get;set;}
        [Display(Name = "ItemExchange_NewHu", ResourceType = typeof(Resources.INV.ItemExchange))]
        public string NewHu{get;set;}
        [Display(Name = "ItemExchange_ExchangeType", ResourceType = typeof(Resources.INV.ItemExchange))]
        public com.Sconit.CodeMaster.ItemExchangeType ItemExchangeType { get; set; }
        public string LotNo { get; set; }

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
            ItemExchange another = obj as ItemExchange;

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
