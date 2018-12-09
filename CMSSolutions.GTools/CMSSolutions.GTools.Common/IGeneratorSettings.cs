using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMSSolutions.GTools.Common
{
    public interface IGeneratorSettings
    {
        UserControl Settings { get; }

        bool ValidateSettings();
    }
}
