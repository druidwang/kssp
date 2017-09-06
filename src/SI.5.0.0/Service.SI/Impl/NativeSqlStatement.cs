using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Service.SI.Impl
{
    public class NativeSqlStatement
    {
        public static string SELECT_FLOW_MASTER_STATEMENT =
                    @"select m.* from SCM_FlowMstr as m 
                    inner join SCM_FlowStrategy as s on m.Code = s.Flow
                    where s.Strategy in (?,?,?) and m.IsAutoCreate = ?";

        public static string SELECT_FLOW_DETAIL_STATEMENT =
                    @"select d.* from SCM_FlowDet as d
                    inner join SCM_FlowMstr as m on d.Flow = m.Code
                    inner join SCM_FlowStrategy as s on m.Code = s.Flow
                    where s.Strategy in (?,?,?) and m.IsAutoCreate = ?
                    and (d.StartDate is null or d.StartDate <= ?) and (d.EndDate is null or d.EndDate > ?) and d.IsAutoCreate = ?";
    }
}
