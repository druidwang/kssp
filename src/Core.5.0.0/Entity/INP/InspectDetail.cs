using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.INP
{
    public partial class InspectDetail
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        #region 废弃
        //[Display(Name = "InspectDetail_CurrentQualifyQty", ResourceType = typeof(Resources.INP.InspectDetail))]
        //public decimal CurrentQualifyQty { get; set; }
        //[Display(Name = "InspectDetail_CurrentRejectQty", ResourceType = typeof(Resources.INP.InspectDetail))]
        //public decimal CurrentRejectQty { get; set; }
        //[Display(Name = "InspectDetail_CurrentReturnQty", ResourceType = typeof(Resources.INP.InspectDetail))]
        //public decimal CurrentReturnQty { get; set; }
        ///// <summary>
        ///// 让步使用数
        ///// </summary>
        //public decimal CurrentConcessionQty { get; set; }
        #endregion

        #region add by liqiuyun
        public decimal CurrentQty { get; set; }
        [Display(Name = "InspectDetail_JudgeFailCode", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string JudgeFailCode { get; set; }
        public CodeMaster.HandleResult HandleResult { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 30)]
        [Display(Name = "InspectDetail_ItemDesc", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string ItemFullDescription
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.ReferenceItemCode) ? this.ItemDescription : this.ItemDescription + "[" + this.ReferenceItemCode + "]";
            }
        }
        [Display(Name = "InspectResult_Note", ResourceType = typeof(Resources.INP.InspectResult))]
        public string CurrentInspectResultNote { get; set; }
        [Display(Name = "InspectDetail_InspectQty", ResourceType = typeof(Resources.INP.InspectDetail))]
        public decimal CurrentInspectQty
        {
            get
            {
                decimal currentInspectQty = this.InspectQty - this.QualifyQty - this.RejectQty;
                return currentInspectQty > 0 ? currentInspectQty : 0;
            }
        }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 90)]
        [Display(Name = "InspectDetail_FailCode", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string FailCodeDescription { get; set; }
        #endregion 

        [Display(Name = "InspectDetail_Defect", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string Defect { get; set; }
        [Display(Name = "InspectResult_JudgeResult", ResourceType = typeof(Resources.INP.InspectResult))]
        public com.Sconit.CodeMaster.JudgeResult JudgeResult { get; set; }
        [Display(Name = "InspectDetail_HandledQty", ResourceType = typeof(Resources.INP.InspectDetail))]
        public decimal HandledQty { get; set; }

        public string WMSResNo { get; set; }
        public string WMSResSeq { get; set; }
        public Boolean IsConsignment { get; set; }
        public Int32? PlanBill { get; set; }
        [Display(Name = "Hu_SupplierLotNo", ResourceType = typeof(Resources.INV.Hu))]
        public string SupplierLotNo { get; set; }
        [Display(Name = "Hu_ExpireDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? ExpireDate { get; set; }
        //PPM報表使用
        [Display(Name = "InspectDetail_IpNo", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string IpNo { get; set; }
        [Display(Name = "InspectDetail_ManufacturePartyName", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string ManufacturePartyName { get; set; }
        public decimal CurrentTransferQty
        {
            get
            {
                decimal currentTransferQty = this.InspectQty - this.QualifyQty - this.RejectQty;
                return currentTransferQty > 0 ? currentTransferQty : 0;
            }
        }

        #endregion
    }
}