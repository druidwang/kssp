using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.Serialization;

//TODO: Add other using statements here

namespace com.Sconit.Entity.INV
{
    public partial class KanBanCard
    {
        #region Non O/R Mapping Properties

        [DataMember]
        public IList<KanBanCardInfo> KanBanDetails { get; set; }
        #endregion
    }
}