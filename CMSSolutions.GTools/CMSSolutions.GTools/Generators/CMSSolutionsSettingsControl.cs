using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMSSolutions.GTools.Common
{
    public partial class CMSSolutionsSettingsControl : UserControl, IGeneratorSettings
    {
        public CMSSolutionsSettingsControl()
        {
            InitializeComponent();
        }

        public string ProjectName
        {
            get { return txtProjectName.Text; }
        }

        public string OutputDirectory
        {
            get { return txtOutputDirectory.Text; }
        }

        public bool GenerateDomainEntities
        {
            get { return cbGenerateDomainEntities.Checked; }
        }

        public bool IncludeSerializableAttributes
        {
            get { return cbIncludeSerializableAttributes.Checked; }
        }

        public bool GenerateControllers
        {
            get { return cbGenerateControllers.Checked; }
        }

        public bool GenerateBreadcrumbs
        {
            get { return cbGenerateBreadcrumbs.Checked; }
        }

        public bool GenerateServices
        {
            get { return cbGenerateServices.Checked; }
        }

        public bool GenerateViewModels
        {
            get { return cbGenerateViewModels.Checked; }
        }

        public UserControl Settings
        {
            get { return this; }
        }

        public bool ValidateSettings()
        {
            return true;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (dlgFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtOutputDirectory.Text = dlgFolderBrowser.SelectedPath;
            }
        }
    }
}
