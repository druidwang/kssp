namespace com.Sconit.SmartDevice
{
    partial class UCBase
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
        protected void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnOrder = new System.Windows.Forms.Button();
            this.tbBarCode = new System.Windows.Forms.TextBox();
            this.lblBarCode = new System.Windows.Forms.Label();
            this.dgList = new System.Windows.Forms.DataGrid();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(7, 23);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(229, 20);
            this.lblMessage.Text = "Message";
            // 
            // btnOrder
            // 
            this.btnOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOrder.Location = new System.Drawing.Point(235, 0);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(40, 20);
            this.btnOrder.TabIndex = 12;
            this.btnOrder.Text = "确定";
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);
            this.btnOrder.GotFocus += new System.EventHandler(this.btnOrder_GotFocus);
            this.btnOrder.KeyUp += new System.Windows.Forms.KeyEventHandler(this.btnOrder_KeyUp);
            this.btnOrder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UCBase_KeyPress);
            // 
            // tbBarCode
            // 
            this.tbBarCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBarCode.Location = new System.Drawing.Point(39, 0);
            this.tbBarCode.MaxLength = 30;
            this.tbBarCode.Name = "tbBarCode";
            this.tbBarCode.Size = new System.Drawing.Size(190, 23);
            this.tbBarCode.TabIndex = 11;
            this.tbBarCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbBarCode_KeyUp);
            this.tbBarCode.LostFocus += new System.EventHandler(this.tbBarCode_LostFocus);
            // 
            // lblBarCode
            // 
            this.lblBarCode.Location = new System.Drawing.Point(2, 4);
            this.lblBarCode.Name = "lblBarCode";
            this.lblBarCode.Size = new System.Drawing.Size(43, 20);
            this.lblBarCode.Text = "条码:";
            // 
            // dgList
            // 
            this.dgList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgList.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dgList.Location = new System.Drawing.Point(0, 44);
            this.dgList.Name = "dgList";
            this.dgList.RowHeadersVisible = false;
            this.dgList.Size = new System.Drawing.Size(279, 206);
            this.dgList.TabIndex = 10;
            this.dgList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UCBase_KeyPress);
            this.dgList.GotFocus += new System.EventHandler(this.dgList_GotFocus);
            this.dgList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UCBase_KeyDown);
            // 
            // UCBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.tbBarCode);
            this.Controls.Add(this.lblBarCode);
            this.Controls.Add(this.dgList);
            this.Name = "UCBase";
            this.Size = new System.Drawing.Size(279, 276);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UCBase_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UCBase_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Label lblBarCode;
        protected System.Windows.Forms.Label lblMessage;
        protected System.Windows.Forms.DataGrid dgList;
        public System.Windows.Forms.TextBox tbBarCode;
        protected System.Windows.Forms.Button btnOrder;

    }
}
