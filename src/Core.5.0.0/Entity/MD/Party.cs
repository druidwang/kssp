using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class Party
    {
        #region Non O/R Mapping Properties
        public string CodeDescription
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Code) && !string.IsNullOrWhiteSpace(this.Name))
                {
                    return this.Code + " [" + this.Name + "]";
                }
                else
                {
                    return this.Code;
                }
            }
        }
        #endregion
    }
}