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

namespace com.Sconit.Web.Controllers.MRP
{
    public class FlowClassifyController : WebAppBaseController
    {
        [SconitAuthorize(Permissions = "URl_MRP_FlowClassify_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "URl_MRP_FlowClassify_View")]
        public ActionResult _SelectFlowClassifyList()
        {
            //IList<FlowClassify> newsflowClassifyList = new List<FlowClassify>();
            var mrpFlowClassifys = genericMgr.FindAll<Entity.MRP.MD.FlowClassify>();
            var flowMasterList = genericMgr.FindAll<FlowMaster>(" from FlowMaster as f where f.Type=? and f.ResourceGroup=? ",
                new object[] { CodeMaster.OrderType.Production, CodeMaster.ResourceGroup.EX });

            var flowClassifys = new List<FlowClassify>();

            //如果FlowClassify中没有而flowMasterList有的增加进来
            foreach (var flowMaster in flowMasterList)
            {
                var flowClassify = new FlowClassify();
                flowClassify.Flow = flowMaster.Code;
                flowClassifys.Add(flowClassify);
            }

            Type tClass = typeof(FlowClassify);
            PropertyInfo[] pClass = tClass.GetProperties();

            foreach (var mrpFlowClassify in mrpFlowClassifys)
            {
                var flowClassify = flowClassifys.FirstOrDefault(p => p.Flow == mrpFlowClassify.Flow);

                if (flowClassify != null)
                {
                    foreach (PropertyInfo pc in pClass)
                    {
                        try
                        {
                            if (pc.Name == mrpFlowClassify.Code)
                            {
                                pc.SetValue(flowClassify, true, null);
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return PartialView(new GridModel(flowClassifys));
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "URl_MRP_FlowClassify_View")]
        public string _SavaFlowClassify(string flows, string classify01s, string classify02s, string classify03s, string classify04s, string classify05s,
             string classify06s, string classify07s, string classify08s, string classify09s, string classify10s)
        {
            try
            {
                genericMgr.Delete<Entity.MRP.MD.FlowClassify>(genericMgr.FindAll<Entity.MRP.MD.FlowClassify>());

                string[] flowArray = flows.Split(',');
                string[] classify01Array = classify01s.Split(',');
                string[] classify02Array = classify02s.Split(',');
                string[] classify03Array = classify03s.Split(',');
                string[] classify04Array = classify04s.Split(',');
                string[] classify05Array = classify05s.Split(',');
                string[] classify06Array = classify06s.Split(',');
                string[] classify07Array = classify07s.Split(',');
                string[] classify08Array = classify08s.Split(',');
                string[] classify09Array = classify09s.Split(',');
                string[] classify10Array = classify10s.Split(',');

                for (int i = 0; i < flowArray.Length; i++)
                {
                    if (Convert.ToBoolean(classify01Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify01";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify02Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify02";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify03Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify03";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify04Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify04";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify05Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify05";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify06Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify06";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify07Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify07";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify08Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify08";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify09Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify09";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                    if (Convert.ToBoolean(classify10Array[i]))
                    {
                        var flowClassify = new Entity.MRP.MD.FlowClassify();
                        flowClassify.Flow = flowArray[i];
                        flowClassify.Code = "Classify10";
                        genericMgr.CreateWithTrim(flowClassify);
                    }
                }
                return Resources.EXT.ControllerLan.Con_ExtrusionFlowFunctionClassificationSavedSuccessfully;
            }
            catch (Exception ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write((ex.InnerException).InnerException);
                return string.Empty;
            }
        }
    }
}
