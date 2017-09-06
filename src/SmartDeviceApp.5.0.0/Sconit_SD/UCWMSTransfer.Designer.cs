namespace com.Sconit.SmartDevice
{
    partial class UCWMSTransfer
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
            this.dgDetail = new System.Windows.Forms.DataGrid();
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
            // dgDetail
            // 
            this.dgDetail.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dgDetail.Location = new System.Drawing.Point(3, 44);
            this.dgDetail.Name = "dgDetail";
            this.dgDetail.Size = new System.Drawing.Size(233, 255);
            this.dgDetail.TabIndex = 137;
            // 
            // UCWMSTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.dgDetail);
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.tbBarCode);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblBarCode);
            this.Name = "UCWMSTransfer";
            this.Size = new System.Drawing.Size(240, 299);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOrder;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblBarCode;
        public System.Windows.Forms.TextBox tbBarCode;
        private System.Windows.Forms.DataGrid dgDetail;


    }
}
