using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeanEngine.Utility;

namespace LeanEngine.Entity
{
    public class Item : EntityBase
    {
        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public string UC { get; set; }

        #region FreeValue
        public string FreeValue1 { get; set; }
        public string FreeValue2 { get; set; }
        public string FreeValue3 { get; set; }
        public string FreeValue4 { get; set; }
        public string FreeValue5 { get; set; }
        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Item another = obj as Item;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.ItemCode == another.ItemCode
                    && Utilities.StringEq(this.FreeValue1, another.FreeValue1)
                    && Utilities.StringEq(this.FreeValue2, another.FreeValue2)
                    && Utilities.StringEq(this.FreeValue3, another.FreeValue3)
                    && Utilities.StringEq(this.FreeValue4, another.FreeValue4)
                    && Utilities.StringEq(this.FreeValue5, another.FreeValue5));
            }
        }
    }
}
