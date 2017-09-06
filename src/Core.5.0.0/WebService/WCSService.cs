using System.Web.Services;
using com.Sconit.Service;
using com.Sconit.Entity;
using System.Collections.Generic;
using System;

namespace com.Sconit.WebService
{
    [WebService(Namespace = "http://com.Sconit.WebService.WCSService/")]
    public class WCSService : BaseWebService
    {
        [WebMethod]
        public bool WCSTaskCallBack(int taskId, int completeStatus, string message)
        {
            throw new NotImplementedException();
        }

        [WebMethod]
        public bool EquipmentFaultFeedBack(int equipmentNo, string faultDesc)
        {
            throw new NotImplementedException();
          
        }    
    }
}
