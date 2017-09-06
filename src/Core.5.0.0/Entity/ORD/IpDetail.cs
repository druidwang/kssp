using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity.INV;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class IpDetail
    {
        #region Non O/R Mapping Properties       
        public decimal ShipQtyInput
        {
            get
            {
                if (IpDetailInputs != null
                    && IpDetailInputs.Count > 0)
                {
                    return IpDetailInputs.Sum(i => i.ShipQty);
                }
                return 0;
            }
        }

        public decimal ReceiveQtyInput
        {
            get
            {
                if (IpDetailInputs != null
                    && IpDetailInputs.Count > 0)
                {
                    return IpDetailInputs.Sum(i => i.ReceiveQty);
                }
                return 0;
            }
        }     

        public decimal RemainReceiveQty
        {
            get
            {
                return this.Qty - this.ReceivedQty;
            }
        }

        //public com.Sconit.CodeMaster.OccupyType CurrentOccupyType { get; set; }
        //public string CurrentOccupyReferenceNo { get; set; }
        public string CurrentPartyFrom { get; set; }
        public string CurrentPartyFromName { get; set; }
        public string CurrentPartyTo { get; set; }
        public string CurrentPartyToName { get; set; }
        public decimal BoxQty { get; set; }
        public bool IsVoid { get; set; }
        public IList<IpLocationDetail> IpLocationDetails { get; set; }
        public IList<IpDetailInput> IpDetailInputs { get; set; }

        [Display(Name = "IpDetail_CurrentReceiveQty", ResourceType = typeof(Resources.ORD.IpDetail))]
        public decimal CurrentReceiveQty { get; set; }
        [Display(Name = "IpDetail_ManufactureDate", ResourceType = typeof(Resources.ORD.IpDetail))]
        public DateTime ManufactureDate { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.IpDetailType, ValueField = "Type")]
        [Display(Name = "IpDetail_Type", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string TypeDescription { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 110)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 130)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "IpMaster_OrderType", ResourceType = typeof(Resources.ORD.IpMaster))]
        public string IpMasterTypeDescription { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 120)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 140)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderSubType, ValueField = "OrderSubType")]
        [Display(Name = "IpDetail_Type", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string SubTypeDescription { get; set; }

        [Display(Name = "IpDetail_LotNo", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string LotNo { get; set; }

        [Display(Name = "IpDetail_HuQty", ResourceType = typeof(Resources.ORD.IpDetail))]
        public decimal HuQty { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 190)]
        [Display(Name = "IpDetail_IsCancel", ResourceType = typeof(Resources.ORD.IpDetail))]
        public Boolean IsCancel { get; set; }
       
        #endregion

        [Display(Name = "IpDetail_SupplierLotNo", ResourceType = typeof(Resources.ORD.IpDetail))]
        public String SupplierLotNo { get; set; }

        //public bool IsOdd { get; set; }
        public decimal MaxUc { get; set; }
        public decimal MinUc { get; set; }

        [Display(Name = "IpDetail_GapQty", ResourceType = typeof(Resources.ORD.IpDetail))]
        public decimal GapQty { get; set; }
        public decimal OrderQty { get; set; }

        [Display(Name = "FlowDetail_Remark", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public String Remark { get; set; }
        #region methods
        public void AddIpLocationDetail(IpLocationDetail ipLocationDetail)
        {
            if (IpLocationDetails == null)
            {
                IpLocationDetails = new List<IpLocationDetail>();
            }
            IpLocationDetails.Add(ipLocationDetail);
        }

        public void AddIpLocationDetail(IList<IpLocationDetail> ipLocationDetailList)
        {
            if (ipLocationDetailList != null)
            {
                if (IpLocationDetails == null)
                {
                    IpLocationDetails = new List<IpLocationDetail>();
                }
                ((List<IpLocationDetail>)IpLocationDetails).AddRange(ipLocationDetailList);
            }
        }

        public void AddIpDetailInput(IpDetailInput ipDetailInput)
        {
            if (IpDetailInputs == null)
            {
                IpDetailInputs = new List<IpDetailInput>();
            }
            IpDetailInputs.Add(ipDetailInput);
        }
        #endregion
    }

    public class IpDetailInput
    {
        public decimal ShipQty { get; set; }
        public decimal ReceiveQty { get; set; }
        //public decimal RejectQty { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public bool IsCreatePlanBill { get; set; }
        public bool IsConsignment { get; set; }
        public int? PlanBill { get; set; }
        public int? ActingBill { get; set; }
        public bool IsFreeze { get; set; }
        public bool IsATP { get; set; }
        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }
        public string OccupyReferenceNo { get; set; }
        public string Bin { get; set; }
        public string SequenceNo { get; set; }
        public string ManufactureParty { get; set; }
        /// <summary>
        /// WMS收货单号
        /// </summary>
        public string WMSRecNo { get; set; }
        /// <summary>
        /// WMS收货单行号
        /// </summary>
        public string WMSRecSeq { get; set; }
        /// <summary>
        /// WMS发货单行号
        /// </summary>
        public string WMSIpSeq { get; set; }

        public IList<IpLocationDetail> ReceivedIpLocationDetailList { get; set; }

        public void AddReceivedIpLocationDetail(IpLocationDetail receivedIpLocationDetail)
        {
            if (ReceivedIpLocationDetailList == null)
            {
                ReceivedIpLocationDetailList = new List<IpLocationDetail>();
            }

            ReceivedIpLocationDetailList.Add(receivedIpLocationDetail);
        }
    }
}