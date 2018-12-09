using System;
using System.Linq;
using System.Windows.Forms;
using CMSSolutions.GTools.Common;
using CMSSolutions.GTools.Common.Wizard;

namespace CMSSolutions.GTools.Views
{
    public partial class GeneratorSettingsControl : UserControl, IWizardPage
    {
        private IGeneratorSettings settingsControl;

        public string SelectedGenerator
        {
            get
            {
                if (cmbGenerator.SelectedIndex != -1)
                {
                    return cmbGenerator.SelectedItem.ToString();
                }
                return string.Empty;
            }
            set { cmbGenerator.SelectedItem = value.ToString(); }
        }

        public GeneratorSettingsControl()
        {
            InitializeComponent();
        }

        private void LoadSettingsControl()
        {
            if (settingsControl != null)
            {
                pnlSettings.Controls.Clear();
                pnlSettings.Controls.Add(settingsControl.Settings);
                settingsControl.Settings.Dock = DockStyle.Fill;
            }
        }

        private void cmbGenerator_SelectedIndexChanged(object sender, EventArgs e)
        {
            settingsControl = Controller.GetGeneratorSettingsControl(SelectedGenerator);

            if (settingsControl != null)
            {
                LoadSettingsControl();
            }
            else
            {
                MessageBox.Show("This Generator has no settings to configure. Click Finish to continue.");
            }
        }

        public UserControl Content
        {
            get { return this; }
        }

        public new void Load()
        {
            var providerNames = Program.GeneratorPlugins.Select(p => p.Name).OrderBy(p => p);
            cmbGenerator.Items.AddRange(providerNames.ToArray());
            
            //If only 1 choice..
            if (cmbGenerator.Items.Count == 1)
            {
                // then select it
                cmbGenerator.SelectedIndex = 0;
            }
        }

        public void Save()
        {
            Program.Configuration.SelectedGenerator = SelectedGenerator;

            //TODO: Save configuration to xml file.
            //throw new NotImplementedException();
        }

        public bool PageValid
        {
            get
            {
                if (settingsControl == null)
                {
                    return false;
                }
                return settingsControl.ValidateSettings();
            }
        }

        public string ValidationMessage
        {
            get
            {
                if (cmbGenerator.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a code generator");
                }
                return null;
            }
        }
    }
}