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
    public class WarningColorController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MRP_WarningColor_View")]
        public ActionResult Index()
        {
            var entityPre = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.ProdLineWarningColors, false);
            var colors = entityPre.Split('-');

            WarningColor warningColor = new WarningColor();
            warningColor.Range1 = float.Parse(colors[1]);
            warningColor.Range2 = float.Parse(colors[2]);
            warningColor.Range3 = float.Parse(colors[3]);
            warningColor.Range4 = float.Parse(colors[4]);
            warningColor.Range5 = float.Parse(colors[5]);
            warningColor.Range6 = float.Parse(colors[6]);

            return View(warningColor);
        }

        [SconitAuthorize(Permissions = "Url_MRP_WarningColor_View")]
        public ActionResult Index(WarningColor warningColor)
        {
            if (ModelState.IsValid)
            {
                EntityPreference entityPreference = this.genericMgr.FindById<EntityPreference>((int)Entity.SYS.EntityPreference.CodeEnum.ProdLineWarningColors);
                StringBuilder str = new StringBuilder("0-");
                str.Append(warningColor.Range1);
                str.Append("-");
                str.Append(warningColor.Range2);
                str.Append("-");
                str.Append(warningColor.Range3);
                str.Append("-");
                str.Append(warningColor.Range4);
                str.Append("-");
                str.Append(warningColor.Range5);
                str.Append("-");
                str.Append(warningColor.Range6);
                str.Append("-99999999");
                entityPreference.Value = str.ToString();
                this.genericMgr.UpdateWithTrim(entityPreference);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedSuccessfully);
            }
            return View(warningColor);
        }
    }
}
