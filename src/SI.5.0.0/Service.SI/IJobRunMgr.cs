using Castle.Windsor;

namespace com.Sconit.Service.SI
{
    public interface IJobRunMgr
    {
        void RunBatchJobs();
    }

    public interface IJobMgr
    {
        void RunJob(JobDataMap dataMap);
    }
}
