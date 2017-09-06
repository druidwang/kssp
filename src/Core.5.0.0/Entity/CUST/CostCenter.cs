using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    public partial class CostCenter
    {
        public string CodeDescription
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Code) && !string.IsNullOrWhiteSpace(this.Description))
                {
                    return this.Code + " [" + this.Description + "]";
                }
                else
                {
                    return this.Code;
                }
            }
        }

    }
}
