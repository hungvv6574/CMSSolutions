namespace CMSSolutions.GTools.Views
{
    partial class GeneratorSettingsControl
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
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.cmbGenerator = new System.Windows.Forms.ComboBox();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.grpSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSettings.Location = new System.Drawing.Point(8, 57);
            this.pnlSettings.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(769, 432);
            this.pnlSettings.TabIndex = 1;
            // 
            // grpSettings
            // 
            this.grpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSettings.Controls.Add(this.pnlSettings);
            this.grpSettings.Controls.Add(this.cmbGenerator);
            this.grpSettings.Location = new System.Drawing.Point(18, 16);
            this.grpSettings.Margin = new System.Windows.Forms.Padding(4);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Padding = new System.Windows.Forms.Padding(4);
            this.grpSettings.Size = new System.Drawing.Size(785, 496);
            this.grpSettings.TabIndex = 1;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Generator Settings";
            // 
            // cmbGenerator
            // 
            this.cmbGenerator.FormattingEnabled = true;
            this.cmbGenerator.Location = new System.Drawing.Point(8, 23);
            this.cmbGenerator.Margin = new System.Windows.Forms.Padding(4);
            this.cmbGenerator.Name = "cmbGenerator";
            this.cmbGenerator.Size = new System.Drawing.Size(209, 24);
            this.cmbGenerator.TabIndex = 0;
            this.cmbGenerator.SelectedIndexChanged += new System.EventHandler(this.cmbGenerator_SelectedIndexChanged);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.Filter = "Xml Files|*.xml";
            // 
            // GeneratorSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpSettings);
            this.Name = "GeneratorSettingsControl";
            this.Size = new System.Drawing.Size(821, 528);
            this.grpSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.ComboBox cmbGenerator;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;


    }
}
