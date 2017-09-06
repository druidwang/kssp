namespace com.Sconit.SmartDevice
{
    partial class UCQuickReturn
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblBarCode = new System.Windows.Forms.Label();
            this.tabPanel = new System.Windows.Forms.TabControl();
            this.tabFlow = new System.Windows.Forms.TabPage();
            this.lblFlowDescInfo = new System.Windows.Forms.Label();
            this.lblFlowDesc = new System.Windows.Forms.Label();
            this.lblFlowInfo = new System.Windows.Forms.Label();
            this.lblFlow = new System.Windows.Forms.Label();
            this.tabHu = new System.Windows.Forms.TabPage();
            this.dgDetail = new System.Windows.Forms.DataGrid();
            this.tabPanel.SuspendLayout();
            this.tabFlow.SuspendLayout();
            this.tabHu.SuspendLayout();
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
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(2, 25);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(234, 16);
            this.lblMessage.Text = "123456789012345678901234567890";
            // 
            // lblBarCode
            // 
            this.lblBarCode.Location = new System.Drawing.Point(4, 1);
            this.lblBarCode.Name = "lblBarCode";
            this.lblBarCode.Size = new System.Drawing.Size(40, 23);
            this.lblBarCode.Text = "条码:";
            // 
            // tabPanel
            // 
            this.tabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tabPanel.Controls.Add(this.tabFlow);
            this.tabPanel.Controls.Add(this.tabHu);
            this.tabPanel.Location = new System.Drawing.Point(3, 44);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.SelectedIndex = 0;
            this.tabPanel.Size = new System.Drawing.Size(232, 219);
            this.tabPanel.TabIndex = 140;
            // 
            // tabFlow
            // 
            this.tabFlow.Controls.Add(this.lblFlowDescInfo);
            this.tabFlow.Controls.Add(this.lblFlowDesc);
            this.tabFlow.Controls.Add(this.lblFlowInfo);
            this.tabFlow.Controls.Add(this.lblFlow);
            this.tabFlow.Location = new System.Drawing.Point(4, 25);
            this.tabFlow.Name = "tabFlow";
            this.tabFlow.Size = new System.Drawing.Size(224, 190);
            this.tabFlow.Text = "路线";
            // 
            // lblFlowDescInfo
            // 
            this.lblFlowDescInfo.Location = new System.Drawing.Point(43, 31);
            this.lblFlowDescInfo.Name = "lblFlowDescInfo";
            this.lblFlowDescInfo.Size = new System.Drawing.Size(178, 82);
            this.lblFlowDescInfo.Text = "123456789012345678901234567890";
            // 
            // lblFlowDesc
            // 
            this.lblFlowDesc.Location = new System.Drawing.Point(3, 31);
            this.lblFlowDesc.Name = "lblFlowDesc";
            this.lblFlowDesc.Size = new System.Drawing.Size(52, 20);
            this.lblFlowDesc.Text = "描述:";
            // 
            // lblFlowInfo
            // 
            this.lblFlowInfo.Location = new System.Drawing.Point(43, 11);
            this.lblFlowInfo.Name = "lblFlowInfo";
            this.lblFlowInfo.Size = new System.Drawing.Size(178, 20);
            this.lblFlowInfo.Text = "12345678901234567890";
            // 
            // lblFlow
            // 
            this.lblFlow.Location = new System.Drawing.Point(3, 11);
            this.lblFlow.Name = "lblFlow";
            this.lblFlow.Size = new System.Drawing.Size(52, 20);
            this.lblFlow.Text = "路线:";
            // 
            // tabHu
            // 
            this.tabHu.Controls.Add(this.dgDetail);
            this.tabHu.Location = new System.Drawing.Point(4, 25);
            this.tabHu.Name = "tabHu";
            this.tabHu.Size = new System.Drawing.Size(224, 236);
            this.tabHu.Text = "条码";
            // 
            // dgDetail
            // 
            this.dgDetail.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dgDetail.Location = new System.Drawing.Point(0, 0);
            this.dgDetail.Name = "dgDetail";
            this.dgDetail.Size = new System.Drawing.Size(224, 237);
            this.dgDetail.TabIndex = 136;
            // 
            // UCQuickReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.tbBarCode);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblBarCode);
            this.Controls.Add(this.tabPanel);
            this.Name = "UCQuickReturn";
            this.Size = new System.Drawing.Size(240, 299);
            this.tabPanel.ResumeLayout(false);
            this.tabFlow.ResumeLayout(false);
            this.tabHu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOrder;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblBarCode;
        private System.Windows.Forms.TabControl tabPanel;
        private System.Windows.Forms.TabPage tabFlow;
        private System.Windows.Forms.TabPage tabHu;
        private System.Windows.Forms.DataGrid dgDetail;
        public System.Windows.Forms.TextBox tbBarCode;
        private System.Windows.Forms.Label lblFlow;
        private System.Windows.Forms.Label lblFlowDesc;
        private System.Windows.Forms.Label lblFlowDescInfo;
        private System.Windows.Forms.Label lblFlowInfo;


    }
}
