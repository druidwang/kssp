using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{
    [Serializable]
    public partial class ProcessApply : EntityBase
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
		private string _apply;
		public string Apply
		{
			get
			{
				return _apply;
			}
			set
			{
				_apply = value;
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
		private Decimal? _qty;
		public Decimal? Qty
		{
			get
			{
				return _qty;
			}
			set
			{
				_qty = value;
			}
		}
		private string _uOM;
		public string UOM
		{
			get
			{
				return _uOM;
			}
			set
			{
				_uOM = value;
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
            ProcessApply another = obj as ProcessApply;

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
