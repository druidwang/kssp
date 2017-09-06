using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.VIEW
{
    public partial class LocationDetailView
    {
        #region Non O/R Mapping Properties
        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 20)]
        [Display(Name = "LocationDetailView_Description", ResourceType = typeof(Resources.View.LocationDetailView))]
        public string ItemDescription { get; set; }

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 30)]
        [Display(Name = "LocationDetailView_Uom", ResourceType = typeof(Resources.View.LocationDetailView))]
        public string Uom { get; set; }

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 50)]
        [Display(Name = "LocationDetailView_Name", ResourceType = typeof(Resources.View.LocationDetailView))]
        public string LocationName { get; set; }

        //[Display(Name = "LocationDetailView_suppliers", ResourceType = typeof(Resources.View.LocationDetailView))]
        //public string suppliers { get; set; }

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 55, ExportTitle = "Hu_manufacture_date", ExportTitleResourceType = typeof(Resources.INV.Hu))]
        [Display(Name = "LocationDetailView_LotNo", ResourceType = typeof(Resources.View.LocationDetailView))]
        public string LotNo { get; set; }

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 23)]
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 26)]
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }
        //TODO: Add Non O/R Mapping Properties here. 

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 117)]
        [Display(Name = "LocationDetailView_TransQualifyQty", ResourceType = typeof(Resources.View.LocationDetailView))]
        public decimal TransQualifyQty { get; set; }

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 118)]
        [Display(Name = "LocationDetailView_TransRejectQty", ResourceType = typeof(Resources.View.LocationDetailView))]
        public decimal TransRejectQty { get; set; }

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 119)]
        [Display(Name = "LocationDetailView_TransQty", ResourceType = typeof(Resources.View.LocationDetailView))]
        public decimal TransQty { get; set; }

        [Export(ExportName = "RealtimeInventoryView", ExportSeq = 119)]
        [Display(Name = "LocationDetailView_SalesTransQty", ResourceType = typeof(Resources.View.LocationDetailView))]
        public decimal SalesTransQty { get; set; }
        #endregion
    }
}