using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using com.Sconit.PrintModel.ORD;
using com.Sconit.PrintModel.INV;

namespace com.Sconit.PrintModel
{
    [DataContract]
    [KnownType(typeof(PrintOrderMaster))]
    [KnownType(typeof(PrintIpMaster))]
    [KnownType(typeof(PrintPickListMaster))]
    [KnownType(typeof(PrintReceiptMaster))]
    [KnownType(typeof(PrintSequenceMaster))]
    [KnownType(typeof(PrintHu))]
    public class PrintBase
    {
    }
}
