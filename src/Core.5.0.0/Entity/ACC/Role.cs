using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ACC
{
    public partial class Role
    {
       
        /// <summary>
        /// 
        /// </summary>
        public Role()
        {
        }

        public Role(Int32 id)
        {
            this.Id = id;
        }

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
        #region Non O/R Mapping Properties

        #endregion
    }
}