using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class ProductionTrace : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string OrderNo { get; set; }

        //public Int32 OrderDetailId { get; set; }
        public string RmHu { get; set; }
        public bool IsCancel { get; set; }

        public string Flow { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string Fg { get; set; }
        public string LotNo { get; set; }
        public string FgHu { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
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
            ShipmentDetail another = obj as ShipmentDetail;

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
