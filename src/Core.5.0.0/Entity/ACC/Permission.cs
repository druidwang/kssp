using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ACC
{
    public partial class Permission
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
}