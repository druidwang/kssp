using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.PRD;
using NHibernate.Criterion;

namespace com.Sconit.Service.Impl
{
    public class RoutingMgrImpl : BaseMgr, IRoutingMgr
    {
        #region 变量
        public IQueryMgr gueryMgr { get; set; }
        #endregion

        #region public methods
        public IList<RoutingDetail> GetRoutingDetails(string routing, DateTime? effectiveDate)
        {
            if (!effectiveDate.HasValue)
            {
                effectiveDate = DateTime.Now;
            }

            DetachedCriteria criteria = DetachedCriteria.For<RoutingDetail>();
            criteria.Add(Expression.Eq("Routing", routing));
            criteria.Add(Expression.Le("StartDate", effectiveDate));
            criteria.Add(Expression.Or(Expression.Ge("EndDate", effectiveDate), Expression.IsNull("EndDate")));

            return gueryMgr.FindAll<RoutingDetail>(criteria);

        }
        #endregion
    }
}
