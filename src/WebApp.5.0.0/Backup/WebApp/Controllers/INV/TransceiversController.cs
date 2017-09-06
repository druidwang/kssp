using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models.SearchModels.ORD;
using com.Sconit.Web.Models;
using com.Sconit.Entity.ORD;
using System.Text;
using com.Sconit.Service;
using com.Sconit.Web.Models.SearchModels.BIL;
using com.Sconit.Entity.BIL;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.VIEW;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Web.Models.ReportModels;
using com.Sconit.CodeMaster;


namespace com.Sconit.Web.Controllers.INV
{
    public class TransceiversController : ReportBaseController
    {

        //public IGenericMgr genericMgr { get; set; }

        #region public
        public ActionResult Index()
        {
            TransceiversSearchModel serch=new TransceiversSearchModel ();
            serch.TypeLocation="0";
            TempData["Display"] = "0";
            TempData["TransceiversSearchModel"] = serch;
            return View();
        }
  

        [GridAction]
        [SconitAuthorize(Permissions = "Menu.Transceivers.Statements")]
        public ActionResult List(GridCommand command, TransceiversSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.EndDate.ToString()))
            {
                searchModel.EndDate = DateTime.Now;
            }
            SearchCacheModel SearchCacheModel= this.ProcessSearchModel(command,searchModel);
            searchModel = SearchCacheModel.SearchObject as TransceiversSearchModel;
           
            if (!string.IsNullOrEmpty(searchModel.SAPLocation))
            TempData["SAPLocation"] = searchModel.SAPLocation;

            TempData["Display"] = searchModel.Level;

