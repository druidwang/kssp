using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityCategoryMgr : FacilityCategoryBaseMgr, IFacilityCategoryMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityCategoryMgrE : com.Sconit.Facility.Service.Impl.FacilityCategoryMgr, IFacilityCategoryMgrE
    {
    }
}

#endregion Extend Class