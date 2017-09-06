using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity;

using System.Text.RegularExpressions;


namespace com.Sconit.Utility
{
    public static class ISIHelper
    {
        /// <summary>
        /// 是否是手机号码
        /// </summary>
        /// <param name="val"></param>
        public static bool IsValidMobilePhone(string val)
        {
            return Regex.IsMatch(val, @"^1[358]\d{9}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }

        public static bool Contains(string value1, string value2)
        {
            if (string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2)) return false;

            string[] users = value2.Split(ISIConstants.ISI_SEPRATOR);
            foreach (var user in users)
            {
                if (value1.Contains(ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR)
                    || value1.Contains(ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR)
                    || value1.Contains(ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR)
                    || value1.Contains(ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR)
                    || value1 == user
                    || value1.StartsWith(user + ISIConstants.ISI_USER_SEPRATOR)
                    || value1.EndsWith(ISIConstants.ISI_USER_SEPRATOR + user))
                {
                    return true;
                }
            }
            return false;
        }

        public static string ShowUser(string users)
        {
            if (!string.IsNullOrEmpty(users))
            {
                if (users.StartsWith(ISIConstants.ISI_LEVEL_SEPRATOR)
                         || users.EndsWith(ISIConstants.ISI_LEVEL_SEPRATOR))
                {
                    string u = users.Substring(1, users.Length - 2);
                    u = u.Replace(",", ", ");
                    return u;
                }
                else
                {
                    return users;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string EditUser(string users)
        {
            if (!string.IsNullOrEmpty(users))
            {
                if (users.StartsWith(ISIConstants.ISI_LEVEL_SEPRATOR)
                         && users.EndsWith(ISIConstants.ISI_LEVEL_SEPRATOR))
                {
                    string u = users.Substring(1, users.Length - 2);
                    //u = u.Replace(",", ", ");
                    return u;
                }
                else
                {
                    return users;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetUser(string users)
        {
            if (!string.IsNullOrEmpty(users))
            {
                string u = users.Replace(" ", "");
                return ISIConstants.ISI_LEVEL_SEPRATOR + u + ISIConstants.ISI_LEVEL_SEPRATOR;
            }
            return string.Empty;
        }

        public static string GetUserName(string assignStartUserNm, string userName, string color)
        {
            if (string.IsNullOrEmpty(assignStartUserNm)) return string.Empty;
            else
            {
                StringBuilder html = new StringBuilder();
                var userNames = assignStartUserNm.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var u in userNames)
                {
                    html.Append("<div>");
                    if (u == userName)
                    {
                        html.Append("<span style='color:" + color + ";'>");
                    }
                    html.Append(u);
                    if (u == userName)
                    {
                        html.Append("</span>");
                    }
                    html.Append("</div>");
                }
                return html.ToString();
            }
        }

        public static string GetUserMerge(string userCodes, string userNames)
        {
            if (userCodes.Length == 0 || userNames.Length == 0) return string.Empty;
            string[] userCodeArray = userCodes.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
            string[] userNameArray = userNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (userCodeArray.Length == 0 || userNameArray.Length == 0 || userCodeArray.Length != userNameArray.Length) return string.Empty;

            StringBuilder userCodeName = new StringBuilder();
            for (int i = 0; i < userNameArray.Length; i++)
            {
                if (userCodeName.Length != 0)
                {
                    userCodeName.Append(",");
                }
                userCodeName.Append(userCodeArray[i].Trim() + "[" + userNameArray[i].Trim() + "]");
            }
            return userCodeName.ToString();
        }

        public static string[] GetUserSplit(string users)
        {
            string[] userArr = users.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

            if (userArr == null || userArr.Length == 0)
            {
                return new string[0];
            }
            StringBuilder userCodes = new StringBuilder();
            StringBuilder userNames = new StringBuilder();
            foreach (var u in userArr)
            {
                var t = u.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                if (t.Length >= 1)
                {
                    if (userCodes.Length > 0)
                    {
                        userCodes.Append(",");
                    }
                    userCodes.Append(t[0]);
                }
                else
                {
                    continue;
                }

                if (t.Length >= 2)
                {
                    if (userNames.Length > 0)
                    {
                        userNames.Append(", ");
                    }
                    userNames.Append(t[1]);
                }
            }
            return new string[] { userCodes.ToString(), userNames.ToString() };
        }

        public static string GetDiff(DateTime dateTime)
        {
            return GetDiff(DateTime.Now, dateTime);
        }

        public static string GetDiff(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts1 = new TimeSpan(dt1.Ticks);
            TimeSpan ts2 = new TimeSpan(dt2.Ticks);
            TimeSpan diff = ts1.Subtract(ts2).Duration();
            if (diff.TotalMilliseconds > 0)
            {
                return GetDiff(diff);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetDiff(TimeSpan diff)
        {
            return GetDiff(diff, false, false);
        }

        public static string GetDiff(TimeSpan diff, bool isSeconds, bool isMilliseconds)
        {
            StringBuilder msg = new StringBuilder();
            if (diff.Days != 0)
            {
                msg.Append(diff.Days + "天");
            }
            if (diff.Hours != 0)
            {
                msg.Append(diff.Hours + "小时");
            }
            if (diff.Minutes != 0)
            {
                msg.Append(diff.Minutes + "分");
            }
            if (isSeconds)
            {
                if (diff.Seconds != 0)
                {
                    msg.Append(diff.Seconds + "秒");
                }
            }
            if (isMilliseconds)
            {
                if (diff.Milliseconds != 0)
                {
                    msg.Append(diff.Milliseconds + "毫秒");
                }
            }
            return msg.ToString();
        }

        public static string GetSerialNo(string taskCode)
        {
            return int.Parse(taskCode.Substring(3)).ToString();
        }

        public static string GetTaskCode(string serialNo)
        {
            return serialNo.PadLeft(9, '0');
        }

        #region 获取指定长度的中英文混合字符串
        /// <summary>
        /// 获取指定长度的中英文混合字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">要截取的长度</param>
        /// <returns></returns>
        public static string GetStrLength(string str, int len)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string result = string.Empty;// 最终返回的结果
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            len -= 4;
            if (byteLen > len)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                    {
                        byteCount += 2;
                    }
                    else// 按英文字符计算加1
                    {
                        byteCount += 1;
                    }
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }
                if (pos >= 0)
                {
                    result = str.Substring(0, pos) + "...";
                }
            }
            else
            {
                result = str;
            }

            return result;
        }
        #endregion


        /// <summary>
        /// 取得某月的第一天
        /// </summary>
        /// <param name="datetime">要取得月份第一天的时间</param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }

        /**/
        /// <summary>
        /// 取得某月的最后一天
        /// </summary>
        /// <param name="datetime">要取得月份最后一天的时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddMilliseconds(-1);
        }


        public static void AppendTestText(string companyName, StringBuilder content, string separator)
        {
            bool isTest = false;
            StringBuilder text = new StringBuilder();
            if (companyName.ToLower().Contains("test"))
            {
                isTest = true;
            }

            if (isTest)
            {
                if (separator == ISIConstants.SMS_SEPRATOR)
                {
                    text.Append("测试短信");
                    text.Append(separator);
                }
                else
                {
                    text.Append("<span style='font-size:13px;color:#0000E5;'>注&#58;&nbsp;此邮件从测试系统发出&#44;&nbsp;请忽略&#46;</span>");
                    text.Append(separator);
                    text.Append(separator);
                }
            }

            content.Insert(0, text);
        }

        //public static void GetReportDetailBody(StringBuilder mailDetailBody, TaskView task, DateTime endDate, IDictionary<string, UserSub> userDic, IDictionary<string, string> statusCodeMstrList)
        //{

        //    mailDetailBody.Append("<tr>");
        //    mailDetailBody.Append("<td><span");
        //    if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
        //    {
        //        mailDetailBody.Append(" style='color:red' ");
        //    }
        //    mailDetailBody.Append(">" + task.Code + "</span><br/>" + task.Subject + (task.Type == ISIConstants.ISI_TASK_TYPE_PROJECT && !string.IsNullOrEmpty(task.TaskType) ? "<br/>[" + task.TaskType + "]" : string.Empty) + "</td>");
        //    mailDetailBody.Append("<td>");

        //    if (!string.IsNullOrEmpty(task.Desc1))
        //    {
        //        mailDetailBody.Append(task.Desc1.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
        //    }
        //    if (!string.IsNullOrEmpty(task.Desc2))
        //    {
        //        mailDetailBody.Append((!string.IsNullOrEmpty(task.Desc1) ? "<br/>" : string.Empty) + "<span style='color:#0000E5;'>补充描述</span>&#58;&nbsp;");
        //        mailDetailBody.Append(task.Desc2.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));

        //        if (task.RefTaskCount.HasValue && task.RefTaskCount.Value > 0)
        //        {
        //            mailDetailBody.Append("<br/><span style='color:#0000E5;'>相关任务</span>&#58;&nbsp;" + task.RefTaskCount.Value);
        //        }
        //    }
        //    mailDetailBody.Append("</td>");

        //    mailDetailBody.Append("<td>");
        //    if (!string.IsNullOrEmpty(task.ExpectedResults))
        //    {
        //        mailDetailBody.Append(task.ExpectedResults.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
        //    }
        //    mailDetailBody.Append("</td>");
        //    mailDetailBody.Append("<td nowrap>" + task.SubmitUserNm);

        //    if (task.Type == ISIConstants.ISI_TASK_TYPE_PROJECT)
        //    {
        //        if (!string.IsNullOrEmpty(task.Phase))
        //        {
        //            mailDetailBody.Append("<br>" + task.Phase);
        //        }
        //        if (!string.IsNullOrEmpty(task.Seq))
        //        {
        //            mailDetailBody.Append("<br>" + task.Seq);
        //        }
        //    }
        //    mailDetailBody.Append("</td>");
        //    mailDetailBody.Append("<td nowrap>" + task.AssignUserNm + "<br>" + statusCodeMstrList[task.Status] + "</td>");
        //    mailDetailBody.Append("<td nowrap>");

        //    if (userDic != null && userDic.Count > 0 && !string.IsNullOrEmpty(task.StartedUser))
        //    {
        //        string[] userCodes = task.StartedUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
        //        if (userCodes != null && userCodes.Length > 0)
        //        {
        //            for (int i = 0; i < userCodes.Length; i++)
        //            {
        //                if (userDic.Keys.Contains(userCodes[i]))
        //                {
        //                    if (i != 0)
        //                    {
        //                        mailDetailBody.Append("<br>");
        //                    }
        //                    mailDetailBody.Append(userDic[userCodes[i]].Name);
        //                }
        //            }
        //        }
        //    }
        //    mailDetailBody.Append("</td>");
        //    mailDetailBody.Append("<td nowrap>" + (task.StatusDate.HasValue ? (task.StatusDate.Value).ToString("yyyy-MM-dd<br>HH:mm") : string.Empty) + "</td>");
        //    mailDetailBody.Append("<td>");
        //    if (!string.IsNullOrEmpty(task.CreateUserNm))
        //    {
        //        mailDetailBody.Append("<span style='color:#0000E5;'>" + task.CreateUserNm + "</span>&#58;&nbsp;" + task.StatusDesc + (task.StatusCount.HasValue && task.StatusCount.Value > 1 ? "<span style='color:#0000E5;'>&#40;" + task.StatusCount.Value + "&#41;</span>" : string.Empty));
        //    }
        //    mailDetailBody.Append("</td>");
        //    mailDetailBody.Append("<td nowrap style='background-color:" + task.Color + ";'>" + task.Flag + "</td>");
        //    mailDetailBody.Append("<td>");
        //    if (task.CommentCreateDate.HasValue && !string.IsNullOrEmpty(task.CommentCreateUserNm))
        //    {
        //        mailDetailBody.Append("<span style='color:#0000E5;'>" + task.CommentCreateUserNm + "&#40;" + task.CommentCreateDate.Value.ToString("yyyy-MM-dd HH:mm") + "&#41;</span>&#58;&nbsp;" + task.Comment.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + (task.CommentCount.HasValue && task.CommentCount.Value > 1 ? "<span style='color:#0000E5;'>&#40;" + task.CommentCount.Value + "&#41;</span>" : string.Empty));
        //    }
        //    mailDetailBody.Append("</td>");
        //    mailDetailBody.Append("<td nowrap >" + (task.AttachmentCount.HasValue && task.AttachmentCount.Value != 0 ? task.AttachmentCount.Value.ToString() : string.Empty) + "</td>");
        //    mailDetailBody.Append("<td nowrap >");
        //    if (task.PlanCompleteDate.HasValue)
        //    {
        //        if (task.PlanCompleteDate.Value < endDate)
        //        {
        //            mailDetailBody.Append("<span style='color:red'>" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd<br>HH:mm") + "</span>");
        //        }
        //        else
        //        {
        //            mailDetailBody.Append(task.PlanCompleteDate.Value.ToString("yyyy-MM-dd<br>HH:mm"));
        //        }
        //    }
        //    mailDetailBody.Append("</td>");
        //    mailDetailBody.Append("</tr>");
        //}

        public static void GetColumnHead(StringBuilder taskListBody)
        {
            taskListBody.Append(
                "<table cellspacing='0' cellpadding='4' rules='all' border='1' style='width:100%;border-collapse:collapse;font-size:12px;'>");
            taskListBody.Append("<tr nowrap style='color:#FFFFFF;background-color:#000060;font-weight:bold;line-height:150%;'>");
            taskListBody.Append("<th nowrap scope='col'>任务主题</th>");
            taskListBody.Append("<th nowrap scope='col'>描述</th>");
            taskListBody.Append("<th nowrap scope='col'>预期结果/达成结果</th>");
            taskListBody.Append("<th nowrap scope='col'>提交人</th>");
            taskListBody.Append("<th nowrap scope='col'>分派人</th>");
            taskListBody.Append("<th nowrap scope='col'>责任人</th>");
            taskListBody.Append("<th nowrap scope='col'>跟踪日期</th>");
            taskListBody.Append("<th nowrap scope='col'>进展</th>");
            taskListBody.Append("<th nowrap scope='col'>标志</th>");
            taskListBody.Append("<th nowrap scope='col'>最新评论</th>");
            taskListBody.Append("<th nowrap scope='col'>附件</th>");
            taskListBody.Append("<th nowrap scope='col'>预计完成时间</th>");
            taskListBody.Append("</tr>");
        }
        public static string GetHtmlBody(string targetContent)
        {
            return SetHighlight(targetContent, false, string.Empty);
        }
        public static string SetHighlight(string targetContent, bool isHighlight, string key)
        {
            if (!string.IsNullOrEmpty(targetContent))
            {
                //targetContent = targetContent.Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray()).Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>");
                targetContent = targetContent.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>");
            }
            if (isHighlight && !string.IsNullOrEmpty(key))
            {
                targetContent = targetContent.Replace(key, "<span style='color:blue;'><b>" + key + "</b></span>");
            }
            return targetContent;
        }

        public static string GetHide(string code, string targetContent)
        {
            if (!string.IsNullOrEmpty(code) && targetContent.Length > 360)
            {
                var con1 = targetContent.Substring(0, 300);
                var con2 = targetContent.Substring(300, targetContent.Length - 300);
                return con1 + "<span  id = 'showStatusDescHideDiv" + code + "' style='display:none;'>" + con2 + "</span><a onclick=\"showHide('" + "showStatusDescHideDiv" + code + "')\"'>Click</a>";
            }
            else
            {
                return targetContent;
            }
        }

        public static string FormatUser(string userCode, int seq)
        {
            StringBuilder str = new StringBuilder();
            if (seq == 1)
            {
                str.Append(ISIConstants.ISI_LEVEL_SEPRATOR);
                str.Append(userCode);
                str.Append(ISIConstants.ISI_USER_SEPRATOR);
            }
            else if (seq == 2)
            {
                str.Append(ISIConstants.ISI_USER_SEPRATOR);
                str.Append(userCode);
                str.Append(ISIConstants.ISI_USER_SEPRATOR);
            }
            else if (seq == 3)
            {
                str.Append(ISIConstants.ISI_USER_SEPRATOR);
                str.Append(userCode);
                str.Append(ISIConstants.ISI_LEVEL_SEPRATOR);
            }
            else if (seq == 4)
            {
                str.Append(ISIConstants.ISI_LEVEL_SEPRATOR);
                str.Append(userCode);
                str.Append(ISIConstants.ISI_LEVEL_SEPRATOR);
            }
            else
            {
                return userCode;
            }
            return str.ToString();
        }
     
        public static string GetModuleType(string taskCode)
        {
            return ISIConstants.TaskTypeDic[taskCode.Substring(0, 3)];
        }

    }

}
