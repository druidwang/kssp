using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class FreezeTransaction : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 20)]
        [Display(Name = "Hu_Item", ResourceType = typeof(Resources.INV.Hu))]
        public string Item { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 50)]
        [Display(Name = "Hu_manufacture_date", ResourceType = typeof(Resources.INV.Hu))]
        public string LotNo { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 60)]
        [Display(Name = "LocationLotDetail_Bin", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string Bin { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 10)]
        [Display(Name = "Hu_HuId", ResourceType = typeof(Resources.INV.Hu))]
        public String HuId { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 40)]
        [Display(Name = "Hu_LocationTo", ResourceType = typeof(Resources.INV.Hu))]
        public String Location { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 75)]
        [Display(Name = "Hu_Reason", ResourceType = typeof(Resources.INV.Hu))]
        public String Reason { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 80, ExportTitleResourceType = typeof(Resources.INV.LocationLotDetail), ExportTitle = "LocationLotDetail_ItemVersion")]
        [Display(Name = "LocationLotDetail_ItemVersion", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public String ItemVersion { get; set; }
        /// <summary>
        /// 操作后的状态
        /// </summary>
        public Boolean Freeze { get; set; }
        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 80)]
        [Display(Name = "Hu_CreateUserName", ResourceType = typeof(Resources.INV.Hu))]
        public string CreateUserName { get; set; }
        [Export(ExportName = "FreezeTrans", ExportSeq = 90)]
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
            FreezeTransaction another = obj as FreezeTransaction;

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
