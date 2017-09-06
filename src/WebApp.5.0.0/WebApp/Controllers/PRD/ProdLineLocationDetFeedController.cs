using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.PRD;
using System.Text;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Web.Controllers.PRD
{
    public class ProdLineLocationDetFeedController : WebAppBaseController
    {
        //
        // GET: /ProdLineLocationDetFeed/

        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_PRD_ProdLineLocationDetFeed")]
        public ActionResult List(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            this.ProcessSearchModel(command, searchModel);
            if (string.IsNullOrEmpty(searchModel.OrderNo))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseInputOrderNumber);
                return View(new List<OrderBomDetail>());
            }
            else
            {
                TempData["_AjaxMessage"] = "";
                string sql = PrepareSearchStatement(searchModel);
                IList<object[]> list = this.queryMgr.FindAllWithNativeSql<object[]>(sql);
                IList<OrderBomDetail> OrderBomDetailList = new List<OrderBomDetail>();
                OrderBomDetailList = (from tak in list
                                      select new OrderBomDetail
                                      {
                                          OrderNo = (string)tak[0],
                                          Item = (string)tak[1],
                                          ItemDescription = (string)tak[2],
                                          ReferenceItemCode = (string)tak[3],
                                          Uom = (string)tak[4],
                                          ManufactureParty = (string)tak[5],
                                          Operation = (int)tak[6],
                                          OpReference = (string)tak[7],
                                          OrderedQty = (decimal)tak[8],
                                          HuId = (string)tak[9],
                                          FeedQty = (decimal)tak[10],
                                      }).ToList();
                return View(OrderBomDetailList);
            }

           
        }

        private string PrepareSearchStatement( ProductLineLocationDetailSearchModel searchModel)
        {
            StringBuilder Sb = new StringBuilder();
            string whereStatement = @"select d.orderno,d.item,d.itemdesc,d.refitemcode,d.uom,isnull(d.manufactureparty,'') as manufactureparty,d.op,d.opref,d.orderqty,isnull(p.huid,'') as huid,isnull(p.qty,0) as qty from ord_orderbomdet d 
                                        left join PRD_ProdLineLocationDet p
                                        on d.item=p.item and d.orderno=p.orderno
                                        and d.op=p.op and d.opref=p.opref
                                        where d.isscanhu=1 and d.OrderNo='" +searchModel.OrderNo+"'";
            Sb.Append(whereStatement);

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                Sb.Append(string.Format(" and d.Item = '{0}'", searchModel.Item));
            }
            if (!string.IsNullOrEmpty(searchModel.HuId))
            {
                Sb.Append(string.Format(" and p.HuId = '{0}'", searchModel.HuId));
            }

            return Sb.ToString();

        }


    }
}
