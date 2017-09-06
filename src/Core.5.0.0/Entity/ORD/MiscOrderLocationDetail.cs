using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class MiscOrderLocationDetail
    {
        #region Non O/R Mapping Properties
         public string  ReferenceItemCode{get;set;}
        public string  ItemDescription{get;set;}
        public Decimal UnitCount { get; set; }
        public string  Location{get;set;}
        public string  ReserveNo{get;set;}
        public string  ReserveLine{get;set;}
        public string EBELN { get; set; }
        public string EBELP { get; set; }
        public string ManufactureParty { get; set; }
        //TODO: Add Non O/R Mapping Properties here. 

        #endregion
    }
}