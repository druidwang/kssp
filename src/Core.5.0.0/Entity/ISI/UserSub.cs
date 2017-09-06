using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.ISI
{

    public partial class UserSub
    {
        public string Code { get; set; }
        public bool IsEmail { get; set; }
        public string Email { get; set; }
        public bool IsSMS { get; set; }
        public string MobilePhone { get; set; }
        public string Name { get; set; }
        public string JobNo { get; set; }
        public string Dept2 { get; set; }

        public string Department { get; set; }

        public string LongName
        {
            get
            {
                return (Name
                            + (!string.IsNullOrEmpty(JobNo) ? " " + this.JobNo : string.Empty)
                            + (!string.IsNullOrEmpty(Dept2) ? " " + this.Dept2 : string.Empty)
                            + (!string.IsNullOrEmpty(Department) ? " " + this.Department : string.Empty));
            }
        }
    }
}
