using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class EmppDetail : EntityBase
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
		private string _taskCode;
		public string TaskCode
		{
			get
			{
				return _taskCode;
			}
			set
			{
				_taskCode = value;
			}
		}
		private Int32? _taskDetail;
		public Int32? TaskDetail
		{
			get
			{
				return _taskDetail;
			}
			set
			{
				_taskDetail = value;
			}
		}
		private string _level;
		public string Level
		{
			get
			{
				return _level;
			}
			set
			{
				_level = value;
			}
		}
		private string _msgID;
		public string MsgID
		{
			get
			{
				return _msgID;
			}
			set
			{
				_msgID = value;
			}
		}
		private Int32? _seqID;
		public Int32? SeqID
		{
			get
			{
				return _seqID;
			}
			set
			{
				_seqID = value;
			}
		}
		private string _srcID;
		public string SrcID
		{
			get
			{
				return _srcID;
			}
			set
			{
				_srcID = value;
			}
		}
		private string _destID;
		public string DestID
		{
			get
			{
				return _destID;
			}
			set
			{
				_destID = value;
			}
		}
		private string _serviceID;
		public string ServiceID
		{
			get
			{
				return _serviceID;
			}
			set
			{
				_serviceID = value;
			}
		}
		private string _srcTerminalId;
		public string SrcTerminalId
		{
			get
			{
				return _srcTerminalId;
			}
			set
			{
				_srcTerminalId = value;
			}
		}
		private Int32? _srcTerminalType;
		public Int32? SrcTerminalType
		{
			get
			{
				return _srcTerminalType;
			}
			set
			{
				_srcTerminalType = value;
			}
		}
		private Int32? _msgFmt;
		public Int32? MsgFmt
		{
			get
			{
				return _msgFmt;
			}
			set
			{
				_msgFmt = value;
			}
		}
		private Int32? _msgLength;
		public Int32? MsgLength
		{
			get
			{
				return _msgLength;
			}
			set
			{
				_msgLength = value;
			}
		}
		private string _status;
		public string Status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
			}
		}
		private string _content;
		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}
		private DateTime? _doneDatetime;
		public DateTime? DoneDatetime
		{
			get
			{
				return _doneDatetime;
			}
			set
			{
				_doneDatetime = value;
			}
		}
		private DateTime? _submitDatetime;
		public DateTime? SubmitDatetime
		{
			get
			{
				return _submitDatetime;
			}
			set
			{
				_submitDatetime = value;
			}
		}
		private string _eventHandler;
		public string EventHandler
		{
			get
			{
				return _eventHandler;
			}
			set
			{
				_eventHandler = value;
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
            EmppDetail another = obj as EmppDetail;

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
