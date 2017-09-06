namespace com.Sconit.SmartDevice
{
    partial class UCMaterialReturn
    {
        ///// <summary> 
        ///// Required designer variable.
        ///// </summary>
        //private System.ComponentModel.IContainer components = null;

        ///// <summary> 
        ///// Clean up any resources being used.
        ///// </summary>
        ///// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    //base.Dispose(disposing);
        //}

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOrder = new System.Windows.Forms.Button();
            this.tbBarCode = new System.Windows.Forms.TextBox();
            this.lblFgInfo = new System.Windows.Forms.Label();
            this.lblFg = new System.Windows.Forms.Label();
            this.lblFgDescription = new System.Windows.Forms.Label();
            this.lblFgDescInfo = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblVANInfo = new System.Windows.Forms.Label();
            this.lblVAN = new System.Windows.Forms.Label();
            this.lblBarCode = new System.Windows.Forms.Label();
            this.tabPanel = new System.Windows.Forms.TabControl();
            this.tabWo = new System.Windows.Forms.TabPage();
            this.lblWoInfo = new System.Windows.Forms.Label();
            this.lblWo = new System.Windows.Forms.Label();
            this.lblFlowInfo = new System.Windows.Forms.Label();
            this.lblFlow = new System.Windows.Forms.Label();
            this.lblSeqInfo = new System.Windows.Forms.Label();
            this.lblSeq = new System.Windows.Forms.Label();
            this.tabMaterial = new System.Windows.Forms.TabPage();
            this.dgDetail = new System.Windows.Forms.DataGrid();
            this.tabPanel.SuspendLayout();
            this.tabWo.SuspendLayout();
            this.tabMaterial.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOrder
            // 
            this.btnOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOrder.Location = new System.Drawing.Point(198, 1);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(40, 20);
            this.btnOrder.TabIndex = 116;
            this.btnOrder.Text = "确定";
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);
            this.btnOrder.KeyUp += new System.Windows.Forms.KeyEventHandler(this.btnOrder_KeyUp);
            // 
            // tbBarCode
            // 
            this.tbBarCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBarCode.Location = new System.Drawing.Point(41, 0);
            this.tbBarCode.MaxLength = 50;
            this.tbBarCode.Name = "tbBarCode";
            this.tbBarCode.Size = new System.Drawing.Size(151, 23);
            this.tbBarCode.TabIndex = 115;
            this.tbBarCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbBarCode_KeyUp);
            this.tbBarCode.LostFocus += new System.EventHandler(this.tbBarCode_LostFocus);
            // 
            // lblFgInfo
            // 
            this.lblFgInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblFgInfo.Location = new System.Drawing.Point(45, 83);
            this.lblFgInfo.Name = "lblFgInfo";
            this.lblFgInfo.Size = new System.Drawing.Size(177, 16);
            this.lblFgInfo.Text = "5801333266_50000";
            // 
            // lblFg
            // 
            this.lblFg.Location = new System.Drawing.Point(4, 83);
            this.lblFg.Name = "lblFg";
            this.lblFg.Size = new System.Drawing.Size(43, 18);
            this.lblFg.Text = "成品:";
            // 
            // lblFgDescription
            // 
            this.lblFgDescription.Location = new System.Drawing.Point(4, 104);
            this.lblFgDescription.Name = "lblFgDescription";
            this.lblFgDescription.Size = new System.Drawing.Size(41, 18);
            this.lblFgDescription.Text = "描述:";
            // 
            // lblFgDescInfo
            // 
            this.lblFgDescInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblFgDescInfo.Location = new System.Drawing.Point(45, 104);
            this.lblFgDescInfo.Name = "lblFgDescInfo";
            this.lblFgDescInfo.Size = new System.Drawing.Size(179, 52);
            this.lblFgDescInfo.Text = "新大康平顶驾驶室总";
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular);
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(2, 25);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(234, 16);
            this.lblMessage.Text = "123456789012345678901234567890";
            // 
            // lblVANInfo
            // 
            this.lblVANInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblVANInfo.Location = new System.Drawing.Point(45, 64);
            this.lblVANInfo.Name = "lblVANInfo";
            this.lblVANInfo.Size = new System.Drawing.Size(179, 16);
            this.lblVANInfo.Text = "123456789012345678901234567890";
            // 
            // lblVAN
            // 
            this.lblVAN.Location = new System.Drawing.Point(4, 64);
            this.lblVAN.Name = "lblVAN";
            this.lblVAN.Size = new System.Drawing.Size(41, 18);
            this.lblVAN.Text = "VAN:";
            // 
            // lblBarCode
            // 
            this.lblBarCode.Location = new System.Drawing.Point(4, 1);
            this.lblBarCode.Name = "lblBarCode";
            this.lblBarCode.Size = new System.Drawing.Size(151, 23);
            this.lblBarCode.Text = "条码:";
            // 
            // tabPanel
            // 
            this.tabPanel.Controls.Add(this.tabWo);
            this.tabPanel.Controls.Add(this.tabMaterial);
            this.tabPanel.Location = new System.Drawing.Point(4, 45);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.SelectedIndex = 0;
            this.tabPanel.Size = new System.Drawing.Size(232, 272);
            this.tabPanel.TabIndex = 140;
            // 
            // tabWo
            // 
            this.tabWo.Controls.Add(this.lblWoInfo);
            this.tabWo.Controls.Add(this.lblWo);
            this.tabWo.Controls.Add(this.lblVANInfo);
            this.tabWo.Controls.Add(this.lblVAN);
            this.tabWo.Controls.Add(this.lblFgDescInfo);
            this.tabWo.Controls.Add(this.lblFlowInfo);
            this.tabWo.Controls.Add(this.lblFlow);
            this.tabWo.Controls.Add(this.lblSeqInfo);
            this.tabWo.Controls.Add(this.lblSeq);
            this.tabWo.Controls.Add(this.lblFgDescription);
            this.tabWo.Controls.Add(this.lblFg);
            this.tabWo.Controls.Add(this.lblFgInfo);
            this.tabWo.Location = new System.Drawing.Point(4, 25);
            this.tabWo.Name = "tabWo";
            this.tabWo.Size = new System.Drawing.Size(224, 243);
            this.tabWo.Text = "工单";
            // 
            // lblWoInfo
            // 
            this.lblWoInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblWoInfo.Location = new System.Drawing.Point(45, 46);
            this.lblWoInfo.Name = "lblWoInfo";
            this.lblWoInfo.Size = new System.Drawing.Size(179, 16);
            this.lblWoInfo.Text = "123456789012345678901234567890";
            // 
            // lblWo
            // 
            this.lblWo.Location = new System.Drawing.Point(4, 46);
            this.lblWo.Name = "lblWo";
            this.lblWo.Size = new System.Drawing.Size(41, 18);
            this.lblWo.Text = "工单:";
            // 
            // lblFlowInfo
            // 
            this.lblFlowInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblFlowInfo.Location = new System.Drawing.Point(45, 26);
            this.lblFlowInfo.Name = "lblFlowInfo";
            this.lblFlowInfo.Size = new System.Drawing.Size(176, 18);
            this.lblFlowInfo.Text = "A";
            // 
            // lblFlow
            // 
            this.lblFlow.Location = new System.Drawing.Point(4, 26);
            this.lblFlow.Name = "lblFlow";
            this.lblFlow.Size = new System.Drawing.Size(41, 18);
            this.lblFlow.Text = "产线:";
            // 
            // lblSeqInfo
            // 
            this.lblSeqInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblSeqInfo.Location = new System.Drawing.Point(45, 7);
            this.lblSeqInfo.Name = "lblSeqInfo";
            this.lblSeqInfo.Size = new System.Drawing.Size(176, 18);
            this.lblSeqInfo.Text = "10";
            // 
            // lblSeq
            // 
            this.lblSeq.Location = new System.Drawing.Point(4, 7);
            this.lblSeq.Name = "lblSeq";
            this.lblSeq.Size = new System.Drawing.Size(41, 18);
            this.lblSeq.Text = "序号:";
            // 
            // tabMaterial
            // 
            this.tabMaterial.Controls.Add(this.dgDetail);
            this.tabMaterial.Location = new System.Drawing.Point(4, 25);
            this.tabMaterial.Name = "tabMaterial";
            this.tabMaterial.Size = new System.Drawing.Size(224, 243);
            this.tabMaterial.Text = "投料";
            // 
            // dgDetail
            // 
            this.dgDetail.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dgDetail.Location = new System.Drawing.Point(0, 0);
            this.dgDetail.Name = "dgDetail";
            this.dgDetail.Size = new System.Drawing.Size(224, 237);
            this.dgDetail.TabIndex = 136;
            // 
            // UCMaterialReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tabPanel);
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.tbBarCode);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblBarCode);
            this.Name = "UCMaterialReturn";
            this.Size = new System.Drawing.Size(240, 320);
            this.tabPanel.ResumeLayout(false);
            this.tabWo.ResumeLayout(false);
            this.tabMaterial.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOrder;
        private System.Windows.Forms.Label lblFgInfo;
        private System.Windows.Forms.Label lblFg;
        private System.Windows.Forms.Label lblFgDescription;
        private System.Windows.Forms.Label lblFgDescInfo;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblVANInfo;
        private System.Windows.Forms.Label lblVAN;
        private System.Windows.Forms.Label lblBarCode;
        private System.Windows.Forms.TabControl tabPanel;
        private System.Windows.Forms.TabPage tabWo;
        private System.Windows.Forms.TabPage tabMaterial;
        private System.Windows.Forms.Label lblWoInfo;
        private System.Windows.Forms.Label lblWo;
        private System.Windows.Forms.Label lblSeq;
        private System.Windows.Forms.Label lblSeqInfo;
        private System.Windows.Forms.Label lblFlowInfo;
        private System.Windows.Forms.Label lblFlow;
        private System.Windows.Forms.DataGrid dgDetail;
        public System.Windows.Forms.TextBox tbBarCode;


    }
}
