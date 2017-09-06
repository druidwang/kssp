using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class KanBanCardInfo : EntityBase
    {
        #region O/R Mapping Properties

        [Display(Name = "KanBanCardInfo_CardNo", ResourceType = typeof(Resources.INV.KanBanCardInfo))]
		public string CardNo { get; set; }
		public string KBICode { get; set; }
        [Display(Name = "KanBanCardInfo_Sequence", ResourceType = typeof(Resources.INV.KanBanCardInfo))]
        public Int32 Sequence { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
            if (CardNo != null)
            {
                return CardNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            KanBanCardInfo another = obj as KanBanCardInfo;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.CardNo == another.CardNo);
            }
        } 
    }
	
}
