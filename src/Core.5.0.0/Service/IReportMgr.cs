using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Castle.Windsor;

namespace com.Sconit.Service
{
    public interface IReportMgr
    {
        List<object> GetRealTimeLocationDetail(string procedureName, SqlParameter[] parameters);
        List<object> GetHistoryInvAjaxPageData(string procedureName, SqlParameter[] parameters);
        List<object> GetInventoryAgeAjaxPageData(string procedureName, SqlParameter[] parameters);
        List<object> GetReportTransceiversAjaxPageData(string procedureName, SqlParameter[] parameters);
        List<object> GetAuditInspectionPageData(string procedureName, SqlParameter[] parameters);//GetAuditInspectionPageData
        List<object> GetHuAjaxPageData(string procedureName, SqlParameter[] parameters);
    }
}
