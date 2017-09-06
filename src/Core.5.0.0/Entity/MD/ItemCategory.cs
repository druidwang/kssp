using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class ItemCategory
    {
        #region Non O/R Mapping Properties

        [Display(Name = "ItemCategory_ParentCategory", ResourceType = typeof(Resources.MD.ItemCategory))]
        public string ParentCategoryDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.SubCategory, ValueField = "SubCategory")]
        [Display(Name = "ItemCategory_SubCategory", ResourceType = typeof(Resources.MD.ItemCategory))]
        public string SubCategoryDescription { get; set; }


        public string CodeDescription
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Code) && !string.IsNullOrWhiteSpace(this.Description))
                {
                    return this.Code + " [" + this.Description + "]";
                }
                else
                {
                    return this.Code;
                }
            }
        }


        #endregion
    }
}