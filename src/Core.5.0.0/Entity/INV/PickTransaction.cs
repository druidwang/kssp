using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.INV
{
    public partial class PickTransaction
    {
        #region Non O/R Mapping Properties
        [Export(ExportName = "PickTrans", ExportSeq = 30)]
        [Display(Name = "Hu_ItemDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string ItemDescription { get; set; }
        [Export(ExportName = "PickTrans", ExportSeq = 70)]
        [Display(Name = "Hu_HuAction", ResourceType = typeof(Resources.INV.Hu))]
        public string HuAction { get; set; }

        [Export(ExportName = "PickTrans", ExportSeq = 33)]
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }

        [Export(ExportName = "PickTrans", ExportSeq = 36)]
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }
        #endregion
    }
}