using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.SI.SAP
{
    [Serializable]
    public partial class SAPSnapShotInv : EntityBase
    {
        #region O/R Mapping Properties
		
        public Int32 Id { get; set; }
        public string MATNR { get; set; }
        public string WERKS { get; set; }
        public string LGORT { get; set; }
        public string LIFNR { get; set; }
        public DateTime MesInvSnaptime { get; set; }
        public string MesTotalInv { get; set; }
        public string MesInspectQty { get; set; }
        public string MesRejectQty { get; set; }
        public string MesQualifiedQty { get; set; }
        public string MesTransIpQty { get; set; }
        public string MesSalesIpQty { get; set; }
        public string MesCsQty { get; set; }
        
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
            SAPSnapShotInv another = obj as SAPSnapShotInv;

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
