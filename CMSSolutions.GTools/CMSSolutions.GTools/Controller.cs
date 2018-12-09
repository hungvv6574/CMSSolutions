using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using CMSSolutions.GTools.Common;
using CMSSolutions.GTools.Common.Data;
using CMSSolutions.GTools.Common.Models;

namespace CMSSolutions.GTools
{
    public static class Controller
    {
        public static IConnectionControl GetConnectionControl(string providerName)
        {
            return Program.DataProviderPlugins.Single(p => p.ProviderName == providerName).ConnectionControl;
        }

        public static BaseProvider GetDataProvider(ConnectionDetails connection)
        {
            return Program.DataProviderPlugins.Single(p => p.ProviderName == connection.ProviderName).GetDataProvider(connection);
        }

        public static IGeneratorSettings GetGeneratorSettingsControl(string name)
        {
            return Program.GeneratorPlugins.Single(p => p.Name == name).Settings;
        }

        public static void RunJob()
        {
            var provider = GetDataProvider(Program.Configuration.Connection);
            var generator = Program.GeneratorPlugins.First(x => x.Name == Program.Configuration.SelectedGenerator);
            generator.Run(provider, Program.Configuration);
        }
    }
}