﻿using System;
using System.Linq;
using System.Windows.Forms;
using CMSSolutions.GTools.Common;
using CMSSolutions.Collections;

namespace CMSSolutions.GTools.Views
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            //settingsTreeView.AddSettingsNode("General", new DefaultSettingsControl());

            Program.DataProviderPlugins.OrderBy(p => p.ProviderName).ForEach(plugin =>
            {
                settingsTreeView.AddSettingsNode(plugin.ProviderName, plugin.SettingsControl);
            });
        }

        private void SaveCurrentControl()
        {
            if (contentPanel.HasChildren)
            {
                ISettingsControl currentControl = contentPanel.Controls[0] as ISettingsControl;
                if (currentControl != null)
                {
                    currentControl.Save();
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveCurrentControl();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SaveCurrentControl();
            this.Close();
        }

        private void settingsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SaveCurrentControl();

            if (e.Node.Tag == null)
            { return; }

            ISettingsControl settingsControl = e.Node.Tag as ISettingsControl;
            UserControl control = settingsControl.ControlContent;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(control);
            control.Dock = DockStyle.Fill;
        }
    }
}
