using CMSSolutions.GTools.Common.Data;
using CMSSolutions.GTools.Common.Models;

namespace CMSSolutions.GTools.Common
{
    public interface IDataProviderPlugin
    {
        string ProviderName { get; }

        IConnectionControl ConnectionControl { get; }

        BaseProvider GetDataProvider(ConnectionDetails connectionDetails);

        ISettingsControl SettingsControl { get; }
    }
}