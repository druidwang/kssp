using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class FailureMode 
    {
        #region Non O/R Mapping Properties

        public string Description
        {
            get
            {
                return this.Code + "[" + this.Desc + "]";
            }
        }

        #endregion
    }
}