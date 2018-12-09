using System;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;
using CMSSolutions.Data;
using CMSSolutions.Data.Common;
using CMSSolutions.Data.Sql;
using CMSSolutions.Data.SqlClient;
using CMSSolutions.GTools.Common;
using System.Linq;
using CMSSolutions.Extensions;
using CMSSolutions.Collections;
using CMSSolutions.GTools.Common.Diagnostics;
using CMSSolutions.GTools.Common.Models;
using CMSSolutions.GTools.Common.Extensions;

namespace CMSSolutions.GTools.Sql
{
    public partial class SqlConnectionControl : UserControl, IConnectionControl
    {
        private const string SQL_CONNECTION_STRING_FORMAT = "Data Source={0};Initial Catalog={1};User={2};Password={3}";
        private const string SQL_CONNECTION_STRING_FORMAT_WA = "Data Source={0};Initial Catalog={1};Integrated Security=true";

        #region Public Properties

        public string Server
        {
            get
            {
                if (cmbServer.SelectedIndex != -1)
                {
                    return cmbServer.SelectedItem.ToString();
                }
                else if (!cmbServer.Text.IsNullOrWhiteSpace())
                {
                    return cmbServer.Text;
                }
                return string.Empty;
            }
            set { cmbServer.Text = value; }
        }

        public string Database
        {
            get
            {
                if (cmbDatabase.SelectedIndex != -1)
                {
                    return cmbDatabase.SelectedItem.ToString();
                }
                else if (!cmbDatabase.Text.IsNullOrWhiteSpace())
                {
                    return cmbDatabase.Text;
                }
                return "master";
            }
            set { cmbDatabase.Text = value; }
        }

        public string UserName
        {
            get { return txtUserName.Text.Trim(); }
            set { txtUserName.Text = value; }
        }

        public string Password
        {
            get { return txtPassword.Text.Trim(); }
            set { txtPassword.Text = value; }
        }

        public bool IntegratedSecurity
        {
            get { return cbIntegratedSecurity.Checked; }
            set { cbIntegratedSecurity.Checked = value; }
        }

        public string ConnectionString
        {
            get
            {
                if (this.IsInWinDesignMode())
                {
                    return string.Empty;
                }

                #region Checks

                if (Server.IsNullOrWhiteSpace())
                {
                    TraceService.Instance.WriteMessage(TraceEvent.Error, "Server is invalid. Please try again.");
                    return string.Empty;
                }
                if (Database.IsNullOrWhiteSpace())
                {
                    TraceService.Instance.WriteMessage(TraceEvent.Error, "Database is invalid. Please try again.");
                    return string.Empty;
                }
                if (!IntegratedSecurity)
                {
                    if (UserName.IsNullOrWhiteSpace())
                    {
                        TraceService.Instance.WriteMessage(TraceEvent.Error, "User Name is invalid. Please try again.");
                        return string.Empty;
                    }
                }

                #endregion

                if (IntegratedSecurity)
                {
                    return string.Format(SQL_CONNECTION_STRING_FORMAT_WA, Server, Database);
                }
                else
                {
                    return string.Format(SQL_CONNECTION_STRING_FORMAT, Server, Database, UserName, Password);
                }
            }
        }

        #endregion

        #region IConnectionControl Members

        public UserControl Content
        {
            get { return this; }
        }

        public ConnectionDetails ConnectionDetails
        {
            get
            {
                //bool isValid = false;
                //using (SqlConnection connection = new SqlConnection(ConnectionString))
                //{
                //    isValid = connection.Validate();
                //}

                return new ConnectionDetails
                {
                    Database = this.Database,
                    IntegratedSecurity = this.IntegratedSecurity,
                    Password = this.Password,
                    Server = this.Server,
                    UserName = this.UserName,
                    ProviderName = Constants.SqlProviderName,
                    ConnectionString = this.ConnectionString
                };
            }
            set
            {
                Server = value.Server;
                IntegratedSecurity = value.IntegratedSecurity;
                Password = value.Password;
                UserName = value.UserName;
                Database = value.Database;
            }
        }

        public bool ValidateConnection()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                return connection.Validate();
            }
        }

        #endregion

        public SqlConnectionControl()
        {
            InitializeComponent();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cmbServer.Items.Clear();
            cmbServer.Items.AddRange(SqlDataSourceEnumerator.Instance.GetAvailableSqlServers().ToArray());
        }

        private void cbIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            txtUserName.Enabled = !IntegratedSecurity;
            txtPassword.Enabled = !IntegratedSecurity;
        }

        private void cmbDatabase_DropDown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Server) && string.IsNullOrEmpty(cmbDatabase.Text))
            {
                cmbDatabase.Items.Clear();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    cmbDatabase.Items.AddRange(connection.GetDatabaseNames().ToArray());
                }
            }
        }

        private void cmbServer_DropDown(object sender, EventArgs e)
        {
            if (cmbServer.Items.Count == 0)
            {
                cmbServer.Items.AddRange(SqlDataSourceEnumerator.Instance.GetAvailableSqlServers().ToArray());
            }
        }

        private void cmbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbDatabase.Items.Clear();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                cmbDatabase.Items.AddRange(connection.GetDatabaseNames().ToArray());
            }
        }
    }
}
