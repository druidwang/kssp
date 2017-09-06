using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.VIEW;
using com.Sconit.Service;
using com.Sconit.Entity.MD;

namespace com.Sconit.WebService
{
    [WebService(Namespace = "http://com.Sconit.WebService.PartyService/")]
    public class PartyService : BaseWebService
    {
        private IGenericMgr genericMgr
        {
            get
            {
                return GetService<IGenericMgr>();
            }
        }

        //private ISecurityMgr securityMgr
        //{
        //    get
        //    {
        //        return GetService<ISecurityMgr>();
        //    }
        //}

        [WebMethod]
        public IList<Region> GetAuthorizedRegion(string userCode)
        {
            IList < UserPermissionView> permissionList = securityMgr.GetUserPermissions(userCode, com.Sconit.CodeMaster.PermissionCategoryType.Region);

            throw new KeyNotFoundException();
        }

       
    }
}
