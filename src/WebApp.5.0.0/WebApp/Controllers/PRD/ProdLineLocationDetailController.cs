using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.PRD;
using com.Sconit.Entity.PRD;
using com.Sconit.Web.Models;
using System.Text;

namespace com.Sconit.Web.Controllers.PRD
{
    public class ProdLineLocationDetailController : WebAppBaseController
    {
        //
        // GET: /ProdLineLocationDetail/
        private static string selectCountStatement = "select count(*) from ProductLineLocationDetail as p";
        private static string selectStatement = "select p from ProductLineLocationDetail as p";

        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_PRD_ProdLineLocationDet")]
        public ActionResult List(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            this.ProcessSearchModel(command, searchModel);
            if (string.IsNullOrEmpty(searchModel.ProductLine) && string.IsNullOrEmpty(searchModel.OrderNo))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseInputProductionLineCodeOrOrderNumber);
            }
            else
            {
                 TempData["_AjaxMessage"] = "";
            }
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);

            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PRD_ProdLineLocationDet")]
        public ActionResult _AjaxList(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.ProductLine) && string.IsNullOrEmpty(searchModel.OrderNo))
            {
                return PartialView(new GridModel(new List<ProductLineLocationDetail>()));
            }
            string sql = this.PrepareSearchStatement(command, searchModel);
            //this.queryMgr.FindAllWithNativeSql<object[]>(sql);
            IList<object[]> objectList = this.queryMgr.FindAllWithNativeSql<object[]>(sql + "select  * from #temp where ID_Num   between   " + (command.Page - 1) * command.PageSize + "   and   " + command.Page* command.PageSize + ";drop table #temp ;");

            IList<object> CountList = this.queryMgr.FindAllWithNativeSql<object>(sql + "select  count(*) as counts from #temp ;drop table #temp;");

            //if (objectList.Count > 0) {
            //    this.queryMgr.FindAllWithNativeSql<object[]>("drop table #temp ;");
            //}

            IList<ProductLineLocationDetail> ProductLineLocationDetailList = new List<ProductLineLocationDetail>();
            ProductLineLocationDetailList = (from tak in objectList
                                             select new ProductLineLocationDetail
                                 {
                                     ProductLine = (string)tak[1],
                                     OrderNo = (string)tak[2],
                                     Operation = (int?)tak[3],
                                     OpReference = (string)tak[4],
                                     TraceCode = (string)tak[5],
                                     Item = (string)tak[6],
                                     ItemDescription = (string)tak[7],
                                     ReferenceItemCode = (string)tak[8],
                                     HuId = (string)tak[9],
                                     LotNo = (string)tak[10],
                                     Qty = (decimal)tak[11],
                                      BackFlushQty= (decimal)tak[12],
                                     VoidQty = (decimal)tak[13],
                                     CreateUserName = (string)tak[14],
                                     CreateDate = (DateTime)tak[15],
                                     LocationFrom=(string)tak[16],
                                    
                                 }).ToList();
         
            GridModel<ProductLineLocationDetail> gridList = new GridModel<ProductLineLocationDetail>();
            gridList.Data = ProductLineLocationDetailList;
            gridList.Total = Convert.ToInt32(CountList[0]);
            return PartialView(gridList);
        }

        private string PrepareSearchStatement(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            StringBuilder Sb = new StringBuilder();
            string whereStatement = @"select identity(int, 1,1) as id_Num,* into #temp from ( select  prodline,orderno,op,opref,tracecode,item,itemdesc,refitemcode,huid,lotno,sum(qty) as qty,sum(bfqty) as bfqty,sum(voidqty) as voidqty,createusernm,createdate,LocFrom 
from PRD_ProdLineLocationDet where isClose=0 "  ;
            Sb.Append(whereStatement);
        string later="group by prodline,orderno,op,opref,tracecode,item,itemdesc,refitemcode,huid,lotno,createusernm,createdate,LocFrom ) as result;";
      
        if (!string.IsNullOrEmpty(searchModel.ProductLine)) {
            Sb.Append(string.Format(" and prodline = '{0}'", searchModel.ProductLine));
        }
        if (!string.IsNullOrEmpty(searchModel.OrderNo))
        {
            Sb.Append(string.Format(" and OrderNo = '{0}'", searchModel.OrderNo));
        }
        if (!string.IsNullOrEmpty(searchModel.Item))
        {
            Sb.Append(string.Format(" and Item = '{0}'", searchModel.Item));
        }

        Sb.Append(later);
        return Sb.ToString();

        }

    }
}
