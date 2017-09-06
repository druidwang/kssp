using System;

namespace com.Sconit.Entity.SYS
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExportAttribute : Attribute
    {
        public string ExportName { get; set; }
        public int ExportSeq { get; set; }
        public string ExportTitle { get; set; }
        public Type ExportTitleResourceType { get; set; }
    }
}
