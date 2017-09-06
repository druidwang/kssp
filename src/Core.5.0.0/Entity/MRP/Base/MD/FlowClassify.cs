using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class FlowClassify : EntityBase
    {
        #region O/R Mapping Properties
        public string Code { get; set; }
        public string Flow { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Code != null && Flow != null)
            {
                return Code.GetHashCode() ^ Flow.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            FlowClassify another = obj as FlowClassify;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Code == another.Code)
                    && (this.Flow == another.Flow);
            }
        }
    }

}
