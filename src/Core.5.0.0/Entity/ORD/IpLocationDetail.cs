using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class IpLocationDetail
    {
        #region Non O/R Mapping Properties
        [Display(Name = "IpLocationDetail_RemainReceiveQty", ResourceType = typeof(Resources.ORD.IpLocationDetail))] 
        public decimal RemainReceiveQty
        {
            get
            {
                return this.Qty - this.ReceivedQty;
            }
        }

        //差异调整的时候用的
        public IpDetail IpDetail
        {
            get;
            set;
        }

        #endregion
    }
}