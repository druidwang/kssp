using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class StockTakeInv : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public String StNo { get; set; }
        public String Location { get; set; }
        public String Item { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public Decimal Qty { get; set; }
        public String LotNo { get; set; }
        public String Bin { get; set; }
        public String HuId { get; set; }

        public bool IsCS { get; set; }
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
            StockTakeInv another = obj as StockTakeInv;

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
