using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{

    public partial class Driver
    {
        #region Non O/R Mapping Properties

        public string CodeDescription
        {
            get 
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    return this.Code;
                }
                else
                { 
                    return this.Code + "[" + this.Name + "]";
                }
            }
        }

        //TODO: Add Non O/R Mapping Properties here. 
        public string Desc
        {
            get
            {
                return this.Name + this.IdNumber;
            }
        }
        #endregion
    }
}