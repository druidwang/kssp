using System;
using System.Xml.Serialization;
using com.Sconit.CodeMaster;

namespace com.Sconit.Entity.MSG
{
    [Serializable]
    public partial class MessageQueue : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string MethodName { get; set; }
        public string ParamValue { get; set; }
        public DateTime CreateTime { get; set; }
        public MQStatusEnum Status { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32 ErrorCount { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
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
            MessageQueue another = obj as MessageQueue;

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
