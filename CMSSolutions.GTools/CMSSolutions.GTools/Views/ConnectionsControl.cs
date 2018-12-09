using System.Linq;
using System.Windows.Forms;
using CMSSolutions.GTools.Common;
using CMSSolutions.GTools.Common.Models;
using CMSSolutions.GTools.Common.Wizard;

namespace CMSSolutions.GTools.Views
{
    public partial class ConnectionsControl : UserControl, IWizardPage
    {
        private IConnectionControl connectionControl = null;

        public ConnectionDetails Connection
        {
            get
            {
                if (connectionControl == null)
                { return null; }
                return connectionControl.ConnectionDetails;
            }
        }

        public string ConnectionType
        {
            get
            {
                if (cmbConnectionType.SelectedIndex != -1)
                {
                    return cmbConnectionType.SelectedItem.ToString();
                }
                return string.Empty;
            }
            set { cmbConnectionType.SelectedItem = value.ToString(); }
        }

        public ConnectionsControl()
        {
            InitializeComponent();

            var providerNames = Program.DataProviderPlugins.Select(p => p.ProviderName).OrderBy(p => p);
            cmbConnectionType.Items.AddRange(providerNames.ToArray());

            if (Program.Configuration.Connection != null)
            {
                var plugin = Program.DataProviderPlugins.SingleOrDefault(p => p.ProviderName == Program.Configuration.Connection.ProviderName);
                this.connectionControl = plugin.ConnectionControl;
                connectionControl.ConnectionDetails = Program.Configuration.Connection;
                LoadConnectionControl();
                cmbConnectionType.SelectedIndexChanged -= new System.EventHandler(this.cmbConnectionType_SelectedIndexChanged);
                cmbConnectionType.Text = plugin.ProviderName;
                cmbConnectionType.SelectedIndexChanged += new System.EventHandler(this.cmbConnectionType_SelectedIndexChanged);
            }
        }

        private void LoadConnectionControl()
        {
            if (connectionControl != null)
            {
                pnlConnection.Controls.Clear();
                UserControl content = connectionControl.Content;
                pnlConnection.Controls.Add(content);
                content.Dock = DockStyle.Fill;
            }
        }

        private void cmbConnectionType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            connectionControl = Controller.GetConnectionControl(ConnectionType);
            LoadConnectionControl();
        }

        private void btnValidateConnection_Click(object sender, System.EventArgs e)
        {
            if (connectionControl != null)
            {
                bool isValid = connectionControl.ValidateConnection();
                if (isValid)
                {
                    MessageBox.Show("Successfully connected", "Successful Connection",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Could not connect", "Unsuccessful Connection",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Connection Not Specified", "No Connection",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public UserControl Content
        {
            get { return this; }
        }

        public new void Load()
        {
        }

        public void Save()
        {
            Program.Configuration.Connection = this.Connection;
        }

        public bool PageValid
        {
            get
            {
                if (connectionControl == null)
                {
                    return false;
                }
                return connectionControl.ValidateConnection();
            }
        }

        public string ValidationMessage
        {
            get { return "Could not establish a connection. Please check and try again."; }
        }
    }
}