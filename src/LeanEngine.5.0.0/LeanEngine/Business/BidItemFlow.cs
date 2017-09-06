using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeanEngine.Entity;

namespace LeanEngine.Business
{
    public class BidItemFlow
    {
        public ItemFlow getWinBidItemFlow(List<ItemFlow> itemFlows)
        {
            if (itemFlows == null || itemFlows.Count == 0)
                return null;

            //base filter
            var q01 = itemFlows.Where(i => i.Flow.OrderTime <= DateTime.Now && i.ReqQty > 0).ToList();
            if (q01.Count == 0)
                return null;
            else if (q01.Count == 1)
                return q01[0];

            //Bidding start,if count=0,then next
            //1.Supplying rate
            var q11 = q01.Where(i => i.SupplyingRate > 0 && i.AcmlOrderedQty >= 0).ToList();
            if (q11.Count == 1)
                return q11[0];
            else if (q11.Count > 1)
            {
                //Priority
                //1) AcmlOrderedQty=0 and SupplyingRate larger
                //2) AcmlOrderedQty / SupplyingRate result smaller
                //var q12 = q11.OrderBy(i => i.AcmlOrderedQty / i.SupplyingRate).OrderByDescending(i => i.SupplyingRate).ToList();
                var q12 = (from q in q11
                           let a = q.AcmlOrderedQty / q.SupplyingRate
                           orderby a, q.SupplyingRate descending
                           select q).ToList();
                if (q12.Count > 0)
                    return q12[0];
            }

            //default return value
            return q01[0];
        }
    }
}
