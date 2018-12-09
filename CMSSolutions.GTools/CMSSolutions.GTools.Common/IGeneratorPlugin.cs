using System.Windows.Forms;
using CMSSolutions.GTools.Common.Configuration;
using CMSSolutions.GTools.Common.Data;

namespace CMSSolutions.GTools.Common
{
    public interface IGeneratorPlugin
    {
        string Name { get; }

        IGeneratorSettings Settings { get; }

        void Run(BaseProvider dataProvider, ConfigFile config);
    }
}