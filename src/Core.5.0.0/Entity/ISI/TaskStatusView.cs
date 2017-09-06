using System;
using System.Linq;
//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class TaskStatusView 
    {
        #region Non O/R Mapping Properties

        public int StartedUserCount
        {
            get
            {
                if (!string.IsNullOrEmpty(this.StartedUser))
                {
                    return this.StartedUser.Split(',').Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string GetAssignStartUserNm(int topNo, string userCode)
        {
            if (!string.IsNullOrEmpty(this.AssignStartUserNm))
            {
                var nm = AssignStartUserNm.Split(new string[] { ISIConstants.ISI_USER_SEPRATOR_VIEW }, StringSplitOptions.RemoveEmptyEntries);

                if (nm.Length <= topNo)
                {
                    return AssignStartUserNm;
                }
                else
                {
                    var nm1 = new string[topNo + 2];
                    Array.Copy(nm, nm1, topNo);
                    nm1[topNo] = "бнбн";
                    var userCodeArr = this.StartedUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).ToList();
                    int index = userCodeArr.IndexOf(userCode);
                    if (index != -1 && index >= topNo)
                    {
                        nm1[topNo + 1] = nm[index];
                        return string.Join(ISIConstants.ISI_USER_SEPRATOR_VIEW, nm1);
                    }
                    return string.Join(ISIConstants.ISI_USER_SEPRATOR_VIEW, nm1.Take(topNo + 1).ToArray());
                }
            }
            return string.Empty;
        }

        public string[] CloseUpLevel
        {
            get
            {
                if (string.IsNullOrEmpty(this.CloseUpUser))
                {
                    return new string[0];
                }
                else
                {
                    return this.CloseUpUser.Substring(1, this.CloseUpUser.Length - 2).Split('|');
                }
            }
        }

        public string TaskSubType
        {
            get
            {
                return this.TaskSubTypeCode + "[" + this.TaskSubTypeDesc + "]";
            }
        }

        public Int32 CommentCount1
        {
            get
            {
                return CommentCount.HasValue ? CommentCount.Value : 0;
            }
        }

        public Int32 CurrentCommentCount1
        {
            get
            {
                return CurrentCommentCount.HasValue ? CurrentCommentCount.Value : 0;
            }
        }

        public Int32 StatusCount1
        {
            get
            {
                return StatusCount.HasValue ? StatusCount.Value : 0;
            }
        }

        public Int32 CurrentStatusCount1
        {
            get
            {
                return CurrentStatusCount.HasValue ? CurrentStatusCount.Value : 0;
            }
        }
        
        #endregion
    }
}