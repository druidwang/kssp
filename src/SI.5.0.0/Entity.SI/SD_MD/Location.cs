namespace com.Sconit.Entity.SI.SD_MD
{
    using System;

    [Serializable]
    public class Location
    {
        #region O/R Mapping Properties
        public string Code { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public Boolean IsActive { get; set; }

        public Boolean AllowNegative { get; set; }

        public Boolean EnableAdvanceWarehouseManagment { get; set; }

        public Boolean IsConsignment { get; set; }

        public Boolean IsMRP { get; set; }

        public Boolean IsInventoryFreeze { get; set; }
        #endregion
    }
}