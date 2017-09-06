using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.SI.SAP
{
    public class InvTransSearchModel : SearchModelBase
    {
      //TCODE, BWART, BLDAT, BUDAT, EBELN, EBELP, VBELN, POSNR, LIFNR, WERKS, LGORT, SOBKZ, MATNR, ERFMG, ERFME, UMLGO, GRUND, KOSTL, XBLNR, RSNUM, RSPOS, FRBNR, SGTXT, OLD, INSMK, XABLN, AUFNR, UMMAT, UMWRK, POSID, CreateDate, LastModifyDate, Status, ErrorCount, BatchNo, CHARG, KZEAR, ErrorId
        public string BWART { get; set; }
        public string BUDAT { get; set; }
        public string EBELN { get; set; }
        public string BUDATTo { get; set; }
        public string EBELP { get; set; }
        public string LIFNR { get; set; }
        public string LGORT { get; set; }
        public string MATNR { get; set; }
        public string Status { get; set; }
        public string XBLNR { get; set; }
        public string RSNUM { get; set; }
        public string RSPOS { get; set; }
        public string FRBNR { get; set; }
        public string SGTXT { get; set; }
        public string XABLN { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}