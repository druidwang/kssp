using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Service.SI
{
    public interface IEDI_ScheduleMgr
    {
        void LoadEDI();

        void EDI2Plan();
    }
}
