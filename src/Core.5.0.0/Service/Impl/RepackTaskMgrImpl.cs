using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.WMS;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using System.Data;
using System.Data.SqlClient;
using Castle.Services.Transaction;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class RepackTaskMgrImpl : BaseMgr, IRepackTaskMgr
    {
        public IGenericMgr genericMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public void AssignRepackTask(IList<RepackTask> repackTaskList, string assignUser)
        {
            if (repackTaskList != null && repackTaskList.Count > 0)
            {
                User lastModifyUser = SecurityContextHolder.Get();
                User user = genericMgr.FindById<User>(Convert.ToInt32(assignUser));
                foreach (RepackTask p in repackTaskList)
                {
                    p.RepackUserId = user.Id;
                    p.RepackUserName = user.FullName;
                    p.LastModifyDate = DateTime.Now;
                    p.LastModifyUserId = lastModifyUser.Id;
                    p.LastModifyUserName = lastModifyUser.FullName;
                    genericMgr.Update(p);
                }

            }
        }

        public IList<Hu> SuggestRepackHu(int repackTaskId)
        {
            DataSet ds = null;
            User user = SecurityContextHolder.Get();
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@RepackTaskId", SqlDbType.Int);
            paras[0].Value = repackTaskId;

            try
            {
                ds = this.genericMgr.GetDatasetByStoredProcedure("USP_WMS_SuggestRepackHu", paras);

                if (ds != null && ds.Tables != null && ds.Tables[0] != null
                    && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow msg in ds.Tables[0].Rows)
                    {
                        if (msg[0].ToString() == "0")
                        {
                            MessageHolder.AddInfoMessage((string)msg[1]);
                        }
                        else if (msg[0].ToString() == "1")
                        {
                            MessageHolder.AddWarningMessage((string)msg[1]);
                        }
                        else
                        {
                            MessageHolder.AddErrorMessage((string)msg[1]);
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        MessageHolder.AddErrorMessage(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        MessageHolder.AddErrorMessage(ex.InnerException.Message);
                    }
                }
                else
                {
                    MessageHolder.AddErrorMessage(ex.Message);
                }

                return null;
            }

            if (ds != null && ds.Tables != null && ds.Tables[1] != null
                  && ds.Tables[1].Rows != null)
            {
                if (ds.Tables[1].Rows.Count > 0)
                {
                    IList<object> huIdList = new List<object>();
                    foreach (DataRow hu in ds.Tables[1].Rows)
                    {
                        if (!huIdList.Contains(hu[0]))
                        {
                            huIdList.Add(hu[0]);
                        }
                    }

                    return genericMgr.FindAllIn<Hu>("from Hu where huId in(?", huIdList);
                }
                else
                {
                    throw new BusinessException("没有推荐的翻箱条码。");
                }
            }
            else
            {
                throw new TechnicalException("返回的条码信息为空。");
            }
        }

        public void ProcessRepackResult(int repackTaskId, IList<string> repackResultIn, IList<string> repackResultOut, DateTime? effectiveDate)
        {
            User user = SecurityContextHolder.Get();
            SqlParameter[] paras = new SqlParameter[6];
            DataTable repackResultInTable = new DataTable();
            repackResultInTable.Columns.Add("HuId", typeof(string));
            foreach (var hu in repackResultIn)
            {
                DataRow row = repackResultInTable.NewRow();
                row[0] = hu;
                repackResultInTable.Rows.Add(row);
            }

            DataTable repackResultOutTable = new DataTable();
            repackResultOutTable.Columns.Add("HuId", typeof(string));
            foreach (var hu in repackResultOut)
            {
                DataRow row = repackResultOutTable.NewRow();
                row[0] = hu;
                repackResultOutTable.Rows.Add(row);
            }
            paras[0] = new SqlParameter("@RepackTaskId", SqlDbType.Int);
            paras[0].Value = repackTaskId;
            paras[1] = new SqlParameter("@RepackResultIn", SqlDbType.Structured);
            paras[1].Value = repackResultInTable;
            paras[2] = new SqlParameter("@RepackResultOut", SqlDbType.Structured);
            paras[2].Value = repackResultOutTable;
            paras[3] = new SqlParameter("@CreateUserId", SqlDbType.Int);
            paras[3].Value = user.Id;
            paras[4] = new SqlParameter("@CreateUserNm", SqlDbType.VarChar);
            paras[4].Value = user.FullName;
            paras[5] = new SqlParameter("@EffDate", SqlDbType.DateTime);
            paras[5].Value = effectiveDate;

            try
            {
                DataSet ds = this.genericMgr.GetDatasetByStoredProcedure("USP_WMS_ProcessRepackResult", paras);

                if (ds != null && ds.Tables != null && ds.Tables[0] != null
                    && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow msg in ds.Tables[0].Rows)
                    {
                        if (msg[0].ToString() == "0")
                        {
                            MessageHolder.AddInfoMessage((string)msg[1]);
                        }
                        else if (msg[0].ToString() == "1")
                        {
                            MessageHolder.AddWarningMessage((string)msg[1]);
                        }
                        else
                        {
                            MessageHolder.AddErrorMessage((string)msg[1]);
                        }
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        MessageHolder.AddErrorMessage(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        MessageHolder.AddErrorMessage(ex.InnerException.Message);
                    }
                }
                else
                {
                    MessageHolder.AddErrorMessage(ex.Message);
                }

                return;
            }
        }
    }
}
