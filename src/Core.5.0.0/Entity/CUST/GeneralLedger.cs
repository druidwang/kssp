using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    public partial class GeneralLedger
    {
        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Description))
                {
                    return this.Code;
                }
                else
                {
                    return this.Code + " [" + this.Description + "]";
                }

            }
        }

    }
}
