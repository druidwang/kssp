using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class Item
    {
        public Item()
        {
        }

        public Item(string code)
        {
            this.Code = code;
        }
        #region Non O/R Mapping Properties

        [Display(Name = "Item_MinUnitCount", ResourceType = typeof(Resources.MD.Item))]
        public Decimal MinUnitCount { get; set; }

        [Display(Name = "Item_SupplierLotNo", ResourceType = typeof(Resources.MD.Item))]
        public string SupplierLotNo { get; set; }

        [Display(Name = "Item_LotNo", ResourceType = typeof(Resources.MD.Item))]
        public string LotNo { get; set; }

        [Display(Name = "Item_HuQty", ResourceType = typeof(Resources.MD.Item))]
        public Decimal HuQty { get; set; }

        [Display(Name = "Item_HuUnitCount", ResourceType = typeof(Resources.MD.Item))]
        public Decimal HuUnitCount { get; set; }

        [Display(Name = "Item_HuUom", ResourceType = typeof(Resources.MD.Item))]
        public string HuUom { get; set; }

        [Display(Name = "Item_ManufactureParty", ResourceType = typeof(Resources.MD.Item))]
        public string ManufactureParty { get; set; }

        public string Deriction { get; set; }
        public string Remark { get; set; }

        [Export(ExportName = "Item", ExportSeq = 75)]
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }

        [Export(ExportName = "Item", ExportSeq = 65)]
        [Display(Name = "Item_ItemCategoryDesc", ResourceType = typeof(Resources.MD.Item))]
        public string ItemCategoryDesc { get; set; }

        public CodeMaster.HuOption HuOption { get; set; }

        [Display(Name = "Item_ManufactureDate", ResourceType = typeof(Resources.MD.Item))]
        public DateTime ManufactureDate { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ItemPriority, ValueField = "ItemPriority")]
        [Display(Name = "Item_ItemPriority", ResourceType = typeof(Resources.MD.Item))]
        public string ItemPriorityDesc { get; set; }

        public string HuTemplate { get; set; }

        public bool IsPrintPallet { get; set; }
        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Description))
                {
                    return this.Code;
                }
                else
                {
                    return this.Code + " [" + this.Description + "]";
                }

            }
        }

        public string FullDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.ReferenceCode))
                {
                    return this.Description;
                }
                else
                {
                    return this.Description + " [" + this.ReferenceCode + "]";
                }
            }
        }
        #endregion
    }
}