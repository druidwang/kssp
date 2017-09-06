using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class LocationArea
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

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