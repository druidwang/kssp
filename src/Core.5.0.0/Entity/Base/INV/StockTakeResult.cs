using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class StockTakeResult : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_StNo", ResourceType = typeof(Resources.INV.StockTake))]
        public String StNo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_Item", ResourceType = typeof(Resources.INV.StockTake))]
        public String Item { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(100, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_ItemDesc", ResourceType = typeof(Resources.INV.StockTake))]
        public String ItemDescription { get; set; }

        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Display(Name = "StockTakeResult_Uom", ResourceType = typeof(Resources.INV.StockTake))]
        public String Uom { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_HuId", ResourceType = typeof(Resources.INV.StockTake))]
        public String HuId { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_LotNo", ResourceType = typeof(Resources.INV.StockTake))]
        public String LotNo { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_StockTakeQty", ResourceType = typeof(Resources.INV.StockTake))]
        public Decimal StockTakeQty { get; set; }

        [Display(Name = "StockTakeResult_InventoryQty", ResourceType = typeof(Resources.INV.StockTake))]
        public Decimal InventoryQty { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_DifferenceQty", ResourceType = typeof(Resources.INV.StockTake))]
        public Decimal DifferenceQty { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_Location", ResourceType = typeof(Resources.INV.StockTake))]
        public String Location { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "StockTakeResult_Bin", ResourceType = typeof(Resources.INV.StockTake))]
        public String Bin { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        //[Display(Name = "StockTakeResult_IsAdj", ResourceType = typeof(Resources.INV.StockTake))]
        public DateTime? EffectiveDate { get; set; }

        public DateTime? BaseInventoryDate { get; set; }

        public Boolean IsAdjust { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }

        public bool IsCS { get; set; }
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
            StockTakeResult another = obj as StockTakeResult;

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
