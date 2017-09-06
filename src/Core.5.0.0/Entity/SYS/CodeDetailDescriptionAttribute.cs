// -----------------------------------------------------------------------
// <copyright file="CodeDetailDescriptionAttribute.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace com.Sconit.Entity.SYS
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
     [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CodeDetailDescriptionAttribute : Attribute
    {
         public com.Sconit.CodeMaster.CodeMaster CodeMaster { get; set; }
         public string ValueField { get; set; }
    }
}
