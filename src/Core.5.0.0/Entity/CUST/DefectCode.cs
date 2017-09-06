using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.CUST
{
    public partial class DefectCode
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

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

        #endregion
    }

    public partial class ProductCode {
        public string Code { get; set; }
    }

    public partial class Assemblies
    {
        public string Code { get; set; }
    }
}