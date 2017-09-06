using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace com.Sconit.Web.Models.ReportModels
{
    public class CustReportModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Sql { get; set; }
        public string ParamType { get; set; }
        public string ParamKey { get; set; }
        public string ParamText { get; set; }

        public int Seq { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public string Text5 { get; set; }
        public string _AddressComboBox { get; set; }
        public string _AssembliesDropDownList { get; set; }
        public string _BomComboBox { get; set; }
        public string _CodeMasterComboBox { get; set; }
        public string _CodeMasterDropDownList { get; set; }
        public string _CommonDropDownList { get; set; }
        public string _ContainerDropDownList { get; set; }
        public string _CostCenterComboBox { get; set; }
        public string _CurrencyDropDownList { get; set; }
        public string _CustomerComboBox { get; set; }
        public string _DateIndexComboBox { get; set; }
        public string _DefectCodeComboBox { get; set; }
        public string _DefectCodeDropDownList { get; set; }
        public string _FailCodeComboBox { get; set; }
        public string _FiShiftComboBox { get; set; }
        public string _FlowComboBox { get; set; }
        public string _FlowItemComboBox { get; set; }
        public string _GeneralLedgerComboBox { get; set; }
        public string _HuToComboBox { get; set; }
        public string _InspectComboBox { get; set; }
        public string _IslandComboBox { get; set; }
        public string _ItemCategoryDropDownList { get; set; }
        public string _ItemComboBox { get; set; }
        public string _ItemPackageComboBox { get; set; }
        public string _ItemPackageDropDownList { get; set; }
        public string _LocationAreaDropDownList { get; set; }
        public string _LocationBinComboBox { get; set; }
        public string _LocationBinDropDownList { get; set; }
        public string _LocationComboBox { get; set; }
        public string _MachineComboBox { get; set; }
        public string _ManufacturePartyComboBox { get; set; }
        public string _ManufacturePartyDropDownList { get; set; }
        public string _MoveTypeDropDownList { get; set; }
        public string _MrpPlanMasterComboBox { get; set; }
        public string _MrpSnapMasterComboBox { get; set; }
        public string _OperationDropDownList { get; set; }
        public string _OrderComboBox { get; set; }
        public string _OrderMasterPartyFromComboBox { get; set; }
        public string _OrderMasterPartyToComboBox { get; set; }
        public string _PartyDropDownList { get; set; }
        public string _PickStrategyComboBox { get; set; }
        public string _PlanNoComboBox { get; set; }
        public string _PriceListComboBox { get; set; }
        public string _ProductCodeDropDownList { get; set; }
        public string _ProductLineFacilityComboBox { get; set; }
        public string _ProductTypeDropDownList { get; set; }
        public string _PurchasePlanMasterComboBox { get; set; }
        public string _RccpPlanMasterComboBox { get; set; }
        public string _RegionCombobox { get; set; }
        public string _RegionWorkShopComboBox { get; set; }
        public string _RejectComboBox { get; set; }
        public string _RoutingComboBox { get; set; }
        public string _RoutingDropDownList { get; set; }
        public string _SAPLocationComboBox { get; set; }
        public string _SectionComboBox { get; set; }
        public string _ShiftComboBox { get; set; }
        public string _SupplierComboBox { get; set; }
        public string _UomDropDownList { get; set; }
        public string _UserComboBox { get; set; }
        public string _WorkingCalendarFlowComboBox { get; set; }
    }
}