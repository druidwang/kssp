using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class Checkup 
    {
        #region Non O/R Mapping Properties

        public string Desc
        {
            get
            {
                return this.CheckupUserNm
                    + (!string.IsNullOrEmpty(this.JobNo) ? " " + this.JobNo : string.Empty)
                    + (!string.IsNullOrEmpty(this.Dept2) ? " " + this.Dept2 : string.Empty)
                    + (!string.IsNullOrEmpty(this.Department) ? " " + this.Department : string.Empty);
            }
        }

        #endregion
    }
}