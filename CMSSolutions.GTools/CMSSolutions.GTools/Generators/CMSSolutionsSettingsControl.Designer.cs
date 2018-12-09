namespace CMSSolutions.GTools.Common
{
    partial class CMSSolutionsSettingsControl
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
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.cbGenerateDomainEntities = new System.Windows.Forms.CheckBox();
            this.cbGenerateServices = new System.Windows.Forms.CheckBox();
            this.cbGenerateViewModels = new System.Windows.Forms.CheckBox();
            this.cbGenerateControllers = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblOutputDirectory = new System.Windows.Forms.Label();
            this.txtOutputDirectory = new System.Windows.Forms.TextBox();
            this.dlgFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.cbGenerateBreadcrumbs = new System.Windows.Forms.CheckBox();
            this.grpEntities = new System.Windows.Forms.GroupBox();
            this.cbIncludeSerializableAttributes = new System.Windows.Forms.CheckBox();
            this.grpViewModels = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grpControllers = new System.Windows.Forms.GroupBox();
            this.grpEntities.SuspendLayout();
            this.grpViewModels.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpControllers.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtProjectName
            // 
            this.txtProjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProjectName.Location = new System.Drawing.Point(84, 10);
            this.txtProjectName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(366, 20);
            this.txtProjectName.TabIndex = 0;
            this.txtProjectName.Text = "CMSSolutions.Websites";
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Location = new System.Drawing.Point(10, 12);
            this.lblProjectName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(71, 13);
            this.lblProjectName.TabIndex = 1;
            this.lblProjectName.Text = "Project Name";
            // 
            // cbGenerateDomainEntities
            // 
            this.cbGenerateDomainEntities.AutoSize = true;
            this.cbGenerateDomainEntities.Checked = true;
            this.cbGenerateDomainEntities.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGenerateDomainEntities.Location = new System.Drawing.Point(10, 20);
            this.cbGenerateDomainEntities.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbGenerateDomainEntities.Name = "cbGenerateDomainEntities";
            this.cbGenerateDomainEntities.Size = new System.Drawing.Size(146, 17);
            this.cbGenerateDomainEntities.TabIndex = 2;
            this.cbGenerateDomainEntities.Text = "Generate Domain Entities";
            this.cbGenerateDomainEntities.UseVisualStyleBackColor = true;
            // 
            // cbGenerateServices
            // 
            this.cbGenerateServices.AutoSize = true;
            this.cbGenerateServices.Checked = true;
            this.cbGenerateServices.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGenerateServices.Location = new System.Drawing.Point(10, 20);
            this.cbGenerateServices.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbGenerateServices.Name = "cbGenerateServices";
            this.cbGenerateServices.Size = new System.Drawing.Size(114, 17);
            this.cbGenerateServices.TabIndex = 5;
            this.cbGenerateServices.Text = "Generate Services";
            this.cbGenerateServices.UseVisualStyleBackColor = true;
            // 
            // cbGenerateViewModels
            // 
            this.cbGenerateViewModels.AutoSize = true;
            this.cbGenerateViewModels.Checked = true;
            this.cbGenerateViewModels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGenerateViewModels.Location = new System.Drawing.Point(10, 20);
            this.cbGenerateViewModels.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbGenerateViewModels.Name = "cbGenerateViewModels";
            this.cbGenerateViewModels.Size = new System.Drawing.Size(133, 17);
            this.cbGenerateViewModels.TabIndex = 4;
            this.cbGenerateViewModels.Text = "Generate View Models";
            this.cbGenerateViewModels.UseVisualStyleBackColor = true;
            // 
            // cbGenerateControllers
            // 
            this.cbGenerateControllers.AutoSize = true;
            this.cbGenerateControllers.Checked = true;
            this.cbGenerateControllers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGenerateControllers.Location = new System.Drawing.Point(9, 20);
            this.cbGenerateControllers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbGenerateControllers.Name = "cbGenerateControllers";
            this.cbGenerateControllers.Size = new System.Drawing.Size(122, 17);
            this.cbGenerateControllers.TabIndex = 6;
            this.cbGenerateControllers.Text = "Generate Controllers";
            this.cbGenerateControllers.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(394, 32);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(56, 19);
            this.btnBrowse.TabIndex = 9;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblOutputDirectory
            // 
            this.lblOutputDirectory.AutoSize = true;
            this.lblOutputDirectory.Location = new System.Drawing.Point(10, 35);
            this.lblOutputDirectory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOutputDirectory.Name = "lblOutputDirectory";
            this.lblOutputDirectory.Size = new System.Drawing.Size(39, 13);
            this.lblOutputDirectory.TabIndex = 11;
            this.lblOutputDirectory.Text = "Output";
            // 
            // txtOutputDirectory
            // 
            this.txtOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDirectory.Location = new System.Drawing.Point(84, 32);
            this.txtOutputDirectory.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtOutputDirectory.Name = "txtOutputDirectory";
            this.txtOutputDirectory.Size = new System.Drawing.Size(306, 20);
            this.txtOutputDirectory.TabIndex = 10;
            this.txtOutputDirectory.Text = "C:\\Test";
            // 
            // cbGenerateBreadcrumbs
            // 
            this.cbGenerateBreadcrumbs.AutoSize = true;
            this.cbGenerateBreadcrumbs.Location = new System.Drawing.Point(135, 20);
            this.cbGenerateBreadcrumbs.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbGenerateBreadcrumbs.Name = "cbGenerateBreadcrumbs";
            this.cbGenerateBreadcrumbs.Size = new System.Drawing.Size(135, 17);
            this.cbGenerateBreadcrumbs.TabIndex = 12;
            this.cbGenerateBreadcrumbs.Text = "Generate Breadcrumbs";
            this.cbGenerateBreadcrumbs.UseVisualStyleBackColor = true;
            // 
            // grpEntities
            // 
            this.grpEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEntities.Controls.Add(this.cbIncludeSerializableAttributes);
            this.grpEntities.Controls.Add(this.cbGenerateDomainEntities);
            this.grpEntities.Location = new System.Drawing.Point(10, 68);
            this.grpEntities.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpEntities.Name = "grpEntities";
            this.grpEntities.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpEntities.Size = new System.Drawing.Size(437, 49);
            this.grpEntities.TabIndex = 13;
            this.grpEntities.TabStop = false;
            this.grpEntities.Text = "Entities";
            // 
            // cbIncludeSerializableAttributes
            // 
            this.cbIncludeSerializableAttributes.AutoSize = true;
            this.cbIncludeSerializableAttributes.Location = new System.Drawing.Point(159, 20);
            this.cbIncludeSerializableAttributes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbIncludeSerializableAttributes.Name = "cbIncludeSerializableAttributes";
            this.cbIncludeSerializableAttributes.Size = new System.Drawing.Size(206, 17);
            this.cbIncludeSerializableAttributes.TabIndex = 3;
            this.cbIncludeSerializableAttributes.Text = "Include Serializable Attributes for WCF";
            this.cbIncludeSerializableAttributes.UseVisualStyleBackColor = true;
            // 
            // grpViewModels
            // 
            this.grpViewModels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpViewModels.Controls.Add(this.cbGenerateViewModels);
            this.grpViewModels.Location = new System.Drawing.Point(10, 122);
            this.grpViewModels.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpViewModels.Name = "grpViewModels";
            this.grpViewModels.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpViewModels.Size = new System.Drawing.Size(437, 49);
            this.grpViewModels.TabIndex = 14;
            this.grpViewModels.TabStop = false;
            this.grpViewModels.Text = "Views";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cbGenerateServices);
            this.groupBox2.Location = new System.Drawing.Point(10, 176);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(437, 49);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Services";
            // 
            // grpControllers
            // 
            this.grpControllers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpControllers.Controls.Add(this.cbGenerateControllers);
            this.grpControllers.Controls.Add(this.cbGenerateBreadcrumbs);
            this.grpControllers.Location = new System.Drawing.Point(12, 229);
            this.grpControllers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpControllers.Name = "grpControllers";
            this.grpControllers.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpControllers.Size = new System.Drawing.Size(437, 49);
            this.grpControllers.TabIndex = 15;
            this.grpControllers.TabStop = false;
            this.grpControllers.Text = "Controllers";
            // 
            // CMSSolutionsSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpControllers);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grpViewModels);
            this.Controls.Add(this.grpEntities);
            this.Controls.Add(this.lblOutputDirectory);
            this.Controls.Add(this.txtOutputDirectory);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblProjectName);
            this.Controls.Add(this.txtProjectName);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "CMSSolutionsSettingsControl";
            this.Size = new System.Drawing.Size(460, 355);
            this.grpEntities.ResumeLayout(false);
            this.grpEntities.PerformLayout();
            this.grpViewModels.ResumeLayout(false);
            this.grpViewModels.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grpControllers.ResumeLayout(false);
            this.grpControllers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.CheckBox cbGenerateDomainEntities;
        private System.Windows.Forms.CheckBox cbGenerateServices;
        private System.Windows.Forms.CheckBox cbGenerateViewModels;
        private System.Windows.Forms.CheckBox cbGenerateControllers;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblOutputDirectory;
        private System.Windows.Forms.TextBox txtOutputDirectory;
        private System.Windows.Forms.FolderBrowserDialog dlgFolderBrowser;
        private System.Windows.Forms.CheckBox cbGenerateBreadcrumbs;
        private System.Windows.Forms.GroupBox grpEntities;
        private System.Windows.Forms.GroupBox grpViewModels;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox grpControllers;
        private System.Windows.Forms.CheckBox cbIncludeSerializableAttributes;
    }
}
