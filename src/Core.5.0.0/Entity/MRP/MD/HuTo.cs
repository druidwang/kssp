using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class HuTo
    {
        #region Non O/R Mapping Properties

        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Description) || this.Code == this.Description)
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
}