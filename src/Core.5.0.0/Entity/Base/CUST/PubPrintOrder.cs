using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class PubPrintOrder : EntityBase, IAuditable
    {
        public int Id { get; set; }

        public string ExcelTemplate { get; set; }
        public string Code { get; set; }
        public string Printer { get; set; }
        public string Client { get; set; }
        public Boolean IsPrinted { get; set; }
        public string PrintUrl { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
    }
}
