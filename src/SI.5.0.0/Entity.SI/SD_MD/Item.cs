using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.SD_MD
{
    [Serializable]
    public partial class Item 
    {
        #region O/R Mapping Properties
        public string Code { get; set; }

        public string ReferenceCode { get; set; }

        public string Uom { get; set; }

        public string Description { get; set; }

        public Decimal UnitCount { get; set; }

        public string ItemCategory { get; set; }

        public Boolean IsActive { get; set; }

        public Boolean IsPurchase { get; set; }

        public Boolean IsSales { get; set; }

        public Boolean IsManufacture { get; set; }

        public Boolean IsSubContract { get; set; }

        public Boolean IsCustomerGoods { get; set; }

        public Boolean IsVirtual { get; set; }

        public Boolean IsKit { get; set; }

        public string Bom { get; set; }

        public string Location { get; set; }

        public string Routing { get; set; }

        public Boolean IsInventoryFreeze { get; set; }

        public Int32 Warranty { get; set; }

        public Int32 WarnLeadTime { get; set; }

        public string Container { get; set; }

        //public Int32 CreateUserId { get; set; }
        //public string CreateUserName { get; set; }
        //public DateTime CreateDate { get; set; }
        //public Int32 LastModifyUserId { get; set; }
        //public string LastModifyUserName { get; set; }
        //public DateTime LastModifyDate { get; set; }
        #endregion

    }
}
