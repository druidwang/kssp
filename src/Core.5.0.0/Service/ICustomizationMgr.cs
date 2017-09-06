using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.MRP.TRANS;
using Resources.Report;
using com.Sconit.Entity.Report;

namespace com.Sconit.Service
{
    public interface ICustomizationMgr
    {
        Hu AgingHu(Hu hu);

        Hu AgedHu(Hu hu, DateTime effectiveDate);

        Hu FilterHu(Hu hu, decimal outQty, DateTime effectiveDate);

        void CancelAgingHu(Hu hu, DateTime effectiveDate);

        void AddNewCustReport(string code);

        void DeleteCustReport(string code);

        void UpdateCustReport(CustReportMaster custReport);

        void CancelFilterHu(Hu hu, decimal newQty, DateTime effectiveDate);

        void CancelItemExchangeHu(ItemExchange itemExchange, DateTime effectiveDate);

        void CancelItemExchangeHu(int itemExchangeId, DateTime effectiveDate);

        IList<PubPrintOrder> GetPubPrintOrderList(string clientCode);

        void SetHuTo(IList<HuToMapping> huToMappingList, MrpMiPlan mrpMiPlan);

        string GetHuTo(IList<HuToMapping> huToMappingList, string flow, string item);
    }
}
