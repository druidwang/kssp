using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ACC;
using com.Sconit.Utility;

namespace com.Sconit.Service.SI.SAP
{
    public interface IMasterDataMgr
    {
        List<ErrorMessage> GetSAPItem(string itemCode, DateTime? reqBeginDate, bool isLoadAll);

        List<ErrorMessage> GetSAPBom(string bom, DateTime? reqBeginDate, bool isLoadAll);

        List<ErrorMessage> GetSAPUomConv(string itemCode, DateTime? reqBeginDate, bool isLoadAll);

        List<ErrorMessage> GetSAPSupplier(string supplier, DateTime? reqBeginDate, bool isLoadAll);

        List<ErrorMessage> GetSAPCustomer(string customer, DateTime? reqBeginDate, bool isLoadAll);

        List<ErrorMessage> GetSAPPriceList(string itemCode, string supplierCode, DateTime? reqBeginDate, bool isLoadAll);
        //void GetSAPPriceList(string )

    }
}
