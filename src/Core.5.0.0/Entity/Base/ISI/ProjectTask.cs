using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class ProjectTask : EntityBase
    {
        #region O/R Mapping Properties
		
		private Int32 _id;
		public Int32 Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		private string _projectType;
		public string ProjectType
		{
			get
			{
				return _projectType;
			}
			set
			{
				_projectType = value;
			}
		}
		private string _projectSubType;
		public string ProjectSubType
		{
			get
			{
				return _projectSubType;
			}
			set
			{
				_projectSubType = value;
			}
		}
		private string _phase;
		public string Phase
		{
			get
			{
				return _phase;
			}
			set
			{
				_phase = value;
			}
		}
		private string _seq;
		public string Seq
		{
			get
			{
				return _seq;
			}
			set
			{
				_seq = value;
			}
		}
		private string _taskType;
		public string TaskType
		{
			get
			{
				return _taskType;
			}
			set
			{
				_taskType = value;
			}
		}
		private string _subject;
		public string Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}
		private string _desc;
		public string Desc
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
			}
		}
		private string _expectedResults;
		public string ExpectedResults
		{
			get
			{
				return _expectedResults;
			}
			set
			{
				_expectedResults = value;
			}
		}
		private string _createUser;
		public string CreateUser
		{
			get
			{
				return _createUser;
			}
			set
			{
				_createUser = value;
			}
		}
		private string _createUserNm;
		public string CreateUserNm
		{
			get
			{
				return _createUserNm;
			}
			set
			{
				_createUserNm = value;
			}
		}
		private DateTime _createDate;
		public DateTime CreateDate
		{
			get
			{
				return _createDate;
			}
			set
			{
				_createDate = value;
			}
		}
		private string _lastModifyUser;
		public string LastModifyUser
		{
			get
			{
				return _lastModifyUser;
			}
			set
			{
				_lastModifyUser = value;
			}
		}
		private string _lastModifyUserNm;
		public string LastModifyUserNm
		{
			get
			{
				return _lastModifyUserNm;
			}
			set
			{
				_lastModifyUserNm = value;
			}
		}
		private DateTime _lastModifyDate;
		public DateTime LastModifyDate
		{
			get
			{
				return _lastModifyDate;
			}
			set
			{
				_lastModifyDate = value;
			}
		}
		private Boolean _isActive;
		public Boolean IsActive
		{
			get
			{
				return _isActive;
			}
			set
			{
				_isActive = value;
			}
		}
        
        #endregion

		public override int GetHashCode()
        {
			if (Id != null)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ProjectTask another = obj as ProjectTask;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Id == another.Id);
            }
        } 
    }
	
}
