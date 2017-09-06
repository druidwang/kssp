using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.PRD
{
    public partial class ShiftMaster
    {
       
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        public string CodeName
        {
            get { return this.Code + "[" + this.Name + "]"; }
        }
        #endregion

    }
}