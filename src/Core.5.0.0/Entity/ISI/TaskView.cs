using System;
using System.Linq;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class TaskView
    {
        public string Code { get; set; }
        public string UserCode { get; set; }
        public string TaskSubTypeCode { get; set; }
        public string TaskSubTypeDesc { get; set; }
        public string TaskSubTypeAssignUser { get; set; }
        public string StartUpUser { get; set; }
        public string CloseUpUser { get; set; }
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

        public string AssignUpUser { get; set; }
        public bool IsAutoAssign { get; set; }

        public decimal? StartPercent { get; set; }

        public string TaskSubType
        {
            get
            {
                return this.TaskSubTypeCode + "[" + this.TaskSubTypeDesc + "]";
            }
        }

        public string FileName { get; set; }
        public string Path { get; set; }
        public string FileExtension { get; set; }
        public string ContentType { get; set; }
        public DateTime? AttachmentCreateDate { get; set; }
        public string AttachmentCreateUserNm { get; set; }
        public Int32? AttachmentCount { get; set; }
        public Int32? StatusCount { get; set; }
        public Int32? CommentCount { get; set; }
        public Int32? RefTaskCount { get; set; }

        public Int32? CurrentAttachmentCount { get; set; }
        public Int32? CurrentStatusCount { get; set; }
        public Int32? CurrentCommentCount { get; set; }

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

        public string TaskAddress { get; set; }
        public string Phase { get; set; }
        public string Seq { get; set; }
        public string Subject { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Status { get; set; }
        public string Flag { get; set; }
        public string Color { get; set; }
        public string Priority { get; set; }
        public string Type { get; set; }
        public string TaskType { get; set; }
        public string BackYards { get; set; }
        public string StartUser { get; set; }
        public string StartUserNm { get; set; }
        public string AssignUser { get; set; }
        public DateTime? AssignDate { get; set; }
        public string AssignUserNm { get; set; }
        public string SubmitUser { get; set; }
        public string SubmitUserNm { get; set; }
        public string ExpectedResults { get; set; }
        public string Comment { get; set; }
        public string CommentCreateUserNm { get; set; }
        public DateTime? CommentCreateDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public string CreateUserNm { get; set; }
        public string TaskCreateUserNm { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? PlanCompleteDate { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskCompleteDate { get; set; }

        public string StatusDesc { get; set; }
        
        public string AssignStartUserNm { get; set; }

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
                    if (index != -1 && index > topNo)
                    {
                        nm1[topNo + 1] = nm[index];
                        return string.Join(ISIConstants.ISI_USER_SEPRATOR_VIEW, nm1);
                    }
                    return string.Join(ISIConstants.ISI_USER_SEPRATOR_VIEW, nm1.Take(topNo + 1).ToArray());
                }
            }
            return string.Empty;
        }

        public string StartedUser { get; set; }

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

    }
}