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

namespace com.Sconit.Service.Impl
{
    public class PickTaskMgrImpl : BaseMgr, IPickTaskMgr
    {
        public IGenericMgr genericMgr { get; set; }

        public void CreatePickTask(IDictionary<int, decimal> shipPlanIdAndQtyDic)
        {
            User user = SecurityContextHolder.Get();
            SqlParameter[] paras = new SqlParameter[3];
            DataTable createPickTaskTable = new DataTable();
            createPickTaskTable.Columns.Add("Id", typeof(Int32));
            createPickTaskTable.Columns.Add("PickQty", typeof(decimal));
            foreach (var i in shipPlanIdAndQtyDic)
            {
                DataRow row = createPickTaskTable.NewRow();
                row[0] = i.Key;
                row[1] = i.Value;
                createPickTaskTable.Rows.Add(row);
            }
            paras[0] = new SqlParameter("@CreatePickTaskTable", SqlDbType.Structured);
            paras[0].Value = createPickTaskTable;
            paras[1] = new SqlParameter("@CreateUserId", SqlDbType.Int);
            paras[1].Value = user.Id;
            paras[2] = new SqlParameter("@CreateUserNm", SqlDbType.VarChar);
            paras[2].Value = user.FullName;

            try
            {
                DataSet msgs = this.genericMgr.GetDatasetByStoredProcedure("USP_WMS_CreatePickTask", paras);

                if (msgs != null && msgs.Tables != null && msgs.Tables[0] != null
                    && msgs.Tables[0].Rows != null && msgs.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow msg in msgs.Tables[0].Rows)
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
            }
        }

        public void PorcessPickResult4PickQty(Dictionary<int, decimal> pickResults)
        {
            User user = SecurityContextHolder.Get();

            DataTable pickResultTable = new DataTable();
            pickResultTable.Columns.Add(new DataColumn("PickTaskId", typeof(int)));
            pickResultTable.Columns.Add(new DataColumn("HuId", typeof(string)));
            pickResultTable.Columns.Add(new DataColumn("Qty", typeof(decimal)));
            foreach (var pickResult in pickResults)
            {
                DataRow row = pickResultTable.NewRow();
                row[0] = pickResult.Key;
                row[1] = null;
                row[2] = pickResult.Value;
                pickResultTable.Rows.Add(row);
            }
            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("@PickResultTable", SqlDbType.Structured);
            paras[0].Value = pickResultTable;
            paras[1] = new SqlParameter("@CreateUserId", SqlDbType.Int);
            paras[1].Value = user.Id;
            paras[1] = new SqlParameter("@CreateUserNm", SqlDbType.VarChar);
            paras[1].Value = user.FullName;

            try
            {
                DataSet msgs = this.genericMgr.GetDatasetByStoredProcedure("USP_WMS_ProcessPickResult", paras);

                if (msgs != null && msgs.Tables != null && msgs.Tables[0] != null
                    && msgs.Tables[0].Rows != null && msgs.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow msg in msgs.Tables[0].Rows)
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
            }
        }

        public void PorcessPickResult4PickLotNoAndHu(Dictionary<int, List<string>> pickResults)
        {
            User user = SecurityContextHolder.Get();

            DataTable pickResultTable = new DataTable();
            pickResultTable.Columns.Add(new DataColumn("PickTaskId", typeof(int)));
            pickResultTable.Columns.Add(new DataColumn("HuId", typeof(string)));
            pickResultTable.Columns.Add(new DataColumn("Qty", typeof(decimal)));
            foreach (var pickResult in pickResults)
            {
                foreach (var huId in pickResult.Value)
                {
                    DataRow row = pickResultTable.NewRow();
                    row[0] = pickResult.Key;
                    row[1] = huId;
                    row[2] = 0;
                    pickResultTable.Rows.Add(row);
                }

            }
            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("@PickResultTable", SqlDbType.Structured);
            paras[0].Value = pickResultTable;
            paras[1] = new SqlParameter("@CreateUserId", SqlDbType.Int);
            paras[1].Value = user.Id;
            paras[2] = new SqlParameter("@CreateUserNm", SqlDbType.VarChar);
            paras[2].Value = user.FullName;

            try
            {
                DataSet msgs = this.genericMgr.GetDatasetByStoredProcedure("USP_WMS_ProcessPickResult", paras);

                if (msgs != null && msgs.Tables != null && msgs.Tables[0] != null
                    && msgs.Tables[0].Rows != null && msgs.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow msg in msgs.Tables[0].Rows)
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
            }
        }

        public void AssignPickTask(IList<PickTask> pickTaskList, string assignUser)
        {
            if (pickTaskList != null && pickTaskList.Count > 0)
            {
                User lastModifyUser = SecurityContextHolder.Get();
                User user = genericMgr.FindById<User>(Convert.ToInt32(assignUser));
                foreach(PickTask p in pickTaskList)
                {
                    p.PickUserId = user.Id;
                    p.PickUserName = user.FullName;
                    p.LastModifyDate = DateTime.Now;
                    p.LastModifyUserId = lastModifyUser.Id;
                    p.LastModifyUserName = lastModifyUser.FullName;
                    genericMgr.Update(p);
                }

            }
        }

        public void PorcessDeliverBarCode2Hu(string deliverBarCode, string HuId)
        {
            User user = SecurityContextHolder.Get();
            SqlParameter[] paras = new SqlParameter[4];
            paras[0] = new SqlParameter("@DeliverBarCode", SqlDbType.VarChar);
            paras[0].Value = deliverBarCode;
            paras[1] = new SqlParameter("@HuId", SqlDbType.VarChar);
            paras[1].Value = HuId;
            paras[2] = new SqlParameter("@CreateUserId", SqlDbType.Int);
            paras[2].Value = user.Id;
            paras[3] = new SqlParameter("@CreateUserNm", SqlDbType.VarChar);
            paras[3].Value = user.FullName;

            try
            {
                DataSet msgs = this.genericMgr.GetDatasetByStoredProcedure("USP_WMS_PorcessDeliverBarCode2Hu", paras);
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
            }
        }
    }
}
