using System;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using NHibernate.Criterion;
using Castle.Services.Transaction;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections;
using com.Sconit.Utility;
using com.Sconit.Entity.CUST;
using NHibernate.Type;
using com.Sconit.Entity.ISI;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Utility;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using System.Net.Mail;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class TaskMgrImpl : BaseMgr, ITaskMgr
    {
        #region 变量
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IEmailMgr emailMgr { get; set; }
        #endregion

        #region public methods


        [Transaction(TransactionMode.Requires)]
        public void CreateTask(TaskMaster task)
        {
            #region 处理代码
            string prefix = string.Empty;
            if (task.Type == CodeMaster.TaskType.Plan)
            {
                prefix = ISIConstants.CODE_PREFIX_PLAN;
            }
            if (task.Type == CodeMaster.TaskType.Issue)
            {
                prefix = ISIConstants.CODE_PREFIX_ISSUE;
            }
            if (task.Type == CodeMaster.TaskType.Improve)
            {
                prefix = ISIConstants.CODE_PREFIX_IMPROVE;
            }
            if (task.Type == CodeMaster.TaskType.Change)
            {
                prefix = ISIConstants.CODE_PREFIX_CHANGE;
            }
            if (task.Type == CodeMaster.TaskType.Privacy)
            {
                prefix = ISIConstants.CODE_PREFIX_PRIVACY;
            }
            if (task.Type == CodeMaster.TaskType.Response)
            {
                prefix = ISIConstants.CODE_PREFIX_RESPONSE;
            }
            if (task.Type == CodeMaster.TaskType.Project)
            {
                prefix = ISIConstants.CODE_PREFIX_PROJECT;
            }
            if (task.Type == CodeMaster.TaskType.Issue)
            {
                prefix = ISIConstants.CODE_PREFIX_PROJECT_ISSUE;
            }
            if (task.Type == CodeMaster.TaskType.Audit)
            {
                prefix = ISIConstants.CODE_PREFIX_AUDIT;
            }
            if (task.Type == CodeMaster.TaskType.Change)
            {
                prefix = ISIConstants.CODE_PREFIX_ENGINEERING_CHANGE;
            }
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = ISIConstants.CODE_PREFIX_ISI;
            }

            task.Code = numberControlMgr.GetTaskNo(prefix);
            #endregion

            task.Status = CodeMaster.TaskStatus.Create;

            genericMgr.Create(task);

        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateTask(TaskMaster task)
        {
            genericMgr.Update(task);
        }

        [Transaction(TransactionMode.Requires)]
        public void SubmitTask(string code, User user)
        {
            TaskMaster task = genericMgr.FindById<TaskMaster>(code);
            SubmitTask(task, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void SubmitTask(TaskMaster task, User user)
        {
            try
            {
                if (task.Status != CodeMaster.TaskStatus.Create)
                {
                    throw new BusinessException(Resources.ISI.TaskMaster.TaskMaster_Errors_StatusErrorWhenSubmit, task.Status.ToString());
                }

                DateTime dateTimeNow = DateTime.Now;

                HandleAssign(task, dateTimeNow, user);

                task.LastModifyDate = dateTimeNow;
                task.LastModifyUserId = user.Id;
                task.LastModifyUserName = user.Name.Trim();
                this.UpdateTask(task);

                if (task.Status == CodeMaster.TaskStatus.Assign && task.IsAutoStart)
                {
                    this.ConfirmTask(task.Code, user);
                }
                else if ((!task.IsNoSend
                                && !ISIHelper.Contains(task.AssignUserId.ToString(), user.Id.ToString())//分派人
                                && !ISIHelper.Contains(task.AssignUpUser, user.Code)
                    //&& !user.Permissions.Contains(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                    ))
                {

                    IList<UserSub> userSubList = SubmitUserSub(task, user);
                    #region 处理发送用户
                    if (userSubList != null && userSubList.Count > 0)
                    {
                        Remind(task, userSubList, user);
                    }
                    #endregion

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ConfirmTask(string taskCode, User user)
        {
            TaskMaster task = genericMgr.FindById<TaskMaster>(taskCode);
            ConfirmTask(task, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void AssignTask(string taskCode,string assignStartUser, DateTime planStartDate, DateTime planCompleteDate, string desc2, string expectedResults, User user)
        {
            TaskMaster task = genericMgr.FindById<TaskMaster>(taskCode);

            //检查状态
            if (task.Status != CodeMaster.TaskStatus.Submit)
            {
                throw new BusinessException(Resources.ISI.TaskMaster.TaskMaster_Errors_StatusErrorWhenAssign, task.Status.ToString());
            }

            DateTime dateTimeNow = DateTime.Now;
           

            bool isSendRemind = false;
            bool isUserSubSend = false;

          
            if (task.Status ==  CodeMaster.TaskStatus.Submit)
            {
                task.Status = CodeMaster.TaskStatus.Assign;
                isSendRemind = true;
            }
         
            task.Description2 = desc2;
            task.ExpectedResults = expectedResults;
            task.PlanStartDate = planStartDate;
            task.PlanCompleteDate = planCompleteDate;

            if (task.Status ==  CodeMaster.TaskStatus.Assign)
            {
                task.AssignDate = dateTimeNow;
                task.AssignUserId = user.Id;
                task.AssignUserName = user.Name.Trim();
            }

            task.LastModifyDate = dateTimeNow;
            task.LastModifyUserId = user.Id;
            task.LastModifyUserName = user.Name.Trim();
            this.UpdateTask(task);

            if (task.IsAutoStart)
            {
                this.ConfirmTask(taskCode, user);
            }

            if (!(task.IsAutoStart && task.IsAutoStatus))
            {
                List<UserSub> userSubList = new List<UserSub>();
                if (isUserSubSend)
                {
                    //订阅提醒
                    IList<UserSub> userSubListT = SubmitUserSub(task, user);

                    if (userSubListT != null && userSubListT.Count > 0)
                    {
                        userSubList.AddRange(userSubListT);
                    }
                }

                if (isSendRemind)
                {
                    //分派提醒
                    IList<UserSub> userSubListT = this.GenerateUserSub(task, task.StartedUser, false, user);
                    if (userSubListT != null && userSubListT.Count > 0)
                    {
                        userSubList.AddRange(userSubListT);
                    }
                }
                if (userSubList != null && userSubList.Count > 0)
                {
                    Remind(task, userSubList, user);
                }
            }
        }

        /// <summary>
        /// 任务开始动作
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="user"></param>
        [Transaction(TransactionMode.Requires)]
        public void ConfirmTask(TaskMaster task, User user)
        {
            //检查状态
            if (task.Status != CodeMaster.TaskStatus.Assign)
            {
                throw new BusinessException(Resources.ISI.TaskMaster.TaskMaster_Errors_StatusErrorWhenConfirm, task.Status.ToString());
            }

            DateTime nowDate = DateTime.Now;

            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI2;

            task.Status = CodeMaster.TaskStatus.InProcess;
            task.StartDate = nowDate;
            task.StartUserId = user.Id;
            task.StartUserName = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUserId = user.Id;
            task.LastModifyUserName = user.Name.Trim();
            this.UpdateTask(task);

            if (task.IsAutoComplete)
            {
                this.CompleteTask(task, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ConfirmTask(string taskCode, DateTime planStartDate, DateTime planCompleteDate, string desc2, string expectedResults, User user)
        {
            TaskMaster task = genericMgr.FindById<TaskMaster>(taskCode);

            //检查状态
            if (task.Status != CodeMaster.TaskStatus.Assign)
            {
                throw new BusinessException(Resources.ISI.TaskMaster.TaskMaster_Errors_StatusErrorWhenConfirm, task.Status.ToString());
            }

            DateTime nowDate = DateTime.Now;

            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI2;

            task.Description2 = desc2;
            task.ExpectedResults = expectedResults;
            task.PlanStartDate = planStartDate;
            task.PlanCompleteDate = planCompleteDate;
            task.Status = CodeMaster.TaskStatus.InProcess;
            task.StartDate = nowDate;
            task.StartUserId = user.Id;
            task.StartUserName = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUserId = user.Id;
            task.LastModifyUserName = user.Name.Trim();
            this.UpdateTask(task);

            if (task.IsAutoComplete)
            {
                this.CompleteTask(taskCode, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CompleteTask(string taskCode, User user)
        {
            TaskMaster task = genericMgr.FindById<TaskMaster>(taskCode);
            this.CompleteTask(task, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void CompleteTask(TaskMaster task, User user)
        {
            //检查状态
            if (task.Status != CodeMaster.TaskStatus.InProcess)
            {
                throw new BusinessException(Resources.ISI.TaskMaster.TaskMaster_Errors_StatusErrorWhenComplete, task.Status.ToString());
            }

            DateTime nowDate = DateTime.Now;

            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI4;
            task.Status = CodeMaster.TaskStatus.Complete;
            task.CompleteDate = nowDate;
            task.CompleteUserId = user.Id;
            task.CompleteUserName = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUserId = user.Id;
            task.LastModifyUserName = user.Name.Trim();
            this.UpdateTask(task);

            if (task.IsAutoClose)
            {
                this.CloseTask(task.Code, user);
            }
            else if (!task.IsCompleteNoRemind)
            {
                //提醒关闭
                IList<UserSub> userSubList = this.GenerateUserSub(task, task.CreateUserId.ToString() + ISIConstants.ISI_USER_SEPRATOR + task.SubmitUserId.ToString() + ISIConstants.ISI_USER_SEPRATOR + task.AssignUserId.ToString(), false, user);
                Remind(task, userSubList, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CompleteTask(string taskCode, string desc2, User user)
        {
            TaskMaster task = genericMgr.FindById<TaskMaster>(taskCode);

            //检查状态
            if (task.Status != CodeMaster.TaskStatus.InProcess)
            {
                throw new BusinessException(Resources.ISI.TaskMaster.TaskMaster_Errors_StatusErrorWhenComplete, task.Status.ToString());
            }

            DateTime nowDate = DateTime.Now;

            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI4;
            task.Description2 = desc2;
            task.Status = CodeMaster.TaskStatus.Complete;
            task.CompleteDate = nowDate;
            task.CompleteUserId = user.Id;
            task.CompleteUserName = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUserId = user.Id;
            task.LastModifyUserName = user.Name.Trim();
            this.UpdateTask(task);

            if (!task.IsCompleteNoRemind)
            {
                //提醒关闭
                IList<UserSub> userSubList = this.GenerateUserSub(task, task.CreateUserId.ToString() + ISIConstants.ISI_USER_SEPRATOR + task.SubmitUserId.ToString() + ISIConstants.ISI_USER_SEPRATOR + task.AssignUserId.ToString(), false, user);
                Remind(task, userSubList, user);
            }
            if (task.IsAutoClose)
            {
                this.CloseTask(taskCode, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseTask(string taskCode, User user)
        {
            TaskMaster task = genericMgr.FindById<TaskMaster>(taskCode);

            //检查状态
            if (task.Status != CodeMaster.TaskStatus.Complete)
            {
                throw new BusinessException(Resources.ISI.TaskMaster.TaskMaster_Errors_StatusErrorWhenClose, task.Status.ToString());
            }

            DateTime nowDate = DateTime.Now;

            task.Status = CodeMaster.TaskStatus.Close;
            task.Flag = ISIConstants.CODE_MASTER_ISI_FLAG_DI5;
            task.Color = string.Empty;

            task.CloseDate = nowDate;
            task.CloseUserId = user.Id;
            task.CloseUserName = user.Name.Trim();
            task.LastModifyDate = nowDate;
            task.LastModifyUserId = user.Id;
            task.LastModifyUserName = user.Name.Trim();
            this.UpdateTask(task);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateTaskDetail(TaskMaster task, string level, IList<UserSub> userSubList, bool isEmailException, bool isSMSException, User user)
        {
            DateTime now = DateTime.Now;
            foreach (UserSub userSub in userSubList)
            {
                TaskDetail taskDetail = new TaskDetail();
                taskDetail.TaskCode = task.Code;
                taskDetail.Status = task.Status;
                taskDetail.IsEmail = userSub.IsEmail;
                taskDetail.IsSMS = userSub.IsSMS;
                taskDetail.BackYards = task.BackYards;
                taskDetail.TaskSubType = task.TaskSubType;
                taskDetail.Description1 = task.Description1;
                taskDetail.Description2 = task.Description2;
                taskDetail.UserName = task.UserName;
                taskDetail.ExpectedResults = task.ExpectedResults;
                taskDetail.Flag = task.Flag;
                taskDetail.Color = task.Color;
                //  taskDetail.FailureMode = task.FailureMode != null ? task.FailureMode.Code : task.FailureModeCode;
                taskDetail.PlanCompleteDate = task.PlanCompleteDate;
                taskDetail.PlanStartDate = task.PlanStartDate;
                taskDetail.Subject = task.Subject;
                taskDetail.UserEmail = task.Email;
                taskDetail.UserMobilePhone = task.MobilePhone;
                taskDetail.CreateDate = now;
                taskDetail.CreateUserId = user.Id;
                taskDetail.CreateUserName = user.Name;
                taskDetail.LastModifyDate = now;
                taskDetail.LastModifyUserId = user.Id;
                taskDetail.LastModifyUserName = user.Name;
                taskDetail.Priority = task.Priority;
                if (isEmailException)
                {
                    taskDetail.EmailStatus = ISIConstants.CODE_MASTER_ISI_SEND_STATUS_FAIL;
                }
                else
                {
                    taskDetail.EmailStatus = taskDetail.IsEmail ? ISIConstants.CODE_MASTER_ISI_SEND_STATUS_SUCCESS : ISIConstants.CODE_MASTER_ISI_SEND_STATUS_NOTSEND;
                }

                if (taskDetail.IsEmail)
                {
                    taskDetail.EmailCount += 1;
                }

                if (isSMSException)
                {
                    taskDetail.SMSStatus = ISIConstants.CODE_MASTER_ISI_SEND_STATUS_FAIL;
                }
                else
                {
                    taskDetail.SMSStatus = taskDetail.IsSMS ? ISIConstants.CODE_MASTER_ISI_SEND_STATUS_SUCCESS : ISIConstants.CODE_MASTER_ISI_SEND_STATUS_NOTSEND;
                }

                if (taskDetail.IsSMS)
                {
                    taskDetail.SMSCount += 1;
                }

                taskDetail.Level = level;
                taskDetail.Email = userSub.Email;
                taskDetail.MobilePhone = userSub.MobilePhone;

                taskDetail.Receiver = userSub.Code;

                taskDetail.IsActive = true;
                genericMgr.Create(taskDetail);
            }
        }



        public void CreateTaskStatus(TaskStatus taskStatus,User currentUser,bool isComplete)
        {
            TaskMaster task = genericMgr.FindById<TaskMaster>(taskStatus.TaskCode);
            //自动开始此任务
            if (task.Status == CodeMaster.TaskStatus.Assign)
            {
                this.ConfirmTask(task.Code, currentUser);
            }
            genericMgr.Create(taskStatus);

            if (taskStatus.IsCurrentStatus
                        || taskStatus.IsRemindAssignUser
                        || taskStatus.IsRemindCommentUser
                        || taskStatus.IsRemindCreateUser
                        || taskStatus.IsRemindStartUser
                        || isComplete)
            {

                if (taskStatus.IsCurrentStatus || isComplete)
                {
                    if (isComplete && task.Status == CodeMaster.TaskStatus.InProcess)
                    {
                        CompleteTask(task, currentUser);
                    }
                    this.UpdateTaskStatus(taskStatus, task);
                }

                this.RemindStatus(task, taskStatus);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateTaskStatus(TaskStatus taskStatus, TaskMaster task)
        {
            task.Flag = taskStatus.Flag;
            task.Color = taskStatus.Color;
            task.LastModifyDate = taskStatus.LastModifyDate;
            task.LastModifyUserId = taskStatus.LastModifyUserId;
            task.LastModifyUserName = taskStatus.LastModifyUserName;
            this.UpdateTask(task);
        }
        #endregion

        #region private methods
        private void Remind(TaskMaster task, string level, double minutes, IList<UserSub> userSubList, User operationUser)
        {
            if (userSubList == null || userSubList.Count == 0) return;

            StringBuilder toEmail = new StringBuilder();
            StringBuilder toMobliePhone = new StringBuilder();

            foreach (UserSub userSub in userSubList)
            {
                if (userSub.IsEmail)
                {
                    if (toEmail.Length != 0)
                    {
                        toEmail.Append(";");
                    }
                    toEmail.Append(userSub.Email);
                }
                if (userSub.IsSMS)
                {
                    if (toMobliePhone.Length != 0)
                    {
                        toMobliePhone.Append(";");
                    }
                    toMobliePhone.Append(userSub.MobilePhone);
                }
            }

            string emailBody = string.Empty;
            string smsBody = string.Empty;

            #region 获取内容

            if (toEmail.Length > 0)
            {
                emailBody = this.GetEmailBody(task, level, minutes, operationUser);
            }

            if (toMobliePhone.Length > 0)
            {
                smsBody = this.GetSMSBody(task, level, minutes, operationUser);
            }

            #endregion

            string userMail = string.Empty;
            MailPriority mailPriority;

            #region email标题 回复人 优先级

            string subject = this.GetSubject(operationUser.Code, operationUser.Name, task.Code, task.Type, task.Priority, task.Subject, level, task.Status);

            if (operationUser != null && !string.IsNullOrEmpty(operationUser.Email) && ISIHelper.IsValidEmail(operationUser.Email))
            {
                userMail = operationUser.Name + "," + operationUser.Email;
            }
            else if (level == ISIConstants.ISI_LEVEL_BASE && !string.IsNullOrEmpty(task.Email) && ISIHelper.IsValidEmail(task.Email))
            {
                if (!string.IsNullOrEmpty(task.UserName))
                {
                    userMail = task.UserName + "," + task.Email;
                }
                else
                {
                    userMail = task.Email;
                }
            }

            if (task.Priority == CodeMaster.TaskPriority.Urgent)
            {
                mailPriority = MailPriority.High;
            }
            else
            {
                mailPriority = MailPriority.Normal;
            }

            #endregion

            bool isEmailException = false;
            bool isSMSException = false;

            #region 邮件与短信发送

            if (!string.IsNullOrEmpty(emailBody) && !string.IsNullOrEmpty(toEmail.ToString()))
            {
                //todo
                emailMgr.SendEmail(subject, emailBody, toEmail.ToString(), string.Empty, mailPriority);
                
                // isEmailException = this.SendEmail(task.Code, subject, emailBody, toEmail.ToString(), userMail, mailPriority, operationUser);
                //isEmailException = this.SendEmail(task.Code, subject, emailBody, "tiansu@yfgm.com.cn", userMail, mailPriority, operationUser);
            }
            if (!string.IsNullOrEmpty(smsBody) && !string.IsNullOrEmpty(toMobliePhone.ToString()))
            {
                isSMSException = this.SendSMS(task.Code, toMobliePhone.ToString(), smsBody, operationUser);
            }
            #endregion

            #region 记录上报TaskDetail
            CreateTaskDetail(task, level, userSubList, isEmailException, isSMSException, operationUser);
            #endregion
        }

        private IList<UserSub> SubmitUserSub(TaskMaster task, User user)
        {
            IList<UserSub> userSubList = null;

            if (task.Status == CodeMaster.TaskStatus.Submit && !string.IsNullOrEmpty(ISIHelper.EditUser(task.AssignUserId.ToString())))
            {
                userSubList = GenerateUserSub(task, task.AssignUserId.ToString(), false, user);
            }
            else if (task.Status == CodeMaster.TaskStatus.Assign)
            {
                if (!string.IsNullOrEmpty(ISIHelper.EditUser(task.StartedUser)))
                {
                    userSubList = GenerateUserSub(task, task.StartedUser, false, user);
                }
                else if (!string.IsNullOrEmpty(ISIHelper.EditUser(task.StartUserId.ToString())))
                {
                    userSubList = GenerateUserSub(task, task.StartUserId.ToString(), false, user);
                }
            }
            return userSubList;
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<UserSub> GenerateUserSub(TaskMaster task, User user)
        {
            return this.GenerateUserSub(task, string.Empty, true, user);
        }

        //查询所有管理员，不考虑订阅
        [Transaction(TransactionMode.Unspecified)]
        private IList<UserSub> GenerateUserSub(string taskSubType, User user)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(User));
            ProjectionList projectionList = Projections.ProjectionList()
                                .Add(Projections.Property("Code"), "Code")
                                .Add(Projections.Property("IsActive"), "IsEmail")
                                .Add(Projections.Property("IsActive"), "IsSMS")
                                .Add(Projections.Property("Email"), "Email")
                                .Add(Projections.Property("MobliePhone"), "MobilePhone");

            DetachedCriteria[] taCrieteria = GetTaskAdminPermissionCriteria();

            criteria.Add(
                    Expression.Or(
                        Subqueries.PropertyIn("Code", taCrieteria[0]),
                        Subqueries.PropertyIn("Code", taCrieteria[1])));

            criteria.SetProjection(Projections.Distinct(projectionList));
            criteria.Add(Expression.Eq("IsActive", true));

            criteria.SetResultTransformer(Transformers.AliasToBean(typeof(UserSub)));
            IList<UserSub> userSubList = genericMgr.FindAll<UserSub>(criteria);

            //GenerateUserSub(taskSubType, ref userSubList);
            return userSubList;
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<UserSub> GenerateUserSub(TaskMaster task, string userCodes, bool isUserSub, User user)
        {
            return GenerateUserSub(task.Type, task.TaskSubType, task.Code, userCodes, isUserSub, user);
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<UserSub> GenerateUserSub(CodeMaster.TaskType taskType, string taskSubTypeCode, string taskCode, string userIds, bool isUserSub, User user)
        {
            string[] userCodeArray = null;
            if (!string.IsNullOrEmpty(userIds))
            {
                userCodeArray = userIds.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                userCodeArray = userCodeArray.Where(u => !string.IsNullOrEmpty(u)).ToArray<string>();
            }

            if (string.IsNullOrEmpty(userIds) || userCodeArray == null || userCodeArray.Length == 0)
            {
                return new List<UserSub>();
            }
            IList<UserSub> userSubList = new List<UserSub>();
            if (!isUserSub)
            {
                string users = string.Join(",", userCodeArray);

                IList<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@TaskType", taskType));
                sqlParam.Add(new SqlParameter("@TaskSubType", taskSubTypeCode));
                sqlParam.Add(new SqlParameter("@TaskCode", taskCode));
                sqlParam.Add(new SqlParameter("@UserCodes", users));
                sqlParam.Add(new SqlParameter("@CurrentUser", user.Code));

                var objList = this.genericMgr.FindAllWithNativeSql<object[]>("exec USP_Search_TaskEmail ?,?,?,?,?",
                new object[] { (int)taskType, taskSubTypeCode, taskCode, users, user.Id.ToString() },
                new IType[] { NHibernateUtil.String, NHibernateUtil.String, NHibernateUtil.String, NHibernateUtil.String, NHibernateUtil.String });


                foreach (var obj in objList)
                {
                    UserSub userSub = new UserSub();
                    userSub.Code = obj[0].ToString();
                    userSub.IsEmail = true;
                    userSub.Email = obj[1].ToString();
                    userSubList.Add(userSub);
                }
            }
            else
            {

                //考虑是否订阅,有权限才能订阅，因此不考虑权限，
                DetachedCriteria criteria = DetachedCriteria.For(typeof(UserSubscription));
                criteria.CreateAlias("User", "u", JoinType.InnerJoin);
                ProjectionList projectionList = Projections.ProjectionList()
                                    .Add(Projections.Property("u.Code"), "Code")
                                    .Add(Projections.Property("IsEmail"), "IsEmail")
                                    .Add(Projections.SqlProjection(@"isnull(Email,USR_Email) as Email", new String[] { "Email" }, new IType[] { NHibernateUtil.String }))
                                    .Add(Projections.Property("IsSMS"), "IsSMS")
                                    .Add(Projections.SqlProjection(@"isnull(MobilePhone, USR_MPhone) as MobilePhone", new String[] { "MobilePhone" }, new IType[] { NHibernateUtil.String }));
                //
                criteria.SetProjection(Projections.Distinct(projectionList));
                criteria.Add(Expression.Eq("u.IsActive", true));
                criteria.Add(Expression.Eq("TaskSubType", taskSubTypeCode));

                if (userCodeArray != null && userCodeArray.Length > 0)
                {
                    criteria.Add(Expression.In("u.Code", userCodeArray));
                }
                if (user != null)
                {
                    criteria.Add(Expression.Not(Expression.Eq("u.Code", user.Code)));
                }

                criteria.Add(Expression.Or(
                    Expression.And(Expression.Eq("IsEmail", true), Expression.Or(Expression.IsNotNull("Email"), Expression.IsNotNull("u.Email"))),
                    Expression.And(Expression.Eq("IsSMS", true), Expression.Or(Expression.IsNotNull("MobilePhone"), Expression.IsNotNull("u.MobliePhone")))
                    ));

                criteria.SetResultTransformer(Transformers.AliasToBean(typeof(UserSub)));
                userSubList = genericMgr.FindAll<UserSub>(criteria);
            }

            return userSubList;
        }

        private void Remind(TaskMaster task, IList<UserSub> userSubList, User operationUser)
        {
            this.Remind(task, ISIConstants.ISI_LEVEL_BASE, 0, userSubList, operationUser);
        }

        private void Remind(TaskMaster task, IList<UserSub> userSubList, string helpContent, User operationUser)
        {
            task.HelpContent = helpContent;
            this.Remind(task, ISIConstants.ISI_LEVEL_HELP, 0, userSubList, operationUser);
        }

        protected void RemindStatus(TaskMaster task, TaskStatus taskStatus)
        {
            #region 获取用户列表
            StringBuilder users = new StringBuilder();
            if (taskStatus.IsRemindCreateUser)
            {
                users.Append(task.CreateUserId.ToString());
                if (!string.IsNullOrEmpty(task.SubmitUserId.ToString()))
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    users.Append(task.SubmitUserId.ToString());
                }
            }
            if (taskStatus.IsRemindAssignUser)
            {
                if (!string.IsNullOrEmpty(task.AssignUserId.ToString()))
                {
                    if (users.Length != 0)
                    {
                        users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    }
                    users.Append(task.AssignUserId.ToString());
                }
                else if (task.AssignUserId != null)
                {
                    if (users.Length != 0)
                    {
                        users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    }
                    users.Append(task.AssignUserId.ToString());
                }
            }
            if (taskStatus.IsRemindStartUser && task.StartedUser != null)
            {
                if (users.Length != 0)
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR);
                }
                users.Append(task.StartedUser);
            }
            if (taskStatus.IsRemindCommentUser)
            {
                //所有评论人
                //   var commentList = commentDetailMgrE.GetComment(task.Code);
                var commentList = new List<CommentDetail>();
                if (commentList != null && commentList.Count > 0)
                {
                    string commentUsers = string.Join(";", commentList.Select(t => t.CreateUser).Distinct().ToArray<string>());
                    if (users.Length != 0)
                    {
                        users.Append(ISIConstants.ISI_USER_SEPRATOR);
                    }
                    users.Append(commentUsers);

                    task.CommentDetail = commentList[0];
                }
            }
            #endregion

            User operationUser = new User();
            operationUser.Id = taskStatus.LastModifyUserId;
            operationUser.FirstName = taskStatus.LastModifyUserName;
            IList<UserSub> userSubList = this.GenerateUserSub(task, users.ToString(), false, operationUser);

            task.TaskStatus = taskStatus;
            this.Remind(task, ISIConstants.ISI_LEVEL_STATUS, 0, userSubList, operationUser);
        }

        [Transaction(TransactionMode.Requires)]
        private bool SendSMS(string code, string toMobliePhone, string msg, User user)
        {
            bool isSMSException = false;
            //try
            //{
            //    emppMgrE.AsyncSend(toMobliePhone, msg, user);
            //}
            //catch (Exception e)
            //{
            //    isSMSException = true;
            //    log.Error("Code=" + code + ",toMobliePhone=" + toMobliePhone + ",operator=" + user.Code + ",e=" + e.Message, e);
            //}

            return isSMSException;
        }

        private string GetDesc(CodeMaster.TaskType type, string userCode)
        {
            return string.Empty;
            //if (type == ISIConstants.ISI_TASK_TYPE_PLAN)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Plan", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_ISSUE)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Issue", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_IMPROVE)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Improve", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_CHANGE)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Change", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_PRIVACY)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Privacy", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_RESPONSE)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Response", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_PROJECT)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Project", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_AUDIT)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Audit", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.PrjIss", userCode);
            //}
            //if (type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
            //{
            //    return languageMgrE.TranslateMessage("ISI.TSK.Enc", userCode);
            //}
            //return languageMgrE.TranslateMessage("ISI.TSK.Task", userCode);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected string GetEmailBody(TaskMaster task, string level, double minutes, User operationUser)
        {

            StringBuilder content = new StringBuilder();
            try
            {
                content.Append("<p style='font-size:15px;'>");
                string separator = ISIConstants.EMAIL_SEPRATOR;
                DateTime now = DateTime.Now;

                var companyName = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.CompanyName);
                ISIHelper.AppendTestText(companyName, content, separator);

                if (level == ISIConstants.ISI_LEVEL_HELP)
                {
                    content.Append(separator);
                    content.Append("<U>求助</U>：" + task.HelpContent);
                    content.Append(separator);
                    content.Append(separator);
                    content.Append(operationUser.Name);
                    content.Append(separator);
                    content.Append(now.ToString("yyyy-MM-dd HH:mm"));
                    content.Append(separator);
                    content.Append(separator);
                }
                if (level == ISIConstants.ISI_LEVEL_STATUS)
                {
                    PutStatusText(task, content, separator);
                    PutCommentText(task, content, separator);
                }
                if (level == ISIConstants.ISI_LEVEL_COMMENT)
                {
                    PutCommentText(task, content, separator);
                    PutStatusText(task, content, separator);
                }
                if (level == ISIConstants.ISI_LEVEL_STARTPERCENT)
                {
                    content.Append(separator);
                    content.Append("按照预计完成时间 " + (task.PlanCompleteDate.HasValue ? task.PlanCompleteDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty) + " 已经过去 " + (task.StartPercent.Value * 100).ToString("0.####") + "% 请注意进度。");
                    content.Append(separator);
                    content.Append(separator);
                    content.Append(separator);
                }
                if (level == ISIConstants.ISI_LEVEL_OPEN)
                {
                    content.Append(separator);
                    content.Append("已经到了预计开始时间 " + (task.PlanStartDate.HasValue ? task.PlanStartDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty) + "，提醒开始执行！");
                    content.Append(separator);
                    content.Append(separator);
                    content.Append(separator);
                }
                if (level == ISIConstants.ISI_LEVEL_COMPLETE)
                {
                    content.Append(separator);
                    content.Append("已经超过预计完成时间 " + (task.PlanCompleteDate.HasValue ? task.PlanCompleteDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty) + "。请注意！");
                    content.Append(separator);
                    content.Append(separator);
                    content.Append(separator);
                }
                if (!string.IsNullOrEmpty(task.Description1))
                {
                    content.Append("<U>描述</U>: " + task.Description1.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                }
                content.Append(separator);
                if (!string.IsNullOrEmpty(task.Description2))
                {
                    content.Append("<I>补充描述</I>: " + task.Description2.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                }
                content.Append(separator + separator);
                if (!string.IsNullOrEmpty(task.ExpectedResults))
                {
                    content.Append("<U>预期结果/达成结果</U>: ");
                    content.Append(task.ExpectedResults.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                    content.Append(separator + separator);
                }
                content.Append("<U>" + this.GetDesc(task.Type, operationUser.Name) + "</U>: ");
                content.Append(task.Code);
                if (task.Priority == CodeMaster.TaskPriority.Urgent)
                {
                    // content.Append("[" + codeMasterMgrE.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_PRIORITY, task.Priority).Description + "]");
                }
                content.Append(separator);
                //  content.Append("<U>状态</U>: " + systemMgr.r( this.codeMasterMgrE.LoadCodeMaster(ISIConstants.CODE_MASTER_ISI_STATUS, task.Status).Description);

                if (!string.IsNullOrEmpty(task.Subject))
                {
                    content.Append(separator);
                    content.Append("<U>标题</U>: " + task.Subject);
                }
                DateTime date = DateTime.Now;

                if (level == ISIConstants.ISI_LEVEL_BASE)//提醒
                {
                    content.Append(separator);
                    if (task.Status == CodeMaster.TaskStatus.Submit)
                    {
                        content.Append("<U>发送类型</U>: 分派提醒");
                    }
                    else if (task.Status == CodeMaster.TaskStatus.Assign)
                    {
                        content.Append("<U>发送类型</U>: 执行提醒");
                    }
                    else
                    {
                        content.Append("<U>发送类型</U>: 提醒");
                    }
                }
                else if (level == ISIConstants.ISI_LEVEL_COMMENT)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 评论");
                }
                else if (level == ISIConstants.ISI_LEVEL_STATUS)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 进展");
                }
                else if (level == ISIConstants.ISI_LEVEL_HELP)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 求助");
                }
                else if (level == ISIConstants.ISI_LEVEL_STARTPERCENT)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 执行进度提醒");
                }
                else if (level == ISIConstants.ISI_LEVEL_OPEN)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 开始执行提醒");
                }
                else if (level == ISIConstants.ISI_LEVEL_COMPLETE)
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 逾期完成提醒");
                }
                else//上报
                {
                    content.Append(separator);
                    content.Append("<U>发送类型</U>: 上报" + level + "级");

                    //分派提醒
                    if (task.Status == CodeMaster.TaskStatus.Submit && task.SubmitDate.HasValue)
                    {
                        string diff = ISIHelper.GetDiff(task.SubmitDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("<U>分派超时</U>: " + diff);
                        }
                        date = task.SubmitDate.Value;
                    }
                    //开始提醒
                    if ((task.Status == CodeMaster.TaskStatus.Assign && task.AssignDate.HasValue))
                    {
                        string diff = ISIHelper.GetDiff(task.AssignDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("<U>确认超时</U>: " + diff);
                        }
                        date = task.AssignDate.Value;
                    }
                    //关闭提醒
                    else if (task.Status == CodeMaster.TaskStatus.Complete && task.CompleteDate.HasValue)
                    {
                        string diff = ISIHelper.GetDiff(task.CompleteDate.Value.AddMinutes(minutes));
                        if (!string.IsNullOrEmpty(diff))
                        {
                            content.Append(separator);
                            content.Append("<U>关闭超时</U>: " + diff);
                        }
                        date = task.CompleteDate.Value;
                    }
                }

                if (!string.IsNullOrEmpty(task.BackYards))
                {
                    content.Append(separator);
                    content.Append("<U>追溯码</U>: " + task.BackYards);
                }
                content.Append(separator);
                content.Append("<U>时间</U>: " + date.ToString("yyyy-MM-dd HH:mm") + separator);

                if (task.Type == CodeMaster.TaskType.Project)
                {
                    content.Append("<U>项目</U>: " + task.TaskSubTypeDesc + separator);
                    content.Append("<U>阶段</U>: " + task.Phase + separator);
                    //content.Append("序号: " + task.Seq + separator);
                }
                else
                {
                    //content.Append("<U>类型</U>: " + (task.TaskSubType != null ? task.TaskSubType.Description : task.TaskSubTypeDesc) + separator);
                    //if (task.FailureMode != null || !string.IsNullOrEmpty(task.FailureModeCode))
                    //{
                    //    content.Append("<U>失效模式</U>: " + (task.FailureMode != null ? task.FailureMode.Code : task.FailureModeCode) + separator);
                    //}
                }

                content.Append("<U>地点</U>: " + task.TaskAddress + separator);

                if (!string.IsNullOrEmpty(task.AssignStartUserName))
                {
                    content.Append("<U>执行人</U>: " + task.AssignStartUserName + separator);
                }
                else
                {
                    //string principals = this.GetUserName(task.AssignStartUserName);
                    //content.Append("<U>执行人</U>: " + principals + separator);
                }

                if (task.PlanStartDate.HasValue)
                {
                    content.Append("<U>预计开始时间</U>: " + task.PlanStartDate.Value.ToString("yyyy-MM-dd HH:mm") + separator);
                }
                if (task.PlanCompleteDate.HasValue)
                {
                    content.Append("<U>预计完成时间</U>: " + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd HH:mm") + separator);
                }

                content.Append(separator + separator);

                if (task.UserName != null && task.UserName.Trim() != string.Empty)
                    content.Append(task.UserName + separator);
                if (task.MobilePhone != null && task.MobilePhone.Trim() != string.Empty && ISIHelper.IsValidMobilePhone(task.MobilePhone))
                    content.Append("Tel: " + task.MobilePhone + separator);
                if (task.Email != null && task.Email.Trim() != string.Empty && ISIHelper.IsValidEmail(task.Email))
                    content.Append("Email: " + task.Email + separator);

                var webAddress = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.WebAddress);

                content.Append(separator);
                content.Append(companyName + separator);
                content.Append("<a href='http://" + webAddress + "'>http://" + webAddress + "</a>");
                content.Append(separator);
                content.Append("</p>");
            }
            catch (Exception e)
            {
                //  log.Error(e.Message, e);
            }
            return content.ToString();
        }

        private void PutCommentText(TaskMaster task, StringBuilder content, string separator)
        {
            if (task.CommentDetail != null)
            {
                content.Append(separator);
                content.Append("<U>评论</U>：");

                if (!string.IsNullOrEmpty(task.CommentDetail.Comment))
                {
                    content.Append(task.CommentDetail.Comment.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                }
                content.Append(separator);
                content.Append(separator);
                content.Append(task.CommentDetail.CreateUserNm);
                content.Append(separator);
                content.Append(task.CommentDetail.CreateDate.ToString("yyyy-MM-dd HH:mm"));
                content.Append(separator);
                content.Append(separator);
                content.Append(separator);
            }
        }

        private void PutStatusText(TaskMaster task, StringBuilder content, string separator)
        {
            if (task.TaskStatus != null)
            {
                content.Append(separator);
                content.Append("<U>进展</U>：");

                if (!string.IsNullOrEmpty(task.TaskStatus.Description))
                {
                    content.Append(task.TaskStatus.Description.Replace(ISIConstants.TEXT_SEPRATOR, separator).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
                    content.Append(separator);
                    content.Append("标志: <span style='background-color:" + task.TaskStatus.Color + "'>" + task.TaskStatus.Flag + "</span>");
                    content.Append(separator);
                    content.Append("开始时间: " + task.TaskStatus.StartDate.ToString("yyyy-MM-dd"));
                    content.Append(separator);
                    content.Append("结束时间: " + task.TaskStatus.EndDate.ToString("yyyy-MM-dd"));
                }
                content.Append(separator);
                content.Append(separator);
                content.Append(task.TaskStatus.LastModifyUserName);
                content.Append(separator);
                content.Append(task.TaskStatus.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));
                content.Append(separator);
                content.Append(separator);
                content.Append(separator);
            }
        }


        [Transaction(TransactionMode.Unspecified)]
        protected string GetSMSBody(TaskMaster task, string level, double minutes, User operationUser)
        {
            StringBuilder content = new StringBuilder();
            string separator = ISIConstants.SMS_SEPRATOR;
            try
            {
                content.Append("ISI  ");
                var companyName = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.CompanyName);
                ISIHelper.AppendTestText(companyName, content, separator);
                if (!string.IsNullOrEmpty(task.HelpContent))
                {
                    content.Append("请协助求助处理");
                    content.Append(separator);
                }

                #region 拼邮件，后续再加
                //content.Append(this.GetDesc(task.Type, operationUser.Code));
                //content.Append(task.Code);
                //if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
                //{
                //    content.Append("[" + codeMasterMgrE.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_PRIORITY, task.Priority).Description + "]");
                //}

                //if (!string.IsNullOrEmpty(task.Subject))
                //{
                //    content.Append(separator);
                //    content.Append("标题:" + task.Subject);
                //}

                //content.Append(separator);
                //content.Append("类型: " + (task.TaskSubType != null ? task.TaskSubType.Description : task.TaskSubTypeDesc) + "|" + this.codeMasterMgrE.LoadCodeMaster(ISIConstants.CODE_MASTER_ISI_STATUS, task.Status).Description);

                //DateTime date = DateTime.Now;

                //if (level == ISIConstants.ISI_LEVEL_BASE)//提醒
                //{

                //    content.Append("|提醒");
                //}
                //else if (level == ISIConstants.ISI_LEVEL_HELP)
                //{

                //    content.Append("|求助");
                //}
                //else if (level == ISIConstants.ISI_LEVEL_COMMENT)
                //{

                //    content.Append("|评论");
                //}
                //else if (level == ISIConstants.ISI_LEVEL_STARTPERCENT)
                //{

                //    content.Append("|执行进度");
                //}
                //else if (level == ISIConstants.ISI_LEVEL_OPEN)
                //{

                //    content.Append("|开始");
                //}
                //else if (level == ISIConstants.ISI_LEVEL_COMPLETE)
                //{

                //    content.Append("|逾期完成");
                //}
                //else//上报
                //{
                //    content.Append("|上报");
                //    //content.Append(level + "级");

                //    //分派提醒
                //    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT && task.SubmitDate.HasValue)
                //    {
                //        string diff = ISIHelper.GetDiff(task.SubmitDate.Value.AddMinutes(minutes));
                //        if (!string.IsNullOrEmpty(diff))
                //        {
                //            content.Append(separator);
                //            content.Append("分派超时:" + diff);
                //        }
                //        date = task.SubmitDate.Value;
                //    }
                //    //开始提醒
                //    if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN && task.AssignDate.HasValue))
                //    {
                //        string diff = ISIHelper.GetDiff(task.AssignDate.Value.AddMinutes(minutes));
                //        if (!string.IsNullOrEmpty(diff))
                //        {
                //            content.Append(separator);
                //            content.Append("确认超时:" + diff);
                //        }
                //        date = task.AssignDate.Value;
                //    }
                //    //关闭提醒
                //    else if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS
                //                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE) && task.StartDate.HasValue)
                //    {
                //        string diff = ISIHelper.GetDiff(task.StartDate.Value.AddMinutes(minutes));
                //        if (!string.IsNullOrEmpty(diff))
                //        {
                //            content.Append(separator);
                //            content.Append("关闭超时:" + diff);
                //        }
                //        if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
                //        {
                //            date = task.StartDate.Value;
                //        }
                //        else if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                //        {
                //            date = task.CompleteDate.Value;
                //        }
                //    }
                //}
                //if (!string.IsNullOrEmpty(task.BackYards))
                //{
                //    content.Append(separator);
                //    content.Append("追溯码:" + task.BackYards);
                //}
                //content.Append(separator);
                //content.Append("时间:" + date.ToString("yyyy-MM-dd HH:mm") + separator);
                ////content.Append("类型:" + task.TaskSubType.Description + separator);
                //if (task.FailureMode != null || !string.IsNullOrEmpty(task.FailureModeCode))
                //{
                //    content.Append("失效模式:" + (task.FailureMode != null ? task.FailureMode.Code : task.FailureModeCode) + separator);
                //}
                //content.Append("地点:" + task.TaskAddress + separator);
                //if (task.PlanStartDate.HasValue)
                //{
                //    content.Append("预计开始时间:" + task.PlanStartDate.Value.ToString("yyyy-MM-dd HH:mm") + separator);
                //}
                //if (task.PlanCompleteDate.HasValue)
                //{
                //    content.Append("预计完成时间:" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd HH:mm") + separator);
                //}
                //if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                //{
                //    content.Append("已经完成!");
                //    return content.ToString();
                //}
                //if (level == ISIConstants.ISI_LEVEL_COMMENT)
                //{
                //    if (task.CommentDetail != null && !string.IsNullOrEmpty(task.CommentDetail.Comment))
                //    {
                //        content.Append(ISIHelper.GetStrLength(task.CommentDetail.Comment, 20));
                //        content.Append(separator);
                //        content.Append(task.CommentDetail.CreateUserNm);
                //        content.Append(separator);
                //        content.Append(task.CommentDetail.CreateDate.ToString("yyyy-MM-dd HH:mm"));

                //    }
                //}
                //else
                //{
                //    if (!string.IsNullOrEmpty(task.Description1))
                //    {
                //        content.Append(ISIHelper.GetStrLength(task.Description1, 20));
                //        content.Append(separator);
                //    }
                //    if (!string.IsNullOrEmpty(task.UserName)
                //        || (!string.IsNullOrEmpty(task.MobilePhone) && ISIHelper.IsValidMobilePhone(task.MobilePhone)))
                //    {
                //        content.Append("[");

                //        if (!string.IsNullOrEmpty(task.UserName))
                //        {
                //            content.Append(task.UserName);
                //        }
                //        if (!string.IsNullOrEmpty(task.MobilePhone) && ISIHelper.IsValidMobilePhone(task.MobilePhone))
                //        {
                //            if (!string.IsNullOrEmpty(task.UserName))
                //            {
                //                content.Append(", ");
                //            }
                //            content.Append(task.MobilePhone);
                //        }
                //        content.Append("]");
                //    }

                //    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                //    {
                //        content.Append(separator);
                //        content.Append("关闭回复 " + ISIHelper.GetSerialNo(task.Code) + "+空格+Y");
                //    }
                //}
                #endregion

            }
            catch (Exception e)
            {
                // log.Error(e.Message, e);
            }
            return content.ToString();
        }
        private string GetSubject(string userCode, string userName, string code, CodeMaster.TaskType type, CodeMaster.TaskPriority priority, string value, string level, CodeMaster.TaskStatus status)
        {
            StringBuilder subject = new StringBuilder();
            subject.Append(userName + " ");

            #region 拼内容，后面再改
            //if (priority == CodeMaster.TaskPriority.Urgent)
            //{
            //    subject.Append(codeMasterMgrE.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_PRIORITY, priority).Description + " ");
            //}

            //if (!string.IsNullOrEmpty(level))
            //{
            //    if (level == ISIConstants.ISI_LEVEL_HELP)
            //    {
            //        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Help", userCode));
            //    }
            //    else if (level == ISIConstants.ISI_LEVEL_BASE)
            //    {
            //        if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
            //        {
            //            subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Assign", userCode));
            //        }
            //        else
            //        {
            //            subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Subscription", userCode));
            //        }
            //    }
            //    else if (level == ISIConstants.ISI_LEVEL_COMMENT)
            //    {
            //        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Comment", userCode));
            //    }
            //    else if (level == ISIConstants.ISI_LEVEL_STATUS)
            //    {
            //        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.TaskStatus", userCode));
            //    }
            //    else if (level == ISIConstants.ISI_LEVEL_STARTPERCENT)
            //    {
            //        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Schedule", userCode));
            //    }
            //    else if (level == ISIConstants.ISI_LEVEL_OPEN)
            //    {
            //        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Open", userCode));
            //    }
            //    else if (level == ISIConstants.ISI_LEVEL_COMPLETE)
            //    {
            //        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.OverDue", userCode));
            //    }
            //    else
            //    {
            //        subject.Append(languageMgrE.TranslateMessage("ISI.Remind.Up", userCode, new string[] { level }));
            //    }


            // }
            #endregion

            subject.Append(" " + this.GetDesc(type, userCode) + ": ");
            if (string.IsNullOrEmpty(value))
            {
                subject.Append(code);
            }
            else
            {
                subject.Append(value);
            }
            return subject.ToString();
        }

        private static DetachedCriteria[] GetTaskAdminPermissionCriteria()
        {
            DetachedCriteria[] criteria = new DetachedCriteria[2];

            DetachedCriteria upSubCriteria = DetachedCriteria.For<UserPermission>();
            upSubCriteria.CreateAlias("User", "u");
            upSubCriteria.CreateAlias("Permission", "pm");
            upSubCriteria.CreateAlias("pm.Category", "pmc"); ;
            upSubCriteria.Add(Expression.Eq("pmc.Code", ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKADMIN));
            upSubCriteria.Add(Expression.In("pm.Code", new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN }));
            upSubCriteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.GroupProperty("u.Code"))));

            DetachedCriteria rpSubCriteria = DetachedCriteria.For<RolePermission>();
            rpSubCriteria.CreateAlias("Role", "r");
            rpSubCriteria.CreateAlias("Permission", "pm");
            rpSubCriteria.CreateAlias("pm.Category", "pmc"); ;
            rpSubCriteria.Add(Expression.Eq("pmc.Code", ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKADMIN));
            rpSubCriteria.Add(Expression.In("pm.Code", new string[] { ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN, ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN }));
            rpSubCriteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.GroupProperty("r.Code"))));

            DetachedCriteria urSubCriteria = DetachedCriteria.For<UserRole>();
            urSubCriteria.CreateAlias("User", "u");
            urSubCriteria.CreateAlias("Role", "r");
            urSubCriteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.GroupProperty("u.Code"))));
            urSubCriteria.Add(Subqueries.PropertyIn("r.Code", rpSubCriteria));

            criteria[0] = upSubCriteria;
            criteria[1] = urSubCriteria;

            return criteria;
        }


        private void HandleAssign(TaskMaster task, DateTime now, User user)
        {
            task.SubmitDate = now;
            task.SubmitUserId = user.Id;
            task.SubmitUserName = user.Name.Trim();

            task.Status = CodeMaster.TaskStatus.Submit;

            if (string.IsNullOrEmpty(task.AssignStartUser))
            {
                #region 排班
                //IList<SchedulingView> schedulingViewList = schedulingMgrE.GetScheduling2(now.Date, now.Date, task.TaskSubType.Code, string.Empty);
                //if (schedulingViewList != null && schedulingViewList.Count > 0)
                //{
                //    SchedulingView schedulingView = schedulingViewList.Where(s => s.StartTime <= now && now <= s.EndTime).FirstOrDefault();
                //    if (schedulingView != null)
                //    {
                //        if (schedulingView.Id.HasValue)
                //        {
                //            //排班表有执行人
                //            task.Scheduling = schedulingView.Id;
                //            task.SchedulingStartUser = schedulingView.StartUser;
                //            task.SchedulingShift = schedulingView.ShiftCode;
                //            task.SchedulingShiftTime = schedulingView.StartTime.ToString("yyyy-MM-dd HH:mm") + " " + schedulingView.EndTime.ToString("yyyy-MM-dd HH:mm");
                //        }
                //        else
                //        {
                //            task.Scheduling = null;

                //            task.AssignStartUser = schedulingView.StartUser;
                //            if (string.IsNullOrEmpty(task.AssignStartUser) && schedulingView.IsAutoAssign)
                //            {
                //                task.AssignStartUser = ISIConstants.ISI_LEVEL_SEPRATOR + user.Code + ISIConstants.ISI_LEVEL_SEPRATOR;
                //            }
                //            task.SchedulingShift = schedulingView.ShiftCode;
                //            task.SchedulingShiftTime = schedulingView.StartTime.ToString("yyyy-MM-dd HH:mm") + " " + schedulingView.EndTime.ToString("yyyy-MM-dd HH:mm");
                //        }
                //    }
                //    else
                //    {
                //        ClearScheduling(task);
                //    }
                //}
                //else
                //{
                //    ClearScheduling(task);
                //}

                #endregion

                if (!string.IsNullOrEmpty(task.SchedulingStartUser) || !string.IsNullOrEmpty(task.AssignStartUser))
                {
                    //userCodes = !string.IsNullOrEmpty(task.SchedulingStartUser) ? task.SchedulingStartUser : task.AssignStartUser;
                    //this.wfDetailMgrE.CreateWFDetail(task.Code, task.Status, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN, now, user);
                    task.Status = CodeMaster.TaskStatus.Assign;
                    task.AssignDate = now;
                    task.AssignUserId = user.Id;
                    task.AssignUserName = user.Name.Trim();

                    if (!task.IsAutoAssign)
                    {
                        task.PlanStartDate = now;
                        task.PlanCompleteDate = this.GetPlanCompleteDate(task.TaskSubType, task.PlanStartDate.Value);
                    }
                }
            }
        }

        private DateTime GetPlanCompleteDate(string taskSubTypeCode, DateTime planStartDate)
        {
            TaskSubType taskSubType = genericMgr.FindById<TaskSubType>(taskSubTypeCode);
            DateTime planEndDate = planStartDate;
            if (taskSubType.AssignUpTime.HasValue)
            {
                planEndDate = planEndDate.AddMinutes(double.Parse(taskSubType.AssignUpTime.Value.ToString()));
            }
            else
            {
                //EntityPreference entityPreference = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_ASSIGN_UP_TIME);
                //if (entityPreference != null && !string.IsNullOrEmpty(entityPreference.Value))
                //{
                //    planEndDate = planEndDate.AddMinutes(double.Parse(entityPreference.Value));
                //}
            }

            if (taskSubType.StartUpTime.HasValue)
            {
                planEndDate = planEndDate.AddMinutes(double.Parse(taskSubType.StartUpTime.Value.ToString()));
            }
            else
            {
                var startUpTime = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.StartUpTime);
                planEndDate = planEndDate.AddMinutes(double.Parse(startUpTime));
            }
            if (taskSubType.CloseUpTime.HasValue)
            {
                planEndDate = planEndDate.AddMinutes(double.Parse(taskSubType.CloseUpTime.Value.ToString()));
            }
            else
            {
                var closeUpTime = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.StartUpTime);
                planEndDate = planEndDate.AddMinutes(double.Parse(closeUpTime));
            }
            return planEndDate;
        }
        #endregion
    }
}
