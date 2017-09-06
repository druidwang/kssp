using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class ShipmentDetail : EntityBase
    {
        #region O/R Mapping Properties

        public string ShipmentNo { get; set; }

        public string IpNo { get; set; }

        public Int32 Id { get; set; }
     
       
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
