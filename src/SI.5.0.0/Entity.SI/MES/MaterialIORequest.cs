using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.MES
{
    public class MaterialIORequest
    {
        public string request_id { get; set; }

        public List<MES_Interface_MaterialIO> data { get; set; }

        public string requester { get; set; }

        public DateTime request_date { get; set; }
    }
}
