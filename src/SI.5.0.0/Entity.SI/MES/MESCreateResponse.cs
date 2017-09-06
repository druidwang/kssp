using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.MES
{
    public class MESCreateResponse
    {
        public int Status{ get;set;}
        public string ErrorCode { get; set; }
        public string ErrorMesaage { get; set; }
        public string BarCode { get; set; }
    }
}
