using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.MES
{
    public class MES_Interface_CreatePallet
    {

        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        public string BoxNos { get; set; }
        public string BoxCount { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string Printer { get; set; }
        public string PalletId { get; set; }
        public Int32 Status { get; set; }
        public string Message { get; set; }
		
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
            MES_Interface_CreatePallet another = obj as MES_Interface_CreatePallet;

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
