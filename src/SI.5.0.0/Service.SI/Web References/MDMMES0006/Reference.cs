﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.18444.
// 
#pragma warning disable 1591

namespace com.Sconit.Service.SI.MDMMES0006 {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    // CODEGEN: The optional WSDL extension element 'Policy' from namespace 'http://schemas.xmlsoap.org/ws/2004/09/policy' was not handled.
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ZWS_MDMMES0006", Namespace="urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZWS_MDMMES0006 : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback ZfunMdmmes0006OperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public ZWS_MDMMES0006() {
            this.Url = global::com.Sconit.Service.SI.Properties.Settings.Default.com_Sconit_Service_SI_MDMMES0006_ZWS_MDMMES0006;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event ZfunMdmmes0006CompletedEventHandler ZfunMdmmes0006Completed;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:sap-com:document:sap:soap:functions:mc-style:ZWS_MDMMES0006:ZfunMdmmes0006Req" +
            "uest", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ZfunMdmmes0006Response", Namespace="urn:sap-com:document:sap:soap:functions:mc-style")]
        public ZfunMdmmes0006Response ZfunMdmmes0006([System.Xml.Serialization.XmlElementAttribute("ZfunMdmmes0006", Namespace="urn:sap-com:document:sap:soap:functions:mc-style")] ZfunMdmmes0006 ZfunMdmmes00061) {
            object[] results = this.Invoke("ZfunMdmmes0006", new object[] {
                        ZfunMdmmes00061});
            return ((ZfunMdmmes0006Response)(results[0]));
        }
        
        /// <remarks/>
        public void ZfunMdmmes0006Async(ZfunMdmmes0006 ZfunMdmmes00061) {
            this.ZfunMdmmes0006Async(ZfunMdmmes00061, null);
        }
        
        /// <remarks/>
        public void ZfunMdmmes0006Async(ZfunMdmmes0006 ZfunMdmmes00061, object userState) {
            if ((this.ZfunMdmmes0006OperationCompleted == null)) {
                this.ZfunMdmmes0006OperationCompleted = new System.Threading.SendOrPostCallback(this.OnZfunMdmmes0006OperationCompleted);
            }
            this.InvokeAsync("ZfunMdmmes0006", new object[] {
                        ZfunMdmmes00061}, this.ZfunMdmmes0006OperationCompleted, userState);
        }
        
        private void OnZfunMdmmes0006OperationCompleted(object arg) {
            if ((this.ZfunMdmmes0006Completed != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ZfunMdmmes0006Completed(this, new ZfunMdmmes0006CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZfunMdmmes0006 {
        
        private string lBukrsField;
        
        private string lDateField;
        
        private string lKunnrField;
        
        private string lTimeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LBukrs {
            get {
                return this.lBukrsField;
            }
            set {
                this.lBukrsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LDate {
            get {
                return this.lDateField;
            }
            set {
                this.lDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LKunnr {
            get {
                return this.lKunnrField;
            }
            set {
                this.lKunnrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LTime {
            get {
                return this.lTimeField;
            }
            set {
                this.lTimeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZstrMdmmes0006 {
        
        private string kunnrField;
        
        private string name1Field;
        
        private string bukrsField;
        
        private string countryField;
        
        private string cityField;
        
        private string remarkField;
        
        private string telf1Field;
        
        private string telfxField;
        
        private string parnrField;
        
        private string pstlzField;
        
        private string telbxField;
        
        private string telf2Field;
        
        private string loevmField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Kunnr {
            get {
                return this.kunnrField;
            }
            set {
                this.kunnrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Name1 {
            get {
                return this.name1Field;
            }
            set {
                this.name1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Bukrs {
            get {
                return this.bukrsField;
            }
            set {
                this.bukrsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Country {
            get {
                return this.countryField;
            }
            set {
                this.countryField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string City {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Remark {
            get {
                return this.remarkField;
            }
            set {
                this.remarkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Telf1 {
            get {
                return this.telf1Field;
            }
            set {
                this.telf1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Telfx {
            get {
                return this.telfxField;
            }
            set {
                this.telfxField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Parnr {
            get {
                return this.parnrField;
            }
            set {
                this.parnrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Pstlz {
            get {
                return this.pstlzField;
            }
            set {
                this.pstlzField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Telbx {
            get {
                return this.telbxField;
            }
            set {
                this.telbxField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Telf2 {
            get {
                return this.telf2Field;
            }
            set {
                this.telf2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Loevm {
            get {
                return this.loevmField;
            }
            set {
                this.loevmField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZfunMdmmes0006Response {
        
        private ZstrMdmmes0006[] itMdmmes0006Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZstrMdmmes0006[] ItMdmmes0006 {
            get {
                return this.itMdmmes0006Field;
            }
            set {
                this.itMdmmes0006Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void ZfunMdmmes0006CompletedEventHandler(object sender, ZfunMdmmes0006CompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ZfunMdmmes0006CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ZfunMdmmes0006CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ZfunMdmmes0006Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ZfunMdmmes0006Response)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591