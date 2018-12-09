using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CMSSolutions.GTools.Common;
using CMSSolutions.GTools.Common.Configuration;
using CMSSolutions.GTools.Common.Extensions;
using CMSSolutions.GTools.Common.Wizard;

namespace CMSSolutions.GTools.Views
{
    public partial class MainForm : Form
    {
        private TraceViewerControl traceViewer = new TraceViewerControl();
        private WizardHost wizardHost = new WizardHost();

        [ImportMany(typeof(IDataProviderPlugin))]
        private IEnumerable<IDataProviderPlugin> dataProviderPlugins = null;

        [ImportMany(typeof(IGeneratorPlugin))]
        private IEnumerable<IGeneratorPlugin> generatorPlugins = null;

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            using (var aggregateCatalog = new AggregateCatalog(
                new AssemblyCatalog(typeof(Program).Assembly),
                new DirectoryCatalog(Path.Combine(Application.StartupPath, "Plugins"))))
            {
                using (CompositionContainer container = new CompositionContainer(aggregateCatalog))
                {
                    container.ComposeParts(this);
                }
            }

            Program.Configuration = new ConfigFile();
            Program.DataProviderPlugins = this.dataProviderPlugins;
            Program.GeneratorPlugins = this.generatorPlugins;
            
            this.dataProviderPlugins = null;
            this.generatorPlugins = null;

            wizardHost.WizardPages.Add(1, new ConnectionsControl());
            wizardHost.WizardPages.Add(2, new TableSelectorControl());
            wizardHost.WizardPages.Add(3, new GeneratorSettingsControl());
            wizardHost.LoadWizard();
            wizardHost.WizardCompleted += wizardHost_WizardCompleted;

            ShowWizard();
        }

        void wizardHost_WizardCompleted()
        {
            Controller.RunJob();
            MessageBox.Show("Done");
        }

        #endregion

        #region Private Methods

        private DialogResult CheckSaveChanges()
        {
            var dialogResult = MessageBox.Show(
                "Do you want to save the current file?",
                "Save Changes?",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.OK)
            {
                if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                {
                    Program.Configuration.SaveAs(dlgSaveFile.FileName);
                }
            }

            return dialogResult;
        }

        public void ShowWizard()
        {
            panelMain.Controls.Clear();
            panelMain.Controls.Add(wizardHost);
            wizardHost.Dock = DockStyle.Fill;
            mnuMainToolsShowTraceViewer.Checked = false;
        }

        public void ShowTraceViewer()
        {
            panelMain.Controls.Clear();
            panelMain.Controls.Add(traceViewer);
            traceViewer.Dock = DockStyle.Fill;
            mnuMainToolsShowTraceViewer.Checked = true;
        }

        private void NewFile()
        {
            CheckSaveChanges();
            Program.Configuration = new ConfigFile();
            ShowWizard();
        }

        private void OpenFile()
        {
            var dialogResult = CheckSaveChanges();
            if (dialogResult == DialogResult.Cancel)
            { return; }

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                Program.Configuration = ConfigFile.Load(dlgOpenFile.FileName);
            }
        }

        private void SaveFile()
        {
            wizardHost.WizardPages.CurrentPage.Save();
            panelMain.Controls.Clear();
            Program.Configuration.Save();
        }

        #endregion

        #region Control Event Handlers

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        #region Main Menu

        private void mnuMainFileNew_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void mnuMainFileOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void mnuMainFileSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void mnuMainFileSaveAs_Click(object sender, EventArgs e)
        {
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                wizardHost.WizardPages.CurrentPage.Save();
                panelMain.Controls.Clear();
                Program.Configuration.SaveAs(dlgSaveFile.FileName);
            }
        }

        private void mnuMainFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuMainToolsShowTraceViewer_Click(object sender, EventArgs e)
        {
            if (mnuMainToolsShowTraceViewer.Checked)
            {
                ShowWizard();
            }
            else
            {
                ShowTraceViewer();
            }
        }

        private void mnuMainToolsOptions_Click(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog();
        }

        private void mnuMainHelpAbout_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        #endregion

        #endregion
    }
}