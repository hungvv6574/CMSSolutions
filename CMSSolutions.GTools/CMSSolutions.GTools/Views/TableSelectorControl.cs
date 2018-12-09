using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using CMSSolutions.GTools.Common.Data;
using CMSSolutions.GTools.Common.Wizard;

namespace CMSSolutions.GTools.Views
{
    public partial class TableSelectorControl : UserControl, IWizardPage
    {
        private BaseProvider dataProvider;

        private List<TableSelection> selectedTables;

        public TableSelectorControl()
        {
            InitializeComponent();
        }

        public UserControl Content
        {
            get { return this; }
        }

        public new void Load()
        {
            dataProvider = Program.DataProviderPlugins
                .First(x => x.ProviderName == Program.Configuration.Connection.ProviderName)
                .GetDataProvider(Program.Configuration.Connection);

            selectedTables = new List<TableSelection>();
            selectedTables.AddRange(dataProvider.TableNames.OrderBy(x => x).Select(x => new TableSelection { TableName = x, IsSelected = false }));
            dgvAvailableTables.DataSource = selectedTables;
        }

        public void Save()
        {
            Program.Configuration.SelectedTables = selectedTables.Where(x => x.IsSelected).Select(x => x.TableName).ToList();
        }

        public bool PageValid
        {
            get { return selectedTables.Where(x => x.IsSelected).Count() > 0; }
        }

        public string ValidationMessage
        {
            get { return "You haven't selected any tables."; }
        }

        private class TableSelection
        {
            [DisplayName("Table")]
            public string TableName { get; set; }

            [DisplayName("Select")]
            public bool IsSelected { get; set; }
        }

        private void cbSelectAll_CheckedChanged(object sender, System.EventArgs e)
        {
            selectedTables.ForEach(x => { x.IsSelected = cbSelectAll.Checked; });
            dgvAvailableTables.Invalidate();
        }
    }
}