using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class LocationBin
    {
        #region Non O/R Mapping Properties

        public string CodeName
        {
            get
            {
                return string.IsNullOrEmpty(this.Code) ? "" : this.Code + " [" + this.Name + "]";
            }
        }

        #endregion
    }
}