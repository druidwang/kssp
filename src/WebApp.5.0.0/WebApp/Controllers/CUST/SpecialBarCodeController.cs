using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.CUST;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.CUST;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.Exception;
using com.Sconit.PrintModel;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.MD;

namespace com.Sconit.Web.Controllers.CUST
{
    public class SpecialBarCodeController : WebAppBaseController
    {

        [SconitAuthorize(Permissions = "Url_CUST_SpecialBarCode_Index")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_CUST_SpecialBarCode_Index")]
        public string Print(string code, int type, bool isPrintBin)
        {
            var specialBarCodeList = GetSpecialBarCodeList(code, type, isPrintBin);
            IList<object> data = new List<object>();
            data.Add(specialBarCodeList);
            return reportGen.WriteToFile("BarCode2DSpecial.xls", data);
        }

        [SconitAuthorize(Permissions = "Url_CUST_SpecialBarCode_Index")]
        public void Export(string code, int type, bool isPrintBin)
        {
            try
            {
                var specialBarCodeList = GetSpecialBarCodeList(code, type, isPrintBin);
                IList<object> data = new List<object>();
                data.Add(specialBarCodeList);
                reportGen.WriteToClient("BarCode2DSpecial.xls", data, code + "_BarCode2DSpecial.xls");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
        }

        private List<SpecialBarCode> GetSpecialBarCodeList(string code, int type, bool isPrintBin)
        {
            var specailBarCodeList = new List<SpecialBarCode>();
            try
            {
                if (type == 4)
                {
                    var flowMaster = this.genericMgr.FindById<FlowMaster>(code);
                    var specailBarCode = new SpecialBarCode();
                    specailBarCode.Code = string.Format("$F{0}", flowMaster.Code);
                    specailBarCode.Desc1 = string.Format(Resources.EXT.ControllerLan.Con_TransferFlow, flowMaster.Code);
                    specailBarCode.Desc2 = flowMaster.Description;
                    specailBarCodeList.Add(specailBarCode);
                }
                else if (type == 5)
                {
                    if (isPrintBin)
                    {
                        var binList = this.genericMgr.FindAll<LocationBin>("from LocationBin where Location =?", code);
                        foreach (var bin in binList)
                        {
                            var specailBarCode = new SpecialBarCode();
                            specailBarCode.Code = string.Format("$B{0}", bin.Code);
                            specailBarCode.Desc1 = string.Format(Resources.EXT.ControllerLan.Con_Bin, bin.Code);
                            specailBarCode.Desc2 = bin.Name;
                            specailBarCodeList.Add(specailBarCode);
                        }
                    }
                    else
                    {
                        var location = this.genericMgr.FindById<Location>(code);
                        var specailBarCode = new SpecialBarCode();
                        specailBarCode.Code = string.Format("$L{0}", location.Code);
                        specailBarCode.Desc1 = string.Format(Resources.EXT.ControllerLan.Con_Location_1, location.Code);
                        specailBarCode.Desc2 = location.Name;
                        specailBarCodeList.Add(specailBarCode);
                    }
                }
                else if (type == 6)
                {
                    var bin = this.genericMgr.FindById<LocationBin>(code);
                    var specailBarCode = new SpecialBarCode();
                    specailBarCode.Code = string.Format("$B{0}", bin.Code);
                    specailBarCode.Desc1 = string.Format(Resources.EXT.ControllerLan.Con_Bin, bin.Code);
                    specailBarCode.Desc2 = bin.Name;
                    specailBarCodeList.Add(specailBarCode);
                }
            }
            catch (Exception)
            {

            }
            return specailBarCodeList;
        }
    }
}
