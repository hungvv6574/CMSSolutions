namespace CMSSolutions.GTools.Views
{
    partial class ConnectionsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.btnValidateConnection = new System.Windows.Forms.Button();
            this.pnlConnection = new System.Windows.Forms.Panel();
            this.cmbConnectionType = new System.Windows.Forms.ComboBox();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.grpConnection.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConnection.Controls.Add(this.btnValidateConnection);
            this.grpConnection.Controls.Add(this.pnlConnection);
            this.grpConnection.Controls.Add(this.cmbConnectionType);
            this.grpConnection.Location = new System.Drawing.Point(17, 16);
            this.grpConnection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpConnection.Size = new System.Drawing.Size(785, 496);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Connection Details";
            // 
            // btnValidateConnection
            // 
            this.btnValidateConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValidateConnection.Location = new System.Drawing.Point(677, 21);
            this.btnValidateConnection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnValidateConnection.Name = "btnValidateConnection";
            this.btnValidateConnection.Size = new System.Drawing.Size(100, 28);
            this.btnValidateConnection.TabIndex = 2;
            this.btnValidateConnection.Text = "Validate";
            this.btnValidateConnection.UseVisualStyleBackColor = true;
            this.btnValidateConnection.Click += new System.EventHandler(this.btnValidateConnection_Click);
            // 
            // pnlConnection
            // 
            this.pnlConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnection.Location = new System.Drawing.Point(8, 57);
            this.pnlConnection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlConnection.Name = "pnlConnection";
            this.pnlConnection.Size = new System.Drawing.Size(769, 432);
            this.pnlConnection.TabIndex = 1;
            // 
            // cmbConnectionType
            // 
            this.cmbConnectionType.FormattingEnabled = true;
            this.cmbConnectionType.Location = new System.Drawing.Point(8, 23);
            this.cmbConnectionType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbConnectionType.Name = "cmbConnectionType";
            this.cmbConnectionType.Size = new System.Drawing.Size(209, 24);
            this.cmbConnectionType.TabIndex = 0;
            this.cmbConnectionType.SelectedIndexChanged += new System.EventHandler(this.cmbConnectionType_SelectedIndexChanged);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.Filter = "Xml Files|*.xml";
            // 
            // ConnectionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.grpConnection);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ConnectionsControl";
            this.Size = new System.Drawing.Size(821, 528);
            this.grpConnection.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.Panel pnlConnection;
        private System.Windows.Forms.ComboBox cmbConnectionType;
        private System.Windows.Forms.Button btnValidateConnection;
    }
}
