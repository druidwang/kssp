using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPPPMES0004 : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 10)]
        [Display(Name = "SAPPPMES_ZMESSC", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZMESSC { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 20)]
        [Display(Name = "SAPPPMES_ZMESLN", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZMESLN { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 30)]
        [Display(Name = "SAPPPMES_ZPTYPE", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZPTYPE { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 40)]
        [Display(Name = "SAPPPMES_BWART_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string BWART_H { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 50)]
        [Display(Name = "SAPPPMES_LGORT_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LGORT_H { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 60)]
        [Display(Name = "SAPPPMES_ERFMG_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ERFMG_H { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 65)]
        [Display(Name = "SAPPPMES_BLDAT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime BLDAT { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 68)]
        [Display(Name = "SAPPPMES_BUDAT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime BUDAT { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 80)]
        [Display(Name = "SAPPPMES_LMNGA_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LMNGA_H { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 90)]
        [Display(Name = "SAPPPMES_XMNGA", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string XMNGA { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 95)]
        [Display(Name = "SAPPPMES_GRUND", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string GRUND { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 70)]
        [Display(Name = "SAPPPMES_ZComnum", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZComnum { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 100)]
        [Display(Name = "SAPPPMES_ZMESGUID", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZMESGUID { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 120, ExportTitle = "SAPPPMES_MATNR_H", ExportTitleResourceType = typeof(Resources.SI.SAPPPMES))]
        [Display(Name = "SAPPPMES_LFSNR", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LFSNR { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 120, ExportTitle = "SAPPPMES_MATNR_H", ExportTitleResourceType = typeof(Resources.SI.SAPPPMES))]
        [Display(Name = "SAPPPMES_MATNR_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string Item { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 110)]
        [Display(Name = "SAPPPMES_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime ZCSRQSJ { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 140)]
        [Display(Name = "SAPPPMES_Status", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public Int32 Status { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 120)]
        [Display(Name = "SAPPPMES_BatchNo", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPPPMES0004", ExportSeq = 130)]
        [Display(Name = "SAPPPMES_UniqueCode", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string UniqueCode { get; set; }

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
            SAPPPMES0004 another = obj as SAPPPMES0004;

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
