namespace com.Sconit.Entity.SI.SD_MD
{
    using System;

    [Serializable]
    public class Bin
    {
        #region Non O/R Mapping Properties
        public string Code { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public string Location { get; set; }
        public string Region { get; set; }
        public Int32 Sequence { get; set; }
        public Boolean IsActive { get; set; }
        #endregion
    }
}