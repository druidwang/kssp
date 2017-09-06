using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Service
{
    public interface IReceiptMgr
    {
        ReceiptMaster TransferOrder2Receipt(OrderMaster orderMaster);
        //ReceiptMaster TransferOrder2Receipt(IList<OrderMaster> orderMasterList);
        ReceiptMaster TransferIp2Receipt(IpMaster ipMaster);
        ReceiptMaster TransferIpGap2Receipt(IpMaster ipMaster, CodeMaster.IpGapAdjustOption ipGapAdjustOption);
        void CreateReceipt(ReceiptMaster receiptMaster);
        void CreateReceipt(ReceiptMaster receiptMaster, DateTime effectiveDate);
        void CreateReceipt(ReceiptMaster receiptMaster, bool isKit, DateTime effectiveDate);
        void CancelReceipt(string receiptNo);
        void CancelReceipt(string receiptNo, DateTime effectiveDate);
        void CancelReceipt(ReceiptMaster receiptMaster);
        void CancelReceipt(ReceiptMaster receiptMaster, DateTime effectiveDate);
    }
}
