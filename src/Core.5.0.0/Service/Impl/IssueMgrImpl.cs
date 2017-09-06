

namespace com.Sconit.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Castle.Services.Transaction;
    using com.Sconit.Entity;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.ISS;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Utility;
    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Type;

    [Transactional]
    public class IssueMgrImpl : BaseMgr, IIssueMgr
    {
        #region 变量
        private static log4net.ILog log = log4net.LogManager.GetLogger("DebugLog");
        public IGenericMgr genericMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IQueryMgr queryMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public IIssueLogMgr issueLogMgr { get; set; }
        #endregion

        #region public methods
        [Transaction(TransactionMode.Requires)]
        public void DeleteIssueTypeTo(string code)
        {
            IssueTypeToMaster issueTypeToMaster = genericMgr.FindById<IssueTypeToMaster>(code);

            if (issueTypeToMaster != null)
            {
                string hql = string.Empty;

                hql = "delete from IssueTypeToUserDetail where IssueTypeTo = ?";
                genericMgr.Update(hql, issueTypeToMaster.Code, NHibernateUtil.String);

                hql = "delete from IssueTypeToRoleDetail where IssueTypeTo = ?";
                genericMgr.Update(hql, issueTypeToMaster.Code, NHibernateUtil.String);

                hql = "delete from IssueTypeToMaster where Code = ?";
                genericMgr.Update(hql, issueTypeToMaster.Code, NHibernateUtil.String);
            }
            else
            {
                throw new BusinessException(Resources.ISS.IssueMaster.NotFound, code);
            }
        }


        [Transaction(TransactionMode.Unspecified)]
        public IList<IssueMaster> GetIssueMasterByIssueAddress(string issueAddress)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(IssueMaster));
            criteria.Add(Expression.Eq("IssueAddress.Id", issueAddress));

            return this.queryMgr.FindAll<IssueMaster>(criteria);
        }

        [Transaction(TransactionMode.Requires)]
        public void Create(IssueMaster issue)
        {
            #region 创建IssueMaster

            issue.Status = com.Sconit.CodeMaster.IssueStatus.Create;
            issue.Code = numberControlMgr.GetIssueNo(issue);
            this.genericMgr.Create(issue);
            if (issue.ReleaseIssue)
            {
                this.Release(issue);
            }

            #endregion
        }


        public void Release(string code)
        {
            IssueMaster issue = this.genericMgr.FindById<IssueMaster>(code);
            this.Release(issue);
        }



        private IssueLevel GetDefaultIssueLevel()
        {
            IList<IssueLevel> issueLevelList = this.genericMgr.FindAll<IssueLevel>("from IssueLevel where isDefault = true ");
            if (issueLevelList != null && issueLevelList.Count > 0)
            {
                return issueLevelList[0];
            }
            else
            {
                return null;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Start(string id, string finishedUserCode, DateTime? finishedDate, string solution)
        {
            IssueMaster issue = this.genericMgr.FindById<IssueMaster>(id);
            if (issue.Status == com.Sconit.CodeMaster.IssueStatus.Submit)
            {
                issue.FinishedUserCode = finishedUserCode;
                issue.Solution = solution;
                if (finishedDate.HasValue)
                {
                    issue.FinishedDate = finishedDate;
                }
                issue.StartDate = DateTime.Now;
                User user = SecurityContextHolder.Get();
                issue.StartUser = user.Id;
                issue.StartUserName = user.FullName;
                issue.Status = com.Sconit.CodeMaster.IssueStatus.InProcess;
                this.genericMgr.Update(issue);

                //发送给计划完成人
                if (!string.IsNullOrWhiteSpace(finishedUserCode))
                {
                    IList<User> userList = this.genericMgr.FindAll<User>("from User where code ='" + finishedUserCode.Trim() + "'");
                    if (userList != null && userList.Count > 0)
                    {

                    }
                }
            }
            else
            {
                throw new BusinessException(Resources.ISS.IssueMaster.Errors_StatusErrorWhenStart, new string[] { issue.Status.ToString(), issue.Code });
            }
        }



        [Transaction(TransactionMode.Requires)]
        public void Release(IssueMaster issue)
        {
            //try
            //{

            IssueLevel issueLevel = this.GetDefaultIssueLevel();
            if (issueLevel == null)
            {
                throw new BusinessException(Resources.ISS.IssueMaster.Errors_DefaultLevelNotFound, issue.IssueType.Code);
            }

            if (issue.Status == com.Sconit.CodeMaster.IssueStatus.Create)
            {
                User user = SecurityContextHolder.Get();

                issue.Status = com.Sconit.CodeMaster.IssueStatus.Submit;
                issue.ReleaseDate = DateTime.Now;
                issue.ReleaseUser = user.Id;
                issue.ReleaseUserName = user.FullName;
                this.genericMgr.Update(issue);

                #region 创建IssueDetail

                #region 用户

                string hql = "select ittud from IssueTypeToUserDetail ittud ";
                hql += "                    join ittud.IssueTypeTo ittm ";
                hql += "                    join ittm.IssueLevel il ";
                hql += "                    join ittm.IssueType it ";
                hql += "                    join ittud.User u ";
                hql += "                    where ((ittud.IsEmail = true and u.Email is not null and u.Email != '') ";
                hql += "                            or (ittud.IsSMS = true and u.MobilePhone is not null and u.MobilePhone != '')) ";
                hql += "                      and il.IsActive = true and ittm.IsActive = true ";
                hql += "                      and ittud.IssueTypeTo=ittm.Code and it.Code =? ";
                hql += "                    order by il.Sequence asc ";

                IList<IssueTypeToUserDetail> issueTypeToUserDetailList = this.genericMgr.FindAll<IssueTypeToUserDetail>(hql, issue.IssueTypeCode);

                IList<IssueDetail> submitSendUser = new List<IssueDetail>();
                foreach (IssueTypeToUserDetail issueTypeToUserDetail in issueTypeToUserDetailList)
                {
                    IssueDetail issueDeatail = new IssueDetail();
                    issueDeatail.User = issueTypeToUserDetail.User;
                    //issueDeatail.UserName = issueTypeToUserDetail.User.FullName;
                    issueDeatail.MobilePhone = issueTypeToUserDetail.User.MobilePhone;
                    issueDeatail.Email = issueTypeToUserDetail.User.Email;
                    issueDeatail.IsActive = true;
                    issueDeatail.EmailCount = 0;
                    issueDeatail.SMSCount = 0;
                    issueDeatail.EmailStatus = com.Sconit.CodeMaster.SendStatus.NotSend;
                    issueDeatail.SMSStatus = com.Sconit.CodeMaster.SendStatus.NotSend;
                    issueDeatail.IssueLevel = issueTypeToUserDetail.IssueTypeTo.IssueLevel.Code;
                    issueDeatail.IsInProcess = issueTypeToUserDetail.IssueTypeTo.IssueLevel.IsInProcess;
                    issueDeatail.IsSubmit = issueTypeToUserDetail.IssueTypeTo.IssueLevel.IsSubmit;
                    issueDeatail.IsDefault = issueTypeToUserDetail.IssueTypeTo.IssueLevel.IsDefault;
                    issueDeatail.IssueCode = issue.Code;
                    issueDeatail.IsSMS = issueTypeToUserDetail.IsSMS;
                    issueDeatail.IsEmail = issueTypeToUserDetail.IsEmail;
                    issueDeatail.Sequence = issueTypeToUserDetail.IssueTypeTo.IssueLevel.Sequence;
                    issueDeatail.IssueTypeToUserDetailId = issueTypeToUserDetail.Id;
                    this.genericMgr.Create(issueDeatail);

                    if (issueDeatail.IsDefault)
                    {
                        submitSendUser.Add(issueDeatail);
                    }
                }
                #endregion

                #region 角色 暂不支持
                #endregion

                #endregion

                this.SendMailAndSMS(issue, issueLevel, submitSendUser);
            }
            else
            {
                throw new BusinessException(Resources.ISS.IssueMaster.Errors_StatusErrorWhenRelease, issue.Code, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.IssueStatus, ((int)issue.Status).ToString()));
            }


            //}
            //catch (Exception e)
            //{
            //    log.Error(e.Message, e);
            //}
        }

        [Transaction(TransactionMode.Requires)]
        private void SendMailAndSMS(IssueMaster issue, IssueLevel level, IList<IssueDetail> issueDetailList)
        {
            try
            {
                if (issueDetailList == null || issueDetailList.Count == 0)
                {
                    throw new BusinessException(Resources.ISS.IssueMaster.EmailListIsNull);
                }

                SendEmail(issue, level.Code, ref issueDetailList);

                SendSMS(issue, level.Code, ref issueDetailList);


                foreach (IssueDetail issueDetail in issueDetailList)
                {
                    if (issueDetail.SMSStatus != com.Sconit.CodeMaster.SendStatus.Fail)
                    {
                        issueDetail.SMSStatus = com.Sconit.CodeMaster.SendStatus.Success;
                        issueLogMgr.LogInfo(issue.Code, issueDetail, issue.Content);
                    }
                    issueDetail.SMSCount = issueDetail.SMSCount + 1;

                    if (issueDetail.EmailStatus != com.Sconit.CodeMaster.SendStatus.Fail)
                    {
                        issueDetail.EmailStatus = com.Sconit.CodeMaster.SendStatus.Success;
                        issueLogMgr.LogInfo(issue.Code, issueDetail, issue.Content);
                    }
                    issueDetail.EmailCount = issueDetail.EmailCount + 1;

                    this.genericMgr.Update(issueDetail);
                }
            }
            catch (Exception e)
            {
                issueLogMgr.LogError(issue.Code, e.Message);
            }
        }

        [Transaction(TransactionMode.Requires)]
        private string GetMobilePhone(string issueCode, ref IList<IssueDetail> issueDetailList)
        {
            StringBuilder mobilePhone = new StringBuilder();
            try
            {
                foreach (IssueDetail issueDetail in issueDetailList)
                {
                    User u = issueDetail.User;
                    if (u != null)
                    {
                        if (ControlHelper.IsValidMobilePhone(u.MobilePhone))
                        {
                            mobilePhone.Append(u.MobilePhone);
                            mobilePhone.Append(";");
                            issueDetail.MobilePhone = u.MobilePhone;
                        }
                        else
                        {
                            issueDetail.SMSStatus = com.Sconit.CodeMaster.SendStatus.Fail;
                            issueLogMgr.LogWarn(issueDetail.IssueCode, issueDetail, Resources.ISS.IssueLog.MobilePhoneIsInvalid);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                issueLogMgr.LogWarn(issueCode, e.Message);
            }
            return mobilePhone.ToString();
        }


        [Transaction(TransactionMode.Requires)]
        private void SendSMS(IssueMaster issue, string level, ref IList<IssueDetail> issueDetailList)
        {
            string body = GetSMSBody(issue, level);

            string toMobilePhone = GetMobilePhone(issue.Code, ref issueDetailList);

            if (toMobilePhone != string.Empty && body != string.Empty)
            {
                //SMSService smsService = new SMSService();
                //smsService.AsyncSend(toMobilePhone, body);
                //smsMgr.AsyncSend(toMobilePhone, body);
            }
            else
            {
                issueLogMgr.LogError(issue.Code, null, "Issue.Code:" + issue.Code + ",toMobilePhone:" + toMobilePhone + ", body:" + body);
                throw new BusinessException(Resources.ISS.IssueMaster.ParamsIsNull, new string[] { issue.Code, toMobilePhone, body });
            }
        }

        [Transaction(TransactionMode.Requires)]
        private void SendEmail(IssueMaster issue, string level, ref IList<IssueDetail> issueDetailList)
        {
            try
            {
                //string userMail = string.Empty;
                //if (issue.Email != null && ControlHelper.IsValidEmail(issue.Email))
                //{
                //    if (issue.UserName != null && issue.UserName.Length > 0)
                //    {
                //        userMail = issue.UserName + "," + issue.Email;
                //    }
                //    else
                //    {
                //        userMail = issue.Email;
                //    }
                //}
                //else
                //{
                //    userMail = systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.SMTPEmailAddr);
                //}

                //string toEmail = GetEmail(issue.Code, ref issueDetailList);

                //string body = GetEmailBody(issue, level);

                //string subject = string.Empty;
                //if (issue.Priority == com.Sconit.CodeMaster.IssuePriority.Urgent)
                //{
                //    subject = systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.IssuePriority, ((int)com.Sconit.CodeMaster.IssuePriority.Urgent).ToString()) + " " + issue.IssueSubject;
                //}
                //else
                //{
                //    subject = issue.IssueSubject;
                //}

                //MailPriority mailPriority;
                //if (issue.Priority == com.Sconit.CodeMaster.IssuePriority.Urgent)
                //{
                //    mailPriority = MailPriority.High;
                //}
                //else
                //{
                //    mailPriority = MailPriority.Normal;
                //}

                //#region email发送
                //EmailService emailService = new EmailService();
                //emailService.AsyncSend(subject, body, toEmail, mailPriority);
                ////smsMgr.SendEmail(subject, body, toEmail, userMail, mailPriority);
                //#endregion
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        private string GetEmail(string issueCode, ref IList<IssueDetail> issueDetailList)
        {
            StringBuilder email = new StringBuilder();
            try
            {
                foreach (IssueDetail issueDetail in issueDetailList)
                {

                    User u = issueDetail.User;
                    if (u != null)
                    {
                        if (ControlHelper.IsValidEmail(u.Email))
                        {
                            email.Append(u.Email);
                            email.Append(";");
                            issueDetail.Email = u.Email;
                        }
                        else
                        {
                            issueDetail.EmailStatus = com.Sconit.CodeMaster.SendStatus.Fail;
                            issueLogMgr.LogWarn(issueDetail.IssueCode, issueDetail, Resources.ISS.IssueLog.EmailAddressIsInvalid);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                issueLogMgr.LogWarn(issueCode, e.Message);
            }
            return email.ToString();
        }

        [Transaction(TransactionMode.Unspecified)]
        public string GetEmailBody(IssueMaster issue, string level)
        {
            return this.GetBody(issue, level, true);
        }
        [Transaction(TransactionMode.Unspecified)]
        public string GetSMSBody(IssueMaster issue, string level)
        {
            return this.GetBody(issue, level, false);
        }
        [Transaction(TransactionMode.Unspecified)]
        public string GetBody(IssueMaster issue, string level, bool isEmail)
        {
            string separator = string.Empty;

            if (isEmail)
            {
                separator = "<br>";
            }
            else
            {
                separator = "\r\n";
            }

            StringBuilder content = new StringBuilder();
            try
            {
                if (isEmail)
                {
                    content.Append(Resources.ISS.IssueMaster.Code + ": " + issue.Code);
                    content.Append(separator);
                }
                content.Append(Resources.ISS.IssueMaster.BackYards + ": " + issue.BackYards);
                content.Append(separator);
                content.Append(Resources.ISS.IssueMaster.IssueSubject + ": " + issue.IssueSubject);

                if (issue.Status == com.Sconit.CodeMaster.IssueStatus.Submit)
                {
                    TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ReleaseDate = new TimeSpan(issue.ReleaseDate.Value.Ticks);
                    TimeSpan diff = now.Subtract(ReleaseDate).Duration();
                    if (diff.Hours > 0)
                    {
                        content.Append("（" + Resources.ISS.IssueMaster.ConfirmOvertime + " " + diff.Hours + "小时）");
                    }
                }
                else if (issue.Status == com.Sconit.CodeMaster.IssueStatus.InProcess)
                {
                    TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan inprocessDate = new TimeSpan(issue.StartDate.Value.Ticks);
                    TimeSpan diff = now.Subtract(inprocessDate).Duration();
                    if (diff.Hours > 0)
                    {
                        content.Append("（" + Resources.ISS.IssueMaster.CompleteOvertime + " " + diff.Hours + "小时）");
                    }
                }
                if (issue.Priority == com.Sconit.CodeMaster.IssuePriority.Urgent)
                {
                    content.Append("[" + systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.IssuePriority, ((int)com.Sconit.CodeMaster.IssuePriority.Urgent).ToString()) + "]");
                }
                content.Append(separator);

                content.Append(Resources.ISS.IssueMaster.DateTime + ": " + DateTime.Now + separator);

                if (isEmail || issue.IssueAddress != null)
                {
                    content.Append(Resources.ISS.IssueMaster.IssueAddress + ": ");
                    content.Append(issue.IssueAddress != null ? issue.IssueAddress : string.Empty);
                    content.Append(separator);
                }
                content.Append(Resources.ISS.IssueMaster.IssueType + ": " + issue.IssueType.Code + separator);
                if (issue.IssueNo != null)
                {
                    content.Append(Resources.ISS.IssueMaster.IssueNo + ": " + issue.IssueNo.Code + separator);
                }
                /*
                if (level != null && level.Length > 0)
                    content.Append("等级: " + codeMasterMgrE.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_ISSUE_TYPE_TO_LEVEL, level).Description + separator);
                content.Append("状态: " + codeMasterMgrE.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_ISSUE_STATUS, issue.Status).Description + separator);
                */

                if (isEmail)
                {
                    content.Append(separator + separator);
                    content.Append(issue.Content);
                    content.Append(separator + separator);
                    if (issue.UserName != null && issue.UserName.Trim() != string.Empty)
                        content.Append(issue.UserName + separator);
                    if (issue.MobilePhone != null && issue.MobilePhone.Trim() != string.Empty && ControlHelper.IsValidMobilePhone(issue.MobilePhone))
                        content.Append("Tel: " + issue.MobilePhone + separator);
                    if (issue.Email != null && issue.Email.Trim() != string.Empty && ControlHelper.IsValidEmail(issue.Email))
                        content.Append("Email: " + issue.Email + separator);
                }
                else
                {
                    if (issue.Content != null && issue.Content.Trim() != string.Empty)
                    {
                        content.Append(issue.Content);
                        content.Append(separator);
                    }
                    if ((issue.UserName != null && issue.UserName.Trim() != string.Empty)
                        || (issue.MobilePhone != null && issue.MobilePhone.Trim() != string.Empty && ControlHelper.IsValidMobilePhone(issue.MobilePhone)))
                    {
                        content.Append("[");
                    }
                    if (issue.UserName != null && issue.UserName.Trim() != string.Empty)
                    {
                        content.Append(issue.UserName);
                    }
                    if (issue.MobilePhone != null && issue.MobilePhone.Trim() != string.Empty && ControlHelper.IsValidMobilePhone(issue.MobilePhone))
                    {
                        content.Append(", " + issue.MobilePhone);
                    }
                    if ((issue.UserName != null && issue.UserName.Trim() != string.Empty)
                        || (issue.MobilePhone != null && issue.MobilePhone.Trim() != string.Empty && ControlHelper.IsValidMobilePhone(issue.MobilePhone)))
                    {
                        content.Append("]");
                    }
                    content.Append(separator);
                    content.Append(Resources.ISS.IssueMaster.Confirmation + " " + issue.Code + "+" + Resources.ISS.IssueMaster.Space + "+Y");
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return content.ToString();

        }

        [Transaction(TransactionMode.RequiresNew)]
        public void SendIssue()
        {
            try
            {
                //已经发送了，但是超过2小时没有确认的。
                //已经确认，超过2小时没有解决的
                IList<IssueMaster> issueMasterSendList = this.genericMgr.FindAll<IssueMaster>("select im from IssueMaster im where im.status=  " + (int)com.Sconit.CodeMaster.IssueStatus.Submit + " and im.status =" + (int)com.Sconit.CodeMaster.IssueStatus.InProcess);
                double completeHours = double.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.CompleteIssueWaitingTime));
                double inProcessHours = double.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.InProcessIssueWaitingTime));
                User user = this.systemMgr.GetMonitorUser();

                IList<IssueLevel> issueLevelsSubmit = null;
                IList<IssueLevel> issueLevelsInprocess = null;

                foreach (IssueMaster issue in issueMasterSendList)
                {
                    if (issue.Status == com.Sconit.CodeMaster.IssueStatus.Submit
                        && !issue.StartDate.HasValue)
                    {
                        issueLevelsSubmit = this.genericMgr.FindAll<IssueLevel>("select il from IssueLevel as il where il.Code in (select distinct det.Level from IssueDetail det where det.Level = il.Code and det.IsSubmit = true)");
                        if (issueLevelsSubmit != null && issueLevelsSubmit.Count > 0)
                        {
                            SendMailAndSMS(issue, inProcessHours, issue.ReleaseDate.Value, issueLevelsSubmit, user);
                        }
                    }
                    else if (issue.Status == com.Sconit.CodeMaster.IssueStatus.InProcess
                        && !issue.CompleteDate.HasValue)
                    {
                        issueLevelsInprocess = this.genericMgr.FindAll<IssueLevel>("select il from IssueLevel as il where il.Code in (select distinct det.Level from IssueDetail det where det.Level = il.Code and det.IsInProcess = true)");
                        if (issueLevelsInprocess != null && issueLevelsInprocess.Count > 0)
                        {
                            SendMailAndSMS(issue, completeHours, issue.ReleaseDate.Value, issueLevelsInprocess, user);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }

        }


        [Transaction(TransactionMode.Requires)]
        private void SendMailAndSMS(IssueMaster issue, double hours, DateTime date, IList<IssueLevel> issueLevels, User user)
        {
            int count = issueLevels.Count;
            foreach (IssueLevel issueLevel in issueLevels)
            {
                if (date.AddHours(count * hours) < DateTime.Now)
                {
                    //是否发送过
                    if (!this.IsSended(issue, issueLevel))
                    {
                        IList<IssueDetail> issueDetailList = this.genericMgr.FindAll<IssueDetail>("select det from IssueDetail where IssueCode = ? and IssueLevel = ? ", new object[] { issue.Code, issueLevel.Code });
                        SendMailAndSMS(issue, issueLevel, issueDetailList);
                    }
                    break;
                }
                count--;
            }
        }

        public bool IsSended(IssueMaster issue, IssueLevel issueLevel)
        {
            string hql = "select count(*) from IssueDetail where IsActive = true and IssueCode =? and Level = ? and (EmailStatus !=0 or SMSStatus !=0 )";

            IList<long> count = this.genericMgr.FindAll<long>(hql, new object[] { issue.Code, issueLevel.Code });

            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }
            return false;
        }


        #region batch

        public int BatchClose(string codeStr, bool isAdmin)
        {
            return Batch("Close", codeStr, isAdmin);
        }

        public int BatchDelete(string codeStr, bool isAdmin)
        {
            return Batch("Delete", codeStr, isAdmin);
        }

        [Transaction(TransactionMode.Requires)]
        private int Batch(string action, string codeStr, bool isAdmin)
        {
            int resultCount = 0;
            IList<string> codeList = (codeStr.Split(',')).ToList<string>();

            if (codeList != null && codeList.Count > 0)
            {
                string hql = string.Empty;
                object[] para = new object[codeList.Count];
                IType[] type = new IType[codeList.Count];

                for (int i = 0; i < codeList.Count; i++)
                {
                    if (i == 0)
                    {
                        if (action == "Delete")
                        {
                            hql += "delete from IssueMaster where Status = " + (int)com.Sconit.CodeMaster.IssueStatus.Create;
                        }
                        else if (action == "Close")
                        {
                            hql += "update from IssueMaster set Status = " + (int)com.Sconit.CodeMaster.IssueStatus.Close + " where Status = " + (int)com.Sconit.CodeMaster.IssueStatus.Complete;
                        }

                        hql += " and Code = ? ";

                        if (!isAdmin)
                        {
                            hql += " and CreateUserId = " + SecurityContextHolder.Get().Id;
                        }
                    }
                    else
                    {
                        hql += " or Code = ? ";
                    }
                    para[i] = codeList[i];
                    type[i] = NHibernateUtil.String;
                }
                if (hql != string.Empty)
                {
                    resultCount = this.genericMgr.Update(hql, para, type);
                }
            }
            return resultCount;
        }

        #endregion

        /*
        [Transaction(TransactionMode.Requires)]
        public void SendFail(User user)
        {
            try
            {
                IList<IssueDetail> issueDetails = this.GetIssueDetail(BusinessConstants.CODE_MASTER_SEND_STATUS_FAIL);
                if (issueDetails != null && issueDetails.Count > 0)
                {
                    IList<EntityPreference> entityPreferences = this.entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[] { BusinessConstants.ENTITY_PREFERENCE_CODE_SMTPEMAILHOST, BusinessConstants.ENTITY_PREFERENCE_CODE_SMTPEMAILADDR, BusinessConstants.ENTITY_PREFERENCE_CODE_SMTPEMAILPASSWD });
                    string SMTPEmailHost = string.Empty;
                    string SMTPEmailPasswd = string.Empty;
                    string emailFrom = string.Empty;

                    foreach (EntityPreference e in entityPreferences)
                    {
                        if (e.Id == (int)EntityPreference.CodeEnum.SMTPEmailHost)
                        {
                            SMTPEmailHost = e.Value;
                        }
                        else if (e.Id == (int)EntityPreference.CodeEnum.SMTPEmailPasswd)
                        {
                            SMTPEmailPasswd = e.Value;
                        }
                        else if (e.Id == (int)EntityPreference.CodeEnum.SMTPEmailAddr)
                        {
                            if (ControlHelper.IsValidEmail(e.Value))
                            {
                                emailFrom = e.Value;

                            }
                            else
                            {
                                IssueLog log = new IssueLog();

                                log.Content = "Mail地址无效 : " + e.Value;
                                log.CreateDate = DateTime.Now;

                                log.Level = IssueLog.LOG_LEVEL_ERROR;
                                this.genericMgr.Create(log);

                                throw new BusinessErrorException("Issue.Error.MailIsInvalid", e.Value);
                            }
                        }
                    }

                    foreach (IssueDetail issueDetail in issueDetails)
                    {
                        string toEmail = issueDetail.Email;
                        IssueMaster issue = issueDetail.Issue;

                        string userMail = string.Empty;
                        if (issue.Email != null && ControlHelper.IsValidEmail(issue.Email))
                        {
                            if (issue.UserName != null && issue.UserName.Length > 0)
                            {
                                userMail = issue.UserName + "," + issue.Email;
                            }
                            else
                            {
                                userMail = issue.Email;
                            }
                        }
                        else
                        {
                            userMail = emailFrom;
                        }

                        string subject = string.Empty;
                        if (issue.Priority == com.Sconit.CodeMaster.IssuePriority.Urgent)
                        {
                            subject = codeMasterMgrE.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_ISSUE_PRIORITY, issue.Priority) + " " + issue.IssueSubject;
                        }
                        else
                        {
                            subject = issue.IssueSubject;
                        }

                        MailPriority mailPriority;
                        if (issue.Priority == com.Sconit.CodeMaster.IssuePriority.Urgent)
                        {
                            mailPriority = MailPriority.High;
                        }
                        else
                        {
                            mailPriority = MailPriority.Normal;
                        }

                        string body = this.GetEmailBody(issue, issueDetail.Level);

                        #region email发送

                        string sendResult = SMTPHelper.SendSMTPEMail(subject, body, emailFrom, toEmail, SMTPEmailHost, SMTPEmailPasswd, userMail, mailPriority);
                        if (sendResult != string.Empty)
                        {
                            throw new BusinessErrorException(sendResult);
                        }
                        #endregion

                    }
                }
                else
                {
                    throw new BusinessErrorException("Issue.Error.MailListIsNull");
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }
         */
        [Transaction(TransactionMode.Unspecified)]
        private IList<IssueDetail> GetIssueDetail(string status)
        {
            try
            {
                DetachedCriteria criteria = DetachedCriteria.For(typeof(IssueDetail));
                criteria.Add(Expression.Eq("EmailStatus", status));
                criteria.Add(Expression.Eq("SMSStatus", status));
                criteria.AddOrder(Order.Asc("EmailStatus"));
                criteria.AddOrder(Order.Asc("SMSStatus"));
                IList<IssueDetail> issueDetailList = this.queryMgr.FindAll<IssueDetail>(criteria);
                if (issueDetailList.Count > 0)
                {
                    return issueDetailList;
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return null;
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<IssueMaster> GetIssueMaster(string[] status)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(IssueMaster));
            criteria.Add(Expression.In("Status", status));
            criteria.AddOrder(Order.Asc("Status"));
            IList<IssueMaster> issueList = queryMgr.FindAll<IssueMaster>(criteria);
            if (issueList.Count > 0)
            {
                return issueList;
            }
            return null;
        }

        #endregion
    }


    [Transactional]
    public class IssueLogMgrImpl : IIssueLogMgr
    {
        public IGenericMgr genericMgr { get; set; }

        #region IssueLog
        public void LogError(string issue, string content)
        {
            this.Log(issue, null, content, IssueLog.LOG_LEVEL_ERROR);
        }
        public void LogFatal(string issue, string content)
        {
            this.Log(issue, null, content, IssueLog.LOG_LEVEL_FATAL);
        }
        public void LogInfo(string issue, string content)
        {
            this.Log(issue, null, content, IssueLog.LOG_LEVEL_INFO);
        }
        public void LogWarn(string issue, string content)
        {
            this.Log(issue, null, content, IssueLog.LOG_LEVEL_WARN);
        }
        public void LogDebug(string issue, string content)
        {
            this.Log(issue, null, content, IssueLog.LOG_LEVEL_DEBUG);
        }

        public void LogError(string issue, IssueDetail issueDetail, string content)
        {
            this.Log(issue, issueDetail, content, IssueLog.LOG_LEVEL_ERROR);
        }
        public void LogFatal(string issue, IssueDetail issueDetail, string content)
        {
            this.Log(issue, issueDetail, content, IssueLog.LOG_LEVEL_FATAL);
        }
        public void LogInfo(string issue, IssueDetail issueDetail, string content)
        {
            this.Log(issue, issueDetail, content, IssueLog.LOG_LEVEL_INFO);
        }
        public void LogWarn(string issue, IssueDetail issueDetail, string content)
        {
            this.Log(issue, issueDetail, content, IssueLog.LOG_LEVEL_WARN);
        }
        public void LogDebug(string issue, IssueDetail issueDetail, string content)
        {
            this.Log(issue, issueDetail, content, IssueLog.LOG_LEVEL_DEBUG);
        }
        [Transaction(TransactionMode.Requires)]
        private void Log(string issue, IssueDetail issueDetail, string content, string logLevel)
        {
            IssueLog log = new IssueLog();
            log.Level = logLevel;
            log.Content = content;

            if (!string.IsNullOrWhiteSpace(issue))
            {
                log.Issue = issue;
            }
            if (issueDetail != null)
            {
                log.Email = issueDetail.User.Email;
                log.MPhone = issueDetail.User.MobilePhone;
                log.IssueDetail = issueDetail.Id;
                log.IsSMS = issueDetail.IsSMS;
                log.IsEmail = issueDetail.IsEmail;
                log.EmailStatus = issueDetail.EmailStatus.ToString();
                log.SMSStatus = issueDetail.SMSStatus.ToString();
            }
            this.genericMgr.Create(log);
        }
        #endregion
    }

    [Transactional]
    public class IssueUtilMgrImpl : IIssueUtilMgr
    {

        public IGenericMgr genericMgr { get; set; }

        public bool HavePermission(string issueCode, com.Sconit.CodeMaster.IssueStatus status)
        {
            return HavePermission(issueCode, SecurityContextHolder.Get(), status);
        }
        [Transaction(TransactionMode.Unspecified)]
        public bool HavePermission(string issueCode, User user, com.Sconit.CodeMaster.IssueStatus status)
        {
            string hql = "select count(*) from IssueDetail where IsActive =true and IssueCode =? and UserId = ? ";

            if (status == com.Sconit.CodeMaster.IssueStatus.InProcess)
            {
                hql += "and IsDefault != true";
            }
            IList<long> count = this.genericMgr.FindAll<long>(hql, new object[] { issueCode, user.Id });

            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }
            return false;
        }
    }
}