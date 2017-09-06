using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class TaskReportView
    {
        public Int32? Id { get; set; }
        public string TaskSubTypeCode { get; set; }
        public string TaskType { get; set; }
        public string TaskSubTypeDesc { get; set; }
        public bool IsActive { get; set; }
    }
}