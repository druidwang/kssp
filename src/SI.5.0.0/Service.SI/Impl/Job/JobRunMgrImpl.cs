using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using com.Sconit.Entity.SI.BAT;
using com.Sconit.Persistence;
using System.Threading.Tasks;
using System.Threading;
using Castle.Services.Transaction;
using System.Reflection;
using com.Sconit.Service.SI;
using com.Sconit.Entity;

namespace com.Sconit.Service.SI.Impl
{
    [Transactional]
    public partial class JobRunMgrImpl : IJobRunMgr
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.BatchJob");
        public IGenericMgr genericMgr { get; set; }

        private static string runBatchJobsLock = string.Empty;
        public void RunBatchJobs()
        {
            lock (runBatchJobsLock)
            {
                log.Info("----------------------------------Invincible's dividing line---------------------------------------");
                log.Info("BatchJobs run start.");

                IList<Trigger> tobeFiredTriggerList = this.GetTobeFiredTrigger();

                if (tobeFiredTriggerList != null && tobeFiredTriggerList.Count > 0)
                {
                    //Parallel.ForEach(tobeFiredTriggerList, (tobeFiredTrigger) =>
                    foreach (Trigger tobeFiredTrigger in tobeFiredTriggerList)
                    {
                        Thread.Sleep(500);

                        JobDetail jobDetail = tobeFiredTrigger.JobDetail;
                        RunLog runLog = new RunLog();
                        try
                        {
                            #region Job运行前处理
                            BeforeJobRun(runLog, tobeFiredTrigger);
                            #endregion

                            #region Job参数获取
                            JobDataMap dataMap = new JobDataMap();
                            IList<JobParameter> jobParameterList = genericMgr.FindAllWithCustomQuery<JobParameter>
                                ("from JobParameter where JobCode = ?", jobDetail.Code);
                            if (jobParameterList != null && jobParameterList.Count > 0)
                            {
                                foreach (JobParameter jobParameter in jobParameterList)
                                {
                                    log.Debug("Set Job Parameter Name:" + jobParameter.Key + ", Value:" + jobParameter.Value);
                                    dataMap.PutData(jobParameter.Key, jobParameter.Value);
                                }
                            }
                            #endregion

                            #region Trigger参数获取
                            IList<TriggerParameter> triggerParameterList = genericMgr.FindAllWithCustomQuery<TriggerParameter>
                                ("from TriggerParameter where TriggerName = ?", tobeFiredTrigger.Name);
                            if (triggerParameterList != null && triggerParameterList.Count > 0)
                            {
                                foreach (TriggerParameter triggerParameter in triggerParameterList)
                                {
                                    log.Debug("Set Trigger Parameter Name:" + triggerParameter.Key + ", Value:" + triggerParameter.Value);
                                    if (!dataMap.ContainKey(triggerParameter.Key))
                                    {
                                        dataMap.PutData(triggerParameter.Key, triggerParameter.Value);
                                    }
                                }
                            }
                            #endregion

                            #region 运行Job
                            //反射方法
                            //object newInstance = Assembly.GetExecutingAssembly().CreateInstance("com.Sconit.Service.SI.Job", false);
                            //Type.GetType("com.Sconit.Service.SI.Job").GetMethod(jobDetail.ServiceType).Invoke(newInstance, new object[] { dataMap, container });
                            RunJob(jobDetail.ServiceType, dataMap);
                            genericMgr.FlushSession();
                            #endregion

                            #region Job运行后处理
                            AfterJobRunSuccess(runLog, tobeFiredTrigger);
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            AfterJobRunFail(runLog, tobeFiredTrigger, ex);
                        }
                        finally
                        {
                            #region 更新BatchTrigger
                            UpdateTrigger(tobeFiredTrigger);
                            #endregion
                        }
                    }
                    //);
                }
                else
                {
                    log.Info("No job found may run in this batch.");
                }

                log.Info("BatchJobs run end.");
            }
        }

        private IList<Trigger> GetTobeFiredTrigger()
        {
            return genericMgr.FindAllWithCustomQuery<Trigger>("from Trigger Where Status = ? and NextFireTime <= ?", new object[] { CodeMaster.TriggerStatus.Open, DateTime.Now });
        }

