using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPPPMES0003 : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 10)]
        [Display(Name = "SAPPPMES_ZMESSC", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZMESSC { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 20)]
        [Display(Name = "SAPPPMES_ZMESLN", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZMESLN { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 30)]
        [Display(Name = "SAPPPMES_ZPTYPE", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZPTYPE { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 50)]
        [Display(Name = "SAPPPMES_WERKS", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string WERKS { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 50)]
        [Display(Name = "SAPPPMES_GMEIN_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string GMEIN_H { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 50)]
        [Display(Name = "SAPPPMES_GRUND", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string GRUND { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 40)]
        [Display(Name = "SAPPPMES_XMNGA", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string XMNGA { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 60)]
        [Display(Name = "SAPPPMES_ZComnum", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZComnum { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 100)]
        [Display(Name = "SAPPPMES_BLDAT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime BLDAT { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 110)]
        [Display(Name = "SAPPPMES_BUDAT", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime BUDAT { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 190)]
        [Display(Name = "SAPPPMES_BWART_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string BWART_I { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 200)]
        [Display(Name = "SAPPPMES_MATNR_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string MATNR_I { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 210)]
        [Display(Name = "SAPPPMES_ERFMG_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ERFMG_I { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 220)]
        [Display(Name = "SAPPPMES_GMEIN_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string GMEIN_I { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 230)]
        [Display(Name = "SAPPPMES_LGORT_I", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string LGORT_I { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 70)]
        [Display(Name = "SAPPPMES_ZMESGUID", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string ZMESGUID { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 80)]
        [Display(Name = "SAPPPMES_ZCSRQSJ", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public DateTime ZCSRQSJ { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 110)]
        [Display(Name = "SAPPPMES_Status", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public Int32 Status { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 90)]
        [Display(Name = "SAPPPMES_BatchNo", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string BatchNo { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 100)]
        [Display(Name = "SAPPPMES_UniqueCode", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string UniqueCode { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 100)]
        [Display(Name = "SAPPPMES_LFSNR", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string MTSNR { get; set; }
        [Export(ExportName = "SAPPPMES0003", ExportSeq = 270)]
        [Display(Name = "SAPPPMES_TailQty", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string TailQty { get; set; }

        [Export(ExportName = "SAPPPMES0003", ExportSeq = 270)]
        [Display(Name = "SAPPPMES_MATNR_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string Item { get; set; }
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
            SAPPPMES0003 another = obj as SAPPPMES0003;

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
