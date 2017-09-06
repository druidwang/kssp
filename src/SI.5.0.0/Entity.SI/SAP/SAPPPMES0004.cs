using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SI.SAP
{
    public partial class SAPPPMES0004
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [com.Sconit.Entity.SYS.Export(ExportName = "SAPPPMES0004", ExportSeq = 125)]
        public string FilterId { get; set; }
        [System.ComponentModel.DataAnnotations.Display(Name = "SAPPPMES_MATNR_H", ResourceType = typeof(Resources.SI.SAPPPMES))]
        public string MATNR_H { get; set; }
        #endregion
    }
}