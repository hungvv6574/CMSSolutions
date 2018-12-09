using System.ComponentModel.Composition;
using CMSSolutions.GTools.Common;
using CMSSolutions.GTools.Common.Data;
using CMSSolutions.GTools.Common.Models;

namespace CMSSolutions.GTools.Sql
{
    [Export(typeof(IDataProviderPlugin))]
    public class SqlServerDataProviderPlugin : IDataProviderPlugin
    {
        #region IScaffolderPlugin Members

        public string ProviderName
        {
            get { return Constants.SqlProviderName; }
        }

        public IConnectionControl ConnectionControl
        {
            get { return new SqlConnectionControl(); }
        }

        public BaseProvider GetDataProvider(ConnectionDetails connectionDetails)
        {
            return new SqlProvider(connectionDetails);
        }

        public ISettingsControl SettingsControl
        {
            get { return null; }
        }

        #endregion
    }
}