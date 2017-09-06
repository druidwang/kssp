using System;
using com.Sconit.Entity.ORD;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SCM
{
    public partial class FlowDetail : IComparable
    {
        #region Non O/R Mapping Properties
        [Display(Name = "FlowDetail_HuQty", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public decimal HuQty { get; set; }

        [Display(Name = "FlowDetail_LotNo", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public String LotNo { get; set; }

        [Display(Name = "FlowDetail_SupplierLotNo", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public String SupplierLotNo { get; set; }

        [Display(Name = "FlowDetail_ManufactureParty", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public String ManufactureParty { get; set; }

        [Display(Name = "FlowDetail_ItemDescription", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public String ItemDescription { get; set; }

        [Display(Name = "FlowDetail_PartyFrom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string PartyFrom { get; set; }

        [Display(Name = "FlowDetail_PartyTo", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string PartyTo { get; set; }

        public decimal OrderQty { get; set; }

        public BindDemand BindDemand { get; set; }

        public FlowMaster CurrentFlowMaster { get; set; }

        public FlowStrategy CurrentFlowStrategy { get; set; }

        [Display(Name = "FlowDetail_FlowStrategy", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string FlowStrategy { get; set; }

        public int ExternalSequence { get; set; }
        [Display(Name = "FlowDetail_ManufactureDate", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public DateTime ManufactureDate { get; set; }

        //public bool IsOdd { get; set; }
        public decimal MaxUc { get; set; }
        public decimal MinUc { get; set; }

        [Range(0, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FlowDetail_CurrentQty", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public double CurrentQty { get; set; }

        [Display(Name = "FlowDetail_Remark", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Remark { get; set; }

        [Display(Name = "FlowDetail_IsFreeze", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public bool IsFreeze { get; set; }

        public string Shift { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Item_Warranty", ResourceType = typeof(Resources.MD.Item))]
        public Int32 Warranty { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Item_WarnLeadTime", ResourceType = typeof(Resources.MD.Item))]
        public Int32 WarnLeadTime { get; set; }
        [Display(Name = "FlowDetail_RoundUpOption", ResourceType = typeof(Resources.SCM.FlowDetail))]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.RoundUpOption, ValueField = "RoundUpOption")]
        public string RoundUpOptionDescription { get; set; }
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }
        [Display(Name = "Item_ItemOption", ResourceType = typeof(Resources.MD.Item))]
        public CodeMaster.ItemOption ItemOption { get; set; }

        #endregion

        public int CompareTo(object obj)
        {
            FlowDetail another = obj as FlowDetail;

            if (another == null)
            {
                return -1;
            }
            else
            {
                return this.Id.CompareTo(another.Id);
            }
        }
        public string DefaultLocationFrom { get; set; }
        public string DefaultLocationTo { get; set; }
        public string DefaultExtraLocationFrom { get; set; }
        public string DefaultExtraLocationTo { get; set; }

        public string ProductType { get; set; }
    }
}