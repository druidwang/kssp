using System;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SYS
{
    public partial class Menu
    {
        #region Non O/R Mapping Properties

        public IList<Menu> ChildrenMenu { get; set; }        

        #endregion
    }
}