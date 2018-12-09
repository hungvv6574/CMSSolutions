using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMSSolutions.GTools.Common;
using CMSSolutions.GTools.Common.Configuration;
using CMSSolutions.GTools.Views;

namespace CMSSolutions.GTools
{
    static class Program
    {
        public static IEnumerable<IDataProviderPlugin> DataProviderPlugins { get; set; }

        public static IEnumerable<IGeneratorPlugin> GeneratorPlugins { get; set; }

        public static ConfigFile Configuration { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
