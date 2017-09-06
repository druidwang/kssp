using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class ReceiptDetail
    {
        #region Non O/R Mapping Properties
        public string CurrentPartyFrom { get; set; }
        public string CurrentPartyFromName { get; set; }
        public string CurrentPartyTo { get; set; }
        public string CurrentPartyToName { get; set; }
        public string CurrentExternalReceiptNo { get; set; }
        public com.Sconit.CodeMaster.OccupyType CurrentOccupyType { get; set; }
        public string CurrentOccupyReferenceNo { get; set; }
        public bool CurrentIsReceiveScanHu { get; set; }
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 75)]
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }
        public IList<ReceiptLocationDetail> ReceiptLocationDetails { get; set; }
        public IList<ReceiptDetailInput> ReceiptDetailInputs { get; set; }
        public bool IsVoid { get; set; }
        public string LotNo { get; set; }

        public string Direction { get; set; }
        public string Remark { get; set; }
        public CodeMaster.HuOption HuOption { get; set; }

        public decimal MaxUc { get; set; }
        public decimal MinUc { get; set; }
        public int BoxQty { get; set; }
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 50)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderSubType, ValueField = "OrderSubType")]
        [Display(Name = "ReceiptMaster_OrderSubTypeDescription", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string OrderSubTypeDescription { get; set; }

        public decimal ReceiveQtyInput
        {
            get
            {
                if (ReceiptDetailInputs != null
                    && ReceiptDetailInputs.Count > 0)
                {
                    return ReceiptDetailInputs.Sum(i => i.ReceiveQty);
                }
                return 0;
            }
        }

        public decimal ScrapQtyInput
        {
            get
            {
                if (ReceiptDetailInputs != null
                    && ReceiptDetailInputs.Count > 0)
                {
                    return ReceiptDetailInputs.Sum(i => i.ScrapQty);
                }
                return 0;
            }
        }

        //public decimal RejectQtyInput
        //{
        //    get
        //    {
        //        if (ReceiptDetailInputs != null
        //            && ReceiptDetailInputs.Count > 0)
        //        {
        //            return ReceiptDetailInputs.Sum(i => i.RejectQty);
        //        }
        //        return 0;
        //    }
        //}

        //public decimal ReceiveAndRejectQtyInput
        //{
        //    get
        //    {
        //        if (ReceiptDetailInputs != null
        //            && ReceiptDetailInputs.Count > 0)
        //        {
        //            return ReceiptDetailInputs.Sum(i => i.ReceiveQty) + ReceiptDetailInputs.Sum(i => i.RejectQty);
        //        }
        //        return 0;
        //    }
        //}
        #endregion

        public void AddReceiptLocationDetail(ReceiptLocationDetail receiptLocationDetail)
        {
            if (ReceiptLocationDetails == null)
            {
                ReceiptLocationDetails = new List<ReceiptLocationDetail>();
            }
            ReceiptLocationDetails.Add(receiptLocationDetail);
        }

        public void AddReceiptLocationDetail(IList<ReceiptLocationDetail> receiptLocationDetailList)
        {
            if (receiptLocationDetailList != null)
            {
                if (ReceiptLocationDetails == null)
                {
                    ReceiptLocationDetails = new List<ReceiptLocationDetail>();
                }
                ((List<ReceiptLocationDetail>)ReceiptLocationDetails).AddRange(receiptLocationDetailList);
            }
        }

        public void AddReceiptDetailInput(ReceiptDetailInput receiptDetailInput)
        {
            if (ReceiptDetailInputs == null)
            {
                ReceiptDetailInputs = new List<ReceiptDetailInput>();
            }
            ReceiptDetailInputs.Add(receiptDetailInput);
        }
    }

    public class ReceiptDetailInput
    {
        public decimal ReceiveQty { get; set; }
        //给生产收货用
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public decimal ScrapQty { get; set; }
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
        public string SequenceNo { get; set; }
        //WMS收货单行号
        public string WMSRecSeq { get; set; }

        public IList<IpLocationDetail> ReceivedIpLocationDetailList { get; set; }
        public void AddIpLocationDetail(IpLocationDetail ipLocationDetail)
        {
            if (ReceivedIpLocationDetailList == null)
            {
                ReceivedIpLocationDetailList = new List<IpLocationDetail>();
            }

            ReceivedIpLocationDetailList.Add(ipLocationDetail);
        }


    }
}