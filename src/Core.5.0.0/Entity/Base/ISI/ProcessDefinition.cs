using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class ProcessDefinition : EntityBase
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
		private string _taskSubType;
		public string TaskSubType
		{
			get
			{
				return _taskSubType;
			}
			set
			{
				_taskSubType = value;
			}
		}
		private Int32 _seq;
		public Int32 Seq
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
		private string _desc1;
		public string Desc1
		{
			get
			{
				return _desc1;
			}
			set
			{
				_desc1 = value;
			}
		}
		private string _userList;
		public string UserList
		{
			get
			{
				return _userList;
			}
			set
			{
				_userList = value;
			}
		}
		private Boolean _isParallel;
		public Boolean IsParallel
		{
			get
			{
				return _isParallel;
			}
			set
			{
				_isParallel = value;
			}
		}
		private Boolean _isOpt;
		public Boolean IsOpt
		{
			get
			{
				return _isOpt;
			}
			set
			{
				_isOpt = value;
			}
		}
		private Boolean _isApprove;
		public Boolean IsApprove
		{
			get
			{
				return _isApprove;
			}
			set
			{
				_isApprove = value;
			}
		}
		private Boolean _isCtrl;
		public Boolean IsCtrl
		{
			get
			{
				return _isCtrl;
			}
			set
			{
				_isCtrl = value;
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
		private Int32 _version;
		public Int32 Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
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
            ProcessDefinition another = obj as ProcessDefinition;

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