        private void BeforeJobRun(RunLog runLog, Trigger tobeFiredTrigger)
        {
            log.Info("Start run job. JobCode:" + tobeFiredTrigger.JobDetail.Code + ", JobName:" + tobeFiredTrigger.JobDetail.Description);
            runLog.JobCode = tobeFiredTrigger.JobDetail.Code;
            runLog.TriggerName = tobeFiredTrigger.Name;
            runLog.StartTime = DateTime.Now;
            runLog.Status = CodeMaster.JobRunStatus.InProcess;
            genericMgr.Create(runLog);
            genericMgr.FlushSession();
        }

        private void AfterJobRunSuccess(RunLog runLog, Trigger tobeFiredTrigger)
        {
            log.Info("Job run successful. JobCode:" + tobeFiredTrigger.JobDetail.Code + ", JobName:" + tobeFiredTrigger.JobDetail.Description);
            genericMgr.ExecuteUpdateWithCustomQuery("update from RunLog set EndTime = ?, Status = ? where Id = ?",
                new object[] { DateTime.Now, CodeMaster.JobRunStatus.Success, runLog.Id });
            genericMgr.FlushSession();
        }

        private void AfterJobRunFail(RunLog runLog, Trigger tobeFiredTrigger, Exception ex)
        {
            try
            {
                log.Error("Job run failure. JobCode:" + tobeFiredTrigger.JobDetail.Code + ", JobName:" + tobeFiredTrigger.JobDetail.Description, ex);
                if (ex.Message != null && ex.Message.Length > 1000)
                {
                    genericMgr.ExecuteUpdateWithCustomQuery("update from RunLog set EndTime = ?, Status = ?, Message = ? where Id = ?",
                        new object[] { DateTime.Now, CodeMaster.JobRunStatus.Failure, ex.Message.Substring(0, 1000), runLog.Id });
                }
                else
                {
                    genericMgr.ExecuteUpdateWithCustomQuery("update from RunLog set EndTime = ?, Status = ?, Message = ? where Id = ?",
                        new object[] { DateTime.Now, CodeMaster.JobRunStatus.Failure, ex.Message, runLog.Id });
                }
            }
            catch (Exception ex1)
            {
                log.Error(ex1.Message, ex1);
            }
        }

        private void UpdateTrigger(Trigger tobeFiredTrigger)
        {
            try
            {
                tobeFiredTrigger.TimesTriggered++;
                tobeFiredTrigger.PreviousFireTime = tobeFiredTrigger.NextFireTime;
                if (tobeFiredTrigger.RepeatCount != 0 && tobeFiredTrigger.TimesTriggered >= tobeFiredTrigger.RepeatCount)
                {
                    //关闭Trigger
                    log.Debug("Close Trigger:" + tobeFiredTrigger.Name);
                    tobeFiredTrigger.Status = CodeMaster.TriggerStatus.Close;
                    tobeFiredTrigger.NextFireTime = null;
                }
                else
                {
                    //设置下次运行时间
                    log.Debug("Set Trigger Next Start Time, Add:" + tobeFiredTrigger.Interval.ToString() + " " + tobeFiredTrigger.IntervalType);
                    DateTime dateTimeNow = DateTime.Now;
                    if (!tobeFiredTrigger.NextFireTime.HasValue)
                    {
                        tobeFiredTrigger.NextFireTime = dateTimeNow;
                    }
                    else
                    {
                        while (tobeFiredTrigger.NextFireTime.Value <= dateTimeNow)
                        {
                            double hoursInterval = Utility.DateTimeHelper.TimeTranfer(tobeFiredTrigger.Interval, tobeFiredTrigger.IntervalType, CodeMaster.TimeUnit.Hour);
                            tobeFiredTrigger.NextFireTime = tobeFiredTrigger.NextFireTime.Value.AddHours(hoursInterval);
                        }
                    }
                    log.Debug("Trigger Next Start Time is set as:" + tobeFiredTrigger.NextFireTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                genericMgr.ExecuteUpdateWithCustomQuery("update from Trigger set TimesTriggered = ?, PreviousFireTime = ?, NextFireTime = ?, Status = ? where Name = ?",
                        new object[] { tobeFiredTrigger.TimesTriggered, tobeFiredTrigger.PreviousFireTime, tobeFiredTrigger.NextFireTime, tobeFiredTrigger.Status, tobeFiredTrigger.Name });
            }
            catch (Exception ex)
            {
                log.Error("Error occur when update batch trigger.", ex);
            }
        }
    }
}
