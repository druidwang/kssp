using System;
using System.Collections.Generic;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.BIL;
using System.IO;
using com.Sconit.Entity.ISI;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Service
{
    public interface ITaskMgr 
    {
      

        void CreateTask(TaskMaster taskMaster);

        void UpdateTask(TaskMaster taskMaster);

        void SubmitTask(string code, User user);

        void SubmitTask(TaskMaster task, User user);

        void ConfirmTask(string taskCode, User user);

        void ConfirmTask(TaskMaster task, User user);

        void ConfirmTask(string taskCode, DateTime planStartDate, DateTime planCompleteDate, string desc2, string expectedResults, User user);

        void CompleteTask(string taskCode, User user);

        void CompleteTask(TaskMaster task, User user);

        void CompleteTask(string taskCode, string desc2, User user);

        void CloseTask(string taskCode, User user);

        void CreateTaskDetail(TaskMaster task, string level, IList<UserSub> userSubList, bool isEmailException, bool isSMSException, User user);

        void AssignTask(string taskCode, string assignStartUser, DateTime planStartDate, DateTime planCompleteDate, string desc2, string expectedResults, User user);

        void CreateTaskStatus(TaskStatus taskStatus, User currentUser, bool isComplete);
    }
}