            //明细列表中所有参数
            ViewBag.BeginDate = searchModel.BeginDate;
            ViewBag.EndDate = searchModel.EndDate;
            ViewBag.TypeLocation = searchModel.TypeLocation;
            ViewBag.Level = searchModel.Level;
            ViewBag.SAPLocation = searchModel.SAPLocation;


            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);

            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (string.IsNullOrEmpty(ViewBag.Location))
                ViewBag.Location = Resources.View.LocationDetailView.LocationDetailView_Location;
            else
            ViewBag.Location = ViewBag.Location;



            if (string.IsNullOrEmpty(searchModel.BeginDate.ToString()))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_StartToTimeCanNotBeEmpty);
                return View();
            }
            if (searchModel.TypeLocation == "0")
            {
                if (string.IsNullOrEmpty(searchModel.Level))
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_SummaryToCanNotBeEmpty);
                    return View();
                }
                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo) && string.IsNullOrEmpty(searchModel.locationFrom) && string.IsNullOrEmpty(searchModel.locationTo)
                     && string.IsNullOrEmpty(searchModel.regionFrom) && string.IsNullOrEmpty(searchModel.regionTo))
                    
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_ConditionSearchNeeded);
                    return View();
                }
              


                if (locationList.Count > 200)
                {
                    if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo))
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_ItemCodeCanNotBeEmpty);
                        return View();
                    }
                }
                if (itemList.Count > 200)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PartExceedTwoHundred);
                    return View();
                }
                if (string.IsNullOrEmpty(searchModel.itemFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.itemTo))
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_CanNotInputSecondItemWhenErrorFirstItemIsEmpty);
                        return View();
                    }
                }
                if (string.IsNullOrEmpty(searchModel.locationFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.locationTo))
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_CanNotInputSecondLocationWhenErrorFirstLocationIsEmpty);
                        return View();
                    }
                }

                if (string.IsNullOrEmpty(searchModel.regionFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.regionTo))
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_CanNotInputSecondAreaWhenErrorFirstAreaIsEmpty);
                        return View();
                    }
                }

             
            }
            else
            {
                if (string.IsNullOrEmpty(searchModel.SAPLocation))
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_SAPLocationCanNotBeEmpty);
                    return View();
                }
            }
         

            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Menu.Transceivers.Statements")]
        public ActionResult _AjaxList(GridCommand command, TransceiversSearchModel searchModel)
        {

            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation, searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            if (locationList.Count > 200)
            {
                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo))
                {
                    return PartialView(new GridModel(new List<Transceivers>()));
                }
            }
            if (itemList.Count > 200)
            {
                return PartialView(new GridModel(new List<Transceivers>()));
            }
            if (string.IsNullOrEmpty(searchModel.BeginDate.ToString()))
            {
                return PartialView(new GridModel(new List<Transceivers>()));
            }

            if (searchModel.TypeLocation == "0")
            {
                if (string.IsNullOrEmpty(searchModel.Level))
                {
                    return PartialView(new GridModel(new List<Transceivers>()));
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom) && string.IsNullOrEmpty(searchModel.itemTo) && string.IsNullOrEmpty(searchModel.locationFrom) && string.IsNullOrEmpty(searchModel.locationTo)
                    && string.IsNullOrEmpty(searchModel.regionFrom) && string.IsNullOrEmpty(searchModel.regionTo)
                     )
                {
                    return PartialView(new GridModel(new List<Transceivers>()));
                }

                if (string.IsNullOrEmpty(searchModel.itemFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.itemTo))
                    {
                        return PartialView(new GridModel(new List<Transceivers>()));
                    }
                }
                if (string.IsNullOrEmpty(searchModel.locationFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.locationTo))
                    {
                        return PartialView(new GridModel(new List<Transceivers>()));
                    }
                }

                if (string.IsNullOrEmpty(searchModel.regionFrom))
                {
                    if (!string.IsNullOrEmpty(searchModel.regionTo))
                    {
                        return PartialView(new GridModel(new List<Transceivers>()));
                    }
                }

            }
            else
            {
                if (string.IsNullOrEmpty(searchModel.SAPLocation))
                {
                    return PartialView(new GridModel(new List<Transceivers>()));
                }
            }
           
     
           


            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<Transceivers> gridModel = GetReportTransceiversAjaxPageData<Transceivers>(reportSearchStatementModel);

          

            return PartialView(gridModel);
        }

       

        #endregion

        #region private

        private ReportSearchStatementModel PrepareSearchStatement(GridCommand command, TransceiversSearchModel searchModel)
        {
            ReportSearchStatementModel reportSearchStatementModel = new ReportSearchStatementModel();
            reportSearchStatementModel.ProcedureName = "USP_Report_RecSendDeposit";

            IList<Location> locationList = GetReportLocations(searchModel.SAPLocation,searchModel.plantFrom, searchModel.plantTo, searchModel.regionFrom, searchModel.regionTo, searchModel.locationFrom, searchModel.locationTo);
            string location = string.Empty;
            foreach (var lcoList in locationList)
            {
                if (location == string.Empty)
                {
                    location = lcoList.Code;
                }
                else
                {
                    location += "," + lcoList.Code;
                }
            }


            IList<Item> itemList = GetReportItems(searchModel.itemFrom, searchModel.itemTo);
            string item = string.Empty;
            foreach (var ite in itemList)
            {
                if (item == string.Empty)
                {
                    item = ite.Code;
                }
                else
                {
                    item += "," + ite.Code;
                }
            }
           

            SqlParameter[] parameters = new SqlParameter[9];

            parameters[0] = new SqlParameter("@Locations", SqlDbType.VarChar, 8000);
            parameters[0].Value = location;

            parameters[1] = new SqlParameter("@Items", SqlDbType.VarChar, 8000);
            parameters[1].Value = item;

            parameters[2] = new SqlParameter("@SortDesc", SqlDbType.VarChar, 50);
            parameters[2].Value = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            parameters[3] = new SqlParameter("@PageSize", SqlDbType.Int);
            parameters[3].Value = command.PageSize;

            parameters[4] = new SqlParameter("@Page", SqlDbType.Int);
            parameters[4].Value = command.Page;

            parameters[5] = new SqlParameter("@SummaryLevel", SqlDbType.VarChar, 50);
            parameters[5].Value = searchModel.Level;

            parameters[6] = new SqlParameter("@BeginDate", SqlDbType.DateTime);
            parameters[6].Value = searchModel.BeginDate;

            parameters[7] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            parameters[7].Value = searchModel.EndDate;

            

            parameters[8] = new SqlParameter("@IsSummaryBySAPLoc", SqlDbType.Bit);
            parameters[8].Value = searchModel.TypeLocation == "1" ? true : false; ;

            
            reportSearchStatementModel.Parameters = parameters;

            return reportSearchStatementModel;
        }
        #endregion

        public ActionResult Edit(string IOType, string Item, string Location, string BeginDate, string EndDate, string TypeLocation, string Level, string SAPLocation)
        {
            IList<object> param = new List<object>();
            IList<Region> listRegion=null;
            IList<Location> listLocation = null;
            string locationStr = string.Empty;
            string hql = string.Empty;
            //如果是les库位
            if (TypeLocation == "0")
            {
                //是按区域来查的
                if (Level == "1")
                {
                    listLocation = genericMgr.FindAll<Location>(" from Location l where l.Region=? ", new object[]{Location} );
                }
                //根据分厂来查
                else  if (Level == "2")
                {
                    listRegion = genericMgr.FindAll<Region>(" from Region r where r.Workshop=? ", new object[] { Location });
                    string RegionStr = string.Empty;
                    foreach (Region loc in listRegion)
                    {
                        if (string.IsNullOrEmpty(RegionStr))
                            RegionStr = loc.Code;
                        else
                            RegionStr += "," + loc.Code;
                    }
                    listLocation = genericMgr.FindAll<Location>(" from Location l where l.Region in (?) ", new object[] { RegionStr });
                }
                //根据工厂
                else if (Level == "3")
                {
                    listRegion = genericMgr.FindAll<Region>(" from Region r where r.Plant=? ", new object[] { Location });
                    string RegionStr = string.Empty;
                    foreach (Region loc in listRegion)
                    {
                        if (string.IsNullOrEmpty(RegionStr))
                            RegionStr = loc.Code;
                        else
                            RegionStr += "," + loc.Code;
                    }
                    listLocation = genericMgr.FindAll<Location>(" from Location l where l.Region in (?) ", new object[] { RegionStr });
                }
                else
                {
                    locationStr = Location;
                }



            }
                //sap库位
            else{
                listLocation = genericMgr.FindAll<Location>(" from Location l where l.SAPLocation = ? ", new object[] { Location });

            }

            if (listLocation != null)
            {
                foreach (Location loc in listLocation)
                {

                    if (string.IsNullOrEmpty(hql))
                    {
                        if (IOType == "0")
                            hql = " from LocationTransaction where LocationTo in (? ";
                        else
                            hql = " from LocationTransaction where LocationFrom in (? ";
                    }
                    else
                    {
                        hql += ",?";
                    }
                    param.Add(loc.Code);
                }
                hql += ")";
            }
            
                hql += " and Item=?  and EffectiveDate between ? and ?  and IOType=?";
          
            param.Add(Item);
            param.Add(Convert.ToDateTime(BeginDate));
            param.Add(Convert.ToDateTime(EndDate));
            param.Add(IOType);

          IList<LocationTransaction> List=genericMgr.FindAll<LocationTransaction>(hql,param.ToArray());

          
            return View(List);
        }
       
    }
}
