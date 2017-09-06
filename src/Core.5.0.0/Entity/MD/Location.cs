using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class Location
    {
        #region Non O/R Mapping Properties

        public string CodeName
        {
            get
            {
                return string.IsNullOrEmpty(this.Code) ? "" : this.Code + " [" + this.Name + "]";
            }
        }

        public string Bins { get; set; }
        #endregion
    }
}