using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Services.Transaction;
using com.Sconit.Web.Models.MRP;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.CUST;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.SCM;
using System.Reflection;
using com.Sconit.Entity.SYS;
using System.Text;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MiMasterDataController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MRP_WarningColor_View")]
        public ActionResult Index()
        {
            var entityPre1 = systemMgr.GetEntityPreferenceValue
                (Entity.SYS.EntityPreference.CodeEnum.MiCleanTime, false);
            var entityPre2 = systemMgr.GetEntityPreferenceValue
                (Entity.SYS.EntityPreference.CodeEnum.MiFilterCapacity, false);
            
            MiMasterData miMasterData = new MiMasterData();
            miMasterData.MiCleanTime = double.Parse(entityPre1);
            miMasterData.MiFilterCapacity = double.Parse(entityPre2);

            return View(miMasterData);
        }

        [SconitAuthorize(Permissions = "Url_MRP_WarningColor_View")]
        public ActionResult Index(MiMasterData miMasterData)
        {
            if (ModelState.IsValid)
            {
                EntityPreference entityPreference1 = this.genericMgr.FindById<EntityPreference>
                    ((int)Entity.SYS.EntityPreference.CodeEnum.MiCleanTime);
                EntityPreference entityPreference2 = this.genericMgr.FindById<EntityPreference>
                    ((int)Entity.SYS.EntityPreference.CodeEnum.MiFilterCapacity);
                entityPreference1.Value = miMasterData.MiCleanTime.ToString();
                entityPreference2.Value = miMasterData.MiFilterCapacity.ToString();
                this.genericMgr.UpdateWithTrim(entityPreference1);
                this.genericMgr.UpdateWithTrim(entityPreference2);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedSuccessfully);
            }
            return View(miMasterData);
        }
    }
}
