using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class RccpTransGroup
    {
        public string Model { get; set; }
        public Double ModelRate { get; set; }

        public int TotalAps { get; set; }

        public string ItemDescription { get; set; }

        public bool IsDown { get; set; }

    }
}
