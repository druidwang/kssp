using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class OrderOperation : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string OrderNo { get; set; }
        public Int32 OrderDetId { get; set; }
        public Int32 Op { get; set; }
        public string OpReference { get; set; }
        public string OpDesc { get; set; }
        public string Location { get; set; }
        public string WorkCenter { get; set; }
        public Boolean NeedReport { get; set; }
        public Boolean IsAutoReport { get; set; }
        public Decimal ReportQty { get; set; }
        public Decimal ScrapQty { get; set; }
        public Decimal BackflushQty { get; set; }
        public Boolean IsRecFG { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }

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
            OrderOperation another = obj as OrderOperation;

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
