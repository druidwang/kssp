using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class Item : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Export(ExportName = "Item", ExportSeq = 10)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Item_Code", ResourceType = typeof(Resources.MD.Item))]
        public string Code { get; set; }
        [Export(ExportName = "Item", ExportSeq = 20)]
        [Display(Name = "Item_ReferenceCode", ResourceType = typeof(Resources.MD.Item))]
        public string ReferenceCode { get; set; }

        [Display(Name = "Item_ShortCode", ResourceType = typeof(Resources.MD.Item))]
        public string ShortCode { get; set; }
        [Export(ExportName = "Item", ExportSeq =40)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Item_Uom", ResourceType = typeof(Resources.MD.Item))]
        public string Uom { get; set; }
        [Export(ExportName = "Item", ExportSeq = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(100, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Item_Description", ResourceType = typeof(Resources.MD.Item))]
        public string Description { get; set; }
        [Export(ExportName = "Item", ExportSeq = 50)]
        [Display(Name = "Item_UC", ResourceType = typeof(Resources.MD.Item))]
        public Decimal UnitCount { get; set; }
        [Export(ExportName = "Item", ExportSeq = 60)]
        [Display(Name = "Item_ItemCategory", ResourceType = typeof(Resources.MD.Item))]
        public string ItemCategory { get; set; }
        [Export(ExportName = "Item", ExportSeq = 80)]
        [Display(Name = "Item_IsActive", ResourceType = typeof(Resources.MD.Item))]
        public Boolean IsActive { get; set; }

        //[Display(Name = "Item_IsPurchase", ResourceType = typeof(Resources.MD.Item))]
        //public Boolean IsPurchase { get; set; }

        //[Display(Name = "Item_IsSales", ResourceType = typeof(Resources.MD.Item))]
        //public Boolean IsSales { get; set; }

        //[Display(Name = "Item_IsManufacture", ResourceType = typeof(Resources.MD.Item))]
        //public Boolean IsManufacture { get; set; }

        //[Display(Name = "Item_IsSubContract", ResourceType = typeof(Resources.MD.Item))]
        //public Boolean IsSubContract { get; set; }

        //[Display(Name = "Item_IsCustomerGoods", ResourceType = typeof(Resources.MD.Item))]
        //public Boolean IsCustomerGoods { get; set; }

        [Display(Name = "Item_IsVirtual", ResourceType = typeof(Resources.MD.Item))]
        public Boolean IsVirtual { get; set; }

        [Display(Name = "Item_IsKit", ResourceType = typeof(Resources.MD.Item))]
        public Boolean IsKit { get; set; }

        [Display(Name = "Item_Bom", ResourceType = typeof(Resources.MD.Item))]
        public string Bom { get; set; }

        [Display(Name = "Item_Location", ResourceType = typeof(Resources.MD.Item))]
        public string Location { get; set; }

        [Display(Name = "Item_Routing", ResourceType = typeof(Resources.MD.Item))]
        public string Routing { get; set; }

        [Display(Name = "Item_IsInvFreeze", ResourceType = typeof(Resources.MD.Item))]
        public Boolean IsInventoryFreeze { get; set; }
        [Export(ExportName = "Item", ExportSeq = 78)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Item_Warranty", ResourceType = typeof(Resources.MD.Item))]
        public Int32 Warranty { get; set; }
        [Export(ExportName = "Item", ExportSeq = 77)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Item_WarnLeadTime", ResourceType = typeof(Resources.MD.Item))]
        public Int32 WarnLeadTime { get; set; }

        //[Display(Name = "Item_IsSection", ResourceType = typeof(Resources.MD.Item))]
        //public Boolean IsSection { get; set; }

        [Display(Name = "Item_ItemPriority", ResourceType = typeof(Resources.MD.Item))]
        public com.Sconit.CodeMaster.ItemPriority ItemPriority { get; set; }

        [Display(Name = "Item_Weight", ResourceType = typeof(Resources.MD.Item))]
        public Double Weight { get; set; }

        [Display(Name = "Item_WorkHour", ResourceType = typeof(Resources.MD.Item))]
        public Double WorkHour { get; set; }

        public string Container { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        /// <summary>
        /// 物料选项
        /// </summary>
        [Display(Name = "Item_ItemOption", ResourceType = typeof(Resources.MD.Item))]
        public CodeMaster.ItemOption ItemOption { get; set; }

        /// <summary>
        /// 物料组
        /// </summary>
        [Export(ExportName = "Item", ExportSeq = 70)]
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }

        /// <summary>
        /// 安全库存
        /// </summary>
        [Display(Name = "Item_SafeStock", ResourceType = typeof(Resources.MD.Item))]
        public double SafeStock { get; set; }
        [Display(Name = "Item_ScrapPercent", ResourceType = typeof(Resources.MD.Item))]
        public Double ScrapPercent { get; set; }

        [Display(Name = "Item_ContainerSize", ResourceType = typeof(Resources.MD.Item))]
        public Double ContainerSize { get; set; }

        /// <summary>
        /// 产品组
        /// </summary>
        [Export(ExportName = "Item", ExportSeq = 76)]
        [Display(Name = "Item_Division", ResourceType = typeof(Resources.MD.Item))]
        public string Division { get; set; }

        public Decimal PalletLotSize { get; set; }
        public Decimal PackageVolume { get; set; }
        public Decimal PackageWeight { get; set; }
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
            Item another = obj as Item;

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
