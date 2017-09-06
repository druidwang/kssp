using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class PickTransaction : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        [Export(ExportName = "PickTrans", ExportSeq = 20)]
        [Display(Name = "Hu_Item", ResourceType = typeof(Resources.INV.Hu))]
        public string Item { get; set; }
        [Export(ExportName = "PickTrans", ExportSeq = 50)]
        [Display(Name = "Hu_manufacture_date", ResourceType = typeof(Resources.INV.Hu))]
        public string LotNo { get; set; }
        [Export(ExportName = "PickTrans", ExportSeq = 60)]
        [Display(Name = "LocationLotDetail_Bin", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string Bin { get; set; }
        [Export(ExportName = "PickTrans", ExportSeq = 10)]
        [Display(Name = "Hu_HuId", ResourceType = typeof(Resources.INV.Hu))]
        public String HuId { get; set; }
        [Export(ExportName = "PickTrans", ExportSeq = 40)]
        [Display(Name = "Hu_LocationTo", ResourceType = typeof(Resources.INV.Hu))]
        public String Location { get; set; }
        /// <summary>
        /// 操作后的状态
        /// </summary>
        public Boolean IsPick { get; set; }
        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "PickTrans", ExportSeq = 80)]
        [Display(Name = "Hu_CreateUserName", ResourceType = typeof(Resources.INV.Hu))]
        public string CreateUserName { get; set; }
        [Export(ExportName = "PickTrans", ExportSeq = 90)]
        [Display(Name = "Hu_CreateDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime CreateDate { get; set; }
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
            PickTransaction another = obj as PickTransaction;

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
