using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class CheckupProject 
    {
        #region Non O/R Mapping Properties

        public string Name
        {
            get
            {
                return this.Code + "[" + this.Desc + "]";
            }
       }

        #endregion
    }
}