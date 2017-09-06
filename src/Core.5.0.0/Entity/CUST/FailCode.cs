using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.CUST
{
    public partial class FailCode
    {
        #region Non O/R Mapping Properties

        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.CHNDescription))
                {
                    return this.Code;
                }
                else
                {
                    return this.Code + " [" + this.CHNDescription + "]";
                }
            }
        }

        public string Description
        {
            get
            {
                string description = string.IsNullOrWhiteSpace(this.CHNDescription) ? this.Code : this.CHNDescription;
                if (string.IsNullOrWhiteSpace(this.ENGDescription))
                {
                    return description;
                }
                else
                {
                    return description + " [" + this.ENGDescription + "]";
                }
            }
        }


        #endregion
    }
}