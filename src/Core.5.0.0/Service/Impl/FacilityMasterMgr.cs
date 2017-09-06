using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using com.Sconit.Facility.Service.Ext;
using com.Sconit.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using System.Linq;
using NHibernate;
using NPOI.SS.UserModel;
using com.Sconit.Entity.Exception;
using NPOI.HSSF.UserModel;
using com.Sconit.Utility;
using System.IO;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityMasterMgr : FacilityMasterBaseMgr, IFacilityMasterMgr
    {
        public IFacilityTransMgrE facilityTransMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public ITaskMgrE taskMgrE { get; set; }
        public IUserSubscriptionMgrE userSubscriptionMgrE { get; set; }
        public ITaskSubTypeMgrE taskSubTypeMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public IFacilityMaintainPlanMgrE facilityMaintainPlanMgrE { get; set; }
        public IFacilityAllocateMgrE facilityAllocateMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public void UpdateFacilityMasterAndCreateFacilityTrans(FacilityMaster facilityMaster, FacilityTrans facilityTrans, string status, string userCode)
        {

            #region ������
            facilityTrans.FacilityCategory = facilityMaster.Category;
            facilityTrans.AssetNo = facilityMaster.AssetNo;
            facilityTrans.FacilityName = facilityMaster.Name;
            facilityTransMgrE.CreateFacilityTrans(facilityTrans);
            #endregion

            #region ����״̬
            facilityMaster.Status = status;
            facilityMaster.LastModifyDate = DateTime.Now;
            facilityMaster.LastModifyUser = userCode;
            UpdateFacilityMaster(facilityMaster);
            #endregion

            #region �����ͼ�����Ҫ�Զ��رն�Ӧ��ISI����
            if (facilityTrans.TransType == FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_FINISH
                || facilityTrans.TransType == FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_INSPECT_FINISH)
            {
                DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskMstr));
                criteria.CreateAlias("TaskSubType", "t");
                criteria.Add(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS }));
                criteria.Add(Expression.Eq("t.Code", "SSGL"));
                criteria.Add(Expression.Eq("Desc2", facilityMaster.FCID));
                //  criteria.Add(Expression.Eq("f.Status", FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE));
                IList<TaskMstr> facilityTaskList = criteriaMgrE.FindAll<TaskMstr>(criteria);
                if (facilityTaskList != null && facilityTaskList.Count > 0)
                {
                    User user = userMgrE.LoadUser(userCode);
                    TaskMstr t = facilityTaskList.First(); //ֻ�ص�һ��
                    if (t.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
                    {
                        taskMgrE.ConfirmTask(t.Code, user);
                    }
                    taskMgrE.CompleteTask(t.Code, user);
                    taskMgrE.CloseTask(t.Code, user);
                }
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public override void CreateFacilityMaster(FacilityMaster facilityMaster)
        {
            base.CreateFacilityMaster(facilityMaster);

            #region ������

            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = facilityMaster.CreateUser;
            facilityTrans.EffDate = DateTime.Now.Date;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.ToChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_CREATE;

            facilityTrans.AssetNo = facilityMaster.AssetNo;
            facilityTrans.FacilityName = facilityMaster.Name;
            facilityTrans.FacilityCategory = facilityMaster.Category;
            facilityTransMgrE.CreateFacilityTrans(facilityTrans);
            #endregion


        }

        public FacilityTrans LoadFacilityMaintain(string fcId)
        {
            return LoadFacilityTrans(fcId, FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_START);
        }

        public FacilityTrans LoadFacilityFix(string fcId)
        {
            return LoadFacilityTrans(fcId, FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_FIX_START);
        }

        public FacilityTrans LoadFacilityInspect(string fcId)
        {
            return LoadFacilityTrans(fcId, FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_INSPECT_START);
        }

        public FacilityTrans LoadFacilityEnvelop(string fcId)
        {
            return LoadFacilityTrans(fcId, FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_ENVELOP);
        }

        private FacilityTrans LoadFacilityTrans(string fcId, string transType)
        {
            FacilityTrans facilityTrans = null;
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityTrans));
            criteria.Add(Expression.Eq("FCID", fcId));
            criteria.Add(Expression.Eq("TransType", transType));
            IList<FacilityTrans> facilityMasterList = criteriaMgrE.FindAll<FacilityTrans>(criteria);

            if (facilityMasterList != null && facilityMasterList.Count > 0)
            {
                facilityTrans = facilityMasterList.OrderByDescending(p => p.Id).First();
            }
            return facilityTrans;
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateFacilityMasterMaintain(FacilityMaster facilityMaster)
        {

            #region ���㱣������ڣ������
            if (facilityMaster.MaintainType != null && facilityMaster.MaintainType != FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_ONCE)
            {
                facilityMaster.MaintainTypePeriod = Convert.ToInt32(facilityMaster.MaintainType.Substring(0, facilityMaster.MaintainType.Length - 1));
            }
            #endregion
            #region �����´�ʱ��
            DateTime dateTimeNow = DateTime.Now.Date;
            if (facilityMaster.MaintainStartDate == null)
            {
                facilityMaster.MaintainStartDate = dateTimeNow;
            }

            //�´�����ʱ��
            facilityMaster.NextMaintainTime = facilityMaster.MaintainStartDate.Value.AddDays(facilityMaster.MaintainLeadTime.HasValue ? 0 - facilityMaster.MaintainLeadTime.Value : 0);

            this.Update(facilityMaster);
            #endregion
        }

        //[Transaction(TransactionMode.Requires)]
        //public void GenerateMaintainInfo()
        //{
        //    #region ȡ������Ҫ��������ʩ
        //    DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityMaster));
        //    criteria.Add(Expression.Le("NextMaintainTime", DateTime.Now));
        //    IList<FacilityMaster> facilityMasterList = criteriaMgrE.FindAll<FacilityMaster>(criteria);
        //    #endregion

        //    #region ����ISI����
        //    User monitorUser = userMgrE.LoadUser(BusinessConstants.SYSTEM_USER_MONITOR);
        //    if (facilityMasterList != null && facilityMasterList.Count > 0)
        //    {
        //        foreach (FacilityMaster facilityMaster in facilityMasterList)
        //        {

        //            #region ����ISI����

        //            TaskMstr task = new TaskMstr();
        //            task.Subject = facilityMaster.FCID + "��Ҫ����";
        //            task.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
        //            task.Desc1 = facilityMaster.FCID + "��Ҫ����,��������Ϊ" + facilityMaster.NextMaintainTime.Value.AddDays(facilityMaster.MaintainLeadTime.HasValue ? facilityMaster.MaintainLeadTime.Value : 0).ToShortDateString();
        //            task.IsAutoRelease = true;

        //            #region д����
        //            task.TaskSubType = taskSubTypeMgrE.LoadTaskSubType("LYQ");  //��д����
        //            task.UserName = monitorUser.Name;
        //            task.TaskAddress = facilityMaster.ChargeSite;
        //            task.Type = ISIConstants.ISI_TASK_TYPE_PLAN;
        //            #endregion

        //            #region ������Ϣ
        //            //task.AssignUser = 
        //            task.PlanStartDate = facilityMaster.NextMaintainTime.Value.AddDays(facilityMaster.MaintainLeadTime.HasValue ? facilityMaster.MaintainLeadTime.Value : 0);
        //            task.PlanCompleteDate = task.PlanStartDate.Value.AddDays(facilityMaster.MaintainPeriod.HasValue ? facilityMaster.MaintainPeriod.Value : 0);
        //            #endregion

        //            #region ��Ҫ�����˵Ļ�
        //            //task.AssignStartUser = ;
        //            //task.AssignStartDate = ;
        //            //task.AssignStartUserNm = "Monitor";
        //            #endregion

        //            taskMgrE.CreateTask(task, monitorUser);
        //            #endregion

        //            #region �����´�ʱ��
        //            if (facilityMaster.MaintainType == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_ONCE)
        //            {
        //                facilityMaster.NextMaintainTime = null;
        //            }
        //            else
        //            {
        //                #region �������ڶ�ά��
        //                if (facilityMaster.MaintainTypePeriod.HasValue)
        //                {
        //                    facilityMaster.NextMaintainTime = facilityMaster.NextMaintainTime.Value.AddDays(facilityMaster.MaintainTypePeriod.Value);
        //                }
        //                #endregion
        //            }
        //            facilityMaster.LastModifyDate = DateTime.Now;
        //            facilityMaster.LastModifyUser = monitorUser.Code;
        //            this.Update(facilityMaster);
        //            #endregion
        //        }
        //    }
        //    #endregion
        //}


        [Transaction(TransactionMode.Requires)]
        public void GenerateISITasks()
        {
            #region ȡ���е�ʱ���Ԥ���ƻ�����ʩ
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityMaintainPlan));
            criteria.CreateAlias("FacilityMaster", "f");
            criteria.CreateAlias("MaintainPlan", "m");
            criteria.Add(Expression.Or(Expression.And(Expression.IsNotNull("NextWarnDate"), Expression.Le("NextWarnDate", DateTime.Now)),
              Expression.And(Expression.Eq("m.Type",FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_FREQUENCY),
                Expression.And(Expression.IsNotNull("f.MaintainTypePeriod"), Expression.LeProperty("NextWarnQty", "f.MaintainTypePeriod")))));
            criteria.Add(Expression.In("f.Status", new string[] { FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, FacilityConstants.CODE_MASTER_FACILITY_STATUS_FIX, FacilityConstants.CODE_MASTER_FACILITY_STATUS_INSPECT, FacilityConstants.CODE_MASTER_FACILITY_STATUS_MAINTAIN }));
            IList<FacilityMaintainPlan> facilityMaintainPlanList = criteriaMgrE.FindAll<FacilityMaintainPlan>(criteria);
            #endregion

            #region ����ISI����
            User monitorUser = userMgrE.LoadUser(BusinessConstants.SYSTEM_USER_MONITOR);
            if (facilityMaintainPlanList != null && facilityMaintainPlanList.Count > 0)
            {

                foreach (FacilityMaintainPlan facilityPlan in facilityMaintainPlanList)
                {
                    #region ����ISI����
                    DateTime maintainDate = DateTime.Now;
                    if (facilityPlan.MaintainPlan.Type != FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_FREQUENCY)
                    {
                        maintainDate = facilityPlan.NextMaintainDate.Value;
                    }

                    TaskMstr task = new TaskMstr();
                    task.Subject = "��ʩ���룺" + facilityPlan.FacilityMaster.FCID + ",�ʲ���ţ�" + facilityPlan.FacilityMaster.AssetNo + ",�ο��ţ�" + facilityPlan.FacilityMaster.RefenceCode + ",��Ҫ���У�" + facilityPlan.MaintainPlan.Description;
                    task.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
                    task.Desc1 = "��ʩ���룺" + facilityPlan.FacilityMaster.FCID + ",�ʲ���ţ�" + facilityPlan.FacilityMaster.AssetNo + ",��ʩ���ƣ�" + facilityPlan.FacilityMaster.Name + ",�ο��ţ�" + facilityPlan.FacilityMaster.RefenceCode + ",��Ҫ���У�" + facilityPlan.MaintainPlan.Description + ",���ڣ�" + maintainDate.ToShortDateString();
                    task.IsAutoRelease = true;
                    task.Desc2 = facilityPlan.FacilityMaster.FCID;  //��ʩ���룬�������ر�������
                    task.ExtNo = facilityPlan.MaintainPlan.Code; //���Դ��룬�������õ�
                    task.RefNo = facilityPlan.FacilityMaster.FCID; //��ʩ���룬�����޸ĺ�������ر��õ�

                    #region д����
                    task.TaskSubType = taskSubTypeMgrE.LoadTaskSubType("SSGL");
                    task.UserName = monitorUser.Name;
                    task.Email = monitorUser.Email;
                    task.MobilePhone = monitorUser.MobliePhone;
                    task.TaskAddress = monitorUser.Address;
                    task.Type = ISIConstants.ISI_TASK_TYPE_PLAN;
                    #endregion

                    #region ������Ϣ
                    if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_FREQUENCY)
                    {
                        task.PlanStartDate = DateTime.Now;
                        task.PlanCompleteDate = DateTime.Now.AddDays(1);
                    }
                    else
                    {
                        task.PlanStartDate = facilityPlan.NextMaintainDate.Value;
                        task.PlanCompleteDate = facilityPlan.NextMaintainDate.Value.AddHours(facilityPlan.MaintainPlan.Period.HasValue ? facilityPlan.MaintainPlan.Period.Value : 0);
                    }
                    #endregion

                    #region ��Ҫ�����˵Ļ�
                    task.AssignStartUser = facilityPlan.MaintainPlan.StartUpUser;
                    task.AssignStartUserNm = userSubscriptionMgrE.GetUserName(task.StartedUser);
                    task.TaskAddress = facilityPlan.FacilityMaster.ChargeSite;
                    #endregion

                    taskMgrE.CreateTask(task, monitorUser);
                    taskMgrE.AssignTask(task.Code, task.BackYards, task.TaskSubTypeCode, new string[] { task.AssignStartUser }, task.PlanStartDate.Value, task.PlanCompleteDate.Value, task.Desc2, task.ExpectedResults, monitorUser);
                    // taskMgrE.ConfirmTask(task.Code, monitorUser);
                    #endregion

                    #region �����´�ʱ�䡢����
                    if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_FREQUENCY)
                    {
                        facilityPlan.NextMaintainQty = facilityPlan.NextMaintainQty + facilityPlan.MaintainPlan.TypePeriod.Value;
                        facilityPlan.NextWarnQty = facilityPlan.NextWarnQty + facilityPlan.MaintainPlan.TypePeriod.Value;
                    }
                    else if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_ONCE)
                    {
                        facilityPlan.NextMaintainDate = null;
                        facilityPlan.NextWarnDate = null;
                    }
                    else
                    {
                        #region �������ڶ�ά��
                        if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_MINUTE)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddMinutes(facilityPlan.MaintainPlan.TypePeriod.Value);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddMinutes(facilityPlan.MaintainPlan.TypePeriod.Value);
                        }
                        else if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_HOUR)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddHours(facilityPlan.MaintainPlan.TypePeriod.Value);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddHours(facilityPlan.MaintainPlan.TypePeriod.Value);
                        }
                        else if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_DAY)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddDays(facilityPlan.MaintainPlan.TypePeriod.Value);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddDays(facilityPlan.MaintainPlan.TypePeriod.Value);
                        }
                        else if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_WEEK)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddDays(7 * facilityPlan.MaintainPlan.TypePeriod.Value);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddDays(7 * facilityPlan.MaintainPlan.TypePeriod.Value);
                        }
                        else if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_MONTH)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddMonths(facilityPlan.MaintainPlan.TypePeriod.Value);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddMonths(facilityPlan.MaintainPlan.TypePeriod.Value);
                        }
                        else if (facilityPlan.MaintainPlan.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_YEAR)
                        {
                            facilityPlan.NextMaintainDate = facilityPlan.NextMaintainDate.Value.AddYears(facilityPlan.MaintainPlan.TypePeriod.Value);
                            facilityPlan.NextWarnDate = facilityPlan.NextWarnDate.Value.AddYears(facilityPlan.MaintainPlan.TypePeriod.Value);
                        }
                        #endregion
                    }

                    facilityMaintainPlanMgrE.UpdateFacilityMaintainPlan(facilityPlan, false);
                    #endregion
                }
            }
            #endregion
        }


        [Transaction(TransactionMode.Requires)]
        public void GenerateMouldISITasks()
        {
            #region ȡ���е�ʱ���Ԥ���ƻ�����ʩ
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityAllocate));
            criteria.CreateAlias("FacilityMaster", "f");
            criteria.Add(Expression.GeProperty("AllocatedQty", "NextWarnQty"));
            criteria.Add(Expression.In("f.Status", new string[] { FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, FacilityConstants.CODE_MASTER_FACILITY_STATUS_FIX, FacilityConstants.CODE_MASTER_FACILITY_STATUS_INSPECT, FacilityConstants.CODE_MASTER_FACILITY_STATUS_MAINTAIN }));
            IList<FacilityAllocate> facilityAllocateList = criteriaMgrE.FindAll<FacilityAllocate>(criteria);
            #endregion

            #region ����ISI����
            User monitorUser = userMgrE.LoadUser(BusinessConstants.SYSTEM_USER_MONITOR);
            if (facilityAllocateList != null && facilityAllocateList.Count > 0)
            {
                foreach (FacilityAllocate facilityAllocate in facilityAllocateList)
                {
                    #region ����ISI����
                    DateTime maintainDate = DateTime.Now;

                    TaskMstr task = new TaskMstr();
                    task.Subject = "��ʩ���룺" + facilityAllocate.FacilityMaster.FCID + ",�ʲ���ţ�" + facilityAllocate.FacilityMaster.AssetNo + ",�ο��ţ�" + facilityAllocate.FacilityMaster.RefenceCode + ",��Ҫ���б���";
                    task.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
                    task.Desc1 = "��ʩ���룺" + facilityAllocate.FacilityMaster.FCID + ",�ʲ���ţ�" + facilityAllocate.FacilityMaster.AssetNo + ",��ʩ���ƣ�" + facilityAllocate.FacilityMaster.Name + ",�ο��ţ�" + facilityAllocate.FacilityMaster.RefenceCode + ",��ģ���Ѵ�" + facilityAllocate.AllocatedQty.ToString("0.##") + ",��Ҫ���б���";
                    task.IsAutoRelease = true;
                    task.Desc2 = facilityAllocate.FacilityMaster.FCID;  //��ʩ���룬�������ر�������
                   // task.ExtNo = facilityAllocate.MaintainPlan.Code; //���Դ��룬�������õ�
                    task.RefNo = facilityAllocate.FacilityMaster.FCID; //��ʩ���룬�����޸ĺ�������ر��õ�

                    #region д����
                    task.TaskSubType = taskSubTypeMgrE.LoadTaskSubType("SSGL");
                    task.UserName = monitorUser.Name;
                    task.Email = monitorUser.Email;
                    task.MobilePhone = monitorUser.MobliePhone;
                    task.TaskAddress = monitorUser.Address;
                    task.Type = ISIConstants.ISI_TASK_TYPE_PLAN;
                    task.PlanStartDate = DateTime.Now;
                    task.PlanCompleteDate = DateTime.Now.AddDays(1);
                    #endregion

                    #region ��Ҫ�����˵Ļ�
                    task.AssignStartUser = facilityAllocate.StartUpUser;
                    task.AssignStartUserNm = userSubscriptionMgrE.GetUserName(task.StartedUser);
                    task.TaskAddress = facilityAllocate.FacilityMaster.ChargeSite;
                    #endregion

                    taskMgrE.CreateTask(task, monitorUser);
                    taskMgrE.AssignTask(task.Code, task.BackYards, task.TaskSubTypeCode, new string[] { task.AssignStartUser }, task.PlanStartDate.Value, task.PlanCompleteDate.Value, task.Desc2, task.ExpectedResults, monitorUser);
                    // taskMgrE.ConfirmTask(task.Code, monitorUser);
                    #endregion

                    #region �����´�ʱ�䡢����
                    facilityAllocate.NextWarnQty = facilityAllocate.NextWarnQty + facilityAllocate.WarnQty;
                    base.Update(facilityAllocate);

                    #endregion

                }
            }
            #endregion
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FacilityMaster> GetFacilityChargeSite()
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityMaster>();
            criteria.Add(Expression.Not(Expression.In("Status", new string[] { FacilityConstants.CODE_MASTER_FACILITY_STATUS_LEND, FacilityConstants.CODE_MASTER_FACILITY_STATUS_SELL })));
            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList()
                .Add(Projections.Alias(Projections.Property("ChargeSite"), "ChargeSite"))));

            criteria.SetResultTransformer(
                new NHibernate.Transform.AliasToBeanResultTransformer(typeof(FacilityMaster)));

            return criteriaMgrE.FindAll<FacilityMaster>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FacilityMaster> GetFacilityChargeOrganization()
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityMaster>();
            criteria.Add(Expression.Not(Expression.In("Status", new string[] { FacilityConstants.CODE_MASTER_FACILITY_STATUS_LEND, FacilityConstants.CODE_MASTER_FACILITY_STATUS_SELL })));
            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList()
                .Add(Projections.Alias(Projections.Property("ChargeOrganization"), "ChargeOrganization"))));

            criteria.SetResultTransformer(
                new NHibernate.Transform.AliasToBeanResultTransformer(typeof(FacilityMaster)));

            return criteriaMgrE.FindAll<FacilityMaster>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FacilityMaster> GetFacilityChargePerson()
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityMaster>();
            criteria.Add(Expression.Not(Expression.In("Status", new string[] { FacilityConstants.CODE_MASTER_FACILITY_STATUS_LEND, FacilityConstants.CODE_MASTER_FACILITY_STATUS_SELL })));
            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList()
                .Add(Projections.Alias(Projections.Property("CurrChargePerson"), "CurrChargePerson"))
                .Add(Projections.Alias(Projections.Property("CurrChargePersonName"), "CurrChargePersonName"))));

            criteria.SetResultTransformer(
                new NHibernate.Transform.AliasToBeanResultTransformer(typeof(FacilityMaster)));

            return criteriaMgrE.FindAll<FacilityMaster>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FacilityMaster> GetMaintainGroupList()
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityMaster>();

            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList()
                .Add(Projections.Alias(Projections.Property("MaintainGroup"), "MaintainGroup"))));
            criteria.Add(Expression.Not(Expression.Eq("MaintainGroup", string.Empty)));
            criteria.SetResultTransformer(
                new NHibernate.Transform.AliasToBeanResultTransformer(typeof(FacilityMaster)));

            return criteriaMgrE.FindAll<FacilityMaster>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FacilityMaster> GetMaintainTypeList()
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityMaster>();

            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList()
                .Add(Projections.Alias(Projections.Property("MaintainType"), "MaintainType"))));
            criteria.Add(Expression.Not(Expression.Eq("MaintainType", string.Empty)));
            criteria.SetResultTransformer(
                new NHibernate.Transform.AliasToBeanResultTransformer(typeof(FacilityMaster)));

            return criteriaMgrE.FindAll<FacilityMaster>(criteria);
        }

        public IList<FacilityMaster> GetFacilityMasterList(string fcId)
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityMaster>();
            criteria.Add(Expression.Like("FCID", fcId, MatchMode.Start))
                .AddOrder(Order.Asc("FCID"));

            IList<FacilityMaster> facilityMasterList = criteriaMgrE.FindAll<FacilityMaster>(criteria);
            if (facilityMasterList != null && facilityMasterList.Count > 100)
            {
                facilityMasterList = facilityMasterList.Take(100).ToList();
            }
            return facilityMasterList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetFacilityTransAttachment(string key)
        {
            return GetAttachment(key, typeof(FacilityTrans));
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetFacilityCategoryAttachment(string key)
        {
            return GetAttachment(key, typeof(FacilityCategory));
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetMaintainPlanAttachment(string key)
        {
            return GetAttachment(key, typeof(MaintainPlan));
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetFacilityMasterAttachment(string key)
        {
            return GetAttachment(key, typeof(FacilityMaster));
        }

        public int GetFacilityTransAttachmentCount(string key)
        {
            return GetAttachmentCount(key, typeof(FacilityTrans));
        }

        public int GetFacilityCategoryAttachmentCount(string key)
        {
            return GetAttachmentCount(key, typeof(FacilityCategory));
        }

        public int GetMaintainPlanAttachmentCount(string key)
        {
            return GetAttachmentCount(key, typeof(MaintainPlan));
        }

        public int GetFacilityMasterAttachmentCount(string key)
        {
            return GetAttachmentCount(key, typeof(FacilityMaster));
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<AttachmentDetail> GetFacilityDistributionAttachment(string key)
        {
            return GetAttachment(key, typeof(FacilityDistribution));
        }

        public int GetFacilityDistributionAttachmentCount(string key)
        {
            return GetAttachmentCount(key, typeof(FacilityDistribution));
        }

        public IList<AttachmentDetail> GetAttachment(string key, Type type)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(AttachmentDetail));
            criteria.Add(Expression.Eq("TaskCode", key));
            if (type != null)
            {
                criteria.Add(Expression.Eq("ModuleType", type.FullName));
            }
            criteria.AddOrder(Order.Desc("CreateDate"));
            return criteriaMgrE.FindAll<AttachmentDetail>(criteria, 0, 500);
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetAttachmentCount(string key, Type type)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(AttachmentDetail));
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Id")));
            criteria.Add(Expression.Eq("TaskCode", key));
            if (type != null)
            {
                criteria.Add(Expression.Eq("ModuleType", type.FullName));
            }
            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0)
            {
                return count[0];
            }
            return 0;
        }

        public IList<FacilityMaintainPlan> ReadFacilityMaintainPlanFromxls(Stream inputStream, User user)
        {
            if (inputStream.Length == 0)
            {
                throw new BusinessErrorException("Import.Stream.Empty");
            }
            IWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 11);

            #region �ж���
            int colFCID = 1;//��ʩ����
            int colAssetNo = 3; //�̶��ʲ�����
            int colMaintainPlanCode = 4;//Ԥ�����Դ���
            int colStartDate = 5;//����
            #endregion

            #region �����ݶ������
            IList<FacilityMaster> facilityMasterList = criteriaMgrE.FindAll<FacilityMaster>();
            IList<MaintainPlan> maintainPlanList = criteriaMgrE.FindAll<MaintainPlan>();
            IList<FacilityMaintainPlan> facilityMaintainPlanList = new List<FacilityMaintainPlan>();
            #endregion


            DateTime dateTimeNow = DateTime.Now;
            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                if (!this.CheckValidDataRow(row, 1, 4))
                {
                    break;//�߽�
                }

                #region ��ȡ����
                #region ��ȡ��ʩ����
                string fcId = GetCellStringValue(row.GetCell(colFCID)) == null ? string.Empty : GetCellStringValue(row.GetCell(colFCID));
                #endregion

                #region ��ȡ�ʲ����
                string assetNo = GetCellStringValue(row.GetCell(colAssetNo)) == null ? string.Empty : GetCellStringValue(row.GetCell(colAssetNo));
                #endregion

                #region �ж�
                if (string.IsNullOrEmpty(fcId) && string.IsNullOrEmpty(assetNo))
                {
                    throw new BusinessErrorException("Facility.Import.FCIDOrAssetNoEmpty", (row.RowNum + 1).ToString());
                }
                FacilityMaster facilityMaster = null;

                if (!string.IsNullOrEmpty(assetNo))
                {
                    facilityMaster = facilityMasterList.Where(p => p.AssetNo == assetNo).FirstOrDefault();
                }
                if (facilityMaster == null)
                {
                    facilityMaster = facilityMasterList.Where(p => p.FCID == fcId).FirstOrDefault();
                }
                if (facilityMaster == null)
                {
                    throw new BusinessErrorException("Facility.Import.FacilityMasterNotExists", (row.RowNum + 1).ToString(), fcId.ToString(), assetNo.ToString());
                }

                #endregion


                #region ��ȡԤ�����Դ���
                string maintainPlanCode = row.GetCell(colMaintainPlanCode) != null ? row.GetCell(colMaintainPlanCode).StringCellValue : string.Empty;
                if (string.IsNullOrEmpty(maintainPlanCode))
                {
                    throw new BusinessErrorException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colMaintainPlanCode.ToString());
                }
                #endregion

                #region �ж�
                MaintainPlan maintainPlan = maintainPlanList.Where(p => p.Code == maintainPlanCode && p.Type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_ONCE).FirstOrDefault();
                if (maintainPlan == null)
                {
                    throw new BusinessErrorException("Facility.Import.MaintainPlanNotExists", (row.RowNum + 1).ToString(), fcId.ToString(), assetNo.ToString());
                }
                #endregion

                #region ��ȡ��ʼ����
                DateTime startDate = row.GetCell(colStartDate) != null ? row.GetCell(colStartDate).DateCellValue : DateTime.Now.Date;
                #endregion
                #endregion

                #region �������
                FacilityMaintainPlan facilityMaintainPlan = new FacilityMaintainPlan();
                facilityMaintainPlan.FacilityMaster = facilityMaster;
                facilityMaintainPlan.MaintainPlan = maintainPlan;
                facilityMaintainPlan.StartDate = startDate;
                facilityMaintainPlanList.Add(facilityMaintainPlan);
                #endregion

            }

            return facilityMaintainPlanList;
        }

        private bool CheckValidDataRow(IRow row, int startColIndex, int endColIndex)
        {
            for (int i = startColIndex; i < endColIndex; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell != null && cell.CellType != NPOI.SS.UserModel.CellType.BLANK)
                {
                    return true;
                }
            }

            return false;
        }
        private string GetCellStringValue(ICell cell)
        {
            string strValue = null;
            if (cell != null)
            {
                if (cell.CellType == CellType.STRING)
                {
                    strValue = cell.StringCellValue;
                }
                else if (cell.CellType == CellType.NUMERIC)
                {
                    strValue = cell.NumericCellValue.ToString("0.########");
                }
                else if (cell.CellType == CellType.BOOLEAN)
                {
                    strValue = cell.NumericCellValue.ToString();
                }
                else if (cell.CellType == CellType.FORMULA)
                {
                    if (cell.CachedFormulaResultType == CellType.STRING)
                    {
                        strValue = cell.StringCellValue;
                    }
                    else if (cell.CachedFormulaResultType == CellType.NUMERIC)
                    {
                        strValue = cell.NumericCellValue.ToString("0.########");
                    }
                    else if (cell.CachedFormulaResultType == CellType.BOOLEAN)
                    {
                        strValue = cell.NumericCellValue.ToString();
                    }
                }
            }
            if (strValue != null)
            {
                strValue = strValue.Trim();
            }
            strValue = strValue == string.Empty ? null : strValue;
            return strValue;
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchMaintainStart(IList<string> fcidList, User user)
        {
            if (fcidList != null && fcidList.Count > 0)
            {
                DateTime dateTimeNow = DateTime.Now;
                foreach (string fcid in fcidList)
                {
                    FacilityMaster facilityMaster = this.LoadFacilityMaster(fcid);

                    FacilityTrans facilityTrans = new FacilityTrans();
                    facilityTrans.CreateDate = DateTime.Now;
                    facilityTrans.CreateUser = user.Code;
                    facilityTrans.EffDate = DateTime.Now.Date;
                    facilityTrans.FCID = facilityMaster.FCID;
                    facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
                    facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
                    facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
                    facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
                    facilityTrans.ToChargePerson = facilityMaster.CurrChargePerson;
                    facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
                    facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
                    facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
                    facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_START;
                    facilityTrans.BatchNo = "MS" + dateTimeNow.ToString("yyyyMMddhhmmss");
                    //facilityTrans.Remark = remark;

                    this.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_MAINTAIN, user.Code);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public int BatchMaintainFinish(IList<string> fcIdList, string startDate, string endDate, string remark, User user)
        {

            int transId = 0;
            
            if (fcIdList != null && fcIdList.Count > 0)
            {
                DateTime dateTimeNow = DateTime.Now;
                foreach (string fcId in fcIdList)
                {

                    FacilityMaster facilityMaster = this.LoadFacilityMaster(fcId);

                    FacilityTrans facilityTrans = new FacilityTrans();
                    FacilityTrans oldFacilityTrans = this.LoadFacilityMaintain(fcId);
                    CloneHelper.CopyProperty(oldFacilityTrans, facilityTrans);

                    if (!string.IsNullOrEmpty(startDate))
                    {
                        facilityTrans.StartDate = Convert.ToDateTime(startDate);
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        facilityTrans.EndDate = Convert.ToDateTime(endDate);
                    }

                    facilityTrans.CreateDate = DateTime.Now;
                    facilityTrans.CreateUser = user.Code;
                    facilityTrans.EffDate = DateTime.Now.Date;
                    facilityTrans.Remark = remark;
                    facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_FINISH;

                    facilityTrans.BatchNo = "MF" + dateTimeNow.ToString("yyyyMMddhhmmss");

                    this.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, user.Code);
                    if (transId == 0)
                    {
                        transId = facilityTrans.Id;
                    }
                }
            }
            return transId;
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityMasterMgrE : com.Sconit.Facility.Service.Impl.FacilityMasterMgr, IFacilityMasterMgrE
    {
    }
}

#endregion Extend Class