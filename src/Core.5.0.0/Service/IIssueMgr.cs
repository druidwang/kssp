

namespace com.Sconit.Service
{
    using System;
    using com.Sconit.Entity.ISS;
    using com.Sconit.Entity.ACC;

    public interface IIssueMgr
    {
        void DeleteIssueTypeTo(string code);

        void Create(IssueMaster issue);

        void Release(string code);

        void Start(string id, string finishedUserCode, DateTime? finishedDate, string solution);

        int BatchClose(string codeStr, bool isAdmin);

        int BatchDelete(string codeStr, bool isAdmin);

    }

    public interface IIssueUtilMgr
    {
        bool HavePermission(string issueCode, com.Sconit.CodeMaster.IssueStatus status);
        bool HavePermission(string issueCode, User user, com.Sconit.CodeMaster.IssueStatus status);
    }

    public interface IIssueLogMgr
    {

        void LogError(string issue, string content);

        void LogFatal(string issue, string content);

        void LogInfo(string issue, string content);

        void LogWarn(string issue, string content);

        void LogDebug(string issue, string content);

        void LogError(string issue, IssueDetail issueDetail, string content);

        void LogFatal(string issue, IssueDetail issueDetail, string content);

        void LogInfo(string issue, IssueDetail issueDetail, string content);

        void LogWarn(string issue, IssueDetail issueDetail, string content);

        void LogDebug(string issue, IssueDetail issueDetail, string content);

    }
}
