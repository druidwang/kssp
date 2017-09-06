using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Entity.SCM;
using com.Sconit.Service;
using com.Sconit.Entity.SYS;
using com.Sconit.CodeMaster;

namespace com.Sconit.Web.Controllers.SCM
{
    public class PickStrategyController : WebAppBaseController
    {
        //
        // GET: /PickStrategy/
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the PickStrategy security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_PickStrategy_View")]
        public ActionResult Index()
        {

            IList<PickStrategy> pickStrategylist = genericMgr.FindAll<PickStrategy>();
            foreach (var item in pickStrategylist)
            {
                item.ShipStrategyDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.ShipStrategy, ((int)item.ShipStrategy).ToString());
                item.PickOddOptionDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.PickOddOption, ((int)item.OddOption).ToString());

            }
           
            return View(pickStrategylist);
        }

      
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_PickStrategy_View")]
        public ActionResult Edit(string id)
        {
            PickStrategy pickStrategy = genericMgr.FindById<PickStrategy>(id);
            return View(pickStrategy);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_PickStrategy_View")]
        public ActionResult Edit(PickStrategy pickStrategy)
        {
            genericMgr.UpdateWithTrim(pickStrategy);
            SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ModificateSuccessfully);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_PickStrategy_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_PickStrategy_View")]
        public ActionResult New(PickStrategy pickStrategy)
        {
            if (ModelState.IsValid)
            {
                genericMgr.CreateWithTrim(pickStrategy);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_AddedSuccessfully);
                return RedirectToAction("Index");
            }
            else
            {
                return View(pickStrategy);
            }
        }
    }
}
