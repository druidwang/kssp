using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Entity.MD;

namespace com.Sconit.Web.Controllers
{
    public class ReportBaseController : WebAppBaseController
    {
        /// <summary>
        /// 根据工厂，区域或者库位获取需要的库位数据
        /// </summary>
        /// <param name="plantFrom">从***工厂</param>
        /// <param name="plantTo">到***工厂</param>
        /// <param name="regionFrom">从***区域</param>
        /// <param name="regionTo">到***区域</param>
        /// <param name="locationFrom">从***库位</param>
        /// <param name="locationTo">到***库位</param>
        /// <returns>目标库位</returns>
        public IList<Location> GetReportLocations(string sapLocation, string plantFrom, string plantTo, string regionFrom, string regionTo, string locationFrom, string locationTo)
        {
            IList<Location> locationList = new List<Location>();
            IList<Region> regionList = new List<Region>();
            if (!string.IsNullOrEmpty(sapLocation))
            {
                locationList = this.queryMgr.FindAll<Location>("from Location l where l.SAPLocation = ? ", new object[] { sapLocation });
                ViewBag.Location = Resources.View.LocationDetailView.LocationDetailView_SAPLocation;
            }
            else if (!string.IsNullOrEmpty(locationFrom))
            {
                if (!string.IsNullOrEmpty(locationTo))
                {
                    locationList = this.queryMgr.FindAll<Location>("from Location l where l.Code between ? and ?", new object[] { locationFrom, locationTo });
                }
                else
                {
                    locationList = this.queryMgr.FindAll<Location>("from Location l where l.Code = ?", new object[] { locationFrom });
                }
                ViewBag.Location = Resources.View.LocationDetailView.LocationDetailView_Location;
            }
            else if (!string.IsNullOrEmpty(regionFrom))
            {
                if (!string.IsNullOrEmpty(regionTo))
                {
                    regionList = this.queryMgr.FindAll<Region>("from Region r where r.Code between ? and ?", new object[] { regionFrom, regionTo });
                }
                else
                {
                    regionList = this.queryMgr.FindAll<Region>("from Region r where r.Code = ? ", new object[] { regionFrom });
                }
                ViewBag.Location = Resources.View.LocationDetailView.LocationDetailView_region;
            }

            if (regionList.Count > 0 && locationList.Count == 0)
            {
                string hql = string.Empty;
                IList<object> paras = new List<object>();
                foreach (var region in regionList)
                {
                    if (hql == string.Empty)
                    {
                        hql = "from Location l where l.Region in (?";
                    }
                    else
                    {
                        hql += ", ?";
                    }
                    paras.Add(region.Code);
                }
                hql += ")";
                locationList = this.queryMgr.FindAll<Location>(hql, paras.ToArray());
            }

            return locationList;
        }

        public IList<Item> GetReportItems(string itemFrom, string itemTo)
        {
            IList<Item> itemList = new List<Item>();
            if (!string.IsNullOrEmpty(itemFrom))
            {
                if (!string.IsNullOrEmpty(itemTo))
                {
                    itemList = this.queryMgr.FindAll<Item>("from Item i where i.Code between ? and ?", new object[] { itemFrom, itemTo });
                }
                else
                {
                    itemList = this.queryMgr.FindAll<Item>("from Item i where i.Code = ? ", new object[] { itemFrom });
                }
                if (string.IsNullOrEmpty(ViewBag.Location))
                ViewBag.Location = Resources.View.LocationDetailView.LocationDetailView_Location;
            }

            return itemList;
        }

    }
}