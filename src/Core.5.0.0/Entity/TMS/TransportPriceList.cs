using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    public partial class TransportPriceList
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TransportMode, ValueField = "TransportMode")]
        [Display(Name = "TransportPriceList_TransportMode", ResourceType = typeof(Resources.TMS.TransportPriceList))]
        public string TransportModeDescription { get; set; }


        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.Description))
                {
                    return this.Code;
                }
                else
                {
                    return this.Code + "[" + this.Description + "]";
                }
            }
        }
        #endregion
    }
}