using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.ISI
{

    public partial class UserSubView
    {
        public Int32? Id { get; set; }
        public string TaskSubTypeCode { get; set; }
        public string TaskType { get; set; }
        public string TaskSubTypeDesc { get; set; }
        public bool IsEmail { get; set; }
        public string Email { get; set; }
        public bool IsSMS { get; set; }
        public string MobilePhone { get; set; }
        //public string UserCode { get; set; }
    }
}
